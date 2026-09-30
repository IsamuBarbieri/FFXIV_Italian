# Contribuire

Le modifiche possono riguardare traduzioni, glossario, documentazione, strumenti o codice.

## Traduzioni

1. Lavora sui file e sui campi concordati; conserva ID e contenuto originale.
2. Segui [il workflow di traduzione](docs/TRANSLATION_WORKFLOW.md), la [guida di stile](docs/STYLE_GUIDE.md) e il [glossario](data/glossary/Glossary.md).
3. Esegui la validazione automatica applicabile ai file modificati e controlla gli avvisi.
4. Mantieni le traduzioni in revisione finché il responsabile del progetto non le approva.
5. Aggiungi al glossario i termini ricorrenti approvati, con riferimento alla fonte e nota sul contesto.

## Codice

- Usa .NET 10 per la soluzione C#.
- Esegui `dotnet test` per modifiche al codice C#.
- I test Python si eseguono con `python -m unittest discover -s tests`.
- Descrivi nell'eventuale pull request i file o i comandi interessati e i controlli eseguiti.

Non includere dump, file estratti o altri asset originali di Final Fantasy XIV. Per i dettagli sull'uso dell'AI consulta [AI_USAGE.md](AI_USAGE.md).
