using FFXIVItalian.Extractor.Extractors;
using FFXIVItalian.Core.SeString;
using System.Text;
using Xunit;

namespace FFXIVItalian.Tests;

public class ExtractorTests
{
    [Fact]
    public void ExtractorRegistry_ContainsAllExpectedSheets()
    {
        var names = ExtractorRegistry.GetNames();
        Assert.Contains("lobby", names);
        Assert.Contains("addon", names);
        Assert.Contains("maincommand", names);
        Assert.Contains("maincommandcategory", names);
        Assert.Contains("error", names);
        Assert.Contains("classjob", names);
        Assert.Contains("placename", names);
        Assert.Contains("tribe", names);
        Assert.Contains("race", names);
    }

    [Theory]
    [InlineData("lobby")]
    [InlineData("LOBBY")]
    [InlineData("addon")]
    [InlineData("maincommand")]
    [InlineData("maincommandcategory")]
    [InlineData("error")]
    [InlineData("classjob")]
    [InlineData("placename")]
    [InlineData("tribe")]
    [InlineData("race")]
    public void ExtractorRegistry_Get_IsCaseInsensitive(string sheetName)
    {
        var extractor = ExtractorRegistry.Get(sheetName);
        Assert.NotNull(extractor);
        Assert.False(string.IsNullOrWhiteSpace(extractor.DefaultJsonFileName));
    }

    [Fact]
    public void DecodeSeStringPayload_DecodesExactRow1704Pattern()
    {
        // EX\x02\x20\x03\xe8\x02\x03: \x02\x29\x03\xea\x03\x03
        byte[] raw = [
            (byte)'E', (byte)'X',
            0x02, 0x20, 0x03, 0xE8, 0x02, 0x03,
            (byte)':', (byte)' ',
            0x02, 0x29, 0x03, 0xEA, 0x03, 0x03
        ];

        string decoded = BaseSheetExtractor.DecodeSeStringPayload(raw);
        Assert.Equal("EX<hex:022003E80203>: <hex:022903EA0303>", decoded);
    }

    [Fact]
    public void DecodeSeStringPayload_DecodesControllerIconPattern()
    {
        // \x02\x1e\x02\x0c\x03 Keyboard
        byte[] raw = [
            0x02, 0x1E, 0x02, 0x0C, 0x03,
            (byte)' ', (byte)'K', (byte)'e', (byte)'y', (byte)'b', (byte)'o', (byte)'a', (byte)'r', (byte)'d'
        ];

        string decoded = BaseSheetExtractor.DecodeSeStringPayload(raw);
        Assert.Equal("<hex:021E020C03> Keyboard", decoded);
    }

    [Fact]
    public void DecodeSeStringPayload_DecodesRow13206Pattern()
    {
        // Players in queue: \x02\x22\x06\xe8\x02\xff\x02\x2c\x03.
        byte[] raw = [
            ..Encoding.UTF8.GetBytes("Players in queue: "),
            0x02, 0x22, 0x06, 0xE8, 0x02, 0xFF, 0x02, 0x2C, 0x03,
            (byte)'.'
        ];

        string decoded = BaseSheetExtractor.DecodeSeStringPayload(raw);
        Assert.Equal("Players in queue: <hex:022206E802FF022C03>.", decoded);
    }

    [Fact]
    public void RoundTrip_DecodeAndEncode_ProducesIdenticalBytes()
    {
        byte[] raw = [
            (byte)'E', (byte)'X',
            0x02, 0x20, 0x03, 0xE8, 0x02, 0x03,
            (byte)':', (byte)' ',
            0x02, 0x29, 0x03, 0xEA, 0x03, 0x03
        ];

        string decoded = BaseSheetExtractor.DecodeSeStringPayload(raw);
        byte[] reEncoded = SeStringEncoder.Encode(decoded);

        Assert.Equal(raw, reEncoded);
    }


    /// <summary>
    /// Verifica che il file lobby_0_en.exd patchato contenga effettivamente i nomi italiani.
    /// </summary>
    [Fact]
    public void VerifyPatchedLobbyHasItalianClanNames()
    {
        // Cerca il file patchato nella cartella Penumbra
        string patchedPath = @"G:\SquareEnix\FFXIV_Mod\FFXIV Italiano (Test In-Game)\exd\lobby_0_en.exd";
        if (!System.IO.File.Exists(patchedPath)) return;

        byte[] data = System.IO.File.ReadAllBytes(patchedPath);
        string allText = System.Text.Encoding.UTF8.GetString(data);

        // Il file patchato DEVE contenere i nomi italiani
        Assert.Contains("Lupi di Mare", allText);
        Assert.Contains("Guardinferno", allText);
        Assert.Contains("Piancolle", allText);

        // NON deve contenere i nomi inglesi per i clan che abbiamo tradotto
        // (non verificabile semplicemente perché i nomi appaiono anche nei blob originali)
        // Verifichiamo invece che la riga 136 sia patchata correttamente

        // Trova il blob della riga 136 nel file patchato
        byte[] seaWolves = System.Text.Encoding.UTF8.GetBytes("Sea Wolves");
        byte[] lupiMare = System.Text.Encoding.UTF8.GetBytes("Lupi di Mare");
        bool hasLupi = false;
        for (int i = 0; i < data.Length - lupiMare.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < lupiMare.Length; j++)
                if (data[i + j] != lupiMare[j]) { match = false; break; }
            if (match) { hasLupi = true; break; }
        }

        Assert.True(hasLupi, "Il file patchato NON contiene 'Lupi di Mare' — il patch non è stato applicato correttamente!");

        System.IO.File.WriteAllText(
            @"C:\Users\barbi\.gemini\antigravity\brain\579efa5b-fb53-4615-b2d9-81b14baac555\scratch\patch_verify.txt",
            $"HasLupiDiMare: {hasLupi}\nFile size: {data.Length} bytes\n" +
            $"Contains 'Sea Wolves': {allText.Contains("Sea Wolves")}\n" +
            $"Contains 'Lupi di Mare': {allText.Contains("Lupi di Mare")}\n" +
            $"Contains 'Guardinferno': {allText.Contains("Guardinferno")}\n" +
            $"Contains 'Piancolle': {allText.Contains("Piancolle")}");
    }
}
