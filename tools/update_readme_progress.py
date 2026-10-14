"""Render the README progress graphic from the extractor's existing row counts."""

import argparse
from datetime import date
from html import escape
from pathlib import Path
import re
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
TOTAL = re.compile(r"^TOTALE PARZIALE/COMPLESSIVO\s+([\d.,]+)\s+([\d.,]+)\s+([\d.,]+)\s", re.MULTILINE)
QUEST = re.compile(r"^Quests (\w+)\s+\([\d.,]+ missioni\)\s+([\d.,]+)\s+([\d.,]+)\s+([\d.,]+)\s", re.MULTILINE)


def counts(values):
    total, complete, pending = (int(value.replace(".", "").replace(",", "")) for value in values)
    if complete + pending != total:
        raise ValueError("Conteggi incoerenti: complete + pendenti deve coincidere con il totale")
    return total, complete


def parse_status(output):
    match = TOTAL.search(output)
    if not match:
        raise ValueError("Totale assente: usare l'output di status senza filtri")
    total, complete = counts(match.groups())
    quests = {key: counts(values) for key, *values in QUEST.findall(output)}
    if set(quests) != {key for key, _, _ in EXPANSIONS}:
        raise ValueError("Espansioni mancanti o nuove: verificare il catalogo prima di pubblicare il grafico")
    shared = total - sum(value[0] for value in quests.values()), complete - sum(value[1] for value in quests.values())
    if not 0 <= shared[1] <= shared[0]:
        raise ValueError("Totale dei contenuti condivisi incoerente")
    return total, complete, shared, quests


def number(value):
    return f"{value:,}".replace(",", ".")


def percentage(total, complete):
    return f"{100 * complete / total if total else 0:.1f}".replace(".", ",") + "%"


def render(output, updated):
    total, complete, shared, quests = parse_status(output)
    parts = [f'''<svg xmlns="http://www.w3.org/2000/svg" width="900" height="782" viewBox="0 0 900 782" role="img" aria-labelledby="title description">
<title id="title">Avanzamento FFXIV Italiano: {number(complete)} righe complete su {number(total)}, {percentage(total, complete)}</title>
<desc id="description">Conteggi del corpus al {updated.isoformat()}. Le quest sono raggruppate per espansione; tutti gli altri fogli sono nei contenuti condivisi. Una riga è completa quando tutti i campi previsti sono compilati.</desc>
<defs><linearGradient id="background" x2="1" y2="1"><stop stop-color="#07131f"/><stop offset="1" stop-color="#102735"/></linearGradient>
<linearGradient id="aether"><stop stop-color="#429dba"/><stop offset="1" stop-color="#8de1cb"/></linearGradient>
<linearGradient id="gold"><stop stop-color="#927b48"/><stop offset="1" stop-color="#dcc692"/></linearGradient></defs>
<rect x="1" y="1" width="898" height="780" rx="20" fill="url(#background)" stroke="#54626b"/>
<path d="M32 24H406M494 24H868" stroke="#bca575" opacity=".65"/>
<path d="M450 13l10 11-10 11-10-11z" fill="none" stroke="#d4be91"/>
<g font-family="Segoe UI,Arial,sans-serif" fill="#e5edf0">
<text x="36" y="63" fill="#cfbb91" font-size="13" letter-spacing="3">LA LOCALIZZAZIONE, IN NUMERI</text>
<text x="864" y="63" text-anchor="end" fill="#91a8b5" font-size="12">{updated.strftime('%d.%m.%Y')}</text>
<text x="36" y="139" font-size="65" font-weight="600" fill="#9be5d4">{percentage(total, complete)}</text>
<text x="294" y="112" font-size="23" font-weight="600">{number(complete)} / {number(total)}</text>
<text x="294" y="140" font-size="14" fill="#9db4c1">righe complete nel corpus</text>
<rect x="36" y="163" width="828" height="12" rx="6" fill="#233d4c"/>''']
    if total and complete:
        parts.append(f'<rect x="36" y="163" width="{828 * complete / total:.2f}" height="12" rx="6" fill="url(#aether)"/>')

    def row(y, label, values, color):
        row_total, row_complete = values
        parts.append(f'''<rect x="24" y="{y}" width="852" height="59" rx="10" fill="#152c3b" opacity=".7"/>
<path d="M53 {y+13}l10 13-10 13-10-13zM53 {y+13}v26M43 {y+26}h20" fill="none" stroke="{color}" stroke-width="1.4"/>
<text x="83" y="{y+24}" font-size="16" fill="#e5edf0">{escape(label)}</text>
<text x="840" y="{y+24}" text-anchor="end" font-size="14" fill="{color}">{number(row_complete)} / {number(row_total)} · {percentage(row_total, row_complete)}</text>
<rect x="83" y="{y+37}" width="757" height="5" rx="2.5" fill="#294555"/>''')
        if row_complete:
            parts.append(f'<rect x="83" y="{y+37}" width="{757 * row_complete / row_total:.2f}" height="5" rx="2.5" fill="{color}"/>')

    parts.append('<text x="36" y="215" font-size="12" fill="#cfbb91" letter-spacing="2">CONTENUTI CONDIVISI</text>')
    row(228, "Interfaccia, sistemi e altri fogli", shared, "#8dd9c8")
    parts.append('<text x="36" y="309" font-size="12" fill="#91a8b5">Include anche dialoghi e testi non appartenenti ai fogli delle quest.</text>')
    parts.append('<text x="36" y="350" font-size="12" fill="#cfbb91" letter-spacing="2">QUEST NARRATIVE · PER ESPANSIONE</text>')
    for index, (key, label, color) in enumerate(EXPANSIONS):
        row(363 + index * 62, label, quests[key], color)
    parts.append('''<path d="M36 744H864" stroke="#536373" opacity=".7"/>
<text x="36" y="768" font-size="11" fill="#91a8b5">Avanzamento editoriale del corpus · Il pacchetto include soltanto le traduzioni approvate.</text>
</g></svg>''')
    return "\n".join(parts) + "\n"


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--status-file", type=Path, help="Output già salvato del comando status, senza filtri")
    parser.add_argument("--date", type=date.fromisoformat, default=date.today())
    args = parser.parse_args()
    output = args.status_file.read_text(encoding="utf-8-sig") if args.status_file else subprocess.run(
        ["dotnet", "run", "--project", "src/FFXIVItalian.Extractor", "--", "status"],
        cwd=ROOT, capture_output=True, text=True, encoding="utf-8", check=True,
    ).stdout
    destination = ROOT / "docs" / "assets" / "progress.svg"
    destination.parent.mkdir(parents=True, exist_ok=True)
    destination.write_text(render(output, args.date), encoding="utf-8")
    print(f"Grafico aggiornato: {destination}")


if __name__ == "__main__":
    main()
