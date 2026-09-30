# Istruzioni per gli agenti AI

## Prima di lavorare

- Per traduzioni, leggi [il workflow](docs/TRANSLATION_WORKFLOW.md), [la guida di stile](docs/STYLE_GUIDE.md), il [prompt tecnico](docs/TRANSLATION_PROMPT.md) e il glossario canonico in data/glossary/Glossary.md.
- Limita il lavoro ai file indicati dall'utente. Modifica file di allineamento solo quando sono indicati come riferimenti o destinazioni.
- Non riformattare interi file JSON per cambiare poche traduzioni.

## Traduzioni

- Traduci e controlla le voci una per una, tenendo conto del contesto, del registro del personaggio e delle decisioni del glossario.
- Conserva ID, struttura JSON, campi sorgente e metadati. Modifica solo i campi di traduzione richiesti.
- Preserva tag e codici SeString. Traduci il testo leggibile dentro i payload esadecimali solo seguendo le regole e gli strumenti esistenti.
- Dopo la traduzione esegui il validatore appropriato sul perimetro modificato. Il controllo automatico non sostituisce la revisione linguistica né l'approvazione dell'utente.
- Applica le correzioni dell'utente. Aggiungi al glossario le decisioni terminologiche ricorrenti che l'utente conferma, con riferimento alla riga e una nota d'uso.
- Non spostare file tra gli stati editoriali e non dichiarare un file approvato senza conferma dell'utente.

## Codice e dati

- Mantieni i confini tra Core, Extractor, DiffTool e Patcher.
- Non includere o versionare dump e asset originali del gioco; il progetto legge i dati dall'installazione locale e genera file per Penumbra.
- Per modifiche al codice usa i test esistenti pertinenti; per sole traduzioni usa i controlli di traduzione.

Per struttura, comandi e documentazione del progetto, consulta [README.md](README.md) e [l'indice dei documenti](docs/README.md).
