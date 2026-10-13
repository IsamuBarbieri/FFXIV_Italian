# Aggiornamenti del client

Dopo che il client ufficiale ha aggiornato il gioco, esegui dalla radice del progetto:

```powershell
dotnet run --project src/FFXIVItalian.Extractor -- update --integrate
```

Il percorso predefinito del client è quello in G. Per un'altra installazione aggiungi `--sqpack <percorso>`.

Il confronto legge i fogli inglesi elencati in `root.exl`, inclusi quelli con percorsi come `custom/000/...`, e cerca i JSON nei tre stati editoriali. Non modifica l'installazione del gioco. Per produrre soltanto il confronto ometti `--integrate`.

| Risultato | Integrazione e lavoro editoriale |
| --- | --- |
| Foglio assente dal corpus | File nuovo in `data/da_tradurre/`, nella categoria e nelle sottocartelle pertinenti. Segue il normale workflow di traduzione. |
| ID aggiunto a un file esistente | Riga inserita nello stesso file con traduzione vuota; le voci esistenti restano intatte. |
| Sorgente modificata | Nessuna sostituzione automatica nel corpus. `review.md` mostra vecchia sorgente, nuova sorgente e traduzione attuale, campo per campo. |
| ID rimosso dal client | Riga conservata nel corpus e segnalata nel rapporto. Nessuna cancellazione automatica. |
| Estrazione incompleta, schema incompatibile o altro errore | Integrazione sospesa e codice di uscita 1. Leggi gli errori nel rapporto prima di ripetere il comando. |

Ogni esecuzione crea `data/updates/<esecuzione>/`, esclusa da Git:

- `report.md`: fogli nuovi e ID aggiunti, modificati o rimossi nei file esistenti, per pianificare il lavoro.
- `report.json`: dettagli strutturati con le voci prima e dopo e i campi sorgente modificati.
- `review.md`: confronto leggibile delle sorgenti modificate con la traduzione da ricontrollare.
- `backup/`: copie precedenti dei file interessati.
- `updated/`: copie complete del possibile aggiornamento, che conservano le traduzioni precedenti anche quando la sorgente cambia. Non sono automaticamente approvate e non vanno copiate in blocco sul corpus.

Per una voce modificata, leggi entrambe le sorgenti e la traduzione nel contesto. Aggiorna nel file corrente il campo sorgente e correggi la traduzione solo se necessario; poi usa il validatore di traduzione. Non cambiare lo stato editoriale senza approvazione. Le differenze già risolte spariscono al confronto successivo; quelle ancora da controllare restano visibili, evitando di perderle dopo una seconda patch.

«Nuovo» significa assente dal corpus locale: non certifica che il foglio sia stato introdotto dall'ultima patch. I file lasciati in `da_tradurre/` sono già conosciuti e non vengono classificati di nuovo come nuovi.

Il confronto e l'integrazione non traducono testi, non pubblicano la mod e non spostano file esistenti tra gli stati editoriali. In caso di interruzione durante l'integrazione, ogni file è scritto attraverso una sostituzione atomica; alcune aggiunte potrebbero essere già applicate. I backup restano disponibili e un nuovo confronto rileva ciò che manca.
