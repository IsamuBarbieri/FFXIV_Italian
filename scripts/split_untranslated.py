#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
split_untranslated.py
---------------------
Partiziona le righe non ancora tradotte di qualsiasi file JSON master di FFXIV
in batch compatti (es. input_batch_A.json, input_batch_B.json...)
per la traduzione assistita da LLM / subagenti a consumo minimo di token.

Supporta:
- Schemi standard {"id": {"original": "...", "translation": "..."}}
- Schemi multi-colonna {"id": {"col_2": "...", "translation_col_2": "...", "name": ...}}
- Schemi quest/dialogo {"id": {"original": "...", "tag": "...", "translation": "..."}}
- Flag --force-field o --detect-english per individuare testi rimasti in inglese
  anche quando il campo di traduzione non è formalmente vuoto (es. textcommand.json).
"""

import argparse
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

ENGLISH_MARKERS = [
    r'\bUSAGE:', r'\bSubcommands?:', r'\bExecute command\b', r'\bToggle between\b',
    r'\bDisplays? the\b', r'\bSet the\b', r'\bSelect the\b', r'\bWhen no subcommand\b',
    r'\bDuration:\b', r'\bAdditional Effect:\b', r'\bCombo Action:\b', r'\bCombo Potency:\b',
    r'\bDelivers an attack\b', r'\bDeals unaspected damage\b', r'\bIncreases?\b',
    r'\bReduces?\b', r'\bRestores?\b', r'\bGrants?\b', r'\bAfflicts?\b',
    r'\bthe\b', r'\bwith\b', r'\bfrom\b', r'\binto\b', r'\bcannot\b'
]
ENGLISH_REGEX = re.compile('|'.join(ENGLISH_MARKERS), re.IGNORECASE)

def get_batch_label(index: int) -> str:
    """Genera etichette A, B, ... Z, AA, AB... per indice 0-based."""
    label = ""
    index += 1
    while index > 0:
        index, rem = divmod(index - 1, 26)
        label = chr(65 + rem) + label
    return label

def detect_translatable_pairs(entry: dict, target_field: str = None) -> List[Tuple[str, str]]:
    """
    Rileva le coppie (campo_sorgente, campo_traduzione) in una voce del master JSON.
    """
    if target_field:
        if target_field in entry:
            tk = f"translation_{target_field}"
            if tk in entry:
                return [(target_field, tk)]
            if "translation" in entry:
                return [(target_field, "translation")]
        return []

    pairs = []
    # Schema 1: Standard original / translation
    if "original" in entry and "translation" in entry:
        return [("original", "translation")]

    # Schema 2: Campi con prefisso translation_ o colonne multiple
    for k in entry.keys():
        if not k.startswith("translation_") and k not in ("tag", "translation"):
            tk = f"translation_{k}"
            if tk in entry:
                pairs.append((k, tk))
            elif "translation" in entry and "original" not in entry:
                pairs.append((k, "translation"))

    return pairs

def is_untranslated(orig_text: str, trans_text: str, check_english: bool = False) -> bool:
    """Verifica se una stringa è considerata non tradotta."""
    if not orig_text or str(orig_text).strip() == "":
        return False
    if trans_text is None or str(trans_text).strip() == "":
        return True
    if str(trans_text).strip() == str(orig_text).strip():
        return True
    if check_english:
        trans_str = str(trans_text)
        # 1. Indicatori header o frasi inglesi palesi
        if any(h in trans_str for h in ["USAGE:", "Subcommands:", "ALIASES:", ">>Example:"]):
            return True
        # 2. Match regex pattern
        matches = ENGLISH_REGEX.findall(trans_str)
        if len(matches) >= 2:
            return True
        # 3. Analisi parole funzionali inglesi nel corpo della descrizione
        desc_part = re.sub(r'<[^>]+>', ' ', trans_str)
        desc_part = re.sub(r'^(USO|ALIAS|Sottocomandi):', '', desc_part)
        words = re.findall(r'[a-zA-Z]+', desc_part.lower())
        if len(words) > 5:
            eng_w = sum(1 for w in words if w in ['the', 'and', 'to', 'of', 'in', 'is', 'for', 'with', 'on', 'at', 'from', 'by', 'as', 'when', 'if', 'this', 'that', 'all', 'your', 'will', 'be', 'can', 'not', 'only', 'or', 'target', 'party', 'subcommand'])
            if eng_w / len(words) >= 0.15:
                return True
    return False

def split_file(input_file: str, batch_size: int = 200, out_dir: str = "scratch",
               prefix: str = "input_batch_", target_field: str = None,
               detect_english: bool = False) -> int:
    if not os.path.exists(input_file):
        print(f"ERRORE: File master non trovato: {input_file}", file=sys.stderr)
        return 1

    os.makedirs(out_dir, exist_ok=True)

    with open(input_file, "r", encoding="utf-8") as f:
        try:
            data = json.load(f)
        except Exception as e:
            print(f"ERRORE nel parsing JSON di {input_file}: {e}", file=sys.stderr)
            return 1

    if not isinstance(data, dict):
        print("ERRORE: La radice del file JSON deve essere un oggetto/dizionario.", file=sys.stderr)
        return 1

    if not data:
        print("File master vuoto.")
        return 0

    first_entry = next(iter(data.values()))
    if not isinstance(first_entry, dict):
        print("ERRORE: Le voci del dizionario devono essere oggetti JSON.", file=sys.stderr)
        return 1

    sample_pairs = detect_translatable_pairs(first_entry, target_field)
    if not sample_pairs:
        print(f"ERRORE: Nessun campo traducibile individuato nel file (chiavi: {list(first_entry.keys())})", file=sys.stderr)
        return 1

    is_single_original = (len(sample_pairs) == 1 and sample_pairs[0][0] == "original")

    untrans: Dict[str, any] = {}
    for k, v in data.items():
        if not isinstance(v, dict):
            continue
        pairs = detect_translatable_pairs(v, target_field)
        entry_untrans = {}
        for orig_key, trans_key in pairs:
            # Per campi name e description di comandi slash, non consideriamo non tradotto se identico al nome slash
            if orig_key in ("name", "description") and v.get(orig_key, "").startswith("/"):
                continue
            orig_val = v.get(orig_key, "")
            trans_val = v.get(trans_key, "")
            if is_untranslated(orig_val, trans_val, check_english=detect_english):
                entry_untrans[orig_key] = orig_val

        if entry_untrans:
            if is_single_original and "original" in entry_untrans:
                untrans[str(k)] = entry_untrans["original"]
            elif len(entry_untrans) == 1 and target_field and target_field in entry_untrans:
                untrans[str(k)] = entry_untrans[target_field]
            else:
                untrans[str(k)] = entry_untrans

    total_untrans = len(untrans)
    print(f"File master: {input_file}")
    print(f"Campi rilevati per traduzione: {[p[0] for p in sample_pairs]}")
    print(f"Voci totali nel master: {len(data)}")
    print(f"Voci con testo non tradotto individuate: {total_untrans}")

    if total_untrans == 0:
        print("Nessuna riga pendente da tradurre.")
        return 0

    try:
        sorted_keys = sorted(untrans.keys(), key=lambda x: int(x))
    except ValueError:
        sorted_keys = sorted(untrans.keys())

    manifest = {
        "master_file": os.path.abspath(input_file),
        "total_untranslated": total_untrans,
        "batch_size": batch_size,
        "target_field": target_field,
        "is_flat_string": is_single_original or (target_field is not None),
        "batches": []
    }

    batch_idx = 0
    for i in range(0, total_untrans, batch_size):
        chunk_keys = sorted_keys[i : i + batch_size]
        batch_id = get_batch_label(batch_idx)
        chunk_dict = {k: untrans[k] for k in chunk_keys}

        batch_filename = f"{prefix}{batch_id}.json"
        batch_path = os.path.join(out_dir, batch_filename)

        with open(batch_path, "w", encoding="utf-8") as out_f:
            json.dump(chunk_dict, out_f, ensure_ascii=False, indent=2)

        manifest["batches"].append({
            "id": batch_id,
            "filename": batch_filename,
            "count": len(chunk_dict),
            "start_key": chunk_keys[0],
            "end_key": chunk_keys[-1]
        })

        print(f"  Batch {batch_id}: {len(chunk_dict)} voci -> {batch_path}")
        batch_idx += 1

    manifest_path = os.path.join(out_dir, "batch_manifest.json")
    with open(manifest_path, "w", encoding="utf-8") as mf:
        json.dump(manifest, mf, ensure_ascii=False, indent=2)

    print(f"\nGenerati {batch_idx} batch in '{out_dir}/'. Manifest salvato in {manifest_path}")
    return 0

def main():
    parser = argparse.ArgumentParser(description="Partiziona qualsiasi file master di traduzione FFXIV in batch compatti.")
    parser.add_argument("input_file", help="Percorso del file JSON master (es. data/translations/system/addon.json o textcommand.json)")
    parser.add_argument("--batch-size", "-b", type=int, default=200, help="Numero di voci per batch (default: 200)")
    parser.add_argument("--out-dir", "-o", default="scratch", help="Cartella di output per i lotti (default: scratch)")
    parser.add_argument("--prefix", "-p", default="input_batch_", help="Prefisso per i file batch (default: input_batch_)")
    parser.add_argument("--field", "-f", default=None, help="Limita il partizionamento a uno specifico campo (es. col_2, original, description)")
    parser.add_argument("--detect-english", "-e", action="store_true", help="Identifica come non tradotte anche le stringhe con residui evidenti di inglese")

    args = parser.parse_args()
    sys.exit(split_file(args.input_file, args.batch_size, args.out_dir, args.prefix, args.field, args.detect_english))

if __name__ == "__main__":
    main()
