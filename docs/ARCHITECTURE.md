# Architettura Tecnica: FFXIV Italiano

Questo documento illustra l'architettura ingegneristica della suite di strumenti per la localizzazione italiana di **Final Fantasy XIV**.

---

## 1. Panoramica del Flusso di Lavoro

```
[ FFXIV SqPack Data ]
         │
         ▼
[ FFXIVItalian.Extractor ] ───(Lumina 7.x + ENpcBase Gender Lookup)
         │
         ▼
[ data/snapshots/snapshot_vX.json ]
         │
         ├──────────────────────────────────────────┐
         │ (Nuova Patch Rilasciata)                 │ (Prima Estrazione)
         ▼                                          ▼
[ FFXIVItalian.DiffTool ]                   [ Pipeline di Traduzione ]
         │ (Row-Level Hash Compare)                 │ (Integrazione Glossario G0-G28)
         ├─► patch_diff_report.md                   │ (SeString Syntax Validator)
         └─► Delta Queue [NEW] & [MODIFIED]         │
                         │                          ▼
                         └──────────────► [ data/translations/*.jsonl ]
                                                    │
                                                    ▼
                                       [ FFXIVItalian.Patcher ]
                                                    │
                                                    ▼
                                          [ FFXIV_Italian.pmp ]
                                                    │
                                                    ▼
                                        (Import in Penumbra / Dalamud)
```

---

## 2. Moduli della Soluzione

### A. `FFXIVItalian.Core`
La libreria condivisa che implementa i modelli e la logica di business:
- **`Models/TranslationRow`**: Rappresentazione standard di una riga di testo, con identificativo di foglio (`SheetName`), indici di riga/sotto-riga, stato di traduzione (`Untranslated`, `Draft`, `Approved`, `Gold`), hash SHA-256 e metadati contestuali per il genere (`SpeakerGender`, `TargetGender`).
- **`SeString/SeStringValidator`**: Analizzatore sintattico per la validazione di markup e macro del motore di FFXIV. Rileva tag sbilanciati (`<Highlight>`), parentesi angolari errate o cancellazione accidentale di placeholder obbligatori (`<FullName>`, `<Forename>`).
- **`Glossary/GlossaryEngine` & `GlossaryLoader`**: Motore di conformità terminologica basato su `07_Glossary.md`. Blocca forme vietate (*es. Maelstrom, Malattia Eterica, Il Bevitoio di Buscarron*) e sanziona conversioni errate delle unità eorzeane (*es. iarde invece di yalm*). Include la matrice dei registri vocali per i comprimari.
- **`Diff/PatchDiffEngine`**: Motore di comparazione tra snapshot di patch diverse.

### B. `FFXIVItalian.Extractor`
- Si interfaccia direttamente con i file `SqPack` del gioco locale (es. `G:\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack`) tramite la libreria **Lumina 7.7.0**.
- Estrae fogli testuali (`Addon`, `Action`, `PlaceName`, `Quest`, `CustomTalk`, ecc.).
- Correla automaticamente ogni riga di dialogo con le tabelle `ENpcResident` ed `ENpcBase` per recuperare il genere anagrafico dell'NPC (`0 = Maschio, 1 = Femmina`).

### C. `FFXIVItalian.DiffTool`
Strumento per la gestione e l'aggiornamento incrementale delle patch:
- **Comando `diff`**: confronta due snapshot di patch (es. patch precedente e nuova patch), isolando:
  - **`UnchangedRows`**: righe identiche (la traduzione italiana approvata viene preservata senza modifiche).
  - **`NewRows`**: nuove quest, oggetti e dialoghi introdotti dalla patch.
  - **`ModifiedRows`**: battute ritoccate da Square Enix (con evidenziazione del delta inglese).
  - **`RemovedRows`**: righe obsolete.
- **Comando `init-glossary`**: compila il file Markdown canonico `07_Glossary.md` in formato JSON indicizzato per il linter.

### D. `FFXIVItalian.Patcher`
- Genera il pacchetto mod `.pmp` compatibile al 100% con **Penumbra** (plugin di Dalamud).
- Produce i file `meta.json` e `default_mod.json` con la mappatura dei file reindirizzati.
- Il file `.pmp` risultante può essere installato con un solo click in Penumbra senza toccare l'installazione originale del gioco.

---

## 3. Gestione di Nuove Patch di Gioco (Procedura Operativa)

Quando Square Enix rilascia un nuovo aggiornamento di FFXIV (es. 7.1 o 7.2):

1. **Generare il nuovo Snapshot**:
   ```powershell
   dotnet run --project src/FFXIVItalian.Extractor -- --snapshot-out data/snapshots/snapshot_new.json
   ```
2. **Eseguire il Diff contro lo snapshot precedente**:
   ```powershell
   dotnet run --project src/FFXIVItalian.DiffTool -- diff --old data/snapshots/snapshot_7_0.json --new data/snapshots/snapshot_new.json --out patch_diff_report.md
   ```
3. **Esaminare `patch_diff_report.md`**:
   Il report mostrerà esattamente quante righe sono nuove o modificate.
4. **Aggiornare il pacchetto `.pmp`**:
   Le righe storiche già tradotte rimangono in italiano; le nuove righe della patch rimangono in inglese finché non vengono tradotte, garantendo la totale assenza di crash (*Best Effort*).
