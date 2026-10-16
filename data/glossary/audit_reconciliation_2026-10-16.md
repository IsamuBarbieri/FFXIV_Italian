# Verifica delle riconciliazioni — 16 ottobre 2026

## Perimetro e metodo

Esaminate le 130 righe nuove o modificate di `Glossary.md` rispetto a `HEAD`, cercando sorgenti e vecchie rese in `data/translations/` e `data/da_revisionare/`. La scansione iniziale comprende 613.834 coppie sorgente/traduzione; ricerche successive coprono anche singolari, plurali, composti, campi tradotti senza sorgente valorizzata e testo leggibile nei payload. Ogni candidato va interpretato nel contesto: un'omissione della forma letterale non dimostra da sola un errore.

Questo è un controllo delle decisioni terminologiche e della loro applicazione, non una revisione linguistica integrale di ogni battuta dei due corpus. Il lavoro preesistente e le modifiche contemporanee sono stati conservati; nessun file è stato spostato o approvato.

## Interventi

Corretti **378 campi tradotti** e ripristinati **9 campi sorgente**, in **75 file JSON**. I conteggi riguardano i campi distinti di questa verifica, non tutto il diff preesistente.

- Completate riconciliazioni rimaste parziali, fra cui Incudine, Guardia Celeste, Arma di Diamante, ceruleo, Nube Oscura, Sentinelle del Bosco, Bravi di Cristallo, Costruttori Celesti e termini degli eventi stagionali. Inclusi i nomi degli oggetti, i ruoli dei PNG e le descrizioni plurali.
- Riparati sei accordi della precedente correzione di `primordial/primeval`: `activities/instancecontenttextdata.json#14123:translation`, `combat/action.json#15930,#42457,#42458,#42460:translation`, `world/bnpcname.json#11076:translation_name`.
- Ripristinati i sorgenti inglesi alterati nei payload: `@review/dialogue/custom/003/cmncntweeklybingo_00372.json#0-4,#52:description` e `@review/dialogue/custom/007/regsea5broenbhar_00769.json#5,#11,#12:description`. Le traduzioni italiane restano nei rispettivi campi tradotti.
- Riparati due payload malformati usando i controlli dei sorgenti e ricodificando le stringhe con `tools/translate_hex_literals.py`: `social/gcarmyexpedition.json#4:translation_description` e `system/logmessage.json#4744:translation`.
- Ripristinati i titoli musicali esclusi dalla traduzione in `housing/orchestrion.json#30,#82,#83,#364,#818,#888:translation_name` e i crediti in `system/creditlisttext.json#4660,#5688:translation`. Tre titoli e un credito erano stati coinvolti impropriamente nel cambio Primal → Primale.

I percorsi brevi dei JSON sono relativi a `data/translations/`; `@review/` indica `data/da_revisionare/`.

## Scelte confermate durante la verifica

Applicate le scelte dell'utente: Sabbie Orientali, Campi Dorati, Cisterna Crepata, Benedizione per il potenziamento delle Grandi Compagnie, protoeterite, Assistente di Blunderville, Muso Scarlatto, Drago Allegro e Figli di San Coinach. Il glossario registra le forme sostituite e il loro ambito.

Risolto anche il conflitto fra la nuova decisione per Wood Wailers e la precedente voce specifica Wood Wailer sentry: l'utente ha scelto **Guardia delle Sentinelle del Bosco**, plurale **Guardie delle Sentinelle del Bosco**. Uniformati i due PNG, i relativi plurali e la citazione attribuita a quel ruolo.

Non sono estensioni della scelta:

- `primal` aggettivo generico, `primordial/primeval`, titoli musicali, crediti e il gruppo The Primals;
- Sanction nei nomi degli attacchi omonimi, distinto dal potenziamento delle Grandi Compagnie;
- Westersands, non interessato dalla scelta per Estersands;
- ogni toponimo contenente Saint Coinach, distinto dall'organizzazione e dai suoi membri;
- parafrasi contestualmente valide, come «collega del Firmamento» per *my skybuilder friend* in `@review/dialogue/custom/006/ctshwdinformation_00633.json#84:translation_description`.

## Utilità del glossario

Non serve duplicare tutti i nomi del corpus. Restano utili le decisioni fra varianti precedentemente discordanti: dopo l'uniformazione, il solo corpus non mostra più quale alternativa sia stata scartata o dove la decisione cessi di valere.

Eliminate sette righe:

- il doppione di Seat of the First Bow, conservando una sola decisione nella tabella dei luoghi;
- Agave Jaws, Blunderville Square, The Finesand Banks, Battle of Silvertear Skies e Crystal War: semplici attestazioni già recuperabili dal corpus;
- star → stella: nota troppo generica per imporre una sola resa ai diversi usi cosmologici e narrativi.

Ridotti gli elenchi di attestazioni a riferimenti sorgente verificati, corretti i riferimenti inesistenti e normalizzate le fonti contestuali con `@review/`. Verificati oltre 220 riferimenti espliciti completi nelle righe nuove o modificate. Conservate le eccezioni tecniche ai trattini e le varianti contestuali degli arredi; documentati gli articoli validi dei toponimi.

## Prevenzione

`docs/TRANSLATION.md` ora richiede un censimento prima della modifica, valori iniziali per campo, ricerche delle vecchie rese, controllo dei rami SeString e confronto finale di sorgenti, metadati e struttura. Vieta sostituzioni globali per sottostringa e riparazioni mediante sostituzione inversa indiscriminata. Distingue la decisione registrata dal completamento della riconciliazione. `AGENTS.md` rimanda a questa procedura senza duplicarla.

## Verifiche

- JSON e confronto dei campi: nessuna variazione a ID, struttura, sorgenti o metadati rispetto a `HEAD` nei 75 file interessati, dopo il ripristino dei nove sorgenti.
- SeString modificati: 24 payload controllati, compreso un ulteriore plurale corretto contemporaneamente; i due inizialmente malformati risultano riparati e le altre modifiche conservano la firma dei controlli. Conservate anche le modifiche contemporanee a testo e plurali.
- Parser Python del glossario: caricamento riuscito.
- Validazione generale: esaminati 675.162 campi negli approvati e 61.810 nei file in revisione. Gli ultimi due avvisi degli approvati riguardavano la decisione conflittuale Wood Wailer sentry, poi risolta con l'utente e verificata sui tre file interessati. Restano i 35 falsi positivi contestuali elencati sotto nei file in revisione.
- Controllo aggiuntivo degli otto fogli UI: superato. Lo script originale si interrompe sul BOM preesistente di `system/descriptionstring.json`; la stessa verifica è stata eseguita in memoria con lettura `utf-8-sig`, senza modificare lo script o la codifica del corpus.
- `git diff --check` sul perimetro modificato: superato.

La validazione dei file in revisione segnala 35 falsi positivi contestuali: 31 identificatori tecnici contenenti NPC/AIRSHIP e quattro rese idiomatiche di misure, senza distanza o peso letterali. I quattro casi sono `@review/dialogue/custom/000/jobdefpld_00091.json#52:translation_description`, `@review/dialogue/custom/006/ctssfscharacter5_00640.json#60,#102:translation_description` e `@review/dialogue/custom/006/regroc2hilda_00648.json#27:translation_description`. Le traduzioni valide non sono state alterate per azzerare il report.
