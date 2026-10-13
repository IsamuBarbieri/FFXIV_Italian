# Final Fantasy XIV Italiano

Progetto fan-made per tradurre in italiano i testi di Final Fantasy XIV e creare un mod installabile con Penumbra. L'estrattore legge i dati dall'installazione locale del gioco; il patcher genera file EXD e il pacchetto Penumbra senza modificare gli archivi originali.

## Stato del progetto

Fotografia del corpus al 3 ottobre 2026. Per i valori aggiornati usa il comando `status`.

| Indicatore | Valore |
| --- | ---: |
| Righe nel corpus | 590.561 |
| Righe complete secondo l'estrattore | 130.300 (22,1%) |
| Righe ancora pendenti | 460.261 |
| File JSON da tradurre | 5.567 |
| File in revisione | 0 |
| File approvati | 367 |
| File delle quest narrative | 5.532 |

Una riga è completa quando tutti i campi testuali previsti hanno una traduzione. Lo stato editoriale è distinto dalla copertura: solo i file approvati vengono compilati dal patcher.

## Come lavoriamo sulle traduzioni

Il responsabile fornisce uno o più file e, quando serve, file di riferimento per l'allineamento. Le traduzioni vengono prodotte e controllate caso per caso, passano nel validatore automatico, ricevono una revisione visiva e vengono corrette in base al feedback. Le scelte ricorrenti confermate si registrano nel glossario.

Il validatore controlla vincoli tecnici e coerenza terminologica; non sostituisce il controllo linguistico o l'approvazione umana. Vedi [il workflow di traduzione](docs/TRANSLATION_WORKFLOW.md) e [AI_USAGE.md](AI_USAGE.md).

### Stati editoriali

| Percorso | Stato |
| --- | --- |
| `data/da_tradurre/<categoria>/` | Senza traduzione italiana. |
| `data/da_revisionare/<categoria>/` | Traduzioni presenti, in attesa di revisione. |
| `data/translations/<categoria>/` | File approvati e inclusi nel build della mod. |

Le categorie sono `activities`, `combat`, `crafting`, `dialogue`, `housing`, `items`, `minigames`, `quests`, `shops`, `social`, `system` e `world`. Le quest narrative sono suddivise per espansione e cartella numerica. Dopo nuove estrazioni o importazioni, esegui `python tools/organize_sheets.py` per riallineare gli stati e le categorie.

Il patcher carica automaticamente i JSON approvati in `data/translations/` quando trova il relativo EXH nel client. Gli handler dedicati conservano le eccezioni di formato dei fogli già curati. Le risorse in `data/assets/` mantengono il percorso relativo; i file nelle altre cartelle sotto `data/` vengono inclusi automaticamente usando il loro percorso relativo a `data/`.

Nelle opzioni del mod in Penumbra puoi attivare o disattivare dieci blocchi di traduzione. Sono preselezionati per impostazione predefinita; dialoghi e missioni condividono il blocco **Missioni**, mentre i contenuti non assegnati a un blocco specifico rientrano in **Interfaccia**.

## Struttura del repository

| Percorso | Contenuto |
| --- | --- |
| `data/` | Corpus JSON, glossario e risorse localizzate. |
| `src/FFXIVItalian.Core` | Modelli, percorsi, glossario e gestione SeString. |
| `src/FFXIVItalian.Extractor` | Estrazione, stato, ricerca e flussi di traduzione a batch. |
| `src/FFXIVItalian.DiffTool` | Confronto delle versioni del gioco. |
| `src/FFXIVItalian.Patcher` | Compilazione EXD e generazione/deploy Penumbra. |
| `tools/` e `scripts/` | Strumenti operativi e validatori. |
| `tests/` | Test C# e Python. |
| `docs/` | Architettura, traduzione, terminologia e verifiche. |
| `tools/README.md` | Uso dei generatori e delle utility per texture. |
| `rebuild.bat` e `rebuild.ps1` | Avvio rapido del rebuild della mod. |
| `FFXIV_Italian.pmp` | Pacchetto Penumbra versionato nel repository. |

Per la mappa completa consulta [l'indice dei documenti](docs/README.md).

## Comandi principali

Prerequisiti: .NET 10 SDK, installazione locale di FFXIV e Penumbra configurato in Dalamud.

```powershell
# Avanzamento corrente
dotnet run --project src/FFXIVItalian.Extractor -- status

# Dopo un aggiornamento del client: confronto e copie aggiornate, senza modificare il corpus
dotnet run --project src/FFXIVItalian.Extractor -- update

# Integra file nuovi e righe aggiunte; conserva le voci modificate per la revisione
dotnet run --project src/FFXIVItalian.Extractor -- update --integrate

# Validare il corpus approvato
dotnet run --project src/FFXIVItalian.Extractor -- validate

# Controllare un file in revisione
dotnet run --project src/FFXIVItalian.Extractor -- validate --review data/da_revisionare/<categoria>/<file>.json

# Traduzione batch alternativa
dotnet run --project src/FFXIVItalian.Extractor -- export-batch <foglio> --size 50
dotnet run --project src/FFXIVItalian.Extractor -- import-batch <foglio> data/batches/<batch>.json
dotnet run --project src/FFXIVItalian.Extractor -- autofill <foglio|all>

# Test e rebuild del mod
dotnet test
python -m unittest discover -s tests
.\rebuild.bat
```

In alternativa, genera o applica la mod con `dotnet run --project src/FFXIVItalian.Patcher`. Le build del patcher usano i file approvati; non modificano i file originali di FFXIV.

Il comando `update` legge il client locale (il percorso predefinito è in G; è disponibile `--sqpack`) e confronta i fogli inglesi di `root.exl` con tutti e tre gli stati editoriali. Ogni esecuzione crea una cartella distinta in `data/updates/`, esclusa da Git, con `report.md`, `report.json`, copie aggiornate in `updated/` e copie originali in `backup/`. Il rapporto elenca file nuovi, ID aggiunti, ID modificati con i campi interessati, righe rimosse ed errori. Il JSON contiene le voci prima e dopo, comprese le traduzioni conservate.

La base di confronto è il corpus attuale: «file nuovo» significa assente dal corpus, anche se già esistente nel vecchio client. `update` senza opzioni non modifica il corpus. Con `--integrate`, i fogli nuovi entrano in `data/da_tradurre/` e le righe aggiunte vengono inserite nel file esistente, nello stesso stato editoriale, con traduzione vuota. Traduzioni, metadati e sorgenti delle righe già presenti restano intatti. L'integrazione si sospende se il confronto ha errori o se un file è cambiato dopo la lettura.

Le sorgenti modificate si controllano in `review.md`: ogni campo mostra prima, dopo e traduzione attuale. Dopo il controllo, aggiorna nel file del corpus la sorgente e, se serve, la traduzione di quella voce; conserva lo stato editoriale e valida il file. Il successivo `update` non segnalerà più le differenze risolte. Le righe rimosse restano conservate e segnalate, finché non si decide come trattarle. Non sostituire in blocco i file approvati con le copie di `updated/`. Il comando non approva file e non ricompila la mod. Per il flusso completo consulta [Aggiornamenti del client](docs/GAME_UPDATE_WORKFLOW.md).

## Documentazione e contributi

- [Guida di stile](docs/STYLE_GUIDE.md)
- [Prompt tecnico per le traduzioni](docs/TRANSLATION_PROMPT.md)
- [Glossario approvato](data/glossary/Glossary.md)
- [Architettura](docs/ARCHITECTURE.md)
- [Come contribuire](CONTRIBUTING.md)

Final Fantasy XIV, i suoi testi e i relativi asset appartengono a Square Enix. Questo progetto è una localizzazione fan-made, non ufficiale e non affiliata a Square Enix.
