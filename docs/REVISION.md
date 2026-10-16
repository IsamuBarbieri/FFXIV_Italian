# Revisione

Questa guida copre traduzioni esistenti, feedback e riconciliazioni. Le convenzioni linguistiche, i registri e SeString sono definiti una sola volta in [Traduzione](TRANSLATION.md).

## Revisione voce per voce

Controlla `git status` e `git diff`, definisci il perimetro e preserva le modifiche presenti. Identifica funzione del foglio, campi collegati, parlante, destinatario e referente. Confronta ogni campo tradotto con la propria sorgente e leggi la sequenza narrativa: naturalezza, significato, registro e coerenza non sono verificabili con il solo elenco dei termini.

Cerca precedenti mirati nel corpus approvato. Per discrepanze reali applica [la riconciliazione](TRANSLATION.md#2-corpus-decisioni-e-discrepanze): presenta i casi all'utente e, dopo la scelta, correggi tutte le occorrenze pertinenti anche nei file approvati. Non effettuare sostituzioni cieche. Una frase sorgente identica può essere riutilizzata soltanto dopo aver verificato che contesto e funzione coincidano.

Controlla il testo interno di ogni SeString, anche nei payload rimasti identici alla sorgente, seguendo [JSON e SeString](TRANSLATION.md#4-json-e-sestring). Controlla entrambi i risultati completi delle condizioni di genere. Verifica comandi letterali, campi nome/descrizione, varianti ammesse e le eccezioni editoriali prima di correggere un presunto residuo inglese.

## Controlli

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

Confronta con `HEAD` per ID e campo, contando i campi modificati e non le righe del diff. Separa terminologia, grammatica/sintassi, maiuscole, punteggiatura e SeString; verifica anche eventuali cambiamenti a sorgenti, metadati e struttura.

Una sostituzione frequente è un indizio, non una regola: apri le frasi complete, distingui la formula riutilizzabile dalla correzione contestuale e conserva fonti e limiti delle decisioni. Non aggiornare più copie delle istruzioni: modifica soltanto la guida responsabile.

## Esito e approvazione

Riporta perimetro verificato, campi corretti, esito tecnico e stato editoriale separatamente. Spiega solo interventi realmente effettuati: uniformazioni, accordi, tipografia, coerenza tra campi, glossario o disambiguazioni multilingue. Segnala limiti e casi ancora da decidere.

Non spostare file fra gli stati e non dichiararli approvati senza conferma dell'utente. Per sole traduzioni usa i controlli pertinenti; per modifiche al codice esegui anche i test interessati. La revisione umana resta necessaria per significato, registro e resa in gioco, anche quando tutti i controlli automatici passano.
