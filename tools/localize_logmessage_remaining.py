"""Finish player-specific LogMessage clauses that need whole-sentence rewrites."""

import json
import re

from localize_logmessage_perspective import PATH, TAG, se_int


SELF = {
    19: 'Il tuo accesso a “{@1}” è stato revocato.',
    67: '{@0}La tua espulsione dal gruppo.',
    91: 'Il tuo invito alla linkshell “{@0}” è stato ritirato.',
    171: 'Provi fastidio per {@1}.',
    209: 'I tuoi occhi si riempiono di lacrime.',
    252: 'Provi una profonda delusione.',
    267: 'I tuoi occhi cominciano a riempirsi di lacrime.',
    439: 'Distruggi {@1}.',
    449: 'Critico! La tua azione {@0} ripristina {@2}{@3} HP a {@4}.',
    454: 'Il tuo attacco viene riflesso da {@1}.',
    536: '   La tua ostilità aumenta.',
    540: 'La tua azione {@0} si interrompe.',
    541: 'La tua azione {@0} si interrompe.',
    542: "L'uso della tua azione {@0} si interrompe.",
    543: 'Bersaglio fuori portata. La tua azione {@0} viene annullata.',
    544: 'Bersaglio fuori portata. La tua azione {@0} viene annullata.',
    545: "Bersaglio fuori portata. L'uso della tua azione {@0} viene annullato.",
    546: 'Il bersaglio è privo di sensi. La tua azione {@0} viene annullata.',
    547: 'Il bersaglio è privo di sensi. La tua azione {@0} viene annullata.',
    548: "Il bersaglio è privo di sensi. L'uso della tua azione {@0} viene annullato.",
    556: 'La tua vita terminerà tra {@1} {@2}.',
    558: 'Sconfiggi {@1}.',
    559: 'Subisci una sconfitta.',
    560: 'Torni in vita.',
    601: 'La tua azione {@0} ripristina {@2}{@3} HP a {@4}.',
    602: 'La tua azione {@0} ripristina {@2}{@3} MP a {@4}.',
    617: 'Vieni distrutto.',
    793: "Non riesci a tingere l'oggetto.",
    1336: 'Il tuo compagno si ritira dalla battaglia.',
    1894: 'La compagnia libera ti espelle.',
    2260: 'Il fluido digestivo ti ricopre!',
    2879: 'La neve ti seppellisce!',
    3075: 'Assumi il ruolo di “{@1}”.',
    3144: 'Ottieni la nomina a {@1}.',
    3542: 'Non riesci a scoprire nulla usando {@1}.',
    4325: 'La tua abilità di desintesi per {@1} aumenta di {@2}.{@3}!',
    4326: 'La tua abilità di desintesi per {@1} diminuisce di {@2}.{@3}!',
    4616: "Ricevi un invito nell'alleanza.",
    4617: "Entri nell'alleanza.",
    5508: 'Riesci a recuperare alcuni ingredienti.',
    5625: 'La fellowship ti aggiunge alla lista nera.',
    5644: 'Il tuo invito alla fellowship è stato ritirato.',
    5904: 'La tua sintesi di prova di {@0} è fallita...',
    5915: 'La tua sintesi di {@0} è fallita a causa della qualità insufficiente.',
    5917: 'La tua sintesi di prova di {@0} è fallita a causa della qualità insufficiente.',
    7631: 'Ti connetti alla squadra PvP.',
    7632: 'Ti disconnetti dalla squadra PvP.',
    8065: 'Non sai che pensare.',
    8069: 'Sei raggiante.',
    8083: 'Sei affascinato.',
    8102: '{@1} ti coglie di sorpresa.',
    8103: 'La sorpresa ti coglie.',
    8123: 'Ti perdi nei tuoi pensieri.',
    8149: 'Mostri evidente disappunto.',
    8210: 'Mostri chiaramente la tua furia verso {@1}.',
    8211: 'Mostri chiaramente la tua furia.',
    8236: 'I tuoi occhi brillano di meraviglia.',
    8237: 'I tuoi occhi brillano di meraviglia verso {@1}.',
    8349: 'Getti a terra {@1}.',
    9219: 'I tuoi PV scendono a {@1}.',
    9275: 'Il tuo invito alla linkshell cross-world «{@0}» è stato revocato.',
    9280: 'La tua espulsione da «{@1}».',
    9725: 'Ora non puoi parlare.',
    10841: 'Raggiungi il luogo della missione per l’operazione sui mech «{@1}».',
    10971: 'Battle High ti pervade.',
}


def se_bytes(template, tags):
    result = bytearray()
    position = 0
    for match in re.finditer(r'\{@(\d+)\}', template):
        result.extend(template[position:match.start()].encode('utf-8'))
        result.extend(bytes.fromhex(tags[int(match.group(1))]))
        position = match.end()
    result.extend(template[position:].encode('utf-8'))
    return bytes(result)


def main():
    raw = PATH.read_text(encoding='utf-8')
    rows = json.loads(raw)
    updates = {}
    for index, template in SELF.items():
        row = rows[str(index)]
        original = row['original']
        current = row['translation']
        if current.startswith('<hex:0208'):
            continue
        actor = next((match for match in TAG.finditer(original)
                      if match.group(1).startswith('022B') and 'FF04796F75' in match.group(1)), None)
        condition = next((part for part in (bytes.fromhex('E4EB02EB03'), bytes.fromhex('E4EB02EB04'))
                          if actor and part in bytes.fromhex(actor.group(1))), None)
        if not condition:
            print('No player condition:', index)
            continue
        tags = TAG.findall(current)
        try:
            own = se_bytes(template, tags)
            other = se_bytes(re.sub(r'<hex:[0-9A-F]+>', lambda m: '{@' + str(tags.index(m.group()[5:-1])) + '}', current), tags)
            body = condition + b'\xFF' + se_int(len(own)) + own + b'\xFF' + se_int(len(other)) + other
            updates[str(index)] = '<hex:' + (
                b'\x02\x08' + se_int(len(body)) + body + b'\x03'
            ).hex().upper() + '>'
        except (IndexError, ValueError) as error:
            print('Failed:', index, error)
    key = None
    lines = []
    for line in raw.splitlines(keepends=True):
        if match := re.match(r'^  "(\d+)": \{', line):
            key = match.group(1)
        if key in updates and line.lstrip().startswith('"translation":'):
            line = '    "translation": ' + json.dumps(updates[key], ensure_ascii=False) + '\n'
        lines.append(line)
    PATH.write_text(''.join(lines), encoding='utf-8')
    print('Updated', len(updates), 'remaining rows')


if __name__ == '__main__':
    main()
