using FFXIVItalian.Core.Translation;
using Lumina;
using Lumina.Data;
using System.Buffers.Binary;
using System.Text.Json.Nodes;

namespace FFXIVItalian.Extractor.Extractors;

public static class QuestExtractor
{
    private static readonly Dictionary<string, (int Min, int Max)> ExpansionFolderRanges = new(StringComparer.OrdinalIgnoreCase)
    {
        ["arr"] = (0, 15),
        ["heavensward"] = (16, 25),
        ["hw"] = (16, 25),
        ["stormblood"] = (26, 32),
        ["sb"] = (26, 32),
        ["shadowbringers"] = (33, 39),
        ["shb"] = (33, 39),
        ["endwalker"] = (40, 48),
        ["ew"] = (40, 48),
        ["dawntrail"] = (49, 55),
        ["dt"] = (49, 55),
    };

    public static string GetExpansionName(int folderNum)
    {
        return folderNum switch
        {
            <= 15 => "arr",
            <= 25 => "heavensward",
            <= 32 => "stormblood",
            <= 39 => "shadowbringers",
            <= 48 => "endwalker",
            _ => "dawntrail"
        };
    }

    /// <summary>
    /// Extracts a single quest EXD sheet into JSON.
    /// Preserves existing Italian translations if the JSON already exists.
    /// </summary>
    public static int ExtractQuest(GameData lumina, string questSheetName, string translationsDir)
    {
        string normSheetName = questSheetName.Replace('\\', '/').Trim();
        string exhPath = $"exd/{normSheetName.ToLowerInvariant()}.exh";
        var exh = lumina.GetFile(exhPath);
        if (exh == null)
        {
            throw new FileNotFoundException($"Foglio quest non trovato: {exhPath}");
        }

        ushort fixedSize = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(0x06, 2));
        ushort colCount = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(0x08, 2));
        ushort pageCount = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(0x0A, 2));

        // Read page table
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

        string targetJsonPath = TranslationPathResolver.ResolveTargetPath(translationsDir, normSheetName);
        var existing = BaseSheetExtractor.LoadExistingJson(targetJsonPath);
        var root = new JsonObject();
        int rowCount = 0;

        foreach (uint startRowId in pageStartRowIds)
        {
            string exdPath = $"exd/{normSheetName.ToLowerInvariant()}_{startRowId}_en.exd";
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

                string tag = ReadQuestString(exd.Data, (int)off + 6, fixedSize, 0, dataSize);
                string orig = ReadQuestString(exd.Data, (int)off + 6, fixedSize, 4, dataSize);

                string trans = "";
                if (existing != null && existing.TryGetPropertyValue(key, out var oldNode) && oldNode is JsonObject oldObj)
                {
                    if (oldObj.TryGetPropertyValue("translation", out var tProp))
                    {
                        trans = tProp?.GetValue<string>() ?? "";
                    }
                    else if (oldObj.TryGetPropertyValue("translation_description", out var tdProp))
                    {
                        trans = tdProp?.GetValue<string>() ?? "";
                    }
                }

                if (string.IsNullOrWhiteSpace(orig) && string.IsNullOrWhiteSpace(trans))
                    continue;

                root[key] = new JsonObject
                {
                    ["tag"] = tag,
                    ["original"] = orig,
                    ["translation"] = trans
                };

                rowCount++;
            }
        }

        var dir = Path.GetDirectoryName(targetJsonPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        BaseSheetExtractor.SaveJsonObject(targetJsonPath, root);
        return rowCount;
    }

    /// <summary>
    /// Extracts all quests in the game or for a specific expansion.
    /// </summary>
    public static (int QuestsExtracted, int RowsExtracted) ExtractQuests(
        GameData lumina,
        string expansionFilter,
        string translationsDir,
        Action<int, int, string>? onProgress = null)
    {
        var rootExl = lumina.GetFile("exd/root.exl");
        if (rootExl == null)
        {
            throw new FileNotFoundException("exd/root.exl non trovato nel client.");
        }

        string text = System.Text.Encoding.UTF8.GetString(rootExl.Data);
        var lines = text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        // Find all quest sheet names
        var questNames = new List<string>();
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length < 1) continue;
            string sheet = parts[0].Trim();
            if (sheet.StartsWith("quest/", StringComparison.OrdinalIgnoreCase))
            {
                questNames.Add(sheet);
            }
        }

        // Single quest check
        if (expansionFilter.StartsWith("quest/", StringComparison.OrdinalIgnoreCase))
        {
            var match = questNames.FirstOrDefault(q => q.Equals(expansionFilter, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                int r = ExtractQuest(lumina, match, translationsDir);
                return (1, r);
            }
            throw new FileNotFoundException($"Quest '{expansionFilter}' non trovata in exd/root.exl.");
        }

        // Apply filter
        bool isAll = expansionFilter.Equals("all", StringComparison.OrdinalIgnoreCase);
        (int Min, int Max)? range = null;

        if (!isAll)
        {
            if (ExpansionFolderRanges.TryGetValue(expansionFilter, out var r))
            {
                range = r;
            }
            else
            {
                throw new ArgumentException($"Espansione non riconosciuta: '{expansionFilter}'. Opzioni valide: all, arr, heavensward, stormblood, shadowbringers, endwalker, dawntrail, o percorso quest specifico.");
            }
        }

        var filteredQuests = new List<string>();
        foreach (var q in questNames)
        {
            var qParts = q.Split('/');
            if (qParts.Length < 3) continue;

            if (isAll)
            {
                filteredQuests.Add(q);
            }
            else if (range.HasValue && int.TryParse(qParts[1], out int fNum))
            {
                if (fNum >= range.Value.Min && fNum <= range.Value.Max)
                {
                    filteredQuests.Add(q);
                }
            }
        }

        int questsExtracted = 0;
        int rowsExtracted = 0;

        for (int i = 0; i < filteredQuests.Count; i++)
        {
            string qName = filteredQuests[i];
            try
            {
                int rows = ExtractQuest(lumina, qName, translationsDir);
                questsExtracted++;
                rowsExtracted += rows;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"\n  [AVVISO] Salto {qName}: {ex.Message}");
                Console.ResetColor();
            }

            onProgress?.Invoke(i + 1, filteredQuests.Count, qName);
        }

        return (questsExtracted, rowsExtracted);
    }

    private static string ReadQuestString(byte[] exdData, int fixedStart, int fixedSize, int colOffset, int dataSize)
    {
        if (fixedStart + colOffset + 4 > exdData.Length) return string.Empty;
        uint relOffset = BinaryPrimitives.ReadUInt32BigEndian(exdData.AsSpan(fixedStart + colOffset, 4));
        int strStart = fixedStart + fixedSize + (int)relOffset;
        if (strStart >= exdData.Length) return string.Empty;

        int strEnd = strStart;
        int maxLen = fixedStart + fixedSize + (dataSize - fixedSize);
        while (strEnd < maxLen && strEnd < exdData.Length && exdData[strEnd] != 0)
        {
            strEnd++;
        }

        return BaseSheetExtractor.DecodeSeStringPayload(exdData.AsSpan(strStart, strEnd - strStart));
    }
}
