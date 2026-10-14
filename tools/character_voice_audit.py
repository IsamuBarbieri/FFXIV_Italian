"""Read-only census of Voiceman text and matching local voice containers."""

import argparse
from collections import Counter, defaultdict
import json
from pathlib import Path
import re
import struct
import zlib

ROOT = Path(__file__).resolve().parents[1]
LABEL = re.compile(r"TEXT_VOICEMAN_(\d+)_([A-Z]?\d+)_(.+)")
LANGUAGES = ("en", "ja", "fr", "de")


def crc(text):
    return zlib.crc32(text.lower().encode("utf-8")) ^ 0xFFFFFFFF


def parse_label(text):
    match = LABEL.fullmatch(text)
    if not match:
        raise ValueError(f"Unrecognized Voiceman label: {text!r}")
    return match.groups()


def classify(label, text):
    if not text.strip():
        return "empty"
    if "NONE_VOICE" in label:
        return "explicit_no_voice"
    _, _, tag = parse_label(label)
    if tag.startswith("SYSTEM") or tag[0].isdigit() and tag not in ("2P", "9S"):
        return "technical"
    clean = re.sub(r"^\(-.*?-\)", "", text.strip()).strip()
    clean = re.sub(r"<hex:[0-9A-Fa-f]+>", "", clean).strip()
    if re.search(r"SHOULD NOT DISPLAY|REPORT IF (?:LABEL )?DISPLAY(?:S|ED)|NO DISPLAY|DO NOT DISPLAY|TO BE DELETED", clean, re.I):
        return "direction"
    if not re.search(r"[A-Za-z\u3040-\u30ff\u4e00-\u9fff]", clean):
        return "nonlexical_or_payload"
    # Bracketed dragon speech is dialogue, not an acting direction.
    if re.fullmatch(r"[\[(].*[\])]", clean, re.S) and (
        clean.upper() == clean or re.search(r"\b(?:grunt|roar|reax|exert(?:ion)?|gasp|scream|groan|sigh|chuckle|laugh(?:ter)?|sobbing|cough|pant(?:ing)?|breathing|snort|throat-clearing|cry|cries|crying|exhaling)\b", clean, re.I)
    ):
        return "direction"
    if re.fullmatch(r"(?:H[amnu]+|[GA][argh]+|U[gh]+|Heh|Hah|Ah|Oh|Gwah)[.!?…\s-]*", clean, re.I):
        return "vocalization_candidate"
    if re.fullmatch(r"(?:Hmph|Huh|Hmmm|Uhh|Urgh|Grr+|Aa+h+|Ohoho(?:ho)*|Ha(?:ha)+|He(?:he)+|Gwahaha(?:ha)*)[.!?…\s-]*", clean, re.I):
        return "vocalization_candidate"
    return "dialogue_candidate"


def read_index(path, entry_size):
    data = path.read_bytes()
    if len(data) < 16 or not data.startswith(b"SqPack"):
        raise ValueError(f"Invalid SqPack index: {path}")
    header = struct.unpack_from("<I", data, 12)[0]
    if header + 16 > len(data):
        raise ValueError(f"Truncated SqPack header: {path}")
    offset, size = struct.unpack_from("<II", data, header + 8)
    if offset < header + 16 or offset + size > len(data) or size % entry_size:
        raise ValueError(f"Invalid SqPack index bounds: {path}")
    format_ = "<QII" if entry_size == 16 else "<II"
    result = {}
    for entry in struct.iter_unpack(format_, data[offset:offset + size]):
        key, location = entry[:2]
        if key in result and result[key] != location:
            raise ValueError(f"Ambiguous hash in {path}: {key:x}")
        result[key] = location
    return result


class VoiceIndex:
    def __init__(self, sqpack):
        self.sqpack = Path(sqpack)
        self.indexes = {}
        self.containers = {}
        self.streams = {}
        for path in sorted(self.sqpack.glob("*/03*.index2")):
            repository = path.parent.name
            index = read_index(path.with_suffix(".index"), 16)
            index2 = read_index(path, 8)
            self.indexes[repository] = (path, index, index2)
        if not self.indexes:
            raise FileNotFoundError("No cutscene indexes found in the supplied sqpack folder")

    def close(self):
        for stream in self.streams.values():
            stream.close()

    def _is_scd(self, index_path, location):
        key = (index_path, location)
        if key in self.containers:
            return self.containers[key]
        if location & 1:
            raise ValueError(f"Synonym entry needs separate resolution: {index_path}, {location:x}")
        dat_id = (location & 14) >> 1
        offset = (location & ~15) * 8
        dat = index_path.with_suffix(f".dat{dat_id}")
        if dat not in self.streams:
            self.streams[dat] = dat.open("rb")
        stream = self.streams[dat]
        stream.seek(offset)
        raw = stream.read(24)
        if len(raw) != 24:
            raise ValueError(f"Truncated file header in {dat}")
        size, type_, raw_size, _, _, count = struct.unpack("<IIIIII", raw)
        if type_ in (1, 2) and raw_size == 0:
            self.containers[key] = False
            return False
        if type_ != 2 or size < 32 or count < 1:
            raise ValueError(f"Unexpected voice file header in {dat} at {offset}")
        if raw_size < 8:
            self.containers[key] = False
            return False
        block = stream.read(8)
        if len(block) != 8:
            raise ValueError(f"Truncated block table in {dat}")
        relative, _, _ = struct.unpack("<IHH", block)
        stream.seek(offset + size + relative)
        raw = stream.read(16)
        if len(raw) != 16:
            raise ValueError(f"Truncated block header in {dat}")
        header, _, compressed, uncompressed = struct.unpack("<IIII", raw)
        if header < 16 or uncompressed > 65536 or compressed != 32000 and compressed > 65536:
            raise ValueError(f"Invalid voice block in {dat}")
        stream.seek(offset + size + relative + header)
        payload = stream.read(uncompressed if compressed == 32000 else compressed)
        first = payload if compressed == 32000 else zlib.decompress(payload, -15)
        if len(first) != uncompressed:
            raise ValueError(f"Voice block size mismatch in {dat}")
        if not any(first):
            self.containers[key] = False
            return False
        result = first.startswith(b"SEDBSSCF")
        if not result:
            raise ValueError(f"Matching path is not an SCD container in {dat}: {first[:16].hex()}")
        self.containers[key] = result
        return result

    def find(self, sheet, line):
        major = int(sheet[:2])
        repository = "ffxiv" if major < 3 else f"ex{major - 2}"
        if repository not in self.indexes:
            return {}
        path, index, index2 = self.indexes[repository]
        folder = f"cut/{repository}/sound/voicem/voiceman_{sheet}"
        found = {}
        for language in LANGUAGES:
            for variant in ("m", "f"):
                filename = f"vo_voiceman_{sheet}_{line.lower()}_{variant}_{language}.scd"
                full_path = f"{folder}/{filename}"
                location = index2.get(crc(full_path))
                if location is None:
                    continue
                split_hash = (crc(folder) << 32) | crc(filename)
                if index.get(split_hash) != location:
                    raise ValueError(f"Index/index2 disagreement for {full_path}")
                if self._is_scd(path, location):
                    found.setdefault(language, []).append(full_path)
        return found


def scan(data_dir, voice_index):
    result = []
    paths = []
    for state in ("translations", "da_revisionare", "da_tradurre"):
        paths.extend(sorted((data_dir / state / "dialogue/cut_scene").rglob("*.json")))
    for path in paths:
        rows = json.loads(path.read_text(encoding="utf-8-sig"))
        for row_id, row in rows.items():
            sheet, line, speaker = parse_label(row["name"])
            expected_sheet = f"voiceman_{sheet}"
            if path.stem != expected_sheet:
                raise ValueError(f"Label/sheet disagreement in {path}#{row_id}")
            text = row["description"]
            result.append({
                "reference": f"{path.relative_to(data_dir.parent).as_posix()}#{row_id}:description",
                "label": row["name"], "sheet": sheet, "line": line,
                "speaker_tag": speaker, "text": text,
                "kind": classify(row["name"], text),
                "japanese_text": bool(re.search(r"[\u3040-\u30ff\u4e00-\u9fff]", text)),
                "audio": voice_index.find(sheet, line),
            })
    return paths, result


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--sqpack", type=Path, required=True)
    parser.add_argument("--out", type=Path, required=True, help="Local, unversioned audit report")
    args = parser.parse_args()
    if args.out.exists():
        parser.error("Output already exists; choose a new report filename")
    if args.out.resolve().is_relative_to((ROOT / "data").resolve()):
        parser.error("Keep audit reports outside data/; this command does not write translation files")
    index = VoiceIndex(args.sqpack)
    try:
        files, rows = scan(ROOT / "data", index)
    finally:
        index.close()
    kinds = dict(Counter(row["kind"] for row in rows))
    tags = defaultdict(list)
    for row in rows:
        tags[row["speaker_tag"]].append(row)
    version_file = args.sqpack.parent / "ffxivgame.ver"
    report = {
        "client_version": version_file.read_text(encoding="utf-8").strip() if version_file.exists() else None,
        "scope": [p.relative_to(ROOT).as_posix() for p in files],
        "summary": {"files": len(files), "rows": len(rows), "speaker_tags": len(tags), "kinds": kinds,
                    "dialogue_with_en_asset": sum(r["kind"] == "dialogue_candidate" and "en" in r["audio"] for r in rows)},
        "rows": rows,
    }
    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(report["summary"], ensure_ascii=False, indent=2))


if __name__ == "__main__":
    main()
