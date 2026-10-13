# Workflow di traduzione assistita

Questo è il flusso principale quando il responsabile fornisce uno o più file da tradurre. I comandi batch rimangono disponibili come alternativa per lavorare su lotti esportati.

## 1. Definire il perimetro

- L'utente indica i file da tradurre e, se necessario, i file di riferimento o di allineamento.
- I file di riferimento forniscono contesto; si modificano solo se l'utente li indica anche come destinazioni.
- Si leggono glossario, guida di stile, prompt tecnico ed esempi vicini nel corpus.
- Si identifica dove appare il foglio nel gioco e si individuano i campi e le righe collegati (per esempio nome/descrizione o uno stato che riprende la riga precedente). Il campo sorgente corrispondente e la catena logica guidano la revisione.
- Si tengono conto delle eccezioni editoriali stabilite: `system/creditlisttext.json` (crediti di gioco non tradotti, `translation` coincide con `original`) e `housing/orchestrion.json` (titoli dei brani musicali in `name`/`translation_name` invariati).


## 2. Tradurre

- Si lavora caso per caso, preservando il senso, il contesto, il registro del personaggio e le scelte terminologiche approvate.
- Usa `data/translations/` come memoria ufficiale della localizzazione, dopo le istruzioni esplicite dell'utente e il glossario. Prima di introdurre nomi, termini, formule ricorrenti o adattamenti, cerca precedenti per testo sorgente o termine nei fogli pertinenti; estendi la ricerca solo se serve. Verifica originale, traduzione, campo e contesto, e riusa la scelta quando referente, significato e funzione coincidono, adattando sintassi e registro. Conserva i riferimenti già verificati durante il lavoro: non serve rileggere tutto il corpus per ogni voce.
- Le correzioni recenti dell'utente aiutano a interpretare le scelte editoriali; `data/da_revisionare/` fornisce solo precedenti contestuali, senza approvarli né promuoverli automaticamente al glossario.
- Se il corpus presenta rese incompatibili per lo stesso elemento, significato e funzione, verifica prima se istruzioni esplicite o note del glossario risolvono il conflitto. Altrimenti presenta le varianti con riferimenti `percorso#ID:campo` e contesto e chiedi all'utente come riconciliarle prima di scegliere o uniformare; continua sulle voci indipendenti. Differenze giustificate da contesto, registro o sintassi restano valide. I file di riferimento si modificano solo se autorizzati come destinazioni.
- Per lore e nomi propri, consulta la wiki quando manca un precedente canonico pertinente, il contesto non è attestato o resta un dubbio. Usa le fonti esterne per chiarire il significato, rispettando le scelte italiane già stabilite nel progetto.
- Si conservano ID, campi originali, metadati e schema JSON. Si modificano solo i campi di traduzione.
- Conserva controlli, variabili e referenti SeString; traduci e ricodifica il testo visibile nei payload. Per il genere del giocatore applica la sezione 2 della guida di stile: puoi aggiungere If(gnum4) quando l'accordo italiano lo richiede, anche se assente nell'originale. Verifica integralmente le frasi femminile e maschile senza modificare il campo sorgente.
- Le ambiguità che dipendono dal contesto si segnalano invece di risolverle inventando informazioni.
- Se l'inglese lascia incerti senso, gioco di parole o meccanica, si confrontano quando disponibili le versioni ufficiali francese, tedesca o giapponese. Si annota la fonte e la decisione solo per i passaggi in cui il confronto è stato utile.

## 3. Validare e revisionare

Esegui il controllo automatico appropriato per i file toccati. Per le traduzioni in revisione, il comando generale è:

```powershell
dotnet run --project src/FFXIVItalian.Extractor -- validate --review data/da_revisionare/<categoria>/<file>.json
```

Per i file approvati, `validate` controlla il corpus canonico. Per script o fogli speciali usa soltanto il validatore specifico che li copre; per esempio `tools/validate_new_sheets.py` controlla il gruppo di fogli dichiarato nel file.

Dopo il controllo automatico, rileggi ogni voce nel contesto per verificare significato, naturalezza, registro, genere, tag e coerenza terminologica. Il validatore non sostituisce questa revisione.

Nel resoconto finale riporta il perimetro e l'esito della verifica; raggruppa solo le correzioni effettuate per allineamento al glossario (con nota d'uso), uniformazione terminologica o sintattica, tipografia o coerenza tra campi. Segnala a parte le disambiguazioni multilingue effettivamente svolte.

Il validatore è di sola verifica e non modifica le traduzioni. Un'applicazione automatica di una correzione è ammessa solo quando l'intera frase sorgente coincide con una frase approvata; se cambia anche solo il contesto circostante, l'agente adatta la correzione leggendo originale e traduzione completi.

Per una scansione terminologica completa dei file in revisione e dei file canonici/deployati, usa:

```powershell
python tools/terminology_review.py scan --include-review --include-approved --all-matches --limit 0 --out data/glossary/review_all.json
```

La coda segnala possibili incoerenze, non errori certi. Controlla ogni candidato nell'intera voce e correggi anche la regola del revisore se produce falsi positivi. Il report non approva né sposta file.

## 4. Revisione dell'utente e correzioni

- L'utente fa un controllo visivo complessivo e indica i cambiamenti desiderati.
- Si applicano le correzioni ai file interessati e si ripete la validazione necessaria.
- Il file resta in `data/da_revisionare/` finché l'utente non lo approva; l'approvazione consiste nello spostarlo in `data/translations/`.

## 5. Aggiornare il glossario

Aggiungi una voce quando l'utente conferma una scelta riutilizzabile. Indica una fonte `percorso#ID:campo` o, se è una scelta esplicita senza attestazione nel corpus, «Decisione dell'utente»; annota varianti e limiti di contesto. Non promuovere automaticamente una proposta del revisore o una soluzione valida per una sola frase.

Per censire le correzioni non committate, segui [il censimento editoriale](TRANSLATION_CENSUS.md). Confronta i campi con `HEAD`, leggi la coppia sorgente/traduzione e registra le distinzioni riutilizzabili nel glossario. Nella colonna Fonte inserisci un solo riferimento al campo sorgente; conserva ulteriori attestazioni ai campi tradotti nella colonna Uso. `@review/` indica un precedente contestuale, senza approvare il file.

Nel ciclo di pulizia alterna validatore, esame manuale dei risultati e correzioni mirate. Riesegui il controllo sullo stesso perimetro dopo ogni intervento, senza disattivare regole per nascondere problemi. Se il lavoro include codice, esegui anche i test pertinenti. Riporta separatamente esito tecnico e stato editoriale; conserva in un commit successivo le correzioni emerse dopo il commit iniziale.

## Workflow batch alternativo

Per traduzioni gestite tramite file batch:

```powershell
dotnet run --project src/FFXIVItalian.Extractor -- export-batch <foglio> --size 50
dotnet run --project src/FFXIVItalian.Extractor -- import-batch <foglio> data/batches/<batch>.json
dotnet run --project src/FFXIVItalian.Extractor -- autofill <foglio|all>
```

Per batch JSON esterni sono disponibili anche `scripts/split_untranslated.py` e `scripts/apply_all_translations.py`; quest'ultimo supporta `--validate-only` prima di modificare il master.
