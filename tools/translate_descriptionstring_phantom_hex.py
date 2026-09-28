"""Translate the two conditional control-guide branches for phantom actions."""

import json
import pathlib
import re

PATH = pathlib.Path(__file__).resolve().parents[1] / "data/translations/da_revisionare/system/descriptionstring.json"
TAG = re.compile(r"<hex:([0-9A-Fa-f]+)>")
SLOT = re.compile(r"\{@(\d+)\}")

LABELS = {
    "Keyboard and Mouse": "Tastiera e Mouse",
    "By clicking phantom action icons displayed on the HUD.":
        "Facendo clic sulle icone delle azioni fantasma nell'HUD.",
    "By pressing ": "Premendo ",
    " with the cross hotbar active.": " con la Barra Croce attiva.",
    "Select Phantom Action": "Seleziona Azione Fantasma",
    "Execute Phantom Action": "Esegui Azione Fantasma",
}

TEXT = (
    "Puoi eseguire e attivare o disattivare le azioni fantasma in questi modi:"
    "{@0}{@1}{@2}{@3}■Assegnare le Azioni dell'Incarico alla Barra Azioni o alla Barra Croce"
    "{@4}Se assegni le Azioni dell'Incarico I e II della scheda Incarichi in Azioni e Tratti "
    "alla Barra Azioni o alla Barra Croce, nella Falce Occulta le icone diventeranno "
    "rispettivamente Esegui Azione Fantasma e Attiva/Disattiva Azione Fantasma."
    "{@5}{@6}■Assegnare le Azioni Fantasma alla Barra Azioni o alla Barra Croce"
    "{@7}Se assegni le Azioni Fantasma I-V della scheda Incarichi in Azioni e Tratti "
    "alla Barra Azioni o alla Barra Croce, nella Falce Occulta le icone diventeranno "
    "automaticamente le rispettive azioni fantasma."
)


def translate_macro(tag):
    raw = bytes.fromhex(TAG.fullmatch(tag).group(1))
    assert raw[:3] == b"\x02\x08\xf2" and raw[-1:] == b"\x03"
    assert len(raw) == int.from_bytes(raw[3:5], "big") + 6
    body = raw[5:-1]
    assert body[:4] == b"\xe4\xe9Q\x01"
    branches = []
    pos = 4
    for _ in range(2):
        assert body[pos:pos + 3] == b"\xff\xf0\xe7"
        branch = body[pos + 3:pos + 3 + 231]
        assert len(branch) == 231 and b"\xff" not in branch
        for old, new in LABELS.items():
            assert old.encode("utf-8") in branch or old in (
                "Keyboard and Mouse",
                "By clicking phantom action icons displayed on the HUD.",
            ), old
            branch = branch.replace(old.encode("utf-8"), new.encode("utf-8"))
        branches.append(branch)
        pos += 3 + 231
    assert pos == len(body)
    new_body = body[:4] + b"".join(
        b"\xff\xf2" + len(branch).to_bytes(2, "big") + branch for branch in branches
    )
    result = b"\x02\x08\xf2" + len(new_body).to_bytes(2, "big") + new_body + b"\x03"
    assert len(result) == int.from_bytes(result[3:5], "big") + 6
    return f"<hex:{result.hex().upper()}>"


def translate_row(original):
    tags = [f"<hex:{value}>" for value in TAG.findall(original)]
    assert len(tags) == 8
    tags[2] = translate_macro(tags[2])
    assert [int(match.group(1)) for match in SLOT.finditer(TEXT)] == list(range(8))
    return SLOT.sub(lambda match: tags[int(match.group(1))], TEXT)


def main():
    rows = json.loads(PATH.read_text(encoding="utf-8"))
    rows["1240"]["translation"] = translate_row(rows["1240"]["original"])
    PATH.write_text(json.dumps(rows, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


if __name__ == "__main__":
    main()
