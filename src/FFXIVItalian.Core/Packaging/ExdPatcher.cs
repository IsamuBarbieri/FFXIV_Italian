using System.Buffers.Binary;
using System.Text;

namespace FFXIVItalian.Core.Packaging;

public class ExdRowData
{
    public uint RowId { get; set; }
    public ushort SubRowCount { get; set; } = 1;
    public byte[] FixedData { get; set; } = [];
    public byte[] StringData { get; set; } = [];
}

public static class ExdPatcher
{
    public const int HeaderSize = 0x20;
    public const int RowHeaderSize = 6;
    public const int RowAlignment = 4;

    /// <summary>
    /// Replaces strings in an EXD page where fixedDataSize and the string column offset are known.
    /// Updates the string column offsets, row data sizes, and index table offsets.
    /// </summary>
    public static byte[] PatchSimpleStringSheet(
        byte[] originalExd,
        int fixedDataSize,
        int stringColumnOffset,
        IReadOnlyDictionary<uint, string> rowReplacements)
    {
        if (originalExd.Length < HeaderSize ||
            originalExd[0] != 'E' || originalExd[1] != 'X' || originalExd[2] != 'D' || originalExd[3] != 'F')
        {
            throw new InvalidDataException("I byte forniti non appartengono a un file EXDF valido.");
        }

        uint indexTableSize = BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan(0x08, 4));
        int rowCount = (int)(indexTableSize / 8);

        var rows = new List<ExdRowData>(rowCount);

        for (int i = 0; i < rowCount; i++)
        {
            int entryPos = HeaderSize + (i * 8);
            uint rowId = BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan(entryPos, 4));
            uint offset = BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan(entryPos + 4, 4));

            if (offset + RowHeaderSize > originalExd.Length)
            {
                continue;
            }

            int dataSize = (int)BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan((int)offset, 4));
            ushort subRowCount = BinaryPrimitives.ReadUInt16BigEndian(originalExd.AsSpan((int)offset + 4, 2));

            int fixedStart = (int)offset + RowHeaderSize;
            int stringStart = fixedStart + fixedDataSize;
            int stringLength = dataSize - fixedDataSize;

            if (stringStart > originalExd.Length || fixedStart + fixedDataSize > originalExd.Length)
            {
                continue;
            }

            byte[] fixedData = originalExd.AsSpan(fixedStart, fixedDataSize).ToArray();
            byte[] stringData;

            if (rowReplacements.TryGetValue(rowId, out var replacementText))
            {
                // Replace with translated UTF-8 text + null terminator
                var utf8 = Encoding.UTF8.GetBytes(replacementText);
                stringData = new byte[utf8.Length + 1];
                Buffer.BlockCopy(utf8, 0, stringData, 0, utf8.Length);
                stringData[^1] = 0; // null terminator

                // Update string offset in fixed data to 0
                if (stringColumnOffset >= 0 && stringColumnOffset + 4 <= fixedData.Length)
                {
                    BinaryPrimitives.WriteUInt32BigEndian(fixedData.AsSpan(stringColumnOffset, 4), 0);
                }
            }
            else
            {
                stringData = originalExd.AsSpan(stringStart, Math.Max(0, stringLength)).ToArray();
            }

            rows.Add(new ExdRowData
            {
                RowId = rowId,
                SubRowCount = subRowCount,
                FixedData = fixedData,
                StringData = stringData
            });
        }

        // Rebuild the EXDF binary file
        return RebuildExdf(rows);
    }

    public static byte[] RebuildExdf(List<ExdRowData> rows)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        int rowCount = rows.Count;
        uint indexSize = (uint)(rowCount * 8);

        // 1. Write Header placeholder
        byte[] header = new byte[HeaderSize];
        header[0] = (byte)'E';
        header[1] = (byte)'X';
        header[2] = (byte)'D';
        header[3] = (byte)'F';
        BinaryPrimitives.WriteUInt16BigEndian(header.AsSpan(4, 2), 2); // Version 2
        BinaryPrimitives.WriteUInt32BigEndian(header.AsSpan(8, 4), indexSize);
        writer.Write(header);

        // 2. Reserve space for index table
        long indexStartPos = ms.Position;
        byte[] indexPlaceholder = new byte[indexSize];
        writer.Write(indexPlaceholder);

        // 3. Write Row Data
        var indexEntries = new (uint RowId, uint Offset)[rowCount];
        long dataStartPos = ms.Position;

        for (int i = 0; i < rowCount; i++)
        {
            var row = rows[i];
            long currentOffset = ms.Position;
            indexEntries[i] = (row.RowId, (uint)currentOffset);

            int dataSize = row.FixedData.Length + row.StringData.Length;

            // Row header (6 bytes)
            byte[] rowHeader = new byte[RowHeaderSize];
            BinaryPrimitives.WriteUInt32BigEndian(rowHeader.AsSpan(0, 4), (uint)dataSize);
            BinaryPrimitives.WriteUInt16BigEndian(rowHeader.AsSpan(4, 2), row.SubRowCount);
            writer.Write(rowHeader);

            // Fixed data + String data
            writer.Write(row.FixedData);
            writer.Write(row.StringData);

            // Alignment to 4 bytes
            int padding = (RowAlignment - (int)(ms.Position % RowAlignment)) % RowAlignment;
            for (int p = 0; p < padding; p++)
            {
                writer.Write((byte)0);
            }
        }

        long dataEndPos = ms.Position;
        uint dataSectionSize = (uint)(dataEndPos - dataStartPos);

        // 4. Go back and write index table
        ms.Seek(indexStartPos, SeekOrigin.Begin);
        foreach (var (rowId, offset) in indexEntries)
        {
            byte[] entryBytes = new byte[8];
            BinaryPrimitives.WriteUInt32BigEndian(entryBytes.AsSpan(0, 4), rowId);
            BinaryPrimitives.WriteUInt32BigEndian(entryBytes.AsSpan(4, 4), offset);
            writer.Write(entryBytes);
        }

        // 5. Go back and write data section size in header (offset 0x0C)
        ms.Seek(0x0C, SeekOrigin.Begin);
        byte[] dataSizeField = new byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(dataSizeField, dataSectionSize);
        writer.Write(dataSizeField);

        return ms.ToArray();
    }
}

