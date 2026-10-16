# Istruzioni per gli agenti AI

- Prima di modificare, controlla `git status` e `git diff` e conserva il lavoro presente.
- Per nuove traduzioni leggi [Traduzione](docs/TRANSLATION.md); per revisioni [Revisione](docs/REVISION.md); dopo una patch [Aggiornamento del client](docs/CLIENT_UPDATE.md). Le regole comuni stanno nella guida di traduzione; non duplicarle negli altri documenti.
- Per dialoghi e testi attribuiti a un personaggio consulta [Voci dei personaggi](docs/CHARACTER_VOICES.md) e soltanto le schede dei parlanti e delle fasi pertinenti; applica il metodo e i limiti della guida di traduzione.
- `data/translations/` è la memoria ufficiale. [Glossary.md](data/glossary/Glossary.md) conserva decisioni, varianti ed eccezioni; le istruzioni esplicite dell'utente prevalgono. I file in revisione sono precedenti contestuali.
- Cerca precedenti mirati e verifica sorgente, traduzione, campo e contesto. Per discrepanze reali chiedi quale forma mantenere con riferimenti `percorso#ID:campo`; dopo la scelta uniforma tutte le occorrenze pertinenti, comprese quelle approvate, e la nuova traduzione. Prosegui le voci indipendenti mentre attendi; non sostituire meccanicamente differenze contestualmente valide.
- Lavora voce per voce sui campi assegnati. Preserva ID, struttura JSON, sorgenti e metadati; non riformattare interi JSON. Esamina sempre i SeString, inclusi i payload: traduci il testo visibile al loro interno preservando controlli e referenti, secondo [JSON e SeString](docs/TRANSLATION.md#4-json-e-sestring). Le sole estensioni del perimetro sono quelle richieste dall'utente o pertinenti alla riconciliazione confermata.
- Non spostare file fra gli stati editoriali e non dichiararli approvati senza conferma dell'utente. Non eseguire rebuild o deploy durante una semplice traduzione/revisione.
- Dopo modifiche alle traduzioni esegui i controlli pertinenti e la revisione linguistica; per uniformazioni applica il censimento e il confronto prima/dopo della [riconciliazione](docs/TRANSLATION.md#2-corpus-decisioni-e-discrepanze). Per codice esegui i test interessati. Correggi i falsi positivi dei controlli, non traduzioni valide per azzerare un report.
- Per censimenti confronta con `HEAD` per ID e campo; separa terminologia, grammatica, maiuscole, tipografia e SeString e conta i campi, non le righe del diff.
- Applica il feedback e registra nel glossario solo decisioni riutilizzabili con fonte sorgente o «Decisione dell'utente» e nota d'uso. Non duplicare il corpus nelle tabelle.
- Mantieni i confini Core, Extractor, DiffTool e Patcher. Non versionare dump o asset originali del gioco.

Comandi, dipendenze, batch e texture: [Architettura e manutenzione](docs/ARCHITECTURE.md). Per lo stato editoriale e le eccezioni su crediti, musica, misure e genere consulta le guide operative.
