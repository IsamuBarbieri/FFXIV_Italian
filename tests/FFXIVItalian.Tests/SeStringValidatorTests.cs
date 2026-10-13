using FFXIVItalian.Core.SeString;
using Xunit;

namespace FFXIVItalian.Tests;

public class SeStringValidatorTests
{
    [Theory]
    [InlineData("One<hex:021F0103>handed Thaumaturge's Arm", "Arma da Taumaturgo a una mano")]
    [InlineData("Two<hex:021F0103>handed Thaumaturge's Arm", "Arma da Taumaturgo a due mani")]
    [InlineData("One<hex:021F0103>handed Conjurer's Arm", "Arma da Incantatore a una mano")]
    [InlineData("Two<hex:021F0103>handed Conjurer's Arm", "Arma da Incantatore a due mani")]
    [InlineData("city<hex:021F0103>state", "città<hex:021F0103>stato")]
    public void Validate_ContextualHyphenAdaptation_Passes(string en, string it)
    {
        Assert.True(SeStringValidator.Validate(en, it).IsValid);
    }

    [Theory]
    [InlineData("Arma da Taumaturgo a una<hex:021F0103> mano")]
    [InlineData("Arma da Incantatore a due<hex:021F0103> mani")]
    [InlineData("Arma a Una <hex:021f0103> Mano")]
    public void Validate_HyphenBetweenNumeralAndHand_Fails(string it)
    {
        var result = SeStringValidator.Validate("One<hex:021F0103>handed Conjurer's Arm", it);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Trattino SeString improprio"));
    }

    [Fact]
    public void Validate_ValidStringWithBalancedTags_Passes()
    {
        string en = "Speak with <Highlight>Baderon</Highlight> in Limsa Lominsa.";
        string it = "Parla con <Highlight>Baderon</Highlight> a Limsa Lominsa.";

        var result = SeStringValidator.Validate(en, it);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_UnbalancedAngleBrackets_Fails()
    {
        string en = "Welcome, traveler.";
        string it = "Benvenuto, <Highlightviandante.";

        var result = SeStringValidator.Validate(en, it);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Parentesi angolari sbilanciate"));
    }

    [Fact]
    public void Validate_UnbalancedPairedTag_Fails()
    {
        string en = "<Highlight>Important Notice</Highlight>";
        string it = "<Highlight>Avviso Importante";

        var result = SeStringValidator.Validate(en, it);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Highlight") && e.Contains("sbilanciato"));
    }

    [Fact]
    public void Validate_MissingEssentialPlaceholder_Fails()
    {
        string en = "Greetings, <FullName>. How fares your journey?";
        string it = "Saluti, viaggiatore. Come procede il tuo viaggio?";

        var result = SeStringValidator.Validate(en, it);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("<FullName>"));
    }
}

