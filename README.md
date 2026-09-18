# FFXIV Italiano: Suite di Localizzazione Professionale

Pipeline software e strumenti ingegneristici per la localizzazione italiana professionale di **Final Fantasy XIV**, basata sul caricatore a runtime **Penumbra** (plugin di Dalamud).

---

## Caratteristiche Principali

1. **Architettura Penumbra `.pmp` (Zero Corruzioni)**:
   - I file originali `.dat` del gioco non vengono mai toccati.
   - Copertura del 100% del gioco: MSQ, secondarie, cutscene, menu, abilità, descrizioni, tooltip e lore.
   - Nessun calo di frame rate e totale stabilità a runtime.
2. **Motore di Diffing per le Patch di Gioco (`DiffTool`)**:
   - Algoritmo di hashing SHA-256 a livello di singola cella per ogni foglio EXD.
   - Quando Square Enix rilascia una nuova patch, il sistema isola istantaneamente solo le nuove righe `[NEW]` e quelle modificate `[MODIFIED]`, senza dover ricontrollare 800.000 righe.
   - *Best-effort su Patch Day*: il modpack `.pmp` può essere aggiornato immediatamente preservando tutte le traduzioni approvate.
3. **Risoluzione Automatica della Concordanza di Genere**:
   - Lookup automatico nei fogli binari `ENpcBase` / `ENpcResident` per estrarre il sesso anagrafico degli NPC parlanti e destinatari.
   - Integrazione delle macro native `SeString` del motore di gioco per adattare dinamicamente gli aggettivi e i participi al sesso del Guerriero della Luce.
4. **Single Source of Truth Terminologica**:
   - Piena adozione del canone di `07_Glossary.md` (v1.43).
   - Linter integrato contro forme vietate (*es. Maelstrom, Malattia Eterica, Il Bevitoio di Buscarron*) e per il rispetto ferreo delle unità eorzeane (*yalm, fulm, malm* invariabili).
5. **Profili Vocali dei Personaggi**:
   - Matrice dei registri linguistici dedicata per ogni comprimario (l'aulico Urianger, lo scanzonato Thancred, il diplomatico Alphinaud, la diretta Alisaie, la sferzante Y'shtola, la solare Tataru, il laconico Estinien e il teatrale Emet-Selch).

---

## Struttura della Soluzione

```
FFXIV_Italian/
├── data/
│   ├── glossary/                  # 07_Glossary.md e database indicizzato glossary.json
│   ├── snapshots/                 # Snapshot storici del testo originale per versione di patch
│   └── translations/              # Corpus di traduzione in formato JSONL
├── docs/
│   ├── ARCHITECTURE.md            # Dettagli dell'architettura software e pipeline
│   └── STYLE_GUIDE.md             # Guida di stile, registri vocali e convenzioni editoriali
├── src/
│   ├── FFXIVItalian.Core/         # Modelli dati, linter SeString, motore glossario e diffing
│   ├── FFXIVItalian.Extractor/    # Estrattore Lumina 7.x da SqPack con lookup genere NPC
│   ├── FFXIVItalian.DiffTool/     # CLI per analisi comparativa del delta tra patch
│   └── FFXIVItalian.Patcher/      # Generatore di pacchetti .pmp per Penumbra
└── tests/
    └── FFXIVItalian.Tests/        # Test suite xUnit (Glossary, SeString, Diff, Packaging)
```

---

## Prerequisiti

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Installazione locale di Final Fantasy XIV (es. `G:\SquareEnix\FINAL FANTASY XIV - A Realm Reborn`)
- [XIVLauncher](https://goatcorp.github.io/) con il plugin **Penumbra** installato in Dalamud.

---

## Guida Rapida ai Comandi

### 1. Compilare l'intera soluzione
```powershell
dotnet build
```

### 2. Eseguire la suite di test
```powershell
dotnet test
```

### 3. Rigenerare il database del glossario canonico
```powershell
dotnet run --project src/FFXIVItalian.DiffTool -- init-glossary --out data/glossary/glossary.json
```

### 4. Testare l'estrazione dati e il controllo del genere NPC
```powershell
dotnet run --project src/FFXIVItalian.Extractor
```

### 5. Generare il pacchetto mod `.pmp` per Penumbra
```powershell
dotnet run --project src/FFXIVItalian.Patcher -- FFXIV_Italian.pmp
```

---

## Licenza e Diritti
Questo progetto è un'iniziativa di localizzazione fan-made della community. Final Fantasy XIV e tutti i relativi asset, marchi e testi sono copyright di SQUARE ENIX CO., LTD.
Nessun file originale protetto da copyright viene redistribuito in questo repository.
