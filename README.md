# FFXIV Italiano: Suite di Localizzazione Professionale

Pipeline ingegneristica e suite di strumenti per la localizzazione italiana professionale di **Final Fantasy XIV**, basata sul caricatore mod a runtime **Penumbra** (plugin di Dalamud).

Il progetto estrae, gestisce, traduce e ricompila in formato binario nativo EXD l'intero corpus testuale del gioco (oltre 433.000 righe e 5.500 quest) senza toccare i file originali `.dat` di Square Enix.

La localizzazione comprende anche alcune texture dell'interfaccia: le scritte incorporate nelle immagini vengono adattate in italiano per rendere coerenti elementi visivi e testi di gioco. Le risorse sono organizzate in `data/assets/` e distribuite tramite Penumbra insieme alla mod.

---

## Stato del Progetto

- **Righe Totali Censite ed Estratte**: **433.250 righe**
- **Righe Totali Attualmente Tradotte**: **39.755 righe (9,2% del corpus di gioco)**
- **Quest Narrative Organizzate per Espansione**: **5.532 missioni** (ARR, Heavensward, Stormblood, Shadowbringers, Endwalker, Dawntrail)
- **Fogli al 100% Tradotti (22 fogli completi)**:
  - `addon.json` (14.976/14.976 - 100%): Interfaccia grafica completa, finestre di sistema, HUD, opzioni, indicatori e notifiche di gioco.
  - `placename.json` (5.302/5.302 - 100%): Toponimi completi, regioni, aree, insediamenti e landmark del mondo di gioco.
  - `status.json` (4.791/4.791 - 100%): Tutti gli status alterati, buff, debuff e descrizioni degli effetti di combattimento.
  - `achievement.json` (4.003/4.003 - 100%): Tutti i nomi, descrizioni e requisiti degli obiettivi e trofei del personaggio.
  - `actiontransient.json` (3.406/3.406 - 100%): Tutte le descrizioni dettagliate, effetti e parametri nei tooltip delle abilità.
  - `fate.json` (1.714/1.714 - 100%): Tutti i nomi e le descrizioni degli eventi F.A.T.E. nel mondo di gioco.
  - `lobby.json` (975/975 - 100%): Schermata del titolo, login, selezione e creazione personaggio, opzioni client, gestione server e data center.
  - `customtalk.json` (952/952 - 100%): Tutte le opzioni e prompt di interazione dei menu NPC (dialoghi brevi, opzioni servitori, chocobo, scambi, ecc.; script ID preservati intatti).
  - `title.json` (885/885 - 100%): Tutti i titoli onorifici dei personaggi giocanti (declinati sia al maschile che al femminile).
  - `trait.json` (682/682 - 100%): Tutti i nomi dei tratti passivi di classe e job di combattimento, gathering e crafting.
  - `traittransient.json` (682/682 - 100%): Tutte le descrizioni e i tooltip dettagliati dei tratti passivi.
  - `textcommand.json` (543/543 - 100%): Tutti i comandi chat slash e guide all'uso (comandi originali preservati per piena compatibilità con macro e guide esterne).
  - `howto.json` (262/262 - 100%): Tutte le guide e finestre tutorial di aiuto per principianti.
  - `weather.json` (209/209 - 100%): Tutte le condizioni meteorologiche di Eorzea e dei mondi di gioco.
  - `itemuicategory.json` (113/113 - 100%): Tutte le categorie dell'inventario e dell'armeria.
  - `maincommand.json` (99/99 - 100%): Menu comandi principali di gioco.
  - `error.json` (63/63 - 100%): Messaggi di errore di sistema e connettività.
  - `logmessage.json` (8.615/8.615 - 100%): Messaggi di sistema, notifiche e log di gioco.
  - `classjob.json` (44/44 - 100%): Tutte le classi e i job di combattimento, gathering e crafting.
  - `howtocategory.json` (16/16 - 100%): Categorie delle guide e dei tutorial.
  - `tribe.json` (16/16 - 100%): Tutti i clan e tribù dei personaggi.
  - `race.json` (8/8 - 100%): Tutte le razze giocabili di Eorzea.
  - `maincommandcategory.json` (7/7 - 100%): Categorie del menu principale.
- **Infrastruttura**:
  - Architettura a cartelle categorizzate (`system`, `world`, `combat`, `items`, `dialogue`, `quests`).
  - Suite script Python per partizionamento, validazione SeString 1:1 e reintegrazione atomica (`scripts/`).
  - Pipeline di traduzione modulare a batch (`status`, `export-batch`, `import-batch`, `autofill`).
  - Patcher binario con hot-deploy istantaneo nella cartella Penumbra attiva (22 file binari EXD registrati in `meta.json` e `default_mod.json`).

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
│   ├── PROMPT_COMPACT.md          # Prompt condensato ultra-ottimizzato per token saving
│   ├── STYLE_GUIDE.md             # Guida di stile linguistico, registri dei comprimari e formattazione
│   ├── TRANSLATION_PROMPT.md      # Metaprompt di sistema per LLM per preservare tag SeString
│   └── WORKFLOW_ANTIGRAVITY.md    # Protocollo operativo interno per sessioni con agente AI
├── scripts/
│   ├── split_untranslated.py      # Partizionamento file master in lotti compatti per subagenti
│   └── apply_all_translations.py  # Validazione SeString 1:1 rigorosa e reintegrazione atomica
├── src/
│   ├── FFXIVItalian.Core/         # Risolutore percorsi, modelli dati, validatore SeString e glossario
│   ├── FFXIVItalian.Extractor/    # Estrattore Lumina 7.x, cruscotto avanzamento, batch manager
│   ├── FFXIVItalian.DiffTool/     # Comparatore delta patch per aggiornamenti di gioco
│   └── FFXIVItalian.Patcher/      # Compilatore binario EXD, generatore .pmp e deployer Penumbra
├── tests/
│   └── FFXIVItalian.Tests/        # Test suite xUnit (39 test: SeString, BatchManager, PathResolver, ecc.)
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
   - Utilizzare le istruzioni di [docs/PROMPT_COMPACT.md](file:///docs/PROMPT_COMPACT.md) (o [docs/TRANSLATION_PROMPT.md](file:///docs/TRANSLATION_PROMPT.md)) e il canone di [docs/STYLE_GUIDE.md](file:///docs/STYLE_GUIDE.md).
   - Preservare inalterati tutti i codici esadecimali `<hex:...>` e i tag di formattazione SeString.
3. **Importare il batch tradotto nel foglio master**:
   ```powershell
   dotnet run --project src/FFXIVItalian.Extractor -- import-batch addon data/batches/addon_batch_XXXXX.json
   ```
4. **Propagare traduzioni identiche (`autofill`)**:
   ```powershell
   dotnet run --project src/FFXIVItalian.Extractor -- autofill
   ```

Per l'architettura parallela avanzata con subagenti e la guida rapida token-zero, consulta [docs/WORKFLOW_ANTIGRAVITY.md](file:///docs/WORKFLOW_ANTIGRAVITY.md) e [docs/BATCH_TRANSLATION_GUIDE.md](file:///docs/BATCH_TRANSLATION_GUIDE.md).

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
   - `Scions of the Seventh Dawn` -> *Figli della Settima Alba*
   - `Aetheryte` -> *Eterite* (invariabile al plurale)
   - `Gil` -> *Gil* (invariabile)
   - Unità di misura eorzeane invariabili (*yalm, fulm, malm, ilms*).
3. **Concordanza di Genere e Profilazione Vocale**:
   - Ove possibile, si utilizzano forme inclusive o macro native di genere FFXIV.
   - Ogni comprimario (Urianger, Thancred, Alphinaud, Y'shtola, Tataru, Estinien, Emet-Selch) segue il registro linguistico documentato nella guida di stile.

---

## Come Contribuire
Se volete partecipare a questo ambizioso progetto potete farlo in varie maniere. Potete revisionare le traduzioni se trovate incongruenze o frasi sbagliate, convertire voi stessi pezzi mancanti con l'AI, oppure potete fare una [donazione](https://ko-fi.com/xeladon) per aiutarmi a sostenere i costi di traduzione. Qualsiasi forma di supporto é bena accetta, anche solo far conoscere il progetto a piú persone possibili. 
Grazie.

## Licenza e Diritti
Questo progetto è un'iniziativa fan-made amatoriale della community italiana. Final Fantasy XIV e tutti i relativi asset, marchi e testi sono proprietà intellettuale e copyright di **SQUARE ENIX CO., LTD.**
Nessun file proprietario protetto da copyright viene ridistribuito nel repository.
