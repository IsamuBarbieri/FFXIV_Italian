using System.Text.Json;
using System.Text.Json.Nodes;

namespace FFXIVItalian.Extractor;

public static class CharacterCreationTranslations
{
    public static void ApplyTo(string lobbyJsonPath)
    {
        if (!File.Exists(lobbyJsonPath))
        {
            throw new FileNotFoundException($"File non trovato: {lobbyJsonPath}");
        }

        string json = File.ReadAllText(lobbyJsonPath);
        var root = JsonNode.Parse(json)?.AsObject() ?? new JsonObject();

        void SetSingle(uint id, string original, string translation)
        {
            string key = id.ToString();
            var obj = new JsonObject
            {
                ["original"] = original,
                ["translation"] = translation
            };
            root[key] = obj;
        }

        void SetMulti(uint id, string name, string translationName, string desc, string translationDesc)
        {
            string key = id.ToString();
            var obj = new JsonObject
            {
                ["name"] = name,
                ["translation_name"] = translationName,
                ["description"] = desc,
                ["translation_description"] = translationDesc
            };
            root[key] = obj;
        }

        // ==========================================
        // 1. LORE DELLE 8 RAZZE GIOCABILI
        // ==========================================
        SetMulti(110,
            "Hyur<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "Hyur<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "The Hyur are said to have first traveled to Eorzea from her surrounding\ncontinents and islands. Three great migratory waves later, they are now\nthe most populous of all the civilized races. They exhibit a relatively\nmodest physique, both in height and build, and are known for their\npeculiarly short, rounded ears. Hyurs are well suited for traveling long\ndistances by foot, a trait thought to account for their swift proliferation.\n\nTheir espousal of an eclectic variety of languages and traditions is\na legacy of their diverse heritage－as is their resulting lack of a\nunified cultural identity.",
            "Si narra che gli Hyur siano giunti per la prima volta a Eorzea dai continenti\ne dalle isole circostanti. Tre grandi ondate migratorie più tardi, costituiscono\normai la più popolosa tra tutte le civiltà. Presentano una corporatura relativamente\nmodesta, sia in altezza che per stazza, e si distinguono per le orecchie\nsingolarmente corte e arrotondate. Gli Hyur sono particolarmente adatti a coprire grandi\ndistanze di marcia, caratteristica considerata alla base della loro rapida proliferazione.\n\nLa loro adozione di un'eclettica varietà di idiomi e usanze è\nl'eredità della loro eterogenea discendenza, così come la conseguente assenza\ndi un'identità culturale unificata.");

        SetMulti(112,
            "Elezen<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "Elezen<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "In former times, the Elezen were the sole inhabitants of Eorzea,\nclaiming dominion over her. Traditionally a nomadic people, the tall,\nslender Elezen believed the realm to be theirs by divine right.\nUnfortunately, this belief made the eventual appearance of the\nHyur in their multitudes akin to an invasion, and a long history of\nconflict ensued.\n\nUltimately, the Elezen diverged into the two clans that exist today.\nThe Wildwood Elezen took to the forests to protect their homeland,\nwhile the Duskwight Elezen withdrew to caves and subterrane, opting\ninstead to avoid all contact with any but their own.",
            "In tempi remoti, gli Elezen erano gli unici abitanti di Eorzea,\nsu cui rivendicavano il dominio assoluto. Popolo tradizionalmente nomade, gli alti e\nslanciati Elezen credevano che il reame appartenesse loro per diritto divino.\nSfortunatamente, tale convinzione rese la comparsa delle moltitudini di\nHyur del tutto simile a un'invasione, scatenando una lunga storia di aspri conflitti.\n\nCon il tempo, gli Elezen si scissero nei due clan odierni.\nGli Elezen Silvani scelsero le foreste per difendere la loro terra natia,\nmentre gli Elezen Crepuscolari si ritirarono in caverne e recessi sotterranei, preferendo\nevitare qualsiasi contatto con chiunque non appartenesse alla loro stirpe.");

        SetMulti(114,
            "Lalafell<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "Lalafell<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "A wee people sporting short, rotund bodies, the Lalafell appear as\nno more than children to the eyes of most. Many of these nimble\nlittle folk hail from the islands of the south seas, where they practice\na simple agricultural lifestyle. It was not until the opening of maritime\ntrade routes that the gradual migration of Lalafells to Eorzea began.\n\nNow one of the most well-established races in the realm, Lalafells\ncan be found in great numbers in nearly every city. Though Lalafellin\nculture places great emphasis on blood relations, individuals are known\nfor getting along amicably with members of all races.",
            "Un popolo minuto dal corpo basso e paffuto, i Lalafell appaiono alla maggior parte\ndegli sguardi come poco più che bambini. Molti di questi agili\ne piccoli individui provengono dalle isole dei mari meridionali, dove conducevano\nuna semplice vita agricola. Fu solo con l'apertura delle rotte commerciali\nmarittime che ebbe inizio la graduale migrazione dei Lalafell verso Eorzea.\n\nOggi divenuti una delle civiltà più radicate del reame, i Lalafell\nsi trovano in gran numero in quasi ogni città. Sebbene la loro cultura\nattribuisca enorme importanza ai legami di sangue, ciascun individuo è noto\nper la straordinaria cordialità con cui convive con ogni altra stirpe.");

        SetMulti(116,
            "Miqo'te<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "Miqo'te<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "The ancestors of the Miqo'te made their way to Eorzea during the\nAge of Endless Frost, traversing the frozen seas in pursuit of the\nwildlife upon which they subsisted. Adaptation to a hunting lifestyle\nhas fashioned them with a keen sense of smell, powerful legs, and\na tail which provides them with exceptional balance.\n\nMiqo'te are known to be very territorial, and many individuals tend\nto lead solitary lifestyles, particularly males.",
            "Gli antenati dei Miqo'te giunsero a Eorzea durante l'Era del\nGelo Perpetuo, attraversando i mari ghiacciati al seguito della fauna selvatica\nda cui traevano sostentamento. L'adattamento alla caccia ha conferito loro un olfatto\nfinissimo, gambe potenti e una coda che garantisce un equilibrio straordinario.\n\nI Miqo'te sono noti per essere fortemente territoriali e molti individui prediligono\nuno stile di vita solitario, in particolare i maschi.");

        SetMulti(118,
            "Roegadyn<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "Roegadyn<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "Known for their brawny builds and piercing eyes, the Roegadyn\nare the largest and most rugged of Eorzea's races. The majority\nof the realm's Roegadyn belong to the Sea Wolf clan, a maritime\npeople who earn their keep on or by the sea, be it as sailors,\nfishermen, or pirates. Comparatively fewer in number are the\nHellsguard, who are known for their more earnest demeanors\nand can often be found working as bodyguards and smithies.",
            "Noti per la muscolatura possente e lo sguardo penetrante, i Roegadyn\nsono la stirpe più imponente e vigorosa di tutta Eorzea. La maggior parte\ndei Roegadyn del reame appartiene al clan dei Lupi di Mare, un popolo marittimo\nche trae sostentamento dalle acque, sia come marinai che come pescatori o pirati.\nInferiori per numero sono invece i Guardinferno, noti per la loro indole più austera\ne spesso impiegati come guardie del corpo o fabbri.");

        SetMulti(140,
            "Au Ra<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "Au Ra<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "The curved horns and patterned scales that characterize the Au Ra\noften give rise to speculation that this Hyuran-sized race are,\nin fact, the progeny of dragons. This has long been a disputed\nsubject, based on the race's split creation myths and the\nevident dissimilarity in their respective physical features－the\nmost egregious of which is the Au Ra's cranial horns, which grant\nthem enhanced hearing and spatial recognition, but remain\nunmatched by any dragon.\n\nIn addition to these cranial features, the Au Ra are also known\nfor the significant difference in height between males and females,\nwith the former often towering above the latter.",
            "Le corna ricurve e le scaglie geometriche che contraddistinguono gli Au Ra\nhanno spesso alimentato l'ipotesi che questa stirpe di statura simile a quella hyuriana\nsia, in realtà, la discendenza dei draghi. Tale congettura è tuttavia oggetto di antica disputa,\nconsiderati i miti contrastanti sulla loro creazione e la palese divergenza nei rispettivi tratti anatomici:\nil più evidente risiede nelle corna craniche degli Au Ra, che garantiscono loro\nun udito amplificato e una fine percezione spaziale, proprietà del tutto assente nei draghi.\n\nOltre a queste caratteristiche craniche, gli Au Ra si distinguono\nper il marcato dimorfismo di statura tra maschi e femmine, nei quali i primi svettano\nnettamente sulle seconde per altezza.");

        SetMulti(458,
            "Hrothgar<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "Hrothgar<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "Hailing from the distant shores of Ilsabard, the Hrothgar are a burly\npeople of leonine appearance－or so the men appear, for they were the\nonly Hrothgar to journey to the shores of Eorzea until only very\nrecently. Now that female Hrothgar have begun to cross the sea, they\ncan be seen standing beside their male counterparts, their\ntall, slender frames and regal mien readily setting them apart.\n\nBoth genders possess sharp claws and pointed fangs, instilling\nfear in other races upon their arrival in Eorzea. Despite their\nintimidating appearance, Hrothgar are on the whole a gentle people,\nand over time were able to establish themselves in the realm as trusted\nfriends and comrades.",
            "Originari delle lontane coste di Ilsabard, gli Hrothgar sono una stirpe\nrobusta dai tratti leonini: o almeno tale era l'aspetto dei soli maschi che, fino a tempi\nmolto recenti, avevano intrapreso il viaggio verso le rive di Eorzea. Ora che anche\nle femmine hrothgar hanno iniziato ad attraversare i mari, è possibile scorgerle\nal fianco dei propri compagni, distinguendosi per la statura slanciata e il portamento fiero e regale.\n\nEntrambi i sessi possiedono artigli affilati e zanne aguzze, dettagli che suscitarono timore\nnelle altre civiltà al loro primo sbarco a Eorzea. A dispetto del loro aspetto minaccioso,\ngli Hrothgar sono tuttavia un popolo benevolo, e nel tempo hanno saputo affermarsi\nnel reame come fidati amici e valorosi compagni d'arme.");

        SetMulti(460,
            "Viera<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "Viera<hex:02090FE802FF0420C2A9FF0420C2AEFF0103>",
            "Of tall stature and slender frame, the Viera's physical appearance is\nstrikingly similar to that of the Hyur and Elezen, save for their\ndistinctive long ears. Adhering to a strict code known as the Green\nWord, they are prohibited from contact with the outside world under\nthreat of exile. Their society is purely matriarchal, and males of age\nare seldom, if ever, found within the bounds of any village. They\ninstead serve as protectors of the wood from the shadows, that no\nevil may ever encroach on their sacred lands.",
            "Di alta statura e corporatura sinuosa, l'aspetto dei Viera presenta\nuna notevole somiglianza con quello di Hyur ed Elezen, eccezion fatta per le loro\ncaratteristiche lunghe orecchie. Fedeli a un rigido codice noto come Parola Verde,\nè loro fatto divieto di entrare in contatto con il mondo esterno sotto pena di esilio. La loro\nsocietà è puramente matriarcale, e i maschi adulti assai raramente dimorano entro i confini\ndei villaggi. Essi agiscono invece come silenziosi custodi della foresta dall'ombra,\naffinché nessuna minaccia osi profanare le loro terre sacre.");

        // ==========================================
        // 2. LORE DEI 16 CLAN
        // ==========================================
        SetMulti(120,
            "Midlander", "Piancolle",
            "The Midlander clan comprises over half of the total population of\nEorzea's Hyurs. They have established themselves throughout\nevery city in the realm and lead lives as diverse as their heritage.\n\nTrained in letters from infancy, the Midlanders are generally more\neducated than many of the other races and clans. Despite the fact\nthat males tend to be slightly taller than females, there are no\nsignificant differences between the genders.",
            "Il clan dei Piancolle costituisce oltre la metà della popolazione totale\ndegli Hyur di Eorzea. Si sono stabiliti in ogni città\ndel reame e conducono esistenze tanto variegate quanto la loro discendenza.\n\nEducati alle lettere sin dall'infanzia, i Piancolle vantano generalmente un'istruzione\nsuperiore a molte altre stirpi e clan. Benché i maschi tendano a essere leggermente\npiù alti delle femmine, non sussistono differenze fisiche sostanziali tra i generi.");

        SetMulti(122,
            "Highlander", "Montanaro",
            "The Highlanders were the first of the Hyur to reach Eorzea.\nTheir name derives from their long tradition of building strongholds\nin the mountains. Compared to their Midland brethren, the\nHighlanders are noticeably larger in build.\n\nOf late, Highlanders have become an increasingly rare sight\nin Eorzea, their number represented almost exclusively by those\nwho fled Ala Mhigo after its fall and now work in the desert\ncity-state of Ul'dah as mercenaries and sellswords.",
            "I Montanari furono i primi tra gli Hyur a giungere a Eorzea.\nIl loro nome trae origine dall'antica usanza di erigere roccaforti\ntra le vette montuose. Rispetto ai loro confratelli Piancolle, i\nMontanari presentano una corporatura visibilmente più possente.\n\nNegli ultimi tempi, i Montanari sono divenuti una presenza sempre più rara\na Eorzea, rappresentati quasi esclusivamente da coloro che fuggirono da Ala Mhigo\ndopo la sua caduta e che ora prestano servizio nella città-stato desertica\ndi Ul'dah come mercenari e soldati di ventura.");

        SetMulti(124,
            "Wildwood", "Silvano",
            "For hundreds of years, the Wildwood Elezen have lived in the\nrelative safety of Eorzea's lush forests. With the formation of\nEorzea's governments, however, many Wildwood ventured forth\nfrom the forests, drawn to the exhilarating cosmopolitanism of\nnearby Gridania. There is minimal dimorphism between the two\ngenders, though males are generally considered to be milder and\nmore contemplative than females, who are renowned for their fierce,\nsteadfast disposition.",
            "Per centinaia di anni, gli Elezen Silvani hanno dimorato nella\nrelativa sicurezza delle lussureggianti foreste di Eorzea. Con la nascita\ndei governi del reame, tuttavia, molti Silvani si spinsero oltre i boschi,\nattratti dal vivace cosmopolitismo della vicina Gridania. Il dimorfismo tra i due sessi\nè minimo, sebbene i maschi siano comunemente considerati più miti e\nriflessivi delle femmine, rinomate per il loro spirito fiero e risoluto.");

        SetMulti(126,
            "Duskwight", "Crepuscolare",
            "The Duskwight Elezen are descendants of those who sought\nsanctuary in the subterranean depths of Eorzea following the rupture\nof ties with their Wildwood brethren. Generations of cave-dwelling\nhave endowed them with pallid skin and keen senses of sight and hearing.\nWhile an unsavory reputation as brigands and outlaws clings to them among\ndwellers of the surface, many Duskwights are proud folk who wish only to\nlive in peace according to their ancestral customs.",
            "Gli Elezen Crepuscolari sono i discendenti di coloro che cercarono rifugio\nnelle vaste caverne sotterranee di Eorzea in seguito alla rottura dei legami\ncon i loro fratelli Silvani. Vivendo da generazioni nell'oscurità, hanno sviluppato\nuna carnagione straordinariamente pallida e un acume visivo e uditivo superiore.\nNonostante la reputazione di briganti e banditi che spesso li accompagna tra gli\nabitanti della superficie, molti Crepuscolari sono individui fieri che desiderano\nsolo vivere in pace secondo le antiche tradizioni dei loro avi.");

        SetMulti(128,
            "Plainsfolk", "Pratoverde",
            "The Plainsfolk have lived for generations in the fertile grasslands of\nthe southern islands, fashioning traditional thatched dwellings of clay.\nTheir hair exhibits earthy, golden hues reminiscent of swaying prairies,\nwhile their large, luminous eyes reflect their friendly and inquisitive nature.\nAdapting swiftly to the urban centers of Eorzea, they excel in commerce,\nagriculture, and all trades calling for ingenuity and industriousness.",
            "I Pratoverde hanno vissuto per generazioni nelle fertili pianure\ndelle isole meridionali, costruendo caratteristiche dimore di paglia e argilla.\nI loro capelli presentano tonalità terrose e dorate che richiamano le distese erbose,\nmentre gli occhi grandi e luminosi riflettono la loro indole cordiale e curiosa.\nAdattandosi rapidamente ai ritmi cittadini di Eorzea, eccellono nel commercio,\nnell'agricoltura e in qualsiasi professione richieda ingegno e laboriosità.");

        SetMulti(130,
            "Dunesfolk", "Dunagialla",
            "The Dunesfolk trace their lineage to the arid, desolate expanses of the\nsouthern islands, where they constructed dwellings atop the backs of giant\nbeasts of burden. The most striking trait of this clan is the luminous\nmembrane covering their pupils, evolved to shield their eyes from the harsh\nglare of the sun and blowing sand. Many Dunesfolk settled in the Thanalan desert,\nfounding the bustling merchant city of Ul'dah and mastering its commerce.",
            "I Dunagialla traggono origine dalle aride e desolate distese delle isole del sud,\ndove erigevano abitazioni sopra i dorsi di grandi bestie da soma.\nLa caratteristica più peculiare di questo clan è la sottile membrana lucida che\nricopre le pupille, evolutasi per proteggere gli occhi dal riverbero accecante del sole e dalla sabbia.\nMolti Dunagialla si sono stabiliti nel deserto di Thanalan, dove hanno fondato\nla sfarzosa città mercantile di Ul'dah, guidandone la finanza e il commercio con impareggiabile maestria.");

        SetMulti(132,
            "Seekers of the Sun", "Cercasole",
            "The Seekers of the Sun have adapted to diurnal life and revere Azeyma, the\nWarden. Members of this clan are distinguished by their vividly colored eyes\nwith slender, vertical pupils reminiscent of predatory cats. Their tribal society\nis strictly structured around twenty-six patrilineal tribes, each associated with\na character of the ancient Eorzean alphabet. Males are divided between dominant\nbreeders and solitary wanderers seeking their own territories.",
            "I Cercasole si sono adattati alla vita diurna e venerano Azeyma, Custode del Sole.\nI membri di questo clan si distinguono per gli occhi dai colori vividi con pupille verticali\ne sottili, simili a quelle dei felini predatori. La loro società tribale è rigidamente\norganizzata attorno a ventisei tribù, ciascuna associata a una lettera dell'antico alfabeto eorzeano.\nI maschi sono suddivisi tra riproduttori dominanti e giovani guerrieri erranti in cerca del proprio territorio.");

        SetMulti(134,
            "Keepers of the Moon", "Guardialuna",
            "The Keepers of the Moon are a nocturnal clan of Miqo'te devoted to Menphina,\nthe Lover. They are recognized by their pronounced canines, fair skin, and large,\nrounded pupils tailored for hunting in the dark recesses of the woods. Unlike the\nSeekers of the Sun, the Keepers live in a distinctly matriarchal culture organized\ninto small family units where mothers pass down their names and authority.",
            "Le Guardialuna sono un clan di Miqo'te notturni devoti a Menphina, Custode della Luna.\nSi distinguono per i canini pronunciati, la carnagione pallida e gli occhi grandi dalle pupille rotonde,\nperfettamente adatti a cacciare nell'oscurità delle selve. A differenza dei Cercasole,\nle Guardialuna vivono in una società spiccatamente matriarcale, organizzata in piccole famiglie\nin cui le madri tramandano il proprio nome e la propria autorità alle figlie.");

        SetMulti(136,
            "Sea Wolves", "Lupi di Mare",
            "Hailing from the frozen archipelagos of the northern seas, the Sea Wolves arrived\nin Eorzea aboard formidable longships, earning widespread awe as intrepid mariners.\nTheir skin carries cool, bluish and greenish tints reminiscent of the ocean depths.\nToday they form the bedrock of the maritime realm of Limsa Lominsa, flourishing as\nmaster sailors, deep-sea fishermen, and naval commanders.",
            "Originari delle gelide isole dei mari settentrionali, i Lupi di Mare approdarono a Eorzea\nsulle loro imponenti navi lunghe, guadagnandosi il rispetto e il timore di tutti come formidabili corsari.\nLa loro carnagione tende a sfumature bluastre e verdognole che richiamano le profondità oceaniche.\nOggi costituiscono la colonna portante della città marinara di Limsa Lominsa, distinguendosi come\ncoraggiosi marinai, pescatori d'alto mare e comandanti navali.");

        SetMulti(138,
            "Hellsguard", "Guardinferno",
            "The Hellsguard are a proud clan of Roegadyn who made their home in the volcanic\nreaches of the northern Abalathia spine. Believing the mouths of craters to be the\ngateways to the underworld, they took upon themselves the sacred vigil over the flaming\nabysses. Their rust-hued skin and immense stature make them fearsome warriors and master\nsmiths, while fostering profound spiritual devotion.",
            "I Guardinferno sono un fiero clan di Roegadyn che ha eletto a propria dimora le regioni vulcaniche\ndi Abalathia. Credendo fermamente che le bocche dei crateri costituiscano i varchi d'accesso agli inferi,\nsi sono assunti il solenne compito di vegliare su tali abissi ardenti. La loro pelle color ruggine\ne la forza erculea li rendono guerrieri temibili e maestri dell'arte della forgiatura, sebbene coltivino\nanche una profonda disciplina spirituale e filosofica.");

        SetMulti(142,
            "Raen", "Raen",
            "The Raen believe themselves to be descendants of the Dawn Father, the solar\ndeity of their creation myth. Their scales and horns gleam with an immaculate\nivory hue, symbolizing tranquility and virtue. Leaving behind the harsh steppes\nfor sheltered valleys and eastern shores, the Raen embraced a peaceful existence,\nexcelling in diplomacy, arts, and meditation while nurturing deep communal harmony.",
            "I Raen credono di discendere dalla Madre dell'Alba, divinità solare del loro pantheon.\nLe loro scaglie e corna sono di un colore bianco avorio immacolato, a simboleggiare purezza e serenità.\nAbbandonate le aspre steppe per valli riparate e coste orientali, i Raen hanno intrapreso\nuno stile di vita pacifico, eccellendo nella diplomazia, nella poesia e nella meditazione spirituale,\nmantenendo un profondo senso di armonia e lealtà verso la comunità.");

        SetMulti(144,
            "Xaela", "Xaela",
            "The Xaela revere the Dusk Mother, primordial entity of the star-strewn night.\nThey boast lustrous scales and horns of pitch black and maintain nomadic ways\nacross the vast Azim Steppe. Divided into dozens of martial tribes perpetually\nwarring and allying among themselves, the Xaela place supreme value on battle prowess,\nsurvival skills, and untrammeled freedom beneath the open sky.",
            "Gli Xaela venerano il Padre dell'Imbrunire, entità primordiale della notte stellata.\nSfoggiano scaglie e corna di una fuligginosa tonalità nera come la pece e mantengono costumi nomadi\nnelle sterminate steppe di Othard. Suddivisi in decine di tribù guerriere costantemente in lotta o alleanza\ntra loro, gli Xaela ripongono il massimo valore nel coraggio in battaglia, nell'abilità venatoria e nella libertà\nassoluta sotto la volta celeste.");

        SetMulti(462,
            "Helions", "Eliano",
            "Helions have traditionally been content to reside in southern Ilsabard\nwith their respective tribes, devout servants to their queens,\none and all—and thus have they only but recently begun to make\ntheir presence felt throughout the remainder of the three continents.\nThough these Hrothgar initially had no word to describe themselves,\nscholars came to call them \"Helions\" after observing how their lives\nall but revolved about their queen's needs, as planets circle the sun.",
            "Gli Eliani risiedevano tradizionalmente nell'Ilsabard meridionale, suddivisi nelle rispettive tribù\ne devoti servitori delle proprie regine: per questa ragione hanno iniziato a palesare la propria presenza\nnel resto dei tre continenti solo in tempi recenti. Sebbene questi Hrothgar non possedessero originariamente\nun vocabolo per definirsi, gli studiosi presero a chiamarli \"Eliani\" notando come le loro vite ruotassero\ninteramente attorno alle esigenze della regina, proprio come pianeti attorno al sole.");

        SetMulti(464,
            "The Lost", "Ramingo",
            "Unlike their Helion brethren, the Lost are unbound by the whims of any\nreigning monarch, having long since found themselves bereft of the\nwarm radiance of their queen. Most find that a nomadic lifestyle\nbest suits their situation, and are known to ply a variety of trades\nthat align with their propensity to travel—seasonal laborer,\nmercenary, and peddler among them.",
            "A differenza dei loro confratelli Eliani, i Raminghi non sono vincolati ai voleri di alcun sovrano regnante,\nessendosi ritrovati da lungo tempo privati del caldo bagliore della loro regina. La maggior parte di essi ritiene\nche uno stile di vita nomade sia il più consono alla propria condizione, dedicandosi a mestieri legati\nalla naturale propensione al viaggio: braccianti stagionali, mercenari e mercanti ambulanti su tutti.");

        SetMulti(466,
            "Rava", "Rava",
            "Viera of the Rava clan live primarily in the Golmore Jungle, their\number skin allowing them to readily blend in with their surroundings.\nThough the males stand as wards of the forest, the females are\nadept hunters and protectors in their own right, fiercely guarding their homes and\ntheir young. Despite attempts by the Dalmascans to subjugate\nthem, they have ever maintained a self-governing dominion. Though\nmost would adhere to the traditions of the wood, some few have\nchosen to venture out into the world.",
            "I Viera del clan Rava dimorano primariamente nella Giungla di Golmore, dove la pelle bruna consente loro\ndi confondersi agevolmente con la fitta vegetazione circostante. Sebbene i maschi agiscano come sentinelle\ndella foresta, le femmine sono cacciatrici e protettrici provette, pronte a difendere con ferocia le dimore\ne i piccoli. A dispetto dei tentativi di sottomissione operati dai Dalmascani, hanno sempre preservato\nla propria sovranità. Benché i più osservino le tradizioni boschive, alcuni hanno scelto di viaggiare nel mondo.");

        SetMulti(468,
            "Veena", "Veena",
            "With skin as fair as the snowcapped mountains towering above, the\nVeena clan make their homes in the forests lining the southwestern\nfoothills of the Skatay Range. Like the members of their sister clan,\nthe Veena live as hunters and gatherers, laboring not only to protect\nthe woods, but to nurture them. As the winds of war swept up a\nnumber of those who left their mountain homes, many such\nhardened hunters chose to set out for the distant shores of Eorzea.",
            "Con una carnagione candida come le vette innevate che svettano sopra di loro, i membri del clan Veena\ndimorano nelle foreste lungo le pendici sud-occidentali della Catena dello Skatay. Al pari delle loro sorelle Rava,\ni Veena vivono di caccia e raccolta, adoperandosi non solo per proteggere i boschi, bensì per nutrirli e curarli.\nQuando i venti di guerra travolsero quanti avevano lasciato le montagne, molti di questi indomiti cacciatori\nscelsero di salpare verso le lontane coste di Eorzea.");

        // ==========================================
        // 3. LE DODICI DIVINITÀ PATRONE (471-482)
        // ==========================================
        SetSingle(471,
            "Halone, mover of glaciers and goddess of war, is the<hex:02100103>guardian deity of Ishgard. She commands the element of<hex:02100103>ice and is associated with the first moon of the Eorzean<hex:02100103>calendar. Halone is the daughter of Rhalgr, and a bitter<hex:02100103>rival of Nophica. She is most often depicted as a<hex:02100103>relentless warrioress armed with a bronze greatshield.<hex:02100103>Her symbol is the three spears.",
            "Halone, Signora dei Ghiacciai e Dea della Guerra, è la<hex:02100103>divinità protettrice di Ishgard. Governa l'elemento del<hex:02100103>ghiaccio ed è associata alla prima luna del calendario<hex:02100103>eorzeano. Halone è la figlia di Rhalgr e l'acerrima<hex:02100103>rivale di Nophica. È per lo più raffigurata come un'implacabile<hex:02100103>guerriera armata di un grande scudo di bronzo.<hex:02100103>Il suo simbolo sono le tre lance.");

        SetSingle(472,
            "Menphina is keeper of the moon and the goddess of love.<hex:02100103>She commands the element of ice and is associated with<hex:02100103>the second moon of the Eorzean calendar. Menphina is the<hex:02100103>sister of Azeyma, and the divine lover of Oschon. She is<hex:02100103>most often depicted as a maid carrying a round skillet.<hex:02100103>Her symbol is the full moon.",
            "Menphina è la Custode della Luna e la Dea dell'Amore.<hex:02100103>Governa l'elemento del ghiaccio ed è associata alla<hex:02100103>seconda luna del calendario eorzeano. Menphina è la<hex:02100103>sorella di Azeyma e la divina amante di Oschon. È<hex:02100103>per lo più raffigurata come una fanciulla recante una padella rotonda.<hex:02100103>Il suo simbolo è la luna piena.");

        SetSingle(473,
            "Thaliak, ruler of rivers and wisdom, and god of knowledge,<hex:02100103>is the guardian deity of Sharlayan. He commands the<hex:02100103>element of water and is associated with the third moon<hex:02100103>of the Eorzean calendar. Thaliak is the father of Llymlaen,<hex:02100103>and the teacher of Byregot. He is most often depicted as a<hex:02100103>reserved scholar holding an ashen staff. His symbol is the scroll.",
            "Thaliak, Signore dei Fiumi e della Saggezza, Dio della Conoscenza,<hex:02100103>è la divinità protettrice di Sharlayan. Governa l'elemento<hex:02100103>dell'acqua ed è associato alla terza luna del calendario<hex:02100103>eorzeano. Thaliak è il padre di Llymlaen e il maestro di Byregot.<hex:02100103>È per lo più raffigurato come un riservato studioso recante<hex:02100103>un bastone di frassino. Il suo simbolo è la pergamena.");

        SetSingle(474,
            "Nymeia is the watcher of celestial bodies and goddess<hex:02100103>of fate. She commands the element of water and is<hex:02100103>associated with the fourth moon of the Eorzean calendar.<hex:02100103>Nymeia is the younger sister of Althyk, and master of<hex:02100103>Rhalgr. She is most often depicted as a weaver donning<hex:02100103>a white silken veil. Her symbol is the spinning wheel.",
            "Nymeia è la Custode dei Corpi Celesti e la Dea del Destino.<hex:02100103>Governa l'elemento dell'acqua ed è associata alla quarta<hex:02100103>luna del calendario eorzeano. Nymeia è la sorella minore<hex:02100103>di Althyk e la sovrana di Rhalgr. È per lo più raffigurata come<hex:02100103>una tessitrice avvolta da un velo di candida seta.<hex:02100103>Il suo simbolo è l'arcolaio.");

        SetSingle(475,
            "Llymlaen, watcher of the seas and goddess of navigation,<hex:02100103>is the guardian deity of Limsa Lominsa. She commands<hex:02100103>the element of wind and is associated with the fifth<hex:02100103>moon of the Eorzean calendar. Llymlaen is the daughter<hex:02100103>of Thaliak, and the elder sister of Nophica. She is most<hex:02100103>often depicted as a strong fisherwoman wielding a<hex:02100103>long-bladed harpoon. Her symbol is the wave.",
            "Llymlaen, Custode dei Mari e Dea della Navigazione, è la<hex:02100103>divinità protettrice di Limsa Lominsa. Governa l'elemento<hex:02100103>del vento ed è associata alla quinta luna del calendario<hex:02100103>eorzeano. Llymlaen è la figlia di Thaliak e la sorella<hex:02100103>maggiore di Nophica. È per lo più raffigurata come una forte<hex:02100103>pescatrice armata di un lungo arpione.<hex:02100103>Il suo simbolo è l'onda.");

        SetSingle(476,
            "Oschon is ruler of the mountains and god of travelers<hex:02100103>and vagrants. He commands the element of wind and is<hex:02100103>associated with the sixth moon of the Eorzean calendar.<hex:02100103>Oschon is the brother of Nald'thal, and the close<hex:02100103>companion of Halone. He is most often depicted as a<hex:02100103>carefree ranger wielding a bow of yew. His symbol is<hex:02100103>the walking stick.",
            "Oschon è il Signore delle Montagne e il Dio dei Viandanti e dei<hex:02100103>Vagabondi. Governa l'elemento del vento ed è associato alla<hex:02100103>sesta luna del calendario eorzeano. Oschon è il fratello di<hex:02100103>Nald'thal e l'inseparabile compagno di Halone. È per lo più<hex:02100103>raffigurato come un cacciatore spensierato con un arco di tasso.<hex:02100103>Il suo simbolo è il bastone da passeggio.");

        SetSingle(477,
            "Byregot is the purveyor of architecture and industry,<hex:02100103>and god of the arts. He commands the element of<hex:02100103>lightning and is associated with the seventh moon of<hex:02100103>the Eorzean calendar. Byregot is the elder brother of<hex:02100103>Halone, and pupil of Thaliak. He is most often depicted<hex:02100103>as an ardent smith with a two-headed hammer.<hex:02100103>His symbol is the hand.",
            "Byregot è l'Artefice dell'Architettura e dell'Industria, Dio delle<hex:02100103>Arti. Governa l'elemento del fulmine ed è associato alla settima<hex:02100103>luna del calendario eorzeano. Byregot è il fratello maggiore di<hex:02100103>Halone e discepolo di Thaliak. È per lo più raffigurato come un<hex:02100103>ardente fabbro armato di un martello a due teste.<hex:02100103>Il suo simbolo è la mano.");

        SetSingle(478,
            "Rhalgr, breaker of worlds, is the god of destruction<hex:02100103>and guardian deity of the now-fallen nation of Ala Mhigo.<hex:02100103>He commands the element of lightning and is associated<hex:02100103>with the eighth moon of the Eorzean calendar. Rhalgr is<hex:02100103>the father of both Byregot and Halone, and serves as<hex:02100103>attendant to Nymeia. He is most often depicted as a<hex:02100103>magus carrying a staff of bronze. His symbol is the<hex:02100103>streaking meteor.",
            "Rhalgr, lo Spezzatore di Mondi, è il Dio della Distruzione<hex:02100103>e la divinità protettrice dell'ormai caduta nazione di Ala Mhigo.<hex:02100103>Governa l'elemento del fulmine ed è associato all'ottava<hex:02100103>luna del calendario eorzeano. Rhalgr è il padre sia di Byregot<hex:02100103>che di Halone, e serve come attendente di Nymeia. È per lo più<hex:02100103>raffigurato come un mago recante un bastone di bronzo.<hex:02100103>Il suo simbolo è la meteora fiammeggiante.");

        SetSingle(479,
            "Azeyma is keeper of the sun and goddess of inquiry.<hex:02100103>She commands the element of fire and is associated<hex:02100103>with the ninth moon of the Eorzean calendar. Azeyma<hex:02100103>is the daughter of Althyk, and the elder sister of<hex:02100103>Menphina. She is most often depicted as a noble lady<hex:02100103>holding a golden fan. Her symbol is the radiant sun.",
            "Azeyma è la Custode del Sole e la Dea dell'Indagine.<hex:02100103>Governa l'elemento del fuoco ed è associata alla nona<hex:02100103>luna del calendario eorzeano. Azeyma è la figlia di Althyk<hex:02100103>e la sorella maggiore di Menphina. È per lo più raffigurata<hex:02100103>come una nobile dama con un ventaglio dorato.<hex:02100103>Il suo simbolo è il sole radioso.");

        SetSingle(480,
            "Nald'thal, overseer of the underworld and god of<hex:02100103>commerce, is the guardian deity of Ul'dah. He<hex:02100103>commands the element of fire and is associated<hex:02100103>with the tenth moon of the Eorzean calendar.<hex:02100103>Nald'thal is the single manifestation of the deific<hex:02100103>twins Nald and Thal. He is most often depicted<hex:02100103>as a discerning merchant holding a balance.<hex:02100103>His symbol is the cowry, an ancient shell currency.",
            "Nald'thal, Sovrintendente dell'Oltretomba e Dio del Commercio,<hex:02100103>è la divinità protettrice di Ul'dah. Governa l'elemento<hex:02100103>del fuoco ed è associato alla decima luna del calendario<hex:02100103>eorzeano. Nald'thal è l'unica manifestazione dei gemelli<hex:02100103>divini Nald e Thal. È per lo più raffigurato come un accorto<hex:02100103>mercante che regge una bilancia.<hex:02100103>Il suo simbolo è la conchiglia cauri, un'antica valuta.");

        SetSingle(481,
            "Nophica, tender of soils and harvests, and goddess<hex:02100103>of abundance, is the guardian deity of Gridania.<hex:02100103>She commands the element of earth and is<hex:02100103>associated with the eleventh moon of the Eorzean<hex:02100103>calendar. Nophica is the daughter of Azeyma, and<hex:02100103>the younger sister of Llymlaen. She is most often<hex:02100103>depicted as a jubilant farmer holding a scythe of<hex:02100103>steel. Her symbol is the spring leaf.",
            "Nophica, Custode delle Zolle e dei Raccolti, Dea dell'Abbondanza,<hex:02100103>è la divinità protettrice di Gridania. Governa l'elemento<hex:02100103>della terra ed è associata all'undicesima luna del calendario<hex:02100103>eorzeano. Nophica è la figlia di Azeyma e la sorella minore<hex:02100103>di Llymlaen. È per lo più raffigurata come una contadina giubilante<hex:02100103>con una falce d'acciaio. Il suo simbolo è la foglia primaverile.");

        SetSingle(482,
            "Althyk is the surveyor of change and space,<hex:02100103>and god of time. He commands the element of<hex:02100103>earth and is associated with the twelfth moon of<hex:02100103>the Eorzean calendar. Althyk is the father of<hex:02100103>Azeyma and Menphina, and elder brother to<hex:02100103>Nymeia. He is most often depicted as an<hex:02100103>austere emperor wielding a mythril greataxe.<hex:02100103>His symbol is the hourglass.",
            "Althyk è l'Agrimensore del Mutamento e dello Spazio, Dio<hex:02100103>del Tempo. Governa l'elemento della terra ed è associato alla<hex:02100103>dodicesima luna del calendario eorzeano. Althyk è il padre di<hex:02100103>Azeyma e Menphina, e fratello maggiore di Nymeia. È per lo più<hex:02100103>raffigurato come un austero imperatore armato di una grande scure<hex:02100103>di mythril. Il suo simbolo è la clessidra.");

        // ==========================================
        // 4. CLASSI INIZIALI E RUOLI (178-192, 1804-1806)
        // ==========================================
        SetMulti(178,
            "Gladiator", "Gladiatore",
            "Gladiators specialize in the handling of all manner of one-handed\nblades, from daggers to longswords, be they single- or double-edged,\nstraight or curved. A defining characteristic of the art is its emphasis\non diverse combat tactics, training its members to bring their martial\nskills to bear in any situation. As such, there are practitioners who\nmarry sword with shield, seeking to defend their fellow companions.\nOthers opt for an empty off hand, choosing instead to focus entirely\non their sword arm. In all instances, the ages-old art of the sword\nmakes the gladiator a formidable adversary in any encounter.\n\nCorresponding Job: Paladin",
            "I Gladiatori sono maestri nell'impiego di ogni genere di lama a una mano,\ndai pugnali alle spade lunghe, siano esse a filo singolo o doppio, diritte\no ricurve. Tratto distintivo dell'arte è l'enfasi posta su tattiche di combattimento\neterogenee, addestrando i praticanti ad applicare le proprie doti marziali\nin qualsiasi frangente. Vi sono guerrieri che abbinano spada e scudo per difendere\ni propri alleati, mentre altri prediligono la mano secondaria libera, concentrandosi\nesclusivamente sul braccio armato. In ogni caso, l'antichissima arte della spada\nrende il gladiatore un avversario formidabile in qualunque scontro.\n\nJob Corrispondente: Paladino");

        SetMulti(180,
            "Pugilist", "Pugile",
            "Pugilists are hand-to-hand fighters who rely on physical conditioning and\nagility, transforming their limbs into deadly weapons. Trained in traditional\nboxing and the use of cestuses and knuckles, practitioners deliver lightning\nflurries of strikes capable of breaking enemy guards. Through rigorous training,\nthey channel their momentum to overwhelm opponents with relentless tempo.\n\nCorresponding Job: Monk",
            "I Pugili sono combattenti corpo a corpo che fanno affidamento sull'agilità e sulla forza\ndelle proprie membra, trasformando il proprio corpo in un'arma micidiale. Addestrati\nnelle discipline del combattimento e nell'impiego di tirapugni e guanti d'arme,\ni praticanti sferrano raffiche di pugni fulminei capaci di scardinare le difese nemiche.\nGrazie a un costante addestramento fisico e spirituale, incanalano il proprio impeto\nper sopraffare l'avversario con ritmo implacabile.\n\nJob Corrispondente: Monaco");

        SetMulti(182,
            "Marauder", "Incursore",
            "Marauders are colossi of the battlefield who wield great two-handed greataxes.\nBorn from the ancient maritime traditions of Lominsan corsairs, they favor brute\nstrength and shock tactics, cleaving through armor and shields with devastating\nblows. Capable of weathering punishing attacks through unyielding grit, marauders\nstand in the vanguard, drawing enemy ire to safeguard their comrades.\n\nCorresponding Job: Warrior",
            "Gli Incursori sono colossi del campo di battaglia armati di possenti asce da guerra bipenni.\nNati dall'antica tradizione marinara dei corsari di Lominsa, privilegiano la forza bruta\ne l'intimidazione, abbattendo fendenti devastanti capaci di spaccare corazze e scudi nemici.\nIn grado di incassare colpi impressionanti grazie a una tempra granitica, gli incursori\nsi pongono in prima linea attirando su di sé l'attenzione dei nemici per proteggere i propri compagni.\n\nJob Corrispondente: Guerriero");

        SetMulti(184,
            "Lancer", "Lanciere",
            "Lancers excel in the martial art of polearms, wielding spears, halberds,\nand pikes with unmatched precision. Utilizing their weapon's reach, they keep\nfoes at bay before delivering deadly thrusts and sweeping strikes. Continuous\nrefinement of balance and positioning allows them to strike unerringly at vulnerabilities\nin enemy defenses, preparing to leap with formidable force.\n\nCorresponding Job: Dragoon",
            "I Lancieri eccellono nell'arte dell'asta, impugnando lance, picche e alabarde con maestria impareggiabile.\nSfruttando la portata delle proprie armi, mantengono i nemici a distanza di sicurezza per poi sferrare\nstoccate fulminee e letali fendenti circolari. Il continuo studio dell'equilibrio e del posizionamento\nconsente loro di colpire con precisione chirurgica i punti deboli delle difese avversarie,\npreparandosi a spiccare balzi poderosi.\n\nJob Corrispondente: Dragoon");

        SetMulti(186,
            "Archer", "Arciere",
            "Archers are masters of ranged combat, blessed with keen eyes and deep knowledge\nof wind currents and ballistic arcs. Originating in the dense canopy of the Black\nShroud, the way of the bow allows rapid fire while maneuvering fluidly across the field.\nSoftening targets long before they can draw near, archers bolster their companions\nwith swift, coordinated tactics.\n\nCorresponding Job: Bard",
            "Gli Arcieri sono maestri del combattimento a distanza, dotati di una mira infallibile e di una profonda\nconoscenza dei venti e delle traiettorie balistiche. Nata nelle folte foreste del Velo Nero,\nla disciplina dell'arco consente di scoccare frecce a raffica mentre ci si sposta con agilità sul terreno di scontro.\nIn grado di indebolire i nemici prima ancora che possano avvicinarsi, gli arcieri supportano i compagni\ncon tattiche agili e coordinate.\n\nJob Corrispondente: Bardo");

        SetMulti(188,
            "Conjurer", "Incantatore",
            "Conjurers commune with the primal forces of nature, channeling the elements of\nearth, wind, and water. Deeply attuned to the elementals dwelling within the realm,\nthey draw pure aether to mend comrades' wounds, cleanse afflictions, and unleash\nnature's wrath against any who threaten balance. Their craft is an inexhaustible\nfount of solace and protection for every fellowship.\n\nCorresponding Job: White Mage",
            "Gli Incantatori attingono alle forze primordiali della natura, invocando gli elementi della terra, del vento\ne dell'acqua. In profonda comunione con gli spiriti elementali che dimorano nel reame, incanalano l'etere\npuro per risanare le ferite degli alleati, purificare le impurità e scagliare vendette della natura contro chi minaccia\nl'armonia del creato. La loro magia è fonte inesauribile di sostegno e protezione per ogni avventuriero.\n\nJob Corrispondente: Mago Bianco");

        SetMulti(190,
            "Thaumaturge", "Taumaturgo",
            "Thaumaturges manipulate destructive aether through the mystical oscillation of\nfire, ice, and lightning. Originating in the mortuary crypts of Ul'dah, they channel\nsearing flames to inflict catastrophic ruin, then transition to biting frost to replenish\ntheir aetherial reserves in an unceasing, rhythmic cycle. Their mastery over the dark\narts makes them among the deadliest magical attackers in the realm.\n\nCorresponding Job: Black Mage",
            "I Taumaturghi manipolano l'etere distruttivo attraverso l'alternanza mistica di fuoco, ghiaccio e folgore.\nOriginari delle cripte e dei complessi funerari di Ul'dah, incanalano le fiamme ardenti per infliggere danni\ncatastrofici, per poi passare al gelo profondo onde rigenerare le proprie riserve eteriche in un ciclo perfetto\ne inarrestabile. La loro padronanza delle arti oscure li rende tra gli attaccanti magici più devastanti del reame.\n\nJob Corrispondente: Mago Nero");

        SetMulti(192,
            "Arcanist", "Arcanista",
            "Arcanists apply geometric rigor and mystic equations to the shaping of aether,\nweaving intricate patterns with conductive inks inside their grimoires. Through\nthese formulae, they breathe life into Carbuncles, faithful aetherial familiars\nthat battle at their side. Skilled in debilitating enemies and administering field\nsaid, arcanists embody the union of scholastic scholarship and battlefield strategy.\n\nCorresponding Jobs: Summoner / Scholar",
            "Gli Arcanisti applicano rigore geometrico ed equazioni mistiche alla manipolazione dell'etere,\ntracciando complessi schemi geometrici con inchiostri conduttivi all'interno dei propri grimori.\nGrazie a tali formule, infondono vita ai Carbuncle, creature eteriche di supporto che combattono al loro fianco.\nAbili sia nell'applicare debilitazioni persistenti sui nemici che nel prestare prime cure agli alleati,\ngli arcanisti incarnano la fusione ideale tra studio accademico e strategia tattica.\n\nJob Corrispondenti: Evocatore / Studioso");

        SetMulti(1804,
            "Tank", "Tank",
            "Stand as a bulwark in the face of danger.\nWith superior defensive abilities, tanks protect their comrades\nby diverting enemy focus and bearing the brunt of battle.\n\nBegin your journey as a gladiator or marauder, and later master\nthe jobs of paladin, warrior, dark knight, and gunbreaker.",
            "Ergiti come un baluardo dinanzi al pericolo.\nGrazie a doti difensive superiori, i tank proteggono i compagni\ndistogliendo l'attenzione dei nemici e sopportando il peso dello scontro.\n\nInizia la tua avventura come gladiatore o incursore, per poi padroneggiare\ni job di paladino, guerriero, cavaliere oscuro ed eterlama.");

        SetMulti(1805,
            "Healer", "Curatore",
            "Provide succor to all in need.\nThough limited in their offensive abilities, healers replenish\nhealth and protect their comrades with myriad rejuvenating skills.\n\nBegin your journey as a conjurer, and later master the jobs of\nwhite mage, scholar, astrologian, and sage.\n※Please note that the job of scholar is unlocked via the \nDPS class of arcanist.",
            "Porta conforto e sollievo a chiunque ne abbia bisogno.\nBenché dotati di capacità offensive limitate, i curatori ristorano la salute\ne difendono i compagni con una miriade di abilità rigeneranti.\n\nInizia la tua avventura come incantatore, per poi padroneggiare i job di\nmago bianco, studioso, astrologo e saggio.\n※Nota: il job dello studioso si sblocca tramite la classe d'attacco dell'arcanista.");

        SetMulti(1806,
            "DPS", "DPS",
            "Decimate your foes.\nPossessing great talent for the offensive, DPS is a role whose sole\nfocus is to rout the enemy with a diverse array of punishing attacks.\n\nBegin your journey as a pugilist, lancer, archer, thaumaturge, or\narcanist. From there, master the jobs of monk, dragoon, ninja, samurai,\nreaper, viper, bard, machinist, dancer, black mage, summoner, red mage,\npictomancer, and blue mage.\n\nNote that the DPS role is divided into three categories.\nMelee DPS specializes in close combat; physical ranged DPS wields arms\nsuch as guns and bows; and magical ranged DPS uses arcane powers to\nassert dominance upon the battlefield.",
            "Annienta i tuoi avversari.\nDotati di un talento straordinario per l'offensiva, i DPS sono un ruolo il cui unico\nobiettivo è sbaragliare il nemico con una variegata sequenza di attacchi devastanti.\n\nInizia la tua avventura come pugile, lanciere, arciere, taumaturgo o\narcanista. Da lì, padroneggia i job di monaco, dragoon, ninja, samurai,\nmietitore, vipera, bardo, artificiere, danzatore, mago nero, evocatore, mago rosso,\npittomante e mago blu.\n\nIl ruolo di DPS si suddivide in tre categorie:\nDPS da mischia (specializzato nel combattimento ravvicinato), DPS fisico a distanza (armi da fuoco e archi)\ne DPS magico a distanza (poteri arcani per dominare il campo di battaglia).");

        // ==========================================
        // 5. CALENDARIO EORZEANO E CITTÀ DI PARTENZA
        // ==========================================
        SetMulti(1973,
            "Eorzean Calendar", "Calendario Eorzeano",
            "The Eorzean year consists of the moon, Menphina, revolving\naround Hydaelyn's two astral and umbral poles while fluctuating\nbetween each of the six elements. In this, the year can be said\nto be the length of twelve moons.",
            "L'anno eorzeano è scandito dal moto della luna Menphina, che ruota\nattorno al polo astrale e ombrale di Hydaelyn oscillando ciclicamente\ntra ciascuno dei sei elementi. In tal modo, si può dire che l'anno\nabbia la durata di dodici lune.");

        SetMulti(1980,
            "Limsa Lominsa", "Limsa Lominsa",
            "On the southern coast of the island of Vylbrand, under the shadow of\nancient cliffs worn by the relentless onslaught of the Rhotano Sea,\nlies the thalassocracy of Limsa Lominsa. Its economy is driven\nprimarily by shipping, but boasts lucrative shipbuilding, fishing, and\nsmithing industries as well. To maintain the safety of its maritime\nroutes, the city employs a formidable navy known as the Knights\nof the Barracuda. Even so, pirate bands run rampant in nearby waters,\nreaving and pillaging.",
            "Sulla costa meridionale dell'isola di Vylbrand, all'ombra di antiche scogliere scolpite dall'incessante impeto del Mare di Rhotano, sorge la talassocrazia di Limsa Lominsa. La sua economia prospera principalmente grazie alla navigazione marittima, vantando fiorenti cantieri navali, pesca d'altura e forge metallurgiche. Per garantire la sicurezza delle rotte marittime, la città schiera una formidabile flotta nota come Cavalieri del Barracuda. Ciononostante, bande di pirati continuano a infestare le acque circostanti, dedite a scorrerie e saccheggi.");

        SetMulti(1981,
            "Gridania", "Gridania",
            "In the eastern reaches of the Aldenard landmass, home to vast, dense\nwoodlands and coursing rivers, lies the forest nation of Gridania. The\ncityscape is a mosaic of labyrinthine waterways and great wooden\nstructures, so gracefully constructed they seem a part of the\nsurrounding environment. The favored goddess of the citizenry is\nNophica, the Matron, but great faith is also placed in the wisdom of\nthe Seedseers－young oracles who guide the nation based on the\nwill of the forest's elementals.",
            "Nelle propaggini orientali della massa continentale di Aldenard, culla di vaste e fitte selve e fiumi impetuosi, sorge la nazione silvana di Gridania. Il paesaggio urbano è un mosaico armonioso di canali d'acqua labirintici e grandi strutture in legno, modellate con tale grazia da sembrare parte integrante dell'ambiente naturale. La dea più venerata dalla cittadinanza è Nophica, la Matrona, ma profonda devozione è riposta anche nella saggezza dei Veggenti, giovani oracoli che guidano la nazione secondo la volontà degli spiriti elementali del bosco.");

        SetMulti(1982,
            "Ul'dah", "Ul'dah",
            "The bustling commercial hub of Ul'dah sits amid the desolate desert\nlandscape of southern Aldenard. Ul'dahn culture is known for its\naffluence, and the wealth of the nation comes in large part from its\nabundant mineral resources and clothcrafting industry. Though it is\nthe sultana who claims sovereignty, true power is wielded by the\nSyndicate, a council sat by six of Ul'dah's most elite and influential.\nNald'thal is the nation's patron deity, and two great halls devoted to\nHis two aspects lie in the east and west of the city.",
            "Il vivace crocevia commerciale di Ul'dah sorge nel cuore dell'arido paesaggio desertico dell'Aldenard meridionale. La cultura uldhiana è celebre per l'opulenza e la ricchezza della nazione, derivante in gran parte dalle abbondanti risorse minerarie e dall'industria tessile. Sebbene la sovranità appartenga alla Sultana, il vero potere è esercitato dal Sindacato, un consiglio formato da sei tra gli esponenti più influenti ed elitari della città. Nald'thal è la divinità patrona della nazione, e due imponenti sale consacrate ai suoi due aspetti sorgono a oriente e a occidente della città.");

        // ==========================================
        // 6. FASI DI CREAZIONE E SLIDER ESTETICI (101-107, 200-258)
        // ==========================================
        SetSingle(101, "Race & Gender", "Razza e Genere");
        SetSingle(102, "Clan", "Clan");
        SetSingle(103, "Gender", "Genere");
        SetSingle(104, "Appearance", "Aspetto");
        SetSingle(105, "Nameday", "Genetliaco");
        SetSingle(106, "Guardian", "Divinità Patrona");
        SetSingle(107, "Class", "Classe");

        SetSingle(200, "Presets", "Preimpostazioni");
        SetSingle(201, "Height", "Altezza");
        SetSingle(202, "Skin Color", "Carnagione");
        SetSingle(203, "Voice", "Voce");
        SetSingle(204, "Muscle Tone", "Tono Muscolare");
        SetSingle(207, "Muscle Tone", "Tono Muscolare");
        SetSingle(209, "Bust Size", "Dimensioni Seno");
        SetSingle(210, "Ear Size", "Dimensioni Orecchie");
        SetSingle(211, "Ear Shape", "Forma Orecchie");
        SetSingle(213, "Ear Size", "Dimensioni Orecchie");
        SetSingle(214, "Ear Shape", "Forma Orecchie");
        SetSingle(215, "Bust Size", "Dimensioni Seno");
        SetSingle(216, "Ear Size", "Dimensioni Orecchie");
        SetSingle(217, "Ear Shape", "Forma Orecchie");
        SetSingle(219, "Ear Size", "Dimensioni Orecchie");
        SetSingle(220, "Ear Shape", "Forma Orecchie");
        SetSingle(221, "Bust Size", "Dimensioni Seno");
        SetSingle(222, "Tail Length", "Lunghezza Coda");
        SetSingle(223, "Tail Shape", "Forma Coda");
        SetSingle(225, "Tail Length", "Lunghezza Coda");
        SetSingle(226, "Tail Shape", "Forma Coda");
        SetSingle(227, "Bust Size", "Dimensioni Seno");
        SetSingle(228, "Muscle Tone", "Tono Muscolare");
        SetSingle(231, "Muscle Tone", "Tono Muscolare");
        SetSingle(233, "Bust Size", "Dimensioni Seno");
        SetSingle(234, "Hairstyle", "Acconciatura");
        SetSingle(235, "Hair Features", "Dettagli Capelli");
        SetSingle(236, "Hair Color", "Colore Capelli");
        SetSingle(237, "Highlights", "Riflessi");
        SetSingle(238, "Face", "Viso");
        SetSingle(239, "Facial Features", "Tratti Somatici");
        SetSingle(240, "Facial Feature Color", "Colore Tratti Somatici");
        SetSingle(241, "Jaw", "Mascella");
        SetSingle(242, "Eyebrows", "Sopracciglia");
        SetSingle(243, "Eye Shape", "Taglio degli Occhi");
        SetSingle(244, "Iris Size", "Dimensione Iride");
        SetSingle(245, "Eye Color", "Colore Occhi");
        SetSingle(246, "Nose", "Naso");
        SetSingle(247, "Mouth", "Bocca");
        SetSingle(248, "Lip Color", "Colore Labbra");
        SetSingle(249, "Face Paint", "Trucco Facciale");
        SetSingle(250, "Face Paint Color", "Colore Trucco Facciale");
        SetSingle(252, "Face", "Viso");
        SetSingle(253, "Tail Length", "Lunghezza Coda");
        SetSingle(254, "Tail Shape", "Forma Coda");
        SetSingle(256, "Tail Length", "Lunghezza Coda");
        SetSingle(257, "Tail Shape", "Forma Coda");
        SetSingle(258, "Bust Size", "Dimensioni Seno");

        // ==========================================
        // 7. ISTRUZIONI E DIALOGHI (1950-1984)
        // ==========================================
        SetSingle(1950, "Select your character's race and gender.", "Seleziona la razza e il genere del tuo personaggio.");
        SetSingle(1951, "Select your character's clan.", "Seleziona il clan del tuo personaggio.");
        SetSingle(1952, "Select your character's gender.", "Seleziona il genere del tuo personaggio.");
        SetSingle(1953, "Customize your character's appearance.", "Personalizza l'aspetto del tuo personaggio.");
        SetSingle(1954,
            "Specify your character's date of birth.<hex:02100103><hex:02100103><hex:0248020203><hex:0249022503>This selection has no effect on gameplay.<hex:0249020103><hex:0248020103>",
            "Specifica la data di nascita del tuo personaggio.<hex:02100103><hex:02100103><hex:0248020203><hex:0249022503>Questa scelta non influisce sull'esperienza di gioco.<hex:0249020103><hex:0248020103>");
        SetSingle(1955,
            "Select your character's patron deity.<hex:02100103><hex:02100103><hex:0248020203><hex:0249022503>This selection has no effect on gameplay.<hex:0249020103><hex:0248020103>",
            "Seleziona la divinità patrona del tuo personaggio.<hex:02100103><hex:02100103><hex:0248020203><hex:0249022503>Questa scelta non influisce sull'esperienza di gioco.<hex:0249020103><hex:0248020103>");
        SetSingle(1956, "Select your starting class and city-state.", "Seleziona la tua classe iniziale e la città-stato di partenza.");
        SetSingle(1961,
            "View your character in the set of race-specific gear he or she will be wearing at the game's onset.",
            "Visualizza il tuo personaggio nell'equipaggiamento tipico della sua razza indossato all'inizio del gioco.");
        SetSingle(1962,
            "View your character in a set of job-specific gear based on the selected class. Available after class selection.",
            "Visualizza il tuo personaggio nell'equipaggiamento esclusivo del job associato alla classe. Disponibile dopo aver selezionato la classe.");
        SetSingle(1970,
            "Quit character creation and return to the Character Selection screen. You will lose all current progress.",
            "Abbandonare la creazione del personaggio e tornare alla schermata di Selezione Personaggio? Tutti i progressi attuali andranno perduti.");
        SetSingle(1977, "Adjust lighting and time of day.", "Regola l'illuminazione e l'ora del giorno.");
        SetSingle(1984,
            "Both forename and surname must be between 2 and 15 characters and not total more than 20 characters combined. Only letters, hyphens, and apostrophes can be used. The first character of either name must be a letter. Hyphens cannot be used in succession or placed immediately before or after apostrophes.",
            "Sia il nome che il cognome devono contenere tra 2 e 15 caratteri e non superare complessivamente 20 caratteri totali. È consentito l'uso esclusivo di lettere, trattini e apostrofi. Il primo carattere di ciascun nome deve essere una lettera. Non è possibile inserire trattini consecutivi o posti immediatamente prima o dopo un apostrofo.");

        // Salva con ordine numerico delle chiavi per una leggibilità perfetta
        var sorted = new JsonObject();
        var orderedKeys = root.Select(p => uint.Parse(p.Key)).OrderBy(k => k);
        foreach (var k in orderedKeys)
        {
            sorted[k.ToString()] = root[k.ToString()]!.DeepClone();
        }

        var opt = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(lobbyJsonPath, sorted.ToJsonString(opt));
    }
}
