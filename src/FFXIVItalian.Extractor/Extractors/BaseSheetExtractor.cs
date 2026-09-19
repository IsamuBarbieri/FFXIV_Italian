using Lumina;
using Lumina.Data;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FFXIVItalian.Extractor.Extractors;

public abstract class BaseSheetExtractor : ISheetExtractor
{
    public abstract string SheetName { get; }
    public abstract string DefaultJsonFileName { get; }
    public abstract string Description { get; }

    public abstract int ExtractAndSave(GameData lumina, string targetJsonPath);

    public virtual void Inspect(GameData lumina, uint? targetRowId = null)
    {
        Console.WriteLine($"==================================================");
        Console.WriteLine($" ISPEZIONE FOGLIO: {SheetName}");
        Console.WriteLine($"==================================================");

        var exh = lumina.GetFile($"exd/{SheetName.ToLowerInvariant()}.exh");
        if (exh == null)
        {
            Console.WriteLine($"Errore: exd/{SheetName.ToLowerInvariant()}.exh non trovato!");
            return;
        }

        ushort fixedSize = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(0x06, 2));
        ushort colCount = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(0x08, 2));
        ushort pageCount = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(0x0A, 2));
        ushort langCount = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(0x0C, 2));

        Console.WriteLine($"EXH: FixedDataSize = {fixedSize}, Colonne = {colCount}, Pagine = {pageCount}, Lingue = {langCount}");

        for (int c = 0; c < colCount; c++)
        {
            int colPos = 0x20 + (c * 4);
            ushort type = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(colPos, 2));
            ushort offset = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(colPos + 2, 2));
            string typeName = type switch
            {
                0 => "String",
                1 => "Bool",
                2 => "Int8",
                3 => "UInt8",
                4 => "Int16",
                5 => "UInt16",
                6 => "Int32",
                7 => "UInt32",
                8 => "Float",
                9 => "Int64",
                10 => "UInt64",
                _ => $"Type_{type}"
            };
            Console.WriteLine($"  * Colonna {c,2}: Tipo = 0x{type:X4} ({typeName,-6}), Offset = {offset}");
        }

        var exd = lumina.GetFile($"exd/{SheetName.ToLowerInvariant()}_0_en.exd");
        if (exd == null)
        {
            Console.WriteLine($"Errore: exd/{SheetName.ToLowerInvariant()}_0_en.exd non trovato!");
            return;
        }

        uint indexSize = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(0x08, 4));
        int rowCount = (int)(indexSize / 8);
        Console.WriteLine($"EXD: Righe indicizzate = {rowCount}, Dimensione file = {exd.Data.Length:N0} byte");

        if (targetRowId.HasValue)
        {
            Console.WriteLine();
            Console.WriteLine($"--- Dettaglio Riga {targetRowId.Value} ---");
            bool found = false;
            for (int i = 0; i < rowCount; i++)
            {
                int entryPos = 0x20 + (i * 8);
                uint rId = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(entryPos, 4));
                uint off = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(entryPos + 4, 4));
                if (rId == targetRowId.Value)
                {
                    found = true;
                    int dataSize = (int)BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan((int)off, 4));
                    ushort subRowCount = BinaryPrimitives.ReadUInt16BigEndian(exd.Data.AsSpan((int)off + 4, 2));
                    Console.WriteLine($"Offset = 0x{off:X6} ({off}), DataSize = {dataSize}, SubRows = {subRowCount}");
                    
                    byte[] rowBytes = exd.Data.AsSpan((int)off, Math.Min(exd.Data.Length - (int)off, dataSize + 6)).ToArray();
                    Console.WriteLine("HEX:\n" + Convert.ToHexString(rowBytes));
                    
                    int stringStart = (int)off + 6 + fixedSize;
                    if (stringStart < exd.Data.Length)
                    {
                        int strLen = Math.Min(dataSize - fixedSize, exd.Data.Length - stringStart);
                        if (strLen > 0)
                        {
                            var strSpan = exd.Data.AsSpan(stringStart, strLen);
                            Console.WriteLine("String Payload:\n" + DecodeSeStringPayload(strSpan));
                        }
                    }
                    break;
                }
            }

            if (!found)
            {
                Console.WriteLine($"Riga {targetRowId.Value} non trovata nel foglio.");
            }
        }
    }

    /// <summary>
    /// Reads a string from an EXD row at fixedStart + colOffset.
    /// Preserves binary control sequences using &lt;hex:...&gt; syntax.
    /// </summary>
    protected static string ReadRowString(byte[] exdData, int fixedStart, int fixedSize, int colOffset, int dataSize)
    {
        uint relOffset = BinaryPrimitives.ReadUInt32BigEndian(exdData.AsSpan(fixedStart + colOffset, 4));
        int strStart = fixedStart + fixedSize + (int)relOffset;
        if (strStart >= exdData.Length) return string.Empty;

        int strEnd = strStart;
        int maxLen = fixedStart + fixedSize + (dataSize - fixedSize);
        while (strEnd < maxLen && strEnd < exdData.Length && exdData[strEnd] != 0)
        {
            strEnd++;
        }

        return DecodeSeStringPayload(exdData.AsSpan(strStart, strEnd - strStart));
    }

    /// <summary>
    /// Decodes SeString bytes into text with &lt;hex:...&gt; representation for opcodes,
    /// ensuring that macro bytecode is never corrupted into Unicode text.
    /// </summary>
    public static string DecodeSeStringPayload(ReadOnlySpan<byte> bytes)
    {
        var sb = new StringBuilder();
        int i = 0;
        while (i < bytes.Length)
        {
            byte b = bytes[i];
            if (b == 0x02) // Control tag start
            {
                int start = i;
                int remaining = bytes.Length - start;
                if (remaining >= 3)
                {
                    byte opcode = bytes[start + 1];
                    int lenMarkerOffset = start + 2;
                    int payloadLen = ReadEncodedLength(bytes.Slice(lenMarkerOffset), out int markerBytesRead);
                    int totalTagLen = 1 + 1 + markerBytesRead + payloadLen + 1; // 0x02 + opcode + marker + payload + 0x03

                    if (payloadLen >= 0 && totalTagLen >= 3 && totalTagLen <= remaining && bytes[start + totalTagLen - 1] == 0x03)
                    {
                        var tagBytes = bytes.Slice(start, totalTagLen);
                        sb.Append($"<hex:{Convert.ToHexString(tagBytes)}>");
                        i = start + totalTagLen;
                        continue;
                    }
                }

                // Fallback scan if length didn't match
                int tagEnd = start + 1;
                while (tagEnd < bytes.Length && bytes[tagEnd] != 0x03)
                {
                    tagEnd++;
                }
                if (tagEnd < bytes.Length && bytes[tagEnd] == 0x03)
                {
                    tagEnd++;
                }
                sb.Append($"<hex:{Convert.ToHexString(bytes.Slice(start, tagEnd - start))}>");
                i = tagEnd;
                continue;
            }

            // Normal UTF-8 run
            int textStart = i;
            while (i < bytes.Length && bytes[i] != 0x02)
            {
                i++;
            }
            sb.Append(Encoding.UTF8.GetString(bytes.Slice(textStart, i - textStart)));
        }

        return sb.ToString();
    }

    private static int ReadEncodedLength(ReadOnlySpan<byte> span, out int markerBytesRead)
    {
        if (span.IsEmpty)
        {
            markerBytesRead = 0;
            return 0;
        }

        byte b = span[0];
        if (b < 0xD0)
        {
            markerBytesRead = 1;
            return Math.Max(0, b - 1);
        }
        if (b == 0xF0 && span.Length >= 5)
        {
            markerBytesRead = 5;
            return Math.Max(0, BinaryPrimitives.ReadInt32BigEndian(span.Slice(1, 4)));
        }
        if (b == 0xF2 && span.Length >= 3)
        {
            markerBytesRead = 3;
            return BinaryPrimitives.ReadUInt16BigEndian(span.Slice(1, 2));
        }
        if (b == 0xFA && span.Length >= 4)
        {
            markerBytesRead = 4;
            return (span[1] << 16) | (span[2] << 8) | span[3];
        }
        if (b == 0xFE && span.Length >= 2)
        {
            markerBytesRead = 2;
            return span[1];
        }

        markerBytesRead = 1;
        return b;
    }

    /// <summary>
    /// Loads an existing JSON file to preserve current translations during re-extraction.
    /// </summary>
    public static JsonObject? LoadExistingJson(string path)
    {
        if (!File.Exists(path)) return null;
        try
        {
            return JsonNode.Parse(File.ReadAllText(path)) as JsonObject;
        }
        catch
        {
            return null;
        }
    }

    public static void SaveJsonObject(string path, JsonObject obj)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(path, obj.ToJsonString(options));
    }
}
