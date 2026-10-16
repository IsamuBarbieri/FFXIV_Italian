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
3. Prima di scrivere, censisci per `percorso#ID:campo` le occorrenze della sorgente e delle vecchie forme italiane nei corpus approvato e in revisione, inclusi singolari, plurali, composti e testo nei payload. Distingui i referenti pertinenti dagli omonimi e dalle eccezioni. Conserva il valore iniziale dei campi interessati: se cambia durante il lavoro, rileggilo e integra le modifiche senza sovrascriverle.
4. Uniforma le occorrenze pertinenti e la nuova traduzione. Questa riconciliazione comprende i file discrepanti anche fuori dal file inizialmente assegnato, salvo esclusioni esplicite dell'utente. Non usare sostituzioni per sottostringa sull'intero JSON: modifica soltanto i campi tradotti censiti e i segmenti testuali verificati, ricodificando i payload con l'helper. Per esempio, la decisione `primal` → «primale» non riguarda automaticamente `primordial`, `primeval`, l'aggettivo generico `primal` o il gruppo musicale «The Primals».
5. Rileggi ogni frase, compresi tutti i rami delle condizioni, e adatta accordi e preposizioni. Confronta prima/dopo per ID e campo: sorgenti, metadati, ID e campi estranei devono restare identici. Valida tutti i file toccati e ripeti la ricerca delle vecchie forme; per ogni residuo pertinente correggi l'omissione o documenta l'eccezione. Se emerge un danno, riparti dai valori iniziali dei soli campi interessati, non da una sostituzione inversa globale.
6. Registra la decisione riutilizzabile, distinguendo la scelta confermata dal suo stato di applicazione. Una riga nel glossario non dimostra che la riconciliazione sia completa: nella consegna riporta campi corretti, residui motivati ed eventuali casi sospesi.

Non uniformare differenze motivate da referente, significato, registro, sintassi o funzione. La conferma della forma autorizza la riconciliazione pertinente, non sostituzioni indiscriminate né cambi di stato editoriale.

Controlla anche le decisioni già presenti per singolari, ruoli e nomi composti del termine. Se una nuova forma generale contrasta con una decisione più specifica, presenta il conflitto all'utente: non presumere che la nuova scelta abroghi l'eccezione o che l'eccezione autorizzi tutte le vecchie varianti.

Nel glossario registra una sola voce per decisione e conserva soprattutto ciò che il corpus non spiega: forma sostituita in una riconciliazione, limite semantico, variante ammessa o eccezione tecnica. Non aggiungere una riga per una normale flessione, preposizione o etichetta già univoca nel corpus. «Decisione dell'utente» richiede una conferma effettiva; un precedente trovato dall'agente va citato come fonte sorgente e non risolve da solo una nuova discrepanza.

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

**Esamina sempre tutti i SeString del campo assegnato, anche se il testo esterno è già tradotto.** La presenza di un tag o di un payload `<hex:...>` non esclude il suo contenuto dalla traduzione. Decodifica e controlla anche le espressioni stringa annidate e tutti i rami delle condizioni: parole, frasi, appellativi e alternative visibili al giocatore vanno tradotti secondo le stesse regole del testo esterno. Non considerare completa una voce finché il suo testo interno non è stato controllato.

Preserva la funzione dei controlli, variabili, icone, colori, interruzioni di riga e referenti dinamici come `<FullName>`, `<Forename>` e `<Surname>`. Per `<string(...)>` ed espressioni analoghe distingui il riferimento dinamico dagli eventuali argomenti testuali: un riferimento, indice o identificatore tecnico resta invariato; una stringa letterale destinata alla visualizzazione va esaminata e tradotta. Non sostituire i riferimenti dinamici con valori inventati. Nomi propri ed eccezioni editoriali seguono corpus e glossario anche all'interno dei payload.

Quando un payload contiene testo visibile traducibile, traduci solo i segmenti testuali pertinenti e ricodifica tutte le lunghezze in byte UTF-8, inclusi i contenitori annidati. I byte del payload e le lunghezze possono quindi cambiare: conservarli identici non è un requisito quando il testo interno viene tradotto. Usa l'helper testato `tools/translate_hex_literals.py` per sostituzioni di segmenti completi; non manipolare lunghezze a occhio. Se il payload contiene soltanto controlli o riferimenti tecnici, conservalo dopo averlo verificato. Se non riesci a decodificarlo o a determinarne la funzione, segnala il caso con `percorso#ID:campo` e continua le voci indipendenti; non saltarlo dichiarando il campo completo. Le eccezioni ai trattini sono limitate ai casi documentati nel glossario.

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

Le schede individuali, il criterio di selezione e il modello per i nuovi profili sono in [Voci dei personaggi](CHARACTER_VOICES.md). Consulta soltanto le schede dei parlanti della scena e la fase narrativa pertinente; il profilo orienta la resa, ma non aggiunge emozioni o informazioni alla sorgente.

Per i PNG generici parti dalla battuta, dal ruolo e dalla situazione. Razza e clan non impongono un registro comune: applica i tratti di un popolo soltanto quando attestati, secondo la sezione precedente. Le schede riguardano il parlato e gli eventuali testi attribuiti al personaggio, non automaticamente titoli, obiettivi o descrizioni del diario.

---
