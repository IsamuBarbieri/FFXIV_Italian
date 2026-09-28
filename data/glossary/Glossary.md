# Glossario approvato — FFXIV Italiano

Fonte terminologica per le traduzioni future. Le voci derivano solo dai file revisionati elencati sotto; le tabelle sono intenzionalmente selettive. I termini inglesi servono da riferimento e non vanno aggiunti tra parentesi nel testo di gioco.

**Catalogo completo dei luoghi:** tutte le 5.302 coppie `name`/`translation` di `data/translations/world/placename.json` fanno parte del glossario, con fonte `world/placename.json#ID:name`. Il file approvato è la tabella integrale; la sezione Luoghi qui sotto evidenzia anche le rese canoniche appena uniformate. Le voci che contengono tag richiedono una revisione SeString.

## File approvati

- `system/addon.json`
- `system/howto.json`
- `system/howtocategory.json`
- `system/lobby.json`
- `system/maincommand.json`
- `system/maincommandcategory.json`
- `world/classjob.json`
- `world/placename.json`
- `world/race.json`
- `world/tribe.json`
- `world/weather.json`

Per aggiungere un file: completarne la revisione, inserirlo in questo elenco e aggiungere solo termini riutilizzabili con una fonte `percorso#ID:campo`. Le varianti dello stesso termine restano righe separate con una nota di contesto. Forme grammaticali e frasi complete richiedono sempre una verifica nel contesto.

Per le **etichette e i testi di gioco**: la frase deve suonare naturale e grammaticalmente corretta in italiano; adattare articoli, preposizioni e accordi al contesto (per esempio «l'appello», «nella Piazza dei Chocobo», «a Vari Splendori»). Nelle etichette, conservare la maiuscola iniziale delle parole del nome inglese anche quando la resa le espande: Waymarks = Marcatori Tattici; Faux Hollows Available = Tane Sospette Disponibili. In prosa, rispettare le maiuscole del termine inglese, ignorando la sola maiuscola dovuta all'inizio frase; sigle e forme interamente maiuscole restano tali. Se il risultato non è grammaticale, correggere l'intera frase senza alterare il termine canonico.

Per le **categorie di attività**: Quest = Missione; Subquest = Missione secondaria; Main Scenario Quest = Missione dello Scenario Principale; Duty = Incarico; Levequest e Leve = Mandato; Trial = Prova; Raid = Incursione; Dungeon = Spedizione; Deep Dungeon = Cripta Profonda; Guildhest = Operazione di Gilda; FATE resta FATE anche al plurale. I qualificatori di categoria (per esempio Alliance, Savage, Extreme) si traducono in italiano. Le forme singolari non attestate nelle fonti sotto sono convenzioni editoriali, non voci approvate. Queste equivalenze valgono per le attività di gioco: *duty* come dovere, *trial* come processo o prova narrativa, *quest* come ricerca generica e i nomi propri richiedono una traduzione contestuale. Non sostituire automaticamente i nomi di istanze o luoghi.

Per le **fonoperle**: Linkshell = Fonoperla, Linkshells = Fonoperle; Cross-world Linkshell = Fonoperla Intermondo, Cross-world Linkshells = Fonoperle Intermondo. Nelle etichette numerate, `[1]`, `[2]` e così via indicano lo stesso schema; il numero resta invariato. In prosa usare la minuscola per i nomi comuni («una fonoperla intermondo», «le fonoperle»), adattando articoli, preposizioni e accordi. «Intermondo» qualifica le Fonoperle; Cross-world Party si traduce «Gruppo Intramondo». Le forme singolari senza numero sono convenzioni editoriali ricavate dalle etichette numerate, mentre le righe sotto riportano solo forme esattamente attestate. Nei nomi di funzione inglesi privi di articolo si può usare una forma italiana abbreviata senza articolo, se naturale: per esempio, Challenge Log = Registro Sfide.

Per il sistema **glamour**: usare «illusione» al singolare e «illusioni» al plurale, adattando articoli e accordi. Le funzioni composte hanno le forme attestate sotto: Comò delle Illusioni, Piastra d'Illusione e Prisma delle Illusioni. In prosa usare le minuscole. «Glamour» in un nome proprio o in un contesto diverso richiede verifica; la sostituzione automatica non basta.

## Voci

### Interfaccia e comandi

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Cancel | Annulla | `system/addon.json#2:original` |  |
| Close | Chiudi | `system/addon.json#1219:original` |  |
| Confirm | Conferma | `system/addon.json#572:original` |  |
| Yes | Sì | `system/addon.json#576:original` |  |
| No | No | `system/addon.json#577:original` |  |
| Item | Oggetto | `system/addon.json#355:original` |  |
| Inventory | Inventario | `system/addon.json#520:original` |  |
| Reward | Ricompensa | `system/addon.json#463:original` |  |
| Apply | Applica | `system/addon.json#1218:original` |  |
| Save | Salva | `system/addon.json#552:original` |  |
| Character | Personaggio | `system/addon.json#230:original` |  |
| Map | Mappa | `system/addon.json#467:original` |  |
| Party | Gruppo | `system/addon.json#176:original` |  |
| Teleport | Teletrasporto | `system/addon.json#186:original` |  |
| Configuration | Configurazione | `system/lobby.json#2:original` |  |
| Settings | Impostazioni | `system/addon.json#2660:original` |  |
| Options | Opzioni | `system/addon.json#465:original` |  |
| System | Sistema | `system/addon.json#1059:original` |  |
| Help | Aiuto | `system/addon.json#2624:original` |  |
| Search | Cerca | `system/addon.json#325:original` |  |
| Back | Indietro | `system/addon.json#2222:original` |  |
| Next | Avanti | `system/addon.json#9800:original` |  |
| Previous | Precedente | `system/addon.json#17797:original` |  |
| Accept | Accetta | `system/addon.json#168:original` |  |
| Decline | Rifiuta | `system/addon.json#169:original` |  |
| Delete | Elimina | `system/addon.json#68:original` |  |
| Select | Seleziona | `system/addon.json#3139:original` |  |
| Name | Nome | `system/addon.json#293:original` |  |
| Level | Livello | `system/addon.json#335:original` |  |
| Rank | Grado | `system/addon.json#732:original` |  |
| Equipment | Equipaggiamento | `system/addon.json#11333:original` |  |
| Gear | Equipaggiamento | `system/addon.json#852:original` |  |
| Crafting | Fabbricazione | `system/addon.json#273:original` |  |
| Gathering | Raccolta | `system/addon.json#276:original` |  |
| Fishing | Pesca | `system/addon.json#2342:original` |  |
| Movement | Movimento | `system/addon.json#1302:original` |  |
| Battle | Battaglia | `system/addon.json#663:original` |  |
| Quests | Missioni | `system/addon.json#454:original` |  |
| Main Scenario Quests | Missioni dello Scenario Principale | `system/howto.json#80:original` | Categoria delle missioni principali. |
| Items | Oggetti | `system/addon.json#1926:original` |  |
| Logs | Registri | `system/maincommand.json#38:name` |  |
| Travel | Viaggio | `system/maincommandcategory.json#4:original` |  |
| Return | Ritorna | `system/addon.json#1460:original` | Comando di ritorno. |
| Return | Indietro | `system/lobby.json#507:original` | Navigazione alla schermata precedente. |
| Return | Rientro | `system/maincommand.json#36:name` | Etichetta nominale contestuale. |
| Exit | Esci | `system/addon.json#2849:original` | Azione. |
| Exit | Uscita | `world/placename.json#528:name` | Etichetta nominale. |
| Duty | Incarichi | `system/maincommandcategory.json#2:original` | Categoria del menu; plurale di Incarico. |
| Duty | Incarico | `system/addon.json#2225:original` | Attività singola. Distinta dalle missioni delle quest. |
| Duties | Incarichi | `system/addon.json#15793:original` | Plurale del termine di gioco. |
| Duty Finder | Ricerca Incarichi | `system/maincommand.json#33:name` | Nome della funzione. |
| Duty Recorder | Registratore Incarichi | `system/maincommand.json#76:name` | Nome della funzione. |
| Hotbar | Barra Azioni | `system/addon.json#48527:original` | Nome dell'interfaccia; in prosa adattare articolo e numero. |
| Duty Support | Supporto Incarichi | `system/maincommand.json#91:name` | Nome della funzione. |
| Duty Roulette | Roulette Incarichi | `system/addon.json#8605:original` | Nome della funzione. |
| Party Finder | Ricerca Gruppo | `system/maincommand.json#57:name` | Nome della funzione di ricerca gruppi. |
| Hall of the Novice | Sala dei Novizi | `system/maincommand.json#70:name` | Nome della funzione. |
| Challenge Log | Registro Sfide | `system/maincommand.json#60:name` | Nome del registro; forma abbreviata senza articolo. |
| Challenge Log Rewards | Ricompense Registro Sfide | `system/addon.json#10116:original` | Etichetta abbreviata, senza articolo. |
| Heaven-on-High | Pilastro dei Cieli | `system/addon.json#4809:original` | Nome del Deep Dungeon. |
| Faux Hollows | Tane Sospette | `system/addon.json#11013:original` | Nome della funzione. |
| Sundry Splendors | Vari Splendori | `system/addon.json#13512:original` | Nome del negozio. |
| Mech Ops | Operazione Mech | `system/addon.json#16724:original` | Nome della modalità; al plurale nel testo: Operazioni Mech. |
| Crucible of the Unbroken | Crogiolo degli Infrangibili | `system/addon.json#17601:original` | Nome del contenuto. |
| Company Chest | Forziere della Compagnia | `system/addon.json#2880:original` | Nome della funzione della Compagnia Libera. |
| Rowena's House of Splendors | Casa degli Splendori di Rowena | `system/addon.json#6170:original` | Nome del negozio. |
| Hunting Log | Registro di Caccia | `system/maincommand.json#8:name` | Nome del registro. |
| Gathering Log | Registro di Raccolta | `system/maincommand.json#7:name` | Nome del registro. |
| Crafting Log | Registro di Fabbricazione | `system/maincommand.json#9:name` | Nome del registro. |
| Fishing Log | Registro di Pesca | `system/maincommand.json#29:name` | Nome del registro. |
| Fellowships | Confraternite | `system/maincommand.json#85:name` | Nome della funzione; singolare: Confraternita. |
| Fellowship Finder | Ricerca Confraternite | `system/maincommand.json#86:name` | Nome della funzione. |
| Strategy Board | Lavagna Strategica | `system/maincommand.json#98:name` | Nome della funzione. |
| Sightseeing Log | Diario Esplorazione | `system/maincommand.json#64:name` | Nome del registro. |
| Vault Oneiron | Volta Oneiron | `system/howto.json#289:original` | Nome del luogo. |
| Armoury Chest | Armeria | `system/maincommand.json#25:name` | Nome della funzione. |
| Chocobo Saddlebag | Bisaccia del Chocobo | `system/maincommand.json#77:name` | Nome della funzione. |
| Blue Magic Spellbook | Grimorio di Magia Blu | `system/maincommand.json#81:name` | Nome della funzione. |
| Aether Currents | Correnti Eteriche | `system/maincommand.json#67:name` | Nome della funzione. |
| Adventurer Plate | Scheda dell'Avventuriero | `system/maincommand.json#93:name` | Nome della funzione. |
| Shared FATE | FATE Condivisi | `system/maincommand.json#84:name` | Nome della funzione. |
| New Game+ | Nuova Partita+ | `system/maincommand.json#88:name` | Nome della modalità. |
| Waymarks | Marcatori Tattici | `system/maincommand.json#58:name` | Nome della funzione. |
| Ready Check | Appello | `system/maincommand.json#59:name` | Nome della funzione. |
| Record Ready Check | Registra Appello | `system/maincommand.json#79:name` | Funzione per registrare un appello prima di registrare un incarico. Varianti ammesse in prosa: «appello di registrazione», «appello per la registrazione». |
| PvP Profile | Profilo PvP | `system/maincommand.json#56:name` | Nome della funzione. |
| PvP Team | Squadra PvP | `system/maincommand.json#78:name` | Nome della funzione; in prosa rispettare le maiuscole dell'originale e adattare genere e numero («una squadra PvP», «le squadre PvP»). |
| Silence Echo | Silenzia Eco | `system/addon.json#12691:original` | Etichetta contestuale; distinta dal termine generico «Silence». |
| Starward Standings | Classifica Stellare | `system/addon.json#16716:original` | Etichetta della classifica; distinta dal nome dell'area «Spalti delle Stelle». |
| Mount Guide | Guida Cavalcature | `system/maincommand.json#61:name` | Nome della funzione. |
| Minion Guide | Guida Minion | `system/maincommand.json#62:name` | Nome della funzione. |
| Raid Finder | Ricerca Incursioni | `system/maincommand.json#72:name` | Nome della funzione. |
| V&C Dungeon Finder | Ricerca Spedizioni V&C | `system/maincommand.json#94:name` | Nome della funzione; comprende varianti e criterio. |
| Levequests | Mandati | `system/addon.json#8602:original` | Singolare: Mandato; tipo specifico di attività. |
| Leves | Mandati | `system/addon.json#8337:original` | Sinonimo di Levequests. |
| Trials | Prove | `system/addon.json#8608:original` | Singolare: Prova; attività istanziata. |
| Raids | Incursioni | `system/addon.json#8609:original` | Singolare: Incursione; attività istanziata. |
| Dungeons | Spedizioni | `system/addon.json#8335:original` | Singolare: Spedizione; attività istanziata. |
| Deep Dungeon | Cripta Profonda | `system/addon.json#2304:original` | Tipo distinto di attività. |
| Guildhests | Operazioni di Gilda | `system/addon.json#3165:original` | Singolare: Operazione di Gilda. |
| FATE | FATE | `system/addon.json#5768:original` | Sigla invariabile. |
| FATEs | FATE | `system/addon.json#8336:original` | Plurale senza punti. |
| Need | Necessità | `system/addon.json#390:original` | Scelta per l'assegnazione del bottino. |
| Greed | Brama | `system/addon.json#391:original` | Scelta per l'assegnazione del bottino. |
| Pass | Rinuncia | `system/addon.json#392:original` | Scelta per rifiutare il bottino. |
| Need, Greed, Pass | Necessità, Brama, Rinuncia | `system/addon.json#12693:original` | Nomi delle tre scelte nel sistema di distribuzione del bottino. Varianti ammesse in prosa: «Necessità, Brama e Rinuncia», «Solo Brama». |
| Aetheryte Tickets | Buoni Eterite | `system/addon.json#8501:original` | Oggetti per il teletrasporto; tradurre «Aetheryte» come «Eterite». |
| Aetheryte Ticket Usage | Uso dei Buoni Eterite | `system/addon.json#8522:original` | Etichetta della funzione. |
| Chocobo Porter | Noleggio Chocobo | `system/addon.json#2730:original` | Servizio di noleggio del Chocobo. |
| Levemete | Ufficiale degli Incarichi | `system/addon.json#678:original` | PNG che assegna Mandati; resa contestuale. |
| Advanced Materia Melding | Innesto Avanzato di Materia | `system/howto.json#99:original` | Nome della funzione di innesto avanzato. |
| Pet Glamours | Illusione dei Famigli | `system/howto.json#233:original` | Resa del sistema di glamour applicato ai famigli. |
| Moogle Treasure Trove | Forziere del Moguri | `system/addon.json#2778:original` | Nome dell'evento. |
| Happy Bunny | Coniglio Felice | `system/addon.json#9791:original` | Nome dell'attività con il coniglio. |
| Forked Towers | Torri Biforcate | `system/addon.json#16699:original` | Nome del contenuto; al singolare «Torre Biforcata». |
| Resident Caretaker | Custode Residenziale | `system/addon.json#3691:original` | Ruolo nella zona residenziale; in prosa usare le minuscole. |
| Crystalline Conflict | Conflitto Cristallino | `system/addon.json#5557:original` | Modalità PvP. |
| Frontline | Prima Linea | `system/addon.json#5558:original` | Modalità PvP; verificare le maiuscole nel contesto. |
| Elite Enemy | Nemico d'élite | `system/addon.json#17542:original` | Etichetta con numero dinamico; abbreviare il numero come «n.». |
| Rival Wings | Ali Rivali | `system/addon.json#5559:original` | Modalità PvP. |
| Triple Triad | Triple Triad | `system/addon.json#9529:original` | Nome invariato del minigioco. |
| Lord of Verminion | Lord of Verminion | `system/addon.json#9550:original` | Nome invariato del minigioco. |
| Gold Saucer | Gold Saucer | `system/addon.json#8612:original` | Nome invariato della destinazione. |
| The Gold Saucer | Gold Saucer | `world/placename.json#1484:name` | Nome della destinazione; variante ammessa in prosa: «Gold Saucer». |
| Leap of Faith | Salto della Fede | `system/addon.json#8475:original` | GATE del Gold Saucer. |
| The Hunt | La Caccia | `system/addon.json#838:original` | Funzione di gioco; distinguere dall'uso comune di hunt. |
| Treasure Hunt | Caccia al Tesoro | `system/addon.json#2276:original` | Modalità di gioco; distinguere dall'uso comune. |
| Journal | Diario di Viaggio | `system/addon.json#450:original` | Nome del menu. |
| Journal | Diario | `system/addon.json#593:original` | Etichetta breve. |
| All | Tutti | `system/addon.json#970:original` | Insieme di elementi. |
| All | Tutto | `system/howtocategory.json#1:original` | Categoria tutorial. |
| Battle | Combattimento | `system/howtocategory.json#4:original` | Categoria tutorial. |
| Crafting | Artigianato | `system/howtocategory.json#12:original` | Categoria tutorial. |
| Crafting | Creazione | `system/addon.json#13233:original` | Etichetta contestuale in addon. |

### Sistemi e funzioni ricorrenti

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Linkshells | Fonoperle | `system/addon.json#410:original` | Plurale; singolare editoriale: Fonoperla. |
| Linkshell [1] | Fonoperla [1] | `system/addon.json#4500:original` | Etichetta numerata: conservare il numero. |
| Cross-world Linkshells | Fonoperle Intermondo | `system/addon.json#12110:original` | Plurale delle fonoperle intermondo. |
| Cross-world Linkshell [1] | Fonoperla Intermondo [1] | `system/addon.json#4397:original` | Etichetta numerata: conservare il numero. |
| Cross-world Linkshell Invites | Inviti alle Fonoperle Intermondo | `system/addon.json#102636:original` | Inviti alla funzione intermondo. |
| Cross-world Party | Gruppo Intramondo | `system/addon.json#10902:original` | Gruppo formato tra mondi diversi. |
| Linkshell Distributor | Distributore di Fonoperle | `world/placename.json#1237:name` | Nome della funzione o del luogo. |
| Home World | Mondo d'Origine | `system/addon.json#4728:original` | Mondo di appartenenza del personaggio. |
| World Visit | Visita Mondo | `system/addon.json#12510:original` | Funzione di viaggio tra mondi. |
| Data Center | Data Center | `system/addon.json#10890:original` | Nome invariato della struttura server. |
| Housing | Alloggi | `system/addon.json#1999:original` | Sistema degli alloggi. |
| Glamours | Illusioni | `system/addon.json#16030:original` | Nome della funzione; in prosa «illusioni». Singolare editoriale: «illusione». |
| Glamour Dresser | Comò delle Illusioni | `system/addon.json#3735:original` | Arredo delle illusioni. In prosa «comò delle illusioni». |
| Glamour Plate | Piastra d'Illusione | `system/addon.json#3185:original` | Piastra che memorizza un insieme di illusioni. |
| Glamour Prism | Prisma delle Illusioni | `system/addon.json#5733:original` | Catalizzatore per applicare illusioni. |
| Cast Glamour | Applica Illusione | `system/addon.json#3700:original` | Comando per applicare un'illusione. |
| Apply Glamours | Applica Illusioni | `system/addon.json#10583:original` | Comando al plurale. |
| Glamour-ready | Pronto per l'illusione | `system/addon.json#5728:original` | Idoneità dell'oggetto; adattare genere e numero in prosa. |
| Outfit Glamour | Completo Illusione | `system/addon.json#15644:original` | Insieme di oggetti registrato come illusione. Varianti ammesse in prosa: «completi pronti per l'illusione». |
| Furnishing Glamours | Illusioni d'Arredo | `system/addon.json#15533:original` | Aspetti alternativi degli arredi; in prosa minuscolo. |
| Registered Glamours | Illusioni Registrate | `system/addon.json#15528:original` | Elenco delle illusioni d'arredo registrate. |
| Placed Glamours | Illusioni Posizionate | `system/addon.json#15539:original` | Illusioni d'arredo già posizionate. |
| Register Glamour | Registra Illusione | `system/addon.json#15525:original` | Registra l'aspetto alternativo di un arredo. |
| Place Glamour | Posiziona Illusione | `system/addon.json#15564:original` | Posiziona un'illusione d'arredo. |
| Store as Glamour | Conserva come Illusione | `system/addon.json#15622:original` | Salva un oggetto come illusione. |
| Armoire | Armadio | `system/addon.json#3734:original` | Arredo di deposito. |
| Retainer | Servitore | `system/addon.json#532:original` | Aiutante del personaggio; in prosa minuscolo. |
| Materia Melding | Innesto Materia | `system/addon.json#993:original` | Funzione per innestare materia. |
| Desynthesis | Desintesi | `system/addon.json#1815:original` | Funzione di smontaggio degli oggetti. |
| Aetherial Reduction | Riduzione Eterea | `system/addon.json#2160:original` | Funzione di riduzione. |
| Item Dyeing | Tintura Oggetti | `system/addon.json#4690:original` | Funzione di tintura dell'equipaggiamento. |
| Custom Deliveries | Consegne su Misura | `system/addon.json#5700:original` | Attività ricorrente. |
| Wondrous Tails | Code Meravigliose | `system/addon.json#5600:original` | Registro di attività. |
| Portraits | Ritratti | `system/addon.json#14650:original` | Funzione del personaggio. |
| Actions & Traits | Azioni e Tratti | `system/maincommand.json#3:name` | Menu del personaggio. |
| Blacklist | Lista Nera | `system/maincommand.json#14:name` | Elenco di giocatori bloccati. |
| Character Configuration | Configurazione Personaggio | `system/maincommand.json#34:name` | Menu delle impostazioni. |
| Collection | Collezione | `system/maincommand.json#87:name` | Menu della collezione. |
| Companion | Compagno | `system/maincommand.json#42:name` | Funzione del compagno. |
| Contacts | Giocatori Recenti | `system/maincommand.json#74:name` | Elenco dei contatti recenti. |
| Countdown | Conto alla Rovescia | `system/maincommand.json#73:name` | Funzione del gruppo. |
| Emotes | Emote | `system/maincommand.json#17:name` | Categoria di azioni sociali. |
| Fashion Accessories | Accessori di Moda | `system/maincommand.json#89:name` | Collezione di accessori. |
| Friend List | Lista Amici | `system/maincommand.json#13:name` | Elenco sociale. |
| HUD Layout | Disposizione HUD | `system/maincommand.json#22:name` | Funzione di disposizione dell'interfaccia. Varianti ammesse in prosa: «disposizione dell'HUD», «disposizione HUD», «disposizione delle barre azioni nell'HUD», «disposizione e dimensione dell'HUD». |
| Key Items | Oggetti Chiave | `system/maincommand.json#11:name` | Categoria di inventario. |
| Keybind | Assegnazione Tasti | `system/maincommand.json#20:name` | Configurazione dei comandi. |
| Mount Speed | Velocità Cavalcatura | `system/maincommand.json#75:name` | Funzione delle cavalcature. |
| Mute List | Lista Silenziati | `system/maincommand.json#96:name` | Nome del menu. Varianti ammesse in prosa: «lista dei personaggi silenziati». |
| Player Search | Ricerca Giocatori | `system/maincommand.json#15:name` | Ricerca di altri giocatori. |
| System Configuration | Configurazione di Sistema | `system/maincommand.json#19:name` | Menu delle impostazioni. |
| User Macros | Macro Utente | `system/maincommand.json#21:name` | Funzione delle macro. |
| Accessibility Settings | Impostazioni Accessibilità | `system/maincommand.json#63:name` | Impostazioni di accessibilità. |
| Graphics Settings | Impostazioni Grafiche | `system/maincommand.json#47:name` | Impostazioni video. |
| Sound Settings | Impostazioni Audio | `system/maincommand.json#46:name` | Impostazioni audio. |
| UI Settings | Impostazioni Interfaccia | `system/maincommand.json#52:name` | Impostazioni dell'interfaccia. |
| Item Settings | Impostazioni Oggetti | `system/maincommand.json#71:name` | Impostazioni degli oggetti. |
| Mouse Settings | Impostazioni Mouse | `system/maincommand.json#48:name` | Impostazioni del mouse. Varianti ammesse in prosa: «impostazioni di [dispositivo] e del mouse» quando il dispositivo è indicato dal tag dinamico. |
| Theme Settings | Impostazioni Tema | `system/maincommand.json#83:name` | Impostazioni grafiche. |
| Configuration Sharing | Condivisione Configurazione | `system/maincommand.json#99:name` | Condivisione delle impostazioni. |
| Party Members | Membri del Gruppo | `system/maincommand.json#12:name` | Nome dell'elenco. Varianti ammesse in prosa: «membri», «tutti i membri» (quando il riferimento al gruppo è chiaro). |
| Sanctuary Crafting Log | Registro di Fabbricazione del Rifugio Insulare | `system/addon.json#14259:original` | Nome del registro di fabbricazione del Rifugio Insulare. |
| Sanctuary Gathering Log | Registro di Raccolta del Rifugio Insulare | `system/addon.json#14260:original` | Nome del registro di raccolta del Rifugio Insulare. |
| Currency | Valute | `system/maincommand.json#66:name` | Nome del menu; altre etichette usano il singolare «Valuta». |

### Termini di gioco

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Free Company | Compagnia Libera | `system/addon.json#646:original` |  |
| Grand Company | Grande Compagnia | `system/addon.json#337:original` |  |
| Subleader | Vice | `system/addon.json#38155:original` | Ruolo nella squadra PvP; in prosa minuscolo «vice». |
| Subleaders | Vice | `system/addon.json#38047:original` | Plurale invariabile; in prosa «i vice». |
| Aetheryte | Eterite | `system/addon.json#8511:original` |  |
| Aethernet | Eternet | `system/addon.json#2720:original` |  |
| Gil | Gil | `system/addon.json#830:original` |  |
| The Black Shroud | Velo Nero | `system/addon.json#1578:original` |  |
| Chocobo | Chocobo | `system/addon.json#9081:original` |  |
| Retainer | Servitore | `system/addon.json#532:original` |  |
| Materia | Materia | `system/addon.json#481:original` |  |
| Mount | Cavalcatura | `system/addon.json#774:original` |  |
| Minion | Minion | `system/addon.json#8303:original` |  |
| Achievement | Obiettivo | `system/addon.json#1484:original` |  |
| Quest | Missione | `system/addon.json#12723:original` | Categoria generale, singolare. |
| Job | Job | `system/addon.json#684:original` |  |
| Class | Classe | `system/addon.json#869:original` |  |
| HP | PV | `system/addon.json#1000:original` |  |
| MP | PM | `system/addon.json#724:original` |  |
| Retainer | Retainer | `system/addon.json#12580:original` | Termine conservato in una specifica etichetta. |
| Mount | Monta | `system/addon.json#4964:original` | Azione contestuale. |
| Mount | Pilota | `system/addon.json#11382:original` | Azione contestuale. |
| Weaponskills | Tecniche | `system/addon.json#14484:original` | Categoria di azione; singolare: Tecnica. |
| HP | PV | `system/addon.json#232:original` | Sigla conservata in un'etichetta. |
| MP | PM | `system/addon.json#233:original` | Sigla conservata in un'etichetta. |

### Classi e mestieri

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Adventurer | Avventuriero | `world/classjob.json#0:name` |  |
| Gladiator | Gladiatore | `world/classjob.json#1:name` |  |
| Pugilist | Pugile | `world/classjob.json#2:name` |  |
| Marauder | Incursore | `world/classjob.json#3:name` |  |
| Lancer | Lanciere | `world/classjob.json#4:name` |  |
| Archer | Arciere | `world/classjob.json#5:name` |  |
| Conjurer | Incantatore | `world/classjob.json#6:name` |  |
| Thaumaturge | Taumaturgo | `world/classjob.json#7:name` |  |
| Carpenter | Falegname | `world/classjob.json#8:name` |  |
| Blacksmith | Fabbro | `world/classjob.json#9:name` |  |
| Armorer | Armaiolo | `world/classjob.json#10:name` |  |
| Goldsmith | Orefice | `world/classjob.json#11:name` |  |
| Leatherworker | Conciatore | `world/classjob.json#12:name` |  |
| Weaver | Tessitore | `world/classjob.json#13:name` |  |
| Alchemist | Alchimista | `world/classjob.json#14:name` |  |
| Culinarian | Cuoco | `world/classjob.json#15:name` |  |
| Miner | Minatore | `world/classjob.json#16:name` |  |
| Botanist | Botanico | `world/classjob.json#17:name` |  |
| Fisher | Pescatore | `world/classjob.json#18:name` |  |
| Paladin | Paladino | `world/classjob.json#19:name` |  |
| Monk | Monaco | `world/classjob.json#20:name` |  |
| Warrior | Guerriero | `world/classjob.json#21:name` |  |
| Dragoon | Dragone | `world/classjob.json#22:name` |  |
| Bard | Bardo | `world/classjob.json#23:name` |  |
| White Mage | Mago Bianco | `world/classjob.json#24:name` |  |
| Black Mage | Mago Nero | `world/classjob.json#25:name` |  |
| Arcanist | Arcanista | `world/classjob.json#26:name` |  |
| Summoner | Evocatore | `world/classjob.json#27:name` |  |
| Scholar | Studioso | `world/classjob.json#28:name` |  |
| Rogue | Furfante | `world/classjob.json#29:name` |  |
| Ninja | Ninja | `world/classjob.json#30:name` |  |
| Machinist | Artificiere | `world/classjob.json#31:name` |  |
| Dark Knight | Cavaliere Oscuro | `world/classjob.json#32:name` |  |
| Astrologian | Astrologo | `world/classjob.json#33:name` |  |
| Samurai | Samurai | `world/classjob.json#34:name` |  |
| Red Mage | Mago Rosso | `world/classjob.json#35:name` |  |
| Blue Mage | Mago Blu | `world/classjob.json#36:name` |  |
| Gunbreaker | Eterlama | `world/classjob.json#37:name` |  |
| Dancer | Danzatore | `world/classjob.json#38:name` |  |
| Reaper | Mietitore | `world/classjob.json#39:name` |  |
| Sage | Saggio | `world/classjob.json#40:name` |  |
| Viper | Vipera | `world/classjob.json#41:name` |  |
| Pictomancer | Pittomante | `world/classjob.json#42:name` |  |
| Beastmaster | Domatore | `world/classjob.json#43:name` |  |

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
| Hellsguard | Guardinferno | `world/tribe.json#10:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Raen | Raen | `world/tribe.json#11:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Xaela | Xaela | `world/tribe.json#12:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Helions | Eliano | `world/tribe.json#13:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| The Lost | Ramingo | `world/tribe.json#14:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Rava | Rava | `world/tribe.json#15:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |
| Veena | Veena | `world/tribe.json#16:name_masculine` | Forma del nome; controllare gli accordi nel contesto. |

### Luoghi

Catalogo integrale: `data/translations/world/placename.json`. Cercare il nome inglese nel campo `name` per ottenere la forma italiana `translation` e l'ID della fonte. Per un nome corto usare `scan --term "NOME"`.

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Vesper Bay | Baia del Vespro | `world/placename.json#274:name` |  |
| Aleport | Portobirra | `world/placename.json#223:name` |  |
| Wineport | Portovino | `world/placename.json#216:name` |  |
| Quarrymill | Cavamulino | `world/placename.json#129:name` |  |
| The Waking Sands | Sabbie del Risveglio | `world/placename.json#356:name` |  |
| The Rising Stones | Le Pietre Risorte | `world/placename.json#481:name` |  |
| Revenant's Toll | Pedaggio del Redivivo | `world/placename.json#411:name` |  |
| Limsa Lominsa | Limsa Lominsa | `world/placename.json#27:name` |  |
| Gridania | Gridania | `world/placename.json#39:name` |  |
| Ul'dah | Ul'dah | `world/placename.json#51:name` |  |
| Ishgard | Ishgard | `world/placename.json#62:name` |  |
| Eorzea | Eorzea | `world/placename.json#21:name` |  |
| La Noscea | La Noscea | `world/placename.json#22:name` |  |
| Central Shroud | Velo Centrale | `world/placename.json#54:name` |  |
| East Shroud | Velo Orientale | `world/placename.json#55:name` |  |
| South Shroud | Velo Meridionale | `world/placename.json#56:name` |  |
| North Shroud | Velo Settentrionale | `world/placename.json#57:name` |  |
| Middle La Noscea | La Noscea Centrale | `world/placename.json#30:name` |  |
| Lower La Noscea | La Noscea Inferiore | `world/placename.json#31:name` |  |
| Upper La Noscea | La Noscea Superiore | `world/placename.json#34:name` |  |
| Western La Noscea | La Noscea Occidentale | `world/placename.json#33:name` |  |
| Eastern La Noscea | La Noscea Orientale | `world/placename.json#32:name` |  |
| Outer La Noscea | Noscea Esterna | `world/placename.json#350:name` |  |
| The Drowning Wench | La Fanciulla Annegata | `world/placename.json#715:name` |  |
| The Quicksand | Le Sabbie Mobili | `world/placename.json#615:name` |  |
| Buscarron's Druthers | Il Capriccio di Buscarron | `world/placename.json#119:name` |  |
| Camp Drybone | Campo Ossasecca | `world/placename.json#300:name` |  |
| Camp Overlook | Campo Belvedere | `world/placename.json#237:name` |  |
| Bronze Lake | Lago di Bronzo | `world/placename.json#177:name` |  |
| Whitebrim | Orlo Bianco | `world/placename.json#383:name` |  |
| Bentbranch Meadows | Prati di Ramostorto | `world/placename.json#94:name` |  |
| Copperbell Mines | Miniere di Camparame | `world/placename.json#48:name` |  |
| The Tam-Tara Deepcroft | La Cripta di Tam-Tara | `world/placename.json#58:name` | Forma canonica applicata alle tre occorrenze. |
| Dzemael Darkhold | Fortezza Oscura di Dzemael | `world/placename.json#64:name` | Forma canonica applicata alle tre occorrenze. |
| Blue Badger Gate | Porta del Tasso Blu | `world/placename.json#87:name` | Forma canonica applicata a entrambe le occorrenze. |
| Naked Rock | Roccianuda | `world/placename.json#92:name` | Forma canonica applicata a entrambe le occorrenze. |
| The Fold | La Piega | `world/placename.json#5125:name` | Forma canonica applicata alle tre occorrenze. |
| Abalathia's Spine | Spina di Abalathia | `system/addon.json#1590:original` | Forma in questa frase; la voce PlaceName autonoma è «La Spina di Abalathia». |
| Crystal Tower Striker | Il Martello della Torre di Cristallo | `system/addon.json#9989:original` | Nome completo; non applicare separatamente il termine «Crystal Tower». |
| Island Sanctuary | Rifugio Insulare | `system/addon.json#1799:original` | Nome dell'area; in prosa usare le minuscole e adattare l'articolo. |
| The Occult Crescent | Falce Occulta | `world/placename.json#4931:name` | Nome dell'area; variante ammessa in prosa: «Falce Occulta» quando l'articolo è retto da una preposizione. |
| Treasure Coffer | Forziere | `system/addon.json#17622:original` | In questi messaggi indica il forziere che contiene il tesoro. Varianti ammesse in prosa: «Scrigno del Tesoro». |
| Residential Area | Zona Residenziale | `system/addon.json#8463:original` | Nome della zona; in prosa usare le minuscole. |
| Market Wards | quartiere mercantile | `system/addon.json#948:original` | Forma singolare contestuale dopo «alcun». |
| Skull Valley | Valleteschio | `world/placename.json#171:name` | Forma canonica applicata a entrambe le occorrenze. |
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

### Meteo

I nomi delle condizioni sono distinti dalle forme grammaticali usate nelle descrizioni.

| Inglese | Italiano | Fonte | Uso |
| --- | --- | --- | --- |
| Clear Skies | Cielo Sereno | `world/weather.json#1:name` |  |
| Fair Skies | Cielo Limpido | `world/weather.json#2:name` |  |
| Clouds | Nuvoloso | `world/weather.json#3:name` |  |
| Fog | Nebbia | `world/weather.json#4:name` |  |
| Wind | Vento | `world/weather.json#5:name` |  |
| Gales | Raffiche di vento | `world/weather.json#6:name` |  |
| Rain | Pioggia | `world/weather.json#7:name` |  |
| Showers | Rovesci | `world/weather.json#8:name` |  |
| Thunder | Tuoni | `world/weather.json#9:name` |  |
| Thunderstorms | Tempesta di fulmini | `world/weather.json#10:name` |  |
| Dust Storms | Tempesta di Polvere | `world/weather.json#11:name` |  |
| Sandstorms | Tempesta di sabbia | `world/weather.json#12:name` |  |
| Hot Spells | Ondata di Caldo | `world/weather.json#13:name` |  |
| Heat Waves | Ondata di Calore Torrido | `world/weather.json#14:name` |  |
| Snow | Neve | `world/weather.json#15:name` |  |
| Blizzards | Bufera di neve | `world/weather.json#16:name` |  |
| Showers | Rovesci passeggeri | `world/weather.json#210:name` | Variante attestata in un'altra voce meteo. |
