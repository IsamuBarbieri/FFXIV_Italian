"""Render the release 1.0 progress graphic and scope report from extractor counts."""

import argparse
from datetime import date
from html import escape
from pathlib import Path
import json
import subprocess


ROOT = Path(__file__).resolve().parents[1]
EXPANSIONS = (
    ("ARR", "A Realm Reborn", "#7cddc8"),
    ("HEAVENSWARD", "Heavensward", "#8fbfef"),
    ("STORMBLOOD", "Stormblood", "#ee8e86"),
    ("SHADOWBRINGERS", "Shadowbringers", "#bd9de6"),
    ("ENDWALKER", "Endwalker", "#e6ce92"),
    ("DAWNTRAIL", "Dawntrail", "#eab888"),
)
GROUPS = (
    ("core", "Interfaccia e contenuti condivisi"),
    ("dialogue", "Dialoghi e testi collegati"),
    ("cutscenes", "Cutscene e battute doppiate"),
    ("catalogs", "Titoli, diari e cataloghi delle missioni"),
)


def scope(path):
    parts = Path(path).as_posix().split("/")
    if len(parts) < 3 or parts[0] not in {"translations", "da_revisionare", "da_tradurre"}:
        raise ValueError(f"Percorso del corpus non valido: {path}")
    if parts[1:3] == ["dialogue", "cut_scene"]:
        return "cutscenes"
    if parts[1] == "dialogue":
        return "dialogue"
    if parts[1] == "quests":
        if len(parts) >= 5 and parts[-2].isdigit():
            key = parts[2].upper()
            if key not in {key for key, _, _ in EXPANSIONS}:
                raise ValueError(f"Espansione non riconosciuta: {path}")
            return key
        return "catalogs"
    return "core"


def parse_status(output):
    sheets = json.loads(output)
    groups = {key: [0, 0, 0] for key, *_ in (*GROUPS, *EXPANSIONS)}
    paths = set()
    for sheet in sheets:
        path = sheet["Path"]
        relative = path.split("/", 1)[1]
        if relative in paths:
            raise ValueError(f"Foglio presente in più stati editoriali: {relative}")
        paths.add(relative)
        stats = sheet["Stats"]
        total, complete, pending = (stats[key] for key in ("TotalRows", "TranslatedRows", "PendingRows"))
        if not all(type(value) is int and value >= 0 for value in (total, complete, pending)) or complete + pending != total:
            raise ValueError(f"Conteggi incoerenti: {path}")
        values = groups[scope(path)]
        values[0] += total
        values[1] += complete
        if path.startswith("translations/"):
            values[2] += complete
    return groups, sheets


def number(value):
    return f"{value:,}".replace(",", ".")


def percentage(total, complete):
    return f"{100 * complete / total if total else 0:.1f}".replace(".", ",") + "%"


def render(output, updated):
    groups, _ = parse_status(output)
    total = sum(values[0] for values in groups.values())
    approved = sum(values[2] for values in groups.values())
    parts = [f'''<svg xmlns="http://www.w3.org/2000/svg" width="900" height="1012" viewBox="0 0 900 1012" role="img" aria-labelledby="title description">
<title id="title">Avanzamento FFXIV Italiano: {number(approved)} righe approvate su {number(total)}, {percentage(total, approved)}</title>
<desc id="description">Conteggi del corpus estratto al {updated.isoformat()}. Interfaccia e contenuti condivisi separati da dialoghi, cutscene, cataloghi delle missioni e quest per espansione. Totale approvato: {number(approved)} su {number(total)} righe. La copertura del corpus non certifica tutto il client né la resa in gioco.</desc>
<defs><linearGradient id="background" x2="1" y2="1"><stop stop-color="#07131f"/><stop offset="1" stop-color="#102735"/></linearGradient>
<linearGradient id="aether"><stop stop-color="#429dba"/><stop offset="1" stop-color="#8de1cb"/></linearGradient></defs>
<rect x="1" y="1" width="898" height="1010" rx="20" fill="url(#background)" stroke="#54626b"/>
<path d="M32 24H406M494 24H868" stroke="#bca575" opacity=".65"/>
<path d="M450 13l10 11-10 11-10-11z" fill="none" stroke="#d4be91"/>
<g font-family="Segoe UI,Arial,sans-serif" fill="#e5edf0">
<text x="36" y="63" fill="#cfbb91" font-size="13" letter-spacing="3">LA LOCALIZZAZIONE, IN NUMERI</text>
<text x="864" y="63" text-anchor="end" fill="#91a8b5" font-size="12">{updated.strftime('%d.%m.%Y')}</text>
<text x="36" y="139" font-size="65" font-weight="600" fill="#9be5d4">{percentage(total, approved)}</text>
<text x="294" y="112" font-size="23" font-weight="600">{number(approved)} / {number(total)}</text>
<text x="294" y="140" font-size="14" fill="#9db4c1">righe approvate nel corpus estratto</text>
<rect x="36" y="163" width="828" height="12" rx="6" fill="#233d4c"/>
<rect x="36" y="163" width="{828 * approved / total if total else 0:.2f}" height="12" rx="6" fill="url(#aether)"/>''']

    def row(y, label, values, color):
        row_total, _, row_approved = values
        parts.append(f'''<rect x="24" y="{y}" width="852" height="59" rx="10" fill="#152c3b" opacity=".7"/>
<path d="M53 {y+13}l10 13-10 13-10-13zM53 {y+13}v26M43 {y+26}h20" fill="none" stroke="{color}" stroke-width="1.4"/>
<text x="83" y="{y+24}" font-size="15" fill="#e5edf0">{escape(label)}</text>
<text x="840" y="{y+24}" text-anchor="end" font-size="14" fill="{color}">{number(row_approved)} / {number(row_total)} · {percentage(row_total, row_approved)}</text>
<rect x="83" y="{y+37}" width="757" height="5" rx="2.5" fill="#294555"/>
<rect x="83" y="{y+37}" width="{757 * row_approved / row_total if row_total else 0:.2f}" height="5" rx="2.5" fill="{color}"/>''')

    parts.append('<text x="36" y="208" font-size="12" fill="#cfbb91" letter-spacing="2">CONTENUTI CONDIVISI</text>')
    row(222, GROUPS[0][1], groups[GROUPS[0][0]], "#8dd9c8")
    parts.append('<text x="36" y="306" font-size="12" fill="#cfbb91" letter-spacing="2">NARRAZIONE E TESTI COLLEGATI</text>')
    for index, (key, label) in enumerate(GROUPS[1:]):
        row(320 + index * 62, label, groups[key], "#91b8cb")
    parts.append('<text x="36" y="528" font-size="12" fill="#cfbb91" letter-spacing="2">QUEST NARRATIVE · PER ESPANSIONE</text>')
    for index, (key, label, color) in enumerate(EXPANSIONS):
        row(542 + index * 62, label, groups[key], color)
    parts.append(f'''<path d="M36 936H864" stroke="#536373" opacity=".7"/>
<text x="36" y="963" font-size="11" fill="#91a8b5">Ogni foglio è contato una volta. Le cutscene hanno una barra separata dalle quest.</text>
<text x="36" y="987" font-size="11" fill="#91a8b5">Bozze escluse dalle barre · La copertura del corpus non certifica l'intero client o la resa in gioco.</text>
</g></svg>''')
    return "\n".join(parts) + "\n"


def release_plan(output, updated):
    groups, sheets = parse_status(output)
    lines = ["# Obiettivo della versione 1.0", "",
        "La versione 1.0 punta a coprire interfaccia, sistemi, oggetti, combattimento, attività, artigianato, alloggi, negozi e contenuti condivisi. Missioni, titoli e diari delle missioni, dialoghi e cutscene hanno un avanzamento distinto; i dialoghi già approvati restano nel pacchetto.", "",
        "Le cutscene sono contenuti narrativi, ma non coincidono con i fogli delle quest: possono contenere anche battute di combattimento e contenuti collaterali. Per questo hanno una barra dedicata. Le cartelle numeriche dei Voiceman non vengono equiparate alle cartelle numeriche delle quest né usate per assegnare automaticamente un'espansione.", "",
        f"## Situazione del corpus al {updated.isoformat()}", "",
        "| Perimetro | Righe nel corpus | Coperte nei fogli approvati | Coperte in tutti gli stati |",
        "| --- | ---: | ---: | ---: |"]
    for key, label in GROUPS:
        total, complete, approved = groups[key]
        lines.append(f"| {label} | {number(total)} | {number(approved)} | {number(complete)} |")
    lines += ["", "Le righe coperte hanno compilati i campi testuali previsti; le colonne tecniche riconosciute (chiavi di configurazione, riferimenti a risorse ed effetti sonori) e i segnaposto verificati non richiedono una traduzione. Le regole sono in `BatchManager.IsTechnicalSourceField` e sono circoscritte ai fogli e ai campi verificati. Un payload SeString con testo visibile resta da tradurre anche se il testo esterno è vuoto.", "",
        "## Cosa manca prima della 1.0", "",
        "Il censimento seguente segnala righe da completare **o verificare**, non altrettante frasi certamente da tradurre. Sono compresi casi invarianti ancora da classificare, formule del registro, intervalli numerici e righe vuote. Non vanno riempiti con traduzioni artificiali per raggiungere il 100%.", "",
        "| Foglio | Righe da completare o verificare | Stato editoriale |", "| --- | ---: | --- |"]
    pending_count = 0
    for sheet in sorted(sheets, key=lambda sheet: sheet["Path"]):
        if scope(sheet["Path"]) != "core":
            continue
        stats = sheet["Stats"]
        pending = stats["PendingRows"] if sheet["Path"].startswith("translations/") else stats["TotalRows"]
        if pending:
            path = sheet["Path"]
            lines.append(f"| [{path.split('/', 1)[1]}](../data/{path}) | {number(pending)} | {path.split('/')[0]} |")
            pending_count += 1
    if not pending_count:
        lines.append("| *Nessun foglio pendente* | 0 | completato |")
    lines += ["", "Nei fogli elencati, dare priorità al testo visibile nei payload di `gatheringcondition`, `gatheringpointbonustype`, `hwdinfoboardarticle` e `configkey`. I testi dei pulsanti del controller e le alternative condizionali richiedono controllo di tutti i rami, non un'esclusione statistica.", "",
        "Questo grafico misura il corpus estratto, non tutti i fogli presenti nel client né le texture. Per la chiusura della versione servono anche il confronto con il catalogo del client, la verifica delle risorse grafiche, la build con controllo di copertura dei campi approvati e la verifica in gioco. Un conteggio completo non approva un file e non sostituisce la revisione editoriale.", "",
        "Snapshot e grafico sono rigenerati con `python tools/update_readme_progress.py`. Nessun file viene spostato o approvato da questo comando.", ""]
    return "\n".join(lines)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--status-file", type=Path, help="Output JSON già salvato del comando status --json")
    parser.add_argument("--date", type=date.fromisoformat, default=date.today())
    args = parser.parse_args()
    output = args.status_file.read_text(encoding="utf-8-sig") if args.status_file else subprocess.run(
        ["dotnet", "run", "--project", "src/FFXIVItalian.Extractor", "--", "status", "--json"],
        cwd=ROOT, capture_output=True, text=True, encoding="utf-8", check=True,
    ).stdout
    destination = ROOT / "docs" / "assets" / "progress.svg"
    destination.parent.mkdir(parents=True, exist_ok=True)
    destination.write_text(render(output, args.date), encoding="utf-8")
    (ROOT / "docs" / "RELEASE_1_0.md").write_text(release_plan(output, args.date), encoding="utf-8")
    print(f"Grafico e piano 1.0 aggiornati: {destination}")


if __name__ == "__main__":
    main()
