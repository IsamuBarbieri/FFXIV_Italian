import sys
sys.stdout.reconfigure(encoding='utf-8')
import json
import glob
import re

# Comprehensive list of common nouns with unambiguous gender
FEM_SING = {
    'spada', 'lancia', 'ascia', 'verga', 'bacchetta', 'pietra', 'tavoletta', 'morte',
    'vita', 'vittoria', 'battaglia', 'guerra', 'terra', 'notte', 'luce', 'forza',
    'voce', 'mente', 'cosa', 'persona', 'gente', 'donna', 'ragazza', 'madre',
    'città', 'selva', 'foresta', 'montagna', 'strada', 'via', 'casa', 'porta',
    'notizia', 'storia', 'fine', 'pace', 'anima', 'cura', 'difesa', 'mira', 'furia',
    'pozione', 'ferita', 'arma', 'armatura', 'corazza', 'cotta', 'maglia', 'tunica',
    'veste', 'giacca', 'giubba', 'cintura', 'fascia', 'sciarpa', 'maschera', 'visiera',
    'moneta', 'stella', 'luna', 'creatura', 'bestia', 'bestiaccia', 'pianta', 'radice'
}

MASC_SING = {
    'colpo', 'fuoco', 'nemico', 'soldato', 'guerriero', 'popolo', 'paese', 'luogo',
    'posto', 'mondo', 'tempo', 'modo', 'fatto', 'caso', 'giorno', 'anno', 'pezzo',
    'piano', 'punto', 'regno', 'sangue', 'cuore', 'corpo', 'fiume', 'vento', 'cielo',
    'mare', 'sole', 'ferro', 'oro', 'argento', 'bronzo', 'legno', 'libro', 'arco',
    'dardo', 'pugnale', 'gladio', 'scudo', 'bastone', 'elmo', 'diadema', 'anello',
    'bracciale', 'stocco', 'maglio', 'piccone', 'mostro', 'demone', 'drago', 'lupo'
}

FEM_PLUR = {
    'spade', 'lance', 'asce', 'verghe', 'bacchette', 'pietre', 'tavolette', 'morti',
    'vite', 'vittorie', 'battaglie', 'guerre', 'terre', 'notti', 'luci', 'forze',
    'voci', 'menti', 'cose', 'persone', 'donne', 'ragazze', 'madri', 'selve',
    'foreste', 'montagne', 'strade', 'vie', 'case', 'porte', 'notizie', 'storie',
    'anime', 'macerie', 'scarpe', 'calighe', 'bende', 'ghette', 'armi', 'armature',
    'corazze', 'cotte', 'maglie', 'tuniche', 'vesti', 'giacche', 'giubbe', 'cinture',
    'fasce', 'sciarpe', 'maschere', 'visiere', 'monete', 'stelle', 'lune', 'creature',
    'bestie', 'piante', 'radici', 'pozioni', 'ferite'
}

MASC_PLUR = {
    'colpi', 'fuochi', 'nemici', 'soldati', 'guerrieri', 'popoli', 'luoghi', 'posti',
    'mondi', 'tempi', 'modi', 'fatti', 'casi', 'giorni', 'anni', 'pezzi', 'piani',
    'punti', 'regni', 'fiumi', 'venti', 'cieli', 'mari', 'libri', 'archi', 'dardi',
    'pugnali', 'gladi', 'scudi', 'bastoni', 'elmi', 'diademi', 'anelli', 'bracciali',
    'stocchi', 'magli', 'picconi', 'stivali', 'guanti', 'pantaloni', 'calzari',
    'mostri', 'demoni', 'draghi', 'lupi'
}

patterns = [
    # 1. Determiner clashes
    ('questo + fem_sing', re.compile(r'\bquesto\s+(' + '|'.join(FEM_SING) + r')\b', re.I)),
    ('questo + plurale', re.compile(r'\bquesto\s+(' + '|'.join(MASC_PLUR | FEM_PLUR) + r')\b', re.I)),
    ('questa + masc_sing', re.compile(r'\bquesta\s+(' + '|'.join(MASC_SING) + r')\b', re.I)),
    ('questa + plurale', re.compile(r'\bquesta\s+(' + '|'.join(MASC_PLUR | FEM_PLUR) + r')\b', re.I)),
    ('questi + fem_sing', re.compile(r'\bquesti\s+(' + '|'.join(FEM_SING) + r')\b', re.I)),
    ('questi + fem_plur', re.compile(r'\bquesti\s+(' + '|'.join(FEM_PLUR) + r')\b', re.I)),
    ('queste + masc_sing', re.compile(r'\bqueste\s+(' + '|'.join(MASC_SING) + r')\b', re.I)),
    ('queste + masc_plur', re.compile(r'\bqueste\s+(' + '|'.join(MASC_PLUR) + r')\b', re.I)),
    ('il + fem_sing', re.compile(r'\bil\s+(' + '|'.join(FEM_SING) + r')\b', re.I)),
    ('la + masc_sing', re.compile(r'\bla\s+(' + '|'.join(MASC_SING) + r')\b', re.I)),
    ('i + fem_plur', re.compile(r'\bi\s+(' + '|'.join(FEM_PLUR) + r')\b', re.I)),
    ('le + masc_plur', re.compile(r'\ble\s+(' + '|'.join(MASC_PLUR) + r')\b', re.I)),
    ('un + fem_sing (no apostrofo)', re.compile(r'\bun\s+(' + '|'.join(FEM_SING) + r')\b', re.I)),
    ('una + masc_sing', re.compile(r'\buna\s+(' + '|'.join(MASC_SING) + r')\b', re.I)),
    
    # 2. Prendi questo / Beccati questo followed by plural
    ('prendi questo + plurale', re.compile(r'\b(?:prendi|beccati|assaggia)\s+questo\s*[,!.]+\s*([a-zA-Z\’\']+)', re.I)),
    
    # 3. Take this / Take that in English
    ('take this/that in orig', re.compile(r'\btake\s+(?:this|that)\b', re.I)),
]

files = sorted(glob.glob('data/da_revisionare/**/*.json', recursive=True))

for fpath in files:
    with open(fpath, 'r', encoding='utf-8-sig') as f:
        data = json.load(f)
    
    found_matches = []
    for item_id, item_data in data.items():
        if not isinstance(item_data, dict):
            continue
        orig = item_data.get('original') or item_data.get('name') or ''
        
        # Check take this / take that
        if orig and re.search(r'\btake\s+(?:this|that)\b', orig, re.I):
            tr = item_data.get('translation') or item_data.get('translation_name') or ''
            found_matches.append((item_id, 'take_this_check', f"ORIG: '{orig}' | TR: '{tr}'"))

        # Check grammatical clashes
        for field, val in item_data.items():
            if not field.startswith('translation') or not isinstance(val, str):
                continue
            for p_name, p_re in patterns:
                if p_name.startswith('take'):
                    continue
                m = p_re.search(val)
                if m:
                    # Filter out False Positives
                    matched_str = m.group(0).lower()
                    # e.g., "fine" can be masculine or feminine ("al fine di", "alla fine")
                    if 'fine' in matched_str and ('il fine' in matched_str or 'questo fine' in matched_str):
                        continue
                    # e.g., "terra" in "la terra" is correct
                    found_matches.append((item_id, p_name, f"MATCH: '{m.group(0)}' in '{val[:90]}'"))
    
    if found_matches:
        print(f"=== {fpath} ({len(found_matches)} matches) ===")
        for m in found_matches:
            print(f"  ID {m[0]} [{m[1]}]: {m[2]}")
