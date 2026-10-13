# Guida di Stile e Convenzioni di Traduzione: FFXIV Italiano

Questo documento definisce i principi editoriali, la gestione dei registri dei personaggi e le regole grammaticali di riferimento per la localizzazione italiana di **Final Fantasy XIV**.

La fonte canonica di riferimento terminologico è costituita da `data/glossary/Glossary.md`. Le annotazioni «Controllo automatico» nel glossario attivano verifiche mirate: il validatore segnala le voci da controllare, ma non riscrive né corregge i file.

Per le categorie di attività usa le equivalenze raccolte nel glossario: Missione, Missione secondaria, Incarico, Mandato, Prova, Incursione, Spedizione, Cripta Profonda, Operazione di Gilda e FATE. In particolare, *Dungeon* come attività istanziata è **Spedizione**; se descrive l'ambiente fisico, per esempio l'ingresso o l'interno, è **sotterraneo**. Le funzioni di gioco che concludono un Incarico e fanno uscire dal dungeon si riferiscono all'attività e usano «spedizione». Mantieni distinti attività, luoghi fisici, nomi propri e usi comuni.

---

## 1. Principi di Base

### Regola 0: Maiuscole e minuscole
Riproduci le maiuscole iniziali delle parole presenti nell'originale e conserva anche un eventuale iniziale minuscola: non trasformare automaticamente una frase che comincia con una minuscola in un titolo. Mantieni minuscoli gli articoli italiani interni alla frase, comprese le forme articolate (*del, dello, della, dei, degli, delle*). Esempio: *Order of the Twin Adder* → **Ordine della Vipera Gemella**; *letter to Goberin* → **lettera a Goberin**.

### Regola A: Adattamento per Videogioco (Niente Doppioni a Schermo)
Nel client di gioco non si usa la notazione `Italiano (Inglese)` (*es. non scriviamo "Baia del Vespro (Vesper Bay)" nei dialoghi o nei menu*). Il testo deve essere **esclusivamente in italiano**, scorrevole, naturale e perfettamente dimensionato per i box dell'interfaccia.

Nei fogli dei gradi delle Grandi Compagnie usa un riferimento breve alla fazione (*Capitano della Fiamma*, *Sottotenente della Tempesta*, *Sottomaresciallo del Serpente*). Traduci anche `col_2` con il nome comune del grado in minuscolo (*capitano*, *sergente*); scegli la forma femminile nel foglio dedicato quando il grado la prevede.

Nelle etichette di categoria di negozi e menu, non ripetere un'azione già implicita nella schermata: per esempio, *Purchase Items* può diventare **Oggetti**, non «Acquista Oggetti». Mantieni il verbo quando l'etichetta descrive davvero un'azione o un comando. Per gli stati che indicano un'attività in corso, usa il gerundio quando è naturale (*Interacting* → **Interagendo**). Nei nomi delle meccaniche, *right/left* indica destra/sinistra, non «giusto/lasciato»: identifica il referente e accorda la direzione (*Destra Mielata*, *Zantetsuken Destro*).
Per la statistica di potenza dell'equipaggiamento, il termine completo è **Livello Oggetto** (*Item Level*); nelle etichette di negozi, menu e interfaccia con vincoli di spazio si impiega sistematicamente l'abbreviazione canonica **Liv.Ogg.** (es. *Liv.Ogg. 720*), rispecchiando le convenzioni delle altre localizzazioni europee (*NvObj* in francese, *G.-St.* in tedesco) e l'uso canonico in `system/addon.json`.

### Regola B: Nomi e toponimi
Per le rese dei nomi e dei luoghi usa le forme documentate in `data/glossary/Glossary.md`; per tutti i luoghi consulta anche il catalogo integrale `data/translations/world/placename.json` (`name` → `translation`). Per i nomi non presenti, verifica la traduzione prima di introdurre una nuova convenzione.

Quando un nome di personaggio è formato da parole inglesi comuni che, nel contesto attestato, funzionano da cognome descrittivo, soprannome o appellativo, adattane il significato in italiano. Conserva il nome proprio non descrittivo e accorda la costruzione alla sintassi italiana: *Edda Blackbosom* → **Edda Cuorenero**, *Blackbosom's attire* → **abito di Cuorenero**. Applica il criterio a PG, PNG e personaggi secondari; verifica lore e correzioni canoniche prima di scegliere la resa. Non tradurre automaticamente cognomi arbitrari, nomi con una forma canonica invariata o composti la cui funzione non è chiara: in caso di dubbio, segnala l'ambiguità.

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
- *Tomestone* è femminile: **la tua tavoletta**, **tavoletta allagana**, **tavolette allagane**. Quando il termine sostituisce un nome in una frase, correggi anche articolo, possessivo, aggettivo, numero e verbo.

### Regola E: Nomi propri dei Loporrit invariati
Tutti i nomi propri dei Loporrit che terminano in *-way* (*Livingway, Growingway, Cookingway, Mappingway, Piercingway, Fusingway, Searchingway, Reportingway*, ecc.) sono nomi propri invariabili e **non si traducono mai** (vietato tradurre "-way" in "-via", es. vietato *«Trafiggivia»* o *«Fondivia»*). Come campo nome, usa **Galleria di Piercingway** senza aggiungere l'articolo assente nell'originale; nella prosa inseriscilo solo se richiesto dalla sintassi italiana. Usa **Condotto di Fusingway** con lo stesso criterio.

### Regola F: Nessun prestito orfano e coerenza tra campi

Le correzioni censite in [TRANSLATION_CENSUS.md](TRANSLATION_CENSUS.md) richiedono di identificare il referente prima del lemma: *Arms* nelle insegne è «Stemma»; *range* può indicare una soglia di livelli invece della portata; gli *stack* degli effetti sono «accumuli». Mantieni distinte tecniche d'arma, abilità, incantesimi e azioni istintive. Le sigle ATT/DIF indicano statistiche nei campi tecnici, non sostituiscono ogni uso di attacco/difesa. Per una dipendenza numerica preferisci «varia in base a» al calco «scala con», senza inventare la formula.

Gli identificatori letterali dei comandi (*/party, /waymark, /cwlinkshell1*) restano invariati; nelle spiegazioni usa gruppo, marcatore tattico e fonoperla. Il divieto di residui inglesi non autorizza a tradurre la sintassi eseguibile. Nell'interfaccia rivolta al singolo giocatore usa il tu; per annunci a un gruppo o battute di personaggi conserva il destinatario e il registro attestati.
Non lasciare mai parole o frammenti in inglese residui nei testi tradotti (es. *plate* al posto di *piastra* nelle spiegazioni dei comandi o delle finestre). Inoltre, garantisci sempre la coerenza interna tra nome dell'oggetto e descrizione nei file come `buddyequip.json` o `item.json`.

Per gli oggetti, controlla ogni campo tradotto insieme al proprio originale: nome, descrizione, categoria e titolo possono richiedere forme diverse. Fai concordare articoli, aggettivi e plurali con il nome italiano (*gladio obsoleto / spatha obsoleta*; *chiave della Porta*, non «chiavi del Porta»). Nei composti di materiali evita la ripetizione involontaria di «di» (*grimorio in pelle di lupo*) e applica le forme del glossario anche nelle descrizioni. Le maiuscole dei nomi visualizzati seguono il campo e l'originale: non trasformare una forma minuscola di prosa in un titolo.

Le correzioni automatiche sono ammesse solo quando l'intera frase sorgente coincide con una frase approvata. Per un termine inserito in una frase più ampia, il validatore segnala un candidato: rileggi originale e traduzione, poi adatta la resa al contesto e alla grammatica. Il validatore non applica sostituzioni ai file.

### Regola G: Contenuti non tradotti o a formato fisso (Crediti, Brani musicali)
Per decisione editoriale, alcuni contenuti mantengono la versione originale e non devono essere tradotti:
- **Crediti di gioco (`system/creditlisttext.json`)**: i crediti di gioco non vanno mai tradotti. Il campo `translation` deve coincidere identico e tale e quale con l'originale (`original`); il validatore verifica l'uguaglianza.
- **Titoli musicali dell'Orchestrion (`housing/orchestrion.json`)**: i titoli dei brani (`name` / `translation_name`) conservano i titoli originali delle musiche e non vanno mai tradotti; si traducono esclusivamente i testi descrittivi (`description` / `translation_description`). Anche qui il validatore verifica che il titolo resti identico.

### Regola H: Unità di misura e pesi eorzeani
Le unità di misura di Eorzea seguono un adattamento morfologico preciso e regolare stabilito per decisione editoriale:
- **Distanza e Lunghezza (genere maschile)**: mantengono la desinenza in *-m* al singolare e prendono la desinenza in *-mi* al plurale:
  - *ilm* (sing.) / *ilmi* (plur.) [pollice]
  - *fulm* (sing.) / *fulmi* (plur.) [piede]
  - *yalm* (sing.) / *yalmi* (plur.) [iarda] (es. *«dieci maledetti yalmi»*)
  - *malm* (sing.) / *malmi* (plur.) [miglio]
- **Peso (genere femminile)**: si adattano come nomi femminili della prima declinazione in *-a* al singolare e in *-e* al plurale:
  - *onza* (sing.) / *onze* (plur.) [oncia]
  - *ponza* (sing.) / *ponze* (plur.) [libbra]
  - *tonza* (sing.) / *tonze* (plur.) [tonnellata]
Quando il riferimento alla misura è letterale, mantieni l'unità e accorda articoli e aggettivi secondo genere e numero. Il validatore verifica la forma corrispondente; «qualche» regge correttamente il singolare anche con valore plurale (es. «qualche ilm»). Se la misura compare in un'espressione idiomatica inglese («every ilm the…», «within an ilm of one's life», «within an ilm of godhood», «I was malms away», «every last onze of…», «every onze of dedication», «a tonze of research left to do», «an ilm or two taller»), traducine il senso in modo naturale: non forzare un'unità che in italiano suonerebbe innaturale. Aggiungi al validatore un'eccezione mirata per ogni idiom verificato.


### Parlato delle Società Alleate e dei popoli affini

La denominazione ufficiale attuale è **Società Alleate** (*Allied Societies*); «tribù delle bestie» e «tribù alleate» sono nomi storici dei contenuti. Il roster ufficiale comprende queste venti società:

| Espansione | Società Alleate |
| :--- | :--- |
| A Realm Reborn | Amalj'aa, Sylph, Kobold, Sahagin, Ixal |
| Heavensward | Vanu Vanu, Vath, Moguri |
| Stormblood | Ananta, Kojin, Namazu |
| Shadowbringers | Pixie, Qitari, Nani |
| Endwalker | Arkasodara, Omicron, Loporrit |
| Dawntrail | Pelupelu, Mamool Ja, Yok Huy |

Le catene intersocietarie riuniscono le società di A Realm Reborn, Heavensward, Stormblood, Endwalker e Dawntrail; Shadowbringers non ne ha una. La lista segue la [banca dati ufficiale delle missioni del Lodestone](https://eu.finalfantasyxiv.com/lodestone/playguide/db/quest/) e la [suddivisione per espansione delle Società Alleate](https://ffxiv.consolegameswiki.com/wiki/Allied_Society_Quests). Goblin, Gnath, Qiqirn e Ondo sono popoli affini utili per la revisione dei dialoghi, ma non aggiungono società al roster.

| Popolo | Tratto da conservare nella resa italiana |
| :--- | :--- |
| **Amalj'aa** | Registro marziale e rituale quando presente; conserva richiami a fratellanza, fuoco e giuramenti se appartengono alla battuta. Niente grafia fonetica inventata. |
| **Sylph** | Mantieni la terza persona riferita a sé o al proprio gruppo (*this one / these ones*) e gli appellativi *walking one*. Usa in modo coerente **questo qui / questi qui** e **camminante / camminanti**, adattando articoli e frase alla sintassi italiana. |
| **Kobold** | Lessico concreto di miniera, fabbricazione, conteggio e scambio quando il testo lo richiama; italiano standard, senza storpiature aggiunte. |
| **Sahagin** | Conserva il soffio iniziale *psh* e le sibilanti allungate già marcate nell'originale. In italiano usa **sss** dentro parole adatte; non sostituirlo con vocali ripetute e non estenderlo a ogni parola. |
| **Ixal** | Preserva le inversioni sintattiche riconoscibili con qualche costruzione marcata ma chiara. Mantieni appellativi come *featherless one* con una resa coerente come **implume**; conserva immagini di piume, taloni e volo quando presenti. |
| **Moguri (Moogle)** | Rendi l'intercalare **kupo** con **kupò** quando compare nell'originale, nella posizione della battuta; non aggiungerlo come suffisso automatico a ogni frase. |
| **Vanu Vanu** | Mantieni tono oracolare, immagini di cielo e volo, l'autoreferenza in terza persona quando presente e l'appellativo **forestiero** per *netherling*. Evita di introdurre costruzioni anomale se la fonte non le mostra. |
| **Gnath dell'Onemind** | Quando la fonte attribuisce la battuta all'alveare, conserva il «noi» collettivo e la volontà condivisa; non trattare il parlante come un individuo isolato. |
| **Vath** | Rendi i segnali sonori espliciti come «clic» e «clac». I Vath del Nonmind hanno identità e obiettivi individuali: non confonderli con la voce collettiva dei Gnath dell'Onemind. |
| **Ananta** | Allunga le sibilanti quando l'originale le allunga; rendile con **sss** in parole italiane adatte, senza alterare parole prive di suoni sibilanti. |
| **Kojin** | Rispetta lessico e tono legati a tesori, scambi, onore e devozione se presenti; non assegnare un accento comune a tutti i Kojin. |
| **Namazu** | Conserva le ripetizioni brevi ed entusiaste dell'originale, in particolare **sì, sì** e **no, no**; non aggiungere riempitivi dove mancano. |
| **Pixie** | Mantieni malizia, giocosità e ironia proprie del singolo interlocutore; niente suffissi o deformazioni fonetiche di specie. |
| **Qitari** | Segui il tono di racconto e memoria degli antenati quando emerge dalla battuta; non inventare arcaismi o un accento uniforme. |
| **Nani** | Mantieni il saluto **Lali-ho** quando presente e traduci in italiano gli appellativi culturali per chi è senza barba. Conserva il tono schietto e bonario senza imitare un accento reale. |
| **Arkasodara** | Italiano standard; rendi il tono solidale, pratico o mercantile del singolo personaggio senza attribuire una parlata deformata all'intero popolo. |
| **Omicron** | Conserva il lessico analitico e tecnico e le formule di unità/macchina se espresse; non spezzare la grammatica italiana per farli sembrare robotici. |
| **Loporrit** | Le voci restano individuali, spesso energiche o ottimiste. I nomi propri in **-way** restano invariati secondo la Regola E. |
| **Pelupelu** | Mantieni cortesia, lessico commerciale e retorica promozionale quando presenti; niente accento inventato. |
| **Mamool Ja** | Rispetta differenze di carattere e contesto culturale tra parlanti; non applicare una grammatica tribale uniforme. |
| **Yok Huy** | Conserva il tono solenne, misurato o cerimoniale quando attestato; evita un italiano pseudoarcaico aggiunto. |
| **Goblin** *(popolo affine)* | Ricrea in italiano le parole composte e i giochi fonici che esistono nella fonte; mantieni interiezioni esplicite come *Pshhh* e non trasferire meccanicamente ogni composto. |
| **Ondo** *(popolo affine)* | Conserva le sibilanti allungate esplicite con **sss**, come per gli Ananta e i Sahagin; mantieni appellativi come *finless one* in forma italiana coerente. |
| **Qiqirn** *(popolo affine)* | Segui la voce del singolo personaggio e le eventuali difficoltà di pronuncia mostrate nella battuta; non attribuire automaticamente a tutti una grafia deformata o un accento. |

Queste indicazioni descrivono solo segnali attestati. In ogni revisione identifica prima chi parla e controlla la battuta inglese: una parola ricorrente da sola non basta ad attribuire il registro a un'intera specie. Quando l'originale non marca una pronuncia, conserva il tono e il lessico culturale senza aggiungere tic vocali.

---

## 2. Risoluzione della Concordanza di Genere

La lingua italiana richiede l'accordo di genere (*maschile/femminile*) per participi e aggettivi.

### A. Personaggio Giocante (Guerriero/a della Luce)
1. Quando il referente è il personaggio giocante, accorda al suo genere appellativi, titoli, articoli, pronomi, aggettivi e participi. Aggiungi una condizione SeString anche se assente nell'inglese, quando serve a conservare significato e registro. Non usare maschile fisso, barre («avventuriero/a»), parentesi o asterischi nel testo visualizzato.
2. Nel JSON usa un payload compilabile `<hex:...>`. La condizione attestata nel corpus è **If(gnum4)**: macro `02 08`, condizione `E9 05` (parametro globale 4), ramo **femminile prima**, ramo **maschile dopo**. Esempio: `<hex:02081FE905FF0D617676656E74757269657261FF0D617676656E7475726965726F03>` mostra «avventuriera» o «avventuriero». La pseudomacro `<If(PlayerParameter(Gender)...)>` non è compilata dall'encoder e non va inserita nei JSON.
3. Preferisci rami con l'intero segmento variabile («un'avventuriera esperta» / «un avventuriero esperto»), includendo articolo e accordi. I suffissi isolati sono ammessi soltanto se entrambe le frasi sono corrette. Ricodifica tutte le lunghezze in **byte UTF-8**, incluse quelle dei contenitori annidati. Per payload esistenti riusa `tools/translate_hex_literals.py` entro i suoi limiti e verifica tutte le lunghezze.
4. Conserva i referenti e le differenze di significato delle condizioni esistenti. Una resa epicena può assorbire una distinzione puramente grammaticale (per esempio «her/his» → «suo»), ma non eliminare informazioni o variabili. Distinzione tecnica: **IfPcGender** (`0B`) usa l'ObjectId e ordina i rami **maschile, femminile**; non sostituirlo con `If(gnum4)`, che riguarda il giocatore locale. Non applicare il genere locale a PNG, altri giocatori, gruppi, nomi di oggetti o titoli musicali. I possessivi italiani si accordano con ciò che è posseduto («i suoi occhi»), non con il possessore.
5. Le formulazioni epicene naturali restano valide se conservano tutto il senso e il registro («Hai dimostrato grande coraggio», «Ti do il benvenuto»). Se la neutralizzazione elimina un appellativo, un titolo, un'immagine o un'informazione importante, ripristinali con la condizione. Non riscrivere una frase valida soltanto per aggiungere un tag.
6. Revisiona separatamente la frase femminile e quella maschile complete, anche nei payload annidati, poi esegui il validatore. L'approvazione editoriale e il collaudo nel client restano distinti dalla validazione tecnica.

Riferimenti: [espressioni Lumina](https://github.com/NotAdam/Lumina/blob/master/src/Lumina/Text/Expressions/ExpressionType.cs), [macro Lumina](https://github.com/NotAdam/Lumina/blob/master/src/Lumina/Text/Payloads/MacroCode.cs); condizione del giocatore attestata in `dialogue/contenttalk.json#112:translation`.

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
