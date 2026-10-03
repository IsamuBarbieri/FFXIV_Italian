# Guida di Stile e Convenzioni di Traduzione: FFXIV Italiano

Questo documento definisce i principi editoriali, la gestione dei registri dei personaggi e le regole grammaticali di riferimento per la localizzazione italiana di **Final Fantasy XIV**.

La fonte canonica di riferimento terminologico è costituita da `data/glossary/Glossary.md`.

Per le categorie di attività usa le equivalenze raccolte nel glossario: Missione, Missione secondaria, Incarico, Mandato, Prova, Incursione, Spedizione, Cripta Profonda, Operazione di Gilda e FATE. Mantieni distinti i tipi di attività. Applica la regola al significato di gioco, non ai nomi propri o agli usi comuni delle stesse parole inglesi.

---

## 1. Principi di Base

### Regola 0: Maiuscole e minuscole
Riproduci nella traduzione le maiuscole iniziali presenti nell'originale, parola per parola. Mantieni minuscoli gli articoli italiani interni alla frase, comprese le forme articolate (*del, dello, della, dei, degli, delle*); la prima parola resta maiuscola quando anche la stringa originale inizia con una maiuscola. Esempio: *Order of the Twin Adder* → **Ordine della Vipera Gemella**.

### Regola A: Adattamento per Videogioco (Niente Doppioni a Schermo)
Nel client di gioco non si usa la notazione `Italiano (Inglese)` (*es. non scriviamo "Baia del Vespro (Vesper Bay)" nei dialoghi o nei menu*). Il testo deve essere **esclusivamente in italiano**, scorrevole, naturale e perfettamente dimensionato per i box dell'interfaccia.

Nei fogli dei gradi delle Grandi Compagnie usa un riferimento breve alla fazione (*Capitano della Fiamma*, *Sottotenente della Tempesta*, *Sottomaresciallo del Serpente*). Traduci anche `col_2` con il nome comune del grado in minuscolo (*capitano*, *sergente*); scegli la forma femminile nel foglio dedicato quando il grado la prevede.

Nelle etichette di categoria di negozi e menu, non ripetere un'azione già implicita nella schermata: per esempio, *Purchase Items* può diventare **Oggetti**, non «Acquista Oggetti». Mantieni il verbo quando l'etichetta descrive davvero un'azione o un comando. Per gli stati che indicano un'attività in corso, usa il gerundio quando è naturale (*Interacting* → **Interagendo**). Nei nomi delle meccaniche, *right/left* indica destra/sinistra, non «giusto/lasciato»: identifica il referente e accorda la direzione (*Destra Mielata*, *Zantetsuken Destro*).

### Regola B: Nomi e toponimi
Per le rese dei nomi e dei luoghi usa le forme documentate in `data/glossary/Glossary.md`; per tutti i luoghi consulta anche il catalogo integrale `data/translations/world/placename.json` (`name` → `translation`). Per i nomi non presenti, verifica la traduzione prima di introdurre una nuova convenzione.

Per la classe artigiana *Armorer* usa **Corazziere** (sigla italiana **COR**), come nel glossario. Mantieni i nomi propri nella forma canonica già attestata: per esempio **Alexandria** non diventa «Alessandria».

### Regola C: Ricerca della Lore e del Contesto Culturale (Wiki & Lorebook)
I nomi, i toponimi, i mostri e i titoli delle missioni non devono **mai** essere tradotti in modo isolato o letterale. È obbligatorio verificare la lore del mondo di gioco attraverso le fonti canoniche (Wiki di FFXIV, Gamer Escape, ConsoleGamesWiki, Lodestone ed *Encyclopaedia Eorzea*):
1. **Origine ed Etimologia Culturale di Razza e Fazione**:
   - Comprendere *perché* un luogo, un NPC o una fazione porta quel nome nella cultura eorzeana (es. convenzioni linguistiche di razza: nomi Roegadyn Guardia dell'Inferno composti descrittivi vs Lupi di Mare in lingua antica; nomi Garleani con gradi latini; toponimi Elezen a Ishgard).
2. **Mostri, Bestiario e Creature Mitologiche**:
   - Verificare l'origine folklorica o classica di ogni creatura per scegliere se mantenerla (es. mostri classici del franchise o della mitologia: *Behemoth, Malboro, Ahriman, Coeurl*) o tradurne gli epiteti descrittivi (*"Stray Coblyn"*, *"Dredge Peiste"*).
3. **Citazioni, Proverbi e Giochi di Parole (Quest & Dialoghi)**:
   - I titoli delle missioni e le battute dell'autore (Koji Fox & team) traboccano di giochi di parole, citazioni teatrali, titoli musicali e proverbi. Verificare sulla wiki la pagina della missione rivela quasi sempre il retroscena o il gioco di parole, consentendo di creare un equivalente italiano brillante invece di una traduzione piatta.
4. **Contesto Scenico e Geopolitico**:
   - Verificare chi partecipa alla scena, il tono drammatico o comico e l'atmosfera della fazione (es. il cinismo mercantilista di Ul'dah, la sacralità animista di Gridania, la parlata marinaresca e rozza di Limsa Lominsa).

### Regola D: Divieto di sostituzione meccanica e accordo sintattico obbligatorio
Quando si allinea un termine al glossario (o durante una correzione terminologica), è **vietato** sostituire la parola in modo cieco/meccanico. Bisogna sempre rileggere la frase e accordare articoli e preposizioni articolate al genere e numero del nuovo termine italiano:
- *Velo Nero* è maschile: **il Velo Nero**, **del Velo Nero**, **nel Velo Nero** (MAI *«della Velo Nero»*, *«la Velo Nero»*, *«nella Velo Nero»*).
- *Vecchia Sharlayan* vuole la preposizione semplice: **di Vecchia Sharlayan** o **della Vecchia Sharlayan** (MAI *«del Vecchia Sharlayan»*).
- *Officine Ferrocielo* è plurale: **dalle Officine Ferrocielo** (MAI *«dalla Officine»*).
- *Cratere delle Braci* è maschile: **al Cratere delle Braci** (MAI *«alla Cratere»*).
- *Prove di Bardam* richiede l'articolo articolato: **alle Prove di Bardam** (MAI *«a Prove di Bardam»*).

### Regola E: Nomi propri dei Loporrit invariati
Tutti i nomi propri dei Loporrit che terminano in *-way* (*Livingway, Growingway, Cookingway, Mappingway, Piercingway, Fusingway, Searchingway, Reportingway*, ecc.) sono nomi propri invariabili e **non si traducono mai** (vietato tradurre "-way" in "-via", es. vietato *«Trafiggivia»* o *«Fondivia»*). Le strutture a essi intitolate mantengono il nome proprio invariato: **la Galleria di Piercingway**, **il Condotto di Fusingway**.

### Regola F: Nessun prestito orfano e coerenza tra campi
Non lasciare mai parole o frammenti in inglese residui nei testi tradotti (es. *plate* al posto di *piastra* nelle spiegazioni dei comandi o delle finestre). Inoltre, garantisci sempre la coerenza interna tra nome dell'oggetto e descrizione nei file come `buddyequip.json` o `item.json`.

---

## 2. Risoluzione della Concordanza di Genere

La lingua italiana richiede l'accordo di genere (*maschile/femminile*) per participi e aggettivi.

### A. Personaggio Giocante (Guerriero/a della Luce)
1. Ove possibile, sfruttare i tag nativi `SeString` del gioco (`<If(PlayerParameter(Gender)...)>`) per mostrare dinamicamente la forma maschile o femminile in tempo reale.
2. In alternativa, formulare la frase in stile **epiceno** (neutro naturale):
   - *Invece di*: "Sono felice che tu sia tornato sano e salvo."
   - *Usa*: "Che gioia sapere che sei incolume." / "È un sollievo rivederti qui."

### B. NPC (Chi parla e a chi ci si riferisce)
Il database `ENpcBase` contiene il flag binario del genere (`0 = Maschio, 1 = Femmina`). Il nostro estrattore inietta automaticamente questi metadati nel JSON di traduzione.
- Se l'NPC destinatario è donna: *"La signora Kan-E-Senna è giunta"* / *"È stata molto chiara"*.
- Se l'NPC è uomo: *"Il comandante Raubahn è arrivato"* / *"È stato molto chiaro"*.

---

## 3. Matrice dei Registri Vocali dei Personaggi Principali

| Personaggio | Stile & Registro | Tratti Chiave | Linee Guida di Scrittura |
| :--- | :--- | :--- | :--- |
| **Urianger Augurelt** | **Aulico, Solenne, Arcaico** | Volgare illustre, cadenza poetica nobile, congiunzioni nobili (*onde, allorché, perocché, vostra mercé*). | Non renderlo ridicolo o caricaturale. Deve suonare come un filosofo colto e devoto. Congiuntivi e condizionali impeccabili. |
| **Thancred Waters** | **Brillante, Spigliato, Ironico** | Carismatico, battuta pronta, disinvoltura elegante, lealtà profonda celata da apparente leggerezza. | Frasi scorrevoli, ritmo veloce, humor asciutto. Mai volgare, ma informale ed empatico. |
| **Alphinaud Leveilleur** | **Diplomatico, Accademico, Eloquente** | Sintassi nobile e complessa, oratore idealista; matura da giovane supponente a leader umile e riflessivo. | Vocabolario forbito, argomentazioni strutturate, rispetto formale per gli interlocutori. |
| **Alisaie Leveilleur** | **Diretta, Incisiva, Appassionata** | Tagliente, impaziente verso i convenevoli e la retorica; coraggiosa, emotiva, pragmatica. | Frasi brevi e decise. Detesta i giri di parole. Energia palpabile. |
| **Y'shtola Rhul** | **Flessibile, Intellettuale, Sferzante (*Sassy*)** | Calma olimpica, arguzia tagliente a mezza bocca, tono sereno con frecciate micidiali sussurrate con grazia. | Autorevolezza tranquilla. Non perde mai la calma; le sue battute sono bordate elegantissime. |
| **Tataru Taru** | **Solare, Vivace, Amministrativa** | Squillante, uso affettuoso di vezzeggiativi; serissima e intransigente quando si toccano le finanze. | Solare e premurosa con i compagni, ma severa come un esattore delle tasse con le ricevute di spesa. |
| **Estinien Sanguedidrago** | **Laconico, Guerriero, Asciutto** | Pochissime parole, nessun fronzolo o cerimonia; pragmatismo militare e lealtà silenziosa. | Nessuna chiacchiera inutile. Dialoghi asciutti, diretti al punto. |
| **Emet-Selch** | **Teatrale, Disincantato, Tragico** | Carisma drammatico, stanchezza millenaria, sarcasmo aristocratico, dolore straziante celato dalla condiscendenza. | Tono teatrale e aristocratico ("Miei cari..."). Ogni battuta è venata di tragica superiorità e nostalgia. |
| **Haurchefant Pietragrigia** | **Cavalieresco, Caloroso, Esuberante** | Entusiasmo contagioso, affetto sincero e appassionato per il Guerriero della Luce, fedeltà incrollabile. | Fervore cavalleresco solare. Accoglienza calorosa ed enfatica ("Uno splendido spettacolo!"). |

---
