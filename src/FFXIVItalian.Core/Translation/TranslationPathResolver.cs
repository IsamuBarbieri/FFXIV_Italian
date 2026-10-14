namespace FFXIVItalian.Core.Translation;

public static class TranslationPathResolver
{
    private static readonly string[] EditorialStates = ["da_tradurre", "da_revisionare"];

    public static string GetStateDirectory(string translationsDir, string state)
    {
        string fullPath = Path.GetFullPath(translationsDir);
        string dataDir = Directory.GetParent(fullPath)?.FullName ?? fullPath;
        return Path.Combine(dataDir, state);
    }

    public static string[] GetCorpusDirectories(string translationsDir) =>
        [translationsDir, .. EditorialStates.Select(state => GetStateDirectory(translationsDir, state))];

    public static IEnumerable<string> GetCorpusFiles(string translationsDir, string searchPattern = "*.json") =>
        GetCorpusDirectories(translationsDir)
            .Where(Directory.Exists)
            .SelectMany(directory => Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories));

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
        ["baseparam"] = "system",
        ["addontransient"] = "system",
        ["description"] = "system",
        ["descriptionstring"] = "system",

        // World
        ["placename"] = "world",
        ["classjob"] = "world",
        ["guardiandeity"] = "world",
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
        ["actioncategory"] = "combat",
        ["actioncomboroute"] = "combat",
        ["actioncomboroutetransient"] = "combat",
        ["aozactiontransient"] = "combat",
        ["classjobactionuicategory"] = "combat",

        // Items
        ["item"] = "items",
        ["itemuicategory"] = "items",
        ["itemsearchcategory"] = "items",
        ["itemseries"] = "items",
        ["itemspecialbonus"] = "items",

        // Dialogue & World Events
        ["balloon"] = "dialogue",
        ["defaulttalk"] = "dialogue",
        ["customtalk"] = "dialogue",
        ["fate"] = "world",
        ["achievement"] = "world",
        ["contentfindercondition"] = "world",
        ["contentfinderconditiontransient"] = "world",
        ["aetheryte"] = "world",
        ["chocobotaxistand"] = "world",
        ["guildleveassignment"] = Path.Combine("quests", "master")
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

        var normalized = sheetName.Replace('\\', '/').ToLowerInvariant();
        if (normalized.Contains('/'))
        {
            string family = normalized.Split('/')[0];
            string category = family switch
            {
                "custom" or "cut_scene" or "opening" or "story" => "dialogue",
                "content" or "dungeon" or "guild_order" or "raid" => "activities",
                "leve" => "quests", "shop" => "shops",
                "transport" or "warp" => "world", "system" => "system",
                _ => "misc"
            };
            return Path.Combine(category, Path.GetDirectoryName(normalized)!);
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
        return Path.Combine(GetStateDirectory(translationsDir, "da_tradurre"), subDir, fileName);
    }

    public static string? ResolveSheetForFile(string jsonPath, IEnumerable<string> candidateSheets)
    {
        var candidates = candidateSheets.ToArray();
        var matches = candidates.Where(sheet => Path.GetFullPath(jsonPath).EndsWith(
            Path.DirectorySeparatorChar + Path.Combine(GetTargetSubdirectory(sheet),
                Path.GetFileName(sheet.Replace('\\', '/')).ToLowerInvariant() + ".json"),
            StringComparison.OrdinalIgnoreCase)).ToArray();
        if (matches.Length == 1) return matches[0];
        if (matches.Length == 0 && candidates.Length <= 1) return candidates.SingleOrDefault();
        throw new InvalidDataException($"Foglio ambiguo per {jsonPath}: verificare categoria e percorso completo.");
    }

    public static string FindFile(string translationsDir, string sheetNameOrRelativePath)
    {
        if (string.IsNullOrWhiteSpace(translationsDir))
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
        if (sheetNameOrRelativePath.Contains('/') || sheetNameOrRelativePath.Contains('\\'))
        {
            string rawSheet = sheetNameOrRelativePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                ? sheetNameOrRelativePath[..^5] : sheetNameOrRelativePath;
            string target = Path.Combine(GetTargetSubdirectory(rawSheet), justName);
            foreach (string directory in GetCorpusDirectories(translationsDir))
                foreach (string relative in new[] { fileName, target })
                    if (File.Exists(Path.Combine(directory, relative))) return Path.Combine(directory, relative);
            // A full sheet name must not fall back to a different, homonymous sheet.
            return Path.Combine(translationsDir, target);
        }
        if (KnownCategories.TryGetValue(sheetBase, out var cat))
        {
            string catPath = Path.Combine(translationsDir, cat, justName);
            if (File.Exists(catPath)) return catPath;
        }

        // 3. Search AllDirectories for an unqualified sheet name.
        var match = GetCorpusFiles(translationsDir, justName).FirstOrDefault();
        if (match != null) return match;

        // 4. Default path
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
                File.Move(file, targetPath, overwrite: false);
                Console.WriteLine($"  [SPOSTATO] {fileName,-25} -> {subDir}/{fileName}");
                moved++;
            }
        }

        return moved;
    }
}
