# Revisione terminologica assistita

Il glossario approvato in `data/glossary/Glossary.md` resta la fonte editoriale. Un file completo non diventa automaticamente una fonte approvata: va revisionato e aggiunto all'elenco del glossario. Ogni voce nuova deve indicare `percorso#ID:campo` e una nota d'uso se il termine è ambiguo.

Lo strumento `tools/terminology_review.py` usa solo la libreria standard di Python. Non modifica le traduzioni durante la raccolta, la scansione o la generazione delle proposte. La coda JSON contiene la frase originale, la traduzione attuale, le varianti canoniche e le fonti. `status` parte sempre da `pending`.

## 1. Ampliare il glossario

```powershell
python tools/terminology_review.py harvest --file system/maincommand.json --out data/glossary/candidates_maincommand.json
python tools/terminology_review.py harvest --file world/placename.json --limit 200 --out data/glossary/candidates_placename.json
python tools/terminology_review.py harvest --file system/addon.json --include-unchanged --limit 200 --out data/glossary/candidates_addon.json
```

I candidati finiscono nel file passato a `--out` (`data/glossary/candidates.json` per impostazione predefinita), ordinati per numero di attestazioni nel file approvato. Esaminare significato e ricorrenza; inserire nel glossario solo le voci riutilizzabili, con la fonte e le eventuali eccezioni. `harvest` non promuove automaticamente nessuna voce. `--include-unchanged` comprende anche i nomi mantenuti in inglese. Tutti i nomi di `world/placename.json` e `system/maincommand.json` possono comunque essere cercati con `scan --term`, anche se non sono ancora righe del glossario; questo evita di copiare migliaia di toponimi nella tabella.

## 2. Creare una coda di revisione

```powershell
python tools/terminology_review.py scan --file misc/addontransient.json --limit 100
python tools/terminology_review.py scan --term "Duty Finder" --limit 100 --out data/glossary/review_duty_finder.json
python tools/terminology_review.py scan --term "The Waking Sands" --old "Sabbie del Risveglio" --out data/glossary/review_waking_sands.json
```

La coda predefinita è `data/glossary/review.json`. `--file` restringe la scansione; senza `--file` vengono letti tutti i JSON di traduzione. I file approvati sono esclusi dalla scansione ordinaria; `--include-approved` li include per analizzare l'impatto di un cambio di nome. `--old` trova anche frasi che contengono la vecchia resa italiana. I nomi lunghi e le voci singole vengono trattati in modo diverso per ridurre gli omonimi. Le stringhe oltre 2500 caratteri richiedono una verifica manuale separata.

`harvest` e `scan` non sovrascrivono una coda esistente. Usare un nuovo percorso `--out` per una nuova sessione, oppure `--overwrite` solo dopo aver archiviato le decisioni precedenti.

La scansione generale mostra al massimo cinque esempi per termine, così un nome molto frequente non riempie tutta la coda; omette inoltre le voci meteo, razze e clan e i termini con più rese italiane. `--term` rimuove questi filtri e trova tutte le occorrenze fino a `--limit`. Per un cambio di nome, usare sempre `--term`, aumentare `--limit` se il comando segnala di averlo raggiunto e controllare anche i file approvati con `--include-approved`.

Quando cambia un nome canonico, aggiornare prima la riga approvata e la corrispondente voce del glossario, poi eseguire `scan --term "NOME INGLESE" --old "VECCHIA FORMA" --include-approved`. Le fonti del glossario devono continuare a corrispondere al testo dei file approvati.

Le segnalazioni sono ipotesi: l'assenza della forma letterale può essere una parafrasi corretta; una parola inglese può essere un nome proprio, un comando tecnico o avere un altro significato. La presenza della forma canonica non dimostra da sola che gli accordi siano giusti.

## 3. Chiedere proposte a un modello locale (facoltativo)

Con Ollama avviato e `gemma4:12b` installato in locale:

```powershell
python tools/terminology_review.py suggest --limit 20
python tools/terminology_review.py suggest --model qwen3.5:9b --limit 20
```

Lo script usa l'endpoint locale `/api/generate` in modalità JSON senza streaming, documentato da [Ollama](https://docs.ollama.com/api/generate). Invia solo la singola frase, la traduzione attuale e le voci rilevanti. Il modello predefinito `gemma4:12b` ha applicato più voci canoniche di `qwen3.5:9b` in una piccola prova su sei righe reali. Entrambi girano interamente sulla RTX 4070 SUPER da 12 GB; Gemma usa quasi tutta la VRAM, mentre Qwen è l'opzione più leggera. La proposta resta `pending`. Se modifica i tag testuali o riscrive una parte troppo grande del testo, viene respinta. Non serve un modello per usare la coda: il revisore può compilare `suggestion` a mano.

## 4. Approvare e applicare

Aprire `data/glossary/review.json`, controllare le frasi e impostare `suggestion` e `status: "approved"` solo per le correzioni volute. Per scartare una segnalazione, usare `status: "rejected"` e una `note`. Poi:

```powershell
python tools/terminology_review.py apply --dry-run
python tools/terminology_review.py apply
dotnet run --project src/FFXIVItalian.Extractor -- validate --review misc/addontransient.json
```

`apply` verifica che originale e traduzione siano ancora uguali a quelli della coda, conserva nell'ordine i tag `<...>` e `{...}`, e sostituisce solo il valore JSON del campo approvato. In caso di riga cambiata si ferma. Controllare il diff prima di considerare conclusa la revisione.
