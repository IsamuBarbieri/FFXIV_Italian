# Traduzione da zero

Guida operativa per nuove traduzioni, soprattutto missioni e cutscene. Per controllare una traduzione esistente usa [Revisione](REVISION.md); dopo una patch usa [Aggiornamento del client](CLIENT_UPDATE.md).

## 1. Perimetro e contesto

Controlla `git status` e `git diff` prima di scrivere. Conserva il lavoro preesistente e modifica solo file e campi assegnati, senza riformattare interi JSON. Non spostare file fra gli stati editoriali senza richiesta dell'utente.

Identifica la funzione di ogni campo: battuta, titolo, descrizione del diario, obiettivo, scelta del giocatore o identificatore tecnico. I fogli narrativi non contengono necessariamente soltanto dialoghi. Leggi la sequenza della scena e i campi collegati, identifica parlante, destinatario e referente dei pronomi. Il campo `tag` aiuta a orientarsi, ma non basta sempre a identificare il personaggio: segnala il contesto mancante senza inventarlo.

Gli stati editoriali dipendono dalla cartella:

| Cartella | Significato |
| --- | --- |
| `data/da_tradurre/` | Lavoro ancora da tradurre. |
| `data/da_revisionare/` | Traduzioni in attesa di revisione e approvazione. |
| `data/translations/` | Corpus approvato e compilato nella mod. |

## 2. Corpus, decisioni e discrepanze

`data/translations/` è la memoria ufficiale del progetto: cerca precedenti mirati per sorgente inglese, nome, termine o formula, iniziando dai fogli pertinenti. Leggi originale, traduzione, campo e contesto; riusa la resa solo quando coincidono referente, significato e funzione, adattando grammatica e registro. Riutilizza i riferimenti già verificati durante la sessione. `data/da_revisionare/` fornisce precedenti contestuali, non scelte approvate.

Le istruzioni esplicite dell'utente prevalgono. [Glossary.md](../data/glossary/Glossary.md) conserva decisioni, varianti ed eccezioni non ricavabili dalla sola coppia sorgente/traduzione; non occorre duplicarvi ogni nome del corpus. Le correzioni recenti dell'utente aiutano a capire le scelte editoriali, senza rendere generale una correzione valida per una sola frase.

Se due rese indicano davvero lo stesso elemento nello stesso significato e funzione, e nessuna decisione già confermata risolve il conflitto:

1. Mostra varianti, contesto e riferimenti `percorso#ID:campo` e chiedi all'utente quale mantenere.
2. Continua le voci indipendenti mentre aspetti la scelta.
3. Dopo la scelta, cerca tutte le occorrenze pertinenti nel corpus approvato e in revisione e uniformale, oltre alla nuova traduzione. Questa riconciliazione comprende i file discrepanti anche fuori dal file inizialmente assegnato, salvo esclusioni esplicite dell'utente.
4. Rileggi ogni frase, adatta accordi e preposizioni, valida tutti i file toccati e registra la decisione riutilizzabile.

Non uniformare differenze motivate da referente, significato, registro, sintassi o funzione. La conferma della forma autorizza la riconciliazione pertinente, non sostituzioni indiscriminate né cambi di stato editoriale.

Per lore e nomi propri consulta prima il corpus. Se manca un precedente pertinente o resta un dubbio, verifica la pagina della missione, del personaggio o dell'elemento sulla wiki di FFXIV. La wiki chiarisce il significato; non sostituisce una resa italiana stabilita. Per inglese ambiguo, giochi di parole o meccaniche confronta, quando disponibili, le versioni ufficiali FR/DE/JA e annota soltanto i confronti decisivi. Segnala le ambiguità irrisolte.

## 3. Convenzioni editoriali

Traduci voce per voce, preservando senso, destinatario e registro. Usa italiano naturale, senza inserire la sorgente inglese fra parentesi nel testo di gioco. Conserva concisione nelle scelte e nei campi con poco spazio; non comprimere il dialogo a scapito della voce del personaggio. Non aggiungere tic vocali assenti nella sorgente.

- Distingui etichette e prosa. Rispetta le maiuscole della sorgente e le note d'uso; articoli e preposizioni italiane interni restano minuscoli. Un'etichetta maiuscola non impone il maiuscolo nella prosa.
- Usa apostrofo ASCII (`'`) e caporali (« »), salvo vincoli tecnici del campo.
- Le istruzioni rivolte al singolo giocatore usano il tu. Dialoghi e annunci conservano il destinatario attestato.
- Uniforma formule e verbi solo quando azione e registro coincidono: `Report to` può essere «Torna da» o «Fai rapporto a».
- Identifica il referente prima del lemma: `Arms` araldico è uno stemma; il `range` di livelli è una soglia, quello spaziale una portata o un raggio. Distingui tecniche d'arma, abilità, incantesimi e azioni istintive. Le sigle ATT/DIF valgono nei campi tecnici, non per ogni uso narrativo.
- `Dungeon` come attività istanziata è «Spedizione»; come ambiente fisico è «sotterraneo». Missioni, Incarichi, Mandati, Prove, Incursioni, Cripte Profonde e Operazioni di Gilda seguono le decisioni contestuali del glossario.
- Conserva comandi e parametri eseguibili (`/party`, `/waymark`, `/cwlinkshell1`); traduci la spiegazione circostante.
- Per nomi composti descrittivi verifica lore e precedenti prima di adattare cognomi, soprannomi e appellativi. I nomi propri dei Loporrit in `-way` restano invariati.
- Le unità letterali di distanza sono maschili: ilm/ilmi, fulm/fulmi, yalm/yalmi, malm/malmi. I pesi sono femminili: onza/onze, ponza/ponze, tonza/tonze. «Qualche» regge il singolare. Rendi naturalmente gli idiomi e documenta le eccezioni mirate del controllo, senza convertirli meccanicamente in misure.
- Mantieni identici i crediti in `system/creditlisttext.json` e i titoli musicali `name`/`translation_name` in `housing/orchestrion.json`; di quest'ultimo traduci soltanto le descrizioni. Le licenze generali e gli avvisi dei software di terze parti consultabili dalla lobby restano originali, compreso `system/addontransient.json#173:original`. Gli avvisi di licenza mostrati nelle finestre di gioco si traducono come il resto della UI: `system/addontransient.json#247:original` e `#1016:original` sono le informazioni sulle esibizioni e non sono esclusi dalla traduzione.

## 4. JSON e SeString

Conserva ID, struttura, campi sorgente e metadati. Modifica soltanto i campi `translation` e `translation_*` richiesti. Salva progressivamente blocchi coerenti della scena, fino a completare il perimetro assegnato.

Conserva controlli, variabili, icone e referenti dei tag: `<FullName>`, `<Forename>`, `<Surname>`, `<string(...)>`, colori e interruzioni di riga non sono testo da tradurre. Decodifica i payload `<hex:...>` prima di intervenirvi. Se contengono testo visibile, traduci solo quel testo e ricodifica tutte le lunghezze in byte UTF-8, inclusi i contenitori annidati. Usa l'helper testato `tools/translate_hex_literals.py` per sostituzioni di segmenti completi; non manipolare lunghezze a occhio. Le eccezioni ai trattini sono limitate ai casi documentati nel glossario.

Per accordi riferiti al giocatore puoi aggiungere **If(gnum4)** quando necessario: macro `02 08`, condizione `E9 05`, ramo **femminile**, poi **maschile**. Esempio avventuriera/avventuriero:

```text
<hex:02081FE905FF0D617676656E74757269657261FF0D617676656E7475726965726F03>
```

Verifica separatamente entrambe le frasi complete, accordando articoli, preposizioni, pronomi, aggettivi e participi. Preferisci segmenti completi; non mostrare barre come «avventuriero/a». Le rese neutre naturali sono ammesse se non perdono senso o registro. I possessivi concordano con la cosa posseduta.

Non applicare il genere locale a PNG, terzi, collettivi o etichette generiche. Conserva le condizioni native: **IfPcGender** (`0B`) riguarda l'ObjectId indicato e ordina i rami **maschile, femminile**. Non è intercambiabile con If(gnum4). La pseudomacro `<If(PlayerParameter(Gender)...)>` non è compilata dall'encoder.

## 5. Verifica e consegna

Segui i [controlli di revisione](REVISION.md#controlli). Rileggi tutte le voci nel contesto dopo la verifica tecnica. Riporta file e campi verificati, esito, ambiguità e motivazioni delle correzioni effettive. Un validatore superato non approva il file: l'approvazione e lo spostamento in `data/translations/` spettano all'utente.

Il lavoro diretto sul JSON è il flusso principale. I batch opzionali e i loro limiti sono documentati in [Architettura e manutenzione](ARCHITECTURE.md#batch-opzionali).

## Registri dei personaggi e dei popoli

### Parlato delle Società Alleate e dei popoli affini

La denominazione di progetto è **Società Alleate** (*Allied Societies*); i riferimenti storici alle tribù vanno interpretati nel loro contesto. I popoli seguenti orientano la ricerca dei precedenti:

| Espansione | Società Alleate |
| :--- | :--- |
| A Realm Reborn | Amalj'aa, Sylph, Kobold, Sahagin, Ixal |
| Heavensward | Vanu Vanu, Vath, Moguri |
| Stormblood | Ananta, Kojin, Namazu |
| Shadowbringers | Pixie, Qitari, Nani |
| Endwalker | Arkasodara, Omicron, Loporrit |
| Dawntrail | Pelupelu, Mamool Ja, Yok Huy |

Per quest e popoli non attestati nel corpus verifica il contesto sulla pagina pertinente della wiki. La tabella non sostituisce il registro effettivo del parlante.

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

### Personaggi principali

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
