"""Give LogMessage's local actor a second-person verb, retaining names for others.

Run once after translating a new batch of LogMessage rows. Existing conditional
rows are left intact, so the operation is repeatable.
"""

import json
import re
from pathlib import Path


PATH = Path(__file__).resolve().parents[1] / "data/da_revisionare/system/logmessage.json"
TAG = re.compile(r"<hex:([0-9A-F]+)>")
WORD = re.compile(r"\s+([A-Za-zÀ-ÿ]+(?:'[A-Za-zÀ-ÿ]+)?)")
IRREGULAR = {
    "è": "sei", "ha": "hai", "fa": "fai", "dà": "dai", "va": "vai",
    "sta": "stai", "sa": "sai", "può": "puoi", "puo": "puoi",
}
MACRO_VERBS = {
    ("take", "takes"): ("Subisci", "subisce"),
    ("have", "has"): ("Hai", "ha"),
    ("respinge", "respinge"): ("Respingi", "respinge"),
    ("evade", "evades"): ("Eviti", "evita"),
    ("recuperi", "recupera"): ("Recuperi", "recupera"),
    ("ottieni", "ottiene"): ("Ottieni", "ottiene"),
    ("assorbi", "assorbe"): ("Assorbi", "assorbe"),
    ("suffer", "suffers"): ("Subisci", "subisce"),
    ("move", "moves"): ("Ti sposti", "si sposta"),
    ("perdi", "perde"): ("Perdi", "perde"),
    ("resist", "resists"): ("Resisti", "resiste"),
    ("make", "makes"): ("Esegui", "esegue"),
    ("begin", "begins"): ("Inizi", "inizia"),
    ("sali a bordo", "sale a bordo"): ("Sali a bordo", "sale a bordo"),
}


def second_person(word):
    if word in IRREGULAR:
        return IRREGULAR[word]
    if word.endswith(("cia", "gia", "chia", "ghia", "ia")):
        return word[:-1]
    if word.endswith("ca"):
        return word[:-2] + "chi"
    if word.endswith("ga"):
        return word[:-2] + "ghi"
    if word.endswith(("a", "e")):
        return word[:-1] + "i"
    return None


def verb_pair(tail):
    match = WORD.match(tail)
    if not match:
        macro = re.match(r"\s+<hex:([0-9A-F]+)>", tail)
        if not macro:
            return None
        if macro.group(1) == "020822E4EB02EB03FF0D73616C69206120626F72646FFF0D73616C65206120626F72646F03":
            return macro.end(), "Sali a bordo", "sale a bordo"
        words = []
        for part in macro.group(1).split("FF")[1:]:
            try:
                value = bytes.fromhex(part[2:]).decode("utf-8").rstrip("\x03")
            except (UnicodeError, ValueError):
                continue
            if re.fullmatch(r"[A-Za-zÀ-ÿ]+(?: [A-Za-zÀ-ÿ]+)*", value):
                words.append(value)
        if len(words) >= 2 and (pair := MACRO_VERBS.get(tuple(words[:2]))):
            return macro.end(), pair[0], pair[1]
        return None
    first = match.group(1)
    rest = tail[match.end():]
    if first == "al" and (special := re.match(r" momento non pu[oò]", rest)):
        return match.end() + special.end(), "Al momento non puoi", "al" + special.group()
    if first == "s'inchina":
        return match.end(), "Ti inchini", first
    if first in ("si", "non"):
        next_word = WORD.match(rest)
        if not next_word:
            return None
        second = next_word.group(1)
        if first == "non" and second == "si":
            third_word = WORD.match(rest[next_word.end():])
            if not third_word or not (inflected := second_person(third_word.group(1))):
                return None
            third = "non si " + third_word.group(1)
            return match.end() + next_word.end() + third_word.end(), "Non ti " + inflected, third
        if first == "non" and second in ("è", "e") and "riuscito" in rest[:40]:
            return None
        inflected = second_person(second)
        if not inflected:
            return None
        return match.end() + next_word.end(), ("Ti " if first == "si" else "Non ") + inflected, first + " " + second
    if first in ("viene", "e"):
        return None
    if first == "è" and not re.match(r" (?:ora (?:nella|un |il |leader)|il |un |in preda|invulnerabile|K\.O\.)", rest):
        return None
    inflected = second_person(first)
    if not inflected:
        return None
    return match.end(), inflected[0].upper() + inflected[1:], first


def se_int(value):
    if 0 <= value < 0xCF:
        return bytes([value + 1])
    if 0 <= value <= 0xFFFF:
        return b"\xF2" + value.to_bytes(2, "big")
    else:
        raise ValueError("SeString branch too long")


def se_string(value):
    return b"\xFF" + se_int(len(value)) + value


def conditional(condition, own, other):
    body = condition + se_string(own.encode("utf-8")) + se_string(other)
    raw = b"\x02\x08" + se_int(len(body)) + body + b"\x03"
    return "<hex:" + raw.hex().upper() + ">"


def convert(original, translated):
    source = next((m for m in TAG.finditer(original)
                   if m.group(1).startswith("022B") and "FF04796F75" in m.group(1)), None)
    if not source:
        return translated
    source_bytes = bytes.fromhex(source.group(1))
    condition = next((part for part in (bytes.fromhex("E4EB02EB03"), bytes.fromhex("E4EB02EB04"))
                      if part in source_bytes), None)
    if condition is None:
        return translated
    parameter = "E908" if condition[-1] == 3 else "E909"
    for actor in TAG.finditer(translated):
        code = actor.group(1)
        if not (code.startswith(("022B", "020818")) and "4F626A537472" in code and parameter in code):
            continue
        prefix = TAG.sub("", translated[:actor.start()]).strip(" \ue06f")
        if prefix and not prefix.endswith((".", "!", "?")):
            continue
        tail = translated[actor.end():]
        pair = verb_pair(tail)
        if not pair:
            continue
        end, own, other_verb = pair
        other = bytes.fromhex(code) + b" " + other_verb.encode("utf-8")
        try:
            replacement = conditional(condition, own, other)
        except ValueError:
            continue
        return translated[:actor.start()] + replacement + tail[end:]
    return translated


def main():
    raw = PATH.read_text(encoding="utf-8")
    data = json.loads(raw)
    changes = {key: updated for key, row in data.items()
               if (updated := convert(row.get("original", ""), row.get("translation", ""))) != row.get("translation", "")}
    lines = []
    key = None
    for line in raw.splitlines(keepends=True):
        if match := re.match(r'^  "(\d+)": \{', line):
            key = match.group(1)
        if key in changes and line.lstrip().startswith('"translation":'):
            line = '    "translation": ' + json.dumps(changes[key], ensure_ascii=False) + ('\r\n' if line.endswith('\r\n') else '\n')
        lines.append(line)
    PATH.write_text("".join(lines), encoding="utf-8")
    print(f"Updated {len(changes)} LogMessage rows")


if __name__ == "__main__":
    main()
