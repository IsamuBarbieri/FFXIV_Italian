"""Human-reviewed terminology queue. Never changes translations without approval."""

import argparse
import collections
import json
import os
from pathlib import Path
import re
import tempfile


ROOT = Path(__file__).resolve().parents[1]
TRANSLATIONS = ROOT / "data" / "translations"
GLOSSARY = ROOT / "data" / "glossary" / "Glossary.md"
PLACE_NAMES = ROOT / "data" / "translations" / "world" / "placename.json"
TAGS = re.compile(r"<[^>]+>|\{[^{}]+\}")
ELISION = re.compile(r"\b(il|lo|la|del|dello|della|al|allo|alla|nel|nello|nella|sul|sullo|sulla|dal|dallo|dalla)\s+([aeiouàèéìòù])", re.IGNORECASE)
KNOWN_GRAMMAR = (
    (re.compile(r"\bAvvio di un appello\b", re.IGNORECASE), "Avvio dell'appello", "avvio_ready_check"),
    (re.compile(r"\bdisponibile a Piazza dei Chocobo\b", re.IGNORECASE), "disponibile nella Piazza dei Chocobo", "preposizione_piazza_chocobo"),
    (re.compile(r"\bTane Sospette disponibili\b", re.IGNORECASE), "Tane Sospette Disponibili", "maiuscole_nome_composto"),
    (re.compile(r"\bTi diamo il benvenuto da Vari Splendori\b", re.IGNORECASE), "Ti diamo il benvenuto a Vari Splendori", "preposizione_vari_splendori"),
)
ROW = re.compile(r'^  "(?P<id>\d+)": \{\r?\n', re.MULTILINE)
FIELD = re.compile(r'(?m)^    "(?P<name>translation(?:_[^"\r\n]+)?)": (?P<value>"(?:\\.|[^"\\])*")(?=,?\r?$)')


def glossary():
    approved, entries = set(), []
    section = category = ""
    for line in GLOSSARY.read_text(encoding="utf-8-sig").splitlines():
        if line.startswith("## "):
            section = line[3:]
        elif line.startswith("### "):
            category = line[4:]
        elif section == "File approvati" and line.startswith("- `"):
            approved.add(line.split("`", 2)[1])
        elif section == "Voci" and line.startswith("| ") and not line.startswith(("| Inglese |", "| --- |")):
            cells = [cell.strip().strip("`") for cell in line.strip("|").split("|")]
            if len(cells) != 4:
                raise ValueError(f"Riga glossario non valida: {line}")
            source = cells[2].split("#", 1)[0]
            if source not in approved:
                raise ValueError(f"Fonte non approvata: {cells[2]}")
            entries.append(dict(english=cells[0], italian=cells[1], reference=cells[2], category=category, usage=cells[3]))
    if "world/placename.json" in approved:
        places = json.loads(PLACE_NAMES.read_text(encoding="utf-8-sig"))
        for row_id, row in places.items():
            english, italian = row["name"], row["translation"]
            if english and italian:
                entries.append(dict(english=english, italian=italian,
                                    reference=f"world/placename.json#{row_id}:name", category="Luoghi",
                                    usage="Nome da PlaceName; verificare il contesto, soprattutto per gli omonimi."))
    return approved, entries


def pairs(row):
    if not isinstance(row, dict):
        return
    for source, original in row.items():
        if not isinstance(original, str) or not original or source.startswith("translation") or source == "tag":
            continue
        target = "translation_" + source
        if target not in row and source in ("original", "name"):
            target = "translation"
        if target in row and isinstance(row[target], str) and row[target]:
            yield source, target, original, row[target]


def selected_files(name):
    if name:
        path = (TRANSLATIONS / name).resolve()
        if not path.is_relative_to(TRANSLATIONS.resolve()) or not path.is_file():
            raise ValueError(f"File di traduzione non valido: {name}")
        return [path]
    return sorted(TRANSLATIONS.rglob("*.json"))


def approved_name_entries(approved, term):
    """Allow targeted checks of approved menu functions without listing every label."""
    result = []
    for relative, category in (("system/maincommand.json", "Interfaccia e comandi"),):
        if relative not in approved:
            continue
        rows = json.loads((TRANSLATIONS / relative).read_text(encoding="utf-8-sig"))
        for row_id, row in rows.items():
            for source, _, english, italian in pairs(row):
                if source == "name" and english.casefold() == term.casefold():
                    result.append(dict(english=english, italian=italian,
                                       reference=f"{relative}#{row_id}:name", category=category, usage=""))
    return result


def write_json(path, value):
    Path(path).write_text(json.dumps(value, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def case_style(text):
    """Classify term casing while ignoring sentence-initial capitalization in phrases."""
    words = re.findall(r"[A-Za-zÀ-ÖØ-öø-ÿ]+", text)
    if not words:
        return "mixed"
    if all(word.isupper() for word in words):
        return "upper"
    if all(word.islower() for word in words):
        return "lower"
    # A capitalized first word followed by lowercase words is often just the
    # start of a sentence (e.g. "Cross-world linkshells are...").
    if " " in text and len(words) > 1 and words[0][0].isupper() and all(word.islower() for word in words[1:]):
        return "lower"
    if len(words) > 1 and all(word[0].isupper() for word in words):
        return "title"
    if len(words) == 1 and words[0][0].isupper():
        return "title"
    return "mixed"


def expected_case(text, style):
    if style == "upper":
        return text.upper()
    if style != "lower":
        return text
    # Keep established mixed-case acronyms such as PvP alongside all-caps ones.
    acronyms = {m.group() for m in re.finditer(r"\b(?:[A-Z0-9]{2,}|[A-Z][a-z]+[A-Z][A-Za-z0-9]*)\b", text)}
    lowered = text.lower()
    for acronym in acronyms:
        lowered = re.sub(r"(?i)(?<!\w)" + re.escape(acronym.lower()) + r"(?!\w)", acronym, lowered)
    return lowered


def grammar_correction(text):
    """Apply only high-confidence, explicitly documented Italian cleanup rules."""
    rules = []
    prefixes = {"il": "l'", "lo": "l'", "la": "l'", "del": "dell'", "dello": "dell'",
                "della": "dell'", "al": "all'", "allo": "all'", "alla": "all'",
                "nel": "nell'", "nello": "nell'", "nella": "nell'", "sul": "sull'",
                "sullo": "sull'", "sulla": "sull'", "dal": "dall'", "dallo": "dall'",
                "dalla": "dall'"}

    def elide(match):
        prefix = prefixes[match.group(1).casefold()]
        if match.group(1)[0].isupper():
            prefix = prefix[0].upper() + prefix[1:]
        return prefix + match.group(2)

    text, count = ELISION.subn(elide, text)
    if count:
        rules.append("elisione_articolo_preposizione")
    for pattern, replacement, rule in KNOWN_GRAMMAR:
        def preserve_case(match):
            if match.group()[0].islower():
                return replacement[0].lower() + replacement[1:]
            return replacement
        text, count = pattern.subn(preserve_case, text)
        if count:
            rules.append(rule)
    return text, rules


def reviewed_maintained_fields(path):
    """Load exact M field decisions from this tool's editable Markdown checklist."""
    path = Path(path)
    if not path.is_file():
        return set()
    decisions = {}
    current = None
    action = ""
    for line in path.read_text(encoding="utf-8-sig").splitlines() + [""]:
        match = re.match(r"- \[[ xX]\] `([^`]+)`", line)
        if match:
            if current and has_maintain_marker(action):
                decisions[current] = checklist_text(original)
            current, action, original, translation = match.group(1), "", "", ""
            continue
        if not current:
            continue
        match = re.match(r"\s*- Inglese: `(.*)`", line)
        if match:
            original = match.group(1)
        match = re.match(r"\s*- Italiano attuale: `(.*)`", line)
        if match:
            translation = match.group(1)
        match = re.match(r"\s*- \*\*Azione:\*\* (.*)", line)
        if match:
            action = match.group(1).strip()
    if current and has_maintain_marker(action):
        decisions[current] = checklist_text(original)
    return decisions


def checklist_text(text):
    """A terminal ellipsis in the checklist marks a truncated preview."""
    text = TAGS.sub("", text)
    text = re.sub(r"\s+", " ", text).strip()
    return text[:-1] if text.endswith("…") else text


def has_maintain_marker(action):
    # R means "regola": it must produce an actionable review, not hide the
    # occurrence as if the user had said to maintain it. MR explicitly starts
    # with M and therefore keeps this exact occurrence while recording a rule.
    return bool(re.match(r"\s*(?:MANTIENI|MR|M)(?![A-Z])", action.upper()))


def reviewed_focus_terms(path):
    """Read field notes that explicitly limit an A decision to one term."""
    path = Path(path)
    if not path.is_file():
        return {}
    focused = {}
    current = action = None
    for line in path.read_text(encoding="utf-8-sig").splitlines() + [""]:
        match = re.match(r"- \[[ xX]\] `([^`]+)`", line)
        if match:
            if current and action:
                term = re.match(r"\s*A\s+solo\s+[\"“]([^\"”]+)[\"”]", action, re.IGNORECASE)
                if term:
                    focused[current] = term.group(1).strip()
            current, action = match.group(1), ""
            continue
        if current:
            match = re.match(r"\s*- \*\*Azione:\*\* (.*)", line)
            if match:
                action = match.group(1).strip()
    if current and action:
        term = re.match(r"\s*A\s+solo\s+[\"“]([^\"”]+)[\"”]", action, re.IGNORECASE)
        if term:
            focused[current] = term.group(1).strip()
    return focused


def source_aligned_translation(text, candidate, style):
    """Check source casing while allowing Italian articles and sentence starts."""
    exact = re.compile(r"(?<!\w)" + re.escape(candidate) + r"(?!\w)")
    if exact.search(text):
        return True
    # In Italian a definite article attached to a proper name is lowercase in
    # running prose even when the glossary stores the standalone title case.
    article = re.match(r"^(Il|Lo|La|I|Gli|Le|L')(?=\s|[A-ZÀ-ÖØ-Þ])(.+)$", candidate)
    if article:
        lower_article = article.group(1).lower() + article.group(2)
        if re.search(r"(?<!\w)" + re.escape(lower_article) + r"(?!\w)", text):
            return True
    if style == "lower":
        plain = TAGS.sub("", text)
        folded = re.compile(r"(?<!\w)" + re.escape(candidate) + r"(?!\w)", re.IGNORECASE)
        for match in folded.finditer(plain):
            actual = match.group()
            if actual[:1].isupper() and actual[1:] == candidate[1:]:
                before = plain[:match.start()].rstrip()
                if not before or before[-1] in ".!?:;":
                    return True
    return False


def usage_variants(usage):
    marker = "Varianti ammesse in prosa:"
    if marker not in usage:
        return []
    tail = usage.split(marker, 1)[1]
    return [value.strip() for value in re.findall(r"«([^»]+)»", tail)]


def usage_variant_pattern(variant):
    """Turn documented placeholders into a match for one preserved dynamic tag."""
    escaped = re.escape(variant)
    return escaped.replace(r"\[dispositivo\]", r"(?:<[^>]+>|\{[^{}]+\})")


def harvest(args):
    if Path(args.out).exists() and not args.overwrite:
        raise ValueError(f"Il file esiste già: {args.out}. Usa un altro --out o --overwrite.")
    approved, entries = glossary()
    existing = {(e["english"].casefold(), e["italian"].casefold()) for e in entries}
    candidates = collections.defaultdict(list)
    files = [args.file] if args.file else sorted(approved)
    for relative in files:
        if relative not in approved:
            raise ValueError(f"Il file non è ancora approvato: {relative}")
        rows = json.loads((TRANSLATIONS / relative).read_text(encoding="utf-8-sig"))
        for row_id, row in rows.items():
            for source, _, english, italian in pairs(row):
                if source not in ("name", "original", "name_masculine", "name_feminine"):
                    continue
                if not 3 <= len(english) <= 70 or TAGS.search(english) or TAGS.search(italian):
                    continue
                if (english.casefold() == italian.casefold() and not args.include_unchanged) or (english.casefold(), italian.casefold()) in existing:
                    continue
                candidates[(english, italian)].append(f"{relative}#{row_id}:{source}")
    result = [dict(english=en, italian=it, sources=refs, decision="pending", usage="")
              for (en, it), refs in candidates.items()]
    result.sort(key=lambda x: (-len(x["sources"]), x["english"].casefold()))
    write_json(args.out, result[:args.limit])
    print(f"{len(result)} candidati; primi {min(len(result), args.limit)} in {args.out}. Approvare a mano prima di aggiungere al glossario.")


def scan(args):
    if Path(args.out).exists() and not args.overwrite:
        raise ValueError(f"Il file esiste già: {args.out}. Usa un altro --out o --overwrite.")
    approved, entries = glossary()
    all_entries = entries
    if args.term:
        entries = [e for e in entries if e["english"].casefold() == args.term.casefold()]
        if not entries:
            entries = approved_name_entries(approved, args.term)
        if not entries:
            raise ValueError(f"Termine assente dal glossario e dai nomi approvati: {args.term}")
    else:
        entries = [e for e in entries if e["category"] != "Luoghi" or
                   (len(e["english"]) >= 6 and not TAGS.search(e["english"] + e["italian"]))]
    if args.old and not args.term:
        raise ValueError("--old richiede --term")
    blocking = None
    maintained = reviewed_maintained_fields(args.checklist) if args.checklist else set()
    focused = reviewed_focus_terms(args.checklist) if args.checklist else {}
    if args.term:
        longer = {e["english"] for e in all_entries if len(e["english"]) > len(args.term)
                  and args.term.casefold() in e["english"].casefold()}
        if longer:
            blocking = re.compile(r"(?<!\w)(?:" + "|".join(re.escape(n) for n in sorted(longer, key=len, reverse=True))
                                  + r")(?!\w)", re.IGNORECASE)
    # Longest first: "Duty Finder" should win over "Duty".
    names = sorted({e["english"] for e in entries}, key=lambda x: (-len(x), x.casefold()))
    pattern = re.compile(r"(?<!\w)(?:" + "|".join(re.escape(n).replace(r"\[1\]", r"\[\d+\]") for n in names)
                         + r")(?!\w)", re.IGNORECASE)
    by_name = collections.defaultdict(list)
    for entry in entries:
        by_name[entry["english"].casefold()].append(entry)
    by_italian = collections.defaultdict(set)
    for name, variants in by_name.items():
        for entry in variants:
            by_italian[entry["italian"].casefold()].add(name)
    queue = []
    sampled = collections.Counter()
    for path in selected_files(args.file):
        relative = path.relative_to(TRANSLATIONS).as_posix()
        if args.approved_only and relative not in approved:
            continue
        if not args.approved_only and relative in approved and not args.include_approved:
            continue
        rows = json.loads(path.read_text(encoding="utf-8-sig"))
        for row_id, row in rows.items():
            for source_field, target_field, original, translation in pairs(row):
                decision_key = f"{relative}#{row_id}:{target_field}"
                corrected, grammar_rules = grammar_correction(translation)
                if grammar_rules and corrected != translation:
                    queue.append(dict(file=relative, row_id=row_id, source_field=source_field,
                                      target_field=target_field, original=original, translation=translation,
                                      english="[controllo grammaticale italiano]", canonical=[],
                                      references=["tools/terminology_review.py"],
                                      usages=grammar_rules, old_italian="", suggestion=corrected,
                                      status="pending", note="; ".join(grammar_rules), finding="grammar",
                                      source_case="", expected_case=[]))
                decision = maintained.get(decision_key)
                if decision and checklist_text(original).startswith(decision):
                    continue
                old_match = bool(args.old and re.search(r"(?<!\w)" + re.escape(args.old) + r"(?!\w)",
                                                         translation, re.IGNORECASE))
                if args.old and not old_match:
                    continue
                if relative == "dialogue/customtalk.json" and (
                        source_field == "name" or
                        (source_field.startswith("col_") and source_field[4:].isdigit() and int(source_field[4:]) < 31)):
                    continue
                if len(original) > 2500:
                    continue  # Large SeString payloads need a separate manual pass.
                searchable = blocking.sub(lambda m: " " * len(m.group()), original) if blocking else original
                matches = {re.sub(r"\[\d+\]", "[1]", match.group()).casefold(): match.group()
                           for match in pattern.finditer(searchable)}
                if old_match:
                    matches.update({name: name for name in by_name})
                for name, matched in matches.items():
                    focus = focused.get(decision_key)
                    if focus and name not in by_italian.get(focus.casefold(), set()):
                        continue
                    if not args.term and not args.all_matches and sampled[name] >= 5:
                        continue  # Broad scan samples each term; --term retrieves every occurrence.
                    variants = by_name[name]
                    term = variants[0]["english"]
                    if (not args.term and (len({e["italian"].casefold() for e in variants}) > 1 or
                                           variants[0]["category"] in ("Meteo", "Razze e clan"))):
                        continue
                    if (not args.old and len(term.split()) == 1 and
                            original.strip().casefold() != term.casefold() and
                            (len(term) < 6 or term.casefold() == variants[0]["italian"].casefold() or
                             variants[0]["category"] not in ("Luoghi", "Termini di gioco")
                             and term.casefold() != "glamours")):
                        continue
                    number = re.search(r"\[(\d+)\]", matched)
                    canonical = list(dict.fromkeys(e["italian"].replace("[1]", number.group() if number else "[1]")
                                                   for e in variants))
                    # A matching Italian form is evidence of consistency, not proof of it.
                    if not args.old:
                        canonical_exact = any(re.search(r"(?<!\w)" + re.escape(v) + r"(?!\w)", translation)
                                              for v in canonical)
                        canonical_casefold = any(re.search(r"(?<!\w)" + re.escape(v) + r"(?!\w)", translation,
                                                           re.IGNORECASE) for v in canonical)
                        style = case_style(matched)
                        aligned = [expected_case(value, style) for value in canonical]
                        allowed = [alias for entry in variants for alias in usage_variants(entry["usage"])]
                        alias_aligned = any(re.search(r"(?<!\w)" + usage_variant_pattern(alias) + r"(?!\w)", translation,
                                                      re.IGNORECASE) for alias in allowed)
                        source_aligned = any(source_aligned_translation(translation, value, style)
                                             for value in aligned)
                        if alias_aligned:
                            continue
                        if (canonical_casefold and not args.strict_canonical_case) or (
                                args.strict_canonical_case and source_aligned):
                            continue
                    else:
                        style, aligned = case_style(matched), []
                    queue.append(dict(file=relative, row_id=row_id, source_field=source_field,
                                      target_field=target_field, original=original, translation=translation,
                                      english=matched if number else variants[0]["english"], canonical=canonical,
                                      finding=("case" if not args.old and canonical_casefold else "translation"),
                                      source_case=style, expected_case=aligned,
                                      references=[e["reference"] for e in variants],
                                      usages=[e["usage"] for e in variants],
                                      old_italian=args.old or "",
                                      suggestion="", status="pending", note=""))
                    sampled[name] += 1
                    if len(queue) >= args.limit:
                        write_json(args.out, queue)
                        print(f"Limite di {args.limit} segnalazioni raggiunto; restringere con --file o --term. Coda: {args.out}")
                        return
    write_json(args.out, queue)
    print(f"{len(queue)} segnalazioni in {args.out}. Ogni proposta richiede verifica nel contesto.")


def same_tokens(before, after):
    return TAGS.findall(before) == TAGS.findall(after)


def apply(args):
    queue = json.loads(Path(args.review).read_text(encoding="utf-8"))
    approved = [x for x in queue if x["status"] == "approved"]
    grouped = collections.defaultdict(list)
    for item in approved:
        grouped[item["file"]].append(item)
    plans = []
    for relative, items in grouped.items():
        path = (TRANSLATIONS / relative).resolve()
        if not path.is_relative_to(TRANSLATIONS.resolve()) or not path.is_file():
            raise ValueError(f"File non valido: {relative}")
        original_bytes = path.read_bytes()
        bom = original_bytes.startswith(b"\xef\xbb\xbf")
        raw = original_bytes.decode("utf-8-sig")
        rows = json.loads(raw)
        row_starts = list(ROW.finditer(raw))
        edits = []
        seen = {}
        for item in items:
            key = (item["row_id"], item["target_field"])
            row = rows[item["row_id"]]
            proposal = item["suggestion"]
            if (row[item["source_field"]] != item["original"] or
                    row[item["target_field"]] != item["translation"] or
                    not isinstance(proposal, str) or not proposal.strip() or
                    not same_tokens(item["translation"], proposal)):
                raise ValueError(f"Riga cambiata o proposta non valida: {relative}#{key}")
            start = next((m.end() for m in row_starts if m.group("id") == item["row_id"]), None)
            if start is None:
                raise ValueError(f"Formato JSON non supportato: {relative}#{key}")
            end = next((m.start() for m in row_starts if m.start() >= start), len(raw))
            found = [m for m in FIELD.finditer(raw, start, end) if m.group("name") == item["target_field"]]
            if len(found) != 1 or json.loads(found[0].group("value")) != item["translation"]:
                raise ValueError(f"Campo JSON non individuato: {relative}#{key}")
            previous = seen.get(key)
            if previous is not None:
                if previous != (item["translation"], proposal):
                    raise ValueError(f"Proposte approvate in conflitto: {relative}#{key}")
                continue
            seen[key] = (item["translation"], proposal)
            edits.append((found[0].start("value"), found[0].end("value"), json.dumps(proposal, ensure_ascii=False)))
        for start, end, value in sorted(edits, reverse=True):
            raw = raw[:start] + value + raw[end:]
        json.loads(raw)
        plans.append((path, (b"\xef\xbb\xbf" if bom else b"") + raw.encode("utf-8"), len(edits)))
    if args.dry_run:
        print(f"Anteprima: {sum(n for _, _, n in plans)} campi approvati in {len(plans)} file; nessuna modifica.")
        return
    for path, raw, _ in plans:
        handle, temp = tempfile.mkstemp(dir=path.parent, prefix=".terminology-", suffix=".json")
        try:
            with os.fdopen(handle, "wb") as stream:
                stream.write(raw)
            os.replace(temp, path)
        finally:
            if os.path.exists(temp):
                os.unlink(temp)
    print(f"Applicati {sum(n for _, _, n in plans)} campi approvati in {len(plans)} file.")


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    modes = parser.add_subparsers(dest="command", required=True)
    candidate = modes.add_parser("harvest", help="Estrae candidati dai soli file approvati")
    candidate.add_argument("--file", help="Percorso relativo a data/translations")
    candidate.add_argument("--include-unchanged", action="store_true", help="Includi nomi mantenuti in inglese")
    candidate.add_argument("--limit", type=int, default=200)
    candidate.add_argument("--out", default="data/glossary/candidates.json")
    candidate.add_argument("--overwrite", action="store_true")
    candidate.set_defaults(run=harvest)
    scan_cmd = modes.add_parser("scan", help="Cerca possibili incoerenze nel corpus")
    scan_cmd.add_argument("--file", help="Percorso relativo a data/translations")
    scan_cmd.add_argument("--term", help="Solo questo termine inglese")
    scan_cmd.add_argument("--old", help="Vecchia forma italiana da cercare dopo una modifica del canone; richiede --term")
    scan_scope = scan_cmd.add_mutually_exclusive_group()
    scan_scope.add_argument("--include-approved", action="store_true",
                            help="Includi i file approvati nella scansione generale")
    scan_scope.add_argument("--approved-only", action="store_true",
                            help="Scansiona esclusivamente i file elencati come approvati nel glossario")
    scan_cmd.add_argument("--all-matches", action="store_true", help="Non limita a cinque le segnalazioni per termine")
    scan_cmd.add_argument("--strict-canonical-case", action="store_true",
                          help="Controlla le maiuscole rispetto alla forma nell'originale, non solo al canone")
    scan_cmd.add_argument("--checklist", default="data/glossary/review_checklist.md",
                          help="Checklist precedente: i campi MANTIENI identici non vengono risegnalati")
    scan_cmd.add_argument("--limit", type=int, default=200)
    scan_cmd.add_argument("--out", default="data/glossary/review.json")
    scan_cmd.add_argument("--overwrite", action="store_true")
    scan_cmd.set_defaults(run=scan)
    apply_cmd = modes.add_parser("apply", help="Applica soltanto le proposte con status=approved")
    apply_cmd.add_argument("--review", default="data/glossary/review.json")
    apply_cmd.add_argument("--dry-run", action="store_true")
    apply_cmd.set_defaults(run=apply)
    args = parser.parse_args()
    if getattr(args, "limit", 1) < 1:
        parser.error("--limit deve essere positivo")
    try:
        args.run(args)
    except (OSError, ValueError, KeyError, json.JSONDecodeError) as exc:
        parser.exit(1, f"Errore: {exc}\n")


if __name__ == "__main__":
    main()
