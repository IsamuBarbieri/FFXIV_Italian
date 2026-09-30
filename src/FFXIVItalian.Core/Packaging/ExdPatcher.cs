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

        var rows = new ExdRowData?[rowCount];

        void PatchRow(int i)
        {
            int entryPos = HeaderSize + (i * 8);
            uint rowId = BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan(entryPos, 4));
            uint offset = BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan(entryPos + 4, 4));

            if (offset + RowHeaderSize > originalExd.Length)
            {
                return;
            }

            int dataSize = (int)BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan((int)offset, 4));
            ushort subRowCount = BinaryPrimitives.ReadUInt16BigEndian(originalExd.AsSpan((int)offset + 4, 2));

            int fixedStart = (int)offset + RowHeaderSize;
            int stringStart = fixedStart + fixedDataSize;
            int stringLength = dataSize - fixedDataSize;

            if (stringStart > originalExd.Length || fixedStart + fixedDataSize > originalExd.Length)
            {
                return;
            }

            byte[] fixedData = originalExd.AsSpan(fixedStart, fixedDataSize).ToArray();
            byte[] stringData;

            if (rowReplacements.TryGetValue(rowId, out var replacementText))
            {
                // Replace with translated UTF-8 / SeString text + null terminator
                var encoded = SeString.SeStringEncoder.Encode(replacementText);
                stringData = new byte[encoded.Length + 1];
                Buffer.BlockCopy(encoded, 0, stringData, 0, encoded.Length);
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

            rows[i] = new ExdRowData
            {
                RowId = rowId,
                SubRowCount = subRowCount,
                FixedData = fixedData,
                StringData = stringData
            };
        }

        if (rowCount >= 1024 && Environment.ProcessorCount > 1)
            Parallel.For(0, rowCount, PatchRow);
        else
            for (int i = 0; i < rowCount; i++) PatchRow(i);

        var orderedRows = new List<ExdRowData>(rowCount);
        foreach (var row in rows)
            if (row is not null) orderedRows.Add(row);

        // Rebuild the EXDF binary file
        return RebuildExdf(orderedRows);
    }

    /// <summary>
    /// Replaces strings in an EXD page where fixedDataSize and two string column offsets are known (e.g. MainCommand).
    /// </summary>
    public static byte[] PatchTwoStringSheet(
        byte[] originalExd,
        int fixedDataSize,
        int string1ColumnOffset,
        int string2ColumnOffset,
        IReadOnlyDictionary<uint, (string? String1, string? String2)> rowReplacements)
    {
        if (originalExd.Length < HeaderSize ||
            originalExd[0] != 'E' || originalExd[1] != 'X' || originalExd[2] != 'D' || originalExd[3] != 'F')
        {
            throw new InvalidDataException("I byte forniti non appartengono a un file EXDF valido.");
        }

        uint indexTableSize = BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan(0x08, 4));
        int rowCount = (int)(indexTableSize / 8);

        var rows = new ExdRowData?[rowCount];

        void PatchRow(int i)
        {
            int entryPos = HeaderSize + (i * 8);
            uint rowId = BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan(entryPos, 4));
            uint offset = BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan(entryPos + 4, 4));

            if (offset + RowHeaderSize > originalExd.Length)
            {
                return;
            }

            int dataSize = (int)BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan((int)offset, 4));
            ushort subRowCount = BinaryPrimitives.ReadUInt16BigEndian(originalExd.AsSpan((int)offset + 4, 2));

            int fixedStart = (int)offset + RowHeaderSize;
            int stringStart = fixedStart + fixedDataSize;
            int stringLength = dataSize - fixedDataSize;

            if (stringStart > originalExd.Length || fixedStart + fixedDataSize > originalExd.Length)
            {
                return;
            }

            byte[] fixedData = originalExd.AsSpan(fixedStart, fixedDataSize).ToArray();
            byte[] stringData;

            if (rowReplacements.TryGetValue(rowId, out var replacement))
            {
                uint origS1Offset = BinaryPrimitives.ReadUInt32BigEndian(fixedData.AsSpan(string1ColumnOffset, 4));
                uint origS2Offset = BinaryPrimitives.ReadUInt32BigEndian(fixedData.AsSpan(string2ColumnOffset, 4));

                string s1Text = replacement.String1 ?? ReadNullTerminatedString(originalExd, stringStart, origS1Offset, stringLength);
                string s2Text = replacement.String2 ?? ReadNullTerminatedString(originalExd, stringStart, origS2Offset, stringLength);

                byte[] s1Bytes = SeString.SeStringEncoder.Encode(s1Text);
                byte[] s2Bytes = SeString.SeStringEncoder.Encode(s2Text);

                // String payload: [s1, 0, s2, 0]
                stringData = new byte[s1Bytes.Length + 1 + s2Bytes.Length + 1];
                Buffer.BlockCopy(s1Bytes, 0, stringData, 0, s1Bytes.Length);
                stringData[s1Bytes.Length] = 0;

                int s2RelOffset = s1Bytes.Length + 1;
                Buffer.BlockCopy(s2Bytes, 0, stringData, s2RelOffset, s2Bytes.Length);
                stringData[^1] = 0;

                // Update fixed data offsets
                BinaryPrimitives.WriteUInt32BigEndian(fixedData.AsSpan(string1ColumnOffset, 4), 0);
                BinaryPrimitives.WriteUInt32BigEndian(fixedData.AsSpan(string2ColumnOffset, 4), (uint)s2RelOffset);
            }
            else
            {
                stringData = originalExd.AsSpan(stringStart, Math.Max(0, stringLength)).ToArray();
            }

            rows[i] = new ExdRowData
            {
                RowId = rowId,
                SubRowCount = subRowCount,
                FixedData = fixedData,
                StringData = stringData
            };
        }

        if (rowCount >= 1024 && Environment.ProcessorCount > 1)
            Parallel.For(0, rowCount, PatchRow);
        else
            for (int i = 0; i < rowCount; i++) PatchRow(i);

        var orderedRows = new List<ExdRowData>(rowCount);
        foreach (var row in rows)
            if (row is not null) orderedRows.Add(row);

        return RebuildExdf(orderedRows);
    }

    /// <summary>
    /// Replaces strings across multiple specified column offsets in an EXD page, preserving unreplaced strings.
    /// </summary>
    public static byte[] PatchMultiColumnStringSheet(
        byte[] originalExd,
        int fixedDataSize,
        IReadOnlyList<int> stringColumnOffsets,
        IReadOnlyDictionary<uint, IReadOnlyDictionary<int, string>> rowReplacements)
    {
        if (originalExd.Length < HeaderSize ||
            originalExd[0] != 'E' || originalExd[1] != 'X' || originalExd[2] != 'D' || originalExd[3] != 'F')
        {
            throw new InvalidDataException("I byte forniti non appartengono a un file EXDF valido.");
        }

        uint indexTableSize = BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan(0x08, 4));
        int rowCount = (int)(indexTableSize / 8);

        var rows = new ExdRowData?[rowCount];

        void PatchRow(int i)
        {
            int entryPos = HeaderSize + (i * 8);
            uint rowId = BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan(entryPos, 4));
            uint offset = BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan(entryPos + 4, 4));

            if (offset + RowHeaderSize > originalExd.Length)
            {
                return;
            }

            int dataSize = (int)BinaryPrimitives.ReadUInt32BigEndian(originalExd.AsSpan((int)offset, 4));
            ushort subRowCount = BinaryPrimitives.ReadUInt16BigEndian(originalExd.AsSpan((int)offset + 4, 2));

            int fixedStart = (int)offset + RowHeaderSize;
            int stringStart = fixedStart + fixedDataSize;
            int stringLength = dataSize - fixedDataSize;

            if (stringStart > originalExd.Length || fixedStart + fixedDataSize > originalExd.Length)
            {
                return;
            }

            byte[] fixedData = originalExd.AsSpan(fixedStart, fixedDataSize).ToArray();
            byte[] stringData;

            if (rowReplacements.TryGetValue(rowId, out var colReplacements))
            {
                using var stringStream = new MemoryStream();
                foreach (var colOffset in stringColumnOffsets)
                {
                    uint origRelOffset = 0;
                    if (colOffset + 4 <= fixedData.Length)
                    {
                        origRelOffset = BinaryPrimitives.ReadUInt32BigEndian(fixedData.AsSpan(colOffset, 4));
                    }

                    byte[] stringBytes;
                    if (colReplacements.TryGetValue(colOffset, out var repText))
                    {
                        stringBytes = SeString.SeStringEncoder.Encode(repText);
                    }
                    else
                    {
                        stringBytes = ReadNullTerminatedBytes(originalExd, stringStart, origRelOffset, stringLength);
                    }

                    uint newRelOffset = (uint)stringStream.Position;
                    if (colOffset + 4 <= fixedData.Length)
                    {
                        BinaryPrimitives.WriteUInt32BigEndian(fixedData.AsSpan(colOffset, 4), newRelOffset);
                    }

                    stringStream.Write(stringBytes);
                    stringStream.WriteByte(0); // null-terminator
                }
                stringData = stringStream.ToArray();
            }
            else
            {
                stringData = originalExd.AsSpan(stringStart, Math.Max(0, stringLength)).ToArray();
            }

            rows[i] = new ExdRowData
            {
                RowId = rowId,
                SubRowCount = subRowCount,
                FixedData = fixedData,
                StringData = stringData
            };
        }

        if (rowCount >= 1024 && Environment.ProcessorCount > 1)
            Parallel.For(0, rowCount, PatchRow);
        else
            for (int i = 0; i < rowCount; i++) PatchRow(i);

        var orderedRows = new List<ExdRowData>(rowCount);
        foreach (var row in rows)
            if (row is not null) orderedRows.Add(row);

        return RebuildExdf(orderedRows);
    }

    private static byte[] ReadNullTerminatedBytes(byte[] data, int baseOffset, uint relativeOffset, int maxLength)
    {
        int start = baseOffset + (int)relativeOffset;
        if (start >= data.Length || relativeOffset >= maxLength)
            return [];

        int end = start;
        while (end < data.Length && end < baseOffset + maxLength && data[end] != 0)
        {
            end++;
        }

        return data.AsSpan(start, end - start).ToArray();
    }

    private static string ReadNullTerminatedString(byte[] data, int baseOffset, uint relativeOffset, int maxLength)
    {
        int start = baseOffset + (int)relativeOffset;
        if (start >= data.Length || relativeOffset >= maxLength)
            return string.Empty;

        int end = start;
        while (end < data.Length && end < baseOffset + maxLength && data[end] != 0)
        {
            end++;
        }

        return Encoding.UTF8.GetString(data, start, end - start);
    }

    public static byte[] RebuildExdf(List<ExdRowData> rows)
    {
        int rowCount = rows.Count;
        uint indexSize = (uint)(rowCount * 8);
        long dataStart = HeaderSize + (long)indexSize;
        long totalLength = dataStart;
        foreach (var row in rows)
        {
            totalLength += RowHeaderSize + (long)row.FixedData.Length + row.StringData.Length;
            totalLength = (totalLength + RowAlignment - 1) & ~(RowAlignment - 1L);
        }

        if (totalLength > int.MaxValue)
            throw new InvalidDataException("La pagina EXDF ricostruita supera la dimensione massima supportata.");

        var output = new byte[(int)totalLength];
        output[0] = (byte)'E';
        output[1] = (byte)'X';
        output[2] = (byte)'D';
        output[3] = (byte)'F';
        BinaryPrimitives.WriteUInt16BigEndian(output.AsSpan(4, 2), 2);
        BinaryPrimitives.WriteUInt32BigEndian(output.AsSpan(8, 4), indexSize);

        int rowDataOffset = (int)dataStart;
        for (int i = 0; i < rowCount; i++)
        {
            var row = rows[i];
            int indexOffset = HeaderSize + i * 8;
            BinaryPrimitives.WriteUInt32BigEndian(output.AsSpan(indexOffset, 4), row.RowId);
            BinaryPrimitives.WriteUInt32BigEndian(output.AsSpan(indexOffset + 4, 4), (uint)rowDataOffset);

            int dataSize = row.FixedData.Length + row.StringData.Length;
            BinaryPrimitives.WriteUInt32BigEndian(output.AsSpan(rowDataOffset, 4), (uint)dataSize);
            BinaryPrimitives.WriteUInt16BigEndian(output.AsSpan(rowDataOffset + 4, 2), row.SubRowCount);
            row.FixedData.AsSpan().CopyTo(output.AsSpan(rowDataOffset + RowHeaderSize));
            row.StringData.AsSpan().CopyTo(output.AsSpan(rowDataOffset + RowHeaderSize + row.FixedData.Length));

            rowDataOffset = (rowDataOffset + RowHeaderSize + dataSize + RowAlignment - 1) & ~(RowAlignment - 1);
        }

        BinaryPrimitives.WriteUInt32BigEndian(output.AsSpan(0x0C, 4), (uint)(rowDataOffset - dataStart));
        return output;
    }
}

