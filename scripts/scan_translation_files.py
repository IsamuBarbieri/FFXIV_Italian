#!/usr/bin/env python3
"""Elenca i file da tradurre dal più piccolo al più grande, escludendo le quest."""

import argparse
import json
import re
from pathlib import Path

from split_untranslated import detect_translatable_pairs

TAG = re.compile(r"<[^>]*>")
# ponytail: CamelCase is treated as an identifier; use schema-aware rules if such labels become translatable.
IDENTIFIER = re.compile(
    r"(?:[A-Z][A-Z0-9_]*|[A-Z][A-Za-z0-9]*[A-Z][A-Za-z0-9]*|"
    r"[0-9]+[A-Za-z][A-Za-z0-9_]*|[A-Za-z][A-Za-z0-9]*(?:_[A-Za-z0-9]+)+|"
    r"Warp[A-Z][A-Za-z0-9]*)\Z"
)
QUEST_DIRS = {"quest", "quests"}


def has_translatable_text(value: object) -> bool:
    if not isinstance(value, str):
        return False
    visible = TAG.sub("", value).strip()
    visible = re.sub(r"\[(?:FC|GM|NOVICE|PvP|CWLS[1-8])\]", "", visible).strip(" <>:()")
    return (any(char.isalpha() for char in visible)
            and not visible.startswith("[翻訳不要]")
            and visible != "未使用"
            and not visible.startswith(("http://", "https://"))
            and (not IDENTIFIER.fullmatch(visible)
                 or re.fullmatch(r"DPS\d+", visible)))


def pending_rows(path: Path) -> int:
    data = json.loads(path.read_text(encoding="utf-8-sig"))
    count = 0
    for entry in data.values():
        if not isinstance(entry, dict):
            continue
        if any(has_translatable_text(entry.get(src, "")) and
               not entry.get(dst, "").strip()
               for src, dst in detect_translatable_pairs(entry)):
            count += 1
    return count


def main() -> None:
    parser = argparse.ArgumentParser(description="Trova i file pendenti più piccoli, senza quest.")
    parser.add_argument("--limit", type=int, default=0, help="Mostra al massimo N file (0 = tutti).")
    parser.add_argument("--max-pending", type=int, help="Mostra solo file con meno di N righe pendenti.")
    args = parser.parse_args()

    root = Path("data/da_tradurre")
    files = [(p, pending_rows(p)) for p in root.rglob("*.json")
             if not any(part.casefold() in QUEST_DIRS for part in p.relative_to(root).parts)]
    files = [(p, n) for p, n in files if n and (args.max_pending is None or n < args.max_pending)]
    files.sort(key=lambda item: (item[0].stat().st_size, item[1], item[0].as_posix().casefold()))
    if args.limit:
        files = files[:args.limit]

    print(f"File pendenti senza cartella quest: {len(files)}"
          + (f" con meno di {args.max_pending} righe" if args.max_pending is not None else "")
          + (f" (primi {args.limit})" if args.limit else ""))
    print(f"{'Righe pendenti':>14}  {'Dimensione':>12}  File")
    for path, count in files:
        print(f"{count:>14,}  {path.stat().st_size:>9,} B  {path.as_posix()}")


if __name__ == "__main__":
    main()
