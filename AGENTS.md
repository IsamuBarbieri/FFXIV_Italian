# Istruzioni per gli agenti AI

## Prima di lavorare

- Prima di modificare, controlla `git status` e `git diff`: conserva le modifiche già presenti e non sovrascrivere il lavoro dell'utente.
- Per traduzioni, leggi [il workflow](docs/TRANSLATION_WORKFLOW.md), [la guida di stile](docs/STYLE_GUIDE.md), il [prompt tecnico](docs/TRANSLATION_PROMPT.md) e il glossario canonico in data/glossary/Glossary.md.
- Limita il lavoro ai file indicati dall'utente. Modifica file di allineamento solo quando sono indicati come riferimenti o destinazioni.
- Non riformattare interi file JSON per cambiare poche traduzioni.
- Per un censimento delle correzioni, confronta le voci con `HEAD`, separa modifiche terminologiche, grammaticali, di maiuscole e SeString; conta i campi, non le righe del diff. Segui [il censimento editoriale](docs/TRANSLATION_CENSUS.md) per distinguere rese riutilizzabili e correzioni contestuali.

## Traduzioni

- Traduci e controlla le voci una per una, tenendo conto del contesto, del registro del personaggio e delle decisioni del glossario.
- Prima di intervenire, identifica la funzione del foglio nel gioco (dialogo, HUD, tooltip, registro, oggetto o menu) e i rapporti tra campi e righe. Verifica le dipendenze pertinenti, per esempio nome e descrizione di un oggetto o un campo che riprende la riga precedente.
- Distingui etichette e prosa: applica le note d'uso del glossario, adattando maiuscole, articoli, preposizioni e accordi alla frase. Uniforma i verbi ricorrenti solo quando indicano la stessa azione e lo stesso registro.
- Usa `data/translations/` come memoria ufficiale della localizzazione del progetto: prima di introdurre una resa, cerca precedenti pertinenti per nomi, termini, formule ricorrenti e scelte di adattamento. Prevalgono le istruzioni esplicite dell'utente e il glossario; il corpus canonico guida le scelte non già definite. I file in `data/da_revisionare/` restano precedenti contestuali, non fonti approvate.
- Cerca in modo mirato per testo sorgente, nome o termine, iniziando dai fogli pertinenti ed estendendo la ricerca solo se serve. Leggi originale, traduzione, campo e contesto dei risultati; riusa la scelta quando coincidono referente, significato e funzione, adattando sintassi e registro. Non rileggere l'intero corpus a ogni voce; riutilizza i riferimenti già verificati durante il lavoro.
- Se trovi rese incompatibili dello stesso elemento nello stesso significato e funzione, e nessuna istruzione esplicita o nota del glossario risolve il conflitto, presenta all'utente le varianti con riferimenti `percorso#ID:campo` e contesto, e chiedi come riconciliarle prima di scegliere o uniformare. Prosegui le voci indipendenti; non estendere le modifiche ai file di riferimento senza autorizzazione. Differenze motivate da significato, referente, registro, sintassi o funzione del campo non sono automaticamente incoerenze.
- Per riferimenti narrativi o nomi propri (personaggi, luoghi, popoli, fazioni, creature, quest e oggetti legati alla lore), verifica prima il corpus canonico. Se manca un precedente pertinente, il contesto non è attestato o resta un dubbio, consulta la pagina pertinente della wiki di FFXIV. Non tradurre in modo isolato o per semplice somiglianza lessicale; segnala le ambiguità irrisolte senza inventare dettagli. La wiki chiarisce la lore, ma non sostituisce una scelta italiana già stabilita nel progetto.
- Quando l'inglese è ambiguo per significato, gioco di parole o meccanica, confronta se disponibili le versioni ufficiali francese, tedesca o giapponese. Usa il confronto come indizio e registra la scelta nei casi decisivi; non è un passaggio obbligatorio per ogni voce.
- Per capire una scelta editoriale, cerca prima correzioni recenti dell'utente nei file canonici e revisionati. Prevalgono le istruzioni esplicite dell'utente e il glossario; una correzione isolata non va generalizzata fuori dal suo contesto.
- Conserva ID, struttura JSON, campi sorgente e metadati. Modifica solo i campi di traduzione richiesti.
- Preserva tag e codici SeString. Traduci il testo leggibile dentro i payload esadecimali solo seguendo le regole e gli strumenti esistenti.
- Per accordi riferiti al giocatore, verifica entrambi i risultati completi di If(gnum4), ramo femminile e poi maschile; non cambiare il referente delle condizioni native. Le sole eccezioni alla conservazione dei trattini sono quelle documentate nel glossario.
- Prima di scegliere un lemma identifica il campo e il referente: Arms araldico non è un'arma, range di livelli non è una distanza, weaponskill non è un'azione istintiva. Conserva comandi e parametri letterali, traducendo la spiegazione circostante.
- Dopo la traduzione esegui il validatore appropriato sul perimetro modificato. Il controllo automatico non sostituisce la revisione linguistica né l'approvazione dell'utente.
- Nel resoconto indica il perimetro verificato e le motivazioni delle correzioni pertinenti: glossario e note d'uso, uniformazioni terminologiche o sintattiche, tipografia e coerenza tra campi, eventuale disambiguazione multilingue. Non inventare categorie prive di interventi.
- Applica le correzioni dell'utente. Aggiungi al glossario le decisioni terminologiche confermate, con riferimento `percorso#ID:campo` oppure «Decisione dell'utente» e una nota d'uso.
- Non spostare file tra gli stati editoriali e non dichiarare un file approvato senza conferma dell'utente.
- **File esclusi o invariati per decisione editoriale**:
  - `system/creditlisttext.json`: i crediti di gioco non vanno mai tradotti; il campo `translation` deve coincidere identico con `original`.
  - `housing/orchestrion.json`: i titoli musicali (`name` / `translation_name`) conservano i titoli originali dei brani e non vanno tradotti (si traducono solo le descrizioni).
  - **Misure e pesi eorzeani**: le unità di misura eorzeane seguono l'adattamento morfologico concordato: distanze/lunghezze maschili con plurale in *-mi* (*ilm/ilmi, fulm/fulmi, yalm/yalmi, malm/malmi*) e pesi femminili della prima declinazione (*onza/onze, ponza/ponze, tonza/tonze*).


## Revisione terminologica

- Per controllare file in revisione e canonici/deployati, usa `python tools/terminology_review.py scan --include-review --include-approved --all-matches --limit 0 --out data/glossary/review_all.json`.
- Considera ogni risultato un candidato: apri la voce nel file, controlla originale, traduzione, tag e contesto prima di correggere. Non applicare o respingere in blocco una coda e non promuovere file a `data/translations/` senza conferma.
- Se la regola automatica segnala una resa contestualmente corretta, correggi la regola o documenta l'eccezione con una verifica mirata; non cambiare una traduzione valida solo per azzerare il report.
- Ripeti controllo e revisione manuale dopo ogni correzione, fino a zero problemi nel perimetro dichiarato. Una coda vuota non attesta l'approvazione linguistica; registra separatamente corpus, campi verificati ed eventuali limiti.
- Nella colonna Fonte del glossario usa una sola fonte sorgente `percorso#ID:campo` oppure esattamente «Decisione dell'utente». Metti attestazioni aggiuntive e riferimenti ai campi `translation_*` nella colonna Uso; per i file in revisione usa `@review/`.

## Codice e dati

- Mantieni i confini tra Core, Extractor, DiffTool e Patcher.
- Non includere o versionare dump e asset originali del gioco; il progetto legge i dati dall'installazione locale e genera file per Penumbra.
- Per modifiche al codice usa i test esistenti pertinenti; per sole traduzioni usa i controlli di traduzione.

Per struttura, comandi e documentazione del progetto, consulta [README.md](README.md) e [l'indice dei documenti](docs/README.md).
