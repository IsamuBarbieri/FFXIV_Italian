namespace FFXIVItalian.Core.Translation;

public static class TranslationPathResolver
{
    private static readonly Dictionary<string, string> KnownCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        // System
        ["addon"] = "system",
        ["lobby"] = "system",
        ["maincommand"] = "system",
        ["maincommandcategory"] = "system",
        ["error"] = "system",
        ["logmessage"] = "system",
        ["textcommand"] = "system",
        ["howto"] = "system",
        ["howtocategory"] = "system",

        // World
        ["placename"] = "world",
        ["classjob"] = "world",
        ["race"] = "world",
        ["tribe"] = "world",
        ["weather"] = "world",
        ["title"] = "world",

        // Combat
        ["action"] = "combat",
        ["actiontransient"] = "combat",
        ["status"] = "combat",
        ["trait"] = "combat",
        ["traittransient"] = "combat",

        // Items
        ["item"] = "items",
        ["itemuicategory"] = "items",

        // Dialogue & World Events
        ["balloon"] = "dialogue",
        ["defaulttalk"] = "dialogue",
        ["customtalk"] = "dialogue",
        ["fate"] = "world",
        ["achievement"] = "world",
        ["instancecontent"] = "world",
        ["aetheryte"] = "world"
    };

    public static string GetTargetSubdirectory(string sheetName)
    {
        if (sheetName.StartsWith("quest/", StringComparison.OrdinalIgnoreCase) ||
            sheetName.StartsWith(@"quest\", StringComparison.OrdinalIgnoreCase))
        {
            var parts = sheetName.Replace('\\', '/').Split('/');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int folderNum))
            {
                string exp = folderNum switch
                {
                    <= 15 => "arr",
                    <= 25 => "heavensward",
                    <= 32 => "stormblood",
                    <= 39 => "shadowbringers",
                    <= 48 => "endwalker",
                    _ => "dawntrail"
                };
                return Path.Combine("quests", exp, parts[1]);
            }
            return "quests";
        }

        return KnownCategories.TryGetValue(sheetName, out var cat) ? cat : "misc";
    }

    public static string ResolveTargetPath(string translationsDir, string sheetName, string? defaultFileName = null)
    {
        string fileName = defaultFileName ?? (sheetName.Contains('/') || sheetName.Contains('\\')
            ? $"{Path.GetFileName(sheetName)}.json"
            : $"{sheetName.ToLowerInvariant()}.json");

        string existing = FindFile(translationsDir, sheetName);
        if (File.Exists(existing)) return existing;

        string subDir = GetTargetSubdirectory(sheetName);
        return Path.Combine(translationsDir, subDir, fileName);
    }

    public static string FindFile(string translationsDir, string sheetNameOrRelativePath)
    {
        if (string.IsNullOrWhiteSpace(translationsDir) || !Directory.Exists(translationsDir))
            return Path.Combine(translationsDir ?? "", sheetNameOrRelativePath);

        string fileName = sheetNameOrRelativePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            ? sheetNameOrRelativePath
            : $"{sheetNameOrRelativePath}.json";

        // 1. Direct path check
        string directPath = Path.Combine(translationsDir, fileName);
        if (File.Exists(directPath)) return directPath;

        // 2. Relative check with Category
        string justName = Path.GetFileName(fileName);
        string sheetBase = Path.GetFileNameWithoutExtension(fileName);
        if (KnownCategories.TryGetValue(sheetBase, out var cat))
        {
            string catPath = Path.Combine(translationsDir, cat, justName);
            if (File.Exists(catPath)) return catPath;
        }

        // 3. Quest path check
        if (sheetNameOrRelativePath.StartsWith("quest/", StringComparison.OrdinalIgnoreCase) ||
            sheetNameOrRelativePath.StartsWith(@"quest\", StringComparison.OrdinalIgnoreCase))
        {
            string rawSheet = sheetNameOrRelativePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                ? sheetNameOrRelativePath[..^5]
                : sheetNameOrRelativePath;
            string targetSub = GetTargetSubdirectory(rawSheet);
            string questPath = Path.Combine(translationsDir, targetSub, justName);
            if (File.Exists(questPath)) return questPath;
        }

        // 4. Search AllDirectories
        var match = Directory.GetFiles(translationsDir, justName, SearchOption.AllDirectories).FirstOrDefault();
        if (match != null) return match;

        // 5. Default path
        string defaultSub = GetTargetSubdirectory(sheetBase);
        return Path.Combine(translationsDir, defaultSub, justName);
    }

    /// <summary>
    /// Reorganizes top-level translation JSON files into categorized subfolders
    /// (system/, world/, combat/, items/, dialogue/, quests/).
    /// </summary>
    public static int Reorganize(string translationsDir)
    {
        if (!Directory.Exists(translationsDir)) return 0;

        int moved = 0;
        foreach (var file in Directory.GetFiles(translationsDir, "*.json", SearchOption.TopDirectoryOnly))
        {
            string fileName = Path.GetFileName(file);
            string sheetName = Path.GetFileNameWithoutExtension(file);
            string subDir = GetTargetSubdirectory(sheetName);

            if (string.IsNullOrEmpty(subDir) || subDir == ".") continue;

            string targetDir = Path.Combine(translationsDir, subDir);
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            string targetPath = Path.Combine(targetDir, fileName);
            if (Path.GetFullPath(file) != Path.GetFullPath(targetPath))
            {
                File.Move(file, targetPath, overwrite: true);
                Console.WriteLine($"  [SPOSTATO] {fileName,-25} -> {subDir}/{fileName}");
                moved++;
            }
        }

        return moved;
    }
}
