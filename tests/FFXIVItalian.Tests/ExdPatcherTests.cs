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

        byte[] patchedExd = ExdPatcher.PatchSimpleStringSheet(originalExd, fixedDataSize: 4, replacements);

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
}
