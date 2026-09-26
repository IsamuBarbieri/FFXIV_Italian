#!/usr/bin/env python3
"""Preview or fix capitalization in completed translation sheets."""

import argparse
import json
import pathlib
import re
import subprocess
import sys

ROOT = pathlib.Path(__file__).resolve().parents[1]
TRANSLATIONS = ROOT / "data" / "translations"
CONNECTORS = set("""
    a an the of to in on and or for with at by from as is are it its you your
    i we us me my he she his her if not il lo la gli le un uno una l d di da
    con su per tra fra e o che non ed ad del dello della dei degli delle al
    allo alla ai agli alle dal dallo dalla dai dagli dalle nel nello nella
    nei negli nelle sul sullo sulla sui sugli sulle dell all dall nell sull
    col coi coll cogl colle pel pei am was were be been being have has had do
    does did can could would should will fino contro sotto sopra verso senza
    come se ma né ne ci si vi ti mi
""".split())
SOURCE_WORD = re.compile(r"[^\W\d_]+(?:['’][^\W\d_]+)*", re.UNICODE)
TARGET_TOKEN = re.compile(r"<hex:[^>]*>|[^\W\d_]+", re.UNICODE)
APOSTROPHE_WORD = re.compile(r"(?<![\w])[^\W\d_]+(?:['’][^\W\d_]+)+(?![\w])", re.UNICODE)
FIELD_LINE = re.compile(r'^(\s*)"(translation(?:_[a-z0-9_]+)?)": ("(?:\\.|[^"\\])*")(,?)\s*$')
ROW_LINE = re.compile(r'^  "(\d+)": \{$')


def source_field(field):
    return "original" if field == "translation" else field.removeprefix("translation_")


def fix_value(source, translation):
    # English possessive suffixes stay lowercase, even in title-style labels.
    result = re.sub(r"(?<=[^\W\d_])'S\b", "'s", translation, flags=re.UNICODE)
    visible_source = re.sub(r"<hex:[^>]*>", " ", source)
    if re.search(r"[.!?;,]", visible_source):
        return result

    main_words = [
        word for word in SOURCE_WORD.findall(visible_source)
        if word.casefold() not in CONNECTORS
    ]
    if len(main_words) < 2 or any(not word[0].isupper() for word in main_words):
        return result

    def capitalize(match):
        token = match.group()
        if token.startswith("<hex:"):
            return token
        if match.start() and result[match.start() - 1] == "-":
            return token
        if token.casefold() == "s" and match.start() and result[match.start() - 1] in "'’":
            return token.lower()
        if token[0].islower() and token.casefold() not in CONNECTORS:
            return token[0].upper() + token[1:]
        return token

    result = TARGET_TOKEN.sub(capitalize, result)
    source_names = {}
    for word in SOURCE_WORD.findall(source):
        stem = re.sub(r"['’][sS]$", "", word)
        if "'" in stem or "’" in stem:
            source_names[stem.casefold()] = stem
    return APOSTROPHE_WORD.sub(
        lambda match: source_names.get(match.group().casefold(), match.group()), result
    )


def completed_sheets():
    result = subprocess.run(
        ["dotnet", "run", "--project", "src/FFXIVItalian.Extractor", "--", "status"],
        cwd=ROOT, check=True, capture_output=True, text=True, encoding="utf-8",
    )
    names = set()
    for line in result.stdout.splitlines():
        columns = line.split()
        if line.rstrip().endswith("[COMPLETO]") and len(columns) > 1 and columns[1].endswith(".json"):
            names.add(columns[1])
    return {path for path in TRANSLATIONS.rglob("*.json") if path.name in names}


def collect_changes(paths):
    changes = {}
    for path in paths:
        data = json.loads(path.read_text(encoding="utf-8"))
        replacements = {}
        for row_id, row in data.items():
            if not isinstance(row, dict):
                continue
            for field, value in row.items():
                if not field.startswith("translation") or not isinstance(value, str):
                    continue
                source = row.get(source_field(field))
                if isinstance(source, str) and source:
                    fixed = fix_value(source, value)
                    if fixed != value:
                        replacements[(row_id, field)] = (value, fixed)
        if replacements:
            changes[path] = replacements
    return changes


def apply_changes(changes):
    for path, replacements in changes.items():
        lines = path.read_bytes().decode("utf-8").splitlines(keepends=True)
        row_id = None
        seen = set()
        for index, line in enumerate(lines):
            raw = line.rstrip("\r\n")
            row_match = ROW_LINE.match(raw)
            if row_match:
                row_id = row_match.group(1)
                continue
            field_match = FIELD_LINE.match(raw)
            if not field_match or row_id is None:
                continue
            field = field_match.group(2)
            key = (row_id, field)
            if key not in replacements:
                continue
            old, new = replacements[key]
            if json.loads(field_match.group(3)) != old:
                raise ValueError(f"File cambiato durante l'elaborazione: {path} {key}")
            ending = "\r\n" if line.endswith("\r\n") else "\n" if line.endswith("\n") else ""
            lines[index] = (
                field_match.group(1) + json.dumps(field) + ": "
                + json.dumps(new, ensure_ascii=False) + field_match.group(4) + ending
            )
            seen.add(key)
        if seen != replacements.keys():
            raise ValueError(f"Campi non trovati in {path}: {replacements.keys() - seen}")
        path.write_bytes("".join(lines).encode("utf-8"))


def self_test():
    assert fix_value("Storm's Eye", "Storm'S Eye") == "Storm's Eye"
    assert fix_value("Infamy of Sil'dih", "L'Infamia di Sil'Dih") == "L'Infamia di Sil'dih"
    assert fix_value("An Amalj'aa's Worst Nightmare", "L'Incubo degli Amalj'Aa") == "L'Incubo degli Amalj'aa"
    assert fix_value("Miner's Secondary Tool", "Attrezzo secondario da Minatore") == "Attrezzo Secondario da Minatore"
    assert fix_value("Fishing Tackle", "Attrezzatura da pesca") == "Attrezzatura da Pesca"
    assert fix_value("The Forest", "La Foresta") == "La Foresta"
    tagged = "<hex:021E020B03> Seleziona singolarmente"
    fixed = fix_value("Select Individually", tagged)
    assert fixed == "<hex:021E020B03> Seleziona Singolarmente"
    assert fix_value("Unobtainable", "Non ottenibile") == "Non ottenibile"


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--apply", action="store_true", help="scrive le correzioni; senza opzione mostra solo l'anteprima")
    parser.add_argument("--self-test", action="store_true", help="verifica le regole senza leggere o modificare file")
    args = parser.parse_args()
    if args.self_test:
        self_test()
        print("Regole di capitalizzazione: OK")
        return

    changes = collect_changes(completed_sheets())
    total = sum(map(len, changes.values()))
    mode = "Applicate" if args.apply else "Da applicare"
    print(f"{mode}: {total} campi in {len(changes)} fogli completati al 100%")
    for path, replacements in changes.items():
        print(f"{path.relative_to(ROOT)}: {len(replacements)}")
        for (row_id, field), (old, new) in list(replacements.items())[:3]:
            print(f"  {row_id} {field}: {old!r} -> {new!r}")
    if args.apply:
        apply_changes(changes)


if __name__ == "__main__":
    main()
