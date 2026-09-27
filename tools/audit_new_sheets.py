"""Report exact translation reuse and readable English inside SeString hex tags."""

import collections
import argparse
import json
import pathlib
import re

ROOT = pathlib.Path(__file__).resolve().parents[1] / "data" / "translations"
NEW = ("addontransient", "classjobactionuicategory", "classjobcategory",
       "itemsearchcategory", "itemseries", "itemspecialbonus", "description",
       "descriptionstring")
HEX = re.compile(r"<hex:([0-9A-Fa-f]+)>")
ASCII = re.compile(rb"[A-Za-z][A-Za-z '\-]{3,}")
APPROVED = ("system/addon.json", "system/howto.json", "system/howtocategory.json",
            "system/lobby.json", "system/maincommand.json",
            "system/maincommandcategory.json", "world/classjob.json",
            "world/placename.json", "world/race.json", "world/tribe.json",
            "world/weather.json")


def fields(row):
    for target, source in (("translation", "original"),
                           ("translation_name", "name"),
                           ("translation_description", "description"),
                           ("translation_col_2", "col_2")):
        if source in row and target in row and row[source]:
            yield source, target


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--apply-exact", action="store_true")
    args = parser.parse_args()
    known = collections.defaultdict(set)
    for relative in APPROVED:
        path = ROOT / relative
        for row in json.loads(path.read_text(encoding="utf-8-sig")).values():
            if not isinstance(row, dict):
                continue
            for source, target in fields(row):
                if row[target]:
                    known[row[source]].add(row[target])

    for name in NEW:
        path = ROOT / "misc" / f"{name}.json"
        rows = json.loads(path.read_text(encoding="utf-8-sig"))
        exact = conflicts = 0
        readable = []
        for row_id, row in rows.items():
            for source, target in fields(row):
                if not row[target]:
                    exact += len(known.get(row[source], ())) == 1
                    conflicts += len(known.get(row[source], ())) > 1
                    candidates = known.get(row[source], ())
                    if args.apply_exact and len(candidates) == 1:
                        candidate = next(iter(candidates))
                        tags = HEX.findall(row[source])
                        has_english_hex = any(ASCII.search(bytes.fromhex(tag)) for tag in tags)
                        if not has_english_hex and tags == HEX.findall(candidate):
                            row[target] = candidate
                for match in HEX.finditer(row[source]):
                    for phrase in ASCII.findall(bytes.fromhex(match.group(1))):
                        readable.append((row_id, source, phrase.decode("ascii", "replace")))
        print(f"{name}: {len(rows)} righe; {exact} corrispondenze univoche; "
              f"{conflicts} conflitti; {len(readable)} frammenti ASCII nei tag")
        if args.apply_exact:
            path.write_text(json.dumps(rows, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
        for item in readable[:8]:
            print("  ", *item)


if __name__ == "__main__":
    main()
