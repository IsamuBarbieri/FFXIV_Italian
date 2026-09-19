# FFXIV Italiano: Suite di Localizzazione Professionale

Pipeline ingegneristica e suite di strumenti per la localizzazione italiana professionale di **Final Fantasy XIV**, basata sul caricatore mod a runtime **Penumbra** (plugin di Dalamud).

Il progetto estrae, gestisce, traduce e ricompila in formato binario nativo EXD l'intero corpus testuale del gioco (oltre 433.000 righe e 5.500 quest) senza toccare i file originali `.dat` di Square Enix.

---

## Stato del Progetto

- **Righe Totali Censite ed Estratte**: **433.250 righe**
- **Quest Narrative Organizzate per Espansione**: **5.532 missioni** (ARR, Heavensward, Stormblood, Shadowbringers, Endwalker, Dawntrail)
- **Fogli al 100% Tradotti (12 fogli core)**:
  - `lobby.json` (975/975 - 100%): Schermata del titolo, login, selezione e creazione personaggio, opzioni client, gestione server e data center.
  - `textcommand.json` (543/543 - 100%): Tutti i comandi chat slash e guide all'uso (comandi originali preservati per piena compatibilità con macro e guide esterne).
  - `howto.json` (262/262 - 100%): Tutte le guide e finestre tutorial di aiuto per principianti.
  - `weather.json` (209/209 - 100%): Tutte le condizioni meteorologiche di Eorzea e dei mondi di gioco.
  - `itemuicategory.json` (113/113 - 100%): Tutte le categorie dell'inventario e dell'armeria.
  - `maincommand.json` (99/99 - 100%): Menu comandi principali di gioco.
  - `error.json` (63/63 - 100%): Messaggi di errore di sistema e connettività.
  - `classjob.json` (44/44 - 100%): Tutte le classi e i job di combattimento, gathering e crafting.
  - `howtocategory.json` (16/16 - 100%): Categorie delle guide e dei tutorial.
  - `tribe.json` (16/16 - 100%): Tutti i clan e tribù dei personaggi.
  - `race.json` (8/8 - 100%): Tutte le razze giocabili di Eorzea.
  - `maincommandcategory.json` (7/7 - 100%): Categorie del menu principale.
- **Infrastruttura**:
  - Architettura a cartelle categorizzate (`system`, `world`, `combat`, `items`, `dialogue`, `quests`).
  - Pipeline di traduzione modulare a batch (`status`, `export-batch`, `import-batch`, `autofill`).
  - Patcher binario con hot-deploy istantaneo nella cartella Penumbra attiva.

---

## Architettura del Repository

```
FFXIV_Italian/
├── data/
│   ├── batches/                   # File batch temporanei per traduzioni (export/import)
│   ├── glossary/                  # 07_Glossary.md (canone terminologico) e glossary.json
│   └── translations/              # Corpus completo dei testi estratti e tradotti
│       ├── system/                # UI, Addon, Lobby, Error, MainCommand, HowTo, TextCommand, LogMessage
│       ├── world/                 # ClassJob, Race, Tribe, PlaceName, Weather, Fate, Achievement, Title
│       ├── combat/                # Action, ActionTransient, Status, Trait, TraitTransient
│       ├── items/                 # Item, ItemUiCategory
│       ├── dialogue/              # Balloon, CustomTalk, DefaultTalk
│       └── quests/                # 5.532 missioni suddivise per espansione (arr, heavensward, etc.)
├── docs/
│   ├── ARCHITECTURE.md            # Architettura software, formati file e ciclo di vita patch
│   ├── BATCH_TRANSLATION_GUIDE.md # Guida pratica alla traduzione a batch con AI o manuale
│   ├── STYLE_GUIDE.md             # Guida di stile linguistico, registri dei comprimari e formattazione
│   └── TRANSLATION_PROMPT.md      # Metaprompt di sistema per LLM per preservare tag SeString
├── src/
│   ├── FFXIVItalian.Core/         # Risolutore percorsi, modelli dati, validatore SeString e glossario
│   ├── FFXIVItalian.Extractor/    # Estrattore Lumina 7.x, cruscotto avanzamento, batch manager
│   ├── FFXIVItalian.DiffTool/     # Comparatore delta patch per aggiornamenti di gioco
│   └── FFXIVItalian.Patcher/      # Compilatore binario EXD, generatore .pmp e deployer Penumbra
├── tests/
│   └── FFXIVItalian.Tests/        # Test suite xUnit (38 test: SeString, BatchManager, PathResolver, ecc.)
├── rebuild.bat                    # Script one-click per ricompilare il modpack
└── rebuild.ps1                    # Script PowerShell one-click per ricompilare il modpack
```

---

## Prerequisiti

1. [.NET 10 SDK](https://dotnet.microsoft.com/download)
2. Installazione locale di **Final Fantasy XIV** (es. `G:\SquareEnix\FINAL FANTASY XIV - A Realm Reborn`)
3. [XIVLauncher](https://goatcorp.github.io/) con il plugin **Penumbra** configurato in Dalamud.

---

## Guida Operativa ai Comandi

### 1. Cruscotto di Avanzamento (`status`)
Mostra in tempo reale lo stato di tutti i fogli di gioco divisi per categoria, con il conteggio di righe tradotte, pendenti e percentuale:
```powershell
dotnet run --project src/FFXIVItalian.Extractor -- status
```

### 2. Workflow di Traduzione a Batch

Per tradurre senza sprecare token e con la massima precisione:

1. **Esportare un lotto di righe non tradotte**:
   ```powershell
   # Esporta fino a 100 righe pendenti dal foglio addon
   dotnet run --project src/FFXIVItalian.Extractor -- export-batch addon --limit 100
   ```
2. **Tradurre il file generato in `data/batches/`**:
   - Utilizzare le istruzioni di [docs/TRANSLATION_PROMPT.md](file:///docs/TRANSLATION_PROMPT.md) e il canone di [docs/STYLE_GUIDE.md](file:///docs/STYLE_GUIDE.md).
   - Preservare inalterati tutti i codici esadecimali `<hex:...>` e i tag di formattazione SeString.
3. **Importare il batch tradotto nel foglio master**:
   ```powershell
   dotnet run --project src/FFXIVItalian.Extractor -- import-batch addon data/batches/addon_batch_XXXXX.json
   ```
4. **Propagare traduzioni identiche (`autofill`)**:
   ```powershell
   dotnet run --project src/FFXIVItalian.Extractor -- autofill
   ```

Per la guida completa al workflow consulta [docs/BATCH_TRANSLATION_GUIDE.md](file:///docs/BATCH_TRANSLATION_GUIDE.md).

---

### 3. Ricompilazione e Applicazione della Mod nel Gioco

Per compilare i fogli EXD tradotti, generare il pacchetto Penumbra `.pmp` e aggiornare istantaneamente la cartella mod attiva:

```powershell
# Tramite script veloce
.\rebuild.bat

# Oppure tramite CLI
dotnet run --project src/FFXIVItalian.Patcher
```

> [!NOTE]
> Il patcher rileva automaticamente il percorso attivo della cartella Penumbra (es. `G:\SquareEnix\FFXIV_Mod\FFXIV Italiano (Test In-Game)`) e copia direttamente i file EXD rigenerati, aggiornando anche `meta.json` e `default_mod.json`.
>
> Inoltre, verifica e garantisce che l'opzione Dalamud `IsResumeGameAfterPluginLoad` sia abilitata in `dalamudConfig.json`, prevenendo il caricamento anticipato di schermate o font non tradotti.

---

### 4. Estrazione ed Ispezione Dati

```powershell
# Ispezionare la struttura di colonne di un qualsiasi foglio EXD originale
dotnet run --project src/FFXIVItalian.Extractor -- inspect Addon

# Estrarre un singolo foglio (o aggiornarlo dai file di gioco)
dotnet run --project src/FFXIVItalian.Extractor -- extract addon

# Estrarre tutte le 5.532 quest narrative divise per espansione
dotnet run --project src/FFXIVItalian.Extractor -- extract-quests

# Riorganizzare automaticamente eventuali fogli piatti nella struttura a categorie
dotnet run --project src/FFXIVItalian.Extractor -- reorganize
```

---

### 5. Suite di Test

Esegui tutti i test automatici per verificare la tenuta dei percorsi, del motore a batch, dei tag SeString e del glossario:
```powershell
dotnet test
```

---

## Filosofia e Regole di Traduzione

1. **Integrità Tecnica Assoluta**:
   - Nessun tag di controllo SeString (`<hex:...>`, `<Sheet(...)>`, `<Highlight>`, ecc.) o carattere speciale Unicode (`\uE051`, `\uE052`, `\u203B`) deve essere rimosso o alterato.
2. **Canone di Gioco (`07_Glossary.md`)**:
   - `Warrior of Light` -> *Guerriero della Luce*
   - `Scions of the Seventh Dawn` -> *Sapienti della Settima Aurora*
   - `Aetheryte` -> *Eterite* (invariabile al plurale)
   - `Gil` -> *Gil* (invariabile)
   - Unità di misura eorzeane invariabili (*yalm, fulm, malm, ilms*).
3. **Concordanza di Genere e Profilazione Vocale**:
   - Ove possibile, si utilizzano forme inclusive o macro native di genere FFXIV.
   - Ogni comprimario (Urianger, Thancred, Alphinaud, Y'shtola, Tataru, Estinien, Emet-Selch) segue il registro linguistico documentato nella guida di stile.

---

## Licenza e Diritti
Questo progetto è un'iniziativa fan-made amatoriale della community italiana. Final Fantasy XIV e tutti i relativi asset, marchi e testi sono proprietà intellettuale e copyright di **SQUARE ENIX CO., LTD.**
Nessun file proprietario protetto da copyright viene ridistribuito nel repository.
