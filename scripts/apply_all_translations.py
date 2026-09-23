#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
apply_all_translations.py
-------------------------
Valida e reintegra in modo atomico le traduzioni generate dai subagenti o LLM
(output_batch_*.json) all'interno di QUALSIASI file JSON master di FFXIV.

Supporta:
- Schemi standard original/translation
- Schemi con campo dedicato specificato in manifest (es. target_field="col_2" -> translation_col_2)
- Schemi multi-campo a dizionario
- Validazione SeString 1:1 rigorosa su ogni stringa prima di toccare il file master.
"""

import argparse
import glob
import json
import os
import re
import sys
from typing import Dict, List, Tuple, Any

# Compatibilità encoding per console Windows
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
if hasattr(sys.stderr, "reconfigure"):
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

HEX_REGEX = re.compile(r'<hex:[0-9A-Fa-f]+>')
NAMED_TAG_REGEX = re.compile(r'<(?:/?(?:FullName|Forename|Surname|br|Highlight|Clickable|string\([^)]*\)|colortype\([^)]*\)|edgecolortype\([^)]*\)|If\([^)]*\)|Sheet\([^)]*\)|Value\([^)]*\)|Split\([^)]*\)))>')

def extract_hex_tags(text: str) -> List[str]:
    return HEX_REGEX.findall(str(text))

def extract_named_tags(text: str) -> List[str]:
    return NAMED_TAG_REGEX.findall(str(text))

def validate_entry(key: str, field_name: str, orig: str, trans: str) -> List[str]:
    """Restituisce una lista di errori riscontrati nella traduzione rispetto all'originale."""
    errors = []

    # 1. Integrità assoluta dei tag SeString <hex:...>
    orig_hex = extract_hex_tags(orig)
    trans_hex = extract_hex_tags(trans)
    if orig_hex != trans_hex:
        errors.append(
            f"[ID {key}, campo {field_name}] Mismatch tag <hex:...>:\n"
            f"  Originale ({len(orig_hex)} tag): {orig_hex}\n"
            f"  Traduzione ({len(trans_hex)} tag): {trans_hex}"
        )

    # 2. Tag speciali nominati (<FullName>, <br>, ecc.)
    orig_named = extract_named_tags(orig)
    trans_named = extract_named_tags(trans)
    if sorted(orig_named) != sorted(trans_named):
        errors.append(
            f"[ID {key}, campo {field_name}] Mismatch tag nominati SeString:\n"
            f"  Originale: {orig_named}\n"
            f"  Traduzione: {trans_named}"
        )

    # 3. Bilanciamento parentesi angolari
    # Verifica che il differenziale di parentesi angolari corrisponda a quello dell'originale
    # (alcuni comandi usano '>' nel testo come '>>Sottocomandi:')
    orig_diff = str(orig).count('<') - str(orig).count('>')
    trans_diff = str(trans).count('<') - str(trans).count('>')
    if orig_diff != trans_diff:
        errors.append(
            f"[ID {key}, campo {field_name}] Differenziale parentesi angolari non corrispondente "
            f"(orig: <: {str(orig).count('<')}, >: {str(orig).count('>')}; trans: <: {str(trans).count('<')}, >: {str(trans).count('>')}) in:\n"
            f"  '{trans}'"
        )

    return errors

def check_bilingual_warning(key: str, field_name: str, orig: str, trans: str) -> List[str]:
    """Rileva sospetti residui di bilinguismo a schermo tipo 'Italiano (Inglese)'."""
    warnings = []
    parentheses_content = re.findall(r'\(([^)]+)\)', str(trans))
    for p in parentheses_content:
        # Se il contenuto tra parentesi è una sottostringa di 3+ caratteri presente nell'originale
        if len(p) >= 3 and p in str(orig):
            # Esclude annotazioni comuni come (PvP), (sottocomando), (opzionale)
            if p.lower() in ("pvp", "sottocomando", "opzionale", "combo", "on", "off"):
                continue
            warnings.append(
                f"[ID {key}, campo {field_name}] Possibile testo bilingue a schermo rilevato tra parentesi '({p})' in:\n"
                f"  Traduzione: {trans}"
            )
    return warnings

def load_batches(batch_dir: str, pattern: str) -> Tuple[Dict[str, Any], List[str]]:
    search_path = os.path.join(batch_dir, pattern)
    batch_files = sorted(glob.glob(search_path))
    all_translations: Dict[str, Any] = {}

    if not batch_files:
        return all_translations, []

    for fpath in batch_files:
        with open(fpath, "r", encoding="utf-8") as f:
            try:
                batch_data = json.load(f)
            except Exception as e:
                print(f"ERRORE nel parsing JSON di {fpath}: {e}", file=sys.stderr)
                sys.exit(1)

            if isinstance(batch_data, dict):
                for k, v in batch_data.items():
                    all_translations[str(k)] = v
            else:
                print(f"ERRORE: Struttura imprevista nel file batch {fpath}", file=sys.stderr)
                sys.exit(1)

    return all_translations, batch_files

def resolve_field_mapping(master_entry: dict, target_field: str = None) -> List[Tuple[str, str]]:
    """Ritorna l'elenco di (campo_originale, campo_destinazione_traduzione)."""
    if target_field:
        tk = f"translation_{target_field}" if f"translation_{target_field}" in master_entry else "translation"
        return [(target_field, tk)]

    if "original" in master_entry and "translation" in master_entry:
        return [("original", "translation")]

    pairs = []
    for k in master_entry.keys():
        if not k.startswith("translation_") and k not in ("tag", "translation"):
            tk = f"translation_{k}"
            if tk in master_entry:
                pairs.append((k, tk))
            elif "translation" in master_entry and "original" not in master_entry:
                pairs.append((k, "translation"))
    return pairs

def apply_translations(master_file: str, batch_dir: str = "scratch", pattern: str = "output_batch_*.json",
                       field: str = None, validate_only: bool = False, strict: bool = False) -> int:
    manifest_target_field = None
    manifest_path = os.path.join(batch_dir, "batch_manifest.json")
    if os.path.exists(manifest_path):
        try:
            with open(manifest_path, "r", encoding="utf-8") as mf:
                mdata = json.load(mf)
                suggested = mdata.get("master_file")
                if (not master_file or not os.path.exists(master_file)) and suggested and os.path.exists(suggested):
                    print(f"File master ricavato dal manifest: {suggested}")
                    master_file = suggested
                manifest_target_field = mdata.get("target_field")
        except Exception:
            pass

    target_field = field or manifest_target_field

    if not master_file or not os.path.exists(master_file):
        print(f"ERRORE: File master non trovato: {master_file}", file=sys.stderr)
        return 1

    all_translations, batch_files = load_batches(batch_dir, pattern)
    if not batch_files:
        print(f"Nessun file di output trovato corrispondente a '{pattern}' in '{batch_dir}'.", file=sys.stderr)
        return 1

    print(f"Caricati {len(batch_files)} file batch ({len(all_translations)} voci totali).")
    if target_field:
        print(f"Campo target configurato: {target_field}")

    with open(master_file, "r", encoding="utf-8") as f:
        try:
            master = json.load(f)
        except Exception as e:
            print(f"ERRORE nel parsing JSON del master {master_file}: {e}", file=sys.stderr)
            return 1

    # Fase di Validazione Rigorosa
    print("\n[1/2] Avvio validazione rigorosa SeString 1:1...")
    errors: List[str] = []
    warnings: List[str] = []

    for key, trans_item in all_translations.items():
        if key not in master:
            errors.append(f"[ID {key}] Chiave presente nel batch ma assente nel master {master_file}!")
            continue

        master_entry = master[key]
        if not isinstance(master_entry, dict):
            errors.append(f"[ID {key}] Voce master non valida (atteso dict).")
            continue

        mappings = resolve_field_mapping(master_entry, target_field)

        # Se il batch contiene direttamente una stringa:
        if isinstance(trans_item, str):
            if mappings:
                orig_col, trans_col = mappings[0]
                orig_val = master_entry.get(orig_col, "")
                errors.extend(validate_entry(key, orig_col, orig_val, trans_item))
                warnings.extend(check_bilingual_warning(key, orig_col, orig_val, trans_item))
        elif isinstance(trans_item, dict):
            # Se il batch è un dizionario campo: valore
            for col_k, col_trans in trans_item.items():
                orig_col = col_k.replace("translation_", "")
                orig_val = master_entry.get(orig_col, "")
                errors.extend(validate_entry(key, orig_col, orig_val, col_trans))
                warnings.extend(check_bilingual_warning(key, orig_col, orig_val, col_trans))

    if warnings:
        print(f"\n--- ATTENZIONE: Rilevati {len(warnings)} possibili problemi di bilinguismo ---")
        for w in warnings[:15]:
            print(f"  {w}")
        if len(warnings) > 15:
            print(f"  ... e altri {len(warnings) - 15} avvisi.")

    if errors:
        print(f"\n--- ERRORE FATALE: Rilevati {len(errors)} errori di validazione SeString! ---", file=sys.stderr)
        for err in errors[:25]:
            print(f"  {err}", file=sys.stderr)
        if len(errors) > 25:
            print(f"  ... e altri {len(errors) - 25} errori omessi.", file=sys.stderr)
        print("\nReintegrazione interrotta: il master JSON NON è stato modificato.", file=sys.stderr)
        return 1

    if strict and warnings:
        print("\nModalità STRICT attiva: interruzione per presenza di avvisi bilinguismo.", file=sys.stderr)
        return 1

    print("[OK] Validazione completata: 0 errori SeString riscontrati.")

    if validate_only:
        print("Modalità --validate-only: nessuna scrittura effettuata.")
        return 0

    # Fase di Aggiornamento Atomico
    print("\n[2/2] Reintegrazione atomica nel master JSON...")
    applied_count = 0
    for key, trans_item in all_translations.items():
        if key in master and isinstance(master[key], dict):
            mappings = resolve_field_mapping(master[key], target_field)
            if isinstance(trans_item, str):
                if mappings:
                    orig_col, trans_col = mappings[0]
                    master[key][trans_col] = trans_item
                    applied_count += 1
            elif isinstance(trans_item, dict):
                for col_k, col_trans in trans_item.items():
                    trans_col = col_k if col_k.startswith("translation_") else f"translation_{col_k}"
                    if trans_col not in master[key] and "translation" in master[key]:
                        trans_col = "translation"
                    master[key][trans_col] = col_trans
                    applied_count += 1

    # Scrittura atomica sicura con file temporaneo
    tmp_path = f"{master_file}.tmp"
    with open(tmp_path, "w", encoding="utf-8") as f:
        json.dump(master, f, ensure_ascii=False, indent=2)

    os.replace(tmp_path, master_file)

    print(f"[OK] Reintegrazione completata con successo!")
    print(f"  - Traduzioni applicate: {applied_count}")
    print(f"  - File aggiornato: {master_file}")
    return 0

def main():
    parser = argparse.ArgumentParser(description="Valida e applica le traduzioni dei batch a qualsiasi file master JSON.")
    parser.add_argument("master_file", nargs="?", default="", help="Percorso del master JSON (es. data/translations/system/addon.json o textcommand.json)")
    parser.add_argument("--batch-dir", "-d", default="scratch", help="Cartella contenente i file di output tradotti (default: scratch)")
    parser.add_argument("--pattern", "-p", default="output_batch_*.json", help="Pattern dei file tradotti (default: output_batch_*.json)")
    parser.add_argument("--field", "-f", default=None, help="Specifica il campo di destinazione (se omesso viene letto dal manifest o dedotto dallo schema)")
    parser.add_argument("--validate-only", "-v", action="store_true", help="Valida i tag SeString senza modificare il master file")
    parser.add_argument("--strict", "-s", action="store_true", help="Blocca la reintegrazione anche se vengono rilevati avvisi di bilinguismo")

    args = parser.parse_args()
    sys.exit(apply_translations(args.master_file, args.batch_dir, args.pattern, args.field, args.validate_only, args.strict))

if __name__ == "__main__":
    main()
