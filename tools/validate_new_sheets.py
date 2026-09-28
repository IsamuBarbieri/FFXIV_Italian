"""Check coverage and SeString integrity for the eight newly extracted sheets."""

import json
import pathlib
import re
import sys
from translate_addontransient_controls_hex import translate_macro
from translate_addontransient_verminion import translate_row as translate_verminion
from translate_descriptionstring_phantom_hex import translate_row as translate_phantom

ROOT = pathlib.Path(__file__).resolve().parents[1] / "data/translations/da_revisionare"
AREAS = {
    "addontransient": "system", "description": "system", "descriptionstring": "system",
    "classjobactionuicategory": "combat", "classjobcategory": "world",
    "itemsearchcategory": "items", "itemseries": "items", "itemspecialbonus": "items",
}
FILES = ("addontransient", "classjobactionuicategory", "classjobcategory",
         "itemsearchcategory", "itemseries", "itemspecialbonus", "description",
         "descriptionstring")
TAG = re.compile(r"<hex:([0-9A-Fa-f]+)>")
ENGLISH = re.compile(rb"[A-Za-z][A-Za-z '\-]{3,}")
UNTRANSLATED_LICENSES = {
    ("addontransient", row_id, "original") for row_id in ("173", "247", "1016")
}


def pairs(row):
    for source, target in (("original", "translation"), ("name", "translation_name"),
                           ("description", "translation_description"),
                           ("col_2", "translation_col_2")):
        if row.get(source):
            yield source, target


def main():
    issues = []
    for name in FILES:
        rows = json.loads((ROOT / AREAS[name] / f"{name}.json").read_text(encoding="utf-8"))
        pending = translated = hex_pending = excluded = 0
        for row_id, row in rows.items():
            for source, target in pairs(row):
                original, value = row[source], row[target]
                if (name, row_id, source) in UNTRANSLATED_LICENSES:
                    excluded += 1
                    if value and value != original:
                        issues.append((name, row_id, target, "la licenza deve restare originale"))
                    continue
                if not value:
                    pending += 1
                    if any(ENGLISH.search(bytes.fromhex(tag)) for tag in TAG.findall(original)):
                        hex_pending += 1
                    continue
                translated += 1
                original_tags = TAG.findall(original)
                translated_tags = TAG.findall(value)
                if name == "itemspecialbonus" and row_id == "10" and source == "description":
                    if len(original_tags) != len(translated_tags) or not translated_tags[1].startswith("0208"):
                        issues.append((name, row_id, target, "macro plurale non valido"))
                    else:
                        macro = bytes.fromhex(translated_tags[1])
                        if len(macro) != macro[2] + 3 or b"Pezzo" not in macro or b"Pezzi" not in macro:
                            issues.append((name, row_id, target, "lunghezza macro plurale errata"))
                elif name == "addontransient" and row_id in {"482", "486", "490", "491"}:
                    if len(original_tags) != len(translated_tags):
                        issues.append((name, row_id, target, "numero di tag diverso"))
                    for old, new in zip(original_tags, translated_tags):
                        if old == new:
                            continue
                        raw = bytes.fromhex(new)
                        if (raw[:2] != b"\x02\x08" or raw[-1:] != b"\x03"
                                or len(raw) != raw[2] + 3
                                or b"Attiva Aggancio alla Griglia" not in raw
                                or b"Disattiva Aggancio alla Griglia" not in raw):
                            issues.append((name, row_id, target, "tag griglia non valido"))
                elif name == "addontransient" and row_id in {"139", "484"}:
                    if len(original_tags) != len(translated_tags):
                        issues.append((name, row_id, target, "numero di tag diverso"))
                    expected = (b"Mostra/Nascondi Magazzino" if row_id == "139"
                                else b"Mostra/Nascondi Glamour Registrati/Posizionati")
                    for old, new in zip(original_tags, translated_tags):
                        if old == new:
                            continue
                        raw = bytes.fromhex(new)
                        if (raw[:2] != b"\x02\x08" or raw[-1:] != b"\x03"
                                or len(raw) != raw[2] + 3 or expected not in raw):
                            issues.append((name, row_id, target, "tag annidato non valido"))
                elif name == "addontransient" and row_id in {"442", "448"}:
                    if (len(original_tags) != 2 or len(translated_tags) != 2
                            or original_tags[0] != translated_tags[0]
                            or f"<hex:{translated_tags[1]}>" != translate_macro(
                                f"<hex:{original_tags[1]}>")):
                        issues.append((name, row_id, target, "macro dei comandi non valido"))
                elif name == "addontransient" and row_id == "187":
                    if value != translate_verminion(original):
                        issues.append((name, row_id, target, "macro di Verminion non valido"))
                elif name == "descriptionstring" and row_id in {"363", "367", "1643"}:
                    if len(original_tags) != len(translated_tags):
                        issues.append((name, row_id, target, "numero di tag diverso"))
                    for old, new in zip(original_tags, translated_tags):
                        if old == new:
                            continue
                        raw = bytes.fromhex(new)
                        if (raw[:2] != b"\x02\x09" or raw[-1:] != b"\x03"
                                or len(raw) != raw[2] + 3
                                or b"Domenica" not in raw
                                or "Lunedì".encode() not in raw
                                or b"Sunday" in raw):
                            issues.append((name, row_id, target, "tag giorni non valido"))
                elif name == "descriptionstring" and row_id == "1240":
                    if value != translate_phantom(original):
                        issues.append((name, row_id, target, "macro azioni fantasma non valido"))
                elif original_tags != translated_tags:
                    issues.append((name, row_id, target, "tag esadecimali modificati"))
                if original.count("<") - original.count(">") != value.count("<") - value.count(">"):
                    issues.append((name, row_id, target, "parentesi angolari sbilanciate"))
        print(f"{name}: {translated} campi tradotti, {pending} pendenti, "
              f"{hex_pending} pendenti con testo nei tag, {excluded} licenze escluse")
    for issue in issues[:30]:
        print("ERRORE:", *issue)
    return bool(issues)


if __name__ == "__main__":
    sys.exit(main())
