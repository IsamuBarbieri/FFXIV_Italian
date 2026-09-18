using System.Text.Json;

namespace FFXIVItalian.Core.Glossary;

public static class GlossaryLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static GlossaryEngine CreateCanonicalEngine()
    {
        var engine = new GlossaryEngine();

        // G6 - Aether Family
        Add(engine, "aether", "etere", GlossaryCategory.Aether, "G6");
        Add(engine, "aetheric", "eterico", GlossaryCategory.Aether, "G6");
        Add(engine, "aetherial", "eterico", GlossaryCategory.Aether, "G6");
        Add(engine, "Aetheryte", "Eterite", GlossaryCategory.Aether, "G6");
        Add(engine, "Aetheryte shard", "Scheggia d'Eterite", GlossaryCategory.Aether, "G6");
        Add(engine, "Aethernet", "Eternet", GlossaryCategory.Aether, "G6");
        Add(engine, "aether current", "corrente eterica", GlossaryCategory.Aether, "G6");
        Add(engine, "Aetherial Sea", "Mare Etereo", GlossaryCategory.Aether, "G6");
        Add(engine, "Lifestream", "Flusso Vitale", GlossaryCategory.Aether, "G6");
        Add(engine, "ceruleum", "ceruleo", GlossaryCategory.Aether, "G6");
        Add(engine, "attunement", "sintonizzazione", GlossaryCategory.Aether, "G6");

        // G7 - Coined & Game Terms
        Add(engine, "Linkpearl", "Fonoperla", GlossaryCategory.GameTerm, "G7");
        Add(engine, "Linkshell", "Fonosfera", GlossaryCategory.GameTerm, "G7");
        Add(engine, "Tomestone", "Tomopietra", GlossaryCategory.GameTerm, "G7");
        Add(engine, "Allagan Tomestone", "Tomopietra Allagana", GlossaryCategory.GameTerm, "G7");
        Add(engine, "Levequest", "Incarico", GlossaryCategory.GameTerm, "G7");
        Add(engine, "Soul Crystal", "Cristallo dell'Anima", GlossaryCategory.GameTerm, "G7");
        Add(engine, "Airship", "Aeronave", GlossaryCategory.GameTerm, "G7");
        Add(engine, "Gysahl Greens", "Erba Gysahl", GlossaryCategory.GameTerm, "G7");
        Add(engine, "Phoenix Down", "Piuma di Fenice", GlossaryCategory.GameTerm, "G7");
        Add(engine, "Phoenix Tail", "Coda di Fenice", GlossaryCategory.GameTerm, "G7");
        Add(engine, "Aether Sickness", "Mal d'Etere", GlossaryCategory.GameTerm, "G7",
            prohibited: ["Malattia Eterica", "Mal di Etere"]);
        Add(engine, "The Calamity", "la Calamità", GlossaryCategory.GameTerm, "G7");
        Add(engine, "Seventh Umbral Era", "Settima Era Ombrosa", GlossaryCategory.GameTerm, "G7");
        Add(engine, "Seventh Astral Era", "Settima Era Astrale", GlossaryCategory.GameTerm, "G7");

        // G8 - Demonyms
        Add(engine, "Eorzean", "Eorzeano/a", GlossaryCategory.Demonym, "G8");
        Add(engine, "Lominsan", "Lominsano/a", GlossaryCategory.Demonym, "G8");
        Add(engine, "Gridanian", "Gridaniano/a", GlossaryCategory.Demonym, "G8");
        Add(engine, "Ul'dahn", "Uldano/a", GlossaryCategory.Demonym, "G8");
        Add(engine, "Garlean", "Garleano/a", GlossaryCategory.Demonym, "G8");
        Add(engine, "Sharlayan", "Sharlayano/a", GlossaryCategory.Demonym, "G8");
        Add(engine, "Doman", "Domano/a", GlossaryCategory.Demonym, "G8");

        // G9 - Places & Hubs
        Add(engine, "Black Shroud", "Velo Nero", GlossaryCategory.Place, "G9");
        Add(engine, "The Waking Sands", "Sabbie del Risveglio", GlossaryCategory.Place, "G9");
        Add(engine, "Whitebrim", "Orlo Bianco", GlossaryCategory.Place, "G9");
        Add(engine, "Whitebrim Front", "Avamposto di Orlo Bianco", GlossaryCategory.Place, "G9");
        Add(engine, "Garlond Ironworks", "Officine Garlond", GlossaryCategory.Place, "G9");
        Add(engine, "Aleport", "Portobirra", GlossaryCategory.Place, "G9");
        Add(engine, "Wineport", "Portovino", GlossaryCategory.Place, "G9");
        Add(engine, "Vesper Bay", "Baia del Vespro", GlossaryCategory.Place, "G9");
        Add(engine, "Bronze Lake", "Lago di Bronzo", GlossaryCategory.Place, "G9");
        Add(engine, "Camp Drybone", "Campo Ossasecca", GlossaryCategory.Place, "G9");
        Add(engine, "Copperbell Mines", "Miniere di Camparame", GlossaryCategory.Place, "G9");
        Add(engine, "Wanderer's Palace", "Palazzo del Viandante", GlossaryCategory.Place, "G9");
        Add(engine, "Camp Overlook", "Campo Belvedere", GlossaryCategory.Place, "G9");
        Add(engine, "Little Solace", "Piccolo Rifugio", GlossaryCategory.Place, "G9");
        Add(engine, "Highbridge", "Ponte Alto", GlossaryCategory.Place, "G9");
        Add(engine, "Quarrymill", "Cavamulino", GlossaryCategory.Place, "G9");
        Add(engine, "Bentbranch Meadows", "Prati di Ramostorto", GlossaryCategory.Place, "G9");
        Add(engine, "Hawthorne Hut", "Capanna Biancospino", GlossaryCategory.Place, "G9");
        Add(engine, "Summerford Farms", "Guado d'Estate", GlossaryCategory.Place, "G9");
        Add(engine, "Fallgourd Float", "Zuccacaduta", GlossaryCategory.Place, "G9");
        Add(engine, "The Rising Stones", "Le Pietre Risorte", GlossaryCategory.Place, "G9");
        Add(engine, "Revenant's Toll", "Pedaggio del Redivivo", GlossaryCategory.Place, "G9");

        // G10 - Factions & Titles
        Add(engine, "The Maelstrom", "La Tempesta", GlossaryCategory.Faction, "G10",
            prohibited: ["Il Maelstrom", "Maelstrom"]);
        Add(engine, "Order of the Twin Adder", "Ordine della Vipera Gemella", GlossaryCategory.Faction, "G10");
        Add(engine, "Immortal Flames", "Fiamme Immortali", GlossaryCategory.Faction, "G10");
        Add(engine, "Yellowjackets", "Giubbe Gialle", GlossaryCategory.Faction, "G10");
        Add(engine, "Brass Blades", "Lame d'Ottone", GlossaryCategory.Faction, "G10");
        Add(engine, "Wood Wailers", "Sentinelle del Bosco", GlossaryCategory.Faction, "G10");
        Add(engine, "Gods' Quiver", "Faretra degli Dei", GlossaryCategory.Faction, "G10");
        Add(engine, "Sultansworn", "Le Spade della Sultana", GlossaryCategory.Faction, "G10");
        Add(engine, "Temple Knights", "Cavalieri del Tempio", GlossaryCategory.Faction, "G10");
        Add(engine, "Scions of the Seventh Dawn", "Figli della Settima Alba", GlossaryCategory.Faction, "G10");
        Add(engine, "Students of Baldesion", "Studenti di Baldesion", GlossaryCategory.Faction, "G10");
        Add(engine, "Crystal Braves", "Bravi di Cristallo", GlossaryCategory.Faction, "G10");
        Add(engine, "Garlean Empire", "Impero Garleano", GlossaryCategory.Faction, "G10");
        Add(engine, "Warrior of Light", "Guerriero della Luce", GlossaryCategory.Faction, "G10");
        Add(engine, "Grand Company", "Grande Compagnia", GlossaryCategory.Faction, "G10");
        Add(engine, "Free Company", "Compagnia Libera", GlossaryCategory.Faction, "G10");

        // G11 - Taverns & Surnames
        Add(engine, "The Drowning Wench", "La Fanciulla Annegata", GlossaryCategory.Place, "G11");
        Add(engine, "The Carline Canopy", "Il Baldacchino di Carline", GlossaryCategory.Place, "G11");
        Add(engine, "The Quicksand", "Le Sabbie Mobili", GlossaryCategory.Place, "G11");
        Add(engine, "Buscarron's Druthers", "Il Capriccio di Buscarron", GlossaryCategory.Place, "G11",
            prohibited: ["Il Bevitoio di Buscarron"]);
        Add(engine, "Haurchefant Greystone", "Haurchefant Pietragrigia", GlossaryCategory.NpcSurnameOrEpithet, "G11");
        Add(engine, "Estinien Wyrmblood", "Estinien Sanguedidrago", GlossaryCategory.NpcSurnameOrEpithet, "G11");
        Add(engine, "Baderon Tenfingers", "Baderon Diecidita", GlossaryCategory.NpcSurnameOrEpithet, "G11");
        Add(engine, "Gerolt Blackthorn", "Gerolt Spinanera", GlossaryCategory.NpcSurnameOrEpithet, "G11");

        // G18 - Classes
        Add(engine, "Gladiator", "Gladiatore", GlossaryCategory.Class, "G18");
        Add(engine, "Paladin", "Paladino", GlossaryCategory.Class, "G18");
        Add(engine, "Marauder", "Incursore", GlossaryCategory.Class, "G18");
        Add(engine, "Warrior", "Guerriero", GlossaryCategory.Class, "G18");
        Add(engine, "Dark Knight", "Cavaliere Oscuro", GlossaryCategory.Class, "G18");
        Add(engine, "Gunbreaker", "Eterlama", GlossaryCategory.Class, "G18");
        Add(engine, "White Mage", "Mago Bianco", GlossaryCategory.Class, "G18");
        Add(engine, "Scholar", "Studioso", GlossaryCategory.Class, "G18");
        Add(engine, "Astrologian", "Astrologo", GlossaryCategory.Class, "G18");
        Add(engine, "Sage", "Saggio", GlossaryCategory.Class, "G18");
        Add(engine, "Monk", "Monaco", GlossaryCategory.Class, "G18");
        Add(engine, "Dragoon", "Dragoon", GlossaryCategory.Class, "G18");
        Add(engine, "Ninja", "Ninja", GlossaryCategory.Class, "G18");
        Add(engine, "Samurai", "Samurai", GlossaryCategory.Class, "G18");
        Add(engine, "Reaper", "Mietitore", GlossaryCategory.Class, "G18");
        Add(engine, "Viper", "Vipera", GlossaryCategory.Class, "G18");
        Add(engine, "Bard", "Bardo", GlossaryCategory.Class, "G18");
        Add(engine, "Machinist", "Artificiere", GlossaryCategory.Class, "G18");
        Add(engine, "Dancer", "Danzatore", GlossaryCategory.Class, "G18");
        Add(engine, "Black Mage", "Mago Nero", GlossaryCategory.Class, "G18");
        Add(engine, "Summoner", "Evocatore", GlossaryCategory.Class, "G18");
        Add(engine, "Red Mage", "Mago Rosso", GlossaryCategory.Class, "G18");
        Add(engine, "Blue Mage", "Mago Blu", GlossaryCategory.Class, "G18");
        Add(engine, "Pictomancer", "Pittomante", GlossaryCategory.Class, "G18");

        // G25 - Bestiary
        Add(engine, "Malboro", "Molboro", GlossaryCategory.Bestiary, "G25");
        Add(engine, "Morbol", "Molboro", GlossaryCategory.Bestiary, "G25");
        Add(engine, "Tonberry", "Tomberry", GlossaryCategory.Bestiary, "G25");
        Add(engine, "Moogle", "Moguri", GlossaryCategory.Bestiary, "G25");
        Add(engine, "Bomb", "Piros", GlossaryCategory.Bestiary, "G25");
        Add(engine, "Goobbue", "Gubbue", GlossaryCategory.Bestiary, "G25");
        Add(engine, "Cactuar", "Kyactus", GlossaryCategory.Bestiary, "G25");
        Add(engine, "Coeurl", "Iaguaro", GlossaryCategory.Bestiary, "G25");
        Add(engine, "Flan", "Budino", GlossaryCategory.Bestiary, "G25");
        Add(engine, "Iron Giant", "Gigante di Ferro", GlossaryCategory.Bestiary, "G25");

        // G27 - Recurring Iconic Phrases
        Add(engine, "May you ever walk in the light of the Crystal",
            "Che il Cristallo illumini per sempre il vostro cammino",
            GlossaryCategory.IconicPhrase, "G27");
        Add(engine, "Till sea swallows all",
            "Fino in fondo all'abisso",
            GlossaryCategory.IconicPhrase, "G27");
        Add(engine, "Hear... Feel... Think",
            "Ascoltate... Percepite... Pensate",
            GlossaryCategory.IconicPhrase, "G27");
        Add(engine, "For those we have lost. For those we can yet save",
            "Per coloro che abbiamo perso. Per coloro che possiamo ancora salvare",
            GlossaryCategory.IconicPhrase, "G27");
        Add(engine, "Remember us. Remember that we once lived",
            "Ricordaci... ricorda che un tempo vivemmo",
            GlossaryCategory.IconicPhrase, "G27");
        Add(engine, "A smile better suits a hero",
            "Un sorriso si addice di più a un eroe",
            GlossaryCategory.IconicPhrase, "G27");

        // Voice Profiles
        engine.AddVoiceProfile(new VoiceProfile
        {
            CharacterName = "Urianger Augurelt",
            Register = "Aulico, Arcaizzante, Solenne",
            StyleDescription = "Volgare illustre e prosa classica nobile; uso ponderato di congiunzioni nobili (onde, allorché, perocché, vostra mercé), inversioni sintattiche colte e cadenze metriche.",
            KeyTraits = ["Solennità", "Arcaismi ponderati", "Compostezza filosofica"],
            CharacteristicPhrases = ["Non v'è notte che possa spegnere la promessa dell'aurora ventura.", "Onde evitare periglio alcuno...", "Vostra mercé."]
        });

        engine.AddVoiceProfile(new VoiceProfile
        {
            CharacterName = "Thancred Waters",
            Register = "Brillante, Spigliato, Ironico",
            StyleDescription = "Charme scanzonato, battuta pronta, disincanto elegante, ritmo vivace e moderno che nasconde profonda lealtà.",
            KeyTraits = ["Ironia affettuosa", "Disinvoltura", "Prontezza di spirito"],
            CharacteristicPhrases = ["Ehilà.", "Avevamo la situazione quasi sotto controllo.", "Che ci crediate o no..."]
        });

        engine.AddVoiceProfile(new VoiceProfile
        {
            CharacterName = "Alphinaud Leveilleur",
            Register = "Accademico, Diplomatico, Eloquente",
            StyleDescription = "Frasi complesse e vocabolario ricercato da oratore precoce; evoluzione da giovane idealista orgoglioso ad alleato saggio e umile.",
            KeyTraits = ["Diplomazia", "Sintassi complessa", "Idealismo nobile"],
            CharacteristicPhrases = ["La diplomazia non è una semplice esibizione di retorica...", "Dobbiamo unire le nostre forze."]
        });

        engine.AddVoiceProfile(new VoiceProfile
        {
            CharacterName = "Alisaie Leveilleur",
            Register = "Diretta, Incisiva, Appassionata",
            StyleDescription = "Tagliente, pragmatica, insofferente ai convenevoli e alle mezze misure; frasi brevi, concise e risolute.",
            KeyTraits = ["Pragmatismo", "Franchezza tagliente", "Impazienza per i convenevoli"],
            CharacteristicPhrases = ["Bando alle ciance!", "Ci penso io prima che sia troppo tardi.", "Niente mezze misure."]
        });

        engine.AddVoiceProfile(new VoiceProfile
        {
            CharacterName = "Y'shtola Rhul",
            Register = "Flessibile, Intellettuale, Sferzante (Sassy)",
            StyleDescription = "Calma olimpica, arguzia affilata, tono sereno con frecciate micidiali sussurrate con grazia magistrale.",
            KeyTraits = ["Arguzia pungente", "Calma incrollabile", "Autorità naturale"],
            CharacteristicPhrases = ["Davvero credevi di potermi cogliere impreparata?", "Ti facevo più accorto di così.", "Bene, procediamo."]
        });

        engine.AddVoiceProfile(new VoiceProfile
        {
            CharacterName = "Tataru Taru",
            Register = "Solare, Vivace, Amministrativa",
            StyleDescription = "Squillante, affettuosa, vezzeggiativi cortesi; diventa glaciale e severa quando si parla del bilancio economico e dei Gil dei Discendenti.",
            KeyTraits = ["Squillante cordialità", "Rigore contabile inflessibile", "Affetto caloroso"],
            CharacteristicPhrases = ["Bentornati a casa!", "Ho verificato che non abbiate speso neanche un gil di troppo!", "Ci penso io, signore!"]
        });

        engine.AddVoiceProfile(new VoiceProfile
        {
            CharacterName = "Estinien Sanguedidrago",
            Register = "Laconico, Guerriero, Asciutto",
            StyleDescription = "Pochissime parole, nessun fronzolo o cerimonia diplomatica; pragmatismo militare e determinazione silenziosa.",
            KeyTraits = ["Laconismo estremo", "Pragmatismo guerriero", "Rifiuto delle formalità"],
            CharacteristicPhrases = ["Meno chiacchiere.", "La lancia è pronta.", "Non m'interessa la politica."]
        });

        engine.AddVoiceProfile(new VoiceProfile
        {
            CharacterName = "Emet-Selch",
            Register = "Teatrale, Disincantato, Tragico",
            StyleDescription = "Carisma drammatico, stanchezza millenaria, sarcasmo aristocratico mista a una solitudine straziante.",
            KeyTraits = ["Sarcasmo aristocratico", "Teatralità stanca", "Grandiosità tragica"],
            CharacteristicPhrases = ["Miei cari 'eroi'...", "Ricordaci... ricorda che un tempo vivemmo.", "Continuate pure ad agitarvi."]
        });

        engine.AddVoiceProfile(new VoiceProfile
        {
            CharacterName = "Haurchefant Pietragrigia",
            Register = "Cavalieresco, Caloroso, Esuberante",
            StyleDescription = "Entusiasmo contagioso, ammirazione sincera e affetto ardente per il Guerriero della Luce.",
            KeyTraits = ["Cavalleria generosa", "Entusiasmo solare", "Affetto devoto"],
            CharacteristicPhrases = ["Uno spettacolo magnifico!", "Un sorriso si addice di più a un eroe.", "Che splendido ardore!"]
        });

        return engine;
    }

    private static void Add(GlossaryEngine engine, string en, string it, GlossaryCategory cat, string ruleId, string[]? prohibited = null)
    {
        engine.AddEntry(new GlossaryEntry
        {
            EnglishTerm = en,
            ItalianTerm = it,
            Category = cat,
            RuleId = ruleId,
            ProhibitedForms = prohibited?.ToList() ?? []
        });
    }

    public static async Task SaveToJsonAsync(GlossaryEngine engine, string filePath)
    {
        var data = new
        {
            Entries = engine.Entries,
            VoiceProfiles = engine.VoiceProfiles.Values
        };
        var json = JsonSerializer.Serialize(data, JsonOptions);
        await File.WriteAllTextAsync(filePath, json);
    }
}

