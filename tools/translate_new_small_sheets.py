"""Apply reviewed translations for the new short UI sheets."""

import json
import pathlib
import re

ROOT = pathlib.Path(__file__).resolve().parents[1] / "data" / "translations" / "misc"

ACTION_NAMES = {
    3: "Famiglio", 13: "Pesca di base", 14: "Preparazione",
    15: "Aggancio ed Esche", 18: "Progresso", 20: "Supporto alla Fabbricazione",
}
ACTION_DESCRIPTIONS = {
    6: "<hex:024804F201FA03><hex:024904F201FB03>Per eseguire queste azioni, o applicarne gli effetti aggiuntivi, è necessaria una particolare posizione di combattimento.<hex:0249020103><hex:0248020103>",
    11: "Azioni specifiche per i collezionabili.",
    14: "Azioni da usare prima di lanciare la lenza.",
    15: "Azioni da usare dopo aver lanciato la lenza.",
    18: "Azioni che aumentano il progresso o l'efficienza.",
    19: "Azioni che aumentano la qualità o l'efficienza.",
    20: "Azioni supplementari che influiscono sulla durabilità o sui PC.",
    21: "Azioni disponibili quando si equipaggia il cristallo dell'anima di un artigiano.",
}

CATEGORY_TEXT = {
    "All Classes": "Tutte le Classi",
    "Disciple of War": "Discepolo della Guerra",
    "Disciple of Magic": "Discepolo della Magia",
    "Disciple of the Land": "Discepolo della Terra",
    "Disciple of the Hand": "Discepolo della Mano",
    "Disciples of War or Magic": "Discepoli della Guerra o della Magia",
    "Disciples of the Land or Hand": "Discepoli della Terra o della Mano",
    "Any Disciple of War (excluding gladiators)": "Qualsiasi Discepolo della Guerra (esclusi i Gladiatori)",
    "Any Disciple of the Hand (excluding culinarians)": "Qualsiasi Discepolo della Mano (esclusi i Cuochi)",
    "Jobs of the Disciples of War or Magic": "Job dei Discepoli della Guerra o della Magia",
    "All classes and jobs (excluding limited jobs)": "Tutte le classi e i job (esclusi i job limitati)",
    "Any Disciple of War or Magic (excluding limited jobs)": "Qualsiasi Discepolo della Guerra o della Magia (esclusi i job limitati)",
    "Disciples of War (excluding limited jobs)": "Discepoli della Guerra (esclusi i job limitati)",
    "Any Disciple of Magic (excluding limited jobs)": "Qualsiasi Discepolo della Magia (esclusi i job limitati)",
    "Any job of the Disciples of War or Magic (excluding limited jobs)": "Qualsiasi job dei Discepoli della Guerra o della Magia (esclusi i job limitati)",
    "Tank (excluding limited jobs)": "Difensori (esclusi i job limitati)",
    "Healer (excluding limited jobs)": "Curatori (esclusi i job limitati)",
    "Physical DPS (excluding limited jobs)": "DPS fisici (esclusi i job limitati)",
    "Melee DPS (excluding limited jobs)": "DPS da mischia (esclusi i job limitati)",
    "Physical Ranged DPS (excluding limited jobs)": "DPS fisici a distanza (esclusi i job limitati)",
    "Magical Ranged DPS (excluding limited jobs)": "DPS magici a distanza (esclusi i job limitati)",
}

JOB_ABBREVIATIONS = {
    "GLA": "GLD", "PGL": "PGL", "MRD": "INC", "LNC": "LNC", "ARC": "ARC",
    "CNJ": "INT", "THM": "TMR", "CRP": "FLG", "BSM": "FBR", "ARM": "ARM",
    "GSM": "ORF", "LTW": "NCT", "WVR": "TST", "ALC": "ALC", "CUL": "CUC",
    "MIN": "MNT", "BTN": "BTN", "FSH": "PSC", "PLD": "PLD", "MNK": "MNC",
    "WAR": "GUE", "DRG": "DRG", "BRD": "BRD", "WHM": "MBN", "BLM": "MNR",
    "ACN": "ACN", "SMN": "EVC", "SCH": "STD", "ROG": "FRT", "NIN": "NJA",
    "MCH": "ART", "DRK": "CVS", "AST": "AST", "SAM": "SMR", "RDM": "MGR",
    "BLU": "MBL", "GNB": "ETR", "DNC": "DNZ", "RPR": "MTR", "SGE": "SGO",
    "VPR": "VPR", "PCT": "PTM", "BST": "DMT",
}

ITEM_SEARCH = {
    1: "Armi principali", 2: "Attrezzi principali", 3: "Attrezzi principali",
    4: "Armature", 5: "Accessori", 6: "Medicinali", 7: "Materiali",
    8: "Altro", 9: "Armi da Pugile", 10: "Armi da Gladiatore",
    11: "Armi da Predatore", 12: "Armi da Arciere", 13: "Armi da Lanciere",
    14: "Armi da Taumaturgo", 15: "Armi da Elementalista",
    16: "Armi da Arcanista", 17: "Scudi", 18: "Armi da Danzatore",
    19: "Attrezzi da Falegname", 20: "Attrezzi da Fabbro",
    21: "Attrezzi da Armaiolo", 22: "Attrezzi da Orefice",
    23: "Attrezzi da Conciatore", 24: "Attrezzi da Tessitore",
    25: "Attrezzi da Alchimista", 26: "Attrezzi da Cuoco",
    27: "Attrezzi da Minatore", 28: "Attrezzi da Botanico",
    29: "Attrezzi da Pescatore", 30: "Attrezzatura da pesca",
    31: "Testa", 32: "Sottovesti", 33: "Busto", 34: "Indumenti intimi",
    35: "Gambe", 36: "Mani", 37: "Piedi", 38: "Vita",
    39: "Collane", 40: "Orecchini", 41: "Bracciali", 42: "Anelli",
    43: "Medicinali", 44: "Ingredienti", 45: "Pietanze", 46: "Prodotti ittici",
    47: "Pietre", 48: "Metalli", 49: "Legname", 50: "Tessuti",
    51: "Pelli", 52: "Ossa", 53: "Reagenti", 54: "Tinture",
    55: "Componenti d'arma", 56: "Arredi", 57: "Materia",
    58: "Cristalli", 59: "Catalizzatori", 60: "Oggetti vari",
    61: "Cristalli dell'Anima", 62: "Frecce", 63: "Oggetti di Missione",
    64: "Altro", 65: "Elementi esterni", 66: "Elementi interni",
    67: "Arredi da esterno", 68: "Sedie e Letti", 69: "Tavoli",
    70: "Oggetti da tavolo", 71: "Oggetti da parete", 72: "Tappeti",
    73: "Armi da Furfante", 74: "Oggetti stagionali", 75: "Minion",
    76: "Armi da Cavaliere Oscuro", 77: "Armi da Artificiere",
    78: "Armi da Astrologo", 79: "Componenti di Aeronavi e Sommergibili",
    80: "Componenti dell'Orchestrion", 81: "Oggetti da giardinaggio",
    82: "Dipinti", 83: "Armi da Samurai", 84: "Armi da Mago Rosso",
    85: "Armi da Studioso", 86: "Armi da Eterlama",
    87: "Armi da Danzatore", 88: "Armi da Mietitore",
    89: "Armi da Saggio", 90: "Oggetti vari registrabili",
    91: "Armi da Vipera", 92: "Armi da Pittomante",
}

ITEM_SERIES = {
    1: "Uniforme della Maelstrom", 2: "Uniforme dell'Ordine della Vipera Gemella",
    3: "Uniforme delle Fiamme Immortali",
    4: "Abito da Signore dell'Estremo Oriente",
    5: "Abito da Dama dell'Estremo Oriente",
    6: "Abito da Patriarca dell'Estremo Oriente",
    7: "Abito da Matriarca dell'Estremo Oriente",
    8: "Abito da Viaggio dell'Estremo Oriente Ex.",
    9: "Uniforme della Cameriera Fedele", 10: "Uniforme del Maggiordomo Fedele",
    11: "Abito del Signore Nezha", 12: "Abito della Dama Nezha",
    13: "Abito da Nobile dell'Estremo Oriente", 14: "Abito Fuga",
    15: "Abito da Gentiluomo dell'Estremo Oriente",
    16: "Abito da Bellezza dell'Estremo Oriente",
    17: "Abito del Carbuncle di Smeraldo", 18: "Abito del Carbuncle di Topazio",
    19: "Abito Angelico", 20: "Abito Demoniaco",
    21: "Abito del Principe delle Fiabe", 22: "Abito della Principessa delle Fiabe",
    23: "Uniforme da Studente dell'Estremo Oriente",
    24: "Uniforme da Studentessa dell'Estremo Oriente",
    25: "Abito di Abes", 26: "Abito dell'Alto Evocatore",
    27: "Abito del Carbuncle di Rubino",
    28: "Accessori del Santo della Mano", 29: "Accessori del Santo della Terra",
    30: "Accessori Occulti",
}

DESCRIPTION_NAMES = {
    3604482: "Regole del Mahjong", 3604483: "Regole di Rival Wings",
    3604484: "Informazioni sugli Alloggi", 3604485: "Alla scoperta di New Game+",
    3604486: "Regole di Frontline", 3604487: "Pesca Oceanica",
    3604488: "Faux Hollows", 3604489: "Il Fronte Meridionale di Bozja",
    3604490: "Grado della Resistenza", 3604491: "Azioni Perdute",
    3604492: "Grappoli di Bozja",
    3604493: "Tornei di Triple Triad: Formati Brevi",
    3604496: "Onorificenze della Resistenza", 3604497: "Zadnor",
    3604498: "PvP", 3604499: "Conflitto Cristallino",
    3604500: "Cripte Profonde", 3604501: "Cripte Profonde",
    3604502: "Cripte Profonde", 3604503: "Chiacchiere",
    3604504: "Padiglione del Novizio", 3604505: "Cripte Profonde",
    3604507: "La Falce Occulta", 3604508: "Pulizie con gli Spriggan",
    3604509: "Consegne Personalizzate", 3604510: "Picchiaduro da Tastiera",
    3604511: "Crogiolo degli Indomiti",
}
DESCRIPTION_SUBTITLES = {
    3604482: "Regole del Mahjong", 3604483: "Rival Wings",
    3604484: "Alloggi", 3604485: "New Game+",
    3604486: "Regole di Frontline", 3604487: "Pesca Oceanica",
    3604488: "Faux Hollows", 3604489: "Il Fronte Meridionale di Bozja",
    3604490: "Grado della Resistenza", 3604491: "Azioni Perdute",
    3604492: "Grappoli di Bozja",
    3604493: "Tornei di Triple Triad: Formati Brevi",
    3604496: "Onorificenze della Resistenza", 3604497: "Zadnor",
    3604498: "PvP", 3604499: "Conflitto Cristallino",
    3604500: "Cripte Profonde", 3604501: "Cripte Profonde",
    3604502: "Cripte Profonde", 3604503: "Blunderville",
    3604504: "Guida al Combattimento", 3604505: "Cripte Profonde",
    3604506: "Esplorazione Cosmica", 3604507: "La Falce Occulta",
    3604508: "Pulizie con gli Spriggan",
    3604509: "Consegne Personalizzate", 3604510: "Picchiaduro da Tastiera",
    3604511: "Crogiolo degli Indomiti",
}

SPECIAL_BONUS = {
    2: "Bonus del Set: ", 4: "Patrocinio: ",
    6: "Bonus del Set (massimo): ", 7: "Effetto di Eureka: ",
    8: "Effetto nell'area di Save the Queen:",
    9: "Effetto della Falce Occulta:",
    10: "Bonus del Set della Falce Occulta: ",
    11: "Effetto del Crogiolo:",
}


def save(name, rows):
    path = ROOT / f"{name}.json"
    path.write_text(json.dumps(rows, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def main():
    path = ROOT / "classjobactionuicategory.json"
    rows = json.loads(path.read_text(encoding="utf-8"))
    for key, row in rows.items():
        row["translation_name"] = ACTION_NAMES.get(int(key), row["translation_name"])
        row["translation_description"] = ACTION_DESCRIPTIONS.get(int(key), row["translation_description"])
    save("classjobactionuicategory", rows)

    path = ROOT / "classjobcategory.json"
    rows = json.loads(path.read_text(encoding="utf-8"))
    for row in rows.values():
        original = row["original"]
        if re.fullmatch(r"[A-Z]{3}(?:[ ,]+[A-Z]{3})*", original):
            row["translation"] = re.sub(r"[A-Z]{3}", lambda m: JOB_ABBREVIATIONS[m.group()], original)
        elif original in CATEGORY_TEXT:
            row["translation"] = CATEGORY_TEXT[original]
        elif not row["translation"]:
            raise ValueError(f"Untranslated ClassJobCategory: {original}")
    save("classjobcategory", rows)

    for name, mapping, field in (("itemsearchcategory", ITEM_SEARCH, "translation"),
                                 ("itemseries", ITEM_SERIES, "translation"),
                                 ("description", DESCRIPTION_NAMES, "translation_name")):
        path = ROOT / f"{name}.json"
        rows = json.loads(path.read_text(encoding="utf-8"))
        for key, row in rows.items():
            if int(key) in mapping:
                row[field] = mapping[int(key)]
        save(name, rows)

    path = ROOT / "description.json"
    rows = json.loads(path.read_text(encoding="utf-8"))
    for key, row in rows.items():
        row["translation_description"] = DESCRIPTION_SUBTITLES[int(key)]
        if int(key) == 3604485:  # つよくてニューゲーム解説
            row["translation_col_2"] = "Guida a New Game+"
    save("description", rows)

    path = ROOT / "itemspecialbonus.json"
    rows = json.loads(path.read_text(encoding="utf-8"))
    for key, row in rows.items():
        row["translation_name"] = SPECIAL_BONUS[int(key)]
    source = "020814E4E80202FF065069656365FF0750696563657303"
    encoded = bytes.fromhex(source)
    translated = encoded.replace(b"\xff\x06Piece", b"\xff\x06Pezzo").replace(
        b"\xff\x07Pieces", b"\xff\x06Pezzi")
    translated = translated[:2] + bytes([len(translated) - 4 + 1]) + translated[3:]
    assert translated[0:2] == b"\x02\x08" and translated[-1:] == b"\x03"
    assert len(translated) == translated[2] + 3
    tag = f"<hex:{translated.hex().upper()}>"
    rows["10"]["translation_description"] = (
        "Richiede <hex:022003E80203> " + tag + " del Set"
    )
    save("itemspecialbonus", rows)


if __name__ == "__main__":
    main()
