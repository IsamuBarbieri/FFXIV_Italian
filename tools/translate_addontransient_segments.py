"""Translate complete AddonTransient control hints while preserving control tags."""

import collections
import json
import pathlib
import re
from translate_hex_literals import replace_literals, replace_nested_literal

ROOT = pathlib.Path(__file__).resolve().parents[1] / "data" / "translations"
TAG = re.compile(r"(<hex:[0-9A-Fa-f]+>)")
ENGLISH_IN_HEX = re.compile(rb"[A-Za-z][A-Za-z '\-]{3,}")

MANUAL = {
    "Subcommands": "Sottocomandi", "Subcommand": "Sottocomando",
    "サブコマンド": "Sottocomandi", "Scroll": "Scorri",
    "Move": "Sposta", "Rotate": "Ruota", "Zoom": "Zoom",
    "Zoom In/Out": "Ingrandisci/Riduci", "Switch Type": "Cambia Tipo",
    "Toggle Floor": "Cambia Piano", "Toggle Map": "Mostra/Nascondi Mappa",
    "Map Link": "Collegamento Mappa", "Teleport": "Teletrasporto",
    "Prioritize/Hide in Duty List": "Dai Priorità/Nascondi nella Lista Incarichi",
    "Paste": "Incolla", "Delete": "Elimina", "History": "Cronologia",
    "Auto-translate": "Traduzione Automatica", "Change Page": "Cambia Pagina",
    "Turn Pages": "Sfoglia", "Return": "Ritorna", "Back": "Indietro",
    "Confirm": "Conferma", "Cancel": "Annulla", "Close": "Chiudi",
    "Select": "Seleziona", "View Details": "Mostra Dettagli",
    "View Search Results": "Mostra Risultati", "Reload List": "Ricarica Lista",
    "Search": "Cerca", "Cycle Between Roles": "Scorri i Ruoli",
    "Specify Class/Job": "Specifica Classe/Job",
    "Display Vista Record": "Mostra Registro Panorami",
    "Display Impressions": "Mostra Impressioni",
    "Retrieve Online Status": "Aggiorna Stato Online",
    "Inventory": "Inventario", "Open Inventory": "Apri Inventario",
    "Change Category": "Cambia Categoria", "Set to Hotbar": "Assegna alla Barra Azioni",
    "Select Item": "Seleziona Oggetto", "Toggle Phase": "Cambia Fase",
    "Commence Voyage": "Inizia Viaggio", "Register": "Registra",
    "Apply Changes and Close": "Applica Modifiche e Chiudi",
    "Select Category": "Seleziona Categoria", "Listen": "Ascolta",
    "Set Roll": "Imposta Brano", "Stop": "Ferma", "Play": "Riproduci",
    "Summon": "Evoca", "Summon/Dismiss": "Evoca/Congeda",
    "Edit Minion Hotbar": "Modifica Barra Azioni Minion",
    "Buyback": "Riacquista", "Change Order": "Cambia Ordine",
    "Change Tier": "Cambia Grado", "Change Order/Tier": "Cambia Ordine/Grado",
    "Turn On/Off": "Attiva/Disattiva", "Apply": "Applica",
    "Gear Subcommands": "Sottocomandi Equipaggiamento",
    "Equipment Subcommands": "Sottocomandi Equipaggiamento",
    "Begin": "Inizia", "Toggle Lists": "Mostra/Nascondi Liste",
    "Change Role Order": "Cambia Ordine Ruoli", "Examine": "Esamina",
    "Toggle Camera": "Cambia Visuale", "View Menu": "Mostra Menu",
    "Move to Room": "Vai alla Stanza", "View Apartment Details": "Mostra Dettagli Appartamento",
    "Toggle Environment": "Cambia Ambiente",
    "Add": "Aggiungi", "Register in Additional PvP Actions": "Registra nelle Azioni PvP Aggiuntive",
    "Add PvP Trait": "Aggiungi Tratto PvP", "Give Up": "Rinuncia",
    "Edit Squadron": "Modifica Squadrone", "Start/Abandon": "Inizia/Abbandona",
    "Start/Retry": "Inizia/Riprova", "Affix Magicite": "Incastona Magicite",
    "Remove Magicite": "Rimuovi Magicite", "Change Target": "Cambia Bersaglio",
    "Return to Plate": "Torna alla Scheda", "Check All On/Off": "Seleziona/Deseleziona Tutti",
    "Remove Item": "Rimuovi Oggetto", "Select Item to Donate": "Seleziona Oggetto da Donare",
    "Move Favorite": "Sposta Preferito", "Execute": "Esegui",
    "New Letter": "Nuova Lettera", "Accept": "Accetta",
    "Release Minion": "Rilascia Minion", "Duty Finder": "Ricerca Incarichi",
    "Duty Support": "Supporto Incarichi", "Duty": "Incarico",
    "Previous": "Precedente", "Next": "Successivo",
    "Previous Location": "Posizione Precedente", "Return to List": "Torna alla Lista",
    "Return to Home World": "Torna al Mondo d'Origine",
    "Execute Action": "Esegui Azione", "Select Logogram": "Seleziona Logogramma",
    "View Tag": "Mostra Etichetta", "Add/Remove Party Member": "Aggiungi/Rimuovi Membro del Gruppo",
    "Toggle On/Off": "Attiva/Disattiva", "Toggle Others On/Off": "Mostra/Nascondi Altri",
    "Filter": "Filtro", "Enable/Disable Quick Gathering": "Attiva/Disattiva Raccolta Rapida",
    "Use": "Usa", "Manage Lost Finds Cache": "Gestisci Deposito Reperti Perduti",
    "Display Map": "Mostra Mappa", "Select Class": "Seleziona Classe",
    "Execute Search": "Avvia Ricerca", "Preview": "Anteprima",
    "Claim Reward": "Ritira Ricompensa", "Select Data Center": "Seleziona Data Center",
    "Details": "Dettagli", "Board Layout": "Disposizione Tabellone",
    "Scour": "Vaglia", "Collector's Standard": "Standard del Collezionista",
    "Display Minion List": "Mostra Elenco Minion", "Embark": "Parti",
    "Close Confirmation": "Chiudi Conferma", "Add to Active Actions": "Aggiungi alle Azioni Attive",
    "Assign New Slot": "Assegna Nuovo Slot", "Set": "Imposta",
    "Manipulate": "Manipola", "Logos Actions": "Azioni Logos",
    "Edit Hotbar": "Modifica Barra Azioni", "Toggle Stage Info": "Mostra/Nascondi Info Scena",
    "Change Calls": "Cambia Chiamate", "Toggle Support Features": "Attiva/Disattiva Supporto",
    "Toggle Act Info": "Mostra/Nascondi Info Atto",
    "Confirm Enemy's Vulnerabilities": "Conferma Debolezze Nemiche",
    "To Details": "Vai ai Dettagli", "Return to Toy Chest": "Torna al Baule dei Giochi",
    "Turn Panel": "Ruota Pannello",
    "Move to Lost Finds Holster": "Sposta nella Fondina dei Reperti Perduti",
    "Manage Lost Finds Holster": "Gestisci Fondina dei Reperti Perduti",
    "Move to Lost Finds Cache": "Sposta nel Deposito Reperti Perduti",
    "Recommended": "Consigliato", "Quick Scroll": "Scorrimento Rapido",
    "Select Destination": "Seleziona Destinazione", "Navigate Map": "Naviga nella Mappa",
    "Toggle Role or Single Class/Job": "Passa da Ruolo a Classe/Job",
    "Toggle Area or Region": "Passa da Area a Regione",
    "Select and Close": "Seleziona e Chiudi", "Confirm and Register": "Conferma e Registrati",
    "Party Selection": "Selezione Gruppo", "Change/Remove Members": "Cambia/Rimuovi Membri",
    "End Selection": "Termina Selezione", "Glamours": "Glamour",
    "Avatar Restrictions": "Restrizioni degli Avatar", "Entering Dungeons": "Accesso alle Spedizioni",
    "Commencing Duty": "Avvio dell'Incarico", "What Next?": "Cosa Fare Ora?",
    "Duty Registration": "Iscrizione all'Incarico", "Duty Status": "Stato dell'Incarico",
    "Playback Controls": "Comandi di Riproduzione", "Open": "Apri",
    "Multiple Select": "Selezione Multipla", "Commend Player": "Assegna un Encomio",
    "Finish Placing Furnishing Glamours": "Termina la Disposizione dei Glamour degli Arredi",
    "Registered Glamours List": "Elenco Glamour Registrati",
    "Synthesize": "Sintetizza", "複数選択": "Selezione Multipla",
    "移動": "Sposta", "拡縮": "Ridimensiona", "回転": "Ruota",
    "カーソルの移動": "Sposta Cursore",
    "General Tab": "Scheda Generale", "Message Board Tab": "Bacheca Messaggi",
    "Members Tab": "Scheda Membri", "Other Functions": "Altre Funzioni",
    "Important Information": "Informazioni Importanti",
    "The Collectables Interface": "Interfaccia dei Collezionabili",
    "Increasing Collectability": "Aumento della Collezionabilità",
    "Collectable Actions": "Azioni per Collezionabili",
    "Improving Collectability": "Migliorare la Collezionabilità",
    "Other Actions": "Altre Azioni", "Entering V&C Dungeons": "Accesso alle Spedizioni V&C",
    "Variant Dungeons": "Spedizioni Varianti",
    "Unraveling the Story": "Scoprire la Storia",
    "Criterion Dungeons": "Spedizioni Criterio",
    "Criterion Dungeon Restrictions": "Restrizioni delle Spedizioni Criterio",
    "Estate Holder Rights": "Diritti dei Proprietari",
    "Rightsholders": "Titolari dei Diritti",
    "Blacklist Referencing": "Riferimento alla Lista Nera",
    "Exceptions": "Eccezioni", "Increase Cursor Speed": "Aumenta Velocità Cursore",
    "Open/Close Preview": "Apri/Chiudi Anteprima",
    "Report could not be sent.": "Impossibile inviare la segnalazione.",
    "Please try again.": "Riprova.",
}


def main():
    known = collections.defaultdict(set)
    for relative in ("system/addon.json", "system/howto.json", "system/lobby.json"):
        for row in json.loads((ROOT / relative).read_text(encoding="utf-8")).values():
            if isinstance(row, dict) and row.get("original") and row.get("translation"):
                known[row["original"]].add(row["translation"])
    exact = {text: next(iter(values)) for text, values in known.items() if len(values) == 1}
    exact.update(MANUAL)

    path = ROOT / "misc" / "addontransient.json"
    rows = json.loads(path.read_text(encoding="utf-8"))
    changed = 0
    grid_rows = {"482", "486", "490", "491"}
    nested_rows = {
        "139": ("Toggle Storeroom", "Mostra/Nascondi Magazzino"),
        "484": ("Toggle Registered/Placed Glamours",
                "Mostra/Nascondi Glamour Registrati/Posizionati"),
    }
    grid_literals = {
        "Enable Grid Snap": "Attiva Aggancio alla Griglia",
        "Disable Grid Snap": "Disattiva Aggancio alla Griglia",
    }
    for row_id, row in rows.items():
        if row["translation"]:
            continue
        original = row["original"]
        if row_id not in grid_rows | nested_rows.keys() and any(
            ENGLISH_IN_HEX.search(bytes.fromhex(tag[5:-1])) for tag in TAG.findall(original)
        ):
            continue
        parts = TAG.split(original)
        translated = []
        for part in parts:
            if TAG.fullmatch(part):
                if row_id in grid_rows and "456E61626C65204772696420536E6170" in part:
                    part = replace_literals(part, grid_literals)
                elif row_id in nested_rows and nested_rows[row_id][0].encode().hex().upper() in part:
                    part = replace_nested_literal(part, *nested_rows[row_id])
                translated.append(part)
                continue
            match = re.fullmatch(r"(\s*)(.*?)(\s*)", part, re.DOTALL)
            assert match
            before, core, after = match.groups()
            if core and core not in exact and core not in ("+", "＋", ".", "/", ":", "'"):
                break
            translated.append(before + exact.get(core, core) + after)
        else:
            row["translation"] = "".join(translated)
            changed += 1
    path.write_text(json.dumps(rows, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"Translated {changed} complete AddonTransient rows")


if __name__ == "__main__":
    main()
