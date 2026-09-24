# Protocollo Operativo Diretto (In-Place & Sequenziale)

Workflow per sessioni di traduzione assistita con Antigravity.  
Ottimizzato per **zero overhead di token**, **nessun subagente**, **zero split/file temporanei**, e **salvataggio incrementale in-place** direttamente nel file JSON originale.

---

## 1. Principi Fondamentali

1. **Traduzione Sequenziale In-Place**:
   - Non si creano file batch temporanei né si usano script di split.
   - Si lavora direttamente sul file master (`data/translations/...`).
   - Si individuano le voci pendenti (campo `translation` vuoto `""` o non tradotto) in ordine numerico/sequenziale di ID.
2. **Salvataggio Progressivo & Ciclo Continuo ad Oltranza**:
   - L'agente traduce e scrive a blocchi consecutivi direttamente nel file JSON su disco.
   - NON si ferma al termine di un blocco: continua automaticamente con il successivo fino all'esaurimento completo delle voci pendenti del file.
   - In caso di esaurimento forzato dei crediti/contesto a metà esecuzione, **nessun progresso viene perso** perché ogni blocco è già persistito su disco.
3. **Nessun Subagente**:
   - L'agente esegue la traduzione direttamente nella sessione corrente con il contesto già attivo. Zero token sprecati per invocare, ri-istruire o sincronizzare subagenti.
4. **Zero Reasoning Prolisso / Zero Stampe in Chat**:
   - Pensare il minimo necessario (traduzione diretta e naturale).
   - Non stampare testo tradotto o tabelle in chat. Risposta solo di conferma a fine blocco/file:
     ```text
     Fatto: tradotte righe da ID [X] a [Y] in [percorso/file.json]
     ```
5. **Coerenza con le Traduzioni Consolidate (Una Tantum)**:
   - Solo prima di iniziare il primo blocco (all'avvio del file), ispezionare un piccolo campione di righe già tradotte (oppure il file di riferimento fornito dall'utente) per catturare le formule fisse ricorrenti. Una volta acquisite nel contesto, non rileggere all'indietro e applicare la stessa convenzione in avanti per tutto il file.
6. **Integrità SeString Immediata**:
   - Poiché il file contiene sia `"original"` che `"translation"`, i tag `<hex:...>`, variabili `<FullName>`, macro `<If(...)>` e tag di formattazione `<br>` si verificano e replicano 1:1 guardando la stringa `"original"`.

---

## 2. Flusso di Lavoro Sequenziale

1. **Lettura del Range**:
   - L'agente ispeziona il file per individuare il primo ID con traduzione mancante (`"translation": ""` o in inglese).
   - Legge un blocco di N elementi consecutivi.
2. **Aggiornamento In-Place**:
   - Compila i campi `translation` direttamente nel file JSON su disco.
3. **Avanzamento**:
   - Si prosegue con il blocco successivo fino al completamento o al limite desiderato.
4. **Verifica Finale**:
   - Esecuzione rapida dei test di regressione:
     ```powershell
     dotnet test
     .\rebuild.bat
     ```
