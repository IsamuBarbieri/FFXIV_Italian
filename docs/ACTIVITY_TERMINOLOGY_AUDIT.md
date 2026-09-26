# Revisione delle categorie di attività

Il comando `dotnet run --project src/FFXIVItalian.Extractor -- validate --review` esamina tutte le 5.568 traduzioni JSON e controlla i campi testuali di ogni file completo. Dopo le correzioni risultano **24 file completi**. Le etichette e i riferimenti alle attività in questi file usano Missione, Incarico, Mandato, Prova, Incursione, Spedizione, Cripta Profonda, Operazione di Gilda e FATE secondo il glossario.

I sette avvisi rimasti riguardano altri significati o testo intenzionalmente mantenuto. Ogni riga riporta `file#ID:campo`, originale, traduzione, categoria apparente e decisione. Nei campi lunghi sono mostrati i frammenti pertinenti; il comando sopra stampa anche l'origine e la traduzione del campo.

| Campo | Originale | Traduzione | Categoria apparente | Decisione |
| --- | --- | --- | --- | --- |
| `combat/status.json#161:translation_name` | Thunder | Thunder | Meteo | Nome dell'effetto di combattimento; non applicare «Tuoni», che è il meteo. |
| `combat/status.json#2452:translation_name` | Return | Ritorno | Comando | Nome dell'effetto; non applicare le forme dei comandi di interfaccia. |
| `system/textcommand.json#153:translation_col_2` | `"The Black Shroud"`, stato `duty` | `"The Black Shroud"`, stato `duty` | Esempi di sintassi | Parametri di ricerca inseriti dall'utente; mantenerli validi. Due avvisi sullo stesso campo. |
| `system/textcommand.json#229:translation_col_2` | `/alarm Raid ...` | `/alarm Raid ...` | Incursione | «Raid» è il nome arbitrario di esempio della sveglia. |
| `system/textcommand.json#273:translation_col_2` | `“Duty Action I”` | `“Duty Action I”` | Incarico | Nome di un'azione nell'esempio del comando. |
| `world/achievement.json#3075:translation_description` | `“A Satrap's Duty”` | `“A Satrap's Duty”` | Incarico | Titolo proprio della missione citata; non è un nome di categoria. |

I nomi degli obiettivi e dei luoghi sono controllati per completezza ma esclusi dalle correzioni automatiche delle categorie. Il filtro di completezza richiede una traduzione per ogni campo visibile non vuoto; ignora i tag e gli identificatori tecnici di CustomTalk.
