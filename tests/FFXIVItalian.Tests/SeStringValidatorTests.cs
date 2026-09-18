using FFXIVItalian.Core.SeString;
using Xunit;

namespace FFXIVItalian.Tests;

public class SeStringValidatorTests
{
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
