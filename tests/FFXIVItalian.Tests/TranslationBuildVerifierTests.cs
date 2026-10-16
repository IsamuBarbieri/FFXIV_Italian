using System.Buffers.Binary;
using FFXIVItalian.Core.Packaging;
using FFXIVItalian.Core.SeString;
using FFXIVItalian.Patcher;

namespace FFXIVItalian.Tests;

public class TranslationBuildVerifierTests
{
    [Fact]
    public void FieldVerificationChecksColumnBytesAndTerminatorIncludingPayloads()
    {
        byte[] text = SeStringEncoder.Encode("È pronta<br><hex:02081FE905FF0D617676656E74757269657261FF0D617676656E7475726965726F03>");
        var fixedData = new byte[8];
        BinaryPrimitives.WriteUInt32BigEndian(fixedData.AsSpan(4), 8);
        byte[] page = ExdPatcher.RebuildExdf([
            new ExdRowData { RowId = 42, FixedData = fixedData, StringData = [.. "English"u8, 0, .. text, 0] }
        ]);
        int rowOffset = (int)BinaryPrimitives.ReadUInt32BigEndian(page.AsSpan(36, 4));
        TranslationBuildVerifier.VerifyField(page, rowOffset, 8, 4, text);
        Assert.Throws<InvalidDataException>(() => TranslationBuildVerifier.VerifyField(page, rowOffset, 8, 0, text));
        Assert.Throws<InvalidDataException>(() => TranslationBuildVerifier.VerifyField(page, rowOffset, 8, 4, text[..^1]));
        page[rowOffset + 6 + 8 + 8] ^= 1;
        Assert.Throws<InvalidDataException>(() => TranslationBuildVerifier.VerifyField(page, rowOffset, 8, 4, text));
    }

    [Theory]
    [InlineData("lobby", "translation_description", 8)]
    [InlineData("contentfindercondition", "translation", 4)]
    [InlineData("classjob", "translation_col_4", 4)]
    [InlineData("gcranklimsamaletext", "translation_description", 4)]
    [InlineData("fate", "translation_col_3", 12)]
    public void FieldMappingHonorsSpecialFormatsAndAdditionalColumns(string sheet, string field, int expected)
    {
        Assert.Equal([expected], TranslationBuildVerifier.GetOffsets(sheet, field, [0, 4, 8, 12, 16]));
    }

    [Fact]
    public void RepeatedDisplayNamesAreVerifiedInBothColumns()
    {
        Assert.Equal([0, 16], TranslationBuildVerifier.GetOffsets("classjob", "translation", [0, 4, 8, 16]));
        Assert.Equal([0, 4], TranslationBuildVerifier.GetOffsets("placename", "translation", [0, 4, 8]));
        Assert.Throws<InvalidDataException>(() => TranslationBuildVerifier.GetOffsets("test", "translation_col_3", [0]));
        Assert.Throws<InvalidDataException>(() => TranslationBuildVerifier.GetOffsets("test", "translation_unknown", [0]));
    }
}
