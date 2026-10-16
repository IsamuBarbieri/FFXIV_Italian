"""Read-only coverage, macro-control and license checks for eight UI sheets."""

import collections
import json
import pathlib
import re
import sys
from translate_hex_literals import control_signature, macro_signature, player_gender_branches


ROOT = pathlib.Path(__file__).resolve().parents[1] / "data/translations"
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
    # General third-party software notices, distinct from translated in-game UI notices.
    ("addontransient", "173", "original")
}


def pairs(row):
    for source, target in (("original", "translation"), ("name", "translation_name"),
                           ("description", "translation_description"),
                           ("col_2", "translation_col_2")):
        if row.get(source):
            yield source, target


def encode_fragments(text):
    return b"".join(bytes.fromhex(part[5:-1]) if part.startswith("<hex:")
                    else part.encode("utf-8")
                    for part in re.split(r"(<hex:[0-9A-Fa-f]+>)", text))


def check_sestring_controls(original, translation):
    source_tags = TAG.findall(original)
    target_tags = TAG.findall(translation)
    if source_tags == target_tags:
        return []
    try:
        native_gender = collections.Counter()
        for raw in source_tags:
            tag = f"<hex:{raw}>"
            if player_gender_branches(tag) is not None:
                native_gender[macro_signature(tag)] += 1

        def preserve_controls(match):
            tag = match.group()
            if player_gender_branches(tag) is None:
                return tag
            signature = macro_signature(tag)
            if native_gender[signature]:
                native_gender[signature] -= 1
                return tag
            # Italian may add player agreement; these verified branches have no controls.
            return ""

        controlled_translation = TAG.sub(preserve_controls, translation)
        if control_signature(encode_fragments(original)) != control_signature(encode_fragments(controlled_translation)):
            return ["controlli o referenti SeString modificati"]
    except (ValueError, IndexError) as error:
        return [f"SeString non decodificabile: {error}"]
    return []


def main():
    issues = []
    for name in FILES:
        rows = json.loads((ROOT / AREAS[name] / f"{name}.json").read_text(encoding="utf-8-sig"))
        pending = translated = hex_pending = excluded = 0
        for row_id, row in rows.items():
            for source, target in pairs(row):
                original, value = row[source], row.get(target, "")
                if (name, row_id, source) in UNTRANSLATED_LICENSES:
                    excluded += 1
                    if value != original:
                        issues.append((name, row_id, target, "la licenza deve restare originale"))
                    continue
                if not value:
                    pending += 1
                    if any(ENGLISH.search(bytes.fromhex(tag)) for tag in TAG.findall(original)):
                        hex_pending += 1
                    continue
                translated += 1
                issues.extend((name, row_id, target, error)
                              for error in check_sestring_controls(original, value))
                if original.count("<") - original.count(">") != value.count("<") - value.count(">"):
                    issues.append((name, row_id, target, "parentesi angolari sbilanciate"))
        print(f"{name}: {translated} campi tradotti, {pending} pendenti, "
              f"{hex_pending} pendenti con testo nei tag, {excluded} licenze escluse")
        if pending:
            issues.append((name, "*", "*", f"{pending} campi pendenti"))
    for issue in issues[:30]:
        print("ERRORE:", *issue)
    return bool(issues)


if __name__ == "__main__":
    sys.exit(main())
