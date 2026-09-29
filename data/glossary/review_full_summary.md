# Riepilogo — revisione dei file completi

## Stato dei file

Sono state controllate 33 schede inizialmente in `da_revisionare`: 31 risultano complete secondo la stessa regola di completezza del cruscotto (tutti i campi di traduzione presenti); `combat/action.json` e `items/item.json` erano incomplete e sono state spostate in `da_tradurre`.

## Controllo automatico e interventi

La scansione esaustiva delle 31 schede complete ha prodotto **2,520 segnalazioni da valutare** in 2,392 campi: 2,442 possibili divergenze terminologiche e 78 casi di maiuscole. 1,506 segnalazioni riguardano esclusivamente nomi provenienti da `world/placename.json`, dove il contesto decide se si tratta davvero del luogo o di un omonimo.

La scansione non ha più proposto correzioni grammaticali dopo la revisione delle regole. La passata precedente aveva segnalato «lo iaijutsu» due volte: entrambe le correzioni in «l'iaijutsu» sono state approvate e applicate dalla pipeline; i tag dinamici sono rimasti identici. I controlli di maiuscole ora rispettano gli acronimi e distinguono i nomi propri dalle parole comuni in minuscolo.

Le restanti proposte richiedono contesto: non le ho applicate automaticamente. La checklist raggruppa le segnalazioni per termine e riporta ogni campo con il testo inglese e italiano da valutare.

## Segnalazioni per file

- `da_revisionare/combat/actiontransient.json`: 151
- `da_revisionare/combat/status.json`: 64
- `da_revisionare/combat/trait.json`: 10
- `da_revisionare/combat/traittransient.json`: 27
- `da_revisionare/dialogue/customtalk.json`: 7
- `da_revisionare/system/addontransient.json`: 20
- `da_revisionare/system/baseparam.json`: 3
- `da_revisionare/system/description.json`: 5
- `da_revisionare/system/descriptionstring.json`: 232
- `da_revisionare/system/error.json`: 1
- `da_revisionare/system/logmessage.json`: 389
- `da_revisionare/system/textcommand.json`: 36
- `da_revisionare/world/achievement.json`: 1,226
- `da_revisionare/world/fate.json`: 274
- `da_revisionare/world/title.json`: 75

Report automatico finale: `review_full_final.json` (2,520 voci pendenti). Report con le due approvazioni: `review_full_applied.json`.
