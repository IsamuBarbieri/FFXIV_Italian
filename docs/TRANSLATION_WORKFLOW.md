# Workflow di traduzione assistita

Questo è il flusso principale quando il responsabile fornisce uno o più file da tradurre. I comandi batch rimangono disponibili come alternativa per lavorare su lotti esportati.

## 1. Definire il perimetro

- L'utente indica i file da tradurre e, se necessario, i file di riferimento o di allineamento.
- I file di riferimento forniscono contesto; si modificano solo se l'utente li indica anche come destinazioni.
- Si leggono glossario, guida di stile, prompt tecnico ed esempi vicini nel corpus.

## 2. Tradurre

- Si lavora caso per caso, preservando il senso, il contesto, il registro del personaggio e le scelte terminologiche approvate.
- Se una resa è incerta, cerca le correzioni recenti dell'utente in `data/translations/` e `data/da_revisionare/`. Usa le prime come canone; le seconde come precedenti contestuali, senza promuoverne automaticamente i termini al glossario.
- Si conservano ID, campi originali, metadati e schema JSON. Si modificano solo i campi di traduzione.
- I tag, i controlli e le variabili SeString restano intatti. I payload esadecimali con testo visibile si traducono e si ricodificano seguendo le istruzioni tecniche esistenti.
- Le ambiguità che dipendono dal contesto si segnalano invece di risolverle inventando informazioni.

## 3. Validare e revisionare

Esegui il controllo automatico appropriato per i file toccati. Per le traduzioni in revisione, il comando generale è:

```powershell
dotnet run --project src/FFXIVItalian.Extractor -- validate --review data/da_revisionare/<categoria>/<file>.json
```

Per i file approvati, `validate` controlla il corpus canonico. Per script o fogli speciali usa soltanto il validatore specifico che li copre; per esempio `tools/validate_new_sheets.py` controlla il gruppo di fogli dichiarato nel file.

Dopo il controllo automatico, rileggi ogni voce nel contesto per verificare significato, naturalezza, registro, genere, tag e coerenza terminologica. Il validatore non sostituisce questa revisione.

Per una scansione terminologica completa dei file in revisione e dei file canonici/deployati, usa:

```powershell
python tools/terminology_review.py scan --include-review --include-approved --all-matches --limit 0 --out data/glossary/review_all.json
```

La coda segnala possibili incoerenze, non errori certi. Controlla ogni candidato nell'intera voce e correggi anche la regola del revisore se produce falsi positivi. Il report non approva né sposta file.

## 4. Revisione dell'utente e correzioni

- L'utente fa un controllo visivo complessivo e indica i cambiamenti desiderati.
- Si applicano le correzioni ai file interessati e si ripete la validazione necessaria.
- Il file resta in revisione finché l'utente non ne approva il passaggio allo stato approvato.

## 5. Aggiornare il glossario

Aggiungi una voce quando l'utente conferma una scelta riutilizzabile. Indica una fonte `percorso#ID:campo` o, se è una scelta esplicita senza attestazione nel corpus, «Decisione dell'utente»; annota varianti e limiti di contesto. Non promuovere automaticamente una proposta del revisore o una soluzione valida per una sola frase.

## Workflow batch alternativo

Per traduzioni gestite tramite file batch:

```powershell
dotnet run --project src/FFXIVItalian.Extractor -- export-batch <foglio> --size 50
dotnet run --project src/FFXIVItalian.Extractor -- import-batch <foglio> data/batches/<batch>.json
dotnet run --project src/FFXIVItalian.Extractor -- autofill <foglio|all>
```

Per batch JSON esterni sono disponibili anche `scripts/split_untranslated.py` e `scripts/apply_all_translations.py`; quest'ultimo supporta `--validate-only` prima di modificare il master.
