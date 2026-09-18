using System.Buffers.Binary;
using System.Text;
using FFXIVItalian.Core.Packaging;
using Xunit;

namespace FFXIVItalian.Tests;

public class ExdPatcherTests
{
    [Fact]
    public void PatchSimpleStringSheet_ModifiesSpecifiedRows_AndPreservesOthers()
    {
        // 1. Build a synthetic EXD file with 3 rows
        var rows = new List<ExdRowData>
        {
            new()
            {
                RowId = 1,
                FixedData = new byte[4], // string offset = 0
                StringData = Encoding.UTF8.GetBytes("Cancel\0")
            },
            new()
            {
                RowId = 2,
                FixedData = new byte[4],
                StringData = Encoding.UTF8.GetBytes("Quit\0")
            },
            new()
            {
                RowId = 3,
                FixedData = new byte[4],
                StringData = Encoding.UTF8.GetBytes("Confirm\0")
            }
        };

        byte[] originalExd = ExdPatcher.RebuildExdf(rows);

        Assert.True(originalExd.Length > ExdPatcher.HeaderSize);
        Assert.Equal((byte)'E', originalExd[0]);
        Assert.Equal((byte)'X', originalExd[1]);
        Assert.Equal((byte)'D', originalExd[2]);
        Assert.Equal((byte)'F', originalExd[3]);

        // 2. Patch row 1 ("Cancel" -> "Annulla") and row 2 ("Quit" -> "Esci dal Gioco")
        var replacements = new Dictionary<uint, string>
        {
            [1] = "Annulla",
            [2] = "Esci dal Gioco"
        };

        byte[] patchedExd = ExdPatcher.PatchSimpleStringSheet(originalExd, fixedDataSize: 4, stringColumnOffset: 0, replacements);

        Assert.NotNull(patchedExd);
        Assert.True(patchedExd.Length > ExdPatcher.HeaderSize);

        // 3. Verify index count
        uint indexSize = BinaryPrimitives.ReadUInt32BigEndian(patchedExd.AsSpan(8, 4));
        Assert.Equal(3u * 8u, indexSize);

        // 4. Verify patched row 1 offset and content
        uint row1Offset = BinaryPrimitives.ReadUInt32BigEndian(patchedExd.AsSpan(ExdPatcher.HeaderSize + 4, 4));
        int row1DataSize = (int)BinaryPrimitives.ReadUInt32BigEndian(patchedExd.AsSpan((int)row1Offset, 4));
        int row1StringStart = (int)row1Offset + ExdPatcher.RowHeaderSize + 4;
        string row1Text = Encoding.UTF8.GetString(patchedExd.AsSpan(row1StringStart, "Annulla".Length));
        Assert.Equal("Annulla", row1Text);

        // 5. Verify unpatched row 3 ("Confirm") was preserved
        uint row3Offset = BinaryPrimitives.ReadUInt32BigEndian(patchedExd.AsSpan(ExdPatcher.HeaderSize + (2 * 8) + 4, 4));
        int row3StringStart = (int)row3Offset + ExdPatcher.RowHeaderSize + 4;
        string row3Text = Encoding.UTF8.GetString(patchedExd.AsSpan(row3StringStart, "Confirm".Length));
        Assert.Equal("Confirm", row3Text);
    }

    [Fact]
    public void PatchTwoStringSheet_ModifiesSpecifiedRows_AndUpdatesBothOffsets()
    {
        // 1. Build a synthetic EXD file with 2 rows and fixedDataSize = 16 (like MainCommand)
        byte[] row1Fixed = new byte[16];
        BinaryPrimitives.WriteUInt32BigEndian(row1Fixed.AsSpan(0, 4), 0); // string 1 at offset 0
        BinaryPrimitives.WriteUInt32BigEndian(row1Fixed.AsSpan(4, 4), 10); // "Inventory\0" is 10 bytes, so string 2 is at offset 10

        byte[] row1String = [
            ..Encoding.UTF8.GetBytes("Inventory\0"),
            ..Encoding.UTF8.GetBytes("Manage items\0")
        ];

        byte[] row2Fixed = new byte[16];
        BinaryPrimitives.WriteUInt32BigEndian(row2Fixed.AsSpan(0, 4), 0);
        BinaryPrimitives.WriteUInt32BigEndian(row2Fixed.AsSpan(4, 4), 8); // "Journal\0" is 8 bytes

        byte[] row2String = [
            ..Encoding.UTF8.GetBytes("Journal\0"),
            ..Encoding.UTF8.GetBytes("View quests\0")
        ];

        var rows = new List<ExdRowData>
        {
            new() { RowId = 1, FixedData = row1Fixed, StringData = row1String },
            new() { RowId = 2, FixedData = row2Fixed, StringData = row2String }
        };

        byte[] originalExd = ExdPatcher.RebuildExdf(rows);

        // 2. Patch Row 1 ("Inventario", "Gestisci il tuo inventario.")
        var replacements = new Dictionary<uint, (string? String1, string? String2)>
        {
            [1] = ("Inventario", "Gestisci il tuo inventario.")
        };

        byte[] patchedExd = ExdPatcher.PatchTwoStringSheet(
            originalExd,
            fixedDataSize: 16,
            string1ColumnOffset: 0,
            string2ColumnOffset: 4,
            replacements);

        Assert.NotNull(patchedExd);

        // 3. Verify Row 1 fixed data offsets
        uint row1Offset = BinaryPrimitives.ReadUInt32BigEndian(patchedExd.AsSpan(ExdPatcher.HeaderSize + 4, 4));
        int row1FixedStart = (int)row1Offset + ExdPatcher.RowHeaderSize;
        uint s1Offset = BinaryPrimitives.ReadUInt32BigEndian(patchedExd.AsSpan(row1FixedStart, 4));
        uint s2Offset = BinaryPrimitives.ReadUInt32BigEndian(patchedExd.AsSpan(row1FixedStart + 4, 4));

        Assert.Equal(0u, s1Offset);
        Assert.Equal((uint)("Inventario".Length + 1), s2Offset);

        // 4. Verify Row 1 string content
        int row1StringStart = row1FixedStart + 16;
        string s1Text = Encoding.UTF8.GetString(patchedExd.AsSpan(row1StringStart + (int)s1Offset, "Inventario".Length));
        string s2Text = Encoding.UTF8.GetString(patchedExd.AsSpan(row1StringStart + (int)s2Offset, "Gestisci il tuo inventario.".Length));

        Assert.Equal("Inventario", s1Text);
        Assert.Equal("Gestisci il tuo inventario.", s2Text);

        // 5. Verify Row 2 was preserved
        uint row2Offset = BinaryPrimitives.ReadUInt32BigEndian(patchedExd.AsSpan(ExdPatcher.HeaderSize + 8 + 4, 4));
        int row2FixedStart = (int)row2Offset + ExdPatcher.RowHeaderSize;
        int row2StringStart = row2FixedStart + 16;
        string r2s1 = Encoding.UTF8.GetString(patchedExd.AsSpan(row2StringStart, "Journal".Length));
        Assert.Equal("Journal", r2s1);
    }
}

