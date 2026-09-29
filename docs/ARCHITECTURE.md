# Architettura Tecnica: FFXIV Italiano

Questo documento illustra l'architettura software, i flussi di dati e l'ingegneria del sistema di localizzazione per **Final Fantasy XIV**.

---

## 1. Visione d'Insieme della Pipeline

```mermaid
flowchart TD
    SqPack["FFXIV SqPack (.dat / .index)"] --> Extractor["FFXIVItalian.Extractor (Lumina 7.7)"]
    
    subgraph Storage["data/"]
        pending["da_tradurre/ (nessuna traduzione)"]
        review["da_revisionare/ (traduzioni da revisionare)"]
        approved["translations/ (file approvati)"]
    end
    
    Extractor --> Storage
    
    subgraph BatchSystem["Batch Translation Pipeline"]
        Export["export-batch"]
        Import["import-batch"]
        Autofill["autofill (cross-sheet memory)"]
    end
    
    Storage <--> BatchSystem
    
    Storage --> Patcher["FFXIVItalian.Patcher"]
    Patcher --> EXD["Compilazione Binaria EXD"]
    EXD --> PMP["FFXIV_Italian.pmp (Archivio Penumbra v4)"]
    EXD --> ActiveMod["Deploy Diretto Cartella Penumbra"]
    ActiveMod --> Dalamud["Dalamud / FFXIV Runtime"]
```

---

## 2. Moduli della Soluzione

### A. `FFXIVItalian.Core`
La libreria di base contenente modelli, astrazioni e validatori:
- **`TranslationPathResolver`**:
  - Risolve automaticamente i nomi dei fogli sia con percorsi piatti legacy che all'interno delle categorie (`system/`, `world/`, `combat/`, `items/`, `dialogue/`, `quests/`).
  - Supporta la scansione ricorsiva di tutti i file JSON e il censimento statistico delle traduzioni.
  - Offre la funzione di categorizzazione automatica (`reorganize`) per spostare i file nella cartella di appartenenza corretta.
- **`TranslationFileReader`**:
  - Carica e analizza i file JSON sia in formato standard a dizionario che in formato specializzato (es. 2-column per le quest con `tag`, `original`, `translation`).
- **`SeString/SeStringValidator`**:
  - Validatore sintattico per tag interni di FFXIV (`<hex:...>`, `<Highlight>`, `<FullName>`, macro condizionali di genere). Rileva tag mancanti o malformati prima della compilazione.
- **`Glossary/GlossaryEngine`**:
  - Motore di coerenza terminologica basato sulle voci approvate di `data/glossary/Glossary.md`; segnala possibili incoerenze da rivedere nel contesto.

---

### B. `FFXIVItalian.Extractor`
Il componente di estrazione ed elaborazione dati:
- **`UniversalSheetExtractor`**:
  - Si interfaccia con **Lumina 7.7** leggendo direttamente gli archivi `SqPack` ufficiali di Final Fantasy XIV.
  - Riconosce dinamicamente tipi di colonne testuali (`SeString`, stringhe semplici) e indici numerici primari, estraendo qualsiasi foglio del gioco senza richiedere codice specifico per foglio.
- **`QuestExtractor`**:
  - Estrae il catalogo completo di tutte le missioni del gioco (oltre 5.500 file di testo narrative).
  - Filtra automaticamente le righe vuote o prive di testo.
  - Organizza i file per espansione (`arr`, `heavensward`, `stormblood`, `shadowbringers`, `endwalker`, `dawntrail`) e cartella numerica (`000`, `001`, ecc.).
- **`BatchManager`**:
  - Gestisce l'esportazione selettiva di righe non tradotte (`export-batch`), l'importazione sicura (`import-batch`), la propagazione di testi ricorrenti (`autofill`) e la visualizzazione dello stato globale (`status`).

---

### C. `FFXIVItalian.Patcher`
Il generatore e distributore della mod:
- **Compilazione Binaria EXD**:
  - Ricostruisce le tabelle binarie EXD secondo la specifica Square Enix (header EXDF, indici di riga, offset e codifica UTF-8/SeString).
- **Integrazione Penumbra v4**:
  - Genera `meta.json` e `default_mod.json` per mappare ogni foglio EXD modificato sul rispettivo percorso virtuale originale.
  - Produce il pacchetto compresso `.pmp`.
  - **Hot-Deploy Diretto**: Copia i binari compilati e i descrittori direttamente nella cartella attiva di Penumbra (configurata in `appsettings.json` o rilevata automaticamente).
- **Sincronizzazione Dalamud**:
  - Verifica e imposta la chiave `IsResumeGameAfterPluginLoad: true` nel file di configurazione di Dalamud (`dalamudConfig.json`), garantendo che il gioco attenda il caricamento completo della mod prima di mostrare la schermata del titolo.

---

### D. `FFXIVItalian.DiffTool`
Strumento per la manutenzione e il monitoraggio degli aggiornamenti di gioco:
- **Hashing a Livello di Cella**: Confronta snapshot tra diverse versioni del gioco per identificare istantaneamente nuove righe (`[NEW]`) o modifiche (`[MODIFIED]`), proteggendo le traduzioni esistenti senza dover rielaborare l'intero database.

---

## 3. Organizzazione del Corpus (`data/`)

I file senza approvazione sono separati dal corpus approvato. `data/da_tradurre/` contiene file senza traduzioni; `data/da_revisionare/` contiene file con almeno una traduzione non approvati; i file elencati nella sezione **File approvati** del glossario restano in `data/translations/`, nelle categorie `system/`, `world/` e le altre aree. Dentro ciascuno stato editoriale ci sono le categorie `activities`, `combat`, `crafting`, `dialogue`, `housing`, `items`, `minigames`, `quests`, `shops`, `social`, `system` e `world`. Le quest narrative usano `quests/<espansione>/<numero>/`; i fogli master relativi alle missioni sono in `quests/master/`.

Per aggiornare le cartelle dopo un'importazione o una nuova estrazione: `python tools/organize_sheets.py`. La presenza di una traduzione non equivale alla revisione, e un file parzialmente tradotto rimane in `da_revisionare/` finché non è approvato.

Percorsi esemplificativi:

| Categoria | Descrizione | Fogli Principali |
|---|---|---|
| `system/` | Interfaccia revisionata | `addon.json`, `lobby.json` |
| `world/` | Nomi revisionati | `classjob.json`, `placename.json` |
| `data/da_revisionare/combat/` | Combattimento con traduzioni da verificare | `action.json`, `status.json` |
| `data/da_revisionare/items/` | Oggetti con traduzioni da verificare | `item.json`, `itemuicategory.json` |
| `data/da_tradurre/dialogue/` | Dialoghi senza traduzioni | `balloon.json`, `defaulttalk.json` |
| `data/da_tradurre/quests/` | Quest narrative senza traduzioni | Per espansione e cartella numerica |

---

## 4. Gestione di Nuove Patch ed Espansioni

Quando Square Enix pubblica una nuova patch o espansione:

1. **Riestrazione o aggiornamento incrementale**:
   ```powershell
   dotnet run --project src/FFXIVItalian.Extractor -- extract
   ```
   Il sistema aggiorna il corpus: le righe già tradotte in italiano rimangono intatte, mentre le nuove righe in lingua inglese vengono aggiunte come pendenti.
2. **Controllo con il cruscotto**:
   ```powershell
   dotnet run --project src/FFXIVItalian.Extractor -- status
   ```
   Le nuove righe compaiono immediatamente come pendenti nel conteggio della categoria appropriata.
3. **Traduzione incrementale a batch**:
   È possibile esportare solo le righe nuove della patch ed importarle senza rischiare regressioni sul testo pregresso.
4. **Rebuild della mod**:
   ```powershell
   .\rebuild.bat
   ```
   I file binari EXD vengono ricompilati in pochi secondi con le nuove stringhe pronte in-game.
