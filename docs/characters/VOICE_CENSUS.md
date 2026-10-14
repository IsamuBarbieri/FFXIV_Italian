# Censimento del parlato nelle cutscene

[Indice dei profili](../CHARACTER_VOICES.md). Rilevazione del **14 ottobre 2026**, client locale **2026.10.06.0000.0000**.

## Perimetro ed esito

- **98 fogli**, **27.141 campi `description`**, **702 etichette di parlante**: tutti i JSON presenti in `data/da_tradurre/dialogue/cut_scene/`, da `022` a `075`. Non ci sono fogli di questa cartella negli altri due stati editoriali.
- **24.708 candidati di parlato con SCD nella variante `en`**, distribuiti su **645 tag**. Sono campi sorgente, non tracce uniche o personaggi distinti.
- **162 dei 266 profili** hanno campioni inglesi attribuiti e riscontrati; **457 riferimenti campione selezionati per il confronto**. Le altre schede restano senza riscontro utile in questo perimetro, non vengono dichiarate prive di doppiaggio.
- **119 ulteriori tag con nome o denominazione corrispondente a ENpcResident** sono elencati sotto: l'audio è riscontrato, il collegamento del nome è un candidato da controllare nella scena prima di costruire il profilo. La lista comprende anche ruoli e forme evocate: non sono tutti nuovi personaggi distinti.

La presenza di voci di soldati, cittadini e piccoli comprimari conferma che «almeno una battuta doppiata» non equivale a «ruolo narrativo massiccio». Il censimento conserva questi parlanti; una scheda permanente richiede comunque identità o continuità utili all'adattamento. I generici mantengono note di scena, senza profili automatici per razza.

## Riscontro testuale complementare

Per chiudere il confronto delle schede prive di campioni utili in questa cartella sono stati cercati tag di parlante in **6.487 file** di quest e dialoghi custom nei tre stati editoriali, esclusi cutscene, master e leve. Sono stati documentati campioni per **103 ulteriori profili** nelle rispettive sezioni: **265/266** schede avevano così una fonte testuale locale tramite tag di parlante. Il controllo successivo delle citazioni nel racconto di Byakko ha fornito due battute di Tenzen: il [prompt personale](SIDE_STORIES.md#tenzen-prompt-personale) porta a **266/266** i profili con riscontro testuale, distinguendo parole riportate e parlato diretto. Questo controllo non verifica il doppiaggio fuori dalle cutscene.

Le corrispondenze aggiuntive usate nelle quest sono `BAISHAEN` → Bajsaljen, `MARSHAK` → Marsak, `MEESIYA` → Misija, `LLYMLEAN` → Llymlaen, `THEHOWLINGBLADE` → Howling Blade e `ARCADIAOWNER` → President. Sono chiavi del contesto verificato, non sostituzioni globali di nomi.

## Cosa dimostra il riscontro

Il codice della riga, compresi codici come `A00010`, viene collegato a `cut/<repository>/sound/voicem/voiceman_<foglio>/vo_voiceman_<foglio>_<codice>_<m|f>_<lingua>.scd`. Lo strumento verifica lo stesso percorso negli indici `.index` e `.index2`, legge il blocco iniziale del file e controlla la firma SCD. I percorsi verso dati vuoti, un byte o blocchi di soli zeri non confermano audio.

Le sigle `m` e `f` indicano varianti di risorsa e non identificano il genere del parlante. La variante `en` non prova da sola la lingua udibile: alcuni campi sorgente contengono giapponese o indicazioni per gli attori. Il contenitore non è stato ascoltato; non sono verificati durata, interpretazione o uso effettivo nella scena pubblicata.

La classificazione del testo è un filtro di candidati, non una revisione linguistica automatica. I tag `NONE_VOICE`, campi vuoti, direttive, risate e grida isolate non soddisfano da soli il criterio della battuta parlata. Le parentesi quadre non vengono eliminate indiscriminatamente: il parlato dei fae e di Bismarck può essere racchiuso fra quadre.

Un tag può indicare più fasi, più identità o un coro. Due tag possono indicare lo stesso personaggio. Il nome visibile, il corpo e il nome del doppiatore non bastano per fondere le voci. I conteggi non coprono automaticamente i dialoghi iniziali di ARR fuori dai fogli 2.2+, tutti i richiami in combattimento o le serie non presenti in questa cartella.

## Correzioni emerse dai campioni

- Zekowa: scena di Shaaloani, parlato acceso e imprecazioni esplicite; corretta la precedente attribuzione ai Hanuhanu e il tono troppo composto.
- Lakshmi e Scarmiglione: sibilanti allungate esplicite; aggiunto il segnale alle rispettive schede senza estenderlo al parlato di altri personaggi.
- Halmarut: facciata iniziale agitata e spontanea, diversa dalle enunciazioni successive; Lumull, Eyaney, Visna e Geode hanno ora indicazioni meno generiche.
- Louisoix: separate la fase sotto controllo nella serie Bahamut e la successiva voce da mentore; Branden distingue il confronto sulla Source dal contesto di Voeburt.
- Ys Iala non è Feo Ul; Zereel Ja non è Sareel Ja; Genolt non va fuso con Gerolt o Grenoldt.
- `DUMMYARDBERT` rimanda alla fase di Elidibus nel corpo di Ardbert; `IRONMASK` viene lasciato separato, dato il riferimento al False Griffin. `MYSTERYVOICE` non riceve un'attribuzione globale: alcune righe cambiano referente.
- La sola risata di Shantotto in `voiceman_07510` non conferma una battuta parlata; Byakko, Suzaku e Seiryu ripetono lo stesso testo in fogli diversi e non forniscono due contesti linguistici distinti.

## Altre denominazioni del corpus da contestualizzare

La corrispondenza seguente usa soltanto normalizzazione di spazi e punteggiatura del nome inglese. Non verifica alias, reincarnazioni o omonimi e non stabilisce il nome italiano. I riferimenti alle battute consentono la verifica della scena; la colonna ENpcResident riguarda il nome.

| Nome candidato | Tag | Candidati EN | Riferimento del nome | Battute per la verifica di contesto |
| --- | --- | ---: | --- | --- |
| Alayla | `ALAYLA` | 24 | `data/translations/world/enpcresident.json#1047936:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07005.json#273:description`; `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#383:description` |
| Almet | `ALMET` | 15 | `data/translations/world/enpcresident.json#1028139:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05002.json#268:description`; `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#226:description` |
| Amalj'aa warrior | `AMALJAAWARRIOR` | 3 | `data/translations/world/enpcresident.json#1004610:name` | `data/da_tradurre/dialogue/cut_scene/055/voiceman_05505.json#134:description`; `data/da_tradurre/dialogue/cut_scene/055/voiceman_05505.json#135:description` |
| A-Ruhn-Senna | `ARUHNSENNA` | 21 | `data/translations/world/enpcresident.json#1004629:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#69:description`; `data/da_tradurre/dialogue/cut_scene/060/voiceman_06005.json#339:description` |
| Ascian prime | `ASCIANPRIME` | 2 | `data/translations/world/enpcresident.json#1012893:name` | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03007.json#104:description` |
| Azami | `AZAMI` | 4 | `data/translations/world/enpcresident.json#1019546:name` | `data/da_tradurre/dialogue/cut_scene/043/voiceman_04300.json#40:description` |
| Barnier | `BARNIER` | 39 | `data/translations/world/enpcresident.json#1039162:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#339:description`; `data/da_tradurre/dialogue/cut_scene/060/voiceman_06005.json#493:description` |
| Bismarck | `BISMARCK` | 8 | `data/translations/world/enpcresident.json#1028360:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#174:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#208:description` |
| Bloewyda | `BLOEWYDA` | 17 | `data/translations/world/enpcresident.json#1039195:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06005.json#366:description`; `data/da_tradurre/dialogue/cut_scene/060/voiceman_06006.json#201:description` |
| Bol Noq' | `BOLNOQ` | 9 | `data/translations/world/enpcresident.json#1046579:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#506:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#517:description` |
| Bragi | `BRAGI` | 4 | `data/translations/world/enpcresident.json#1027224:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#64:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#159:description` |
| Bremondt | `BREMONDT` | 10 | `data/translations/world/enpcresident.json#1003004:name` | `data/da_tradurre/dialogue/cut_scene/025/voiceman_02500.json#480:description`; `data/da_tradurre/dialogue/cut_scene/025/voiceman_02500.json#496:description` |
| Br'uk Evu | `BRUKEVU` | 1 | `data/translations/world/enpcresident.json#1046521:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#96:description` |
| Bunchin | `BUNCHIN` | 14 | `data/translations/world/enpcresident.json#1019178:name` | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#98:description`; `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#118:description` |
| Carvallain | `CARVALLAIN` | 8 | `data/translations/world/enpcresident.json#1000915:name` | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#277:description`; `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#289:description` |
| Cassard | `CASSARD` | 13 | `data/translations/world/enpcresident.json#1027854:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#381:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#71:description` |
| Crystarium man | `CRYSTARIUMMAN` | 3 | `data/translations/world/enpcresident.json#1028162:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#549:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#163:description` |
| Crystarium woman | `CRYSTARIUMWOMAN` | 2 | `data/translations/world/enpcresident.json#1028163:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#550:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#162:description` |
| Curtis | `CURTIS` | 8 | `data/translations/world/enpcresident.json#1019693:name` | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04005.json#235:description`; `data/da_tradurre/dialogue/cut_scene/040/voiceman_04005.json#245:description` |
| Cymet | `CYMET` | 6 | `data/translations/world/enpcresident.json#1028141:name` | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#178:description` |
| Damielliot | `DAMIELLIOT` | 2 | `data/translations/world/enpcresident.json#1017811:name` | `data/da_tradurre/dialogue/cut_scene/055/voiceman_05500.json#183:description`; `data/da_tradurre/dialogue/cut_scene/055/voiceman_05500.json#185:description` |
| Delia | `DELIA` | 2 | `data/translations/world/enpcresident.json#1049242:name` | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#84:description` |
| Dewlala | `DEWLALA` | 19 | `data/translations/world/enpcresident.json#1012841:name` | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03002.json#61:description`; `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#262:description` |
| Durante | `DURANTE` | 19 | `data/translations/world/enpcresident.json#1045149:name` | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#391:description`; `data/da_tradurre/dialogue/cut_scene/065/voiceman_06500.json#134:description` |
| Ecaate | `ECAATE` | 2 | `data/translations/world/enpcresident.json#1050390:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#326:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#330:description` |
| Enuo | `ENUO` | 3 | `data/translations/world/enpcresident.json#1058622:name` | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07500.json#224:description` |
| Eulmoran soldier | `EULMORANSOLDIER` | 1 | `data/translations/world/enpcresident.json#1028355:name` | `data/da_tradurre/dialogue/cut_scene/051/voiceman_05100.json#210:description` |
| Fyrgeiss | `FYRGEISS` | 1 | `data/translations/world/enpcresident.json#1007204:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06005.json#319:description` |
| Ga Bu | `GABU` | 12 | `data/translations/world/enpcresident.json#1016815:name` | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#133:description`; `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#282:description` |
| Galene | `GALENE` | 15 | `data/translations/world/enpcresident.json#1039480:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06004.json#529:description`; `data/da_tradurre/dialogue/cut_scene/060/voiceman_06004.json#547:description` |
| Genolt | `GENOLT` | 4 | `data/translations/world/enpcresident.json#1048084:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#193:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#233:description` |
| Ghen Gen | `GHENGEN` | 8 | `data/translations/world/enpcresident.json#1027872:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#382:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#404:description` |
| Glynard | `GLYNARD` | 4 | `data/translations/world/enpcresident.json#1027322:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#63:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#157:description` |
| Gnath priest | `GNATHPRIEST` | 3 | `data/translations/world/enpcresident.json#1012649:name` | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#96:description`; `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#100:description` |
| Gossan | `GOSSAN` | 39 | `data/translations/world/enpcresident.json#1051787:name` | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#0:description`; `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#561:description` |
| Grinnaux | `GRINNAUX` | 1 | `data/translations/world/enpcresident.json#1011898:name` | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#95:description` |
| Grithil | `GRITHIL` | 6 | `data/translations/world/enpcresident.json#1027441:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#198:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#417:description` |
| Grynewaht | `GRYNEWAHT` | 21 | `data/translations/world/enpcresident.json#1018376:name` | `data/da_tradurre/dialogue/cut_scene/035/voiceman_03500.json#204:description`; `data/da_tradurre/dialogue/cut_scene/040/voiceman_04003.json#203:description` |
| Guthjon | `GUTHJON` | 9 | `data/translations/world/enpcresident.json#1028148:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#136:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#148:description` |
| Gyodo | `GYODO` | 4 | `data/translations/world/enpcresident.json#1019548:name` | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04001.json#47:description`; `data/da_tradurre/dialogue/cut_scene/040/voiceman_04001.json#50:description` |
| Haldrath | `HALDRATH` | 8 | `data/translations/world/enpcresident.json#1011864:name` | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#320:description`; `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#331:description` |
| Halric | `HALRIC` | 1 | `data/translations/world/enpcresident.json#1028127:name` | `data/da_tradurre/dialogue/cut_scene/051/voiceman_05100.json#146:description` |
| Hhwato | `HHWATO` | 4 | `data/translations/world/enpcresident.json#1046988:name` | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#79:description`; `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#100:description` |
| House Fortemps steward | `HOUSEFORTEMPSSTEWARD` | 5 | `data/translations/world/enpcresident.json#1012377:name` | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#86:description`; `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#90:description` |
| Hyllfyr | `HYLLFYR` | 14 | `data/translations/world/enpcresident.json#1001039:name` | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#226:description`; `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#240:description` |
| Igeyorhm | `IGEYORHM` | 2 | `data/translations/world/enpcresident.json#1004762:name` | `data/da_tradurre/dialogue/cut_scene/053/voiceman_05300.json#120:description`; `data/da_tradurre/dialogue/cut_scene/053/voiceman_05300.json#121:description` |
| Ikaruga | `IKARUGA` | 2 | `data/translations/world/enpcresident.json#1019154:name` | `data/da_tradurre/dialogue/cut_scene/042/voiceman_04200.json#32:description`; `data/da_tradurre/dialogue/cut_scene/042/voiceman_04200.json#33:description` |
| Irvithe | `IRVITHE` | 8 | `data/translations/world/enpcresident.json#1027288:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#197:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#409:description` |
| Isadora | `ISADORA` | 7 | `data/translations/world/enpcresident.json#1048069:name` | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#278:description`; `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#290:description` |
| Ishgardian priest | `ISHGARDIANPRIEST` | 3 | `data/translations/world/enpcresident.json#1012386:name` | `data/da_tradurre/dialogue/cut_scene/031/voiceman_03100.json#151:description`; `data/da_tradurre/dialogue/cut_scene/031/voiceman_03100.json#153:description` |
| Ixali chieftain | `IXALICHIEFTAIN` | 3 | `data/translations/world/enpcresident.json#1004700:name` | `data/da_tradurre/dialogue/cut_scene/055/voiceman_05505.json#1:description`; `data/da_tradurre/dialogue/cut_scene/055/voiceman_05505.json#16:description` |
| Kai-Shirr | `KAISHIRR` | 22 | `data/translations/world/enpcresident.json#1028121:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#284:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05004.json#83:description` |
| Kanilokka | `KANILOKKA` | 4 | `data/translations/world/enpcresident.json#1051781:name` | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#41:description`; `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#48:description` |
| Katliss | `KATLISS` | 5 | `data/translations/world/enpcresident.json#1027232:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#66:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#154:description` |
| Kemakka | `KEMAKKA` | 20 | `data/translations/world/enpcresident.json#1047075:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#40:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#431:description` |
| Ketill | `KETILL` | 8 | `data/translations/world/enpcresident.json#1058625:name` | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07505.json#217:description`; `data/da_tradurre/dialogue/cut_scene/075/voiceman_07505.json#230:description` |
| Ketonye | `KETONYE` | 1 | `data/translations/world/enpcresident.json#1049687:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#564:description` |
| Kokkol Dankkol | `KOKKOLDANKKOL` | 33 | `data/translations/world/enpcresident.json#1039194:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06005.json#244:description`; `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#146:description` |
| Kytte | `KYTTE` | 7 | `data/translations/world/enpcresident.json#1037288:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#452:description`; `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#456:description` |
| Linuhanu | `LINUHANU` | 20 | `data/translations/world/enpcresident.json#1046725:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#382:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#488:description` |
| M-017 | `M017` | 28 | `data/translations/world/enpcresident.json#1039140:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06006.json#495:description`; `data/da_tradurre/dialogue/cut_scene/060/voiceman_06006.json#556:description` |
| Maelie | `MAELIE` | 2 | `data/translations/world/enpcresident.json#1012168:name` | `data/da_tradurre/dialogue/cut_scene/031/voiceman_03100.json#155:description`; `data/da_tradurre/dialogue/cut_scene/031/voiceman_03100.json#160:description` |
| Maelstrom officer | `MAELSTROMOFFICER` | 4 | `data/translations/world/enpcresident.json#1010220:name` | `data/da_tradurre/dialogue/cut_scene/044/voiceman_04400.json#292:description`; `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#94:description` |
| Magnus | `MAGNUS` | 17 | `data/translations/world/enpcresident.json#1028147:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#104:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#335:description` |
| Mahuwsa | `MAHUWSA` | 8 | `data/translations/world/enpcresident.json#1047405:name` | `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#260:description`; `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#285:description` |
| Malachite | `MALACHITE` | 17 | `data/translations/world/enpcresident.json#1053602:name` | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#353:description`; `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#472:description` |
| Mappingway | `MAPPINGWAY` | 27 | `data/translations/world/enpcresident.json#1038907:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06002.json#69:description`; `data/da_tradurre/dialogue/cut_scene/075/voiceman_07500.json#179:description` |
| Mehrahd | `MEHRAHD` | 7 | `data/translations/world/enpcresident.json#1039580:name` | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#48:description`; `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#529:description` |
| Mehryde | `MEHRYDE` | 9 | `data/translations/world/enpcresident.json#1037381:name` | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#2:description`; `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#39:description` |
| Mehvan | `MEHVAN` | 1 | `data/translations/world/enpcresident.json#1039051:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#212:description` |
| Mettius | `METTIUS` | 5 | `data/translations/world/enpcresident.json#1045172:name` | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#172:description`; `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#185:description` |
| Miayli | `MIAYLI` | 62 | `data/translations/world/enpcresident.json#1054226:name` | `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#124:description`; `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#445:description` |
| Mihleel | `MIHLEEL` | 2 | `data/translations/world/enpcresident.json#1037318:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#340:description`; `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#341:description` |
| Miilal Ja | `MIILALJA` | 9 | `data/translations/world/enpcresident.json#1047607:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07002.json#284:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07002.json#333:description` |
| Milos | `MILOS` | 12 | `data/translations/world/enpcresident.json#1048070:name` | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#292:description`; `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#296:description` |
| Montichaigne | `MONTICHAIGNE` | 52 | `data/translations/world/enpcresident.json#1038067:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#380:description`; `data/da_tradurre/dialogue/cut_scene/060/voiceman_06005.json#237:description` |
| Moren | `MOREN` | 8 | `data/translations/world/enpcresident.json#1027226:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#68:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#167:description` |
| Naahe | `NAAHE` | 8 | `data/translations/world/enpcresident.json#1050389:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#314:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#316:description` |
| Nabriales | `NABRIALES` | 37 | `data/translations/world/enpcresident.json#1004763:name` | `data/da_tradurre/dialogue/cut_scene/025/voiceman_02500.json#119:description`; `data/da_tradurre/dialogue/cut_scene/025/voiceman_02500.json#173:description` |
| Nahbdeen | `NAHBDEEN` | 31 | `data/translations/world/enpcresident.json#1037646:name` | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#313:description`; `data/da_tradurre/dialogue/cut_scene/065/voiceman_06500.json#409:description` |
| Nitowikwe | `NITOWIKWE` | 8 | `data/translations/world/enpcresident.json#1046985:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#451:description`; `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#146:description` |
| Nostalgia | `NOSTALGIA` | 26 | `data/translations/world/enpcresident.json#1047453:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#349:description`; `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#126:description` |
| Oul Sigun | `OULSIGUN` | 12 | `data/translations/world/enpcresident.json#1027656:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#175:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#400:description` |
| Pakwene | `PAKWENE` | 1 | `data/translations/world/enpcresident.json#1049684:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#563:description` |
| Pepoane | `PEPOANE` | 22 | `data/translations/world/enpcresident.json#1049683:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#574:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#697:description` |
| Peshiwa | `PESHIWA` | 1 | `data/translations/world/enpcresident.json#1046931:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#186:description` |
| Raganfrid | `RAGANFRID` | 6 | `data/translations/world/enpcresident.json#1020841:name` | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#229:description`; `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#263:description` |
| Ramuh | `RAMUH` | 22 | `data/translations/world/enpcresident.json#1005162:name` | `data/da_tradurre/dialogue/cut_scene/023/voiceman_02300.json#97:description`; `data/da_tradurre/dialogue/cut_scene/023/voiceman_02300.json#120:description` |
| Resistance healer | `RESISTANCEHEALER` | 2 | `data/translations/world/enpcresident.json#1026216:name` | `data/da_tradurre/dialogue/cut_scene/044/voiceman_04400.json#241:description`; `data/da_tradurre/dialogue/cut_scene/044/voiceman_04400.json#243:description` |
| Rhetkympf | `RHETKYMPF` | 2 | `data/translations/world/enpcresident.json#1003540:name` | `data/da_tradurre/dialogue/cut_scene/065/voiceman_06505.json#251:description`; `data/da_tradurre/dialogue/cut_scene/065/voiceman_06505.json#254:description` |
| Robor | `ROBOR` | 32 | `data/translations/world/enpcresident.json#1047935:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07005.json#287:description`; `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#385:description` |
| Sahagin priest | `SAHAGINPRIEST` | 5 | `data/translations/world/enpcresident.json#1007095:name` | `data/da_tradurre/dialogue/cut_scene/055/voiceman_05505.json#5:description`; `data/da_tradurre/dialogue/cut_scene/055/voiceman_05505.json#14:description` |
| Shenza | `SHENZA` | 22 | `data/translations/world/enpcresident.json#1046983:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#117:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#156:description` |
| Singingway | `SINGINGWAY` | 5 | `data/translations/world/enpcresident.json#1038957:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06002.json#192:description`; `data/da_tradurre/dialogue/cut_scene/060/voiceman_06005.json#166:description` |
| Sleepingway | `SLEEPINGWAY` | 5 | `data/translations/world/enpcresident.json#1039125:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06006.json#244:description` |
| Sul Uin | `SULUIN` | 11 | `data/translations/world/enpcresident.json#1027654:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#156:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#202:description` |
| Taapu | `TAAPU` | 2 | `data/translations/world/enpcresident.json#1048099:name` | `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#263:description`; `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#264:description` |
| Tansui | `TANSUI` | 37 | `data/translations/world/enpcresident.json#1018976:name` | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#0:description`; `data/da_tradurre/dialogue/cut_scene/042/voiceman_04200.json#75:description` |
| Teeshal Ja | `TEESHALJA` | 33 | `data/translations/world/enpcresident.json#1051780:name` | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#55:description`; `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#184:description` |
| Temulun | `TEMULUN` | 10 | `data/translations/world/enpcresident.json#1019083:name` | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04003.json#56:description`; `data/da_tradurre/dialogue/cut_scene/040/voiceman_04003.json#74:description` |
| Thaffe | `THAFFE` | 15 | `data/translations/world/enpcresident.json#1028146:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#94:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#333:description` |
| Theva | `THEVA` | 3 | `data/translations/world/enpcresident.json#1027313:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#223:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#224:description` |
| Tobli | `TOBLI` | 18 | `data/translations/world/enpcresident.json#1046571:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#524:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#561:description` |
| Tsukuyomi | `TSUKUYOMI` | 4 | `data/translations/world/enpcresident.json#1025203:name` | `data/da_tradurre/dialogue/cut_scene/043/voiceman_04300.json#151:description`; `data/da_tradurre/dialogue/cut_scene/043/voiceman_04300.json#154:description` |
| Uimet | `UIMET` | 5 | `data/translations/world/enpcresident.json#1028140:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05002.json#270:description`; `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#201:description` |
| Vorporlor | `VORPORLOR` | 39 | `data/translations/world/enpcresident.json#1046860:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07001.json#163:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07001.json#419:description` |
| Wayakkwe | `WAYAKKWE` | 11 | `data/translations/world/enpcresident.json#1047443:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#366:description`; `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#74:description` |
| Wihuwte | `WIHUWTE` | 25 | `data/translations/world/enpcresident.json#1046974:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#16:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#89:description` |
| Wilfsunn | `WILFSUNN` | 12 | `data/translations/world/enpcresident.json#1039196:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06005.json#361:description`; `data/da_tradurre/dialogue/cut_scene/060/voiceman_06006.json#198:description` |
| Wrenden | `WRENDEN` | 11 | `data/translations/world/enpcresident.json#1029472:name` | `data/da_tradurre/dialogue/cut_scene/051/voiceman_05100.json#177:description`; `data/da_tradurre/dialogue/cut_scene/051/voiceman_05100.json#204:description` |
| Xande | `XANDE` | 3 | `data/translations/world/enpcresident.json#1009370:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#614:description` |
| Ys Iala | `YSIALA` | 9 | `data/translations/world/enpcresident.json#1027655:name` | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#168:description`; `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#204:description` |
| Zanuhali | `ZANUHALI` | 26 | `data/translations/world/enpcresident.json#1047345:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#327:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#481:description` |
| Zasshal | `ZASSHAL` | 12 | `data/translations/world/enpcresident.json#1042163:name` | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#51:description`; `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#76:description` |
| Zawpya | `ZAWPYA` | 5 | `data/translations/world/enpcresident.json#1046984:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#143:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#458:description` |
| Zereel Ja | `ZEREELJA` | 34 | `data/translations/world/enpcresident.json#1047693:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07002.json#252:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07002.json#401:description` |
| Zeromus | `ZEROMUS` | 5 | `data/translations/world/enpcresident.json#1045151:name` | `data/da_tradurre/dialogue/cut_scene/065/voiceman_06500.json#201:description`; `data/da_tradurre/dialogue/cut_scene/065/voiceman_06500.json#210:description` |
| Zodiark | `ZODIARK` | 24 | `data/translations/world/enpcresident.json#1038057:name` | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#816:description`; `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#841:description` |
| Zurmurwur | `ZURMURWUR` | 12 | `data/translations/world/enpcresident.json#1047737:name` | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07001.json#265:description`; `data/da_tradurre/dialogue/cut_scene/070/voiceman_07001.json#381:description` |

## Inventario completo dei tag

Ogni tag compare una sola volta, compresi generici, cori e righe tecniche. «EN» conta i candidati di parlato con un SCD `en`; «altre» conta candidati con SCD in altra lingua ma senza quello `en`. Il riferimento è un candidato parlato riscontrato, se disponibile, altrimenti una riga rappresentativa. Il rapporto locale contiene i percorsi audio esatti e tutte le righe; qui non vengono copiati i testi del gioco.

| Tag | Profilo / nome candidato | EN | Altre | Esito del tag | Riferimento |
| --- | --- | ---: | ---: | --- | --- |
| `000_000` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#58:description` |
| `000_001` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#59:description` |
| `000_001_NONE_VOICE` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/034/voiceman_03400.json#3:description` |
| `000_002` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#60:description` |
| `000_002_NONE_VOICE` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/034/voiceman_03400.json#5:description` |
| `000_003` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#274:description` |
| `000_003_NONE_VOICE` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/034/voiceman_03400.json#6:description` |
| `000_004_NONE_VOICE` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/034/voiceman_03400.json#339:description` |
| `000_005_NONE_VOICE` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#57:description` |
| `000_006_NONE_VOICE` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06002.json#310:description` |
| `000_1` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03002.json#44:description` |
| `000_2` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03002.json#46:description` |
| `000_3` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#43:description` |
| `001_SYSTEM` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/042/voiceman_04200.json#287:description` |
| `010_001_NONE_VOICE` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#604:description` |
| `010_002_NONE_VOICE` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#606:description` |
| `100_001_NONE_VOICE` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07002.json#315:description` |
| `100_002_NONE_VOICE` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07002.json#317:description` |
| `2P` | [2P](SIDE_STORIES.md) | 0 | 0 | Nessun SCD riscontrato nel percorso verificato | `data/da_tradurre/dialogue/cut_scene/051/voiceman_05101.json#0:description` |
| `9S` | [9S](SIDE_STORIES.md) | 0 | 0 | Nessun SCD riscontrato nel percorso verificato | `data/da_tradurre/dialogue/cut_scene/051/voiceman_05101.json#1:description` |
| `ABEL` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/033/voiceman_03300.json#188:description` |
| `ALAMHIGANCITIZEN1` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#30:description` |
| `ALAMHIGANCITIZEN2` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#31:description` |
| `ALAMHIGANCITIZEN3` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#32:description` |
| `ALAMHIGANCITIZENA` | Identità / ruolo da contestualizzare | 9 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#71:description` |
| `ALAMHIGANCITIZENB` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#78:description` |
| `ALAMHIGANCITIZENC` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#79:description` |
| `ALAMHIGANDELEGATION` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#552:description` |
| `ALAMHIGANLIBERATORA` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#120:description` |
| `ALAMHIGANLIBERATORB` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#172:description` |
| `ALAMHIGOGARLEANOFFICER` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#81:description` |
| `ALAYLA` | Alayla (nome da contestualizzare) | 24 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07005.json#267:description` |
| `ALCHEMISTOFTHEGREATWORKA` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#482:description` |
| `ALCHEMISTOFTHEGREATWORKB` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#483:description` |
| `ALCHEMISTOFTHEGREATWORKC` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#484:description` |
| `ALEXANDRIABOY71` | Identità / ruolo da contestualizzare | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#10:description` |
| `ALEXANDRIAMAN71A` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#16:description` |
| `ALEXANDRIAMAN71B` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#200:description` |
| `ALEXANDRIAMOTHER71` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#11:description` |
| `ALEXANDRIASOLDIER` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#475:description` |
| `ALEXANDRIAWOMAN71A` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#18:description` |
| `ALEXANDRIAWOMAN71B` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#196:description` |
| `ALGUS` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04101.json#0:description` |
| `ALISAIE` | [Alisaie](RECURRING.md) | 1109 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/024/voiceman_02401.json#0:description` |
| `ALL` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato; include testo non inglese | `data/da_tradurre/dialogue/cut_scene/023/voiceman_02300.json#201:description` |
| `ALLAGMAN` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#602:description` |
| `ALMET` | Almet (nome da contestualizzare) | 15 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05002.json#256:description` |
| `ALPHINAUD` | [Alphinaud](RECURRING.md) | 1586 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#8:description` |
| `ALPHINAUDANDALISAIE` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/024/voiceman_02401.json#105:description` |
| `ALXAAL` | [Alxaal](SIDE_STORIES.md) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07510.json#4:description` |
| `AMALJAACHIEFTAIN` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/055/voiceman_05500.json#161:description` |
| `AMALJAAWARRIOR` | Amalj'aa warrior (nome da contestualizzare) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/055/voiceman_05505.json#133:description` |
| `AMAUROTADMINISTRATOR` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#538:description` |
| `AMELIANCE` | [Ameliance](ENDWALKER.md) | 33 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#406:description` |
| `AMON` | [Amon](ENDWALKER.md) | 138 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#528:description` |
| `ANANTASOLDIER` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04005.json#26:description` |
| `ANCIENTMEMORYA` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06004.json#894:description` |
| `ANCIENTMEMORYB` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06004.json#895:description` |
| `ANCIENTSA` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#299:description` |
| `ANCIENTSB` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#301:description` |
| `ANCIENTSHADOWA` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#751:description` |
| `ANCIENTSHADOWB` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#753:description` |
| `ANCIENTSHADOWC` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#754:description` |
| `ANCIENTSHADOWD` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#755:description` |
| `ANCIENTSHADOWE` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#756:description` |
| `ANNOUNCER` | Identità / ruolo da contestualizzare | 17 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#308:description` |
| `ANXIETY05306` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#312:description` |
| `ARENVALD` | [Arenvald](STORMBLOOD.md) | 60 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#200:description` |
| `ARTOIREL` | [Artoirel](HEAVENSWARD.md) | 19 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/031/voiceman_03100.json#138:description` |
| `ARUHNSENNA` | A-Ruhn-Senna (nome da contestualizzare) | 21 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#68:description` |
| `ASAHI` | [Asahi](STORMBLOOD.md) | 120 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/042/voiceman_04200.json#131:description` |
| `ASAHIFATHER` | Identità / ruolo da contestualizzare | 10 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/043/voiceman_04300.json#53:description` |
| `ASAHIMOTHER` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/043/voiceman_04300.json#90:description` |
| `ASCIAN` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/031/voiceman_03100.json#85:description` |
| `ASCIANPRIME` | Ascian prime (nome da contestualizzare) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03007.json#104:description` |
| `ATHENA` | [Athena](SIDE_STORIES.md) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06401.json#6:description` |
| `AWAHEEN` | [Ahewann](ENDWALKER.md) | 52 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#640:description` |
| `AYMERIC` | [Aymeric](HEAVENSWARD.md) | 320 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/024/voiceman_02400.json#44:description` |
| `AZAMI` | Azami (nome da contestualizzare) | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/042/voiceman_04200.json#186:description` |
| `AZDAJA` | [Azdaja](ENDWALKER.md) | 20 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#360:description` |
| `BAGGAMONAN` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/043/voiceman_04301.json#0:description` |
| `BAKOOLJAJA` | [Bakool Ja Ja](DAWNTRAIL.md) | 180 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#105:description` |
| `BARDOFDARKNESS` | [Renda-Rae](SHADOWBRINGERS.md) | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/034/voiceman_03400.json#61:description` |
| `BARNIER` | Barnier (nome da contestualizzare) | 39 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#338:description` |
| `BARONKNIGHT` | Identità / ruolo da contestualizzare | 0 | 0 | Nessun SCD riscontrato nel percorso verificato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#397:description` |
| `BAZAARMOBA` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#49:description` |
| `BAZAARMOBB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#56:description` |
| `BAZAARMOBC` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#57:description` |
| `BEQLUGG` | [Beq Lugg](SHADOWBRINGERS.md) | 93 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/051/voiceman_05100.json#73:description` |
| `BEWILDERMENT05306` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#313:description` |
| `BIGGS` | [Biggs](RECURRING.md) | 17 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#206:description` |
| `BIGGSWEDGE` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#80:description` |
| `BISMARCK` | Bismarck (nome da contestualizzare) | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#174:description` |
| `BJARNI` | Bjarni (nome da contestualizzare) | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07505.json#305:description` |
| `BLACKKNIGHT` | [Golbez](ENDWALKER.md) | 143 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/062/voiceman_06200.json#192:description` |
| `BLACKMAGEOFDARKNESS` | [Nyelbert](SHADOWBRINGERS.md) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/034/voiceman_03400.json#245:description` |
| `BLACKREGULATOR73` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#141:description` |
| `BLOEWYDA` | Bloewyda (nome da contestualizzare) | 17 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06005.json#366:description` |
| `BLOODYEXECUTIONERA` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#192:description` |
| `BLOODYEXECUTIONERB` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#193:description` |
| `BLOODYEXECUTIONERC` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#194:description` |
| `BODYGUARD01623` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/035/voiceman_03500.json#128:description` |
| `BOLNOQ` | Bol Noq' (nome da contestualizzare) | 9 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#506:description` |
| `BOONEWAFEMALEAUD` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#292:description` |
| `BOSSOFFUATH` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#314:description` |
| `BOTANISTMAN` | Identità / ruolo da contestualizzare | 10 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#246:description` |
| `BOY05426` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#63:description` |
| `BRAGI` | Bragi (nome da contestualizzare) | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#64:description` |
| `BRASSBLADE02400` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/025/voiceman_02500.json#332:description` |
| `BRAVESOLDIER` | Identità / ruolo da contestualizzare | 13 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#64:description` |
| `BRAWNYKNIGHT` | brawny knight (nome da contestualizzare) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#334:description` |
| `BREMONDT` | Bremondt (nome da contestualizzare) | 10 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/025/voiceman_02500.json#480:description` |
| `BRUKEVU` | Br'uk Evu (nome da contestualizzare) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#96:description` |
| `BUNCHIN` | Bunchin (nome da contestualizzare) | 14 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#98:description` |
| `BYAKKO` | [Byakko](SIDE_STORIES.md) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/042/voiceman_04201.json#0:description` |
| `CAHCIUA` | [Cahciua](DAWNTRAIL.md) | 122 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#702:description` |
| `CALMNESS05306` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#316:description` |
| `CALYX` | [Calyx](DAWNTRAIL.md) | 156 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#112:description` |
| `CANALTOWNGIRL` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07005.json#131:description` |
| `CANALTOWNMALE` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07005.json#133:description` |
| `CARVALLAIN` | Carvallain (nome da contestualizzare) | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#277:description` |
| `CASSARD` | Cassard (nome da contestualizzare) | 13 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#381:description` |
| `CAVALRY02236` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#112:description` |
| `CAVALRY02400` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/024/voiceman_02400.json#85:description` |
| `CEFCA` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07515.json#0:description` |
| `CHAINUZZ` | [Chai-Nuzz](SHADOWBRINGERS.md) | 90 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#319:description` |
| `CHARIBERT` | [Charibert](HEAVENSWARD.md) | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03004.json#85:description` |
| `CID` | [Cid](RECURRING.md) | 91 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#79:description` |
| `CIRINA` | [Cirina](STORMBLOOD.md) | 45 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04003.json#3:description` |
| `CITIZENA02240` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#217:description` |
| `CITIZENB02240` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#218:description` |
| `CITIZENC02240` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#244:description` |
| `CLERKPX0093` | Clerk PX-0093 (nome da contestualizzare) | 0 | 0 | Nessun SCD riscontrato nel percorso verificato | `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#496:description` |
| `CLOWNOFBLUE` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#258:description` |
| `CLOWNOFRED` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#259:description` |
| `COLLEAGUESOLDIER` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#421:description` |
| `CONFUSION05306` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#391:description` |
| `CONRAD` | [Conrad](STORMBLOOD.md) | 46 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#56:description` |
| `CRYSTALBRAVE02400` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/025/voiceman_02500.json#334:description` |
| `CRYSTARIUMCITIZENA` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#349:description` |
| `CRYSTARIUMCITIZENB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#350:description` |
| `CRYSTARIUMCITIZENC` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#357:description` |
| `CRYSTARIUMCITIZEND` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#358:description` |
| `CRYSTARIUMGARDOLD` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#23:description` |
| `CRYSTARIUMGARDPICKET` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#95:description` |
| `CRYSTARIUMGARDYOUNG` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#14:description` |
| `CRYSTARIUMGATEKEEPER` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#160:description` |
| `CRYSTARIUMGIRL` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#552:description` |
| `CRYSTARIUMMAN` | Crystarium man (nome da contestualizzare) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#549:description` |
| `CRYSTARIUMOLDMAN` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#551:description` |
| `CRYSTARIUMWOMAN` | Crystarium woman (nome da contestualizzare) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#550:description` |
| `CURTIS` | Curtis (nome da contestualizzare) | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04005.json#235:description` |
| `CYMET` | Cymet (nome da contestualizzare) | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05002.json#261:description` |
| `DAMIELLIOT` | Damielliot (nome da contestualizzare) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/055/voiceman_05500.json#183:description` |
| `DARKNOKKEN` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#455:description` |
| `DELIA` | Delia (nome da contestualizzare) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#84:description` |
| `DEWLALA` | Dewlala (nome da contestualizzare) | 19 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03002.json#58:description` |
| `DIRTYMAN` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/051/voiceman_05100.json#207:description` |
| `DOMANMAID` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/043/voiceman_04300.json#78:description` |
| `DOPROBAKOOLJAJAMINIONS` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#391:description` |
| `DOPROMALECUSTOMER` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#97:description` |
| `DOTHARLMAN` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04003.json#4:description` |
| `DOTHARLWOMAN` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04003.json#136:description` |
| `DRUSUS` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04005.json#132:description` |
| `DULIACHAI` | [Dulia-Chai](SHADOWBRINGERS.md) | 49 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#317:description` |
| `DUMMYARDBERT` | [Elidibus](ARR.md) | 21 | 2 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#118:description` |
| `DURANTE` | Durante (nome da contestualizzare) | 19 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#391:description` |
| `E3F1GARLEANSOLDIER` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#208:description` |
| `E3F1VILLAGER` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#35:description` |
| `E3F2GARLEANMESSENGER` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04004.json#81:description` |
| `E3F2GARLEANSOLDIER` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04004.json#62:description` |
| `E3F2VILLAGERA` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#273:description` |
| `E3F2VILLAGERB` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#274:description` |
| `E3F2VILLAGERC` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#424:description` |
| `E3F2VILLAGERD` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#327:description` |
| `E3F2VILLAGERE` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/043/voiceman_04300.json#23:description` |
| `E3F2VILLAGERF` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/043/voiceman_04300.json#24:description` |
| `EAA` | Identità / ruolo da contestualizzare | 17 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06006.json#380:description` |
| `EAB` | Identità / ruolo da contestualizzare | 13 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06006.json#382:description` |
| `EALDNARCHE` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/073/voiceman_07310.json#0:description` |
| `EARTHMONOLITH` | [Scarmiglione](ENDWALKER.md) | 24 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/062/voiceman_06200.json#38:description` |
| `ECAATE` | Ecaate (nome da contestualizzare) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#326:description` |
| `EDEN3BOSS3` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05401.json#7:description` |
| `EDEN3BOSS4` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05401.json#8:description` |
| `EDEN3BOSS5` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05401.json#13:description` |
| `EDMONT` | [Edmont de Fortemps](HEAVENSWARD.md) | 120 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#16:description` |
| `ELIDIBUS` | [Elidibus](ARR.md) | 188 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03008.json#22:description` |
| `EMETSELCH` | [Emet-Selch](SHADOWBRINGERS.md) | 522 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#117:description` |
| `EMMANELLAIN` | [Emmanellain](HEAVENSWARD.md) | 48 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#111:description` |
| `EMPLOYEEMAN` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#396:description` |
| `EMPLOYEEWOMAN` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#397:description` |
| `ENDSINGER` | Identità / ruolo da contestualizzare | 20 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#842:description` |
| `ENDSINGER_NONE_VOICE` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06101.json#4:description` |
| `ENUO` | Enuo (nome da contestualizzare) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07500.json#222:description` |
| `ERENVILLE` | [Erenville](DAWNTRAIL.md) | 365 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#212:description` |
| `ESTINIEN` | [Estinien](RECURRING.md) | 513 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#6:description` |
| `EULMORANSOLDIER` | Eulmoran soldier (nome da contestualizzare) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/051/voiceman_05100.json#210:description` |
| `EULMOREADJUTANT` | Identità / ruolo da contestualizzare | 17 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#425:description` |
| `EULMOREMANA` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/053/voiceman_05300.json#222:description` |
| `EULMOREMANB` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/053/voiceman_05300.json#223:description` |
| `EULMOREPEOPLEA` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05004.json#166:description` |
| `EULMOREPEOPLEB` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05004.json#177:description` |
| `EULMOREPEOPLEC` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05004.json#180:description` |
| `EULMOREPEOPLED` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05004.json#183:description` |
| `EULMORESOLDIERA` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#90:description` |
| `EULMORESOLDIERB` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#92:description` |
| `EULMORESOLDIERC` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#457:description` |
| `EVERLASTINGDARKNESS` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05002.json#245:description` |
| `EYANEY` | [Eyaney](DAWNTRAIL.md) | 34 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#204:description` |
| `EYNZAHRSLAFYRSYN` | [Eynzahr Slafyrsyn](ARR.md) | 13 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#168:description` |
| `FACILITATORMALE` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#143:description` |
| `FAKEGODBEAST` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#25:description` |
| `FANDANIEL` | [Fandaniel](ENDWALKER.md) | 85 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/053/voiceman_05300.json#615:description` |
| `FEARFULCITIZEN` | fearful citizen (nome da contestualizzare) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#311:description` |
| `FEARFULCITIZENA` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#347:description` |
| `FEARFULCITIZENB` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#348:description` |
| `FEMALEHERETIC` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03004.json#3:description` |
| `FEOUL` | [Feo Ul](SHADOWBRINGERS.md) | 73 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#140:description` |
| `FIREMONOLITH` | [Rubicante](ENDWALKER.md) | 41 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/062/voiceman_06200.json#201:description` |
| `FLAMESSOLDIER` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#213:description` |
| `FLAVIEN` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#328:description` |
| `FOLLOWEROFTELEDJI` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#256:description` |
| `FORDOLA` | [Fordola](STORMBLOOD.md) | 104 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#140:description` |
| `FORDOLAFATHER` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#69:description` |
| `FORDOLAMOTHER` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#66:description` |
| `FORMERMINIONVOIDSENT` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/062/voiceman_06200.json#267:description` |
| `FOURCHENAULT` | [Fourchenault](ENDWALKER.md) | 167 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/055/voiceman_05505.json#43:description` |
| `FYRGEISS` | Fyrgeiss (nome da contestualizzare) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06005.json#319:description` |
| `G3F1IMPERIALCENTURION` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04005.json#30:description` |
| `G3F2VILLAGER` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#128:description` |
| `GABU` | Ga Bu (nome da contestualizzare) | 12 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#133:description` |
| `GAIA` | [Gaia](SHADOWBRINGERS.md) | 15 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05201.json#1:description` |
| `GALENE` | Galene (nome da contestualizzare) | 15 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06004.json#528:description` |
| `GALUF` | Identità / ruolo da contestualizzare | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#337:description` |
| `GARLEANGENERAL` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#603:description` |
| `GARLEANHARPOONER` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04005.json#164:description` |
| `GARLEANLEADERA` | Identità / ruolo da contestualizzare | 7 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#142:description` |
| `GARLEANLEADERB` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#145:description` |
| `GARLEANLEADERC` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04004.json#189:description` |
| `GARLEANLEADERD` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04004.json#197:description` |
| `GARLEANLEADERDOMA` | Identità / ruolo da contestualizzare | 16 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#2:description` |
| `GARLEANOFFICER` | Identità / ruolo da contestualizzare | 7 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04005.json#151:description` |
| `GARLEANSOLDIER10320` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/063/voiceman_06300.json#173:description` |
| `GARLEANSOLDIER10410` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/063/voiceman_06300.json#189:description` |
| `GARUDA` | [Garuda](ARR.md) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06006.json#234:description` |
| `GATETOWNMAN` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#264:description` |
| `GATETOWNMIDDLEAGE` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#253:description` |
| `GATETOWNWOMAN` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#265:description` |
| `GENOLT` | Genolt (nome da contestualizzare) | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#193:description` |
| `GEODE` | [Geode](DAWNTRAIL.md) | 20 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#704:description` |
| `GHENGEN` | Ghen Gen (nome da contestualizzare) | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#382:description` |
| `GIANTCOLIBRI` | giant colibri (nome da contestualizzare) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/065/voiceman_06505.json#77:description` |
| `GILGAMESH` | [Gilgamesh](RECURRING.md) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#259:description` |
| `GIRL05426` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#64:description` |
| `GLYNARD` | Glynard (nome da contestualizzare) | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#62:description` |
| `GNATHPRIEST` | Gnath priest (nome da contestualizzare) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#96:description` |
| `GOSETSU` | [Gosetsu](STORMBLOOD.md) | 175 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/035/voiceman_03500.json#116:description` |
| `GOSSAN` | Gossan (nome da contestualizzare) | 39 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#0:description` |
| `GRAHATIA` | [G'raha Tia](RECURRING.md) | 502 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/053/voiceman_05300.json#589:description` |
| `GRIDANIANDELEGATION` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#556:description` |
| `GRINNAUX` | Grinnaux (nome da contestualizzare) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#95:description` |
| `GRITHIL` | Grithil (nome da contestualizzare) | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#198:description` |
| `GROWINGWAY` | [Growingway](ENDWALKER.md) | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06002.json#119:description` |
| `GRYNEWAHT` | Grynewaht (nome da contestualizzare) | 21 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/035/voiceman_03500.json#204:description` |
| `GUARD02500` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/025/voiceman_02500.json#403:description` |
| `GULOOLJA` | [Gulool Ja](DAWNTRAIL.md) | 76 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#285:description` |
| `GULOOLJAJA` | [Gulool Ja Ja](DAWNTRAIL.md) | 142 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#150:description` |
| `GURFURLUR` | [Gurfurlur](DAWNTRAIL.md) | 27 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07001.json#198:description` |
| `GUTHJON` | Guthjon (nome da contestualizzare) | 9 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#136:description` |
| `GYODO` | Gyodo (nome da contestualizzare) | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04001.json#47:description` |
| `HAAMBORTMAN` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#128:description` |
| `HALDRATH` | Haldrath (nome da contestualizzare) | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#320:description` |
| `HALMARUT` | [Halmarut](DAWNTRAIL.md) | 64 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#446:description` |
| `HALRIC` | Halric (nome da contestualizzare) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/051/voiceman_05100.json#146:description` |
| `HANCOCK` | [Hancock](STORMBLOOD.md) | 58 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04001.json#2:description` |
| `HANNISHBOYA` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#91:description` |
| `HANNISHBOYB` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#272:description` |
| `HANNISHMAN04735A` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#31:description` |
| `HANNISHMAN04735B` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#32:description` |
| `HANNISHMANA` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#90:description` |
| `HANNISHMANB` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#265:description` |
| `HANNISHOLDWOMANA` | Identità / ruolo da contestualizzare | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#97:description` |
| `HANNISHOLDWOMANB` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#271:description` |
| `HANNISHWOMANA` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#92:description` |
| `HANUHANUBEARERA` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#437:description` |
| `HANUHANUBEARERB` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#438:description` |
| `HANUHANUBEARERC` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#439:description` |
| `HANUHANUBEARERD` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#440:description` |
| `HANUHANUMALEAUDA` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#299:description` |
| `HAURCHEFANT` | [Haurchefant](HEAVENSWARD.md) | 71 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/023/voiceman_02300.json#59:description` |
| `HEPHAESTUS` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/062/voiceman_06201.json#0:description` |
| `HERMES` | [Hermes](ENDWALKER.md) | 209 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06004.json#117:description` |
| `HHETSARROMAN` | Identità / ruolo da contestualizzare | 15 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#73:description` |
| `HHUSATAHWIAUDA` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#23:description` |
| `HHUSATAHWIAUDSB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#24:description` |
| `HHUSATAHWIFEMALEA` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#27:description` |
| `HHUSATAHWIFEMALEB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#64:description` |
| `HHUSATAHWIMALEA` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#65:description` |
| `HHUSATAHWIMALEB` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#87:description` |
| `HHWATO` | Hhwato (nome da contestualizzare) | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#79:description` |
| `HIEN` | [Hien](STORMBLOOD.md) | 322 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04003.json#27:description` |
| `HIGHLANDERGLADIATOR` | Highlander gladiator (nome da contestualizzare) | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#131:description` |
| `HILDA` | [Hilda](HEAVENSWARD.md) | 16 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03004.json#70:description` |
| `HOARYBOULDER` | [Hoary Boulder](RECURRING.md) | 18 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#27:description` |
| `HONOROIT` | [Honoroit](HEAVENSWARD.md) | 13 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#122:description` |
| `HONU` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#73:description` |
| `HOOBIGOBAKOOLJAJAMINIONSA` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#682:description` |
| `HOOBIGOBAKOOLJAJAMINIONSB` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#689:description` |
| `HOOBIGOFEMALEAUD` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#300:description` |
| `HOOBIGOMALESOLDIER` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#83:description` |
| `HOOBIGOMALEVILLAGER` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07002.json#67:description` |
| `HOUSEFORTEMPSSTEWARD` | House Fortemps steward (nome da contestualizzare) | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#84:description` |
| `HRAESVELGR` | [Hraesvelgr](HEAVENSWARD.md) | 84 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#131:description` |
| `HUNGARYGARLEAN10320` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/063/voiceman_06300.json#134:description` |
| `HUNMURRUK` | [Hunmu Rruk](DAWNTRAIL.md) | 31 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07002.json#19:description` |
| `HYDAELYN` | [Venat](ENDWALKER.md) | 134 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03005.json#3:description` |
| `HYLLFYR` | Hyllfyr (nome da contestualizzare) | 14 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#224:description` |
| `HYPERBOREIASTAFF` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06004.json#822:description` |
| `HYTHLODAEUS` | [Hythlodaeus](ENDWALKER.md) | 246 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#512:description` |
| `ICEHEART` | [Ysayle](HEAVENSWARD.md) | 118 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/024/voiceman_02400.json#88:description` |
| `IGEYOHRM` | Identità / ruolo da contestualizzare | 15 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#126:description` |
| `IGEYORHM` | Igeyorhm (nome da contestualizzare) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/053/voiceman_05300.json#120:description` |
| `IKARUGA` | Ikaruga (nome da contestualizzare) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/042/voiceman_04200.json#32:description` |
| `ILBERD` | [Ilberd](ARR.md) | 66 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/024/voiceman_02400.json#0:description` |
| `IMMIGRATIONCONTROLSTAFF` | Identità / ruolo da contestualizzare | 19 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#100:description` |
| `IMPERIALCENTURION` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/042/voiceman_04200.json#45:description` |
| `IMPERIALDOCTOR` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/042/voiceman_04200.json#289:description` |
| `IMPERIALENGINEER` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/045/voiceman_04500.json#158:description` |
| `IMPERIALSOLDIER` | imperial soldier (nome da contestualizzare) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/055/voiceman_05505.json#130:description` |
| `IMPERIALSOLDIER04505` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/045/voiceman_04505.json#115:description` |
| `IRONMASK` | Identità / ruolo da contestualizzare | 26 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/034/voiceman_03400.json#169:description` |
| `IRONWORKSENGINEER04500` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/045/voiceman_04500.json#98:description` |
| `IRRITATION05306` | Identità / ruolo da contestualizzare | 9 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#314:description` |
| `IRVITHE` | Irvithe (nome da contestualizzare) | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#197:description` |
| `ISADORA` | Isadora (nome da contestualizzare) | 7 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#278:description` |
| `ISHGARDIANPRIEST` | Ishgardian priest (nome da contestualizzare) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/031/voiceman_03100.json#149:description` |
| `ISSE` | [Isse](STORMBLOOD.md) | 27 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#279:description` |
| `IXALICHIEFTAIN` | Ixali chieftain (nome da contestualizzare) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/055/voiceman_05505.json#1:description` |
| `JIFUYA` | [Jifuya](STORMBLOOD.md) | 12 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/042/voiceman_04200.json#102:description` |
| `JOURNEYSHEADNURSE` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#477:description` |
| `JOURNEYSHEADPATIENT` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#476:description` |
| `JULLUS` | [Jullus](ENDWALKER.md) | 168 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#329:description` |
| `KAIROS` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06004.json#746:description` |
| `KAISHIRR` | Kai-Shirr (nome da contestualizzare) | 22 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#283:description` |
| `KANESENNA` | [Kan-E-Senna](ARR.md) | 102 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/023/voiceman_02300.json#82:description` |
| `KANILOKKA` | Kanilokka (nome da contestualizzare) | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#41:description` |
| `KATANASOLDIER` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/045/voiceman_04505.json#114:description` |
| `KATLISS` | Katliss (nome da contestualizzare) | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#66:description` |
| `KEMAKKA` | Kemakka (nome da contestualizzare) | 20 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#39:description` |
| `KETENRAMM` | [Ketenramm](DAWNTRAIL.md) | 35 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07002.json#377:description` |
| `KETILL` | Ketill (nome da contestualizzare) | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07505.json#111:description` |
| `KETONYE` | Ketonye (nome da contestualizzare) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#564:description` |
| `KIDNAPPEDPIRATE` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#209:description` |
| `KOANA` | [Koana](DAWNTRAIL.md) | 186 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#192:description` |
| `KOJINSOLDIER` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#217:description` |
| `KOKKOLDANKKOL` | Kokkol Dankkol (nome da contestualizzare) | 33 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06005.json#244:description` |
| `KOUJINTAICHO` | Identità / ruolo da contestualizzare | 9 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#164:description` |
| `KRAKENSDECKHAND` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#283:description` |
| `KRILE` | [Krile](RECURRING.md) | 747 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/031/voiceman_03100.json#30:description` |
| `KYTTE` | Kytte (nome da contestualizzare) | 7 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#449:description` |
| `LADYINWAITING2` | Identità / ruolo da contestualizzare | 9 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/024/voiceman_02400.json#345:description` |
| `LAHABREA` | [Lahabrea](ARR.md) | 23 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#404:description` |
| `LAKSHMI` | [Lakshmi](STORMBLOOD.md) | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04005.json#71:description` |
| `LASTWOMAN71` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#241:description` |
| `LAURENTIUS` | [Laurentius](ARR.md) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/035/voiceman_03500.json#59:description` |
| `LEGISLATORA` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#346:description` |
| `LEGISLATORB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#368:description` |
| `LINERCREW` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#32:description` |
| `LINUHANU` | Linuhanu (nome da contestualizzare) | 20 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#382:description` |
| `LIVINGWAY` | [Livingway](ENDWALKER.md) | 140 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06002.json#57:description` |
| `LOLORITO` | [Lolorito](ARR.md) | 55 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#46:description` |
| `LOUISOIX` | [Louisoix](RECURRING.md) | 57 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/024/voiceman_02401.json#6:description` |
| `LUCIA` | [Lucia](HEAVENSWARD.md) | 192 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/024/voiceman_02400.json#49:description` |
| `LUMULL` | [Lumull](DAWNTRAIL.md) | 61 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#115:description` |
| `LYNA` | [Lyna](SHADOWBRINGERS.md) | 101 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#48:description` |
| `LYSE` | [Lyse](RECURRING.md) | 341 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#9:description` |
| `LYSENARRATION` | [Lyse](RECURRING.md) | 28 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#50:description` |
| `M017` | M-017 (nome da contestualizzare) | 28 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06006.json#495:description` |
| `MABLU` | [Mablu](DAWNTRAIL.md) | 14 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#528:description` |
| `MAELIE` | Maelie (nome da contestualizzare) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/031/voiceman_03100.json#155:description` |
| `MAELSTROMOFFICER` | Maelstrom officer (nome da contestualizzare) | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/044/voiceman_04400.json#289:description` |
| `MAGNAI` | [Magnai](STORMBLOOD.md) | 36 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04003.json#119:description` |
| `MAGNUS` | Magnus (nome da contestualizzare) | 17 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#103:description` |
| `MAHUWSA` | Mahuwsa (nome da contestualizzare) | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#260:description` |
| `MALACHITE` | Malachite (nome da contestualizzare) | 17 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#353:description` |
| `MALEHERETIC` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03004.json#2:description` |
| `MAPPINGWAY` | Mappingway (nome da contestualizzare) | 27 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06002.json#69:description` |
| `MASTERSTIGMAONE` | Identità / ruolo da contestualizzare | 15 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06006.json#473:description` |
| `MATOYA` | [Matoya](RECURRING.md) | 76 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03006.json#64:description` |
| `MATSYA` | [Matsya](ENDWALKER.md) | 35 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#100:description` |
| `MAXIMA` | [Maxima](STORMBLOOD.md) | 52 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/043/voiceman_04300.json#139:description` |
| `MEFFRID` | [Meffrid](STORMBLOOD.md) | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#125:description` |
| `MEHRAHD` | Mehrahd (nome da contestualizzare) | 7 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#48:description` |
| `MEHRYDE` | Mehryde (nome da contestualizzare) | 9 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#1:description` |
| `MEHVAN` | Mehvan (nome da contestualizzare) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#212:description` |
| `MEMORIAMASTERA04661` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#393:description` |
| `MEMORIAMASTERA10430` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/063/voiceman_06300.json#201:description` |
| `MEMORIAMASTERB10430` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/063/voiceman_06300.json#202:description` |
| `MEMORY` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato; include testo non inglese | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#27:description` |
| `MENPHINA` | [Menphina](SIDE_STORIES.md) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/063/voiceman_06301.json#3:description` |
| `MERLWYB` | [Merlwyb](ARR.md) | 138 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#130:description` |
| `MESOTERMINAL` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07005.json#114:description` |
| `MESSENGER02341` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/034/voiceman_03400.json#21:description` |
| `METEION` | [Meteion](ENDWALKER.md) | 194 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06004.json#115:description` |
| `METEM` | [Metem](SIDE_STORIES.md) | 18 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07010.json#0:description` |
| `METTIUS` | Mettius (nome da contestualizzare) | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#172:description` |
| `MIAYLI` | Miayli (nome da contestualizzare) | 62 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#124:description` |
| `MIDGARDSORMR` | [Midgardsormr](HEAVENSWARD.md) | 63 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/025/voiceman_02500.json#24:description` |
| `MIHLEEL` | Mihleel (nome da contestualizzare) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#340:description` |
| `MIILALJA` | Miilal Ja (nome da contestualizzare) | 9 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07002.json#284:description` |
| `MILOS` | Milos (nome da contestualizzare) | 12 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#283:description` |
| `MINFILIA` | [Minfilia](RECURRING.md) | 259 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#83:description` |
| `MITRON` | [Mitron](SHADOWBRINGERS.md) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07102.json#2:description` |
| `MNAAGO` | [M'naago](STORMBLOOD.md) | 25 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/035/voiceman_03500.json#18:description` |
| `MOENBRYDA` | [Moenbryda](ARR.md) | 80 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/024/voiceman_02400.json#117:description` |
| `MONTICHAIGNE` | Montichaigne (nome da contestualizzare) | 52 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#380:description` |
| `MORDMERCHANTA` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#396:description` |
| `MORDMERCHANTB` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#397:description` |
| `MORDMERCHANTC` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#398:description` |
| `MOREN` | Moren (nome da contestualizzare) | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#553:description` |
| `MYSTERIOUSCOOPERATOR` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#394:description` |
| `MYSTERIOUSLADY` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/055/voiceman_05505.json#166:description` |
| `MYSTERIOUSPERSON` | [Zero](ENDWALKER.md) | 426 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#42:description` |
| `MYSTERIOUSVOICE` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07500.json#198:description` |
| `MYSTERYVOICE` | Identità / ruolo da contestualizzare | 592 | 0 | SCD en + testo candidato; include testo non inglese | `data/da_tradurre/dialogue/cut_scene/044/voiceman_04400.json#222:description` |
| `NAAHE` | Naahe (nome da contestualizzare) | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#314:description` |
| `NABAATHARENGPEOPLEA` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#256:description` |
| `NABAATHARENGPEOPLEB` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#257:description` |
| `NABAATHARENGPEOPLEC` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#258:description` |
| `NABAATHARENGPEOPLED` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#259:description` |
| `NABRIALES` | Nabriales (nome da contestualizzare) | 37 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/024/voiceman_02400.json#185:description` |
| `NAHBDEEN` | Nahbdeen (nome da contestualizzare) | 31 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#313:description` |
| `NALDTHAL` | [Nald'thal](SIDE_STORIES.md) | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06101.json#0:description` |
| `NAMIKKA` | [Namikka](DAWNTRAIL.md) | 20 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#306:description` |
| `NANAMO` | [Nanamo Ul Namo](ARR.md) | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#264:description` |
| `NANAMOSTEWARD` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#140:description` |
| `NANAMOULNAMO` | [Nanamo Ul Namo](ARR.md) | 174 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#34:description` |
| `NARRATION` | Identità / ruolo da contestualizzare | 0 | 0 | Nessun SCD riscontrato nel percorso verificato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#26:description` |
| `NERO` | [Nero](RECURRING.md) | 32 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/035/voiceman_03500.json#137:description` |
| `NEWSPHENE` | [Sphene](DAWNTRAIL.md) | 44 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#209:description` |
| `NEXTHEROBOY` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#441:description` |
| `NIDHANA` | [Nidhana](ENDWALKER.md) | 121 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#470:description` |
| `NIDHOGG` | [Nidhogg](HEAVENSWARD.md) | 23 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#314:description` |
| `NITOWIKWE` | Nitowikwe (nome da contestualizzare) | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#451:description` |
| `NOKKEN` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#432:description` |
| `NOPHICA` | [Nophica](SIDE_STORIES.md) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/063/voiceman_06301.json#0:description` |
| `NOSTALGIA` | Nostalgia (nome da contestualizzare) | 26 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#344:description` |
| `OBLIVIONWOMAN73` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#79:description` |
| `OFFICECLERK` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06005.json#409:description` |
| `OLDEULMOREMAYOR` | Identità / ruolo da contestualizzare | 7 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05004.json#403:description` |
| `OLDTITANIA` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#364:description` |
| `OLDWOMAN02236` | Identità / ruolo da contestualizzare | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#90:description` |
| `ORIGINALSPHENE` | [Sphene](DAWNTRAIL.md) | 212 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#11:description` |
| `ORONIRMAN` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04003.json#5:description` |
| `OSCHON` | [Deryk](SIDE_STORIES.md) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/065/voiceman_06501.json#0:description` |
| `OTIS` | [Otis](DAWNTRAIL.md) | 53 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#276:description` |
| `OULSIGUN` | Oul Sigun (nome da contestualizzare) | 12 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#154:description` |
| `OUTSKIRTSCAREGIVERA` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#629:description` |
| `OUTSKIRTSCAREGIVERB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#628:description` |
| `OUTSKIRTSFARMERA` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#643:description` |
| `OUTSKIRTSFARMERB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#642:description` |
| `PAKWENE` | Pakwene (nome da contestualizzare) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#563:description` |
| `PALADINOFDARKNESS` | [Branden](SHADOWBRINGERS.md) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/034/voiceman_03400.json#59:description` |
| `PALAKASMAN` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#179:description` |
| `PALAKASOLDMAN` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#192:description` |
| `PALAKASWOMANA` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#180:description` |
| `PALAKASWOMANB` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06003.json#183:description` |
| `PANDAEMONIUM` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06401.json#2:description` |
| `PANICFISHERMAN10720` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/063/voiceman_06300.json#284:description` |
| `PANICKEDFIGHTER` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/045/voiceman_04505.json#4:description` |
| `PAPALYMO` | [Papalymo](RECURRING.md) | 62 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/024/voiceman_02400.json#129:description` |
| `PASSER02356` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/035/voiceman_03500.json#117:description` |
| `PEDDLEROFNORVRANDT` | Identità / ruolo da contestualizzare | 9 | 1 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#31:description` |
| `PELUPELUMALEAUDA` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#272:description` |
| `PEOPLEOFNIGHTA` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05002.json#74:description` |
| `PEOPLEOFNIGHTB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05002.json#75:description` |
| `PEPOANE` | Pepoane (nome da contestualizzare) | 22 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#574:description` |
| `PESHIWA` | Peshiwa (nome da contestualizzare) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#186:description` |
| `PIPIN` | [Pipin](ARR.md) | 33 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#102:description` |
| `PIPPIN` | [Pipin](ARR.md) | 12 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/025/voiceman_02500.json#479:description` |
| `PRESERVATIONMAN73` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#178:description` |
| `PRESERVATIONWOMAN73` | Identità / ruolo da contestualizzare | 9 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#179:description` |
| `PRISHE` | [Prishe](SIDE_STORIES.md) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07510.json#2:description` |
| `PROPYLAIONSTAFF` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06004.json#59:description` |
| `PUDDINGWAY` | [Puddingway](ENDWALKER.md) | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06002.json#183:description` |
| `PUPPETEERVOIDCENT` | Identità / ruolo da contestualizzare | 15 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07500.json#120:description` |
| `PUPPETMAN` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05004.json#105:description` |
| `QALYANAPATRIARCH` | Identità / ruolo da contestualizzare | 9 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04005.json#70:description` |
| `QUEENACTOR` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07005.json#213:description` |
| `QUINTUS` | [Quintus](ENDWALKER.md) | 41 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#355:description` |
| `RAGANFRID` | Raganfrid (nome da contestualizzare) | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#229:description` |
| `RAMUH` | Ramuh (nome da contestualizzare) | 22 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/023/voiceman_02300.json#96:description` |
| `RANJIT` | [Ran'jit](SHADOWBRINGERS.md) | 55 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#12:description` |
| `RASHO` | [Rasho](STORMBLOOD.md) | 20 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#67:description` |
| `RAUBAHN` | [Raubahn](ARR.md) | 325 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#38:description` |
| `RAUBAHNMERLWYBKANESENNA` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03008.json#5:description` |
| `RAVANA` | [Ravana](HEAVENSWARD.md) | 15 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#99:description` |
| `READEROFONDO` | Identità / ruolo da contestualizzare | 9 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05005.json#215:description` |
| `REFEREE` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07010.json#1:description` |
| `REFUGEEMAN` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06005.json#1:description` |
| `REGULA` | [Regula van Hydrus](HEAVENSWARD.md) | 18 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03005.json#18:description` |
| `REQUESTEDLEGISLATOR` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#279:description` |
| `RESIDENTBOY05300` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#99:description` |
| `RESIDENTMAN05300` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#102:description` |
| `RESIDENTTREA` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#333:description` |
| `RESIDENTTREB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#360:description` |
| `RESIDENTTREC` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#361:description` |
| `RESISTANCE` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/042/voiceman_04200.json#247:description` |
| `RESISTANCEFIGHTER` | Resistance fighter (nome da contestualizzare) | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/045/voiceman_04505.json#30:description` |
| `RESISTANCEHEALER` | Resistance healer (nome da contestualizzare) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/044/voiceman_04400.json#241:description` |
| `RESISTANCEWARRIOR` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#351:description` |
| `RESOLUTESOLDIER` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#62:description` |
| `RESTLESSGARLEAN10320` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/063/voiceman_06300.json#136:description` |
| `REVOLUTIONARY1` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03004.json#62:description` |
| `REVOLUTIONARY2` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03004.json#66:description` |
| `RHETKYMPF` | Rhetkympf (nome da contestualizzare) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/065/voiceman_06505.json#251:description` |
| `ROBEMAN` | Identità / ruolo da contestualizzare | 28 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/042/voiceman_04200.json#290:description` |
| `ROBOR` | Robor (nome da contestualizzare) | 32 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07005.json#268:description` |
| `ROEGADYNGLADIATOR` | Roegadyn gladiator (nome da contestualizzare) | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/041/voiceman_04100.json#133:description` |
| `RUNAR` | [Runar](SHADOWBRINGERS.md) | 50 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05002.json#71:description` |
| `RYNE` | [Ryne](SHADOWBRINGERS.md) | 232 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#600:description` |
| `SADU` | [Sadu](STORMBLOOD.md) | 39 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04003.json#135:description` |
| `SAHAGINPRIEST` | Sahagin priest (nome da contestualizzare) | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/055/voiceman_05505.json#5:description` |
| `SAILORPANICVOICEA` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#38:description` |
| `SAILORPANICVOICEB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#40:description` |
| `SAILORPANICVOICEC` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#41:description` |
| `SAILORSUNBURNED` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#9:description` |
| `SALVAGECREWA` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07505.json#116:description` |
| `SALVAGECREWB` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07505.json#131:description` |
| `SAREELJA` | [Sareel Ja](DAWNTRAIL.md) | 35 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#257:description` |
| `SEIRYU` | [Seiryu](SIDE_STORIES.md) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/045/voiceman_04501.json#4:description` |
| `SEKISHINTAISOLDIER` | Identità / ruolo da contestualizzare | 7 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04001.json#57:description` |
| `SERPENTSSOLDIER` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#118:description` |
| `SETO` | [Seto](SHADOWBRINGERS.md) | 36 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#370:description` |
| `SHAALOANIPASSENGER` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#118:description` |
| `SHADOWHUNTER` | [Gaius](ARR.md) | 85 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/045/voiceman_04500.json#107:description` |
| `SHADOWLORD` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07101.json#0:description` |
| `SHALE` | [Shale](DAWNTRAIL.md) | 118 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#194:description` |
| `SHANTOTTO` | [Shantotto](SIDE_STORIES.md) | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07510.json#6:description` |
| `SHARLAYANCITIZENA` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06002.json#248:description` |
| `SHARLAYANCITIZENB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06002.json#249:description` |
| `SHENZA` | Shenza (nome da contestualizzare) | 22 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#117:description` |
| `SHETONAFEMALEAUD` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#307:description` |
| `SHETONAMALEAUD` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#422:description` |
| `SHIVA` | Shiva (nome da contestualizzare) | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/024/voiceman_02400.json#173:description` |
| `SICARD` | [Sicard](ARR.md) | 39 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#195:description` |
| `SINGINGWAY` | Singingway (nome da contestualizzare) | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06002.json#191:description` |
| `SKULLSOLDIERA` | Identità / ruolo da contestualizzare | 6 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#127:description` |
| `SKULLSOLDIERB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#133:description` |
| `SKULLSOLDIERC` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04005.json#33:description` |
| `SLEEPINGWAY` | Sleepingway (nome da contestualizzare) | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06006.json#229:description` |
| `SOLDIER03608` | Identità / ruolo da contestualizzare | 15 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#68:description` |
| `SOLDIERA` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07500.json#93:description` |
| `SOLDIERA02354` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/035/voiceman_03500.json#56:description` |
| `SOLDIERB` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07500.json#96:description` |
| `SOLDIERB02354` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/035/voiceman_03500.json#57:description` |
| `SOLDIERC02354` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/035/voiceman_03500.json#58:description` |
| `SOLUSZOSGALVUS` | [Emet-Selch](SHADOWBRINGERS.md) | 40 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/044/voiceman_04400.json#318:description` |
| `SOLUTIONNFIGHTER` | Identità / ruolo da contestualizzare | 23 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#138:description` |
| `SOLUTIONRESIDENT73` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#1:description` |
| `SOMEONEOFFUATHA` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#310:description` |
| `SOMEONEOFFUATHB` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#311:description` |
| `SOMEONEOFFUATHC` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#312:description` |
| `SOMEONEOFFUATHD` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#313:description` |
| `SOMEONEOFNUMOUA` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#462:description` |
| `SOMEONEOFNUMOUB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#463:description` |
| `SONGALAMHIGO_NONE_VOICE` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04006.json#158:description` |
| `SOROBAN` | [Soroban](STORMBLOOD.md) | 55 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04001.json#56:description` |
| `SOULEATER` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06401.json#0:description` |
| `SPHENE` | [Sphene](DAWNTRAIL.md) | 249 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#506:description` |
| `SPYVOIDSENT` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/062/voiceman_06200.json#244:description` |
| `STORMOFFICER` | storm officer (nome da contestualizzare) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#295:description` |
| `SULUIN` | Sul Uin (nome da contestualizzare) | 11 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#153:description` |
| `SUSANO` | [Susano](STORMBLOOD.md) | 12 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#168:description` |
| `SUZAKU` | [Suzaku](SIDE_STORIES.md) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/044/voiceman_04401.json#2:description` |
| `SYLVETREL` | Identità / ruolo da contestualizzare | 10 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#319:description` |
| `SYSTEM` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#282:description` |
| `SYSTEM_000` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#161:description` |
| `SYSTEM_010` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#162:description` |
| `SYSTEM_020` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#163:description` |
| `SYSTEM_030` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#164:description` |
| `SYSTEM_040` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#165:description` |
| `SYSTEM_050` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#166:description` |
| `SYSTEM_060` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#167:description` |
| `SYSTEM_NONE_VOICE` | Identità / ruolo da contestualizzare | 0 | 0 | Vuoto / tecnico / NONE_VOICE | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#140:description` |
| `TAAPU` | Taapu (nome da contestualizzare) | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/073/voiceman_07300.json#263:description` |
| `TANSUI` | Tansui (nome da contestualizzare) | 37 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#0:description` |
| `TATARU` | [Tataru](RECURRING.md) | 173 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/025/voiceman_02500.json#521:description` |
| `TEESHALJA` | Teeshal Ja (nome da contestualizzare) | 33 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/071/voiceman_07100.json#49:description` |
| `TELEDJIADELEDJI` | [Teledji Adeledji](ARR.md) | 43 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#43:description` |
| `TEMPLEKNIGHT02400` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/025/voiceman_02500.json#344:description` |
| `TEMPLEKNIGHT2` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03004.json#13:description` |
| `TEMPLERKNIGHT10810` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/031/voiceman_03100.json#131:description` |
| `TEMULUN` | Temulun (nome da contestualizzare) | 10 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04003.json#56:description` |
| `TESHITAPIRATE` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#5:description` |
| `TESLEEN` | [Tesleen](SHADOWBRINGERS.md) | 21 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#454:description` |
| `THAFFE` | Thaffe (nome da contestualizzare) | 15 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05003.json#94:description` |
| `THANCRED` | [Thancred](RECURRING.md) | 615 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#0:description` |
| `THEMIS` | [Themis](SIDE_STORIES.md) | 26 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06401.json#4:description` |
| `THETWELVE` | Identità / ruolo da contestualizzare | 8 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/065/voiceman_06501.json#2:description` |
| `THEVA` | Theva (nome da contestualizzare) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#220:description` |
| `THORDAN` | [Thordan VII](HEAVENSWARD.md) | 73 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#106:description` |
| `TIAMAT` | [Tiamat](HEAVENSWARD.md) | 42 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03007.json#57:description` |
| `TIMID05306` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#317:description` |
| `TIREDDRAGON` | Identità / ruolo da contestualizzare | 13 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06006.json#344:description` |
| `TOBLI` | Tobli (nome da contestualizzare) | 18 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#524:description` |
| `TONAWAATAUQMALEAUDA` | Identità / ruolo da contestualizzare | 7 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#252:description` |
| `TONAWAATAUQMALEAUDB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#439:description` |
| `TRAINSYSTEM` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/074/voiceman_07400.json#286:description` |
| `TRAITORB02236` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#97:description` |
| `TRAITORC02236` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#98:description` |
| `TREPIDATION05306` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#315:description` |
| `TRIALOFFICIANT` | Identità / ruolo da contestualizzare | 7 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#93:description` |
| `TSUKUYOMI` | Tsukuyomi (nome da contestualizzare) | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/043/voiceman_04300.json#151:description` |
| `TULIYOLLALCHILD` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#192:description` |
| `TULIYOLLALFEMALE` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07005.json#22:description` |
| `TULIYOLLALMALE` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07005.json#21:description` |
| `TULIYOLLALSOLDIERA` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#180:description` |
| `TULIYOLLALSOLDIERB` | Identità / ruolo da contestualizzare | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#181:description` |
| `TULIYOLLALSOLDIERC` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#183:description` |
| `TULIYOLLALSOLDIERD` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#188:description` |
| `TURMOIL05306` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#386:description` |
| `UIMET` | Uimet (nome da contestualizzare) | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05002.json#265:description` |
| `ULDAHNDELEGATION` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#555:description` |
| `ULTIMA` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/045/voiceman_04501.json#0:description` |
| `UNSETTLEDRESEARCHER` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/053/voiceman_05300.json#603:description` |
| `URIANGER` | [Urianger](RECURRING.md) | 626 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#2:description` |
| `VALIGARMANDA` | Valigarmanda (nome da contestualizzare) | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07001.json#254:description` |
| `VARIS` | [Varis zos Galvus](ARR.md) | 78 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03005.json#20:description` |
| `VARSHAHN` | [Vrtra](ENDWALKER.md) | 149 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#475:description` |
| `VARSHAHNBIG` | [Vrtra](ENDWALKER.md) | 145 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/062/voiceman_06200.json#0:description` |
| `VAUTHRY` | [Vauthry](SHADOWBRINGERS.md) | 78 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05000.json#341:description` |
| `VENAS` | [Venat](ENDWALKER.md) | 236 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/052/voiceman_05200.json#305:description` |
| `VIDOFNIR` | [Vidofnir](HEAVENSWARD.md) | 24 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#77:description` |
| `VIGILANTEMALEA` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#93:description` |
| `VILLAGEVOIDSENTA` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/062/voiceman_06200.json#231:description` |
| `VILLAGEVOIDSENTB` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/062/voiceman_06200.json#232:description` |
| `VILLAGEVOIDSENTC` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/062/voiceman_06200.json#233:description` |
| `VILLAGEVOIDSENTD` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/062/voiceman_06200.json#234:description` |
| `VIRASOLDIER` | Identità / ruolo da contestualizzare | 15 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04005.json#45:description` |
| `VISNA` | [Visna](DAWNTRAIL.md) | 82 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/075/voiceman_07505.json#49:description` |
| `VOICEGUIDE` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06004.json#0:description` |
| `VOIDPERSON04735` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#402:description` |
| `VOIDSENT04735` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/064/voiceman_06400.json#313:description` |
| `VOIDSENT10410` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/063/voiceman_06300.json#192:description` |
| `VOIDSENTA` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#458:description` |
| `VOIDSENTB` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#459:description` |
| `VOIDSENTC` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#460:description` |
| `VOIDSENTD` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#461:description` |
| `VORPORLOR` | Vorporlor (nome da contestualizzare) | 39 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07001.json#163:description` |
| `VRITRA` | [Vrtra](ENDWALKER.md) | 155 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#654:description` |
| `WAITEROFTHELASTSTAND` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06006.json#43:description` |
| `WAITRESS02236` | Identità / ruolo da contestualizzare | 20 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/032/voiceman_03200.json#80:description` |
| `WARRIOROFDARKNESS` | [Ardbert](SHADOWBRINGERS.md) | 280 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/031/voiceman_03100.json#84:description` |
| `WARRIOROFLIGHTAVATARS` | Identità / ruolo da contestualizzare | 2 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/053/voiceman_05300.json#226:description` |
| `WATCHEROFMOON` | [The Watcher](ENDWALKER.md) | 73 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#726:description` |
| `WATERMONOLITH` | [Cagnazzo](ENDWALKER.md) | 13 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/062/voiceman_06200.json#196:description` |
| `WAWLIKA` | [Wawlika](DAWNTRAIL.md) | 27 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#137:description` |
| `WAYAKKWE` | Wayakkwe (nome da contestualizzare) | 11 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07004.json#366:description` |
| `WEDGE` | [Wedge](RECURRING.md) | 16 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#205:description` |
| `WHITEMAGEOFDARKNESS` | [Lamitt](SHADOWBRINGERS.md) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/034/voiceman_03400.json#63:description` |
| `WICKEDTHUNDER` | [Eutrope](SIDE_STORIES.md) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07010.json#6:description` |
| `WIHUWTE` | Wihuwte (nome da contestualizzare) | 25 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#15:description` |
| `WILFSUNN` | Wilfsunn (nome da contestualizzare) | 12 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06005.json#361:description` |
| `WINDMONOLITH` | [Barbariccia](ENDWALKER.md) | 25 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/062/voiceman_06200.json#199:description` |
| `WOUNDEDIMPERIAL` | wounded imperial (nome da contestualizzare) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/042/voiceman_04200.json#263:description` |
| `WOUNDEDOLDMAN` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#334:description` |
| `WRENDEN` | Wrenden (nome da contestualizzare) | 11 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/051/voiceman_05100.json#176:description` |
| `WUKEVU` | [Wuk Evu](DAWNTRAIL.md) | 42 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#357:description` |
| `WUKLAMAT` | [Wuk Lamat](DAWNTRAIL.md) | 1026 | 0 | SCD en + testo candidato; include testo non inglese | `data/da_tradurre/dialogue/cut_scene/065/voiceman_06505.json#10:description` |
| `WUUR` | Identità / ruolo da contestualizzare | 13 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#96:description` |
| `XANDE` | Xande (nome da contestualizzare) | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06000.json#613:description` |
| `XBALAALMALEAUDA` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#251:description` |
| `XBALAALMALEAUDB` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#273:description` |
| `XBALAALMALECUSTOMER` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#98:description` |
| `XBALAALMALESOLDIER` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#81:description` |
| `XBALAALMALEVILLAGER` | Identità / ruolo da contestualizzare | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07002.json#66:description` |
| `YDA` | [Lyse](RECURRING.md) | 66 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/024/voiceman_02400.json#125:description` |
| `YESTERLANDBOY` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07005.json#176:description` |
| `YESTERLANDGIRL` | Identità / ruolo da contestualizzare | 4 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07005.json#175:description` |
| `YESTERLANDTROUPE` | Identità / ruolo da contestualizzare | 9 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07005.json#200:description` |
| `YOKHUYENEMYA` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07001.json#343:description` |
| `YOTSUYU` | [Yotsuyu](STORMBLOOD.md) | 116 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04002.json#29:description` |
| `YOUNGTEMPLEKNIGHT` | Identità / ruolo da contestualizzare | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03003.json#16:description` |
| `YSAYLE` | [Ysayle](HEAVENSWARD.md) | 0 | 0 | Solo vocalizzazioni / materiale non sufficiente | `data/da_tradurre/dialogue/cut_scene/023/voiceman_02300.json#71:description` |
| `YSHTOLA` | [Y'shtola](RECURRING.md) | 1217 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#77:description` |
| `YSIALA` | Ys Iala (nome da contestualizzare) | 9 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/050/voiceman_05001.json#152:description` |
| `YUGIRI` | [Yugiri](STORMBLOOD.md) | 192 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/022/voiceman_02200.json#6:description` |
| `YUYUHASE` | [Yuyuhase](ARR.md) | 1 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/025/voiceman_02500.json#336:description` |
| `ZADA` | Identità / ruolo da contestualizzare | 17 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/054/voiceman_05400.json#243:description` |
| `ZANUHALI` | Zanuhali (nome da contestualizzare) | 26 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#327:description` |
| `ZASSHAL` | Zasshal (nome da contestualizzare) | 12 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/061/voiceman_06100.json#51:description` |
| `ZAWPYA` | Zawpya (nome da contestualizzare) | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#143:description` |
| `ZEKOWA` | [Zekowa](DAWNTRAIL.md) | 26 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07003.json#11:description` |
| `ZELENIA` | Identità / ruolo da contestualizzare | 3 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/072/voiceman_07200.json#231:description` |
| `ZENOS` | [Zenos](STORMBLOOD.md) | 236 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/040/voiceman_04000.json#144:description` |
| `ZEPHIRIN` | [Zephirin](HEAVENSWARD.md) | 15 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/030/voiceman_03001.json#110:description` |
| `ZEREELJA` | Zereel Ja (nome da contestualizzare) | 34 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07002.json#252:description` |
| `ZEROMUS` | Zeromus (nome da contestualizzare) | 5 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/065/voiceman_06500.json#201:description` |
| `ZODIARK` | Zodiark (nome da contestualizzare) | 24 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/060/voiceman_06001.json#816:description` |
| `ZORAALJA` | [Zoraal Ja](DAWNTRAIL.md) | 126 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07000.json#256:description` |
| `ZURMURWUR` | Zurmurwur (nome da contestualizzare) | 12 | 0 | SCD en + testo candidato | `data/da_tradurre/dialogue/cut_scene/070/voiceman_07001.json#265:description` |

## Ripetere la verifica

Esegui dalla radice, scegliendo un nuovo nome per il rapporto:

```powershell
python tools/character_voice_audit.py --sqpack "G:/SquareEnix/FINAL FANTASY XIV - A Realm Reborn/game/sqpack" --out tools/reports/character_voice_audit_new.json
python -m unittest discover -s tests -p test_character_voice_audit.py
```

Il rapporto in `tools/reports/` è escluso da Git. Lo strumento legge JSON e SqPack e scrive soltanto quel rapporto: non esporta audio, non modifica il client e non aggiorna o approva traduzioni. Per una nuova patch ripeti il riscontro; un percorso non trovato non dimostra assenza di doppiaggio.

Il layout degli indici è riscontrabile nelle [strutture SqPack di Lumina](https://github.com/NotAdam/Lumina/blob/master/src/Lumina/Data/Structs/SqPackIndexHeader.cs); il collegamento tra codici e percorsi è verificato sugli archivi locali, non assunto dal solo nome del foglio.
