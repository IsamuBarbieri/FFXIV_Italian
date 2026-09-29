#!/usr/bin/env python3
"""Scan FFXIV translation JSON files for corrupted SeString values.

The scanner is read-only. It reports the JSON file, entry path, field and
specific reason for every suspicious value, then exits with status 1 when
issues are found.
"""

import argparse
import json
import re
import sys
from pathlib import Path
from typing import Any, Iterator, Optional, Tuple

if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
if hasattr(sys.stderr, "reconfigure"):
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")


HEX_TAG = re.compile(r"<hex:([0-9A-Fa-f]+)>")
HEX_START = re.compile(r"<hex:")
TAG = re.compile(r"<[^>]*>")
READABLE_HEX = re.compile(rb"[A-Za-z][A-Za-z ',.!?/-]{4,}[A-Za-z.!?]")
HEX_IDENTIFIERS = {"ClassJob", "Item", "Player", "Target"}
REPLACEMENT_CHARS = "\ufffd\ufffe\uffff"


def walk_strings(value: Any, path: str = "$") -> Iterator[Tuple[str, str]]:
    """Yield JSON paths and string values from any nested JSON structure."""
    if isinstance(value, dict):
        for key, child in value.items():
            yield from walk_strings(child, f"{path}.{key}")
    elif isinstance(value, list):
        for index, child in enumerate(value):
            yield from walk_strings(child, f"{path}[{index}]")
    elif isinstance(value, str):
        yield path, value


def line_number(raw_text: str, json_path: str) -> Optional[int]:
    """Best-effort line lookup for the top-level numeric entry ID."""
    match = re.search(r"\$\.([^\.\[]+)(?:\.(?:original|translation))?$", json_path)
    if not match:
        return None
    key = json.dumps(match.group(1), ensure_ascii=False)
    found = re.search(rf"^\s*{re.escape(key)}\s*:", raw_text, re.MULTILINE)
    return raw_text.count("\n", 0, found.start()) + 1 if found else None


def readable_hex_text(payload: bytes) -> bytes:
    """Drop FF text-length headers before searching SeString payloads for English."""
    readable = bytearray()
    index = 0
    while index < len(payload):
        if payload[index] == 0xFF and index + 1 < len(payload):
            marker = payload[index + 1]
            if marker < 0xD0:
                readable.append(0)
                index += 2
                continue
            if marker == 0xF2 and index + 3 < len(payload):
                readable.append(0)
                index += 4
                continue
            if marker == 0xF0 and index + 5 < len(payload):
                readable.append(0)
                index += 6
                continue
            readable.append(0)
            index += 1
            continue
        readable.append(payload[index])
        index += 1
    return bytes(readable)


def untranslated_hex(original: str, translation: str) -> list[str]:
    """Find readable source text still present in the matching translated payload."""
    issues = []
    for index, (source, target) in enumerate(zip(HEX_TAG.findall(original), HEX_TAG.findall(translation)), 1):
        try:
            source_bytes, target_bytes = bytes.fromhex(source), bytes.fromhex(target)
        except ValueError:
            continue  # The malformed tag is reported by scan_string.
        # 02 08/09 carry literal alternatives; other opcodes commonly contain sheet lookup keys.
        if source_bytes[:2] not in (b"\x02\x08", b"\x02\x09"):
            continue
        source_text = readable_hex_text(source_bytes)
        target_text = readable_hex_text(target_bytes)
        for match in READABLE_HEX.finditer(source_text):
            phrase = match.group().decode("ascii").strip(" ,.!?/-")
            encoded_phrase = phrase.encode("ascii")
            whole_phrase = re.compile(rb"(?<![A-Za-z])" + re.escape(encoded_phrase) + rb"(?![A-Za-z])")
            if phrase in HEX_IDENTIFIERS or not whole_phrase.search(target_text):
                continue
            issues.append(f"untranslated text in hex tag {index}: {phrase[:80]}")
    return issues


def scan_string(value: str, compare_to: Optional[str] = None, english_hex: bool = False) -> list:
    issues: list[str] = []
    plain_text = HEX_TAG.sub("", value)

    replacements = sorted({f"U+{ord(char):04X}" for char in plain_text if char in REPLACEMENT_CHARS})
    if replacements:
        issues.append(f"replacement character ({', '.join(replacements)})")

    controls = sorted({f"U+{ord(char):04X}" for char in plain_text.replace("\r\n", "") if ord(char) < 0x20 and char not in "\t\n"})
    if controls:
        issues.append(f"raw control character ({', '.join(controls)})")

    for match in HEX_START.finditer(value):
        end = value.find(">", match.start())
        if end < 0:
            issues.append("unterminated <hex:...> tag")
            break
        candidate = value[match.start() : end + 1]
        hex_match = HEX_TAG.fullmatch(candidate)
        if not hex_match:
            issues.append(f"malformed hex tag ({candidate[:60]})")
            continue
        payload = hex_match.group(1)
        if len(payload) % 2:
            issues.append("hex tag with odd number of digits")
        elif payload.startswith("02") and not payload.endswith("03"):
            issues.append("control tag starts with 02 but does not end with 03")

    for tag in TAG.findall(value):
        if tag.startswith("<hex:") and not HEX_TAG.fullmatch(tag):
            issues.append(f"malformed hex tag ({tag[:60]})")

    if compare_to is not None:
        original_tags = HEX_TAG.findall(compare_to)
        current_tags = HEX_TAG.findall(value)
        if original_tags != current_tags:
            issues.append("translation hex tags differ from original")
        if english_hex:
            issues.extend(untranslated_hex(compare_to, value))

    return list(dict.fromkeys(issues))


def scan_file(path: Path, compare_translation: bool, originals_only: bool, english_hex: bool = False) -> list:
    try:
        raw_text = path.read_text(encoding="utf-8")
        data = json.loads(raw_text)
    except (OSError, UnicodeError, json.JSONDecodeError) as error:
        return [{"file": str(path), "path": "$", "issues": [f"cannot read JSON: {error}"]}]

    results: list[dict[str, Any]] = []
    if isinstance(data, dict):
        for key, entry in data.items():
            if not isinstance(entry, dict):
                continue
            for field, value in entry.items():
                if not isinstance(value, str):
                    continue
                if originals_only and field != "original":
                    continue
                source_field = field.removeprefix("translation_") if field.startswith("translation_") else "original" if field == "translation" else None
                if field == "translation" and "original" not in entry:
                    source_field = "name"
                reference = entry.get(source_field) if source_field and value.strip() else None
                issues = scan_string(value, reference if compare_translation else None,
                                     english_hex and isinstance(reference, str))
                if english_hex and not compare_translation and isinstance(reference, str):
                    issues.extend(untranslated_hex(reference, value))
                if issues:
                    json_path = f"$.{key}.{field}"
                    results.append({
                        "file": str(path),
                        "path": json_path,
                        "line": line_number(raw_text, json_path),
                        "issues": issues,
                        "value_preview": value[:160],
                    })
    else:
        for json_path, value in walk_strings(data):
            issues = scan_string(value)
            if issues:
                results.append({"file": str(path), "path": json_path, "issues": issues, "value_preview": value[:160]})
    return results


def collect_files(inputs: list) -> list:
    files: list[Path] = []
    for item in inputs:
        path = Path(item)
        if path.is_file() and path.suffix.lower() == ".json":
            files.append(path)
        elif path.is_dir():
            files.extend(sorted(path.rglob("*.json")))
        else:
            print(f"WARN: percorso ignorato: {item}", file=sys.stderr)
    return sorted(set(files))


def main() -> int:
    parser = argparse.ArgumentParser(description="Scansiona JSON FFXIV alla ricerca di valori corrotti.")
    parser.add_argument("paths", nargs="*", default=["data/translations"], help="File o directory JSON da scansionare.")
    parser.add_argument("--report", type=Path, help="Salva i risultati completi in un report JSON.")
    parser.add_argument("--no-compare", action="store_true", help="Non confrontare i tag hex tra original e translation.")
    parser.add_argument("--originals-only", action="store_true", help="Analizza solo i valori sorgente nel campo original.")
    parser.add_argument("--no-english-hex", action="store_true", help="Disattiva il controllo del testo inglese nei tag hex.")
    args = parser.parse_args()

    files = collect_files(args.paths)
    if not files:
        print("Nessun file JSON trovato.", file=sys.stderr)
        return 2

    findings = [
        finding
        for path in files
        for finding in scan_file(path, not args.no_compare, args.originals_only, not args.no_english_hex)
    ]
    if args.report:
        args.report.parent.mkdir(parents=True, exist_ok=True)
        args.report.write_text(json.dumps(findings, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")

    print(f"File JSON analizzati: {len(files)}")
    print(f"Valori sospetti: {len(findings)}")
    original_count = sum(finding["path"].endswith(".original") for finding in findings)
    translation_count = sum(finding["path"].endswith(".translation") for finding in findings)
    if not args.originals_only:
        print(f"  original corrotti: {original_count}")
        print(f"  translation sospette: {translation_count}")
    for finding in findings:
        location = f"{finding['file']}:{finding.get('line', '?')}:{finding['path']}"
        print(f"- {location}: {'; '.join(finding['issues'])}")
        print(f"  {finding.get('value_preview', '')[:160]}")
    if args.report:
        print(f"Report: {args.report}")
    return 1 if findings else 0


if __name__ == "__main__":
    raise SystemExit(main())
