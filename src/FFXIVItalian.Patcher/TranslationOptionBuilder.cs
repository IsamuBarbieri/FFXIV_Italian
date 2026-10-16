using System.Security.Cryptography;
using System.Text;
using FFXIVItalian.Core.Packaging;

namespace FFXIVItalian.Patcher;

public static class TranslationOptionBuilder
{
    private sealed record Block(string Key, string Name, string Description);

    private static readonly Block[] Blocks =
    [
        new("missions", "Missioni e dialoghi", "Dialoghi, scelte e menu dei PNG; testi delle missioni approvate presenti in questa build."),
        new("world", "Mondo di gioco", "Nomi di PNG, nemici, luoghi, razze e clan; titoli, reputazioni, gradi delle Grandi Compagnie, F.A.T.E. e altri testi del mondo di gioco."),
        new("jobs", "Classi e mestieri", "Nomi, sigle e categorie delle classi e dei mestieri."),
        new("items", "Oggetti", "Nomi, descrizioni e categorie degli oggetti comuni."),
        new("event-items", "Oggetti legati a eventi", "Oggetti e categorie legati agli eventi."),
        new("collectibles", "Oggetti collezionabili", "Collezioni, schedari e oggetti collezionabili."),
        new("combat", "Azioni, abilità, tratti e stati", "Testi di combattimento, azioni, abilità, tratti e stati."),
        new("achievements", "Obiettivi", "Obiettivi e relative categorie."),
        new("logs", "Registro di combattimento e messaggi di sistema", "Registro, errori e messaggi di sistema."),
        new("interface", "Interfaccia e altri contenuti", "Titolo, creazione del personaggio, menu, guide, attività, artigianato, alloggi, negozi, contenuti sociali e minigiochi; texture italiane disponibili."),
    ];

    private static readonly HashSet<string> LogSheets = new(StringComparer.OrdinalIgnoreCase)
    {
        "error", "logfilter", "logkind", "logmessage"
    };

    private static readonly HashSet<string> CollectibleSheets = new(StringComparer.OrdinalIgnoreCase)
    {
        "cabinetsubcategory", "itemseries"
    };

    public static List<PenumbraOptionGroup> Create(string translationsDir, IEnumerable<string> gamePaths)
    {
        var blockBySheet = new Dictionary<string, Block>(StringComparer.OrdinalIgnoreCase);
        foreach (string jsonPath in Directory.EnumerateFiles(translationsDir, "*.json", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(translationsDir, jsonPath);
            string sheet = Path.GetFileNameWithoutExtension(jsonPath);
            Block block = GetBlock(relativePath);
            if (blockBySheet.TryGetValue(sheet, out Block? existing) && existing != block)
                throw new InvalidDataException($"Il foglio '{sheet}' ricade in più blocchi Penumbra.");
            blockBySheet[sheet] = block;
        }

        var filesByBlock = Blocks.ToDictionary(
            block => block.Key,
            _ => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
            StringComparer.Ordinal);
        var ownerByGamePath = new Dictionary<string, Block>(StringComparer.OrdinalIgnoreCase);

        foreach (string gamePath in gamePaths)
        {
            string normalizedPath = gamePath.Replace('\\', '/');
            if (normalizedPath.Equals("readme_ita.txt", StringComparison.OrdinalIgnoreCase))
                continue;

            Block block;
            if (normalizedPath.StartsWith("exd/", StringComparison.OrdinalIgnoreCase))
            {
                string sheet = GetSheetName(normalizedPath);
                if (normalizedPath.StartsWith("exd/quest/", StringComparison.OrdinalIgnoreCase))
                    block = FindBlock("missions");
                else if (blockBySheet.TryGetValue(sheet, out Block? mapped))
                    block = mapped;
                else
                    block = GetFallbackBlock(sheet);
            }
            else
            {
                block = FindBlock("interface");
            }

            if (ownerByGamePath.TryGetValue(normalizedPath, out Block? previous) && previous != block)
                throw new InvalidDataException($"Il file '{normalizedPath}' appartiene a più blocchi Penumbra.");

            ownerByGamePath[normalizedPath] = block;
            filesByBlock[block.Key][normalizedPath] = normalizedPath;
        }

        return Blocks
            .Where(block => filesByBlock[block.Key].Count > 0)
            .Select(block => new PenumbraOptionGroup
            {
                Id = StableId($"group:{block.Key}"),
                Name = block.Name,
                Description = $"{block.Description} Pagine di testo: {filesByBlock[block.Key].Count(path => path.Key.EndsWith(".exd", StringComparison.OrdinalIgnoreCase))}; risorse grafiche o aggiuntive: {filesByBlock[block.Key].Count(path => !path.Key.EndsWith(".exd", StringComparison.OrdinalIgnoreCase))}. La copertura è limitata ai contenuti approvati disponibili.",
                DefaultSettings = 1,
                Options =
                [
                    new PenumbraOption
                    {
                        Id = StableId($"option:{block.Key}:enabled"),
                        Name = "Traduzione italiana attiva",
                        Description = "Deseleziona per usare il testo originale del gioco in questo blocco.",
                        Files = filesByBlock[block.Key],
                    }
                ],
            })
            .ToList();
    }

    private static Block GetBlock(string relativePath)
    {
        string[] parts = relativePath.Replace('\\', '/').Split('/');
        string category = parts[0];
        string sheet = Path.GetFileNameWithoutExtension(relativePath);

        if (category.Equals("quests", StringComparison.OrdinalIgnoreCase) ||
            category.Equals("dialogue", StringComparison.OrdinalIgnoreCase))
            return FindBlock("missions");

        if (category.Equals("combat", StringComparison.OrdinalIgnoreCase) ||
            (category.Equals("crafting", StringComparison.OrdinalIgnoreCase) && sheet.Equals("craftaction", StringComparison.OrdinalIgnoreCase)))
            return FindBlock("combat");

        if (category.Equals("world", StringComparison.OrdinalIgnoreCase))
        {
            if (sheet.StartsWith("achievement", StringComparison.OrdinalIgnoreCase))
                return FindBlock("achievements");
            if (sheet.Equals("classjob", StringComparison.OrdinalIgnoreCase) ||
                sheet.Equals("classjobcategory", StringComparison.OrdinalIgnoreCase))
                return FindBlock("jobs");
            return FindBlock("world");
        }

        if (category.Equals("items", StringComparison.OrdinalIgnoreCase))
        {
            if (sheet.StartsWith("eventitem", StringComparison.OrdinalIgnoreCase))
                return FindBlock("event-items");
            if (IsCollectibleSheet(sheet))
                return FindBlock("collectibles");
            return FindBlock("items");
        }

        if (category.Equals("system", StringComparison.OrdinalIgnoreCase) && LogSheets.Contains(sheet))
            return FindBlock("logs");

        if (category.Equals("crafting", StringComparison.OrdinalIgnoreCase) && sheet.StartsWith("collectables", StringComparison.OrdinalIgnoreCase))
            return FindBlock("collectibles");

        return FindBlock("interface");
    }

    private static Block GetFallbackBlock(string sheet)
    {
        if (sheet.StartsWith("quest", StringComparison.OrdinalIgnoreCase))
            return FindBlock("missions");
        if (sheet.StartsWith("achievement", StringComparison.OrdinalIgnoreCase))
            return FindBlock("achievements");
        if (sheet.Equals("classjob", StringComparison.OrdinalIgnoreCase) ||
            sheet.Equals("classjobcategory", StringComparison.OrdinalIgnoreCase))
            return FindBlock("jobs");
        if (sheet.StartsWith("eventitem", StringComparison.OrdinalIgnoreCase))
            return FindBlock("event-items");
        if (IsCollectibleSheet(sheet) || sheet.StartsWith("collectables", StringComparison.OrdinalIgnoreCase))
            return FindBlock("collectibles");
        if (LogSheets.Contains(sheet))
            return FindBlock("logs");
        return FindBlock("interface");
    }

    private static bool IsCollectibleSheet(string sheet) =>
        CollectibleSheets.Contains(sheet) || sheet.StartsWith("collectable", StringComparison.OrdinalIgnoreCase);

    private static string GetSheetName(string gamePath)
    {
        string fileName = Path.GetFileName(gamePath);
        if (!fileName.EndsWith(".exd", StringComparison.OrdinalIgnoreCase))
            return Path.GetFileNameWithoutExtension(fileName);

        string sheetAndPage = fileName[..^4];
        if (sheetAndPage.EndsWith("_en", StringComparison.OrdinalIgnoreCase))
            sheetAndPage = sheetAndPage[..^3];

        int separator = sheetAndPage.LastIndexOf('_');
        if (separator > 0 && uint.TryParse(sheetAndPage[(separator + 1)..], out _))
            sheetAndPage = sheetAndPage[..separator];
        return sheetAndPage;
    }

    private static Block FindBlock(string key) => Blocks.Single(block => block.Key == key);

    private static Guid StableId(string key)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes($"FFXIVItalian.PenumbraOptions/{key}"));
        return new Guid(hash.AsSpan(0, 16));
    }
}
