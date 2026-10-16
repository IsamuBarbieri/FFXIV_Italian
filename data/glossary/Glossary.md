# Decisioni terminologiche ed eccezioni — FFXIV Italiano

`data/translations/` è la memoria ufficiale della localizzazione. Cerca lì le rese già approvate, confrontando sorgente, traduzione, campo e contesto; non duplicare qui ogni nome o etichetta. Questo registro conserva decisioni dell'utente, varianti contestuali, accordi ed eccezioni che la sola coppia sorgente/traduzione non spiega. Le istruzioni esplicite dell'utente prevalgono.

Per discrepanze reali dello stesso referente, significato e funzione, presenta varianti e riferimenti all'utente. Dopo la scelta uniforma tutte le occorrenze pertinenti nel corpus approvato e in revisione, oltre alla nuova traduzione, con accordi e controlli specifici. Differenze motivate dal contesto restano valide. Procedura: [Traduzione](../../docs/TRANSLATION.md#2-corpus-decisioni-e-discrepanze).

Lo stato editoriale dipende dal percorso: `@review/` è una fonte contestuale e non approva il file. I termini inglesi sono riferimenti e non vanno aggiunti fra parentesi al testo di gioco. Il catalogo integrale dei luoghi è `data/translations/world/placename.json`, letto direttamente dai controlli; le voci Luoghi qui presenti documentano decisioni e varianti.

Nella colonna **Uso**, `Controllo automatico: residuo.` segnala il termine inglese se resta nella traduzione di una frase; `Controllo automatico: residuo plurale.` controlla anche il plurale inglese in `-s`; `Controllo automatico: suffisso equipaggiamento.` applica il controllo ai termini di materiali e provenienza in `items/item.json`; `Controllo automatico: suffisso ruolo equipaggiamento.` verifica i suffissi di ruolo solo alla fine dei nomi di equipaggiamento; `Controllo automatico: intercalare.` controlla l'intercalare parlato preservando composti come «noce kupo». Questi controlli generano candidati da verificare: il validatore non modifica i file. La forma eorzeana delle misure è verificata in base al singolare o al plurale presente nell'originale.

## Licenze e avvisi nella UI

**Decisione dell'utente:** le informazioni e gli avvisi di licenza mostrati nelle finestre di gioco vanno tradotti. Questo include le informazioni sulle esibizioni in `system/addontransient.json#247:original` e `#1016:original`. Le licenze generali consultabili dalla lobby restano in inglese; il testo dei software di terze parti in `system/addontransient.json#173:original` mantiene l'originale. Distingui la funzione del testo, non applicare un'esclusione a tutte le voci che contengono «license» o all'intero foglio.

## Concordanza di genere del personaggio giocante

**Decisione dell'utente:** gli appellativi e gli accordi riferiti al giocatore si adattano al genere del personaggio mediante SeString. Le rese neutre naturali sono ammesse quando conservano significato e registro; non eliminare titoli o snaturare la frase per evitare la condizione.

- Coppie d'uso: **avventuriero / avventuriera**, **Guerriero della Luce / Guerriera della Luce**, **eroe / eroina**, **amico / amica**, **forestiero / forestiera**, **signore / signora**. La barra descrive le due forme e non va mostrata nel gioco. «Gilda degli Avventurieri» resta il nome dell'organizzazione; i plurali collettivi non dipendono dal genere locale.
- Nel JSON usa **If(gnum4)** compilato: condizione `E9 05`, ramo **femminile**, ramo **maschile**. Esempio «avventuriera» / «avventuriero»: `<hex:02081FE905FF0D617676656E74757269657261FF0D617676656E7475726965726F03>`. Fonte tecnica contestuale: `dialogue/contenttalk.json#112:translation`. Le lunghezze contano i byte UTF-8.
- Accorda articoli, preposizioni, pronomi, aggettivi e participi in entrambi i rami. Preferisci il segmento completo («un'avventuriera esperta» / «un avventuriero esperto»). I possessivi concordano con ciò che è posseduto, non con il possessore.
- Non applicare il genere locale a PNG o terzi, né a etichette generiche come «Scheda dell’Avventuriero» o «Nuovo Avventuriero». Il nome di una categoria non indica il genere di chi la consulta. Preserva i referenti delle condizioni native: IfPcGender (`0B`) usa l'ObjectId indicato e ordina i rami **maschile, femminile**. Nomi di oggetti, titoli musicali, citazioni rivolte a un PNG identificato e collettivi richiedono il proprio contesto.
- Regole e verifica dei due risultati: [Traduzione](../../docs/TRANSLATION.md#4-json-e-sestring). La pseudomacro `<If(PlayerParameter(Gender)...)>` non è compilata dall'encoder.

## Voci

### Interfaccia e comandi

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| NPC | PNG | Decisione dell'utente | Controllo automatico: residuo plurale. Sigla di personaggio non giocante, maschile invariabile al singolare e al plurale; anche NPCs diventa PNG. Usare «il PNG», «i PNG», «del PNG», «dei PNG», «al PNG», «ai PNG», «dal PNG», «dai PNG», «nel PNG», «nei PNG», «sul PNG», «sui PNG», «un PNG». Conservare la sigla maiuscola anche in prosa. Non modificare comandi, identificatori tecnici o campi sorgente. Riferimenti: `shops/fittingshopcategory.json#52:translation`. |
| Party | Gruppo | `system/addon.json#176:original` |  Singolare «gruppo», plurale «gruppi»; evita il residuo «party» nella prosa. Conserva /party come comando e Party nei soli identificatori tecnici. |
| Delete | Elimina | `system/addon.json#68:original` | Varianti ammesse in prosa: «Eliminare». |
| Item Level | Livello Oggetto | Decisione dell'utente | Controllo automatico: residuo. Nome della statistica di potenza degli oggetti. Nelle etichette di menu, shop e tabelle con vincoli di spazio usare «Liv.Ogg.» (variante ammessa; Varianti ammesse in etichette: «Liv.Ogg.»). |
| IL | Liv.Ogg. | Decisione dell'utente | Abbreviazione canonica di Item Level (Livello Oggetto). Rispecchia il francese «NvObj» e il tedesco «G.-St.». Varianti ammesse in prosa: «Liv.Ogg.». |
| Rank | Grado | `system/addon.json#732:original` | Nel Mahjong indica la posizione in classifica; varianti ammesse in prosa: «Posizione». |
| Crafting | Fabbricazione | `system/addon.json#273:original` | Nome dell'attività di fabbricazione; distinto dalle altre etichette contestuali del lemma. |
| Gathering | Raccolta | `system/addon.json#276:original` | Varianti ammesse in prosa per l'attività in corso: «Raccogliendo». |
| Battle | Battaglia | `system/addon.json#663:original` | Battaglia nella UI; distinta dalla categoria tutorial «Combattimento». |
| Main Scenario Quests | Missioni dello Scenario Principale | `system/howto.json#80:original` | Categoria delle missioni principali. |
| Return | Ritorna | `system/addon.json#1460:original` | Comando di ritorno. |
| Return | Indietro | `system/lobby.json#507:original` | Navigazione alla schermata precedente. |
| Return | Rientro | `system/maincommand.json#36:name` | Etichetta nominale contestuale. |
| Return | Ritorno | `combat/status.json#2452:name` | Nome dello status; distinto dai comandi di ritorno. |
| Exit | Esci | `system/addon.json#2849:original` | Azione. |
| Phrontistery / Phrontisterion | Phrontisterio | Decisione dell'utente | Nome dell'istituzione di Frondale; mantenere «Phrontisterio» anche nei nomi dei ruoli e nelle varianti inglesi Phrontistery/Phrontisterion. Riferimenti: `world/enpcresident.json#1017813:name`; `world/enpcresident.json#1033006:name`. |
| Exit | Uscita | `world/placename.json#528:name` | Etichetta nominale. |
| Duty | Incarichi | `system/maincommandcategory.json#2:original` | Categoria del menu; plurale di Incarico. |
| Duty | Incarico | `system/addon.json#2225:original` | Attività singola. Distinta dalle missioni delle quest. |
| Saga of Dawn and Duty | Saga dell'Alba e del Dovere | `combat/action.json#37193:original` | Nome di un attacco; «duty» è dovere, non Incarico. |
| Duties | Incarichi | `system/addon.json#15793:original` | Plurale del termine di gioco. |
| Duty Finder | Ricerca Incarichi | `system/maincommand.json#33:name` | Nome della funzione. Varianti ammesse in prosa: «Ricerca degli Incarichi», «Incarico trovato» nei messaggi di abbinamento. |
| Duty Recorder | Registratore Incarichi | `system/maincommand.json#76:name` | Nome della funzione. |
| Hotbar | Barra Azioni | `system/addon.json#1305:original` | Nome dell'interfaccia; in prosa adattare articolo e numero. |
| Duty Support | Supporto Incarichi | `system/maincommand.json#91:name` | Nome della funzione. |
| Duty Roulette | Roulette Incarichi | `system/addon.json#8605:original` | Nome della funzione. |
| Party Finder | Ricerca Gruppo | `system/maincommand.json#57:name` | Nome della funzione di ricerca gruppi. |
| Hall of the Novice | Sala dei Novizi | `system/maincommand.json#70:name` | Nome della funzione. |
| Novice Network | Rete dei Novizi | Decisione dell'utente | Nome del canale/funzione; usare con gli articoli e le preposizioni richiesti dalla frase. Riferimenti: `system/addon.json#651:original`; `system/addon.json#4508:original`. |
| Returner | Ritornato | Decisione dell'utente | Nome dello status/categoria; al plurale «Ritornati» e, se la frase richiede l’accordo femminile, «Ritornata». Lasciare invariati i token tecnici dei comandi e della ricerca. Riferimenti: `social/onlinestatus.json#31:original`; `system/textcommandparam.json#424:original`. |
| Challenge Log | Registro Sfide | `system/maincommand.json#60:name` | Nome del registro; forma abbreviata senza articolo. Maiuscole fisse: «Registro Sfide».  Varianti ammesse in prosa: «Registro delle Sfide». |
| Challenge Log Rewards | Ricompense Registro Sfide | `system/addon.json#10116:original` | Etichetta abbreviata, senza articolo. |
| Heaven-on-High | Pilastro dei Cieli | `system/addon.json#4809:original` | Nome del Deep Dungeon. Decisione dell'utente: nelle occorrenze di `Heaven<hex:021F0103>on<hex:021F0103>High` in `data/da_revisionare/`, usare «Pilastro dei Cieli» eliminando esclusivamente i due tag del trattino `<hex:021F0103>` interni al nome. Conservare tutti gli altri tag; l'eccezione non si estende ad altri nomi o ai file canonici. |
| Faux Hollows | Tane Sospette | `system/addon.json#11013:original` | Nome della funzione. |
| Sundry Splendors | Vari Splendori | `system/addon.json#13512:original` | Nome del negozio. |
| Mech Ops | Operazione Mech | `system/addon.json#16724:original` | Modalità dell'Esplorazione Cosmica, con piloti e supporto a terra; varianti ammesse in prosa: «operazione mech», «Operazioni Mech», «operazioni mech». |
| Crucible of the Unbroken | Crogiolo degli Infrangibili | `system/addon.json#17601:original` | Nome del contenuto. |
| Company Chest | Forziere della Compagnia | `system/addon.json#2880:original` | Nome della funzione della Compagnia Libera. Variante ammessa solo per vincoli di spazio della UI: «Forziere» in `system/addon.json#2963:translation`. |
| Rowena's House of Splendors | Casa degli Splendori di Rowena | `system/addon.json#6170:original` | Nome del negozio. |
| Hunting Log | Registro di Caccia | `system/maincommand.json#8:name` | Nome del registro; varianti ammesse in prosa: «registro di caccia», «voce del registro di caccia», «rango di difficoltà del registro di caccia». |
| Field Record | Rapporti sul Campo | `system/mcguffinuidata.json#3:original` | Nome della raccolta delle schede di Save the Queen; usare la forma canonica anche se l'originale è singolare. |
| Gathering Log | Registro di Raccolta | `system/maincommand.json#7:name` | Nome del registro; varianti ammesse in prosa: «taccuino di raccolta». |
| Crafting Log | Registro di Fabbricazione | `system/maincommand.json#9:name` | Nome del registro. |
| Fishing Log | Registro di Pesca | `system/maincommand.json#29:name` | Nome del registro; varianti ammesse in prosa: «taccuino di pesca», «registro di pesca». |
| Fellowships | Confraternite | `system/maincommand.json#85:name` | Nome della funzione; singolare: Confraternita. |
| Fellowship Finder | Ricerca Confraternite | `system/maincommand.json#86:name` | Nome della funzione. Varianti ammesse in prosa: «Ricerca dei Sodalizi». |
| Strategy Board | Lavagna Strategica | `system/maincommand.json#98:name` | Nome della funzione; varianti ammesse in prosa: «lavagna strategica», «lavagne strategiche», «Lavagne Strategiche», «Lavagna Tattica». |
| Unending Codex | Codex Infinito | `system/mcguffinuidata.json#5:original` | Nome della raccolta; mantenere la forma invariabile e adattare articoli e preposizioni alla sintassi italiana («nel Codex Infinito»). |
| The Source | La Sorgente | `minigames/tripletriadcard.json#387:col_2` | Variante del nome del mondo usata sistematicamente nei testi revisionati. |
| Amajina & Sons Mineral Concern | Impresa Mineraria Amajina e Figli | `world/contentfinderconditiontransient.json#3:original` | Varianti ammesse in prosa: «Compagnia Mineraria Amajina e Figli». |
| Far East | Estremo Oriente | `world/mkdlore.json#1:description` | Varianti ammesse in prosa: «Lontano Oriente». |
| Grand Company | Grande Compagnia | `dialogue/contenttalk.json#42:original` | Varianti ammesse in prosa: «Compagnia Maggiore». |
| The Churning Mists | Nebbie Vorticose | `world/contentfinderconditiontransient.json#37:original` | Nome dell'area; forma preferita dall'utente. |
| Sightseeing Log | Diario Esplorazione | `system/maincommand.json#64:name` | Nome del registro; maiuscole fisse: Diario Esplorazione. |
| Vault Oneiron | Volta Oneiron | `system/howto.json#289:original` | Nome del luogo. |
| Armoury Chest | Armeria | `system/maincommand.json#25:name` | Nome della funzione. |
| Chocobo Saddlebag | Bisaccia del Chocobo | `system/maincommand.json#77:name` | Nome della funzione. |
| Blue Magic Spellbook | Grimorio di Magia Blu | `system/maincommand.json#81:name` | Nome della funzione. |
| Accursed Hoard | Tesoro Maledetto | Decisione dell'utente | Nome del tesoro recuperabile nelle Cripte Profonde; usa «pezzo/frammento del Tesoro Maledetto» per *piece*, secondo il contesto. |
| Aether Currents | Correnti Eteriche | `system/maincommand.json#67:name` | Nome della funzione; varianti ammesse in prosa: «correnti eteriche», «corrente eterica». |
| Adventurer Plate | Scheda dell'Avventuriero | `system/maincommand.json#93:name` | Nome della funzione; varianti ammesse in prosa: «scheda dell'avventuriero», «schede degli avventurieri». |
| Shared FATE | FATE Condivisi | `system/maincommand.json#84:name` | Nome della funzione. |
| New Game+ | Nuova Partita+ | `system/maincommand.json#88:name` | Nome della modalità. |
| Waymarks | Marcatori Tattici | `system/maincommand.json#58:name` | Nome della funzione. Singolare in prosa: «marcatore tattico»; evita il residuo «waymark». Conserva /waymark, /wmark e i sottocomandi letterali nella sintassi dei comandi. Attestazione: `system/textcommand.json#178:translation_col_2`. |
| Ready Check | Appello | `system/maincommand.json#59:name` | Nome della funzione. |
| Record Ready Check | Registra Appello | `system/maincommand.json#79:name` | Funzione per registrare un appello prima di registrare un incarico. Varianti ammesse in prosa: «appello di registrazione», «appello per la registrazione», «verifica di prontezza alla registrazione». L'etichetta del comando mantiene le maiuscole fisse: «Registra Appello». |
| PvP Profile | Profilo PvP | `system/maincommand.json#56:name` | Nome della funzione. |
| PvP Team | Squadra PvP | `system/maincommand.json#78:name` | Nome della funzione; varianti ammesse in prosa: «una squadra PvP», «le squadre PvP», «delle squadre PvP», «squadre PvP». |
| Silence Echo | Silenzia Eco | `system/addon.json#12691:original` | Etichetta contestuale; distinta dal termine generico «Silence». |
| Starward Standings | Classifica Stellare | `system/addon.json#16716:original` | Etichetta della classifica; distinta dal nome dell'area «Spalti delle Stelle». |
| Mount Guide | Guida Cavalcature | `system/maincommand.json#61:name` | Nome della funzione. |
| Minion Guide | Guida Minion | `system/maincommand.json#62:name` | Nome della funzione.  Varianti ammesse in prosa: «Guida ai Minion». |
| Raid Finder | Ricerca Incursioni | `system/maincommand.json#72:name` | Nome della funzione. |
| V&C Dungeon Finder | Ricerca Spedizioni V&C | `system/maincommand.json#94:name` | Nome della funzione; comprende varianti e criterio. Varianti ammesse in prosa: «finestra di ricerca delle spedizioni V&C». |
| Levequests | Mandati | `system/addon.json#8602:original` | Singolare: Mandato; tipo specifico di attività. |
| Leves | Mandati | `system/addon.json#8337:original` | Sinonimo di Levequests; varianti ammesse in prosa: «Incarico di [dispositivo]». |
| Supply & Provisioning Missions | Missioni di Rifornimento e Approvvigionamento | Decisione dell'utente | Denominazione congiunta delle due attività delle Grandi Compagnie. Non usare «Provviste» per Provisioning. Variante ammessa in prosa: «missioni di rifornimento e approvvigionamento». |
| Supply and Provisioning Missions | Missioni di Rifornimento e Approvvigionamento | Decisione dell'utente | Stessa denominazione con la congiunzione estesa. Variante ammessa in prosa: «missioni di rifornimento e approvvigionamento». |
| Trials | Prove | `system/addon.json#8608:original` | Singolare: Prova; attività istanziata. |
| Raids | Incursioni | `system/addon.json#8609:original` | Singolare: Incursione; attività istanziata. |
| Dungeons | Spedizioni | `system/addon.json#8335:original` | Attività istanziata; al singolare: Spedizione. Per il luogo fisico usare Sotterraneo, secondo il contesto. |
| Dungeon | Spedizione | Decisione dell'utente | Etichetta dell'attività istanziata. |
| Dungeon (physical place) | Sotterraneo | Decisione dell'utente | Luogo fisico: usare «sotterraneo» in riferimenti espliciti all'ambiente, come «this dungeon», l'ingresso o l'interno. Le funzioni che concludono un Incarico, per esempio la votazione per terminare e lasciare il dungeon, si riferiscono all'attività e usano «spedizione». Il validatore confronta i riferimenti con la frase tradotta, senza segnalare «spedizione» usato correttamente altrove nello stesso testo. |
| Deep Dungeon | Cripta Profonda | `system/addon.json#2304:original` | Tipo distinto di attività. Varianti ammesse in prosa: «cripta profonda», «cripte profonde», rispettando le maiuscole della frase. |
| Guildhests | Operazioni di Gilda | `system/addon.json#3165:original` | Singolare: Operazione di Gilda. |
| FATE | FATE | `system/addon.json#5768:original` | Sigla invariabile. |
| FATEs | FATE | `system/addon.json#8336:original` | Plurale senza punti. |
| Need | Necessità | `system/addon.json#390:original` | Scelta per l'assegnazione del bottino. |
| Greed | Brama | `system/addon.json#391:original` | Scelta per l'assegnazione del bottino. |
| Pass | Rinuncia | `system/addon.json#392:original` | Scelta per rifiutare il bottino; nella schermata del Mahjong, variante ammessa in prosa: «Passa». |
| Need, Greed, Pass | Necessità, Brama, Rinuncia | `system/addon.json#12693:original` | Nomi delle tre scelte nel sistema di distribuzione del bottino. Varianti ammesse in prosa: «Necessità, Brama e Rinuncia», «Solo Brama». |
| Aetheryte Tickets | Buoni Eterite | `system/addon.json#8501:original` | Oggetti per il teletrasporto; tradurre «Aetheryte» come «Eterite». |
| Aetheryte Ticket Usage | Uso dei Buoni Eterite | `system/addon.json#8522:original` | Etichetta della funzione. |
| Chocobo Porter | Noleggio Chocobo | `system/addon.json#2730:original` | Servizio di noleggio del Chocobo. Varianti ammesse in prosa: «chocobo da trasporto». |
| Levemete | Ufficiale degli Incarichi | `system/addon.json#678:original` | PNG che assegna Mandati; resa contestuale. |
| Advanced Materia Melding | Innesto Avanzato di Materia | `system/howto.json#99:original` | Nome della funzione di innesto avanzato. |
| Familiar | Famiglio | Decisione dell'utente | Forma singolare nel testo dinamico di `system/addon.json#17657`: usare quando il conteggio è 1. |
| Familiars | Famigli | Decisione dell'utente | Forma plurale nel testo dinamico di `system/addon.json#17657`: usare quando il conteggio è diverso da 1. |
| Pet Glamours | Illusione dei Famigli | `system/howto.json#233:original` | Resa del sistema di glamour applicato ai famigli. |
| Moogle Treasure Trove | Forziere del Moguri | `system/addon.json#2778:original` | Nome dell'evento; varianti ammesse in prosa: «Tesoro dei Moogle». |
| Happy Bunny | Coniglio Felice | `system/addon.json#9791:original` | Nome dell'attività; in etichetta usare «Coniglio Felice». Varianti ammesse in prosa: «coniglio felice», «Coniglietto Felice», «coniglietto felice». In `world/enpcresident.json#1042437:translation_name`, l'originale tutto minuscolo richiede «coniglio felice»; al plurale: «conigli felici». |
| Forked Towers | Torri Biforcate | `system/addon.json#16699:original` | Nome del contenuto; al singolare «Torre Biforcata». |
| Resident Caretaker | Custode Residenziale | `system/addon.json#3691:original` | Ruolo nella zona residenziale; varianti ammesse in prosa: «custode residente», «custode locale», «custode», «custode del quartiere». |
| Crystalline Conflict | Conflitto Cristallino | `system/addon.json#5557:original` | Modalità PvP. |
| The Feast | Il Banchetto | `system/addon.json#5510:original` | Modalità PvP; in prosa adattare l'articolo («nel Banchetto», «del Banchetto»). Varianti ammesse in prosa per il senso comune: «banchetto». |
| Frontline | Prima Linea | `system/addon.json#5558:original` | Modalità PvP; verificare le maiuscole nel contesto. |
| Elite Enemy | Nemico d'élite | `system/addon.json#17542:original` | Etichetta con numero dinamico; abbreviare il numero come «n.»; varianti ammesse in prosa: «nemici d'élite», «nemici élite». |
| Rival Wings | Ali Rivali | `system/addon.json#5559:original` | Modalità PvP. |
| Soaring | Ascesa | Decisione dell'utente | Nome dell'effetto di Ali Rivali; non usarlo per il senso generico di «volteggiare». Riferimenti: `combat/status.json#1465:name`; `system/descriptionstring.json#144:original`. |
| Cruise Chaser | Inseguitore | Decisione dell'utente | Nome del machina di Ali Rivali; adattare articolo e numero. |
| Oppressor | Oppressore | Decisione dell'utente | Nome del machina di Ali Rivali; adattare articolo e numero. |
| Brute Justice | Giustizia Brutale | Decisione dell'utente | Nome del machina di Ali Rivali; adattare articolo e numero. |
| Triple Triad | Triple Triad | `system/addon.json#9529:original` | Nome ufficiale del minigioco; varianti ammesse in prosa: «Tris». |
| Lord of Verminion | Lord of Verminion | `system/addon.json#9550:original` | Nome invariato del minigioco. |
| Gold Saucer | Gold Saucer | `system/addon.json#8612:original` | Nome invariato della destinazione. |
| Manderville Gold Saucer | Gold Saucer Manderville | Decisione dell'utente | Nome completo del luogo; usare anche nei composti che ne indicano attrazioni, arredi, punti e crediti. Sostituisce «Gold Saucer di Manderville». Non altera la forma breve «Gold Saucer» né le sigle MGP, MGF e MGC. |
| The Gold Saucer | Gold Saucer | `world/placename.json#1484:name` | Nome della destinazione; variante ammessa in prosa: «Gold Saucer». |
| The Gold Saucer | Il Gold Saucer | `world/placename.json#1484:name` | Forma autonoma del toponimo. |
| Leap of Faith | Salto della Fede | `system/addon.json#8475:original` | GATE del Gold Saucer. |
| The Hunt | La Caccia | `system/addon.json#838:original` | Nome della funzione di gioco; prefisso di un titolo composto da ignorare; distinguere dall'uso comune di hunt; varianti ammesse in prosa: «la caccia». |
| Treasure Hunt | Caccia al Tesoro | `system/addon.json#2276:original` | Modalità di gioco; distinguere dall'uso comune; varianti ammesse in prosa: «Caccia al tesoro», «caccia al tesoro». |
| Journal | Diario di Viaggio | `system/addon.json#450:original` | Nome del menu. |
| Journal | Diario | `system/addon.json#593:original` | Etichetta breve. |
| All | Tutti | `system/addon.json#970:original` | Insieme di elementi; variante ammessa in prosa: «Tutte». |
| All | Tutto | `system/howtocategory.json#1:original` | Categoria tutorial. |
| Battle | Combattimento | `system/howtocategory.json#4:original` | Categoria tutorial. |
| Crafting | Artigianato | `system/howtocategory.json#12:original` | Categoria tutorial. |
| Crafting | Creazione | `system/addon.json#13233:original` | Etichetta contestuale in addon. |
| Overview | Panoramica | `system/descriptionstring.json#122:original` | Titolo di una sezione informativa. |
| Qty. | Qtà | `system/addon.json#618:original` | Abbreviazione dell'etichetta «Quantity». |
| Collectability | Collezionabilità | `system/addon.json#1056:original` | Statistica usata nel sistema di raccolta. |
| Default | Predefinito | `system/addon.json#1217:original` | Valore predefinito nelle opzioni. |
| Category | Categoria | `system/addon.json#612:original` | Etichetta di categoria nell'interfaccia. |
| Requirements | Requisiti | `system/addon.json#992:original` | Titolo di una sezione informativa. |
| Rewards | Ricompense | `system/descriptionstring.json#582:original` | Titolo di una sezione informativa. |
| Leave | Abbandona | `system/addon.json#2508:original` | Comando per lasciare un gruppo o un'attività. |
| Proceed | Procedi | `system/addon.json#924:original` | Comando o pulsante di conferma.  Varianti ammesse in prosa: «Procedere». |
| Remove | Rimuovi | `system/addon.json#85:original` | Comando per rimuovere un elemento. |
| Edit | Modifica | `system/addon.json#442:original` | Comando per modificare un elemento. |
| Class/Job | Classe/Job | `system/addon.json#315:original` | Etichetta d'interfaccia; mantenere «Job» secondo la voce canonica. |
| Healer | Curatore | `system/addon.json#1083:original` | Ruolo di classe; in prosa usare «curatore» e «curatori». |
| Normal | Normale | `combat/status.json#2204:name` | Stato o opzione; accordare in base al sostantivo in contesto. |

### Sistemi e funzioni ricorrenti

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Sanction (Grandi Compagnie) | Benedizione | Decisione dell'utente | Potenziamento degli equipaggiamenti delle Grandi Compagnie; sostituisce «Sanzione di Compagnia», «Sanzione» e «Patrocinio» in questo contesto. Non estendere agli attacchi omonimi di `combat/action.json` né a sanction nel senso comune di permesso. Riferimenti: `combat/status.json#245:name`; `items/item.json#4636:col_2`. |
| Linkshells | Fonoperle | `system/addon.json#410:original` | Plurale; singolare editoriale: Fonoperla. Evita «fonosfera»; conserva /linkshell e /cwlinkshell1 come comandi, ma traduci il nome della funzione nella spiegazione. |
| Linkshell [1] | Fonoperla [1] | `system/addon.json#4500:original` | Etichetta numerata: conservare il numero. |
| Cross-world Linkshells | Fonoperle Intermondo | `system/addon.json#12110:original` | Plurale delle fonoperle intermondo; varianti ammesse in prosa: «fonoperla intermondo», «fonoperle intermondo». |
| Cross-world Linkshell [1] | Fonoperla Intermondo [1] | `system/addon.json#4397:original` | Etichetta numerata: conservare il numero. |
| Cross-world Linkshell Invites | Inviti alle Fonoperle Intermondo | `system/addon.json#102636:original` | Inviti alla funzione intermondo. |
| Cross-world Party | Gruppo Intramondo | `system/addon.json#10902:original` | Gruppo formato tra mondi diversi; maiuscole fisse: Gruppo Intramondo. |
| Linkshell Distributor | Distributore di Fonoperle | `world/placename.json#1237:name` | Nome della funzione o del luogo. |
| Home World | Mondo d'Origine | `world/contentfinderconditiontransient.json#195:original` | Mondo di appartenenza del personaggio. Varianti ammesse in prosa: «Mondo di Origine», «Mondo di Residenza». |
| World Visit | Visita Mondo | `system/addon.json#12510:original` | Funzione di viaggio tra mondi.  Varianti ammesse in prosa: «Visita Mondi», «Guida alla Visita Mondi». |
| Data Center | Data Center | `system/howtopage.json#2506:name` | Nome invariato della struttura server; variante ammessa in prosa per il termine minuscolo nell'originale: «data center».  Varianti ammesse in prosa: «centro dati», «centri dati». Varianti ammesse in prosa: «Centro Dati». |
| Housing | Alloggi | `system/addon.json#1999:original` | Sistema degli alloggi. Varianti ammesse in prosa: «Abitazioni», «Abitazione». |
| Glamours | Illusioni | `system/addon.json#16030:original` | Nome della funzione. Varianti ammesse in prosa: «illusioni», «Illusioni» nei titoli, «Completi Pronti per l'Illusione» per gli Outfit Glamour. Singolare editoriale: «illusione». |
| Glamour Dresser | Comò delle Illusioni | `system/addon.json#3735:original` | Arredo delle illusioni. Varianti ammesse in prosa: «comò delle illusioni», «i comò delle illusioni». |
| Glamour Plate | Piastra d'Illusione | `system/addon.json#3185:original` | Piastra che memorizza un insieme di illusioni. Varianti ammesse in prosa: «piastra d'illusione», «piastre d'illusione», «Piastre d'Illusione» (etichetta del pulsante), rispettando le maiuscole richieste dalla frase. |
| Glamour Prism | Prisma delle Illusioni | `system/addon.json#5733:original` | Catalizzatore per applicare illusioni. Varianti ammesse in prosa: «prisma delle illusioni», «prismi delle illusioni», rispettando le maiuscole della frase. |
| Cast Glamour | Applica Illusione | `system/addon.json#3700:original` | Comando per applicare un'illusione. Varianti ammesse in prosa: «applicare un'illusione», «applicare l'illusione», «applicata un'illusione». Il nome del comando mantiene le maiuscole fisse: «Applica Illusione».  Varianti ammesse in prosa: «applica l'illusione». |
| Apply Glamours | Applica Illusioni | `system/addon.json#10583:original` | Comando al plurale con maiuscole fisse: «Applica Illusioni». Varianti ammesse in prosa: «applicare illusioni», «applicare le illusioni». |
| Glamour-ready | Pronto per l'illusione | `system/addon.json#5728:original` | Idoneità dell'oggetto; adattare genere e numero in prosa. |
| Outfit Glamour | Completo Illusione | `system/addon.json#15644:original` | Insieme di oggetti registrato come illusione. Varianti ammesse in prosa: «completi pronti per l'illusione», «completo pronto per l'illusione», «Completi Pronti per l'Illusione», «Completo Pronto per l'Illusione». |
| Furnishing Glamours | Illusioni d'Arredo | `system/addon.json#15533:original` | Aspetti alternativi degli arredi. Varianti ammesse in prosa: «illusioni d'arredo», «Glamour degli Arredi»; mantenere le minuscole nel testo corrente.  Varianti ammesse in prosa: «illusione d'arredo». |
| Registered Glamours | Illusioni Registrate | `system/addon.json#15528:original` | Nome dell'elenco delle illusioni d'arredo registrate. Varianti ammesse in prosa: «Illusioni Registrate» per l'etichetta, «illusioni registrate» in prosa. |
| Placed Glamours | Illusioni Posizionate | `system/addon.json#15539:original` | Illusioni d'arredo già posizionate.  Varianti ammesse in prosa: «illusione d'arredo è attualmente posizionata sulla tua isola». |
| Register Glamour | Registra Illusione | `system/addon.json#15525:original` | Registra l'aspetto alternativo di un arredo. |
| Place Glamour | Posiziona Illusione | `system/addon.json#15564:original` | Posiziona un'illusione d'arredo. |
| Store as Glamour | Conserva come Illusione | `system/addon.json#15622:original` | Salva un oggetto come illusione. |
| Armoire | Armadio | `system/addon.json#3734:original` | Arredo di deposito. |
| Retainer | Servitore | `system/addon.json#532:original` | Aiutante del personaggio; in prosa minuscolo. |
| Materia Melding | Innesto Materia | `system/addon.json#993:original` | Funzione per innestare materia. Variante ammessa in prosa: «Innesto della Materia». |
| Desynthesis | Desintesi | `system/addon.json#1815:original` | Funzione di smontaggio degli oggetti. |
| Aetherial Reduction | Riduzione Eterea | `system/addon.json#2160:original` | Funzione di riduzione; varianti ammesse in prosa: «riduzione eterea», «Riduzione Eterica», «la riduzione eterea», «riduzioni eteree». |
| Item Dyeing | Tintura Oggetti | `system/addon.json#4690:original` | Funzione di tintura dell'equipaggiamento; varianti ammesse in prosa: «tintura degli oggetti», «finestra di tintura degli oggetti». |
| Custom Deliveries | Consegne Personalizzate | `system/notebookdivisioncategory.json#6:original` | Attività ricorrente; varianti ammesse in prosa: «Consegne su Misura». |
| Custom Deliveries | Consegne su Misura | `world/placename.json#1298:name` | Forma approvata per l'etichetta nel catalogo dei luoghi; variante della funzione «Consegne Personalizzate». |
| Wondrous Tails | Code Meravigliose | `system/addon.json#5600:original` | Registro di attività. |
| Portraits | Ritratti | `system/addon.json#14650:original` | Funzione del personaggio. |
| Actions & Traits | Azioni e Tratti | `system/maincommand.json#3:name` | Menu del personaggio. |
| Blacklist | Lista Nera | `system/maincommand.json#14:name` | Elenco di giocatori bloccati. |
| Character Configuration | Configurazione Personaggio | `system/maincommand.json#34:name` | Menu delle impostazioni; varianti ammesse in prosa: «configurazione del personaggio», «Configurazione del Personaggio», «impostazioni del personaggio».  Varianti ammesse in prosa: «configurazione attuale del tuo personaggio». Varianti ammesse in prosa: «impostazioni del Client e del personaggio». |
| Collection | Collezione | `system/maincommand.json#87:name` | Menu della collezione. |
| Companion | Compagno | `system/maincommand.json#42:name` | Funzione del compagno. |
| Contacts | Giocatori Recenti | `system/maincommand.json#74:name` | Elenco dei contatti recenti. |
| Countdown | Conto alla Rovescia | `system/maincommand.json#73:name` | Funzione del gruppo. |
| Emotes | Emote | `system/maincommand.json#17:name` | Categoria di azioni sociali. |
| Fashion Accessories | Accessori di Moda | `system/maincommand.json#89:name` | Collezione di accessori. Varianti ammesse in prosa: «accessorio di moda», «accessori di moda», rispettando le maiuscole della frase. |
| Friend List | Lista Amici | `system/maincommand.json#13:name` | Elenco sociale. |
| HUD Layout | Disposizione HUD | `system/maincommand.json#22:name` | Funzione di disposizione dell'interfaccia. Varianti ammesse in prosa: «disposizione dell'HUD», «disposizione HUD», «disposizione speciale dell'HUD», «disposizione delle barre azioni nell'HUD», «disposizione e dimensione dell'HUD», «layout HUD», «editor del layout HUD».  Varianti ammesse in prosa: «layout dell'HUD corrente». Varianti ammesse in prosa: «disposizione predefinita dell'HUD». |
| Key Items | Oggetti Chiave | `system/maincommand.json#11:name` | Categoria di inventario; varianti ammesse in prosa: «oggetti chiave», «scheda Oggetti chiave». |
| Keybind | Assegnazione Tasti | `system/maincommand.json#20:name` | Configurazione dei comandi. |
| Mount Speed | Velocità Cavalcatura | `system/maincommand.json#75:name` | Funzione delle cavalcature; varianti ammesse in prosa: «velocità della cavalcatura», «velocità delle cavalcature», «quella della cavalcatura», «Velocità Cavalcature Terrestri», «velocità delle cavalcature terrestri». |
| Mute List | Lista Silenziati | `system/maincommand.json#96:name` | Nome del menu. Maiuscole fisse: «Lista Silenziati». Varianti ammesse in prosa: «lista dei personaggi silenziati».  Varianti ammesse in prosa: «elenco dei personaggi silenziati». |
| Player Search | Ricerca Giocatori | `system/maincommand.json#15:name` | Ricerca di altri giocatori. |
| System Configuration | Configurazione di Sistema | `system/howtopage.json#21:name` | Menu delle impostazioni.  Varianti ammesse in prosa: «configurazione del sistema». |
| User Macros | Macro Utente | `system/howtopage.json#82:name` | Funzione delle macro.  Varianti ammesse in prosa: «macro personali». |
| Accessibility Settings | Impostazioni Accessibilità | `system/maincommand.json#63:name` | Impostazioni di accessibilità. |
| Graphics Settings | Impostazioni Grafiche | `system/maincommand.json#47:name` | Impostazioni video.  Varianti ammesse in prosa: «parametri di rendering grafico». |
| Sound Settings | Impostazioni Audio | `system/maincommand.json#46:name` | Impostazioni audio.  Varianti ammesse in prosa: «impostazioni sonore», «volumi sonori». |
| UI Settings | Impostazioni Interfaccia | `system/maincommand.json#52:name` | Impostazioni dell'interfaccia. Varianti ammesse in prosa: «Impostazioni UI», «interfaccia di gioco», «parametri dell'interfaccia grafica», «impostazioni dell'interfaccia utente». |
| Item Settings | Impostazioni Oggetti | `system/maincommand.json#71:name` | Impostazioni degli oggetti. |
| Mouse Settings | Impostazioni Mouse | `system/maincommand.json#48:name` | Impostazioni del mouse. Varianti ammesse in prosa: «impostazioni di [dispositivo] e del mouse», «impostazioni Tastiera/Mouse», «parametri del puntatore del mouse» quando il dispositivo è indicato dal tag dinamico. |
| Theme Settings | Impostazioni Tema | `system/maincommand.json#83:name` | Impostazioni grafiche. |
| Configuration Sharing | Condivisione Configurazione | `system/maincommand.json#99:name` | Condivisione delle impostazioni; varianti ammesse in prosa: «condivisione della configurazione». |
| Party Members | Membri del Gruppo | `system/maincommand.json#12:name` | Nome dell'elenco. Varianti ammesse in prosa: «membro del gruppo», «membri del gruppo», «membri vicini del gruppo», «membri del suo gruppo», «membri», «altri membri», «tutti i membri», «compagni di squadra», «tutto il gruppo», «il gruppo», «gruppo prefissato», «gruppo abbinato», «gruppo di massimo quattro giocatori», «gruppi da uno a quattro giocatori», «gruppo in anticipo», «con il gruppo», «in gruppo», «menu del gruppo», «nessun membro nel gruppo», «membri nel gruppo», «altri membri nel gruppo», «membri per il gruppo» (quando il riferimento al gruppo è chiaro).  Varianti ammesse in prosa: «Membri del party». |
| Sanctuary Crafting Log | Registro di Fabbricazione del Rifugio Insulare | `system/addon.json#14259:original` | Nome del registro di fabbricazione del Rifugio Insulare; variante in prosa: «registro di fabbricazione del Rifugio Insulare». |
| Sanctuary Gathering Log | Registro di Raccolta del Rifugio Insulare | `system/addon.json#14260:original` | Nome del registro di raccolta del Rifugio Insulare; varianti ammesse in prosa: «registro di raccolta del Rifugio Insulare». |
| Currency | Valute | `system/maincommand.json#66:name` | Nome del menu; altre etichette usano il singolare «Valuta». |
| Currency | Valuta | `items/itemuicategory.json#100:original` | Forma singolare usata nelle etichette. |


| Lost Finds Cache | Deposito dei Reperti Perduti | `system/addontransient.json#350:original` | Deposito delle azioni perdute del Fronte Meridionale di Bozja. |
### Termini di gioco

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Free Company | Compagnia Libera | `system/howtopage.json#1760:name` | Varianti ammesse in prosa: «Compagnia Libera», «compagnia libera», «compagnie libere» quando il riferimento indica la stessa organizzazione.  Varianti ammesse in prosa: «Compagnie Libere». |
| Grand Company | Grande Compagnia | `system/addon.json#337:original` | Varianti ammesse in prosa: «Grandi Compagnie». |
| Subleader | Vice | `system/addon.json#11771:original` | Ruolo nella squadra PvP; in prosa minuscolo «vice». |
| Subleaders | Vice | `system/addon.json#11819:original` | Plurale invariabile; in prosa «i vice». |
| Aetheryte | Eterite | `system/addon.json#8511:original` | Nome comune della struttura; distinto dai toponimi omonimi. |
| Aetherial node(s) | Nodo Eterico / Nodi Eterici | Decisione dell'utente | Nome comune dei nodi eterici; in prosa usare la minuscola e accordare singolare/plurale. Riferimenti: `activities/publiccontenttextdata.json#2134:original`; `world/eobjname.json#2009727:name`. |
| Keystone (rite of succession) | Pietrachiave | Decisione dell'utente | Nome delle sette pietre ottenute dagli elettori nel rito di successione; scrivere una parola: «pietrachiave» al singolare e «pietrachiavi» al plurale. Non estendere a chiavi di volta architettoniche, alle pietre del Palazzo del Viandante o agli usi figurati. Riferimenti: `activities/ktgtypewordtextdata.json#1685:name`; `world/eobjname.json#2005518:name`. |
| proto-aetheryte | protoeterite | Decisione dell'utente | Nome comune dell'apparato antico di Eureka; in prosa minuscolo. Sostituisce «proto-eterite» ed «etereporto prototipo». Riferimento: `world/contentfinderconditiontransient.json#598:original`. |
| Crystal Forge | Forgia di Cristallo | Decisione dell'utente | Nome dell’apparato usato nelle spedizioni Eureka; al plurale «Forge di Cristallo». Versione migliorata: «Forgia di Cristallo Mark II». Riferimenti: `dialogue/customtalk.json#721389:col_32`; `world/eobjname.json#2009555:name`. |
| Logos Manipulator | Sintetizzatore Logos | Decisione dell'utente | Nome del dispositivo che estrae e trasmuta i mnemi dai logogrammi; nei richiami abbreviati allo stesso dispositivo usare «Sintetizzatore». Riferimenti: `dialogue/customtalk.json#721411:col_32`; `system/addon.json#11640:original`. |
| Aetherpool | Eterico | Decisione dell'utente | Controllo automatico: residuo. Aggettivo da accordare al sostantivo: «arma/armatura/impugnatura eterica», «armi/armature/impugnature eteriche», «equipaggiamento/vetro/frammento eterico», «frammenti/scudi eterici». Eliminare «di aetherpool» quando diventa un aggettivo; adeguare articoli e altri aggettivi. Maiuscole secondo il campo sorgente e la funzione, minuscolo in prosa. Sostituisce anche le precedenti rese «etereo/eterea» e «della riserva eterea» riferite ad aetherpool; non modificare gli altri usi di etereo né sorgenti, identificatori o tag. Varianti ammesse in prosa: «eterica», «eterici», «eteriche». |
| Vitiated Aether | Etere corrotto | Decisione dell'utente | Flusso energetico delle spedizioni Eureka; in prosa usare la minuscola. Riferimenti: `activities/publiccontenttextdata.json#2001:original`; `system/logmessage.json#3741:original`. |
| Scrip | Gettone | Decisione dell'utente | Valuta degli Artigiani e dei Raccoglitori; al plurale usare «Gettoni degli Artigiani/dei Raccoglitori». |
| Tankbuster | Colpo Devastante | Decisione dell'utente | Nome della meccanica di attacco ad alto danno rivolta al difensore; usare questa resa nelle etichette, adattando articoli e accordi in prosa. |
| Instinctual Skill | Azione Istintiva | Decisione dell'utente | Termine di gioco del Domatore e dei famigli. Plurale: «Azioni Istintive». In prosa usare «azione istintiva / azioni istintive», rispettando le maiuscole dell'originale; sostituisce «abilità istintiva/istintuale» e «tecnica istintiva». Riferimento: `items/xbmitem.json#19:translation_col_3`. Non estendere questa resa a ogni occorrenza di skill: identifica prima la categoria di azione nel contesto. |
| Simular | Simulacro | Decisione dell'utente | Categoria di mangime; nei nomi composti usare «Simulacro di [ingrediente]» e al plurale «Simulacri di [ingrediente]». |
| EXP | ESP | Decisione dell'utente | Controllo automatico: residuo. Sigla italiana dei punti esperienza; usare «ESP» nelle etichette e descrizioni (es. «Bonus ESP»). Riferimento: `items/item.json#2632:translation_col_2`. |
| Rolanberry | Rolanbacca | Decisione dell'utente | Controllo automatico: residuo. Nome del frutto e ingrediente, femminile: «la Rolanbacca», «della Rolanbacca». Nei nomi di ricette usare la forma idiomatica «alle Rolanbacche»; accordare articoli e aggettivi al femminile plurale (es. «le Rolanbacche essiccate e aspre»). Eccezione: «Rolanberry Field» è il nome proprio «Campo di Rolanbacche»; «Rolanberry children» indica i bambini del campo, non i frutti. Riferimento: `items/item.json#4815:translation_name`. |
| Rolanberries | Rolanbacche | Decisione dell'utente | Controllo automatico: residuo plurale. Plurale femminile: «le/delle/alle Rolanbacche»; concordare gli aggettivi (es. «Rolanbacche essiccate e aspre»). Riferimento: `items/item.json#4815:translation_description`. |
| Rolanberry Red | Rosso Rolan | `items/stain.json#9:name` | Nome della tonalità; nel nome dell'oggetto usare «Tintura Rosso Rolan», senza interpretare Rolanberry come frutto. |
| Minotaur Malm | Il Malm del Minotauro / Malm del Minotauro | `world/placename.json#322:name` | Nome proprio della località; mantenere «Malm» anche nei composti (es. `world/placename.json#470:name`). |
| Claret Dragon | Drago Cremisi | Decisione dell'utente | Nome della creatura; nel composto «Claret Dragon's Power» usare «Potere del Drago Cremisi». |
| Scion of the Seventh Dawn | Erede della Settima Alba | `combat/companiontransient.json#91:description` | Nome invariabile per genere: al singolare «Erede della Settima Alba», al plurale «Eredi della Settima Alba». |
| Scion | Erede | Decisione dell'utente | Forma singolare; «Erede» non varia per genere. |
| Scions | Eredi | Decisione dell'utente | Forma plurale; per il gruppo usare «Eredi della Settima Alba». |
| Delineation | Compendio | `crafting/craftaction.json#100179:description` | Risorsa consumata dalle Azioni da Specialista; usare «Compendio di [disciplina]» e, per «Crafter's Delineation», «Compendio dell'Artigiano». |
| Light-heavyweight | Pesi Leggeri | Decisione dell'utente | Categoria di peso AAC M1–M4. |
| Cruiserweight | Pesi Medi | Decisione dell'utente | Categoria di peso AAC M1–M4. |
| Heavyweight | Pesi Massimi | Decisione dell'utente | Categoria di peso AAC M1–M4. |
| Ultimate raid | Incursione Suprema | Decisione dell'utente | «Suprema» è la resa della modalità Ultimate. |
| Blunderful | Blunderful | Decisione dell'utente | Nome ufficiale della cornice Fall Guys; lasciare in inglese. |
| Blunderous | Blunderous | Decisione dell'utente | Nome ufficiale della cornice Fall Guys; lasciare in inglese. |
| Blunder-villed | Blunder-villed | Decisione dell'utente | Nome ufficiale della cornice Fall Guys; lasciare in inglese. |
| The Black Shroud | Velo Nero | `system/addon.json#1578:original` | Nome dell'area. Genere grammaticale maschile: usare «il Velo Nero», «del Velo Nero», «nel Velo Nero». Vietato categoricamente l'uso di articoli o preposizioni femminili («la Velo Nero», «della Velo Nero», «nella Velo Nero»). |
| Airship | Aeronave | Decisione dell'utente | Controllo automatico: residuo plurale. Plurale inglese: «Airships» → «Aeronavi». |
| Mount | Cavalcatura | `system/addon.json#774:original` | Nome della cavalcatura; distinto dalle azioni «Monta» e «Pilota». |
| Achievement | Obiettivo | `system/addon.json#1484:original` | Per la categoria di gioco usare «Obiettivo» al singolare e «Obiettivi» al plurale; accordare gli articoli («gli obiettivi», «degli obiettivi»). In prosa, il termine resta minuscolo. |
| Quest | Missione | `system/addon.json#12723:original` | Categoria generale, singolare. |
| Job Gauge | Barra Job | `system/guidetitle.json#1:name` | Nome dell'indicatore per il Job; varianti ammesse in prosa: «Indicatore Job».  Varianti ammesse in prosa: «indicatore Job». |
| Addersgall Gauge | Barra dell'Umore | Decisione dell'utente | Barra Job del Saggio; mantenere «Addersgall» quando si parla della risorsa accumulata. |
| Retainer | Retainer | `system/addon.json#12580:original` | Termine conservato in una specifica etichetta. |
| Mount | Monta | `system/addon.json#4964:original` | Azione contestuale. |
| Mount | Pilota | `system/addon.json#11382:original` | Azione contestuale. |
| Weaponskills | Tecniche | `system/addon.json#14484:original` | Categoria di azione; singolare: «tecnica». Nelle descrizioni tecniche è attestato anche «tecniche d'arma» (`combat/status.json#1346:translation_description`). Non confondere con abilità, incantesimi o azioni istintive. |
| Byregot's Strike | Colpo di Byregot | `combat/action.json#29029:original` | Nome di un'azione; «strike» qui significa colpo, non sciopero. |
| Byregot's Strike | Il Colpo di Byregot | `world/placename.json#324:name` | Forma autonoma del toponimo. |
| Air Raid | Incursione Aerea | `combat/action.json#6602:original` | Nome di un'azione; «raid» qui è reso come «incursione». |
| Formation: Air Raid | Formazione: Incursione Aerea | `combat/action.json#20400:original` | Nome di un'azione. |
| Call Strategic Raid | Chiama Incursione Strategica | `combat/action.json#24578:original` | Nome di un'azione. |
| Hair Raid | Incursione dei Capelli | `combat/action.json#30076:original` | Nome di un'azione; stessa resa per le varianti dello stesso nome. |
| Morrowmotes | Soffio d'Aurora | `combat/action.json#4026:original` | Nome di un'azione. |
| Morrowmotes | Soffio d'Aurora | `world/placename.json#2156:name` | Nome di luogo; stessa resa del nome dell'azione. |
| Dungeon Siege | Assedio Sotterraneo | `world/achievement.json#742:name` | Nome della serie di obiettivi; mantiene il numero romano. |
| Dungeon Master | Signore del Sotterraneo | `world/achievement.json#746:name` | Nome di un obiettivo. |
| Dungeon Master | Signora del Sotterraneo | `world/title.json#109:description` | Forma femminile del titolo; accordo di genere. |
| HP | PV | `system/addon.json#232:original` | Sigla conservata in un'etichetta. |
| MP | PM | `system/addon.json#233:original` | Sigla conservata in un'etichetta. |


| Maelstrom Command | Comando della Tempesta | `world/contentfinderconditiontransient.json#40:original` | Nome del luogo o della struttura. |
| Conjurers' Guild | Gilda degli Incantatori | `system/lobby.json#188:description` | Nome del luogo o della struttura. |
| Dosis | Dose | Decisione dell'utente | Nome dell'incantesimo del saggio; mantenere il singolare anche nelle varianti Dose II, Dose III e Dose Eukrasica (II/III), comprese le azioni dei PNG e i riferimenti nei payload SeString. Non modificare il sostantivo italiano comune «dosi». Riferimenti: `combat/action.json#24283:original`; `combat/actiontransient.json#24290:translation`. |

### Misure e pesi

Decisione dell'utente: adattamento morfologico delle unità di misura eorzeane:
- **Distanza / Lunghezza (maschili)**: singolare in *-m*, plurale in *-mi* (*ilm / ilmi, fulm / fulmi, yalm / yalmi, malm / malmi*).
- **Peso (femminili)**: singolare in *-a*, plurale in *-e* (*onza / onze, ponza / ponze, tonza / tonze*).

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| yalm | yalm | Decisione dell'utente | Unità di misura eorzeana di lunghezza (maschile, circa 1 iarda / 0,9 m). Riferimento: `dialogue/defaulttalk.json#589846:name`. |
| yalms | yalmi | Decisione dell'utente | Plurale maschile (es. «dieci maledetti yalmi»). |
| malm | malm | Decisione dell'utente | Unità di misura eorzeana di distanza (maschile, circa 1 miglio / 1,6 km). |
| malms | malmi | Decisione dell'utente | Plurale maschile. |
| ilm | ilm | Decisione dell'utente | Unità di misura eorzeana di lunghezza (maschile, circa 1 pollice / 2,5 cm). |
| ilms | ilmi | Decisione dell'utente | Plurale maschile. |
| fulm | fulm | Decisione dell'utente | Unità di misura eorzeana di lunghezza (maschile, circa 1 piede / 30 cm). |
| fulms | fulmi | Decisione dell'utente | Plurale maschile. |
| onze | onza | Decisione dell'utente | Unità di peso eorzeana (femminile, circa 1 oncia / 28 g). |
| onzes | onze | Decisione dell'utente | Plurale femminile. |
| ponze | ponza | Decisione dell'utente | Unità di peso eorzeana (femminile, circa 1 libbra / 453 g). |
| ponzes | ponze | Decisione dell'utente | Plurale femminile. |
| tonze | tonza | Decisione dell'utente | Unità di peso eorzeana (femminile, circa 1 tonnellata). |
| tonzes | tonze | Decisione dell'utente | Plurale femminile. |

Nel controllo delle unità, non declinare le forme che appartengono a nomi propri attestati: «Hell of Malms» / «Inferno dei Malm» (`world/placename.json#1773:name`), «Baldewyn Four Yalms» (`world/bnpcname.json#1381:name`) e il titolo musicale «Six Fulms Under» (`housing/orchestrion.json#855:name`). Queste eccezioni valgono solo per i nomi indicati.

### Classi e mestieri

Per le etichette delle armi, *One<hex:021F0103>handed* e *Two<hex:021F0103>handed* diventano **a una mano** e **a due mani**, senza il tag del trattino `<hex:021F0103>`. Il trattino appartiene al composto inglese e non va reinserito nella locuzione italiana. Questa eccezione non autorizza la rimozione di altri tag né dei trattini necessari in italiano (es. «città-stato»). Fonte: Decisione dell'utente; `items/itemuicategory.json#6:translation`, `#7:translation`, `#8:translation`, `#9:translation`.

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| DoW/DoM | Combattenti/Maghi | `system/addon.json#1080:original` | Etichetta breve per le categorie Discepoli della Guerra e Discepoli della Magia. |
| DoH/DoL | Artigiani/Raccoglitori | `system/addon.json#1081:original` | Etichetta breve per le categorie Discepoli della Mano e Discepoli della Terra. |
| Lancer | Lanciere | `world/classjob.json#4:name` |  Forma femminile per PNG identificati nei campi di supporto: «Lanciera». Varianti ammesse in prosa: «Lanciera». Non applicare il genere del giocatore a questi ruoli. |
| Archer | Arciere | `world/classjob.json#5:name` |  Forma femminile per PNG identificati nei campi di supporto: «Arciera». Varianti ammesse in prosa: «Arciera». Non applicare il genere del giocatore a questi ruoli. |
| Thaumaturge | Taumaturgo | `world/classjob.json#7:name` |  Forma femminile per PNG identificati nei campi di supporto: «Taumaturga». Varianti ammesse in prosa: «Taumaturga». Non applicare il genere del giocatore a questi ruoli. |
| Armorer | Corazziere | `world/classjob.json#10:name` | Nome della classe artigiana; usare «corazziere» anche nei composti e nelle descrizioni. |
| White Mage | Mago Bianco | `world/classjob.json#24:name` | Varianti ammesse in prosa: «mago bianco», «Maga Bianca», «Maga Bianca Makai». |
| Black Mage | Mago Nero | `world/classjob.json#25:name` | Varianti ammesse in prosa: «mago nero», «maghi neri», «Maga Nera», «maghe nere», «Maga Nera Makai». |
| Dark Knight | Cavaliere Oscuro | `world/mounttransient.json#56:description` | Variante ammessa in prosa: «cavaliere oscuro».  Varianti ammesse in prosa: «Cavalieri Oscuri», «cavalieri oscuri». |
| Red Mage | Mago Rosso | `world/classjob.json#35:name` | Varianti ammesse in prosa: «mago rosso», «Maga Rossa», «maga rossa», «Maga Rossa Makai». |
| Blue Mage | Mago Blu | `world/classjob.json#36:name` | Varianti ammesse in prosa: «mago blu», «maghi blu». |
| Beastmaster | Domatore | `world/classjob.json#43:name` |  Nei riferimenti al Job usa «Domatore», in prosa «domatore» secondo le maiuscole sorgente; evita il residuo «beastmaster». Le azioni dei famigli sono «azioni istintive», non tecniche d'arma. |

### Sigle delle classi e dei mestieri

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| GLA | GLA | `world/classjobcategory.json#2:original` | Sigla del Gladiatore. |
| PGL | PGL | `world/classjobcategory.json#3:original` | Sigla del Pugile. |
| MRD | ICR | `world/classjobcategory.json#4:original` | Sigla dell'Incursore. |
| LNC | LNC | `world/classjobcategory.json#5:original` | Sigla del Lanciere. |
| ARC | ARC | `world/classjobcategory.json#6:original` | Sigla dell'Arciere. |
| CNJ | INC | `world/classjobcategory.json#7:original` | Sigla dell'Incantatore. |
| THM | TAU | `world/classjobcategory.json#8:original` | Sigla del Taumaturgo. |
| CRP | FAL | `world/classjobcategory.json#9:original` | Sigla del Falegname. |
| BSM | FBR | `world/classjobcategory.json#10:original` | Sigla del Fabbro. |
| ARM | COR | `world/classjobcategory.json#11:original` | Sigla del Corazziere. |
| GSM | ORE | `world/classjobcategory.json#12:original` | Sigla dell'Orefice. |
| LTW | CON | `world/classjobcategory.json#13:original` | Sigla del Conciatore. |
| WVR | TES | `world/classjobcategory.json#14:original` | Sigla del Tessitore. |
| ALC | ALC | `world/classjobcategory.json#15:original` | Sigla dell'Alchimista. |
| CUL | CUO | `world/classjobcategory.json#16:original` | Sigla del Cuoco. |
| MIN | MIN | `world/classjobcategory.json#17:original` | Sigla del Minatore. |
| BTN | BOT | `world/classjobcategory.json#18:original` | Sigla del Botanico. |
| FSH | PES | `world/classjobcategory.json#19:original` | Sigla del Pescatore. |
| PLD | PLD | `world/classjobcategory.json#20:original` | Sigla del Paladino. |
| MNK | MNC | `world/classjobcategory.json#21:original` | Sigla del Monaco. |
| WAR | GUE | `world/classjobcategory.json#22:original` | Sigla del Guerriero, applicata solo nelle categorie di classi e mestieri. |
| DRG | DRG | `world/classjobcategory.json#23:original` | Sigla del Dragone. |
| BRD | BRD | `world/classjobcategory.json#24:original` | Sigla del Bardo. |
| WHM | MBI | `world/classjobcategory.json#25:original` | Sigla del Mago Bianco. |
| BLM | MNE | `world/classjobcategory.json#26:original` | Sigla del Mago Nero. |
| ACN | ACN | `world/classjobcategory.json#27:original` | Sigla dell'Arcanista. |
| SMN | EVO | `world/classjobcategory.json#28:original` | Sigla dell'Evocatore. |
| SCH | STU | `world/classjobcategory.json#29:original` | Sigla dello Studioso. |
| ROG | FUR | `world/classjobcategory.json#91:original` | Sigla del Furfante. |
| NIN | NIN | `world/classjobcategory.json#92:original` | Sigla del Ninja. |
| MCH | ART | `world/classjobcategory.json#96:original` | Sigla dell'Artificiere. |
| DRK | COS | `world/classjobcategory.json#98:original` | Sigla del Cavaliere Oscuro. |
| AST | AST | `world/classjobcategory.json#99:original` | Sigla dell'Astrologo. |
| SAM | SAM | `world/classjobcategory.json#111:original` | Sigla del Samurai. |
| RDM | MRO | `world/classjobcategory.json#112:original` | Sigla del Mago Rosso. |
| BLU | MBL | `world/classjobcategory.json#129:original` | Sigla del Mago Blu. |
| GNB | ETE | `world/classjobcategory.json#149:original` | Sigla dell'Eterlama. |
| DNC | DNZ | `world/classjobcategory.json#150:original` | Sigla del Danzatore. |
| RPR | MTR | `world/classjobcategory.json#180:original` | Sigla del Mietitore. |
| SGE | SGO | `world/classjobcategory.json#181:original` | Sigla del Saggio. |
| VPR | VPR | `world/classjobcategory.json#196:original` | Sigla della Vipera. |
| PCT | PTM | `world/classjobcategory.json#197:original` | Sigla del Pittomante. |
| BST | DMT | `world/classjobcategory.json#203:original` | Sigla del Domatore. |

### Suffissi di equipaggiamento (Ruoli e Discipline)

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| of Fending | della Difesa | Decisione dell'utente | Controllo automatico: suffisso ruolo equipaggiamento. Suffisso per l'equipaggiamento da difensore (Tank). Sostituisce «da difensore» e «di difesa». |
| of Maiming | della Lacerazione | Decisione dell'utente | Controllo automatico: suffisso ruolo equipaggiamento. Suffisso per l'equipaggiamento da assaltatore pesante (Dragoon / Reaper). |
| of Slaying | della Furia | Decisione dell'utente | Controllo automatico: suffisso ruolo equipaggiamento. Suffisso per l'equipaggiamento/accessori da mischia (Strength DPS). Sostituisce «da attacco». |
| of Striking | dell'Impeto | Decisione dell'utente | Controllo automatico: suffisso ruolo equipaggiamento. Suffisso per l'equipaggiamento da mischia corpo a corpo (Monk / Samurai). Sostituisce «da combattimento». |
| of Scouting | dell'Incursione | Decisione dell'utente | Controllo automatico: suffisso ruolo equipaggiamento. Suffisso per l'equipaggiamento da ricognizione/incursione (Ninja / Viper). Sostituisce «da esploratore». |
| of Aiming | della Mira | Decisione dell'utente | Controllo automatico: suffisso ruolo equipaggiamento. Suffisso per l'equipaggiamento da attaccante fisico a distanza (Physical Ranged DPS). Sostituisce «da mira». |
| of Casting | dell'Arcano | Decisione dell'utente | Controllo automatico: suffisso ruolo equipaggiamento. Suffisso per l'equipaggiamento da incantatore (Magic Ranged DPS). Sostituisce «da incantatore» e «per il lancio di incantesimi». |
| of Healing | della Cura | Decisione dell'utente | Controllo automatico: suffisso ruolo equipaggiamento. Suffisso per l'equipaggiamento da curatore (Healer). Sostituisce «di guarigione» e «da guaritore». |
| of War | della Guerra | Decisione dell'utente | Controllo automatico: suffisso ruolo equipaggiamento. Suffisso per l'equipaggiamento generico dei Discepoli della Guerra (Disciples of War). |
| of Magic | della Magia | Decisione dell'utente | Controllo automatico: suffisso ruolo equipaggiamento. Suffisso per l'equipaggiamento generico dei Discepoli della Magia (Disciples of Magic). |
| of Crafting | della Creazione | Decisione dell'utente | Controllo automatico: suffisso ruolo equipaggiamento. Suffisso per l'equipaggiamento dei Discepoli della Mano (Disciples of the Hand). Sostituisce «da sintesi». |
| of Gathering | della Raccolta | Decisione dell'utente | Controllo automatico: suffisso ruolo equipaggiamento. Suffisso per l'equipaggiamento dei Discepoli della Terra (Disciples of the Land). Sostituisce «da raccolta». |

### Oggetti ed equipaggiamento

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Artemis Bow | arco di Artemide | Decisione dell'utente | Controllo automatico: residuo. Arma relic per Bardo e relativi stadi evolutivi. |
| Stardust Rod | verga della Polvere Stellare | Decisione dell'utente | Controllo automatico: residuo. Arma relic per Mago Nero e relativi stadi evolutivi. |
| Chicken Knife | Coltello da Pollo | Decisione dell'utente | Controllo automatico: residuo. Arma iconica per Mago Rosso. |
| break blade | lama spezzante | Decisione dell'utente | Spada da gladiatore. |
| thousand needle | ago dei mille aghi | Decisione dell'utente | Strumento da tessitore. |
| dual haken | doppio haken | Decisione dell'utente | Ascia da marauder. |
| the Destroyer's Stead | Sostegno del Distruttore | Decisione dell'utente | Lancia sacra di Ser Vellguine. |
| Worm of the Dell | Drago della Conca | Decisione dell'utente | Bastone sacro di Ser Haumeric (worm nel senso araldico di drago/lindworm). |
| cluster | aggregato | Decisione dell'utente | Categoria di cristalli elementali di alto rango (es. aggregato di fulmine, aggregato planare). |
| Blade's (Bozja gear) | della Lama | Decisione dell'utente | Set di equipaggiamento delle Lame di Bozja (es. diadema della Lama della Difesa). |
| Gunblade | Eterlama | Decisione dell'utente | Controllo automatico: residuo. Arma del job Gunbreaker (femminile: l'eterlama, armi da eterlama). |
| Titanbronze | Bronzo Titanico | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Lega e set di equipaggiamento (es. spada di bronzo titanico, lingotto di bronzo titanico). |
| Deepgold | Oro Cupo | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Lega aurifera e set di equipaggiamento (es. spada d'oro cupo, lingotto d'oro cupo). |
| Stonegold | Litoro | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Lega aurifera-litica e set di equipaggiamento (es. stocco di litoro, lingotto di litoro). |
| Bluespirit | Azzurrite | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Minerale etereo azzurro e set di equipaggiamento (es. spada d'azzurrite, minerale d'azzurrite). |
| Truegold | Aurovero | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Minerale/metallo aurifero puro (es. sabbia d'aurovero, minerale d'aurovero). |
| Equipaggiamento in materiali composti | in [materiale] di [creatura/origine] | Decisione dell'utente | Per evitare ripetizioni di «di» (es. «grimorio in pelle di lobo pregiato» anziché «di pelle di...»; «targa in pelle di raptor»). Non si applica a contenitori/unità di misura come «tazza di tisana» o «pezza di cuoio di». |
| Mistic Memory | della Memoria Mistica | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Set di equipaggiamento (es. celata della Memoria Mistica della Difesa). |
| Queensknight | del Cavaliere della Regina | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Set di armi ed equipaggiamento (es. falcione del Cavaliere della Regina). |
| Sky Rat | del Ratto del Cielo | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Set di equipaggiamento (es. maschera del Ratto del Cielo della Difesa). |
| Radiant's (gear) | della Schiera Radiosa | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Set di equipaggiamento della Schiera Radiosa di Thavnair (es. spada bastarda della Schiera Radiosa). |
| Law's Order | dell'Ordine Giusto | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Set di armi di Bozja (es. spada bastarda dell'Ordine Giusto). |
| Courtly Lover | dell'Amante Cortese | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Set di armi ed equipaggiamento (es. spada dell'Amante Cortese). |
| Tomestone | Tavoletta | Decisione dell'utente | Controllo automatico: residuo plurale. Singolare femminile: «la tavoletta allagana»; plurale: «le tavolette allagane». Nei testi, rileggere la frase e adattare articoli, possessivi, aggettivi e verbi; non sostituire il lemma alla cieca. |
| Wolf Mark | Marchio del Lupo | Decisione dell'utente | Valuta PvP; plurale: Marchi del Lupo (es. 500 Marchi del Lupo). Sostituisce «Contrassegno del Lupo». |
| Dated (gear) | obsoleto / obsoleta | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Equipaggiamento della versione 1.0 (es. gladio di bronzo obsoleto, spatha di bronzo obsoleta). Sostituisce la dicitura «d'epoca». |
| Cesti | Tirapugni | Decisione dell'utente | Categoria d'arma per pugile/monaco (es. paio di tirapugni in pelle di dodo). Sostituisce «cesti». |
| Gridanian (gear/items) | gridaniano / gridaniana | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Aggettivo relativo a Gridania (es. macuahuitl gridaniano, spatha gridaniana). |
| Lominsan | lominsano / lominsana | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Aggettivo relativo a Limsa Lominsa (es. spada corta lominsana, arco lominsano). Sostituisce la formula perifrastica «di Limsa Lominsa». |
| Ul'dahn (gear/items) | ul'dahno / ul'dahna | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Aggettivo relativo a Ul'dah (es. falchione ul'dahno, scimitarra ul'dahna). Sostituisce la formula perifrastica «di Ul'dahn». |
| GC Factions (short form) | della Fiamma / della Vipera / della Tempesta | Decisione dell'utente | Forma abbreviata per equipaggiamenti e gradi delle Grandi Compagnie (es. «libro del sergente della Fiamma», «pugnale del soldato semplice della Vipera», «spadone dell'ufficiale della Tempesta»). Evita ripetizioni prolungate come «delle Fiamme Immortali» o «dell'Ordine della Vipera Gemella». Nei gradi di Gridania il nome breve è «della Vipera», non «del Serpente» (`world/gcrankgridaniafemaletext.json#1:translation_name`). |
| Elm (material) | d'olmo | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Forma elisa obbligatoria (es. «tronco d'olmo», «tavola d'olmo», «macuahuitl d'olmo»). Evitare la forma non elisa «di olmo». |
| Snakestongue (gear) | Linguaserpe | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento composto a parola singola per la serie di equipaggiamento PvP (es. «elmo Linguaserpe», «lancia Linguaserpe»). Riferimento: `items/item.json#7378:name`. |
| Bearsmaw (gear) | Fauciorso | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento composto a parola singola per la serie di equipaggiamento PvP (es. «armetto Fauciorso», «ascia Fauciorso»). Riferimento: `items/item.json#7372:name`. |
| Hawkwing (gear) | Alafalco | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento composto a parola singola per la serie di equipaggiamento PvP (es. «armetto Alafalco», «artigli Alafalco»). Riferimento: `items/item.json#7366:name`. |
| Lionsmane (gear) | Chiomaleone | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento composto a parola singola per la serie di equipaggiamento PvP (es. «armetto Chiomaleone», «scudo Chiomaleone»). Riferimento: `items/item.json#7360:name`. |
| Birdsong (gear) | Cantaugello | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento composto a parola singola per la serie di equipaggiamento PvP (es. «maschera Cantaugello», «arco Cantaugello»). Riferimento: `items/item.json#7384:name`. Non applicare ai titoli dei rulli orchestrion. |
| Swansgrace (gear) | Graziacigno | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento composto a parola singola per la serie di equipaggiamento PvP (es. «cappuccio Graziacigno», «bastone Graziacigno»). Riferimento: `items/item.json#7390:name`. |
| Wolfseye (gear) | Occhiolupo | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento composto a parola singola per la serie di equipaggiamento PvP (es. «cappello Occhiolupo», «bastone Occhiolupo»). Riferimento: `items/item.json#7396:name`. |
| Elkhorn (gear) | Cornoalce | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento composto a parola singola per la serie di equipaggiamento PvP (es. «cappello Cornoalce», «grimorio Cornoalce»). Riferimento: `items/item.json#7402:name`. |
| Elktail (gear) | Codalce | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento composto a parola singola per la serie di equipaggiamento PvP (es. «cappello Codalce», «grimorio Codalce»). Riferimento: `items/item.json#7408:name`. |
| Hellwolf (gear) | Infernalupo | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento composto a parola singola per la serie di accessori PvP (es. «bracciale Infernalupo della Difesa»). Riferimento: `items/item.json#7414:name`. |
| Roseblood (gear) | Rosasangue | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento composto a parola singola per la serie di equipaggiamento PvP (es. «cappello Rosasangue», «abito Rosasangue»). Riferimento: `items/item.json#42661:name`. |
| Evenstar (gear) | Stellavespro | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento composto a parola singola per la serie di equipaggiamento (es. «cappello Stellavespro», «giubba Stellavespro»). Riferimento: `items/item.json#7340:name`. |
| Daystar (gear) | Stelladiurna | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento composto a parola singola per la serie di equipaggiamento (es. «diadema Stelladiurna», «veste Stelladiurna»). Riferimento: `items/item.json#7330:name`. |
| Gloam (gear) | Crepuscolo | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento per la serie di equipaggiamento (es. «cuffia Crepuscolo», «sopravveste Crepuscolo»). Riferimento: `items/item.json#7310:name`. |
| Auroral (gear) | Aurorale | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Adattamento aggettivale per la serie di equipaggiamento (es. «cuffia Aurorale», «bracciali Aurorali»). Riferimento: `items/item.json#7320:name`. |
| Hoplon (shield) | Oplone | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Termine canonico italiano per lo scudo rotondo (plur. «oploni», es. «oplone di bronzo», «oplone Noct»). Riferimento: `items/item.json#7350:name`. |
| Conquerer (weapon) | Conquistatore | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Ascia da guerra da Marauder/Warrior (es. «Conquistatore», «Conquistatore logoro»). Riferimento: `items/item.json#7353:name`. |
| Liberator (weapon) | Liberatore | Decisione dell'utente | Controllo automatico: suffisso equipaggiamento. Lancia da Dragoon (es. «Liberatore», «Liberatore logoro»). Riferimento: `items/item.json#7354:name`. |
| Burtgang | Burtgang | Decisione dell'utente | Nome proprio mitologico di spada leggendaria di origine antico-nordica/germanica (*Þiðreks saga*), conservato invariato. Riferimento: `items/item.json#7351:name`. |
| Lockbox (Eureka) | Forziere | Decisione dell'utente | Nome degli scrigni chiusi che si trovano nelle spedizioni Eureka; accordare al plurale «forzieri» e nei composti descrittivi. Riferimenti: `items/item.json#17341:name`; `items/item.json#17341:col_3`. |



### Razze e clan

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Hyur | Hyur | `world/race.json#1:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Elezen | Elezen | `world/race.json#2:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Lalafell | Lalafell | `world/race.json#3:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Miqo'te | Miqo'te | `world/race.json#4:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Roegadyn | Roegadyn | `world/race.json#5:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Au Ra | Au Ra | `world/race.json#6:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Hrothgar | Hrothgar | `world/race.json#7:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Viera | Viera | `world/race.json#8:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Midlander | Piancolle | `world/tribe.json#1:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Highlander | Montanaro | `world/tribe.json#2:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Wildwood | Silvano | `world/tribe.json#3:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Duskwight | Crepuscolare | `world/tribe.json#4:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Plainsfolk | Pratoverde | `world/tribe.json#5:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Dunesfolk | Dunagialla | `world/tribe.json#6:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Seeker of the Sun | Cercasole | `world/tribe.json#7:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Keeper of the Moon | Guardialuna | `world/tribe.json#8:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Sea Wolf | Lupo di Mare | `world/tribe.json#9:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Hellsguard | Guardinferno | `world/tribe.json#10:name_masculine` | I nomi composti da parole comuni si traducono come descrizioni italiane; non applicare questa regola ai nomi dei Lupi di Mare in lingua Roegadyn antica. Decisione dell'utente. |
| Raen | Raen | `world/tribe.json#11:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Xaela | Xaela | `world/tribe.json#12:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Helions | Eliano | `world/tribe.json#13:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| The Lost | Ramingo | `world/tribe.json#14:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Rava | Rava | `world/tribe.json#15:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Veena | Veena | `world/tribe.json#16:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |

### Parlato delle Società Alleate

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Sylph: this one / these ones | questo qui / questi qui | `dialogue/defaulttalk.json#590258:name` | Autoreferenza in terza persona; conservare il tratto e accordare al numero. |
| Sylph: walking one(s) | camminante / camminanti | `dialogue/defaulttalk.json#590270:name` | Appellativo per chi non è Sylph; evitare «camminatore», che suggerisce un mestiere o un ruolo. |
| Sahagin: sibilanti allungate | sss | `combat/companiontransient.json#61:col_2` | Decisione dell'utente: rendere con sibilanti allungate solo le parole marcate dalla fonte, senza allungare vocali a caso. |
| Ananta: sibilanti allungate | sss | `combat/companiontransient.json#262:col_2` | Decisione dell'utente, verificata sulle battute originali di Sarisha ed Eshana: seguire la stessa convenzione degli Sahagin. |
| Ondo: sibilanti allungate | sss | `dialogue/npcyell.json#8552:original` | Resa attestata nelle battute degli Ondo del Tempest; conservare il segnale quando è presente nella fonte. |
| Ixal: featherless one(s) | implume / implumi | `activities/instancecontenttextdata.json#13005:original` | Appellativo rivolto a chi non ha piume; mantenere la voce inversa degli Ixal quando è marcata, senza rendere il testo oscuro. |
| kupo | kupò | `dialogue/defaulttalk.json#592316:name` | Controllo automatico: intercalare. Per l'intercalare pronunciato usare la grafia «kupò»; parole citate, composti e nomi propri come «noce kupo» e «Uccell-kupo» restano invariati. |
| Vath: click / clack | clic / clac | `dialogue/balloon.json#5293:original` | Tradurre come segnali sonori quando sono testo visibile; non trattarli come tag SeString. |
| Namazu: yes, yes / no, no | sì, sì / no, no | `crafting/fishparameter.json#1246:name` | Conservare la ripetizione entusiasta se presente nella battuta; non aggiungere particelle a frasi che non la contengono. |
| Vanu Vanu: netherling | forestiero | `world/fateevent.json#249:name` | Appellativo per chi viene da fuori; mantenere la forma coerente con genere e numero della frase, senza renderlo «creatura infernale». |

### Luoghi

Catalogo integrale: `data/translations/world/placename.json`. Cercare il nome inglese nel campo `name` per ottenere la forma italiana `translation` e l'ID della fonte. Per un nome corto usare `scan --term "NOME"`.

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Seat of the First Bow | Seggio del Primo Arco | Decisione dell'utente | Nome del luogo; usare «Seggio» e non «Sede». Riferimenti: `world/placename.json#346:name`; `world/warp.json#131075:description`. Varianti ammesse nelle etichette: «Il Seggio del Primo Arco». |
| Baldesion Arsenal | Arsenale di Baldesion | Decisione dell'utente | Nome della spedizione; in prosa accordare articolo e preposizioni alla forma maschile («nell'Arsenale di Baldesion»). Riferimenti: `world/achievement.json#2227:description`; `activities/publiccontenttextdata.json#2132:original`. |
| Temenos Rookery | La Colombaia di Temenos | Decisione dell'utente | Nome della struttura di allevamento degli amaro nel Crystarium; usare la forma completa per questo luogo, mantenendo «Garzaia» per gli altri rookery. Riferimenti: `world/adventure.json#2162896:name`; `world/placename.json#3246:name`. |
| Slitherbough | Fendiscoglio | Decisione dell'utente | Toponimo; usare questa forma per il villaggio e per i riferimenti alla sua provenienza, adattando la frase. Riferimenti: `world/chocobotaxistand.json#1179718:original`; `world/placename.json#3170:name`. |
| Estersands | Sabbie Orientali | Decisione dell'utente | Regione del deserto di Dalmasca; sostituisce «Dune Orientali». La decisione non riguarda Westersands. Riferimento: `@review/dialogue/custom/004/reghinallaganstoragesystem_00491.json#73:description`. |
| Fields of Gold | Campi Dorati | Decisione dell'utente | Toponimo di Ok'hanu; sostituisce «Campi d'Oro». Riferimenti: `world/placename.json#4591:name`; `items/eventitemhelp.json#2003513:original`. Varianti ammesse nelle etichette: «I Campi Dorati». |
| Cracked Cistern | Cisterna Crepata | Decisione dell'utente | Toponimo; sostituisce «Cisterna Incrinata». Conservare l'articolo richiesto dalla frase o dall'etichetta. Riferimento: `world/placename.json#4605:name`. Varianti ammesse nelle etichette: «La Cisterna Crepata». |
| Zephyr Gate | Porta Zefiro | Decisione dell'utente | Toponimo; usare questa forma, con articoli e preposizioni richiesti dalla frase. Riferimenti: `world/placename.json#179:name`; `world/placename.json#735:name`. |
| The Salt Strand | La Riva Salata | Decisione dell'utente | Toponimo; usare questa forma, con articoli e preposizioni richiesti dalla frase. Riferimenti: `world/adventure.json#2162693:name`; `world/placename.json#338:name`. |
| The Hard Place | L'Incudine | Decisione dell'utente | Nome del locale di Adkiragh; usare «all'Incudine» nei riferimenti al luogo e «dell'Incudine» quando la frase richiede la preposizione articolata. Riferimenti: `world/placename.json#2096:name`; `dialogue/defaulttalk.json#595139:name`. |
| Archeo Alexandria | Archeo Alexandria | `world/placename.json#4676:name` | Conservare il toponimo ufficiale «Alexandria», senza adattarlo in «Alessandria». |
| The Source | La Fonte | `system/akatsukinotestring.json#33:original` | Nome del mondo; varianti ammesse in prosa: «la Sorgente», «Sorgente». |
| The Missing Gem | La Gemma Mancante | `world/placename.json#2267:name` | Nome dell'isola / settore esplorativo nel Diadema. |
| The One River | Fiumescelto | `crafting/spearfishingitem.json#20034:original` | Nome del fiume in Yanxia; forma preferita dall'utente. |
| The Ruby Sea | Il Mar Rubino | `world/contentfinderconditiontransient.json#235:original` | Nome del mare; varianti ammesse in prosa: «Mare di Rubino». |
| Minion Square | Piazza dei Minion | `world/placename.json#1578:name` | Area del Gold Saucer dedicata a Lord of Verminion; mantenere «Minion» invariato. |
| Beankeep | Assistente di Blunderville | Decisione dell'utente | Ruolo dell'incaricato di Blunderville; sostituisce «Custode dei Fagioli». Riferimento: `world/enpcresident.json#1046439:name`. |
| The Sea of Clouds | Il Mar di Nuvole | `world/contentfinderconditiontransient.json#11:original` | Nome del mare; varianti ammesse in prosa: «Mare di Nuvole». |
| Mt. Gulg | Monte Gulg | `world/contentfinderconditiontransient.json#659:original` | Forma usata in prosa; varianti ammesse per il nome autonomo: «Mt. Gulg». |
| Mt. Gulg | Mt. Gulg | `world/placename.json#2997:name` | Variante ammessa per il toponimo autonomo; in prosa usare «Monte Gulg». |
| The First | Il Primo Mondo | `world/mounttransient.json#170:description` | Nome del Primo Riflesso; varianti ammesse in prosa: «Prima Riflessione», «della Prima», «del Primo», «la Prima», «il Primo», «nel Primo». |
| The Interdimensional Rift | La Faglia Interdimensionale | `combat/companiontransient.json#345:description` | Nome del luogo; varianti ammesse in prosa: «Fenditura Interdimensionale». |
| The Tower of Zot | La Torre di Zot | `minigames/tripletriadcard.json#336:col_2` | Nome del luogo; varianti ammesse in prosa: «Torre di Babil». |
| The Warring Triad | Triade Guerriera | `minigames/tripletriadcard.json#168:col_2` | Nome del gruppo di tre eikon. |
| Dragonsong War | Guerra del Canto dei Draghi | Decisione dell'utente | Nome del conflitto ishgardiano; uniformare nei titoli e nella prosa riferiti allo stesso evento. Riferimenti: `activities/instancecontenttextdata.json#32621:original`; `system/opencontentcandidatename.json#33:original`. |
| The World of Darkness | Il Mondo dell'Oscurità | `dialogue/gimmickbill.json#60:original` | Nome del Tredicesimo; varianti ammesse in prosa: «Mondo delle Tenebre». |
| The Battlehall | La Sala d'Armi | `dialogue/customtalk.json#721265:col_31` | Nome del luogo; varianti ammesse in prosa: «Sala dei Duelli». |
| The Scholar | Erudito | `minigames/omikujiguidance.json#2501:description` | In questi testi indica l'archetipo del saggio; non il Job. Varianti ammesse in prosa: «studioso». |
| The Scholar | Lo Studioso | `world/placename.json#3826:name` | Nome del landmark di Thaliak nello Scholar's Harbor; variante approvata per il nome del luogo. |
| Hair Raid | Incursione dei Capelli | `combat/action.json#30076:original` | Nome di un'azione; variante ammessa in prosa: «Assalto di Capelli». |
| Wanderlust | Sete di Viaggio | `world/placename.json#2150:name` | Nome proprio; variante ammessa per il compagno: «Spirito Vagabondo». |
| Tessellation | Tassellazione | `system/addon.json#8678:original` | Opzione grafica; variante ammessa: «La Tassellatura». |
| Tessellation | La Tassellatura | `world/placename.json#3240:name` | Nome del ponte che collega l'Exarch Gate al Crystarium; variante approvata per l'opzione grafica. |
| Gatekeeper | Guardiano dei Cancelli | `world/title.json#207:description` | Titolo; variante ammessa in prosa: «Custode delle Porte». |
| The Rothlyt Sound | Stretto di Rothlyt | `minigames/tripletriadcard.json#10:col_2` | Nome dello stretto; varianti ammesse in prosa: «Stretto di Rothlyt». |
| The Aetherfont | La Fonte Eterica | `minigames/tripletriadcard.json#381:col_2` | Nome del luogo. |
| Pilgrim's Traverse | Traversata del Pellegrino | `world/placename.json#2150:name` | Nome proprio; variante ammessa in prosa: «Cammino del Pellegrino». |
| Kydonia Knolls | I Poggi di Kydonia | `combat/companiontransient.json#495:description` | Nome di una formazione rocciosa. |
| Moongate Hub | Lo Snodo della Porta Lunare | `crafting/wksplanetselect.json#0:original` | Nome della base lunare. |
| Maelstrom | La Tempesta | `world/placename.json#1775:name` | Nome della compagnia; come sostantivo comune, «maelstrom» si traduce «vortice». |
| Maelstrom | Vortice | `combat/action.json#7127:original` | Uso comune in un nome d'azione; la Compagnia usa «La Tempesta». |
| Camp Skull Valley | Campo Valle Teschio | `world/placename.json#224:name` | Nome completo della località; distinto da «Skull Valley». |
| Loading Station | Stazione di Carico | `world/placename.json#2599:name` | Varianti ammesse in prosa: «stazione», «stazioni», «stazioni di carico». |
| Camp Iron Lake | Campo Lago di Ferro | `world/placename.json#176:name` | Nome completo della località; distinto dal luogo «Iron Lake». |
| Camp Nine Ivies | Campo Nove Edere | `world/placename.json#74:name` | Nome completo della località; distinto dal luogo «Nine Ivies». |
| Camp Black Brush | Campo Sterpenero | `world/placename.json#248:name` | Nome completo della località; distinto dal luogo «Black Brush». |
| Camp Broken Water | Campo Acqua Spezzata | `world/placename.json#254:name` | Nome completo della località; distinto dal luogo «Broken Water». |

| The Tam-Tara Deepcroft | La Cripta di Tam-Tara | `world/placename.json#58:name` | Forma canonica applicata alle tre occorrenze. |
| Dzemael Darkhold | Fortezza Oscura di Dzemael | `world/placename.json#64:name` | Forma canonica applicata alle tre occorrenze. |
| Blue Badger Gate | Porta del Tasso Blu | `world/placename.json#87:name` | Forma canonica applicata a entrambe le occorrenze. |
| Naked Rock | Roccianuda | `world/placename.json#92:name` | Forma canonica applicata a entrambe le occorrenze. |
| The Fold | La Piega | `world/placename.json#5125:name` | Forma canonica applicata alle tre occorrenze. |
| Abalathia's Spine | Spina di Abalathia | `system/addon.json#1590:original` | Forma in questa frase; la voce PlaceName autonoma è «La Spina di Abalathia». |
| Crystal Tower Striker | Il Martello della Torre di Cristallo | `system/addon.json#9989:original` | Nome completo; non applicare separatamente il termine «Crystal Tower». |
| Island Sanctuary | Rifugio Insulare | `system/addon.json#1799:original` | Nome dell'area; in prosa usare le minuscole e adattare l'articolo. |
| Hideaway | Base dell'Isola | `system/descriptionstring.json#684:original` | Nel Rifugio Insulare indica la base operativa sud-occidentale dell'isola; variante ammessa in prosa: «base». |
| The Occult Crescent | Falce Occulta | `world/placename.json#4931:name` | Nome dell'area; variante ammessa in prosa: «Falce Occulta» quando l'articolo è retto da una preposizione. |
| The Occult Crescent | La Falce Occulta | `system/description.json#3604507:name` | Forma autonoma del nome dell'area. |
| Treasure Coffer | Forziere | `system/addon.json#17622:original` | In questi messaggi indica il forziere che contiene il tesoro. Varianti ammesse in prosa: «Scrigno del Tesoro». |
| Treasure Coffer | Scrigno del Tesoro | `system/addon.json#10113:original` | Variante usata come etichetta autonoma. |
| Residential Area | Zona Residenziale | `system/addon.json#8463:original` | Nome della zona; in prosa usare le minuscole. |
| Market Wards | quartiere mercantile | `system/addon.json#948:original` | Forma singolare contestuale dopo «alcun». |
| Market Wards | Rioni del Mercato | `world/placename.json#753:name` | Nome proprio della località; distinto dall'uso comune singolare. |
| Starward Standings | Spalti delle Stelle | `world/placename.json#2567:name` | Nome della località; la funzione usa «Classifica Stellare». |
| Abalathia's Spine | La Spina di Abalathia | `world/placename.json#497:name` | Forma autonoma del toponimo; nelle frasi può cadere l'articolo. |
| Skull Valley | Valleteschio | `world/adventure.json#2162716:col_2` | Forma canonica applicata a entrambe le occorrenze. |
| Via Praetoria | Via Praetoria | `world/placename.json#429:name` | Forma canonica applicata a entrambe le occorrenze. |
| Central Hall | Salone Centrale | `world/placename.json#5484:name` | Forma canonica applicata a entrambe le occorrenze. |
| Chocobokeep | Chocobiere | `world/placename.json#2315:name` | Forma canonica applicata a tutte le occorrenze. |
| Fang Cage | Gabbia della Zanna | `world/placename.json#1700:name` | Forma canonica applicata a tutte le occorrenze. |
| Claw Cage | Gabbia dell'Artiglio | `world/placename.json#1699:name` | Forma canonica applicata a tutte le occorrenze. |
| Inner Sanctum | Sancta Sanctorum | `world/placename.json#1560:name` | Forma canonica applicata a entrambe le occorrenze. |
| The Presence Chamber | Camera d'Udienza | `world/placename.json#872:name` | Forma canonica applicata a entrambe le occorrenze. |
| Third Floor | Terzo Piano | `world/placename.json#1529:name` | Forma canonica applicata a tutte le occorrenze. |
| Sohm Al Summit | Vetta Sohm Al | `world/placename.json#1010:name` | Forma canonica applicata a entrambe le occorrenze. |
| Dimwold | Selvacupa | `world/placename.json#1015:name` | Forma canonica applicata a entrambe le occorrenze. |
| Mirage Creek | Torrente Miraggio | `world/placename.json#1018:name` | Forma canonica applicata a entrambe le occorrenze. |
| The Slow Wash | La Lenta Corrente | `world/placename.json#1020:name` | Forma canonica applicata a entrambe le occorrenze. |
| The Sultana's Breath Subdivision | Quartieri del Respiro della Sultana | `world/placename.json#1191:name` | Forma canonica applicata a entrambe le occorrenze. |
| The Sultana's Breath | Il Respiro della Sultana | `world/placename.json#1211:name` | Forma canonica applicata a entrambe le occorrenze. |


| Old Sharlayan | Vecchia Sharlayan | `world/placename.json#3705:name` | Nome del luogo. In prosa usare la preposizione semplice: «di Vecchia Sharlayan» o «della Vecchia Sharlayan» (vietato l'uso di «del Vecchia Sharlayan»). |
| Netroot Radius | Raggio di Radicirete | `crafting/wksdevgrade.json#205:col_3` | Nome del luogo o della struttura. |
| Piercingway Tunnel | Galleria di Piercingway | `world/placename.json#5238:name` | Nome del luogo o della struttura, senza articolo nel campo nome. I nomi propri dei Loporrit terminanti in -way restano invariati; in prosa inserire l'articolo solo quando richiesto dalla sintassi italiana. |
| The Cosmoor | L'Ormeggio Cosmico | `crafting/wksmissiontext.json#89:original` | Nome del luogo o della struttura. |
| Zorgor the Boundless | Zorgor lo Sconfinato | `dialogue/gimmickbill.json#90:original` | Nome del luogo o della struttura. |
| Rhotano Sea | Mare di Rhotano | `world/adventure.json#2162693:col_2` | Nome del luogo o della struttura. |
| Castrum Abania | Castrum Abania | `world/adventure.json#2162850:col_2` | Nome del luogo o della struttura. |
| The Aurum Vale | Valdorata | `world/contentfinderconditiontransient.json#5:original` | Nome del luogo o della struttura. |
| The Matron's Lethe | Il Lete della Matrona | `world/contentfinderconditiontransient.json#49:original` | Nome del luogo o della struttura. |
| Bowl of Embers | Cratere delle Braci | `world/placename.json#357:name` | Nome del luogo. Maschile: «il Cratere delle Braci», «al Cratere delle Braci», «del Cratere delle Braci» (vietato l'uso dell'articolo femminile «alla Cratere»). |
| Windspath Gardens | Giardini del Cammino del Vento | `world/fateevent.json#690:col_6` | Nome del luogo o della struttura. |
| The Unmaking | La Disfazione | `world/mounttransient.json#444:description` | Nome del luogo o della struttura. |
| Bestways Burrow | Tana dei Modi Migliori | `world/placename.json#3966:name` | Nome del luogo o della struttura. |

| The Resonatorium | Il Risonatorio | `activities/mycwarresultnotebook.json#38:col_2` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Risonatorio». |
| Shin-Zantetsuken Containment Unit | Unità di Contenimento Shin-Zantetsuken | `activities/publiccontenttextdata.json#2126:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Unità di Contenimento della Zantetsuken Autentica». |
| Lance of Virtue Containment Unit | Unità di Contenimento Lancia della Virtù | `activities/publiccontenttextdata.json#2128:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Unità di Contenimento della Lancia della Virtù». |
| Proto Ozma Containment Unit | Unità di Contenimento Proto Ozma | `activities/publiccontenttextdata.json#2130:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Unità di Contenimento di Proto Ozma». |
| Smoldering Bedchamber | Camera da Letto Fumante | `activities/vvdvoteroutelabel.json#6:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Camera Ardente». |
| Sea-maid's Mirror | Specchio della Sirena | `activities/vvdvoteroutelabel.json#7:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Specchio della Fanciulla del Mare». |
| Goldsmiths' Guild | Gilda degli Orafi | `combat/companiontransient.json#4:description` | Nome del luogo o della struttura. Varianti ammesse in prosa: «capogilda degli Orafi». |
| Memeroon's Trading Post | La Locanda di Memeroon | `combat/companiontransient.json#26:description` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Bottega di Memeroon», «Bazar di Memeroon». |
| Saint Mocianne's Arboretum | Arboreto di Santa Mocianne | `combat/companiontransient.json#166:description` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Arboreto di Saint Mocianne». |
| The Antitower | L'Antitorre | `combat/companiontransient.json#178:description` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Torre dell'Antitour». |
| Leatherworkers' Guild | Gilda dei Conciatori | `combat/companiontransient.json#196:description` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Gilda dei Lavoratori di Cuoio». |
| The Sirensong Sea | Il Mar Canto | `combat/companiontransient.json#258:description` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Mare del Canto delle Sirene». |
| Hells' Lid | Tappinferno | `combat/companiontransient.json#279:description` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Bocca dell'Inferno». |
| The Sil'dihn Subterrane | I Sotterranei di Sil'dih | `combat/companiontransient.json#463:description` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Sotterranei di Sil'dih». |
| The Underworld | Il Regno Sotterraneo | `combat/companiontransient.json#477:description` | Varianti ammesse in prosa: «Inferi» per il luogo personificato, «inferi» in senso comune o idiomatico. |
| The Skydeep Cenote | Il Cenote Abissale | `combat/companiontransient.json#519:description` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Cenote delle Profondità Celesti». |
| Marsh Ligaka | Palude Ligaka | `crafting/spearfishingitem.json#20262:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «palude di Ligaka». |
| The Ut'ohmu Horizon | Orizzonte Ut'ohmu | `crafting/spearfishingitem.json#20267:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Orizzonte di Ut'ohmu». |
| Rooting Towerling | La Torretta Radicante | `crafting/wksdevgrade.json#232:col_2` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Torretta Radicante». |
| Capsule Chasm | Voragine delle Capsule | `crafting/wkstext.json#15:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Abisso Capsule». |
| Fusingway Vent | Condotto di Fusingway | `crafting/wkstext.json#16:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Fenditura di Fusingway». |
| Grandma Laurel | Nonna Alloro | `crafting/wkstext.json#29:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «nonna Laurel». |
| The Capitol | Il Campidoglio | `dialogue/contenttalk.json#208:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Capitale». |
| Mistwake | Scia delle Nebbie | `world/placename.json#5350:name` | Adattamento richiesto dall'utente. Nome della spedizione e del luogo; femminile: «nella Scia delle Nebbie», «della Scia delle Nebbie». Per equipaggiamento e creature usare «della Scia delle Nebbie». «Wake» interpretato come scia; la wiki attesta il contesto del Continente della Nebbia, non un'etimologia esplicita. Altra attestazione: `items/cabinetsubcategory.json#198:translation`. |
| The Meso Terminal | Il Terminale Meso | `world/placename.json#5266:name` | Nome del luogo o della struttura. In prosa si può usare «Terminale Meso» senza articolo quando la sintassi lo consente. |
| Full Bloom Gardens | Giardini della Piena Fioritura | `dialogue/pointmenu.json#18:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Giardini Fioriti». |
| Tomb of the Nullstone | Tomba della Pietra Nullificante | `minigames/tripletriadcard.json#152:col_2` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Tomba della Pietranulla». |
| Watts's Anvil | L'Incudine di Watts | `minigames/tripletriadcard.json#296:col_2` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Fucina di Watts», «Incudine di Watts». |
| Matoya's Relict | L'Eremo di Matoya | `minigames/tripletriadcard.json#306:col_2` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Relitto di Matoya». |
| The Promenade | Il Lungomare | `world/adventure.json#2162701:col_2` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Passeggiata». |
| The Fragrant Chamber | La Camera Profumata | `world/adventure.json#2162701:col_2` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Sala Profumata». |
| Wailing Barracks | Caserme Ululanti | `world/adventure.json#2162725:col_2` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Caserme dei Lamentosi». |
| The Trivium | Il Trivio | `world/adventure.json#2162894:col_2` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Trivio». |
| The Buttress | Il Contrafforte | `world/adventure.json#2162897:col_2` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Volta». |
| Dwarven Hollows | Gli Antri dei Nani | `world/adventure.json#2162913:col_2` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Cunicoli Nanici». |
| House of Winds High | L'Alta Casa dei Venti | `world/adventure.json#2162994:col_2` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Casa Alta dei Venti». |
| Tuliyollal Trailhead | L'Imbocco del Sentiero di Tuliyollal | `world/adventure.json#2163006:col_2` | Nome del luogo o della struttura. Varianti ammesse in prosa: «punto di partenza del sentiero di Tuliyollal». |
| Akh Afah Amphitheatre | L'Anfiteatro Akh Afah | `world/contentfindercondition.json#79:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Anfiteatro di Akh Afah». |
| Steps of Faith | Gradini della Fede | `world/contentfindercondition.json#169:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Ultimi Passi della Fede». |
| South Horn | Corno Sud | `world/contentfindercondition.json#1018:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Corno Meridionale». |
| North Horn | Corno Nord | `world/contentfindercondition.json#1093:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Corno Settentrionale». |
| The Haar | La Foschia | `world/contentfinderconditiontransient.json#23:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Foschia». |
| Naldiq & Vymelli's | Naldiq e Vymelli | `world/contentfinderconditiontransient.json#40:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Naldiq & Vymelli». |
| The Duergar Mountains | Montagne dei Duergar | `world/contentfinderconditiontransient.json#779:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Monti dei Duergar». |
| Royal Allagan Sunway | La Via Solare Reale Allagana | `world/fateevent.json#81:description` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Strada Solare Allagana Reale». |
| The Wanderer's Haven | Il Rifugio del Vagabondo | `world/mkdlore.json#46:description` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Rifugio del Viandante». |
| Secluded Stacks / The Secluded Stacks | Biblioteca Appartata | `world/placename.json#5196:name` | Sala bibliotecaria in cima alla Torre del Sangue, contenente tomi, tavolette e manoscritti. «Stacks» ha valore bibliotecario; non usare «cataste». In prosa: «biblioteca appartata», rispettando le maiuscole dell'originale. Altra attestazione: `world/mkdlore.json#30:translation_name`. |
| The Diadem | Il Diadema | `system/addon.json#8462:original` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Diadema». |
| Skybuilders' Board | Bacheca dei Costruttori Celesti | Decisione dell'utente | Nome proprio della bacheca dei lavori del Firmamento; al plurale «bacheche dei Costruttori Celesti». Riferimenti: `system/addon.json#13200:original`; `world/eobjname.json#2010775:name`. |
| Sagolii Desert | Deserto di Sagolii | `system/lobby.json#132:description` | Nome del luogo o della struttura. Varianti ammesse in prosa: «Deserto di Sagolii». |
| Irmin Hedge | Siepe di Irmin | `combat/action.json#26756:original` | Nome della barriera o evocazione protettiva vegetale. |
| The Forbidden Land, Eureka | Terra Proibita, Eureka | Decisione dell'utente | Nome del luogo; aggiungere il nome della regione quando la fonte specifica Anemos, Pagos, Pyros o Hydatos. Riferimenti: `system/addon.json#2305:original`; `world/contentfindercondition.json#581:original`. |
### Meteo

I nomi delle condizioni sono distinti dalle forme grammaticali usate nelle descrizioni.

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Showers | Rovesci | `world/weather.json#8:name` | Condizione meteorologica; distinta dalla variante «Rovesci passeggeri» attestata in un'altra voce meteo. |
| Thunder | Tuoni | `world/weather.json#9:name` | Condizione meteorologica al plurale; distinta dal nome dell'elemento «Tuono» nelle azioni. |
| Thunder | Tuono | `combat/action.json#144:original` | Nome dell'elemento nelle azioni; la condizione meteo usa il plurale «Tuoni». |
| Sandstorms | Tempesta di sabbia / Tempeste di sabbia | `world/weather.json#12:name` | Usare il singolare o il plurale secondo la frase. |
| Showers | Rovesci passeggeri | `world/weather.json#210:name` | Variante attestata in un'altra voce meteo. |

### Rifugio Insulare

Le voci identificano strutture, aree, visioni, missioni e mammets del Rifugio Insulare. I nomi degli NPC sono ruoli/nominativi di mammets specifici, non descrizioni generiche.

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Island Sanctuary Guide | Guida del Rifugio Insulare | `system/logmessage.json#10136:original` | Nome della guida di aiuto del Rifugio Insulare. |
| Landmarks | Punti di riferimento | Decisione dell'utente | Nel Rifugio Insulare indica le strutture costruibili dedicate; usare la stessa forma nelle etichette e in prosa. Riferimenti: `system/addon.json#14269:original`; `system/addon.json#15253:original`. |
| Sanctuary Rank [1] | Rango [1] del Rifugio Insulare | `world/achievement.json#3089:description` | Progressione dell'isola; con il numero usare «rango 10 del rifugio insulare». |
| Islekeep | Custode dell'Isola | `activities/mjitext.json#66:original` | Titolo del proprietario dell'isola; in prosa usare «custode dell'isola». |
| Cozy Cabin | Casetta Accogliente | `activities/mjitext.json#2:original` | Struttura principale dell'isola. |
| Cropland | Campo Coltivato | `activities/mjitext.json#62:original` | Area di coltivazione dell'isola. |
| Pasture | Pascolo | `activities/mjitext.json#63:original` | Area per gli animali catturati. |
| Mossy Mountains | Montagne Muschiose | `activities/mjitext.json#43:original` | Area di spedizione dei mammets del granaio. |
| Mountain Hollow | Cavità della Montagna | `activities/mjiprogress.json#39:description` | Area interna alla grotta dell'isola, raggiunta durante la progressione. |
| The Coziest of Cabins | La Casetta più Accogliente | `activities/mjiprogress.json#1:name` | Visione del Rifugio Insulare. |
| Top Crops | Coltivazioni Eccellenti | `activities/mjiprogress.json#7:name` | Visione legata alla costruzione del Campo Coltivato. |
| Adorable Animals | Animali Adorabili | `activities/mjiprogress.json#13:name` | Visione legata alla costruzione del Pascolo. |
| A Workshop of Wonders | Una Bottega delle Meraviglie | `activities/mjiprogress.json#20:name` | Visione legata alla costruzione dell'officina. |
| The Perfect Paradise | Il Paradiso Perfetto | `activities/mjiprogress.json#26:name` | Visione di sviluppo dell'isola. |
| Passionate Pioneering | Pionierismo Appassionato | `activities/mjiprogress.json#28:name` | Visione di sviluppo dell'isola. |
| Delightful Discovery | Una Scoperta Entusiasmante | `activities/mjiprogress.json#30:name` | Visione che sblocca la Cavità della Montagna. |
| The Land of Luxury | La Terra del Lusso | `activities/mjiprogress.json#35:name` | Visione di sviluppo dell'isola. |
| A Subterranean Investigation | Un'Indagine Sotterranea | `activities/mjiprogress.json#37:name` | Missione che porta alla Cavità della Montagna; distinta dalla visione «A Subterranean Expansion». |
| An Idyllic Island | Un'Isola Idilliaca | `activities/mjiprogress.json#42:name` | Visione conclusiva dello sviluppo dell'isola. |
| Felicitous Furball | Batuffolo Felice | `activities/mjiprogress.json#1:description` | Mammet che guida la squadra di aiutanti dell'isola. |
| Produce Producer | Curatore dei prodotti | `system/addon.json#14326:original` | Mammet che gestisce la coltivazione automatizzata; varianti ammesse in prosa: «coltivatore», «responsabile dei raccolti», «addetto alle colture». In `world/enpcresident.json#1043465:translation_name`, rispettare il minuscolo dell'originale: «curatore dei prodotti»; al plurale: «curatori dei prodotti». |
| Creature Comforter | Custode delle creature | `system/addon.json#15223:original` | Mammet che gestisce la cura degli animali del Pascolo; varianti ammesse in prosa: «responsabile degli animali», «addetto agli animali». In `world/enpcresident.json#1043466:translation_name`, rispettare il minuscolo dell'originale: «custode delle creature»; al plurale: «custodi delle creature». |
| Pathological Pathfinder | Esploratore Ossessivo | `activities/mjiprogress.json#21:description` | Mammet che libera ed espande il terreno; rende il tono comico del nome. |
| Determined Digger | Scavatore Determinato | `activities/mjiprogress.json#33:description` | Mammet addetto agli scavi nella Cavità della Montagna. |

### Titoli narrativi

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Once More, to the Ruby Sea | Ancora una volta, al Mar Rubino | `world/achievement.json#1788:description` | Titolo della missione principale che conduce al Mar Rubino. |
| It’s Probably Pirates | Probabilmente sono pirati | Decisione dell'utente | Titolo della missione; mantenere questa forma nei requisiti di sblocco. Riferimenti: `dialogue/defaulttalk.json#594798:col_2`; `dialogue/defaulttalk.json#596333:description`. |

### Nomi propri

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Yellowjackets | Giubbe Gialle | Decisione dell'utente | Nome della milizia di Limsa Lominsa; un membro è una «Giubba Gialla» al singolare e le «Giubbe Gialle» al plurale. Accordare articoli e aggettivi al contesto. Riferimenti: `world/bnpcname.json#981:description`; `world/enpcresident.json#1003285:description`. |
| Warrior of Darkness | Guerriero dell'Oscurità | Decisione dell'utente | Titolo riferito al personaggio giocante; accordare con il genere mediante If(gnum4), usando «Guerriera dell'Oscurità» al femminile. Riferimenti: `world/bnpcname.json#5240:name`; `world/enpcresident.json#1013810:name`. |
| Shadowkeeper | Custode delle Ombre | Decisione dell'utente | Personaggio femminile; usare l'articolo e gli accordi al femminile («la Custode»). Riferimenti: `combat/action.json#22304:original`; `world/bnpcname.json#9769:name`. |
| Faux Commander | Comandante Volpe | Decisione dell'utente | Nome proprio del personaggio e delle ricompense associate; usare «Comandante Volpe» al singolare e «Comandanti Volpe» al plurale. Riferimenti: `housing/characarddecoration.json#405:original`; `world/enpcresident.json#1028179:name`. |
| Berube skipper | Amaro Berube | Decisione dell'utente | Nome della razza di Skip; usare per l'animale, non per chi lo alleva. Riferimenti: `items/item.json#27251:col_2`; `@review/dialogue/custom/005/ctsdkbcharacter3_00569.json#26:description`. |
| Brute Bomber | Bombarolo Brutale | Decisione dell'utente | Nome del combattente dell'Arcadion; usare questa forma in dialoghi, messaggi, etichette e riferimenti al personaggio. Riferimenti: `housing/bannerbg.json#380:original`; `housing/bannerdesignpreset.json#361:original`. |

| Gods' Quiver | Faretra degli Dei | Decisione dell'utente | Nome della fazione; usare questa forma in tutto il corpus. Riferimenti: `world/bnpcname.json#621:name`; `world/bnpcname.json#1861:name`. |
| Convictors | Redento / Redenti | Decisione dell'utente | Nome della fazione e dei suoi membri: singolare «Redento», plurale «Redenti». Per la sede, usare «Rifugio dei Redenti». Riferimenti: `world/enpcresident.json#1015169:description`; `world/enpcresident.json#1015170:description`. |
| せーりーふー | せーりーふー | Decisione dell'utente | Lasciare la stringa in giapponese nei cinque campi `description` dei file custom: non ha una resa lessicale chiara e probabilmente non è usata nei file inglesi. |
| Nepto Dragon | Neptodrago | Decisione dell'utente | Nome della creatura; al plurale «Neptodraghi». Riferimenti: `items/item.json#8754:name`; `items/item.json#8754:col_3`. |
| Eastern alliance | Alleanza Orientale | Decisione dell'utente | Nome della coalizione; usare questa forma anche quando la frase richiede l'articolo o un'altra preposizione. Riferimenti: `world/bnpcname.json#10180:name`; `world/bnpcname.json#10180:description`. |
| Sultansworn | Giurati della Sultana | Decisione dell'utente | Nome della fazione; declinare «Giurato» e «Giurati» in base al referente, mantenendo il nome approvato. Riferimenti: `world/bnpcname.json#970:name`; `world/bnpcname.json#1911:name`. |
| Crystal Braves | Bravi di Cristallo | Decisione dell'utente | Nome della fazione; usare questa forma in prosa e nei nomi. Riferimenti: `world/enpcresident.json#1001881:description`; `world/enpcresident.json#1006967:description`. |
| Landsguard | Guardia Territoriale | Decisione dell'utente | Nome della forza di Tuliyollal; usare questa forma in dialoghi, nomi dei membri e testi di lore. Riferimenti: `world/enpcresident.json#1046791:name`; `world/enpcresident.json#1051928:name`. |
| Songstress of Ul'dah | Cantautrice di Ul'dah | Decisione dell'utente | Titolo dell'artista; mantenere «Cantautrice» nelle occorrenze pertinenti. Riferimenti: `dialogue/balloon.json#2882:original`; `dialogue/defaulttalk.json#600699:name`. |
| Allagan Empire | Impero Allagano | Decisione dell'utente | Nome dell'impero; accordare l'aggettivo al nome «Impero». Riferimenti: `dialogue/npcyell.json#18017:original`; `items/eventitemhelp.json#2000936:original`. |
| Wood Wailers | Sentinelle del Bosco | Decisione dell'utente | Nome della fazione di Gridania; distinguere i riferimenti alla fazione dai guardaboschi generici. Riferimenti: `world/enpcresident.json#1002841:description`; `world/enpcresident.json#1002842:description`. Per un membro usare «Sentinella del Bosco»; accordare al femminile grammaticale anche quando il referente è un uomo. Per il ruolo sentry: «Guardia delle Sentinelle del Bosco» (plurale «Guardie delle Sentinelle del Bosco»). Sostituisce «Guardaboschi», «Guardiaselva» e Wood Wailer quando indicano la fazione o i suoi membri. |
| Twelveswood | Bosco dei Dodici | Decisione dell'utente | Nome della foresta; usare «Bosco» nelle occorrenze pertinenti. Riferimenti: `minigames/racingchocoboname.json#1254:original`; `dialogue/defaulttalk.json#589867:name`. |
| Final Days | Ultimi Giorni | Decisione dell'utente | Nome dell'evento; uniformare «Ultimi Giorni» nelle occorrenze pertinenti. Riferimenti: `system/akatsukinotestring.json#8:original`; `system/akatsukinotestring.json#9:original`. |
| The Unending Journey | Il Viaggio Senza Fine | Decisione dell'utente | Nome della funzione e dei relativi oggetti; allineare il menu alla forma già presente in `items/item.json#13069:translation_name`. Riferimenti: `items/item.json#13069:col_3`; `system/addon.json#4800:original`. |
| Alexandrite | Alexandrite | Decisione dell'utente | Nome del catalizzatore; mantenere la grafia scelta anche nei nomi degli oggetti (es. «frammento di Alexandrite»). Riferimenti: `dialogue/pointmenustring.json#63:original`; `items/item.json#7883:col_3`. |
| Clutchfather Novv | Padre della Covata Novv | Decisione dell'utente | Titolo di Novv; usare «Covata» per il gruppo familiare, non «Branco». Riferimenti: `dialogue/defaulttalk.json#591280:name`; `combat/companiontransient.json#61:col_2`. |
| Heavensturn | Capodanno Celeste | Decisione dell'utente | Nome della ricorrenza; usare la forma canonica in prosa e nei nomi degli oggetti collegati. Riferimenti: `housing/orchestrion.json#83:name`; `shops/fittingshopcategory.json#102:original`. |
| Advent cake(s) (Capodanno Celeste) | Mochi di Capodanno | Decisione dell'utente | Nome dei dolci di riso tradizionali del Capodanno Celeste; in prosa usare «mochi di Capodanno», senza estendere la forma a mochi generici. Riferimenti: `items/eventitem.json#2001940:name`; `items/eventitem.json#2001940:col_2`. |
| Kobold(s) | koboldo / koboldi | Decisione dell'utente | Nome del popolo: «koboldo» al singolare, «koboldi» al plurale; accordare anche le forme aggettivali. Riferimenti: `minigames/tripletriadcard.json#17:name`; `world/enpcresident.json#1003859:name`. |
| Primal(s) | primale / primali | Decisione dell'utente | Nome delle entità evocate: «primale» al singolare e «primali» al plurale. Accordare l'articolo al genere della singola entità («il/la primale»; «i/le primali»). L'aggettivo generico *primal* può richiedere «primordiale» (es. «furia primordiale»), senza indicare un'entità evocata. «Primordiale» resta l'aggettivo comune quando la fonte dice *primordial/primeval* e non indica un Primale. Conservare «The Primals» nei nomi di gruppo, nei titoli musicali e nei crediti. Riferimenti: `minigames/racingchocoboname.json#1419:original`; `minigames/tripletriadcardtype.json#1:original`. |
| Somanoutics | Somanautica | Decisione dell'utente | Nome dell'arte del Saggio; usare «Somanautica» anche nei riferimenti alla disciplina, come «fondamenti della Somanautica». Riferimenti: `activities/instancecontenttextdata.json#32034:original`; `dialogue/defaulttalk.json#598424:name`. |
| Zodiac Weapon(s) | Arma dello Zodiaco / Armi dello Zodiaco | Decisione dell'utente | Nome della categoria; accordare «Arma/Armi» al numero e mantenere «dello Zodiaco». Riferimenti: `system/addon.json#2491:original`; `world/achievementcategory.json#63:original`. |
| Blackbosom | Cuorenero | Decisione dell'utente | Soprannome/cognome composto di Edda; mantenere la forma invariabile come nome proprio in tutti i testi, compresi NPC e personaggi secondari. Usare «Edda Cuorenero» e «di Cuorenero» secondo la sintassi. |
| Baderon Tenfingers | Baderon Diecidita | Decisione dell'utente | Soprannome composto: conserva il nome proprio Baderon e traduce «Tenfingers». Riferimento: `world/enpcresident.json#1011882:translation_name`. |
| Bert the Beloved | Bert il Benamato | Decisione dell'utente | Soprannome di Bert; usare questa forma nelle battute e nei riferimenti al personaggio. Riferimenti: `dialogue/defaulttalk.json#599452:name`; `dialogue/defaulttalk.json#598920:name`. |
| Chirpy Chocobo | Chocobo Cinguettante | Decisione dell'utente | Mascotte dei festeggiamenti (fête) della Ricostruzione di Ishgard («Tosatura dello Yak»). Riferimento: `dialogue/balloon.json#6318:translation`. |
| Mighty Moogle | Moguri Possente | Decisione dell'utente | Mascotte dei festeggiamenti (fête) della Ricostruzione di Ishgard («Fatti di Stoffa Morbida»). Riferimento: `dialogue/balloon.json#6317:translation`. |
| Perky Piggy | Porcellino Vispo | Decisione dell'utente | Mascotte dei festeggiamenti (fête) della Ricostruzione di Ishgard («Caccia al Balocco»). Riferimento: `dialogue/balloon.json#6304:translation`. |
| Iceheart | Cuore di Ghiaccio | Decisione dell'utente | Nome tradotto; al plurale «Cuori di Ghiaccio». Mantenere le maiuscole del nome proprio e scrivere «a molla» con la minuscola. |
| Coeurlfist | Pugno di Coeurl | Decisione dell'utente | Soprannome; tradurre solo «Fist» e conservare «Coeurl». |
| magitek avenger | vendicatore magitec | Decisione dell'utente | Nome comune di macchina; plurale «vendicatori magitec» e minuscolo nei campi che hanno la fonte minuscola. |
| white whittret | donnola bianca | Decisione dell'utente | Nome comune della creatura; concordare l'aggettivo al plurale («donnole bianche»). |
| bridesmoogle | moguri nuziale | Decisione dell'utente | Nome comune della creatura; «moguri» resta invariabile e l'aggettivo concorda («moguri nuziali» al plurale). |
| Redback | Dorsorosso | Decisione dell'utente | Nome proprio del minion; plurale «Dorsorossi» quando la fonte indica più esemplari. |
| Minitek | minitec | Decisione dell'utente | Adattamento di «magitek» usato come nome comune; rispettare il minuscolo salvo inizio frase. |
| bitty duckbill | ornitorinco minuscolo | Decisione dell'utente | Nome comune della creatura; concordare al plurale («ornitorinchi minuscoli»). |
| gimme kitten | gattino scroccone | Decisione dell'utente | Nome comune del minion; concordare al plurale («gattini scrocconi»). |
| puny mage | maghetto minuscolo | Decisione dell'utente | Nome comune; concordare al plurale («maghetti minuscoli»). |
| flame recruit | recluta della fiamma | Decisione dell'utente | Ruolo militare; mantenere «fiamma» minuscolo e concordare al plurale. |
| magic pot | vaso magico | Decisione dell'utente | Nome comune della creatura; usare «vasi magici» al plurale. |
| Ascian | Ascian / Asciani | Decisione dell'utente | Nome del popolo; singolare «Ascian», plurale «Asciani». |
| flame private | soldato semplice della fiamma | Decisione dell'utente | Grado militare; forme femminili «soldatessa semplice» e «soldatesse semplici»; «fiamma» minuscolo. |
| Silent Moss the Solemn | Muschio Silente il Solenne | Decisione dell'utente | Nome proprio e titolo; conservare maiuscole e ordine della resa. |
| glaring goodwife | massaia furiosa | Decisione dell'utente | Nome comune dell'NPC; concordare al plurale («massaie furiose»). |
| House Fortemps guard | Guardia della Casata Fortemps | Decisione dell'utente | Ruolo; usare la maiuscola nella denominazione autonoma, la minuscola nella prosa; «Casata Fortemps» è un nome proprio. |
| House Fortemps knight | Cavaliere della Casata Fortemps / Cavaliera della Casata Fortemps / Cavalieri della Casata Fortemps | Decisione dell'utente | Ruolo; concordare genere e numero («Cavaliere/Cavalieri», «Cavaliera/Cavalieri» secondo il referente). |
| adventurous frontier hand | lavoratore di frontiera avventuroso | Decisione dell'utente | Ruolo; concordare genere e numero con il referente. |
| Tailfeather hunter | Cacciatore di Codapiuma | Decisione dell'utente | Ruolo nei nomi autonomi; nella prosa usare la minuscola e concordare il numero («cacciatori»). Codapiuma resta un nome proprio. |
| moogle | moguri | Decisione dell'utente | Nome comune della specie, invariabile al plurale; minuscolo salvo inizio frase o nome proprio. Per «one-minded moogle», conservare l'aggettivo già presente: «moguri risoluto». |
| Imperial pilus prior | Primipilo imperiale | Decisione dell'utente | Grado militare; plurale «Primipili imperiali». |
| Serpent Commander Heuloix | Comandante della Vipera Heuloix | Decisione dell'utente | Titolo e nome proprio; conservare le maiuscole della forma del nome. |
| graverobber | tombarolo | Decisione dell'utente | Nome comune di ruolo; concordare al plurale («tombaroli»). |
| Skysteel engineer | Ingegnere di Ferrocielo | Decisione dell'utente | Ruolo nei nomi autonomi; plurale «Ingegneri di Ferrocielo». Nella prosa usare la minuscola. |
| Bombardier researcher | ricercatore dei Bombardieri | Decisione dell'utente | Ruolo; concordare genere e numero, mantenendo «Bombardieri» come nome proprio della compagnia. |
| Wood Wailer sentry | Guardia delle Sentinelle del Bosco | Decisione dell'utente | Ruolo; plurale «Guardie delle Sentinelle del Bosco». Sostituisce «Sentinella Guardabosco» e la proposta «Sentinella del Bosco di guardia». Riferimenti: `world/bnpcname.json#453:name`; `world/enpcresident.json#1041741:name`. |
| Bloody Executioner deckhand | Mozzo degli Esecutori Sanguinari | Decisione dell'utente | Ruolo; concordare «Mozzo/Mozzi» e conservare il nome della nave. |
| lightning sprite | elementale del fulmine | Decisione dell'utente | Nome comune della creatura; plurale «elementali del fulmine». |
| magitek | magitec | Decisione dell'utente | Adattamento italiano; mantenere invariabile e rispettare il maiuscolo/minuscolo della fonte. Anche il composto «Praemagitek» diventa «Praemagitec». |
| Alexandrian | Alexandrino | Decisione dell'utente | Aggettivo/etnonimo; declinare in genere e numero («Alexandrina», «Alexandrini», «Alexandrine»); lasciare «Alexandria» invariato come toponimo. |

| Ironheart | Cuordiferro | Decisione dell'utente | Cognome della famiglia; usare questa forma per Millith, Nedrick, Kester, Ulger e gli altri membri. Riferimenti: `world/enpcresident.json#1019464:name`; `world/enpcresident.json#1019464:description`. |
| Sphere scroll(s) | pergamena sferica / pergamene sferiche | Decisione dell'utente | Nome dell'oggetto; mantenere questa forma al singolare e al plurale. Riferimenti: `dialogue/customtalk.json#721067:col_31`; `system/howto.json#175:original`. |
| Zodiac glass | Lente dello Zodiaco | Decisione dell'utente | Nome dell'oggetto; al plurale «Lenti dello Zodiaco». Riferimenti: `items/item.json#8659:name`; `items/item.json#8659:col_3`. |
| Continental Circus | Circo Continentale | Decisione dell'utente | Nome della compagnia; usare la forma italiana nelle menzioni e nei nomi. Riferimenti: `world/enpcresident.json#1010179:col_2`; `world/enpcresident.json#1010180:col_2`. |
| Hippo Riders | Cavalcatori di Ippopotami | Decisione dell'utente | Nome del gruppo di corrieri thavnairiani; nelle menzioni usare «i Cavalcatori di Ippopotami». Riferimenti: `housing/bannerbg.json#156:original`; `housing/bannerdesignpreset.json#125:original`. |
| Scarlet Snout | Muso Scarlatto | Decisione dell'utente | Soprannome di Papà Gruff; sostituisce la forma inglese nei riferimenti allo stesso personaggio. Riferimento: `dialogue/defaulttalk.json#597222:name`. |
| Doomsday Clown | Pagliaccio del Giorno del Giudizio | `@review/dialogue/custom/007/feshlw2021actionguide_00737.json#8:description` | Titolo alternativo di Papà Gruff; usare questa forma quando la fonte presenta il soprannome. |
| Yes We Cant | Sì, Non Possiamo | Decisione dell'utente | Titolo della missione; conservare il gioco di parole scelto dall'utente. Riferimenti: `world/achievement.json#1027:description`; `system/lobby.json#642:original`. |
| Ceremony Planner (Claribel) | Organizzatrice della Cerimonia | Decisione dell'utente | Ruolo di Claribel, accordato al personaggio femminile; usare «organizzatrice» nei riferimenti al suo incarico. Riferimenti: `world/enpcresident.json#1009761:col_2`; `world/enpcresident.json#1010508:col_2`. |
| Bloody Executioners | Boia Sanguinari | Decisione dell'utente | Nome della fazione; usare questa forma nelle menzioni del gruppo. Riferimenti: `world/enpcresident.json#1010265:description`; `world/enpcresident.json#1010266:description`. |
| Durai Papers | Documenti dei Durai | Decisione dell'utente | Nome della testata/raccolta; usare questa forma nelle menzioni. Riferimenti: `dialogue/npcyell.json#7122:original`; `world/contentfinderconditiontransient.json#636:original`. |
| Majestic Theater Company | Compagnia Teatrale Majestica | Decisione dell'utente | Nome della compagnia; mantenere «Imperiale» quando la fonte include «Imperial». Riferimenti: `dialogue/defaulttalk.json#596749:name`; `dialogue/defaulttalk.json#596781:name`. |
| Mythril Eye | L'Occhio di Mithril | Decisione dell'utente | Nome del giornale; in prosa usare le forme contratte «dell'Occhio», «dall'Occhio». Riferimenti: `world/enpcresident.json#1016354:name`; `world/enpcresident.json#1018403:name`. |
| Velodyna Gatekeepers | Guardiani di Velodyna | Decisione dell'utente | Nome della fazione; per il ruolo individuale usare «Guardiano/Guardiana di Velodyna» secondo il personaggio. Riferimenti: `housing/bannerbg.json#185:original`; `housing/bannerdesignpreset.json#154:original`. |

### Eventi e termini di lore

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| The Twelve Archons | i Dodici Arconti | `dialogue/snipetalk.json#48:name` | Eroi della leggenda legata a Hatching-tide; la resa riguarda il gruppo storico. |
| Dragon Gay | drago allegro | Decisione dell'utente | Nel titolo della fiaba «The Boy and the Dragon Gay», gay significa «allegro»; usare «Il Ragazzo e il Drago Allegro», anche nei titoli derivati. Sostituisce «Drago Gay» e «Drago Gioioso». Riferimento: `world/eobjname.json#2007490:name`. |
| Sixth Umbral Calamity | Sesta Calamità Umbrale | Decisione dell'utente | Nome dell’evento storico; usare questa forma nelle menzioni della calamità. Riferimenti: `crafting/fishparameter.json#528:name`; `dialogue/defaulttalk.json#598467:name`. |
| The Echo | L'Eco | `combat/status.json#42:name` | Nome del potere dei Discendenti. |
| Endsinger | Cantamorte | Decisione dell'utente | Nome e titolo dell'entità femminile; usare «la Cantamorte» e «della Cantamorte». |
| Cloud of Darkness | Nube Oscura | Decisione dell'utente | Nome della regina delle creature del vuoto; usare «Nube Oscura» al singolare e «Nubi Oscure» per le manifestazioni plurali. Riferimenti: `minigames/tripletriadcard.json#140:name`; `world/bnpcname.json#3240:name`. |
| The Scions of the Seventh Dawn | Eredi della Settima Alba | `world/contentfinderconditiontransient.json#284:original` | Nome dell'organizzazione; usare la forma estesa al primo riferimento. Variante ammessa con articolo: «Gli Eredi della Settima Alba». |
| Little Ladies' Day | Giornata delle Piccole Dame | Decisione dell'utente | Controllo automatico: residuo. Nome della festa stagionale; accordare articoli e preposizioni alla forma femminile «Giornata» (es. «durante la Giornata delle Piccole Dame»). |
| Hatching-tide | Festa delle Uova | Decisione dell'utente | Nome della festa stagionale; riferimento contestuale: `world/enpcresident.json#1011803:translation_name`. Eccezione tecnica confermata dall'utente: in `world/fate.json#1594:translation_description`, `#1595:translation_description`, `#1856:translation_description`; `data/da_revisionare/dialogue/custom/006/fesest2020transform_00668.json#0,#2,#9,#11,#18:translation_description`; `data/da_revisionare/dialogue/custom/007/fesest2021guide_00712.json#0,#1,#4:translation_description`; `data/da_revisionare/dialogue/custom/007/fesest2021addpubenpc_00713.json#0,#11:translation_description`; e `data/da_revisionare/dialogue/custom/007/fesest2021recollection_00714.json#4,#39:translation_description`; `data/da_revisionare/dialogue/custom/008/fespdyest2024guid_00843.json#0,#4,#19-23,#26:translation_description`; `data/da_revisionare/dialogue/custom/009/fesest2025guide_00914.json#0-4:translation_description`; `data/da_revisionare/dialogue/custom/009/fesest2026guide_00959.json#0-4:translation_description`, rendere `Hatching<hex:021F0103>tide` come «Festa delle Uova», eliminando esclusivamente il tag del trattino `<hex:021F0103>`. L'eccezione vale soltanto per queste voci; conservare tutti gli altri tag. |
| Scholasticate | Il Seminario | Decisione dell'utente | Istituzione di Sant'Endalim; nei ruoli rispettare il minuscolo dell'originale: «docente del seminario», «diacono del seminario». Riferimento: `world/enpcresident.json#1011215:translation_name`. |
| Starlight Celebration | Celebrazione delle Stelle | Decisione dell'utente | Nome della festa stagionale; riferimento contestuale: `world/enpcresident.json#1010742:translation_name`. |
| The Masked Carnivale | Il Carnevale Mascherato | Decisione dell'utente | Nome dell'attività; mantenere «Carnevale Mascherato» nelle menzioni e accordare articoli/preposizioni. Riferimenti: `activities/contenttype.json#27:original`; `system/addon.json#8832:original`. |
| Starlight sentinel | sentinella delle Stelle | Decisione dell'utente | Nome dell’albero decorato per la Celebrazione delle Stelle; accordare genere e numero. Riferimenti: `items/item.json#13066:name`; `items/item.json#13066:col_3`. Varianti ammesse per gli arredi specifici: «abete sentinella della Celebrazione delle Stelle», «sentinella argentata della Celebrazione delle Stelle», «sentinella argentata autentica della Celebrazione delle Stelle». Gli aggettivi distinguono gli oggetti; non eliminarli per uniformare il nome. |
| Zodiac Braves | Eroi dello Zodiaco | Decisione dell'utente | Nome collettivo degli eroi di Ivalice; al singolare usare «Eroe dello Zodiaco». Riferimenti: `items/item.json#9500:col_2`; `items/item.json#9491:col_2`. |
| Soul attunement | sintonia spirituale | Decisione dell'utente | Termine per la progressione spirituale delle armi reliquia; usare questa forma nelle istruzioni e nei messaggi di stato. Riferimenti: `system/logmessage.json#4655:original`; `system/addon.json#3010:original`. |
| Festal Cant | Canto Festivo | Decisione dell'utente | Nome dell'incanto usato durante la parata. Riferimenti: `combat/action.json#2360:original`; `dialogue/npcyell.json#2305:original`. |
| Moonfire Faire | Festa del Fuoco Lunare | Decisione dell'utente | Nome della festa stagionale estiva; riferimento contestuale: `world/enpcresident.json#1009242:translation_col_2`. |
| Grand Chaperone | Gran Supervisore | Decisione dell'utente | Titolo di Haermaga alla Festa del Fuoco Lunare; usare «Gran Supervisore» anche nei dialoghi. Riferimenti: `world/enpcresident.json#1017199:col_2`; `dialogue/defaulttalk.json#601190:name`. |
| Eorzean Nimble Warrior course | percorso del Guerriero Agile Eorzeano | Decisione dell'utente | Nome del percorso acrobatico della Festa del Fuoco Lunare; usare «percorso del Guerriero Agile Eorzeano» nelle menzioni. Riferimenti: `dialogue/balloon.json#6893:original`; `dialogue/balloon.json#4021:original`. |
| Great White Tsuchinoko | Grande Tsuchinoko Bianco | Decisione dell'utente | Nome proprio della creatura leggendaria; accordare articoli e numero al contesto. Riferimenti: `items/item.json#41472:description`; `world/bnpcname.json#12612:description`. |
| The Rising | La Rinascita | Decisione dell'utente | Nome dell'evento annuale; nei ruoli usare «della Rinascita». Riferimento: `world/enpcresident.json#1009290:translation_name`. |
| All Saints' Wake | Veglia di Tutti i Santi | Decisione dell'utente | Nome della festa stagionale; riferimento contestuale: `world/enpcresident.json#1043978:translation_name`. |
| voidsent | creatura del vuoto (sing.), creature del vuoto (pl.) | Decisione dell'utente | Controllo automatico: residuo plurale. Nome comune della specie; rispettare il minuscolo dell'originale. Riferimento: `world/enpcresident.json#1042066:translation_name`. |
| Dawnservant | Custode dell'Alba | Decisione dell'utente | Titolo del sovrano di Tural; accordare articoli e preposizioni con «Custode» e mantenere minuscola la preposizione interna «dell'». Riferimenti: `combat/actiontransient.json#37129:translation`. |
| Beastkin | Bestiomorfi | Decisione dell'utente | Categoria tassonomica dei famigli; singolare «Bestiomorfo». In prosa usare il minuscolo quando il nome non è un'etichetta di categoria. Riferimenti: `combat/actiontransient.json#44895:translation`. |
| Vilekin | Vilomorfi | Decisione dell'utente | Categoria tassonomica dei famigli; singolare «Vilomorfo». In prosa usare il minuscolo quando il nome non è un'etichetta di categoria. Riferimenti: `combat/actiontransient.json#44895:translation`. |
| Cloudkin | Aeromorfi | Decisione dell'utente | Categoria tassonomica dei famigli; singolare «Aeromorfo». In prosa usare il minuscolo quando il nome non è un'etichetta di categoria. Riferimenti: `combat/actiontransient.json#44895:translation`. |
| Seedkin | Fitomorfi | Decisione dell'utente | Categoria tassonomica dei famigli; singolare «Fitomorfo». In prosa usare il minuscolo quando il nome non è un'etichetta di categoria. Riferimenti: `combat/actiontransient.json#44895:translation`. |
| Sunbeard | Barbasole | Decisione dell'utente | Nome della creatura, maschile invariabile: «il barbasole», «i barbasole», «del barbasole», «dei barbasole». Usare «barbasole» in prosa e nei nomi sorgente minuscoli; «Barbasole» quando la fonte ha l'iniziale maiuscola. Per bushy sunbeard: «barbasole folto», plurale «barbasole folti». Riferimenti: `world/fateevent.json#743:translation_name`; `world/fate.json#1865:translation_description`. |
| Wavekin | Idromorfi | Decisione dell'utente | Categoria tassonomica dei famigli; singolare «Idromorfo». In prosa usare il minuscolo quando il nome non è un'etichetta di categoria. Riferimenti: `combat/actiontransient.json#44895:translation`. |
| Scalekin | Squamorfi | Decisione dell'utente | Categoria tassonomica dei famigli; singolare «Squamorfo». In prosa usare il minuscolo quando il nome non è un'etichetta di categoria. Riferimenti: `combat/actiontransient.json#44895:translation`. |
| Soulkin | Animamorfi | Decisione dell'utente | Categoria tassonomica dei famigli; singolare «Animamorfo». In prosa usare il minuscolo quando il nome non è un'etichetta di categoria. Riferimenti: `combat/actiontransient.json#44895:translation`. |
| Ashkin | Necromorfi | Decisione dell'utente | Categoria tassonomica dei famigli; singolare «Necromorfo». In prosa usare il minuscolo quando il nome non è un'etichetta di categoria. Riferimenti: `combat/actiontransient.json#44895:translation`. |
| Spoken | Parlanti | `@review/dialogue/custom/009/ctsetcxbmloosefroxtalk_00992.json#14:description` | Categoria tassonomica per le creature capaci di parlare e farsi capire; usare «Parlanti» come etichetta e «parlante» in prosa. |
| Forgekin | Meccanomorfi | `@review/dialogue/custom/009/ctsetcxbmloosefroxtalk_00992.json#15:description` | Categoria tassonomica delle creature meccaniche; usare «meccanomorfo» al singolare e «meccanomorfi» al plurale. |

| Sundrop Dance | Danza della Goccia Solare | Decisione dell'utente | Nome della danza rituale dei Vanu Vanu; mantenere il nome scelto nelle battute. Riferimenti: `combat/companiontransient.json#172:col_3`; `world/emote.json#120:original`. |
| The Hounds of Ishgard | i Mastini di Ishgard | Decisione dell'utente | Nome della fazione; usare «i Mastini» in prosa. Riferimenti: `dialogue/defaulttalk.json#594136:name`. |
| Bombard(s) | Piro / Piros | Decisione dell'utente | Nome dei nemici della Festa del Fuoco Lunare; singolare «Piro», plurale «Piros». Non confondere con Bombardier/Bombardieri. Riferimenti: `world/bnpcname.json#1642:name`; `world/bnpcname.json#1642:description`. |
| Cascadiers | Cascadiers | Decisione dell'utente | Nome della fazione legata alla Festa del Fuoco Lunare; lasciarlo invariato. Riferimenti: `world/fate.json#532:col_3`; `world/fate.json#533:col_3`. |
| Saint of Nymeia | Santo di Nymeia | Decisione dell'utente | Titolo del personaggio della Celebrazione delle Stelle; tradurre anche nei riferimenti al Santo nel dialogo. Riferimenti: `world/enpcresident.json#1016274:name`; `world/enpcresident.json#1017823:name`. |
| the Spinner (Nymeia) | la Tessitrice | Decisione dell'utente | Epiteto della dea Nymeia; non applicare a spinner come esca da pesca o a usi comuni del verbo. Riferimenti: `items/item.json#43558:col_3`; `system/lobby.json#161:original`. |
| Valentione's Day | Giornata di Valentione | Decisione dell'utente | Nome della ricorrenza; usare questa forma nei riferimenti all'evento e nei nomi collegati. Riferimenti: `shops/fittingshopcategory.json#103:original`; `world/eobjname.json#2009385:name`. |
| Emissary of ardor | emissario dell’ardore / emissaria dell’ardore | `world/achievement.json#2484:description` | Ruolo della Casata Valentione; accordare «emissario/emissaria» al genere. Per “head emissary” usare «emissaria capo». Nuova traduzione: `data/da_revisionare/dialogue/custom/008/fesvlt2024guide_00844.json#17,#20,#23:description`. |
| Lifestream | Flusso Vitale | Decisione dell'utente | Termine di lore; accordare articoli e preposizioni alla forma maschile («nel Flusso Vitale», «del Flusso Vitale»). Riferimenti: `activities/ktgtypewordtextdata.json#609:name`; `activities/ktgtypewordtextdata.json#956:name`. |

### Distinzioni ricorrenti dal censimento

Rese attestate nelle correzioni del corpus: il qualificatore inglese delimita il contesto, non è testo da mostrare nel client. Il metodo di censimento è documentato in [Revisione](../../docs/REVISION.md#censimento-del-diff).

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Arms (heraldry) | Stemma | `housing/bannerdecoration.json#36:original` | In decorazioni e insegne Arms ha valore araldico: «Stemma del Conflitto di Bronzo 1», non «Armi». Non applicare ai nomi di armi o alle categorie di equipaggiamento. |
| Stack (effect) | Accumulo | `crafting/craftaction.json#100161:description` | Plurale «accumuli» per il numero di applicazioni di un effetto; evita «cumuli». Per pile di oggetti, accumuli fisici o il verbo to stack valuta il referente. |
| ATK | ATT | `combat/companiontransient.json#8:col_4` | Sigla della statistica Attacco nelle descrizioni dei minion; maschile: «l'ATT», «dell'ATT». Non abbreviare ogni uso narrativo di attack. |
| DEF | DIF | `combat/companiontransient.json#6:col_4` | Sigla della statistica Difesa nelle descrizioni dei minion; femminile: «la DIF», «della DIF». Non abbreviare ogni uso narrativo di defense. |
| Weaponskill | Tecnica d'arma | `combat/status.json#1346:description` | Singolare della categoria; nelle etichette brevi è ammesso «Tecnica». Non usare «abilità con armi». Varianti ammesse in prosa: «tecnica», «tecniche», «tecniche d'arma». |
| Potency scales with item level | La potenza varia in base al livello oggetto | `combat/actiontransient.json#41592:original` | Formula per la dipendenza della potenza dal livello oggetto; evita il calco «scala con». Non presumere una crescita lineare né trasferire la frase intera a descrizioni con condizioni diverse. |
| Range (level threshold) | Soglia | `combat/traittransient.json#35:original` | In Stealth/Furtività indica il livello massimo dei nemici evitabili: «Estende al livello 30 la soglia…». Per una distanza spaziale usa invece portata o raggio secondo il campo. |
| Sons of Saint Coinach | Figli di San Coinach | Decisione dell'utente | Nome dell'organizzazione di studiosi; per un membro «Figlio di San Coinach». Sostituisce «Figli/Figlio di Saint Coinach»; la scelta riguarda l'organizzazione e i suoi membri, non automaticamente ogni toponimo contenente Saint Coinach. Riferimento: `world/enpcresident.json#1011641:description`. |
| Anima Weapon | Arma Anima / Armi Anima | Decisione dell'utente | «Anima» è il nome proprio della reliquia; accordare «arma» al numero e agli aggettivi. Riferimenti: `items/item.json#13223:col_2`; `items/item.json#13225:col_2`. |
| Ultima Weapon | Arma Ultima | Decisione dell'utente | Nome della macchina da guerra allagana; in prosa usare l’articolo femminile «l’Arma Ultima». Riferimenti: `minigames/tripletriadcard.json#51:name`; `world/bnpcname.json#2137:name`. |
| Crystal Caravan | Carovana di Cristallo | Decisione dell'utente | Nome della compagnia teatrale; usare «Carovana di Cristallo» anche nei ruoli dei suoi membri. Riferimenti: `world/enpcresident.json#1017489:name`; `world/enpcresident.json#1017493:name`. |
| Ruby Tithe | Decima di Rubino | Decisione dell'utente | Tributo riscosso dalla Confederazione nel Mar Rubino. Riferimenti: `dialogue/defaulttalk.json#594010:name`; `minigames/tripletriadcard.json#214:col_2`. |
| Ruby Princess | Principessa di Rubino | Decisione dell'utente | Titolo della sovrana di Shisui; accordare il titolo nelle forme plurali. Riferimenti: `world/bnpcname.json#6241:name`; `activities/instancecontenttextdata.json#14903:original`. |
### Decisioni terminologiche: Ishgard e Firmamento

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Skybuilders | Costruttori Celesti | Decisione dell'utente | Nome del gruppo di artigiani e raccoglitori del Firmamento; usare «Costruttori Celesti» anche nei composti e nei riferimenti al gruppo. Riferimenti: `items/item.json#28731:col_3`; `items/item.json#28745:name`. |
| Skybuilders' Scrip(s) | Gettone dei Costruttori Celesti / Gettoni dei Costruttori Celesti | Decisione dell'utente | Valuta ottenuta contribuendo ai lavori del Firmamento; al plurale usare «Gettoni dei Costruttori Celesti». Riferimenti: `items/item.json#28063:name`; `items/item.json#28063:col_3`. |
| Heavens’ Ward | Guardia Celeste | Decisione dell'utente | Nome dell’ordine cavalleresco di Ishgard; per i membri usare «cavalieri della Guardia Celeste». Riferimenti: `housing/orchestrion.json#398:name`; `items/cabinetsubcategory.json#107:original`. |
| Ishgard Restoration | Ricostruzione di Ishgard | Decisione dell'utente | Nome del progetto e dell'attività; mantenere «Ricostruzione» nelle etichette e nelle menzioni della ricostruzione ufficiale. Riferimenti: `system/notebookdivision.json#2006:original`; `crafting/hwdinfoboardarticletransient.json#19:name`. |
| Skyrise Celebration | Celebrazione del Firmamento | Decisione dell'utente | Nome dell'evento che comprende le Fêtes; accordare articoli e preposizioni al femminile singolare. Riferimenti: `crafting/hwddevlevelwebtext.json#114:name`; `crafting/hwddevlevelwebtext.json#115:name`. |

### Nomi di oggetti e bevande

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Bozjan Clusters | Aggregati di Bozja | Decisione dell'utente | Valuta degli scontri critici; usare la forma scelta anche nel nome del menu e nei dialoghi. Riferimenti: `items/item.json#31135:description`; `system/addon.json#13822:original`. |
| Diamond Weapon | Arma di Diamante | Decisione dell'utente | Nome proprio della macchina da guerra; accordare al femminile. Riferimenti: `minigames/tripletriadcard.json#324:name`; `world/bnpcname.json#9953:name`. |
| Wicked White | Maledetto Bianco | Decisione dell'utente | Nome del cocktail; mantenere coerente il riferimento nelle battute dei clienti. Nuova traduzione `data/da_revisionare/dialogue/custom/006/regnor4girutt_00688.json#16,#43,#70,#73,#98:description`. |
| Alcoholic Beaverage | Bevanda al Castoro | Decisione dell'utente | Nome del cocktail; conservare la battuta sui castori. Nuova traduzione `data/da_revisionare/dialogue/custom/006/regnor4girutt_00688.json#17,#47,#74,#103:description`. |

### Terminologia aggiuntiva: Bozja ed Eulmore

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Boutique of Splendors | Boutique degli Splendori | Decisione dell'utente | Nome della boutique di Rowena; usare questa forma nei dialoghi e nelle descrizioni dei suoi incaricati e materiali. Riferimenti: `items/item.json#39766:col_2`; `items/item.json#38749:col_2`. |
| Resplendent Tool(s) | attrezzo splendente / attrezzi splendenti | Decisione dell'utente | Nome degli attrezzi da artigiano e dei relativi componenti; usare «Scambio degli Attrezzi Splendenti» per Resplendent Tool Exchange. Riferimenti: `shops/specialshop.json#1770333:original`; `world/enpcresident.json#1027566:col_2`. |
| Blade (grado della Resistenza Bozjana) | Lama | Decisione dell'utente | Titolo rivolto all'avventuriero nel contesto della Resistenza Bozjana; non applicare al nome «Lame di Gunnhildr» né agli usi comuni di blade. Riferimenti: `combat/action.json#36347:original`; `combat/action.json#36356:original`. |

### Terminologia Dawntrail confermata

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Earthenshire | Borgoargilla | Decisione dell'utente | Toponimo da scrivere unito e senza articolo; accordare le preposizioni al contesto. Riferimenti: `world/adventure.json#2162998:name`; `world/chocobotaxistand.json#1179740:original`. |
| Skyruin | Flagello Celeste | Decisione dell'utente | Epiteto di Valigarmanda; usare «il Flagello Celeste» nei riferimenti diretti, accordando articoli e preposizioni. Lasciare invariati i titoli musicali in `housing/orchestrion.json`, secondo la guida di traduzione. Riferimenti: `combat/action.json#36161:original`; `combat/action.json#36162:original`. |
| Ceruleum | ceruleo | Decisione dell'utente | Usare «ceruleo» quando indica il combustibile o il minerale, con maiuscola solo a inizio frase o nei titoli dei campi. Lasciare invariati i titoli musicali e il nome della tinta «Ceruleum Blue». Riferimenti: `system/addon.json#11358:original`; `system/akatsukinotestring.json#92:original`. |
| Xak Tural Railing | Ferrovia di Xak Tural | Decisione dell'utente | Nome dell'azienda ferroviaria; usare la forma completa alla prima menzione e «Ferrovia» quando il contesto è chiaro. Riferimenti: `items/item.json#44858:col_2`; `combat/companiontransient.json#539:description`. |
### Esplorazione Cosmica

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Artifact(s) (Esplorazione Cosmica) | Manufatto / manufatti | Decisione dell'utente | Reperti analizzati dai droni nelle rovine; usare «manufatto» al singolare e «manufatti» al plurale. Riferimenti: `world/eobjname.json#2015138:name`; `world/eobjname.json#2015138:description`. |
| Ancient Record(s) | Registro Antico / registri antichi | Decisione dell'utente | Nome dei dati recuperati dall’analisi dei manufatti; usare «registro antico» al singolare e «registri antichi» al plurale. Riferimenti: `items/item.json#50411:name`; `items/item.json#50411:col_3`. |
