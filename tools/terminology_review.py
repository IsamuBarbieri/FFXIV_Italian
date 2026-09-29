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
HEX_TAG = re.compile(r"<hex:([0-9a-f]+)>", re.IGNORECASE)
ELISION = re.compile(r"\b((?i:il|del|dello|della|al|allo|alla|nel|nello|nella|sul|sullo|sulla|dal|dallo|dalla))\s+(?!(?i:i[aeiouàèéìòù]))([aeiouàèéìòù])")
IAIJUTSU_ARTICLES = re.compile(r"\b(?P<article>il|lo|l['’]|del|dello|dell['’]|al|allo|all['’]|nel|nello|nell['’]|sul|sullo|sull['’]|dal|dallo|dall['’])\s*(?P<term>iaijutsu)\b", re.IGNORECASE)
KNOWN_GRAMMAR = (
    (re.compile(r"\bAvvio di un appello\b", re.IGNORECASE), "Avvio dell'appello", "avvio_ready_check"),
    (re.compile(r"\bdisponibile a Piazza dei Chocobo\b", re.IGNORECASE), "disponibile nella Piazza dei Chocobo", "preposizione_piazza_chocobo"),
    (re.compile(r"\bTane Sospette disponibili\b", re.IGNORECASE), "Tane Sospette Disponibili", "maiuscole_nome_composto"),
    (re.compile(r"\bTi diamo il benvenuto da Vari Splendori\b", re.IGNORECASE), "Ti diamo il benvenuto a Vari Splendori", "preposizione_vari_splendori"),
    (re.compile(r"\bQuesto Tecnica\b", re.IGNORECASE), "Questa Tecnica", "accordo_questa_tecnica"),
    (re.compile(r"\bl['’]esecuzione di tecnica\b", re.IGNORECASE), "l'esecuzione di una tecnica", "articolo_tecnica_generica"),
    (re.compile(r"\bdispositivi di Input\b", re.IGNORECASE), "dispositivi di input", "maiuscola_input_device"),
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
        if not path.is_relative_to(TRANSLATIONS.resolve()):
            raise ValueError(f"Percorso di traduzione non valido: {name}")
        if path.is_file():
            return [path]
        if path.is_dir():
            return sorted(path.rglob("*.json"))
        raise ValueError(f"File o cartella di traduzione non validi: {name}")
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


def plain_text(text):
    """Read visible text from ordinary markup and SeString hex-encoded payloads."""
    def decode_hex(match):
        try:
            payload = bytes.fromhex(match.group(1))
        except ValueError:
            return " "
        payload = re.sub(rb"\x02[\x48\x49]\x04.{3}\x03", b"", payload)
        payload = re.sub(rb"\x02[\x48\x49]\x02\x01\x03", b"", payload)
        payload = re.sub(rb"\x02\x10\x01\x03", b" ", payload)
        decoded = payload.decode("utf-8", errors="replace")
        return "".join(" " if not char.isprintable() or char == "\ufffd" else char for char in decoded)

    text = HEX_TAG.sub(decode_hex, text)
    text = TAGS.sub("", text)
    return re.sub(r"\s+", " ", text).strip()


def case_style(text):
    """Classify term casing while ignoring sentence-initial capitalization in phrases."""
    words = re.findall(r"[A-Za-zÀ-ÖØ-öø-ÿ]+", text)
    if not words:
        return "mixed"
    if all(word.isupper() for word in words):
        return "upper"
    if all(word.islower() for word in words):
        return "lower"
    acronyms = {m.group() for m in re.finditer(r"\b(?:[A-Z0-9]{2,}|[A-Z][a-z]+[A-Z][A-Za-z0-9]*)\b", text)}
    lexical = [word for word in words if word not in acronyms]
    if lexical and all(word.islower() for word in lexical):
        return "lower"
    # A capitalized first word followed by lowercase words is sentence case,
    # not a title (e.g. a list label such as "Friend list").
    if " " in text and len(words) > 1 and words[0][0].isupper() and all(word.islower() for word in words[1:]):
        return "sentence"
    if len(words) > 1 and all(word[0].isupper() for word in words):
        return "title"
    if len(words) == 1 and words[0][0].isupper():
        return "title"
    return "mixed"


def sentence_initial(text, position):
    prefix = re.sub(r"[\s※■•・]+$", "", plain_text(text[:position])).rstrip()
    return not prefix or bool(re.search(r"[.!?…][\"'’”»\])}]*\s*$", prefix))


def place_name_case_matches(term, matched, at_sentence_start):
    expected = re.findall(r"[A-Za-zÀ-ÖØ-öø-ÿ]+", term)
    actual = re.findall(r"[A-Za-zÀ-ÖØ-öø-ÿ]+", TAGS.sub("", matched))
    if len(expected) != len(actual):
        return True
    for index, (canonical, found) in enumerate(zip(expected, actual)):
        # English articles are lowercase in running prose even when they are
        # capitalized in the standalone place name.
        if index == 0 and canonical.casefold() in {"the", "a", "an"}:
            continue
        if canonical != found:
            return False
    return True


def exact_place_occurrences(term, occurrences, original, source_field, relative):
    """Avoid treating a place-name prefix inside a different title as the place."""
    plain = plain_text(original)
    aligned = TAGS.sub(lambda match: " " * len(match.group()), original)
    for matched, position in occurrences:
        if source_field == "name" and plain.strip(" .!?…'’\"“”«»()[]").casefold() != term.casefold():
            continue
        if (relative.endswith("/world/title.json") and source_field == "description" and
                plain.strip(" .!?…'’\"“”«»()[]").casefold() != term.casefold()):
            continue
        end = position + len(matched)
        for opening, closing in (("“", "”"), ("«", "»"), ('"', '"')):
            quote_start = aligned.rfind(opening, 0, position)
            quote_end = aligned.find(closing, end)
            if (quote_start >= 0 and quote_end >= end and
                    aligned.find(closing, quote_start + len(opening), position) < 0 and
                    aligned.find(opening, end, quote_end) < 0):
                quoted = aligned[quote_start + len(opening):quote_end].strip(" .!?…'’\"“”«»()[]")
                if quoted.casefold() != TAGS.sub("", matched).casefold():
                    break
        else:
            quoted = None
        if quoted is not None and quoted.casefold() != TAGS.sub("", matched).casefold():
            continue
        if (source_field == "original" and
                ("/combat/" in f"/{relative}" or "/items/" in f"/{relative}") and
                plain.strip(" .!?…'’\"“”«»()[]").casefold() == term.casefold()):
            # A full name in an action/trait/item field is a content title, not a PlaceName reference.
            continue
        if (relative.endswith("/combat/status.json") and source_field in {"original", "name"} and
                plain.strip(" .!?…'’\"“”«»()[]").casefold() == term.casefold()):
            # A standalone status name can collide with a PlaceName without referring to that location.
            continue
        if (relative.endswith("/combat/actiontransient.json") and source_field == "original" and
                term.casefold() == "resolution" and
                re.search(r"\bchanges to\s+$", aligned[max(0, position - 150):position], re.IGNORECASE)):
            # Resolution is an action name here, not either PlaceName with the same spelling.
            continue
        if (relative.endswith("/combat/actiontransient.json") and source_field == "original" and
                term.casefold() == "starfall" and
                re.search(r"\bPrediction of\s+$", aligned[max(0, position - 100):position])):
            # The tagged Prediction of Starfall is an action effect, not the PlaceName.
            continue
        if len(term.split()) == 1:
            previous_word = re.search(r"([A-ZÀ-ÖØ-Þ][A-Za-zÀ-ÖØ-öø-ÿ'’]*)\s+$", aligned[:position])
            if previous_word:
                # A one-word PlaceName can be the last word of a different proper name
                # (e.g. the location-like suffix in "Phantom Flurry").
                continue
        previous_prefix = re.search(r"([A-ZÀ-ÖØ-Þ][A-Za-zÀ-ÖØ-öø-ÿ'’]*)\s+$", aligned[:position])
        if previous_prefix and previous_prefix.group(1).casefold() == "camp":
            # Camp + area name is a distinct PlaceName; match its full glossary
            # entry rather than the area-name suffix on its own.
            continue
        if term.casefold().startswith("the "):
            prefix = aligned[:position]
            if re.search(r"\bof\s+$", prefix, re.IGNORECASE):
                title_prefix = re.search(r"([A-ZÀ-ÖØ-Þ][A-Za-zÀ-ÖØ-öø-ÿ'’]*)\s+of\s+$", prefix)
                if title_prefix:
                    # Keep "The Deep" distinct from the longer title "Doom of the Deep".
                    continue
            if (term.casefold() == "the burn" and
                    re.search(r"\bFeeling\s+$", prefix)):
                # Do not match "The Burn" inside the longer titled engagement "Feeling the Burn".
                continue
        if term.casefold() == "stacks":
            before = aligned[max(0, position - 45):position]
            after = aligned[position + len(matched):position + len(matched) + 5]
            if re.search(r"\bMana\s+$", before, re.IGNORECASE):
                continue
            if re.search(r"(?:maximum|repertoire|mana|\d+)\s+$", before, re.IGNORECASE) and after.lstrip().startswith(":"):
                continue
        if not place_name_case_matches(term, matched, sentence_initial(original, position)):
            continue
        tail = aligned[end:]
        next_word = re.match(r"\s+([A-ZÀ-ÖØ-Þ][A-Za-zÀ-ÖØ-öø-ÿ'’]*)", tail)
        linked_extension = re.match(
            r"\s+(?:for|of|in|on|at|and|the|to)\s+[A-ZÀ-ÖØ-Þ][A-Za-zÀ-ÖØ-öø-ÿ'’]*",
            tail, re.IGNORECASE)
        if next_word or linked_extension:
            # A following capitalized word or title link extends the full name.
            continue
        yield matched, position


def exact_function_occurrences(term, occurrences, original, ignore_title_extensions=False):
    """Match a named feature exactly, while allowing a lowercase English article in prose."""
    aligned = TAGS.sub(lambda match: " " * len(match.group()), original)
    term_words = term.split()
    for matched, position in occurrences:
        matched_words = TAGS.sub("", matched).split()
        if len(term_words) != len(matched_words):
            continue
        if term_words[0].casefold() == "the":
            exact_words = (matched_words[0].casefold() == "the" and
                           matched_words[1:] == term_words[1:])
        else:
            exact_words = matched == term
        if not exact_words and not (sentence_initial(original, position) and
                                    matched.casefold() == term.casefold()):
            continue
        if ignore_title_extensions:
            tail = aligned[position + len(matched):]
            next_word = re.match(r"\s+([A-ZÀ-ÖØ-Þ][A-Za-zÀ-ÖØ-öø-ÿ'’]*)", tail)
            linked_extension = re.match(
                r"\s+(?:for|of|in|on|at|and|the)\s+[A-ZÀ-ÖØ-Þ][A-Za-zÀ-ÖØ-öø-ÿ'’]*",
                tail, re.IGNORECASE)
            if next_word or linked_extension:
                continue
        yield matched, position


def expected_case(text, style):
    if style == "fixed":
        return text
    if style == "upper":
        return text.upper()
    if style not in ("lower", "sentence"):
        return text
    # Keep established mixed-case acronyms such as PvP alongside all-caps ones.
    acronyms = {m.group() for m in re.finditer(r"\b(?:[A-Z0-9]{2,}|[A-Z][a-z]+[A-Z][A-Za-z0-9]*)\b", text)}
    lowered = text.lower()
    for acronym in acronyms:
        lowered = re.sub(r"(?i)(?<!\w)" + re.escape(acronym.lower()) + r"(?!\w)", acronym, lowered)
    if style == "sentence":
        first = re.search(r"[A-Za-zÀ-ÖØ-öø-ÿ]+", lowered)
        if first:
            lowered = lowered[:first.start()] + first.group().capitalize() + lowered[first.end():]
    return lowered


def canonical_forms(value):
    """Include correct Italian preposition+article forms of titled names."""
    article = re.match(r"(?i)^(Il|Lo|La|I|Gli|Le|L['’])\s*(.+)$", value)
    if not article:
        return [value]
    base = article.group(2)
    forms = {
        "il": ("il", "del", "al", "nel", "sul", "dal"),
        "lo": ("lo", "dello", "allo", "nello", "sullo", "dallo"),
        "la": ("la", "della", "alla", "nella", "sulla", "dalla"),
        "i": ("i", "dei", "ai", "nei", "sui", "dai"),
        "gli": ("gli", "degli", "agli", "negli", "sugli", "dagli"),
        "le": ("le", "delle", "alle", "nelle", "sulle", "dalle"),
        "l'": ("l'", "dell'", "all'", "nell'", "sull'", "dall'"),
        "l’": ("l’", "dell’", "all’", "nell’", "sull’", "dall’"),
    }[article.group(1).casefold()]
    separator = "" if article.group(1).casefold() in {"l'", "l’"} else " "
    variants = [prefix + separator + base for prefix in forms]
    variants += [form[0].upper() + form[1:] for form in variants if form]
    return list(dict.fromkeys([value] + variants))


def canonical_present(text, value, ignore_case=False):
    text = plain_text(text)
    flags = re.IGNORECASE if ignore_case else 0
    return any(re.search(r"(?<!\w)" + re.escape(form) + r"(?!\w)", text, flags)
               for form in canonical_forms(value))


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
    articles = {"il": "lo", "lo": "lo", "l'": "lo", "l’": "lo",
                "del": "dello", "dello": "dello", "dell'": "dello", "dell’": "dello",
                "al": "allo", "allo": "allo", "all'": "allo", "all’": "allo",
                "nel": "nello", "nello": "nello", "nell'": "nello", "nell’": "nello",
                "sul": "sullo", "sullo": "sullo", "sull'": "sullo", "sull’": "sullo",
                "dal": "dallo", "dallo": "dallo", "dall'": "dallo", "dall’": "dallo"}

    def iaijutsu_article(match):
        article = articles[match.group("article").casefold()]
        if match.group("article")[0].isupper():
            article = article.capitalize()
        return article + " " + match.group("term")

    text, count = IAIJUTSU_ARTICLES.subn(iaijutsu_article, text)
    if count:
        rules.append("articolo_iaijutsu_semiconsonante")
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
    if canonical_present(text, candidate):
        return True
    # In Italian a definite article attached to a proper name is lowercase in
    # running prose even when the glossary stores the standalone title case.
    article = re.match(r"(?i)^(Il|Lo|La|I|Gli|Le|L['’]|Un|Uno|Una|Un['’])(?=\s|[A-ZÀ-ÖØ-Þ])(.+)$", candidate)
    if article:
        lower_article = article.group(1).lower() + article.group(2)
        if re.search(r"(?<!\w)" + re.escape(lower_article) + r"(?!\w)", text):
            return True
    if style == "lower":
        plain = plain_text(text)
        folded = re.compile(r"(?<!\w)" + re.escape(candidate) + r"(?!\w)", re.IGNORECASE)
        for match in folded.finditer(plain):
            actual = match.group()
            if actual[:1].isupper() and actual[1:] == candidate[1:]:
                before = plain[:match.start()].rstrip()
                if not before or before[-1] in ".!?:;":
                    return True
    return False


def usage_variants(usage):
    variants = []
    marker = re.search(r"Varianti ammesse in prosa:\s*(.*)", usage, re.IGNORECASE)
    if marker:
        tail = marker.group(1)
        variants.extend(value.strip() for value in re.findall(r"«([^»]+)»", tail))
    singular = re.search(r"\bsingolare:\s*«([^»]+)»", usage, re.IGNORECASE)
    if singular:
        variants.append(singular.group(1).strip())
    return list(dict.fromkeys(variants))


def usage_variant_present(text, variant):
    """Match a documented prose form using its Italian sentence position for casing."""
    pattern = re.compile(r"(?<!\w)" + usage_variant_pattern(variant) + r"(?!\w)", re.IGNORECASE)
    for visible in (text, plain_text(text)):
        for match in pattern.finditer(visible):
            expected = variant
            if case_style(variant) == "lower" and sentence_initial(visible, match.start()):
                first = re.search(r"[A-Za-zÀ-ÖØ-öø-ÿ]", expected)
                if first:
                    expected = expected[:first.start()] + first.group().upper() + expected[first.end():]
            matched_variant = re.sub(r"(?:<[^>]+>|\{[^{}]+\})", "[dispositivo]", match.group())
            if matched_variant == expected:
                return True
    return False


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
            blocking = re.compile(r"(?<![\w-])(?:" + "|".join(re.escape(n) for n in sorted(longer, key=len, reverse=True))
                                  + r")(?![\w-])", re.IGNORECASE)
    # Longest first: "Duty Finder" should win over "Duty".
    names = sorted({e["english"] for e in entries}, key=lambda x: (-len(x), x.casefold()))
    patterns = []
    for name in names:
        words = re.split(r"\s+", name)
        patterns.append(r"(?:[\s]|<[^>]+>|\{[^{}]+\})+".join(
            re.escape(word).replace(r"\[1\]", r"\[\d+\]") for word in words))
    pattern = re.compile(r"(?<![\w-])(?:" + "|".join(patterns)
                         + r")(?![\w-])", re.IGNORECASE)
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
                matches = collections.defaultdict(list)
                for match in pattern.finditer(searchable):
                    name = re.sub(r"\s+", " ", re.sub(r"\[\d+\]", "[1]",
                                      TAGS.sub("", match.group()))).strip().casefold()
                    matches[name].append((match.group(), match.start()))
                if old_match:
                    matches.update({name: [(name, 0)] for name in by_name})
                for name, occurrences in matches.items():
                    focus = focused.get(decision_key)
                    if focus and name not in by_italian.get(focus.casefold(), set()):
                        continue
                    if not args.term and not args.all_matches and sampled[name] >= 5:
                        continue  # Broad scan samples each term; --term retrieves every occurrence.
                    variants = by_name.get(name)
                    if not variants:
                        # Regex matches may normalize a tagged/dynamic source form
                        # differently from the glossary key; it is not a review hit.
                        continue
                    term = variants[0]["english"]
                    if term.casefold() == "ready check" and re.search(r"\bensemble mode\b", original, re.IGNORECASE):
                        continue
                    if any("Nome della funzione" in entry["usage"] for entry in variants):
                        ignore_title_extensions = any("prefisso di un titolo composto da ignorare" in
                                                      entry["usage"].casefold() for entry in variants)
                        occurrences = list(exact_function_occurrences(term, occurrences, original,
                                                                     ignore_title_extensions))
                        if not occurrences:
                            continue
                    if any(entry["category"] == "Luoghi" for entry in variants):
                        aligned_occurrences = list(exact_place_occurrences(term, occurrences, original,
                                                                          source_field, relative))
                        if not aligned_occurrences:
                            continue
                        occurrences = aligned_occurrences
                    matched, match_position = occurrences[0]
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
                        canonical_exact = any(canonical_present(translation, v) for v in canonical)
                        canonical_casefold = any(canonical_present(translation, v, ignore_case=True)
                                                 for v in canonical)
                        fixed_case = any(re.search(r"\bmaiuscole fisse:", entry["usage"], re.IGNORECASE)
                                         for entry in variants)
                        style = "fixed" if fixed_case else case_style(TAGS.sub("", matched))
                        aligned = [expected_case(value, style) for value in canonical]
                        allowed = [alias for entry in variants for alias in usage_variants(entry["usage"])]
                        alias_aligned = any(usage_variant_present(translation, alias) for alias in allowed)
                        source_aligned = any(source_aligned_translation(translation, value, style)
                                             for value in aligned)
                        if alias_aligned:
                            continue
                        if (canonical_casefold and not args.strict_canonical_case) or (
                                args.strict_canonical_case and (source_aligned or
                                                                len(re.findall(r"[A-Za-zÀ-ÖØ-öø-ÿ]+", matched)) == 1
                                                                and sentence_initial(original, match_position))):
                            continue
                    else:
                        style, aligned = case_style(TAGS.sub("", matched)), []
                    queue.append(dict(file=relative, row_id=row_id, source_field=source_field,
                                      target_field=target_field, original=original, translation=translation,
                                      english=matched if number else variants[0]["english"], canonical=canonical,
                                      source_matches=[dict(text=value, start=position,
                                                           before=original[max(0, position - 60):position],
                                                           after=original[position + len(value):position + len(value) + 60])
                                                      for value, position in occurrences],
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
    left, right = TAGS.findall(before), TAGS.findall(after)
    if len(left) != len(right):
        return False
    for old, new in zip(left, right):
        if old.startswith("<hex:") and new.startswith("<hex:"):
            try:
                old_payload = bytes.fromhex(HEX_TAG.fullmatch(old).group(1))
                new_payload = bytes.fromhex(HEX_TAG.fullmatch(new).group(1))
            except (ValueError, AttributeError):
                return False
            old_controls = bytes(value for value in old_payload if value < 32 or value == 127)
            new_controls = bytes(value for value in new_payload if value < 32 or value == 127)
            if old_controls != new_controls:
                return False
        elif old != new:
            return False
    return True


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
