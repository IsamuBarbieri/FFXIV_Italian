# Protocollo Operativo di Traduzione con Agente AI (Antigravity)

Questo protocollo stabilisce la procedura standard per le sessioni di traduzione assistita da agente AI su **FFXIV Italiano**, ottimizzata per **ridurre al minimo il consumo di token** (input, output e reasoning) ed eliminare il rischio di corruzione dei tag SeString.

---

## 1. Regola Ferrea Anti-Token-Bloat per l'Agente

Nelle future sessioni di chat, l'agente DEVE seguire queste direttive vincolanti:
1. **Zero Traduzioni in Chat**: Non stampare MAI nella finestra di chat elenchi di stringhe tradotte, tabelle comparative, frammenti JSON di anteprima o spiegazioni linguistiche prolisse (salvo richiesta esplicita dell'utente).
2. **Scrittura Diretta su Disco**: Tutte le traduzioni devono essere scritte direttamente nei file su disco o nei file batch dedicati.
3. **Risposta di Conferma Monoriga**: Al completamento di ogni operazione, l'agente risponde unicamente con un messaggio secco:
   ```text
   Fatto: [N] righe tradotte in [percorso/file]
   ```
   *(Esempio: `Fatto: 38 righe tradotte in data/translations/quests/arr/000/ClsArc000_00021.json`)*.
4. **Riferimento di Contesto Condensato**: Quando viene fornito il prompt o istruito un subagente, fare riferimento unicamente a [docs/PROMPT_COMPACT.md](file:///docs/PROMPT_COMPACT.md).

---

## 2. Modalità Operative

### Modalità A: Singoli File Compatti (< 300 righe)
*Ideale per missioni (`data/translations/quests/`), dialoghi circoscritti o piccoli fogli UI (`error.json`, `howto.json`).*

1. **Lettura del File**:
   L'agente ispeziona il file target su disco.
2. **Modifica Diretta**:
   L'agente compila i campi `"translation"` direttamente nel file JSON, rispettando [docs/PROMPT_COMPACT.md](file:///docs/PROMPT_COMPACT.md) (tag SeString identici, abilità in inglese, no bilinguismo).
3. **Validazione e Ricompilazione**:
   Eseguire i test di regressione del progetto:
   ```powershell
   dotnet test
   .\rebuild.bat
   ```
4. **Conferma in Chat**:
   `Fatto: [N] righe tradotte in [file]`

---

### Modalità B: File Massivi (> 300 righe, fino a 15.000+)
*Ideale per fogli di grandi dimensioni come `data/translations/system/addon.json`, `logmessage.json`, `combat/action.json`.*

Sono disponibili due percorsi operativi:

#### Percorso B1: Architettura Parallela con Subagenti & Script Python (Consigliata)
Permette la traduzione ad alta velocità di centinaia o migliaia di righe con validazione automatica 1:1 prima della scrittura nel master.

1. **Partizionamento in Batch Compatti**:
   ```powershell
   python scripts/split_untranslated.py data/translations/system/addon.json --batch-size 500 --out-dir scratch
   ```
   *Genera i file `scratch/input_batch_A.json`, `input_batch_B.json`, ecc. contenenti solo il dizionario compatto `{"id": "original"}`.*

2. **Traduzione Parallela (Agente Principale + Subagenti)**:
   - Invocare i subagenti tramite `invoke_subagent` per i lotti successivi (es. Batch B, C, D...) fornendo il contesto da [docs/PROMPT_COMPACT.md](file:///docs/PROMPT_COMPACT.md).
   - L'agente principale traduce il Batch A in parallelo.
   - Ogni subagente legge `input_batch_X.json` e scrive direttamente `scratch/output_batch_X.json` come dizionario `{"id": "traduzione"}`.

3. **Validazione SeString 1:1 e Reintegrazione Atomica**:
   ```powershell
   python scripts/apply_all_translations.py data/translations/system/addon.json --batch-dir scratch
   ```
   *Lo script valida al 100% l'integrità dei tag `<hex:...>` prima di toccare il file master. Se un tag non corrisponde esattamente, l'operazione viene bloccata senza corruzioni.*

4. **Test e Ricompilazione Finale**:
   ```powershell
   dotnet test
   .\rebuild.bat
   ```

5. **Pulizia e Conferma**:
   ```powershell
   Remove-Item -Recurse -Force scratch
   ```
   Risposta in chat: `Fatto: [N] righe tradotte in data/translations/system/addon.json`

---

#### Percorso B2: Pipeline Nativa CLI (`FFXIVItalian.Extractor`)
*Utilizza il `BatchManager` integrato in C#.*

1. **Esportazione Batch**:
   ```powershell
   dotnet run --project src/FFXIVItalian.Extractor -- export-batch addon --limit 200
   ```
   *Crea il file in `data/batches/addon_batch_<timestamp>.json`.*

2. **Traduzione del File Batch**:
   L'agente compila i campi `"Translation"` nel file batch in `data/batches/`.

3. **Reimportazione nel Master**:
   ```powershell
   dotnet run --project src/FFXIVItalian.Extractor -- import-batch addon data/batches/addon_batch_<timestamp>.json
   ```

4. **Propagazione Duplicati**:
   ```powershell
   dotnet run --project src/FFXIVItalian.Extractor -- autofill
   ```

5. **Ricompilazione**:
   ```powershell
   .\rebuild.bat
   ```
   Risposta in chat: `Fatto: [N] righe tradotte in addon.json`

---

## 3. Checklist di Sicurezza Rapida

Prima di considerare concluso qualsiasi lotto di traduzione:
- [ ] **Tag SeString `<hex:...>`**: conteggio, ordine ed esadecimale identici all'originale.
- [ ] **Nessun Bilinguismo**: niente testo inglese tra parentesi nel client.
- [ ] **Abilità e Comandi Slash**: nomi abilità/magie (*Fire, Cure, Provoke*) e comandi slash (*`/gpose`*) categoricamente in inglese.
- [ ] **Glossario Fondamentale**: *La Tempesta*, *Figli della Settima Alba*, *Mal d'Etere*, unità (*yalm, fulm, ilm, ponze...*) invariabili al plurale.
- [ ] **Stile Epiceno**: frasi rivolte al giocatore prive di marcatura rigida di genere quando non condizionali.
- [ ] **Test di Integrità**: `dotnet test` eseguito con 0 errori.

