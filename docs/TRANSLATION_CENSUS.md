# Censimento delle correzioni editoriali

Il confronto delle modifiche non committate con `HEAD`, eseguito il 13 ottobre 2026, ha rilevato 318 file tracciati modificati o eliminati e 52.166 campi di traduzione cambiati. Nei JSON confrontabili sono cambiati soltanto campi `translation` e `translation_*`: nessun campo sorgente o metadato. Il censimento copre tutte le modifiche iniziali, compresi codice, test, strumenti e documentazione; il file non tracciato `-` è una coda terminologica già presente, non una fonte di decisioni editoriali.

I conteggi sotto indicano segmenti sostituiti nei campi confrontati con un diff per parole, normalizzando gli apostrofi per il confronto. Sono indizi di ricorrenza, non conteggi di tutte le occorrenze di un termine. Maiuscole, punteggiatura e bytecode non vanno scambiati per nuove decisioni terminologiche.

| Correzione osservata | Segmenti | Indicazione riutilizzabile |
| --- | ---: | --- |
| Attacco → attacco | 598 | Maiuscole secondo il campo sorgente, senza trasformare la prosa in titoli. |
| Inserimento di «paio di» | 495 | Nomi di occhiali: verifica singolare/plurale e descrizione; non applicare a ogni oggetto. |
| essere un Discepolo → usare una classe dei Discepoli | 204 | Requisiti di classe rivolti al giocatore: conserva l'appartenenza alla categoria senza imporre un appellativo maschile. |
| Mistwake → della Scia delle Nebbie | 186 | Forma già richiesta dall'utente; preposizioni e genere seguono Scia. |
| primordiale → primal | 146 | Nome dell'entità distinto dall'aggettivo comune; forma invariabile del glossario. |
| Armi → Stemma del | 138 | Arms araldico distinto dalle armi. |
| Tonnellate → tonze | 107 | Unità eorzeane, femminile plurale; distinguere usi letterali e idiomatici. |
| del Serpente → della Vipera | 72 | Forma breve della fazione nei gradi di Gridania. |
| yalm → yalmi | 68 | Plurale eorzeano, senza convertire in metri. |
| difesa → DIF | 59 | Sigla della statistica nei campi tecnici dei minion. |
| EXP → ESP | 57 | Sigla italiana dell'esperienza; non cambiare identificatori sorgente. |
| scala con il → varia in base al | 52 | Evitare il calco per la dipendenza numerica. |
| beastmaster → domatore | 40 | Forma del Job già nel glossario; rispettare le maiuscole sorgente. |
| portata → soglia | 36 | In Furtività il range riguarda livelli di nemici, non distanza. |
| cumuli → accumuli | 36 | Applicazioni cumulative di effetti, non pile fisiche. |
| party → gruppo | 28 | Tradurre la prosa preservando /party. |
| abilità con armi → tecniche d'arma | 27 | Distinguere le categorie di azione. |
| abilità istintuali → azioni istintive | 26 | Terminologia specifica dei famigli. |
| waymark → marcatore tattico | 25 | Tradurre la spiegazione preservando /waymark e /wmark. |
| fonosfera → fonoperla | 19 | Coerenza con il nome della funzione, anche nelle spiegazioni dei comandi. |

Altre famiglie importanti sono l'aggiunta di If(gnum4), gli accordi fra nome e descrizione, la correzione di traduzioni letterali di attacchi e la distinzione fra istruzioni al singolo e annunci collettivi. Le numerose modifiche ai nomi di azioni e ai riferimenti narrativi non autorizzano equivalenze generiche: per esempio Swing non diventa sempre Fendente Circolare, né Swipe sempre Spazzata. Per ogni nuova resa di lore consulta la pagina pertinente della wiki e registra l'ambito attestato.

Il glossario conserva le scelte esplicite già presenti e aggiunge distinzioni tecniche attestate nei file canonici, ciascuna con fonte e limite d'uso. La wiki documenta il contesto di gioco, non approva la resa italiana: per esempio [Lost Actions](https://ffxiv.consolegameswiki.com/wiki/Lost_Actions) distingue categorie di azione e applicazioni cumulative degli effetti.

## Come ripetere il censimento

1. Leggi `git status` e `git diff`; conserva le modifiche preesistenti e dichiara il riferimento usato per il confronto.
2. Confronta i JSON per ID e campo. Separa terminologia, sintassi, maiuscole, punteggiatura e payload SeString; verifica anche modifiche a sorgenti, struttura e metadati.
3. Ordina le sostituzioni ricorrenti, poi apri le voci complete con il campo sorgente corrispondente. Un segmento frequente può appartenere a una formula ripetuta e non essere una regola generale.
4. Cerca la voce nel glossario: amplia la nota quando basta; aggiungi una voce soltanto per una distinzione riutilizzabile. Metti una sola fonte sorgente nella colonna Fonte, ulteriori attestazioni nella colonna Uso. Usa `@review/` per i precedenti in revisione.
5. Aggiorna AGENTS, workflow, guida e prompt solo per istruzioni operative emerse dal censimento. Non duplicare l'intero glossario nei prompt.
6. Esegui validatore tecnico e scansione terminologica sul perimetro dichiarato; esamina manualmente ogni segnalazione. Correggi la voce errata oppure la regola responsabile del falso positivo, con una verifica mirata, e ripeti fino a zero problemi. Non cambiare stati editoriali per superare un controllo.
