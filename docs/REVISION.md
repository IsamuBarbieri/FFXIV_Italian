# Revisione

Questa guida copre traduzioni esistenti, feedback e riconciliazioni. Le convenzioni linguistiche, i registri e SeString sono definiti una sola volta in [Traduzione](TRANSLATION.md).

La revisione è un passaggio editoriale volto a ottenere un adattamento italiano professionale da videogioco. La traduzione iniziale deve già verificare significato e terminologia; chi revisiona ne controlla la correttezza, ma concentra il lavoro sulla resa italiana, sulla voce e sull'efficacia del testo in gioco. Anche una frase grammaticalmente corretta e conforme al glossario può richiedere una riscrittura se suona tradotta, rigida o poco adatta alla scena.

## Revisione voce per voce

Controlla `git status` e `git diff`, definisci il perimetro e preserva le modifiche presenti. Identifica funzione del foglio, campi collegati, parlante, destinatario e referente. Confronta ogni campo tradotto con la propria sorgente e leggi la sequenza narrativa: naturalezza, significato, registro e coerenza non sono verificabili con il solo elenco dei termini.

Procedi su ogni voce nel suo contesto:

1. Ricostruisci dalla sorgente che cosa comunica il testo e quale effetto deve produrre: intenzione, emozione, informazioni, rapporti fra interlocutori ed eventuali ironia o sottintesi. Non dedurli soltanto dalla traduzione esistente.
2. Leggi l'italiano come testo destinato al giocatore. Valuta sintassi, scorrevolezza, ritmo, ordine delle informazioni, idiomaticità e precisione. Individua calchi, collocazioni innaturali, pronomi ridondanti, ambiguità involontarie, ripetizioni accidentali e formulazioni burocratiche o macchinose. Distinguili dalle ripetizioni e dalle costruzioni volute del personaggio.
3. Adatta la resa alla funzione del campo e alla voce pertinente, secondo le convenzioni di [Traduzione](TRANSLATION.md#3-convenzioni-editoriali) e le schede dei parlanti. Verifica che le battute funzionino nello scambio, che le risposte si colleghino alle battute precedenti e che titoli, obiettivi e descrizioni siano efficaci nel proprio uso.
4. Riscrivi dove serve, anche cambiando costruzione e distribuzione della frase, per ottenere italiano naturale e lo stesso effetto comunicativo. Conserva significato, informazioni, intensità e caratterizzazione: non abbellire con dettagli inventati, non alzare automaticamente il registro e non appiattire le voci. Mantieni le rese già efficaci; non cambiare per semplice preferenza fra alternative equivalenti.
5. Ricontrolla la nuova resa contro la sorgente per escludere omissioni, aggiunte o slittamenti di senso, quindi rileggi in continuità la scena o il gruppo di testi collegati. Verifica anche terminologia e vincoli tecnici prima di considerare conclusa la voce.

Consulta il corpus anche come riferimento stilistico: cerca passaggi pertinenti dello stesso personaggio, della stessa fase narrativa o della stessa funzione testuale e confronta il modo di formulare le frasi, oltre ai singoli termini. I precedenti orientano la continuità editoriale, ma non rendono automaticamente buona una costruzione poco naturale e non impongono formule identiche in contesti diversi. Applica i limiti di ricerca e le regole sulle discrepanze della guida di traduzione; non estendere la revisione a una riscrittura generale dei file consultati.

Cerca precedenti mirati nel corpus approvato. Per discrepanze reali applica [la riconciliazione](TRANSLATION.md#2-corpus-decisioni-e-discrepanze): presenta i casi all'utente e, dopo la scelta, correggi tutte le occorrenze pertinenti anche nei file approvati. Non effettuare sostituzioni cieche. Una frase sorgente identica può essere riutilizzata soltanto dopo aver verificato che contesto e funzione coincidano.

Controlla il testo interno di ogni SeString, anche nei payload rimasti identici alla sorgente, seguendo [JSON e SeString](TRANSLATION.md#4-json-e-sestring). Controlla entrambi i risultati completi delle condizioni di genere. Verifica comandi letterali, campi nome/descrizione, varianti ammesse e le eccezioni editoriali prima di correggere un presunto residuo inglese. Elimina spazi finali superflui ereditati dalla sorgente e accertati che i composti con trattino usino il trattino standard `-` anziché il tag SeString di sillabazione `<hex:021F0103>`.

## Controlli

I controlli automatici supportano la lettura editoriale e non ne determinano il perimetro: esamina tutte le voci assegnate, comprese quelle senza segnalazioni. La scansione terminologica completa serve per un audit terminologico o una riconciliazione; non è un passaggio obbligatorio di ogni revisione stilistica.

Esegui dalla radice del repository il validatore sul file o sulla cartella interessati:

```powershell
dotnet run --project src/FFXIVItalian.Extractor -- validate <percorso-file-o-cartella>
```

Senza percorso, `validate` controlla il corpus approvato. `validate --review <percorso>` è l'audit terminologico dei file completi; non sostituisce la verifica delle voci di un file parzialmente tradotto.

Per la scansione terminologica completa di approvati e file in revisione:

```powershell
python tools/terminology_review.py scan --include-review --include-approved --all-matches --limit 0 --out data/glossary/review_all.json
```

`scan` e `harvest` non sovrascrivono una coda esistente: usa un nuovo nome o `--overwrite` dopo aver conservato le decisioni. Ogni risultato è un candidato; apri sorgente, traduzione, campo, tag e contesto prima di correggere o respingere. Non trattare l'assenza di una forma letterale come errore certo: una parafrasi o un omonimo possono essere corretti. La coda vuota non dimostra qualità linguistica.

Per una modifica di canone cerca anche la vecchia forma italiana:

```powershell
python tools/terminology_review.py scan --term "TERMINE INGLESE" --old "VECCHIA FORMA" --include-review --include-approved --all-matches --limit 0 --out data/glossary/review_change.json
```

Per i payload leggibili rimasti identici e per valori corrotti:

```powershell
python scripts/scan_corrupted_values.py <percorso-file-o-cartella> --report data/glossary/review_hex.json
```

Nomi propri e identificatori interni possono restare originali. Il controllo non sostituisce la decodifica delle macro. Per gli otto fogli UI che richiedono verifiche aggiuntive su macro e licenze, usa `python tools/validate_ui_sheets.py`; l'elenco dei fogli è dichiarato nello script.

Se una regola segnala una traduzione contestualmente valida, correggi la regola o documenta l'eccezione con una verifica mirata. Non cambiare una frase valida per azzerare un rapporto. Dopo le correzioni ripeti i controlli sullo stesso perimetro fino a risolvere i problemi effettivi.

## Code di proposte

`tools/terminology_review.py` produce una coda con sorgente, traduzione attuale, forme canoniche e fonti. Le proposte restano `pending`; nessuna scansione modifica il corpus. Per applicare una coda già verificata:

```powershell
python tools/terminology_review.py apply --review <coda.json> --dry-run
python tools/terminology_review.py apply --review <coda.json>
```

Solo le voci con `status: "approved"` vengono applicate. Il tool verifica che sorgente e traduzione non siano cambiate e conserva i tag nell'ordine. Le modifiche intenzionali al bytecode richiedono intervento diretto e verifica specifica. `rejected` e una nota documentano un falso positivo. Non approvare o respingere in blocco la coda.

## Feedback e decisioni

Applica le correzioni dell'utente e verifica nuovamente le voci. Registra nel glossario solo decisioni riutilizzabili, varianti, limiti d'uso o eccezioni che il corpus non spiega. La colonna Fonte contiene una sola fonte sorgente `percorso#ID:campo` oppure esattamente «Decisione dell'utente»; ulteriori attestazioni e riferimenti a `translation_*` vanno nella colonna Uso. `@review/` indica una fonte contestuale senza approvare il file.

Non copiare sistematicamente nomi ed etichette dal corpus nel glossario. `harvest` è disponibile per raccogliere candidati mirati, non come passaggio obbligatorio dopo ogni file.

## Censimento del diff

Confronta con `HEAD` per ID e campo, contando i campi modificati e non le righe del diff. Separa adattamento/stile, terminologia, grammatica/sintassi, maiuscole, punteggiatura e SeString; verifica anche eventuali cambiamenti a sorgenti, metadati e struttura. Distingui le riscritture per naturalezza, ritmo o voce dalle sole correzioni grammaticali; se un campo comprende più tipi di intervento, segnalali senza duplicarlo nel totale.

Una sostituzione frequente è un indizio, non una regola: apri le frasi complete, distingui la formula riutilizzabile dalla correzione contestuale e conserva fonti e limiti delle decisioni. Non aggiornare più copie delle istruzioni: modifica soltanto la guida responsabile.

## Esito e approvazione

Riporta perimetro verificato, campi corretti, esito tecnico e stato editoriale separatamente. Descrivi anzitutto il lavoro sull'adattamento e mostra pochi esempi significativi prima/dopo, con riferimenti `percorso#ID:campo` e una breve motivazione. Spiega solo interventi realmente effettuati, distinguendo riscritture stilistiche, correzioni di senso, uniformazioni e verifiche tecniche. Se la resa era già efficace, dichiaralo senza forzare modifiche. Segnala limiti e casi ancora da decidere; se hai svolto soltanto un controllo terminologico, non presentarlo come revisione editoriale completa.

Non spostare file fra gli stati e non dichiararli approvati senza conferma dell'utente. Per sole traduzioni usa i controlli pertinenti; per modifiche al codice esegui anche i test interessati. La revisione umana resta necessaria per significato, registro e resa in gioco, anche quando tutti i controlli automatici passano.
