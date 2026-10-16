# Obiettivo della versione 1.0

La versione 1.0 punta a coprire interfaccia, sistemi, oggetti, combattimento, attività, artigianato, alloggi, negozi e contenuti condivisi. Missioni, titoli e diari delle missioni, dialoghi e cutscene hanno un avanzamento distinto; i dialoghi già approvati restano nel pacchetto.

Le cutscene sono contenuti narrativi, ma non coincidono con i fogli delle quest: possono contenere anche battute di combattimento e contenuti collaterali. Per questo hanno una barra dedicata. Le cartelle numeriche dei Voiceman non vengono equiparate alle cartelle numeriche delle quest né usate per assegnare automaticamente un'espansione.

## Situazione del corpus al 2026-10-16

| Perimetro | Righe nel corpus | Coperte nei fogli approvati | Coperte in tutti gli stati |
| --- | ---: | ---: | ---: |
| Interfaccia e contenuti condivisi | 263.901 | 263.901 | 263.901 |
| Dialoghi e testi collegati | 75.034 | 75.034 | 75.034 |
| Cutscene e battute doppiate | 27.141 | 0 | 374 |
| Titoli, diari e cataloghi delle missioni | 17.862 | 89 | 89 |

Le righe coperte hanno compilati i campi testuali previsti; le colonne tecniche riconosciute (chiavi di configurazione, riferimenti a risorse ed effetti sonori) e i segnaposto verificati non richiedono una traduzione. Le regole sono in `BatchManager.IsTechnicalSourceField` e sono circoscritte ai fogli e ai campi verificati. Un payload SeString con testo visibile resta da tradurre anche se il testo esterno è vuoto.

## Cosa manca prima della 1.0

Il censimento seguente segnala righe da completare **o verificare**, non altrettante frasi certamente da tradurre. Sono compresi casi invarianti ancora da classificare, formule del registro, intervalli numerici e righe vuote. Non vanno riempiti con traduzioni artificiali per raggiungere il 100%.

| Foglio | Righe da completare o verificare | Stato editoriale |
| --- | ---: | --- |
| *Nessun foglio pendente* | 0 | completato |

Nei fogli elencati, dare priorità al testo visibile nei payload di `gatheringcondition`, `gatheringpointbonustype`, `hwdinfoboardarticle` e `configkey`. I testi dei pulsanti del controller e le alternative condizionali richiedono controllo di tutti i rami, non un'esclusione statistica.

Questo grafico misura il corpus estratto, non tutti i fogli presenti nel client né le texture. Per la chiusura della versione servono anche il confronto con il catalogo del client, la verifica delle risorse grafiche, la build con controllo di copertura dei campi approvati e la verifica in gioco. Un conteggio completo non approva un file e non sostituisce la revisione editoriale.

Snapshot e grafico sono rigenerati con `python tools/update_readme_progress.py`. Nessun file viene spostato o approvato da questo comando.
