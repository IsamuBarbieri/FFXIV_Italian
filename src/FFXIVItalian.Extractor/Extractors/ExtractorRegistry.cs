using System;
using System.Collections.Generic;
using System.Linq;

namespace FFXIVItalian.Extractor.Extractors;

public static class ExtractorRegistry
{
    private static readonly Dictionary<string, ISheetExtractor> Extractors = new(StringComparer.OrdinalIgnoreCase)
    {
        // Tier 1: Boot & System
        ["lobby"] = new LobbyExtractor(),
        ["addon"] = new AddonExtractor(),
        ["maincommand"] = new MainCommandExtractor(),
        ["maincommandcategory"] = new MainCommandCategoryExtractor(),
        ["error"] = new ErrorExtractor(),
        ["logmessage"] = new UniversalSheetExtractor("LogMessage", "logmessage.json", "Messaggi di sistema, log di combattimento e chat."),
        ["textcommand"] = new UniversalSheetExtractor("TextCommand", "textcommand.json", "Comandi slash della chat (/tell, /party, /pinfo...)."),
        ["howto"] = new UniversalSheetExtractor("HowTo", "howto.json", "Guide di Aiuto Attivo (Tutorial e spiegazioni di gioco)."),
        ["howtocategory"] = new UniversalSheetExtractor("HowToCategory", "howtocategory.json", "Categorie delle Guide di Aiuto Attivo."),

        // Tier 2: World & Character
        ["classjob"] = new ClassJobExtractor(),
        ["race"] = new RaceExtractor(),
        ["tribe"] = new TribeExtractor(),
        ["placename"] = new PlaceNameExtractor(),
        ["grandcompany"] = new UniversalSheetExtractor("GrandCompany", description: "Nomi delle Grandi Compagnie."),
        ["fcreputation"] = new UniversalSheetExtractor("FCReputation", description: "Gradi di reputazione delle Grandi Compagnie."),
        ["beastreputationrank"] = new UniversalSheetExtractor("BeastReputationRank", description: "Gradi di reputazione delle società alleate."),
        ["gcranklimsamaletext"] = new UniversalSheetExtractor("GCRankLimsaMaleText", description: "Gradi della Maelstrom, forma maschile."),
        ["gcranklimSafemaletext"] = new UniversalSheetExtractor("GCRankLimsaFemaleText", description: "Gradi della Maelstrom, forma femminile."),
        ["gcrankgridaniamaletext"] = new UniversalSheetExtractor("GCRankGridaniaMaleText", description: "Gradi dell'Ordine della Vipera Gemella, forma maschile."),
        ["gcrankgridaniafemaletext"] = new UniversalSheetExtractor("GCRankGridaniaFemaleText", description: "Gradi dell'Ordine della Vipera Gemella, forma femminile."),
        ["gcrankuldahmaletext"] = new UniversalSheetExtractor("GCRankUldahMaleText", description: "Gradi delle Fiamme Immortali, forma maschile."),
        ["gcrankuldahfemaletext"] = new UniversalSheetExtractor("GCRankUldahFemaleText", description: "Gradi delle Fiamme Immortali, forma femminile."),
        ["title"] = new UniversalSheetExtractor("Title", "title.json", "Titoli onorifici e sbloccabili del personaggio."),
        ["weather"] = new UniversalSheetExtractor("Weather", "weather.json", "Condizioni meteorologiche (Sereno, Pioggia, Nebbia...)."),

        // Tier 3: Gameplay & Actions
        ["action"] = new UniversalSheetExtractor("Action", "action.json", "Nomi delle abilità, magie e mosse di combattimento."),
        ["actiontransient"] = new UniversalSheetExtractor("ActionTransient", "actiontransient.json", "Descrizioni ed effetti delle abilità nei tooltip."),
        ["status"] = new UniversalSheetExtractor("Status", "status.json", "Nomi e descrizioni di effetti di stato, buff e debuff."),
        ["trait"] = new UniversalSheetExtractor("Trait", "trait.json", "Tratti passivi delle classi e mestieri."),
        ["traittransient"] = new UniversalSheetExtractor("TraitTransient", "traittransient.json", "Descrizioni dei tratti passivi."),

        // Tier 4: Items
        ["item"] = new UniversalSheetExtractor("Item", "item.json", "Nomi e descrizioni degli oggetti ed equipaggiamento."),
        ["itemuicategory"] = new UniversalSheetExtractor("ItemUICategory", "itemuicategory.json", "Categorie UI degli oggetti nell'inventario."),

        // Tier 5: Dialogue & World Events
        ["balloon"] = new UniversalSheetExtractor("Balloon", "balloon.json", "Fumetti di dialogo sopra la testa degli NPC."),
        ["defaulttalk"] = new UniversalSheetExtractor("DefaultTalk", "defaulttalk.json", "Dialoghi standard degli NPC nel mondo."),
        ["customtalk"] = new UniversalSheetExtractor("CustomTalk", "customtalk.json", "Dialoghi speciali e interazioni NPC."),
        ["fate"] = new UniversalSheetExtractor("Fate", "fate.json", "Titoli, descrizioni e obiettivi dei FATE."),
        ["achievement"] = new UniversalSheetExtractor("Achievement", "achievement.json", "Trofei e obiettivi sbloccabili."),
        ["instancecontent"] = new UniversalSheetExtractor("InstanceContent", "instancecontent.json", "Dungeon, Trial e Raid.")
    };

    public static ISheetExtractor? Get(string sheetName)
    {
        if (Extractors.TryGetValue(sheetName, out var extractor))
            return extractor;

        // Fallback: allow extracting any arbitrary game sheet via UniversalSheetExtractor
        return new UniversalSheetExtractor(sheetName);
    }

    public static IReadOnlyList<ISheetExtractor> GetAll()
    {
        return Extractors.Values.ToList();
    }

    public static IReadOnlyList<string> GetNames()
    {
        return Extractors.Keys.ToList();
    }
}
