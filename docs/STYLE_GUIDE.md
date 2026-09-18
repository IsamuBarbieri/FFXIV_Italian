# Guida di Stile e Convenzioni di Traduzione: FFXIV Italiano

Questo documento definisce i principi editoriali, la gestione dei registri dei personaggi e le regole grammaticali di riferimento per la localizzazione italiana di **Final Fantasy XIV**.

La fonte canonica di riferimento terminologico è costituita da `data/glossary/07_Glossary.md` (v1.43).

---

## 1. Principi di Base

### Regola A: Adattamento per Videogioco (Niente Doppioni a Schermo)
Nel client di gioco non si usa la notazione `Italiano (Inglese)` (*es. non scriviamo "Baia del Vespro (Vesper Bay)" nei dialoghi o nei menu*). Il testo deve essere **esclusivamente in italiano**, scorrevole, naturale e perfettamente dimensionato per i box dell'interfaccia.

### Regola B: Trasparenza Semantica dei Nomi (G1)
1. **Nomi Oparchi o Culturali (INVARIATI)**:
   - Nomi e cognomi di popoli o lingue inventate non si toccano: *Alphinaud Leveilleur*, *Thancred Waters*, *Y'shtola Rhul*, *Urianger Augurelt*, *Merlwyb Bloefhiswyn*, *Kan-E-Senna*, *Raubahn Aldynn*, *Nanamo Ul Namo*.
   - Capitali e macro-regioni: *Eorzea*, *Limsa Lominsa*, *Gridania*, *Ul'dah*, *Ishgard*, *Kugane*, *Garlemald*.
2. **Cognomi ed Epiteti Trasparenti con Significato (TRADURRE)**:
   - Quando una parola inglese comune (pianta, metallo, colore, azione) è usata come cognome o soprannome, si traduce per mantenere l'intento dell'autore:
     - *Haurchefant Greystone* -> **Haurchefant Pietragrigia** (il cognome bastardo nobile di Coerthas).
     - *Estinien Wyrmblood* -> **Estinien Sanguedidrago** (il titolo/lignaggio del Dragone Azzurro).
     - *Baderon Tenfingers* -> **Baderon Diecidita**.
     - *Gerolt Blackthorn* -> **Gerolt Spinanera**.
     - *Nedrick Ironheart* -> **Nedrick Cuordiferro**.
3. **Toponimi Descrittivi (TRADURRE)**:
   - *Aleport* -> **Portobirra** | *Wineport* -> **Portovino** | *Lakeland* -> **Terralago** | *Quarrymill* -> **Cavamulino**.
   - *The Waking Sands* -> **Sabbie del Risveglio** | *The Rising Stones* -> **Le Pietre Risorte**.
   - *The Drowning Wench* -> **La Fanciulla Annegata** | *The Carline Canopy* -> **Il Baldacchino di Carline** | *Buscarron's Druthers* -> **Il Capriccio di Buscarron**.

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

## 4. Regole Ferree di Glossario (Anti-Errori Comuni)

1. **La Tempesta (G10)**: La Grande Compagnia di Limsa Lominsa è **La Tempesta** (MAI *"Maelstrom"* o *"Il Maelstrom"*).
2. **Figli della Settima Alba (G10)**: L'organizzazione degli Scions è **I Figli della Settima Alba**.
3. **Mal d'Etere (G7)**: Il debuff da teletrasporto/resurrezione è **Mal d'Etere** (MAI *"Malattia Eterica"*).
4. **Unità di Misura (G28)**: *yalm*, *fulm*, *ilm*, *malm*, *ponze*, *onze*, *tonze* sono **invariabili** e **non vanno mai tradotte** in metri o iarde (vietato *"iarde"* o *"yalms"*).
5. **Abilità e Magie (G24)**: I nomi iconici di mosse e incantesimi di franchise rimangono invariati (*Fire, Blizzard, Cure, Limit Break*); **le descrizioni, gli effetti e i tooltip vengono interamente tradotti in italiano**.

