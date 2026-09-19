using FFXIVItalian.Core.SeString;
using System.Text;
using Xunit;

namespace FFXIVItalian.Tests;

public class SeStringEncoderTests
{
    [Fact]
    public void Encode_PlainText_ReturnsUtf8Bytes()
    {
        string text = "Ciao Mondo!";
        var bytes = SeStringEncoder.Encode(text);
        Assert.Equal(Encoding.UTF8.GetBytes(text), bytes);
    }

    [Fact]
    public void Encode_Row25Pattern_ProducesExactNativeSeString()
    {
        // Row 25: "Log in with <string(lstr1)>?"
        string input = "Log in with <string(lstr1)>?";
        var bytes = SeStringEncoder.Encode(input);

        // Native bytes from game exd for Row 25:
        // 4C 6F 67 20 69 6E 20 77 69 74 68 20 02 29 03 EA 02 03 3F
        string expectedHex = "4C6F6720696E207769746820022903EA02033F";
        Assert.Equal(expectedHex, Convert.ToHexString(bytes));
    }

    [Fact]
    public void Encode_ItalianRow25_EmbedsMacroCorrectly()
    {
        string input = "Vuoi accedere con <string(lstr1)>?";
        var bytes = SeStringEncoder.Encode(input);

        // Expected: "Vuoi accedere con " + \x02\x29\x03\xea\x02\x03 + "?"
        byte[] prefix = Encoding.UTF8.GetBytes("Vuoi accedere con ");
        byte[] macro = [0x02, 0x29, 0x03, 0xEA, 0x02, 0x03];
        byte[] suffix = [(byte)'?'];

        byte[] expected = [..prefix, ..macro, ..suffix];
        Assert.Equal(expected, bytes);
    }

    [Fact]
    public void Encode_HexTag_EmitsExactRawBytes()
    {
        var bytes = SeStringEncoder.Encode("<hex:022206E802FF022C03>");
        Assert.Equal("022206E802FF022C03", Convert.ToHexString(bytes));
    }

    [Fact]
    public void Encode_Row629Pattern_ProducesExactNativeSeString()
    {
        // Row 629: Log in with <string(lstr1)>?<br><colortype(506)><edgecolortype(507)>※Cancel and access the subcommand menu to re-edit your character.<edgecolortype(0)><colortype(0)>
        string input = "Log in with <string(lstr1)>?<br><colortype(506)><edgecolortype(507)>※Cancel and access the subcommand menu to re-edit your character.<edgecolortype(0)><colortype(0)>";
        var bytes = SeStringEncoder.Encode(input);

        string expectedHex = "4C6F6720696E207769746820022903EA02033F02100103024804F201FA03024904F201FB03E280BB43616E63656C20616E64206163636573732074686520737562636F6D6D616E64206D656E7520746F2072652D6564697420796F7572206368617261637465722E02490201030248020103";
        Assert.Equal(expectedHex, Convert.ToHexString(bytes));
    }
}

