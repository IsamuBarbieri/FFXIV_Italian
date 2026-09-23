#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
split_untranslated.py
---------------------
Partiziona le righe non ancora tradotte di un file JSON master di FFXIV
in batch compatti (es. input_batch_A.json, input_batch_B.json...)
per la traduzione parallela con subagenti o LLM a consumo minimo di token.
"""

import argparse
import json
import os
import sys

# Compatibilità encoding per console Windows
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
if hasattr(sys.stderr, "reconfigure"):
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")

def get_batch_label(index: int) -> str:
    """Genera etichette A, B, ... Z, AA, AB... per indice 0-based."""
    label = ""
    index += 1
    while index > 0:
        index, rem = divmod(index - 1, 26)
        label = chr(65 + rem) + label
    return label

def split_file(input_file: str, batch_size: int = 500, out_dir: str = "scratch", prefix: str = "input_batch_") -> int:
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

    # Estrazione righe non tradotte
    untrans = {}
    for k, v in data.items():
        if isinstance(v, dict):
            orig = v.get("original", "")
            trans = v.get("translation", "")
            # Considera non tradotta se translation è assente o vuota, e original non è vuoto
            if orig and (trans is None or str(trans).strip() == ""):
                untrans[str(k)] = orig

    total_untrans = len(untrans)
    print(f"File master: {input_file}")
    print(f"Righe totali nel master: {len(data)}")
    print(f"Righe non tradotte individuate: {total_untrans}")

    if total_untrans == 0:
        print("Nessuna riga pendente da tradurre.")
        return 0

    # Ordina le chiavi (numerico se possibile, altrimenti alfabetico)
    try:
        sorted_keys = sorted(untrans.keys(), key=lambda x: int(x))
    except ValueError:
        sorted_keys = sorted(untrans.keys())

    manifest = {
        "master_file": os.path.abspath(input_file),
        "total_untranslated": total_untrans,
        "batch_size": batch_size,
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

        print(f"  Batch {batch_id}: {len(chunk_dict)} righe -> {batch_path}")
        batch_idx += 1

    manifest_path = os.path.join(out_dir, "batch_manifest.json")
    with open(manifest_path, "w", encoding="utf-8") as mf:
        json.dump(manifest, mf, ensure_ascii=False, indent=2)

    print(f"\nGenerati {batch_idx} batch in '{out_dir}/'. Manifest salvato in {manifest_path}")
    return 0

def main():
    parser = argparse.ArgumentParser(description="Partiziona file master di traduzione FFXIV in batch compatti.")
    parser.add_argument("input_file", help="Percorso del file JSON master (es. data/translations/system/addon.json)")
    parser.add_argument("--batch-size", "-b", type=int, default=500, help="Numero di righe per batch (default: 500)")
    parser.add_argument("--out-dir", "-o", default="scratch", help="Cartella di output per i lotti (default: scratch)")
    parser.add_argument("--prefix", "-p", default="input_batch_", help="Prefisso per i file batch (default: input_batch_)")

    args = parser.parse_args()
    sys.exit(split_file(args.input_file, args.batch_size, args.out_dir, args.prefix))

if __name__ == "__main__":
    main()
