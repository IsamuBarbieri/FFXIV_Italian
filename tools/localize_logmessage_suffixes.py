"""Adjust possessives and later verbs for the local LogMessage actor."""

import json
import re

from localize_logmessage_perspective import PATH, TAG, conditional


LATER_VERBS = {
    103: ('e arrossisce', 'e arrossisci'),
    125: ('e va nel panico', 'e vai nel panico'),
    167: ('e gli occhi gli si riempiono', 'e gli occhi ti si riempiono'),
    216: ('e rimane perplesso', 'e non sai che pensare'),
    594: ('e non subisce danni', 'e non subisci danni'),
    3585: ('e rilascia la preda', 'e rilasci la preda'),
    4345: ('e ottiene', 'e ottieni'),
    4346: ('e ottiene', 'e ottieni'),
    8022: ('e si nasconde la faccia', 'e ti nascondi la faccia'),
    8323: ('e accarezza', 'e accarezzi'),
    8348: ('e cade a terra', 'e cadi a terra'),
    11599: ('e non può trasportarne altri', 'e non puoi trasportarne altri'),
    11602: ('e non può trasportarne altri', 'e non puoi trasportarne altri'),
}
POSSESSIVES = {'propria': 'tua', 'proprio': 'tuo', 'propri': 'tuoi',
               'proprie': 'tue', 'sua': 'tua', 'suo': 'tuo', 'suoi': 'tuoi'}


def main():
    raw = PATH.read_text(encoding='utf-8')
    rows = json.loads(raw)
    changes = {}
    for key, row in rows.items():
        translated = row.get('translation', '')
        if not translated.startswith('<hex:0208'):
            continue
        source = next((m for m in TAG.finditer(row.get('original', ''))
                       if m.group(1).startswith('022B') and 'FF04796F75' in m.group(1)), None)
        if not source:
            continue
        condition = next((part for part in (bytes.fromhex('E4EB02EB03'), bytes.fromhex('E4EB02EB04'))
                          if part in bytes.fromhex(source.group(1))), None)
        if not condition:
            continue
        current = translated
        if int(key) in LATER_VERBS:
            other, own = LATER_VERBS[int(key)]
            current = current.replace(other, conditional(condition, own, other.encode('utf-8')), 1)
        if key != '4310':
            for other, own in POSSESSIVES.items():
                current = re.sub(r'\b' + other + r'\b',
                                 lambda _: conditional(condition, own, other.encode('utf-8')),
                                 current)
        if current != translated:
            changes[key] = current
    key = None
    lines = []
    for line in raw.splitlines(keepends=True):
        if match := re.match(r'^  "(\d+)": \{', line):
            key = match.group(1)
        if key in changes and line.lstrip().startswith('"translation":'):
            line = '    "translation": ' + json.dumps(changes[key], ensure_ascii=False) + '\n'
        lines.append(line)
    PATH.write_text(''.join(lines), encoding='utf-8')
    print('Updated', len(changes), 'suffix rows')


if __name__ == '__main__':
    main()
