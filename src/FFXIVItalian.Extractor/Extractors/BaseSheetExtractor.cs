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
    private static readonly Encoding StrictUtf8 = new UTF8Encoding(false, true);

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

        int pageTableStart = 0x20 + (colCount * 4);
        uint startRowId = 0;
        if (pageCount > 0 && pageTableStart + 4 <= exh.Data.Length)
        {
            startRowId = BinaryPrimitives.ReadUInt32BigEndian(exh.Data.AsSpan(pageTableStart, 4));
        }

        var exd = lumina.GetFile($"exd/{SheetName.ToLowerInvariant()}_{startRowId}_en.exd");
        if (exd == null)
        {
            Console.WriteLine($"Errore: exd/{SheetName.ToLowerInvariant()}_{startRowId}_en.exd non trovato!");
            return;
        }

        uint indexSize = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(0x08, 4));
        int rowCount = (int)(indexSize / 8);
        Console.WriteLine($"EXD ({startRowId}): Righe indicizzate = {rowCount}, Dimensione file = {exd.Data.Length:N0} byte");

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
            var textBytes = bytes.Slice(textStart, i - textStart);
            if (textBytes.IndexOfAny((byte)0x01, (byte)0x03, (byte)0x0E) >= 0)
            {
                sb.Append($"<hex:{Convert.ToHexString(textBytes)}>");
                continue;
            }

            try
            {
                sb.Append(StrictUtf8.GetString(textBytes));
            }
            catch (DecoderFallbackException)
            {
                sb.Append($"<hex:{Convert.ToHexString(textBytes)}>");
            }
        }

        return sb.ToString();
    }

    private static int ReadEncodedLength(ReadOnlySpan<byte> span, out int markerBytesRead)
    {
        markerBytesRead = 0;
        if (span.IsEmpty) return -1;
        byte marker = span[0];
        if (marker is > 0 and < 0xD0)
        {
            markerBytesRead = 1;
            return marker - 1;
        }
        if (marker is < 0xF0 or > 0xFE) return -1;

        // Packed integers store only nonzero bytes, selected by the marker's bit mask.
        int mask = (marker + 1) & 0x0F;
        uint value = 0;
        markerBytesRead = 1;
        for (int bit = 8, shift = 24; bit > 0; bit >>= 1, shift -= 8)
        {
            if ((mask & bit) == 0) continue;
            if (markerBytesRead >= span.Length || span[markerBytesRead] == 0) return -1;
            value |= (uint)span[markerBytesRead++] << shift;
        }
        return value <= int.MaxValue ? (int)value : -1;
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

        var options = new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        File.WriteAllText(path, obj.ToJsonString(options));
    }
}
