using FFXIVItalian.Core.Glossary;
using Xunit;

namespace FFXIVItalian.Tests;

public class GlossaryTests
{
    private readonly GlossaryEngine _engine = GlossaryLoader.CreateCanonicalEngine();

    [Fact]
    public void CanonicalEngine_ContainsEssentialGlossaryEntries()
    {
        Assert.NotEmpty(_engine.Entries);
        Assert.NotEmpty(_engine.VoiceProfiles);

        // Check key lore terms from 07_Glossary.md
        Assert.Contains(_engine.Entries, e => e.EnglishTerm == "Aetheryte" && e.ItalianTerm == "Eterite");
        Assert.Contains(_engine.Entries, e => e.EnglishTerm == "The Maelstrom" && e.ItalianTerm == "La Tempesta");
        Assert.Contains(_engine.Entries, e => e.EnglishTerm == "Warrior of Light" && e.ItalianTerm == "Guerriero della Luce");
        Assert.Contains(_engine.Entries, e => e.EnglishTerm == "Scions of the Seventh Dawn" && e.ItalianTerm == "Figli della Settima Alba");
        Assert.Contains(_engine.Entries, e => e.EnglishTerm == "Haurchefant Greystone" && e.ItalianTerm == "Haurchefant Pietragrigia");
        Assert.Contains(_engine.Entries, e => e.EnglishTerm == "Estinien Wyrmblood" && e.ItalianTerm == "Estinien Sanguedidrago");
    }

    [Fact]
    public void ValidateTranslation_CatchesProhibitedMaelstromUsage()
    {
        string en = "Report to the Maelstrom command.";
        string it = "Fai rapporto al comando di Maelstrom.";

        var result = _engine.ValidateTranslation(en, it);

        Assert.False(result.IsCompliant);
        Assert.Contains(result.ProhibitedUsages, p => p.Contains("Maelstrom") && p.Contains("La Tempesta"));
    }

    [Fact]
    public void ValidateTranslation_CatchesForbiddenUnitConversions_PerRuleG28()
    {
        string en = "The target is 10 yalms away.";
        string it = "Il bersaglio dista 10 iarde da qui.";

        var result = _engine.ValidateTranslation(en, it);

        Assert.False(result.IsCompliant);
        Assert.Contains(result.Warnings, w => w.Contains("G28") && w.Contains("iarda"));
    }

    [Fact]
    public void ValidateTranslation_CatchesPluralizedYalm_PerRuleG28()
    {
        string en = "Walk 5 yalms forward.";
        string it = "Avanza di 5 yalms.";

        var result = _engine.ValidateTranslation(en, it);

        Assert.False(result.IsCompliant);
        Assert.Contains(result.Warnings, w => w.Contains("G28") && w.Contains("invariabile"));
    }

    [Fact]
    public void ValidateTranslation_AcceptsCompliantTranslation()
    {
        string en = "May you ever walk in the light of the Crystal, Warrior of Light.";
        string it = "Che il Cristallo illumini per sempre il vostro cammino, Guerriero della Luce.";

        var result = _engine.ValidateTranslation(en, it);

        Assert.True(result.IsCompliant);
        Assert.Empty(result.Warnings);
        Assert.Empty(result.ProhibitedUsages);
    }
}

