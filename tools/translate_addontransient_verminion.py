"""Translate the Lord of Verminion guide, including text embedded in macros."""

import json
import pathlib
import re

PATH = pathlib.Path(__file__).resolve().parents[1] / "data/translations/system/addontransient.json"
TAG = re.compile(r"<hex:([0-9A-Fa-f]+)>")

SEGMENTS = {
    2: "Conosci le Tue Strutture", 7: "Pietre Arcane",
    10: "  Tu e l'avversario avete tre cristalli immobili ciascuno",
    11: "  sul campo. Distruggi tutte le Pietre Arcane nemiche per",
    12: "  vincere.",
    15: "Varchi",
    18: "  I minion appaiono davanti al varco selezionato.",
    19: "  Riportali nell'area di un varco per recuperare PV.",
    20: "  Puoi evocare minion anche da un varco distrutto, ma non",
    21: "  potranno lasciare la sua area. Viceversa, quelli all'esterno",
    22: "  non potranno entrarvi.",
    25: "Occhio Esploratore",
    28: "  Questa struttura rivela i minion avversari. Se viene",
    29: "  distrutta, vedrai solo le unità nemiche vicine ai tuoi",
    30: "  minion.",
    33: "Scudo",
    36: "  Questa struttura rafforza la difesa delle Pietre Arcane.",
    37: "  Distruggi lo Scudo nemico per rendere le Pietre Arcane",
    38: "  avversarie più facili da spezzare.",
    42: "Rigenerazione delle Strutture",
    45: "  Tutte le strutture tranne le Pietre Arcane possono essere",
    46: "  distrutte solo temporaneamente. Le loro funzioni tornano",
    47: "  pienamente attive dopo un certo tempo.",
    48: "  ※Posiziona i minion vicino a una struttura distrutta per",
    49: "  　accelerarne la riattivazione.",
    53: "Sfrutta i Punti Forti dei Tuoi Minion",
    56: "  Ogni tipo di minion infligge più danni a determinate",
    57: "  strutture. Impara i punti forti di ciascuna unità e scegli",
    58: "  il minion giusto!",
    62: "Ribalta le Sorti con le Azioni Speciali",
    65: "  Per usare un'azione speciale servono queste condizioni:",
    66: "  ・Quattro minion dello stesso tipo nel gruppo d'azione",
    67: "  ・Punti azione al massimo per ogni minion del gruppo",
    68: "   (I punti azione si accumulano col tempo)",
    69: "  Per eseguire un'azione speciale, seleziona un minion del",
    70: "  gruppo d'azione e premi Esegui Azione nella barra azioni",
    71: "  dei minion.",
    75: "Usare le Trappole",
    78: "  Alcuni minion piazzano trappole con le loro azioni speciali.",
    79: "  Una trappola si arma dieci secondi dopo essere stata piazzata.",
    80: "  Puoi attivarla premendo Attiva Trappola nella barra azioni",
    81: "  dei minion.",
    82: "  ※Puoi piazzare una sola trappola alla volta.",
    88: "Scorciatoie da Tastiera",
    91: "  I seguenti tasti svolgono funzioni diverse mentre giochi",
    92: "  a Lord of Verminion.",
    94: "  Barra azioni 2 - Slot 1-12: riga superiore dei minion",
    95: "  Barra azioni 3 - Slot 1: esegui azione",
    96: "  Barra azioni 3 - Slot 2: attiva trappola",
    97: "  Barra azioni 3 - Slot 3: ritira minion",
    100: "  Guida Minion: Elenco Battaglia",
}

MACRO_TEXT = {
    "Playing with a ": "Giocare con un ",
    "Selecting Minions": "Selezionare i Minion",
    "Selecting an individual minion": "Selezionare un Singolo Minion",
    "Select minion from party list": "Seleziona un minion dall'elenco del gruppo",
    "Selecting multiple minions": "Selezionare Più Minion",
    "Activate selection circle and select targeted minions": "Attiva il cerchio di selezione e scegli i minion",
    "Useful Commands": "Comandi Utili",
    "General": "Generali",
    "Switch summoning gate": "Cambia varco di evocazione",
    "Toggle minion party list display": "Mostra/Nascondi l'elenco dei minion del gruppo",
    "Select summoning queue": "Seleziona la coda di evocazione",
    "Execute special action": "Esegui azione speciale",
    "Move ground target pointer": "Muovi il cursore del bersaglio a terra",
    "Reset ground target pointer": "Reimposta il cursore del bersaglio a terra",
    "Display help": "Mostra guida",
    "Minion Selection": "Selezione dei Minion",
    "Select all minions near currently selected minion": "Seleziona tutti i minion vicini a quello selezionato",
    "Select all nearby minions of same variety as currently": "Seleziona i minion vicini dello stesso tipo di quello",
    "      selected minion": "      selezionato",
    "Select all minions of same variety as currently selected": "Seleziona tutti i minion dello stesso tipo di quello",
    "      minion": "      selezionato",
    "Minimap Controls": "Comandi della Minimappa",
    " while pressed : Record map jump point (release button to jump)":
        " tenuto premuto: segna il punto di salto (rilascia per saltare)",
    "Map jump point selection": "Seleziona punto di salto sulla mappa",
    "Restore map jump point defaults": "Ripristina i punti di salto predefiniti",
    "Mouse Controls": "Comandi del Mouse",
    "Left-Click : Select individual minion": "Clic sinistro: seleziona un singolo minion",
    "Left-clicking on your currently selected minion will select all":
        "Un clic sinistro sul minion selezionato sceglie tutti i",
    "nearby minions of the same variety.": "minion vicini dello stesso tipo.",
    "Left-Click + Drag : Select all minions in the selection square":
        "Clic sinistro e trascina: seleziona i minion nel riquadro",
}


def translate_main_macro(tag):
    raw = bytes.fromhex(TAG.fullmatch(tag).group(1))
    assert raw[:3] == b"\x02\x08\xf2" and raw[-1:] == b"\x03"
    assert len(raw) == int.from_bytes(raw[3:5], "big") + 6
    body = raw[5:-1]
    assert body[:4] == b"\xe1\xe9Q\x01"
    branches = []
    pos = 4
    for _ in range(2):
        assert body[pos:pos + 2] == b"\xff\xf2"
        size = int.from_bytes(body[pos + 2:pos + 4], "big")
        branch = body[pos + 4:pos + 4 + size]
        assert len(branch) == size and b"\xff" not in branch
        for old, new in MACRO_TEXT.items():
            branch = branch.replace(old.encode("utf-8"), new.encode("utf-8"))
        branches.append(branch)
        pos += 4 + size
    assert pos == len(body)
    new_body = body[:4] + b"".join(
        b"\xff\xf2" + len(branch).to_bytes(2, "big") + branch for branch in branches
    )
    result = b"\x02\x08\xf2" + len(new_body).to_bytes(2, "big") + new_body + b"\x03"
    assert len(result) == int.from_bytes(result[3:5], "big") + 6
    return f"<hex:{result.hex().upper()}>"


def translate_fragment(tag, old, new, same_size=False):
    raw = bytes.fromhex(TAG.fullmatch(tag).group(1))
    old_bytes, new_bytes = old.encode("utf-8"), new.encode("utf-8")
    assert raw.count(old_bytes) == 1
    if same_size:
        assert len(old_bytes) == len(new_bytes)
    return f"<hex:{raw.replace(old_bytes, new_bytes).hex().upper()}>"


def translate_row(original):
    parts = TAG.split(original)
    assert len(parts) == 201
    tags = [f"<hex:{value}>" for value in parts[1::2]]
    for index, segment in enumerate(parts[::2]):
        if segment:
            assert index in SEGMENTS, ("missing segment", index, segment)
        elif index in SEGMENTS:
            raise AssertionError(("unexpected segment", index))
    assert len(SEGMENTS) == sum(bool(segment) for segment in parts[::2])
    tags[84] = translate_main_macro(tags[84])
    tags[93] = translate_fragment(
        tags[93], "  Hotbar 1 - Slots 1 to 12 : Minion hotbar bottom row",
        "  Barra azioni 1 - Slot 1-12: riga minion inferiore  ", True,
    )
    tags[98] = translate_fragment(
        tags[98], "  Draw/Sheathe Weapon : Switch summoning gate",
        "  Estrai/Riponi arma: cambia varco di evocazione",
    )
    output = []
    for i, segment in enumerate(parts[::2]):
        output.append(SEGMENTS.get(i, ""))
        if i < len(tags):
            output.append(tags[i])
    return "".join(output)


def main():
    rows = json.loads(PATH.read_text(encoding="utf-8"))
    rows["187"]["translation"] = translate_row(rows["187"]["original"])
    PATH.write_text(json.dumps(rows, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


if __name__ == "__main__":
    main()
