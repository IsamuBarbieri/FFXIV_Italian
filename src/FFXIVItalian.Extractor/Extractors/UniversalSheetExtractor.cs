using Lumina;
using Lumina.Data;
using System.Buffers.Binary;
using System.Text.Json.Nodes;

namespace FFXIVItalian.Extractor.Extractors;

public class UniversalSheetExtractor : BaseSheetExtractor
{
    private readonly string _sheetName;
    private readonly string _defaultJsonFileName;
    private readonly string _description;

    public override string SheetName => _sheetName;
    public override string DefaultJsonFileName => _defaultJsonFileName;
    public override string Description => _description;

    public UniversalSheetExtractor(string sheetName, string? defaultJsonFileName = null, string? description = null)
    {
        _sheetName = sheetName;
        _defaultJsonFileName = defaultJsonFileName ?? $"{sheetName.ToLowerInvariant()}.json";
        _description = description ?? $"Foglio EXD {sheetName}";
    }

    public override int ExtractAndSave(GameData lumina, string targetJsonPath)
    {
        var exh = lumina.GetFile($"exd/{_sheetName.ToLowerInvariant()}.exh");
        if (exh == null)
        {
            throw new FileNotFoundException($"exd/{_sheetName.ToLowerInvariant()}.exh non trovato.");
        }

        ushort fixedSize = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(0x06, 2));
        ushort colCount = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(0x08, 2));
        ushort pageCount = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(0x0A, 2));
        ushort langCount = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(0x0C, 2));

        // Find all string columns (type == 0)
        var stringCols = new List<(int ColIndex, ushort Offset)>();
        for (int c = 0; c < colCount; c++)
        {
            int colPos = 0x20 + (c * 4);
            if (colPos + 4 > exh.Data.Length) break;
            ushort type = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(colPos, 2));
            ushort offset = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(colPos + 2, 2));
            if (type == 0)
            {
                stringCols.Add((c, offset));
            }
        }

        if (stringCols.Count == 0)
        {
            Console.WriteLine($"Il foglio '{_sheetName}' non contiene colonne di tipo String.");
            return 0;
        }

        // Read page table: starts at 0x20 + (colCount * 4)
        int pageTableStart = 0x20 + (colCount * 4);
        var pageStartRowIds = new List<uint>();
        for (int p = 0; p < pageCount; p++)
        {
            int pagePos = pageTableStart + (p * 8);
            if (pagePos + 8 > exh.Data.Length) break;
            uint startRowId = BinaryPrimitives.ReadUInt32BigEndian(exh.Data.AsSpan(pagePos, 4));
            pageStartRowIds.Add(startRowId);
        }

        if (pageStartRowIds.Count == 0)
        {
            pageStartRowIds.Add(0);
        }

        var existing = LoadExistingJson(targetJsonPath);
        var root = new JsonObject();
        int totalExtracted = 0;

        foreach (uint startRowId in pageStartRowIds)
        {
            string exdPath = $"exd/{_sheetName.ToLowerInvariant()}_{startRowId}_en.exd";
            var exd = lumina.GetFile(exdPath);
            if (exd == null) continue;

            uint indexSize = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(0x08, 4));
            int count = (int)(indexSize / 8);

            for (int i = 0; i < count; i++)
            {
                int entryPos = 0x20 + (i * 8);
                if (entryPos + 8 > exd.Data.Length) break;

                uint rId = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(entryPos, 4));
                uint off = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(entryPos + 4, 4));
                if (off + 6 > exd.Data.Length) continue;

                int dataSize = (int)BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan((int)off, 4));
                string key = rId.ToString();

                // Extract all string columns
                var strings = new List<string>();
                bool hasAnyText = false;
                foreach (var (_, colOffset) in stringCols)
                {
                    string s = ReadRowString(exd.Data, (int)off + 6, fixedSize, colOffset, dataSize);
                    strings.Add(s);
                    if (!string.IsNullOrWhiteSpace(s)) hasAnyText = true;
                }

                bool hasExisting = existing != null && existing.TryGetPropertyValue(key, out _);
                if (!hasAnyText && !hasExisting) continue;

                // Build entry object preserving existing translations
                JsonObject entryObj;
                if (stringCols.Count == 1)
                {
                    string translation = "";
                    if (existing != null && existing.TryGetPropertyValue(key, out var oldNode) && oldNode is JsonObject oldObj)
                    {
                        if (oldObj.TryGetPropertyValue("translation", out var t)) translation = t?.GetValue<string>() ?? "";
                        else if (oldObj.TryGetPropertyValue("translation_name", out var tn)) translation = tn?.GetValue<string>() ?? "";
                    }

                    entryObj = new JsonObject
                    {
                        ["original"] = strings[0],
                        ["translation"] = translation
                    };
                }
                else if (stringCols.Count == 2)
                {
                    string tName = "";
                    string tDesc = "";
                    if (existing != null && existing.TryGetPropertyValue(key, out var oldNode) && oldNode is JsonObject oldObj)
                    {
                        if (oldObj.TryGetPropertyValue("translation_name", out var tn)) tName = tn?.GetValue<string>() ?? "";
                        else if (oldObj.TryGetPropertyValue("translation", out var t)) tName = t?.GetValue<string>() ?? "";

                        if (oldObj.TryGetPropertyValue("translation_description", out var td)) tDesc = td?.GetValue<string>() ?? "";
                    }

                    entryObj = new JsonObject
                    {
                        ["name"] = strings[0],
                        ["translation_name"] = tName,
                        ["description"] = strings[1],
                        ["translation_description"] = tDesc
                    };
                }
                else
                {
                    entryObj = new JsonObject();
                    for (int sIdx = 0; sIdx < strings.Count; sIdx++)
                    {
                        string fieldName = sIdx == 0 ? "name" : (sIdx == 1 ? "description" : $"col_{sIdx}");
                        string transFieldName = $"translation_{fieldName}";
                        string transVal = "";

                        if (existing != null && existing.TryGetPropertyValue(key, out var oldNode) && oldNode is JsonObject oldObj)
                        {
                            if (oldObj.TryGetPropertyValue(transFieldName, out var t)) transVal = t?.GetValue<string>() ?? "";
                        }

                        entryObj[fieldName] = strings[sIdx];
                        entryObj[transFieldName] = transVal;
                    }
                }

                root[key] = entryObj;
                totalExtracted++;
            }
        }

        SaveJsonObject(targetJsonPath, root);
        return totalExtracted;
    }
}
