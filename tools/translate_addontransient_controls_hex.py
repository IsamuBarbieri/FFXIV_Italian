"""Translate the paired keyboard/controller guide macro without changing controls."""

import json
import pathlib
import re

PATH = pathlib.Path(__file__).resolve().parents[1] / "data/translations/da_revisionare/system/addontransient.json"
TAG = re.compile(r"<hex:([0-9A-Fa-f]+)>")

OPTIONS = {
    "tab, shift+tab": "tab, maiusc+tab",
    "Tab, Shift+Tab": "Tab, Maiusc+Tab",
    "space bar": "barra spaziatrice",
    "Space Bar": "Barra Spaziatrice",
    "D-pad": "croce direzionale",
    "Directional Buttons": "Pulsanti Direzionali",
}

LABELS = {
    " or ": " o ",
    "Move Camera": "Muovi Telecamera",
    "Reset Camera": "Reimposta Telecamera",
    "Change Target Character": "Cambia Personaggio Bersaglio",
    "Pause/Play": "Pausa/Riprendi",
    "Open/Close Controls": "Apri/Chiudi Comandi",
    "Draw/Sheathe Weapon": "Estrai/Riponi Arma",
    "Auto Run": "Corsa Automatica",
    "Movement": "Movimento",
    "Jump": "Salta",
    "Buttons": "Pulsanti",
    "Button": "Pulsante",
    "Keys": "Tasti",
    "Key": "Tasto",
    "Stick": "Levetta",
}


def resize_branch(raw):
    for old, new in OPTIONS.items():
        old_bytes = old.encode("utf-8")
        new_bytes = new.encode("utf-8")
        needle = b"\xff" + bytes([len(old_bytes) + 1]) + old_bytes
        count = raw.count(needle)
        assert count <= 1, (old, count)
        if count:
            raw = raw.replace(needle, b"\xff" + bytes([len(new_bytes) + 1]) + new_bytes)
    for old, new in LABELS.items():
        raw = raw.replace(old.encode("utf-8"), new.encode("utf-8"))
    return raw


def translate_macro(tag):
    raw = bytes.fromhex(TAG.fullmatch(tag).group(1))
    assert raw[:5] == b"\x02\x08\xf2\x03\xff" and raw[-1:] == b"\x03"
    assert len(raw) == int.from_bytes(raw[3:5], "big") + 6
    body = raw[5:-1]
    assert body[:4] == b"\xe4\xe9L\x01"
    branches = []
    pos = 4
    for _ in range(2):
        assert body[pos:pos + 2] == b"\xff\xf2"
        size = int.from_bytes(body[pos + 2:pos + 4], "big")
        branch = body[pos + 4:pos + 4 + size]
        assert len(branch) == size
        branches.append(resize_branch(branch))
        pos += 4 + size
    assert pos == len(body)
    new_body = body[:4] + b"".join(
        b"\xff\xf2" + len(branch).to_bytes(2, "big") + branch for branch in branches
    )
    result = b"\x02\x08\xf2" + len(new_body).to_bytes(2, "big") + new_body + b"\x03"
    assert len(result) == int.from_bytes(result[3:5], "big") + 6
    return f"<hex:{result.hex().upper()}>"


def main():
    rows = json.loads(PATH.read_text(encoding="utf-8"))
    for row_id in ("442", "448"):
        row = rows[row_id]
        tags = TAG.findall(row["original"])
        assert len(tags) == 2 and tags[0] == "02100103"
        old_tag = f"<hex:{tags[1]}>"
        row["translation"] = row["original"].replace(
            "※Keyboard/Mouse settings listed are default.",
            "※Le impostazioni Tastiera/Mouse elencate sono quelle predefinite.",
        ).replace(old_tag, translate_macro(old_tag))
    PATH.write_text(json.dumps(rows, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


if __name__ == "__main__":
    main()
