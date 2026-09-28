# Revisione terminologica assistita

Il glossario approvato in `data/glossary/Glossary.md` resta la fonte editoriale. Un file completo non diventa automaticamente una fonte approvata: va revisionato e aggiunto all'elenco del glossario. Ogni voce nuova deve indicare `percorso#ID:campo` e una nota d'uso se il termine è ambiguo.

Lo strumento `tools/terminology_review.py` usa solo la libreria standard di Python. Non modifica le traduzioni durante la raccolta o la scansione. La coda JSON contiene la frase originale, la traduzione attuale, le varianti canoniche e le fonti. `status` parte sempre da `pending`.

## 1. Ampliare il glossario

```powershell
python tools/terminology_review.py harvest --file system/maincommand.json --out data/glossary/candidates_maincommand.json
python tools/terminology_review.py harvest --file system/addon.json --include-unchanged --limit 200 --out data/glossary/candidates_addon.json
```

I candidati finiscono nel file passato a `--out` (`data/glossary/candidates.json` per impostazione predefinita), ordinati per numero di attestazioni nel file approvato. Esaminare significato e ricorrenza; inserire nel glossario solo le voci riutilizzabili, con la fonte e le eventuali eccezioni. `harvest` non promuove automaticamente nessuna voce. `--include-unchanged` comprende anche i nomi mantenuti in inglese. Tutti i nomi di `world/placename.json` sono già parte del glossario completo dei luoghi; `system/maincommand.json` resta ricercabile con `scan --term` senza copiare tutte le etichette nella tabella.

## 2. Creare una coda di revisione

```powershell
python tools/terminology_review.py scan --file misc/addontransient.json --limit 100
python tools/terminology_review.py scan --term "Duty Finder" --limit 100 --out data/glossary/review_duty_finder.json
python tools/terminology_review.py scan --term "Glamours" --include-approved --limit 100 --out data/glossary/review_glamours.json
python tools/terminology_review.py scan --term "The Waking Sands" --old "Sabbie del Risveglio" --out data/glossary/review_waking_sands.json
python tools/terminology_review.py scan --term "Dzemael Darkhold" --out data/glossary/review_dzemael.json
```

La coda predefinita è `data/glossary/review.json`. `--file` restringe la scansione; senza `--file` vengono letti tutti i JSON di traduzione. I file approvati sono esclusi dalla scansione ordinaria; `--include-approved` li include per analizzare l'impatto di un cambio di nome. `--old` trova anche frasi che contengono la vecchia resa italiana. I nomi lunghi e le voci singole vengono trattati in modo diverso per ridurre gli omonimi. Le stringhe oltre 2500 caratteri richiedono una verifica manuale separata.

`harvest` e `scan` non sovrascrivono una coda esistente. Usare un nuovo percorso `--out` per una nuova sessione, oppure `--overwrite` solo dopo aver archiviato le decisioni precedenti.

La scansione generale mostra al massimo cinque esempi per termine, così un nome molto frequente non riempie tutta la coda; omette inoltre le voci meteo, le razze e i clan e i nomi di luogo più corti di sei caratteri. `--term` rimuove questi filtri e trova tutte le occorrenze fino a `--limit`. Il catalogo dei luoghi comprende tutte le 5.302 righe di `world/placename.json`; le 20 discrepanze individuate sono state uniformate alle rese canoniche nel glossario. Per un cambio di nome, usare sempre `--term`, aumentare `--limit` se il comando segnala di averlo raggiunto e controllare anche i file approvati con `--include-approved`.

Quando cambia un nome canonico, aggiornare prima la riga approvata e la corrispondente voce del glossario, poi eseguire `scan --term "NOME INGLESE" --old "VECCHIA FORMA" --include-approved`. Le fonti del glossario devono continuare a corrispondere al testo dei file approvati.

Le segnalazioni sono ipotesi: l'assenza della forma letterale può essere una parafrasi corretta; una parola inglese può essere un nome proprio, un comando tecnico o avere un altro significato. La presenza della forma canonica non dimostra da sola che gli accordi siano giusti.

## 3. Revisionare nella chat di coding

Aprire una chat di coding con GPT-6 Luna e chiedere:

> Leggi `data/glossary/Glossary.md` e `data/glossary/review.json`. Per ogni riga `pending`, controlla il termine nel contesto di `original` e modifica solo `suggestion` nel JSON con la traduzione completa, facendo il minimo cambiamento necessario alla traduzione attuale. Rispetta `canonical` e `usages`; se il termine è un omonimo o la resa attuale è corretta, lascia `suggestion` vuoto e spiega brevemente in `note`. Conserva identici e nello stesso ordine tutti i tag `<...>` e `{...}`. Non modificare i file di traduzione e non approvare le righe.

Il modello si sceglie nella chat, non nello script. Il file della coda resta `pending` finché il revisore non controlla le proposte. Per cercare inglese ancora presente dentro i tag hex, eseguire anche:

```powershell
python scripts/scan_corrupted_values.py data/translations --report data/glossary/review_hex.json
```

Il report indica file, riga e frammento leggibile rimasto identico nel payload originale e tradotto. Sono segnalazioni da verificare: alcuni nomi interni del bytecode e nomi propri devono restare invariati. Per tradurre un payload SeString, ricodificare il testo in UTF-8 e aggiornare le lunghezze dei blocchi senza alterare i codici di controllo. Il controllo di integrità ordinario segnala anche i tag hex cambiati intenzionalmente; verificare tali casi nel contesto.

## 4. Approvare e applicare

Aprire `data/glossary/review.json`, controllare le frasi e impostare `suggestion` e `status: "approved"` solo per le correzioni volute. Per scartare una segnalazione, usare `status: "rejected"` e una `note`. Poi:

```powershell
python tools/terminology_review.py apply --dry-run
python tools/terminology_review.py apply
dotnet run --project src/FFXIVItalian.Extractor -- validate --review misc/addontransient.json
```

`apply` verifica che originale e traduzione siano ancora uguali a quelli della coda, conserva nell'ordine i tag `<...>` e `{...}`, e sostituisce solo il valore JSON del campo approvato. In caso di riga cambiata si ferma. Controllare il diff prima di considerare conclusa la revisione.
