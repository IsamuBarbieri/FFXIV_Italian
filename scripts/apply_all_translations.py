#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
apply_all_translations.py
-------------------------
Valida e reintegra in modo atomico le traduzioni generate dai subagenti o LLM
(output_batch_*.json) all'interno del file JSON master di FFXIV.
Esegue una verifica SeString 1:1 rigorosa su ogni riga prima di qualsiasi modifica.
"""

import argparse
import glob
import json
import os
import re
import sys
from typing import Dict, List, Tuple

# Compatibilità encoding per console Windows
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
if hasattr(sys.stderr, "reconfigure"):
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

HEX_REGEX = re.compile(r'<hex:[0-9A-Fa-f]+>')
NAMED_TAG_REGEX = re.compile(r'<(?:/?(?:FullName|Forename|Surname|br|Highlight|Clickable|string\([^)]*\)|colortype\([^)]*\)|edgecolortype\([^)]*\)|If\([^)]*\)|Sheet\([^)]*\)|Value\([^)]*\)|Split\([^)]*\)))>')

def extract_hex_tags(text: str) -> List[str]:
    return HEX_REGEX.findall(text)

def extract_named_tags(text: str) -> List[str]:
    return NAMED_TAG_REGEX.findall(text)

def validate_entry(key: str, orig: str, trans: str) -> List[str]:
    """Restituisce una lista di errori riscontrati nella traduzione rispetto all'originale."""
    errors = []

    # 1. Integrità assoluta dei tag SeString <hex:...>
    orig_hex = extract_hex_tags(orig)
    trans_hex = extract_hex_tags(trans)
    if orig_hex != trans_hex:
        errors.append(
            f"[ID {key}] Mismatch tag <hex:...>:\n"
            f"  Originale ({len(orig_hex)} tag): {orig_hex}\n"
            f"  Traduzione ({len(trans_hex)} tag): {trans_hex}"
        )

    # 2. Tag speciali nominati (<FullName>, <br>, ecc.)
    orig_named = extract_named_tags(orig)
    trans_named = extract_named_tags(trans)
    if sorted(orig_named) != sorted(trans_named):
        errors.append(
            f"[ID {key}] Mismatch tag nominati SeString:\n"
            f"  Originale: {orig_named}\n"
            f"  Traduzione: {trans_named}"
        )

    # 3. Bilanciamento parentesi angolari
    if trans.count('<') != trans.count('>'):
        errors.append(
            f"[ID {key}] Tag sbilanciati (<: {trans.count('<')}, >: {trans.count('>')}) in:\n"
            f"  '{trans}'"
        )

    return errors

def check_bilingual_warning(key: str, orig: str, trans: str) -> List[str]:
    """Rileva sospetti residui di bilinguismo a schermo tipo 'Italiano (Inglese)'."""
    warnings = []
    parentheses_content = re.findall(r'\(([^)]+)\)', trans)
    for p in parentheses_content:
        # Se il contenuto tra parentesi è una sottostringa di 3+ caratteri presente nell'originale
        if len(p) >= 3 and p in orig:
            warnings.append(
                f"[ID {key}] Possibile testo bilingue a schermo rilevato tra parentesi '({p})' in:\n"
                f"  Traduzione: {trans}"
            )
    return warnings

def load_batches(batch_dir: str, pattern: str) -> Tuple[Dict[str, str], List[str]]:
    search_path = os.path.join(batch_dir, pattern)
    batch_files = sorted(glob.glob(search_path))
    all_translations: Dict[str, str] = {}

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
                    if isinstance(v, dict):
                        # Caso in cui il batch mantenga struttura { "original": ..., "translation": ... }
                        trans_val = v.get("translation", "")
                    else:
                        trans_val = str(v)
                    all_translations[str(k)] = trans_val
            else:
                print(f"ERRORE: Struttura imprevista nel file batch {fpath}", file=sys.stderr)
                sys.exit(1)

    return all_translations, batch_files

def apply_translations(master_file: str, batch_dir: str = "scratch", pattern: str = "output_batch_*.json",
                       validate_only: bool = False, strict: bool = False) -> int:
    # Se master_file non esiste ma esiste batch_manifest.json nella cartella batch, proviamo a ricavarlo
    if not os.path.exists(master_file):
        manifest_path = os.path.join(batch_dir, "batch_manifest.json")
        if os.path.exists(manifest_path):
            with open(manifest_path, "r", encoding="utf-8") as mf:
                mdata = json.load(mf)
                suggested = mdata.get("master_file")
                if suggested and os.path.exists(suggested):
                    print(f"File master ricavato dal manifest: {suggested}")
                    master_file = suggested

    if not os.path.exists(master_file):
        print(f"ERRORE: File master non trovato: {master_file}", file=sys.stderr)
        return 1

    all_translations, batch_files = load_batches(batch_dir, pattern)
    if not batch_files:
        print(f"Nessun file di output trovato corrispondente a '{pattern}' in '{batch_dir}'.", file=sys.stderr)
        return 1

    print(f"Caricati {len(batch_files)} file batch ({len(all_translations)} traduzioni totali).")

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

    for key, trans in all_translations.items():
        if key not in master:
            errors.append(f"[ID {key}] Chiave presente nel batch ma assente nel master {master_file}!")
            continue

        master_entry = master[key]
        if not isinstance(master_entry, dict):
            errors.append(f"[ID {key}] Voce master non valida (atteso dict).")
            continue

        orig = master_entry.get("original", "")
        entry_errors = validate_entry(key, orig, trans)
        errors.extend(entry_errors)

        entry_warnings = check_bilingual_warning(key, orig, trans)
        warnings.extend(entry_warnings)

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
    for key, trans in all_translations.items():
        if key in master and isinstance(master[key], dict):
            master[key]["translation"] = trans
            applied_count += 1

    # Scrittura atomica sicura con file temporaneo
    tmp_path = f"{master_file}.tmp"
    with open(tmp_path, "w", encoding="utf-8") as f:
        json.dump(master, f, ensure_ascii=False, indent=2)

    os.replace(tmp_path, master_file)

    # Calcolo statistiche rimanenti
    remaining_untrans = sum(
        1 for v in master.values()
        if isinstance(v, dict) and v.get("original") and (not v.get("translation") or str(v.get("translation")).strip() == "")
    )

    print(f"[OK] Reintegrazione completata con successo!")
    print(f"  - Traduzioni applicate: {applied_count}")
    print(f"  - Righe pendenti rimanenti nel master: {remaining_untrans}")
    print(f"  - File aggiornato: {master_file}")
    return 0

def main():
    parser = argparse.ArgumentParser(description="Valida e applica le traduzioni dei batch al file master JSON.")
    parser.add_argument("master_file", nargs="?", default="", help="Percorso del master JSON (es. data/translations/system/addon.json)")
    parser.add_argument("--batch-dir", "-d", default="scratch", help="Cartella contenente i file di output tradotti (default: scratch)")
    parser.add_argument("--pattern", "-p", default="output_batch_*.json", help="Pattern dei file tradotti (default: output_batch_*.json)")
    parser.add_argument("--validate-only", "-v", action="store_true", help="Valida i tag SeString senza modificare il master file")
    parser.add_argument("--strict", "-s", action="store_true", help="Blocca la reintegrazione anche se vengono rilevati avvisi di bilinguismo")

    args = parser.parse_args()
    sys.exit(apply_translations(args.master_file, args.batch_dir, args.pattern, args.validate_only, args.strict))

if __name__ == "__main__":
    main()
