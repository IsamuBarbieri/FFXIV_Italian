# Censimento dei testi ancora in inglese

Analisi del client locale e dello screenshot ricevuto il 27 settembre 2026. Il catalogo completo dei fogli master testuali ancora non registrati è in `UNEXTRACTED_SHEETS_CATALOG.txt`; si rigenera con `dotnet run --project src/FFXIVItalian.Extractor -- discover`. Il comando `search <testo>` ora cerca in tutte le pagine e colonne testuali dei fogli master, compresi quelli non registrati.

## Risultato del censimento

- `root.exl`: 7.120 fogli testuali multilingua, compresi quest e dialoghi specifici.
- Fogli master testuali non registrati dopo questa analisi: 358. La lista contiene il nome e il numero di colonne testuali di ciascun foglio; la presenza di una colonna testuale non implica che ogni riga contenga una frase da tradurre.
- Nuovi fogli estratti e registrati: `AddonTransient` (595 righe), `ClassJobActionUICategory` (21), `ClassJobCategory` (188), `ItemSearchCategory` (92), `ItemSeries` (30), `ItemSpecialBonus` (8), `Description` (28), `DescriptionString` (1.646). I JSON sono in `data/translations/misc/`; tutti i campi testuali non legali sono tradotti e il patcher ora include questi otto fogli. `python tools/validate_new_sheets.py` verifica che non restino campi pendenti e che i tag EXD siano preservati. La licenza Lua e gli avvisi di licenza sulle esibizioni in `AddonTransient` (173, 247, 1016) restano originali per richiesta dell'utente.
- `DescriptionString` 247 contiene già nel client inglese frammenti mancanti indicati da `[...]`; la traduzione conserva queste lacune senza inventare testo.
- Il comando `status` del 27 settembre 2026 conta 437.561 righe nel corpus, 51.197 con almeno una traduzione e 386.364 pendenti. La percentuale per riga non equivale alla copertura dei singoli campi. Le otto nuove tabelle sono complete, ma il resto del gioco rimane ampiamente in inglese.

## Testi dello screenshot

| Testo visibile | Fonte accertata | Stato / azione necessaria |
| --- | --- | --- |
| `Ark Angel's Cuirass of Maiming` | `Item`, riga 44520, colonna stringa offset 12 | Già estratto in `items/item.json`; `translation_col_3` vuoto. Il patcher non include alcuna pagina `Item`. |
| `Paladin`, `Dragoon`, altri job | `ClassJob`, per esempio riga 19, offset 16 | Traduzioni presenti in `world/classjob.json`; il `.pmp` e la cartella Penumbra attiva contengono `Paladino`. Il motivo per cui lo screenshot mostra l'inglese richiede verifica in gioco del mod caricato e dell'eventuale cache. |
| `Strength`, `Critical Hit`, `Direct Hit Rate` | `BaseParam` e alcune righe `Addon` | Traduzioni già presenti nei JSON e patch previste dal patcher. Anche qui lo screenshot non prova che manchi l'estrazione. |
| `Item Level`, `Repair Level`, `Advanced Melding Forbidden` | `Addon` | Traduzioni già presenti nel JSON. Lo screenshot mostra alcune etichette italiane, ma non necessariamente la versione attuale delle traduzioni. |
| `Disciple of the Hand` | `ClassJobCategory`, riga 33; anche `Addon` e altri fogli | Il nuovo `ClassJobCategory` è estratto, tradotto e collegato al patcher. La voce composita del tooltip va verificata in gioco dopo la patch. |
| `Market Prohibited`, `Hide item details` | Nessuna corrispondenza esatta nei fogli master EXD inglesi | Probabile testo generato dal client, da un altro tipo di risorsa o composto a runtime. La ricerca EXD da sola non consente di attribuirne la fonte. |

## Priorità di lavoro

1. Completare le traduzioni dei fogli già estratti: `Item` (50.880 righe, una con almeno un campo tradotto), `Action` (45.518, sei), `Balloon` (10.457, zero), `DefaultTalk` (11.584, zero), `ContentFinderCondition` (863, zero) e `ContentFinderConditionTransient` (620, zero). Questi conteggi sono per righe con almeno un campo tradotto; non misurano i campi ancora vuoti nelle righe parziali.
2. Per estendere il censimento, selezionare dal catalogo i fogli master mancanti con testo visibile e poi estrarli. Il catalogo contiene ancora 358 fogli master testuali non registrati; quest e dialoghi specifici sono censiti separatamente.
3. Collegare al patcher i fogli tradotti che oggi non vengono distribuiti, soprattutto le 106 pagine EXD di `Item`. Gli otto nuovi fogli sono collegati, ma il pacchetto aggiornato non è ancora stato generato né verificato in gioco.
4. Riprovare lo stesso tooltip con il `.pmp` aggiornato e la mod effettivamente attiva. La presenza di `Paladino` nel pacchetto, a fronte di `Paladin` nello screenshot, indica che il solo censimento dei fogli non spiega tutto.

Il catalogo non equivale a un elenco di ogni frase inglese del gioco: per farlo occorre estrarre anche i 358 fogli master rimanenti e censire le risorse non EXD. È però una mappa verificabile dei fogli mancanti e delle fonti delle stringhe osservate.
