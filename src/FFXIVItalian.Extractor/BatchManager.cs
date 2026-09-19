using FFXIVItalian.Core.Glossary;
using FFXIVItalian.Core.SeString;
using FFXIVItalian.Core.Translation;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FFXIVItalian.Extractor;

public record SheetStats(
    string SheetName,
    string FileName,
    int TotalRows,
    int TranslatedRows,
    int PendingRows,
    double Percentage);

public static class BatchManager
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static SheetStats CalculateStats(string jsonFilePath)
    {
        string sheetName = Path.GetFileNameWithoutExtension(jsonFilePath);
        string fileName = Path.GetFileName(jsonFilePath);

        if (!File.Exists(jsonFilePath))
        {
            return new SheetStats(sheetName, fileName, 0, 0, 0, 0.0);
        }

        try
        {
            string content = File.ReadAllText(jsonFilePath);
            using var doc = JsonDocument.Parse(content);

            int total = 0;
            int translated = 0;

            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                total++;
                if (IsRowTranslated(prop.Value))
                {
                    translated++;
                }
            }

            int pending = Math.Max(0, total - translated);
            double pct = total > 0 ? Math.Round((double)translated / total * 100.0, 1) : 0.0;
            return new SheetStats(sheetName, fileName, total, translated, pending, pct);
        }
        catch
        {
            return new SheetStats(sheetName, fileName, 0, 0, 0, 0.0);
        }
    }

    public static bool IsRowTranslated(JsonElement elem)
    {
        if (elem.ValueKind == JsonValueKind.String)
        {
            return !string.IsNullOrWhiteSpace(elem.GetString());
        }

        if (elem.ValueKind == JsonValueKind.Object)
        {
            // Case 1: Clan / Race dual column (translation_name_masculine and translation_name_feminine)
            bool hasMasc = elem.TryGetProperty("translation_name_masculine", out var masc);
            bool hasFem = elem.TryGetProperty("translation_name_feminine", out var fem);
            if (hasMasc || hasFem)
            {
                string mStr = hasMasc && masc.ValueKind == JsonValueKind.String ? masc.GetString() ?? "" : "";
                string fStr = hasFem && fem.ValueKind == JsonValueKind.String ? fem.GetString() ?? "" : "";
                return !string.IsNullOrWhiteSpace(mStr) || !string.IsNullOrWhiteSpace(fStr);
            }

            // Case 2: Multi-column dialogue sheets with human prompts (e.g. CustomTalk col_31 / col_32)
            bool hasC31 = elem.TryGetProperty("col_31", out var c31);
            bool hasC32 = elem.TryGetProperty("col_32", out var c32);
            if (hasC31 || hasC32)
            {
                string c31Text = hasC31 && c31.ValueKind == JsonValueKind.String ? c31.GetString() ?? "" : "";
                string c32Text = hasC32 && c32.ValueKind == JsonValueKind.String ? c32.GetString() ?? "" : "";
                if (string.IsNullOrWhiteSpace(c31Text) && string.IsNullOrWhiteSpace(c32Text))
                {
                    // No human prompt text in this row; internal engine script identifier
                    return true;
                }

                bool c31Ok = string.IsNullOrWhiteSpace(c31Text) ||
                    (elem.TryGetProperty("translation_col_31", out var tc31) && !string.IsNullOrWhiteSpace(tc31.GetString()));
                bool c32Ok = string.IsNullOrWhiteSpace(c32Text) ||
                    (elem.TryGetProperty("translation_col_32", out var tc32) && !string.IsNullOrWhiteSpace(tc32.GetString()));

                return c31Ok && c32Ok;
            }

            // Case 3: Dual string command (translation_name and translation_description)
            if (elem.TryGetProperty("translation_description", out var descProp))
            {
                string desc = descProp.ValueKind == JsonValueKind.String ? descProp.GetString() ?? "" : "";
                string name = "";
                if (elem.TryGetProperty("translation_name", out var nameProp) && nameProp.ValueKind == JsonValueKind.String)
                {
                    name = nameProp.GetString() ?? "";
                }
                return !string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(desc);
            }

            // Case 3: Simple string with translation / translation_name
            if (elem.TryGetProperty("translation", out var transProp) && transProp.ValueKind == JsonValueKind.String)
            {
                return !string.IsNullOrWhiteSpace(transProp.GetString());
            }

            if (elem.TryGetProperty("translation_name", out var tNameProp) && tNameProp.ValueKind == JsonValueKind.String)
            {
                return !string.IsNullOrWhiteSpace(tNameProp.GetString());
            }
        }

        return false;
    }

    public static void PrintStatus(string translationsDir, string? filter = null)
    {
        Console.WriteLine();
        Console.WriteLine("=========================================================================================");
        Console.WriteLine(" FFXIV Italian - Cruscotto di Avanzamento Traduzione (Stato dei Fogli)");
        Console.WriteLine("=========================================================================================");
        Console.WriteLine($"{"Categoria / Foglio",-26} {"File JSON / Gruppo",-26} {"Totale",8} {"Tradotte",10} {"Pendenti",10} {"Avanzamento",14}");
        Console.WriteLine(new string('-', 98));

        if (!Directory.Exists(translationsDir))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Cartella {translationsDir} non trovata!");
            Console.ResetColor();
            return;
        }

        var allFiles = Directory.GetFiles(translationsDir, "*.json", SearchOption.AllDirectories).ToList();

        // Categorize files
        var masterFiles = allFiles.Where(f => !f.Contains(Path.DirectorySeparatorChar + "quests" + Path.DirectorySeparatorChar)).ToList();
        var questFiles = allFiles.Where(f => f.Contains(Path.DirectorySeparatorChar + "quests" + Path.DirectorySeparatorChar)).ToList();

        int grandTotal = 0;
        int grandTranslated = 0;
        int grandPending = 0;

        bool isPendingFilter = !string.IsNullOrWhiteSpace(filter) &&
            (filter.Equals("pending", StringComparison.OrdinalIgnoreCase) || filter.Equals("--pending", StringComparison.OrdinalIgnoreCase));
        string? categoryFilter = !string.IsNullOrWhiteSpace(filter) && !isPendingFilter ? filter.Trim().ToUpperInvariant() : null;

        var allMasterStats = new List<(string Category, SheetStats Stats)>();

        // Group master files by folder name
        var masterGroups = masterFiles
            .GroupBy(f =>
            {
                var dir = Path.GetDirectoryName(f) ?? "";
                var rel = Path.GetRelativePath(translationsDir, dir);
                return rel == "." ? "root" : rel.ToUpperInvariant();
            })
            .OrderBy(g => g.Key);

        foreach (var group in masterGroups)
        {
            if (categoryFilter != null && !group.Key.Contains(categoryFilter, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!isPendingFilter)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[CATEGORIA: {group.Key}]");
                Console.ResetColor();
            }

            foreach (var file in group.OrderBy(f => Path.GetFileName(f)))
            {
                var stats = CalculateStats(file);
                allMasterStats.Add((group.Key, stats));

                if (!isPendingFilter)
                {
                    grandTotal += stats.TotalRows;
                    grandTranslated += stats.TranslatedRows;
                    grandPending += stats.PendingRows;
                    PrintRow(stats.SheetName, stats.FileName, stats.TotalRows, stats.TranslatedRows, stats.PendingRows, stats.Percentage);
                }
            }

            if (!isPendingFilter)
            {
                Console.WriteLine();
            }
        }

        if (isPendingFilter)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[FOGLI CON RIGHE PENDENTI - ORDINATI PER PERCENTUALE DI COMPLETAMENTO]");
            Console.ResetColor();

            var pendingOnly = allMasterStats
                .Where(s => s.Stats.PendingRows > 0)
                .OrderByDescending(s => s.Stats.Percentage)
                .ThenBy(s => s.Stats.PendingRows);

            foreach (var (cat, s) in pendingOnly)
            {
                grandTotal += s.TotalRows;
                grandTranslated += s.TranslatedRows;
                grandPending += s.PendingRows;
                PrintRow($"{s.SheetName} ({cat.ToLowerInvariant()})", s.FileName, s.TotalRows, s.TranslatedRows, s.PendingRows, s.Percentage);
            }
            Console.WriteLine();
        }

        // Group quest files by expansion
        if (questFiles.Count > 0 && (categoryFilter == null || categoryFilter.Contains("QUEST")))
        {
            if (!isPendingFilter)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[CATEGORIA: QUEST NARRATIVE - {questFiles.Count:N0} missioni censite]");
                Console.ResetColor();
            }

            var questGroups = questFiles
                .GroupBy(f =>
                {
                    var rel = Path.GetRelativePath(Path.Combine(translationsDir, "quests"), f);
                    var parts = rel.Split(Path.DirectorySeparatorChar);
                    return parts.Length > 0 ? parts[0].ToUpperInvariant() : "ALTRE";
                })
                .OrderBy(g => g.Key);

            foreach (var qGroup in questGroups)
            {
                int qTotal = 0;
                int qTrans = 0;
                int qPend = 0;

                foreach (var file in qGroup)
                {
                    var s = CalculateStats(file);
                    qTotal += s.TotalRows;
                    qTrans += s.TranslatedRows;
                    qPend += s.PendingRows;
                }

                grandTotal += qTotal;
                grandTranslated += qTrans;
                grandPending += qPend;

                double qPct = qTotal > 0 ? Math.Round((double)qTrans / qTotal * 100.0, 1) : 0.0;
                string label = $"Quests {qGroup.Key}";
                string fileInfo = $"({qGroup.Count()} missioni)";
                PrintRow(label, fileInfo, qTotal, qTrans, qPend, qPct);
            }
            Console.WriteLine();
        }

        Console.WriteLine(new string('-', 98));
        double overallPct = grandTotal > 0 ? Math.Round((double)grandTranslated / grandTotal * 100.0, 1) : 0.0;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{"TOTALE PARZIALE/COMPLESSIVO",-53} {grandTotal,8:N0} {grandTranslated,10:N0} {grandPending,10:N0} {$"{overallPct:F1}%",14}");
        Console.ResetColor();
        Console.WriteLine("=========================================================================================");
        Console.WriteLine();

        // Actionable guidance on where to continue
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=========================================================================================");
        Console.WriteLine(" 🎯 GUIDA OPERATIVA: DA DOVE CONTINUARE (Priorità Suggerite)");
        Console.WriteLine("=========================================================================================");
        Console.ResetColor();

        var inProgress = allMasterStats
            .Where(s => s.Stats.PendingRows > 0 && s.Stats.Percentage > 0)
            .OrderByDescending(s => s.Stats.Percentage)
            .ToList();

        if (inProgress.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" 1. 🚀 Fogli IN CORSO (Priorità alta: completamento progressivo):");
            Console.ResetColor();
            foreach (var (cat, s) in inProgress)
            {
                Console.WriteLine($"    * {s.SheetName,-14} : {s.PendingRows,6:N0} pendenti ({s.Percentage:F1}% fatto) [{cat.ToLowerInvariant()}]");
            }
            Console.WriteLine();
        }

        var quickWins = allMasterStats
            .Where(s => s.Stats.PendingRows > 0 && s.Stats.PendingRows <= 2000 && s.Stats.Percentage == 0)
            .OrderBy(s => s.Stats.PendingRows)
            .ToList();

        if (quickWins.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" 2. ⚡ QUICK WINS (Fogli intermedi a dimensione contenuta):");
            Console.ResetColor();
            foreach (var (cat, s) in quickWins)
            {
                Console.WriteLine($"    * {s.SheetName,-14} : {s.PendingRows,6:N0} righe ({s.Percentage:F1}% fatto) [{cat.ToLowerInvariant()}]");
            }
            Console.WriteLine();
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(" 3. 📜 TRAMA E MISSIONI (La narrazione di gioco):");
        Console.ResetColor();
        Console.WriteLine("    * Quests ARR     : 58.910 righe pendenti (1.599 missioni) - Campagna principale di A Realm Reborn");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("-----------------------------------------------------------------------------------------");
        Console.WriteLine(" COME AVVIARE UN BATCH DI TRADUZIONE:");
        Console.WriteLine("   1. Esporta:  dotnet run --project src/FFXIVItalian.Extractor -- export-batch <foglio> --size 50");
        Console.WriteLine("   2. Traduci:  fornisci il file in data/batches/ seguendo docs/TRANSLATION_PROMPT.md");
        Console.WriteLine("   3. Importa:  dotnet run --project src/FFXIVItalian.Extractor -- import-batch <foglio> <batch.json>");
        Console.WriteLine("   4. Rebuild:  .\\rebuild.bat  (oppure dotnet run --project src/FFXIVItalian.Patcher)");
        Console.WriteLine("-----------------------------------------------------------------------------------------");
        Console.WriteLine(" FILTRI STATUS DISPONIBILI:");
        Console.WriteLine("   status pending               Mostra solo i fogli incompleti ordinati per avanzamento");
        Console.WriteLine("   status system|world|combat   Mostra solo la categoria specificata");
        Console.WriteLine("=========================================================================================");
        Console.WriteLine();
    }

    private static void PrintRow(string name, string file, int total, int trans, int pend, double pct)
    {
        string pctStr = $"{pct,5:F1}%";
        Console.Write($"{name,-26} {file,-26} {total,8:N0} {trans,10:N0} {pend,10:N0} ");

        if (pct >= 100.0 && total > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{pctStr,14} [COMPLETO]");
        }
        else if (pct > 0.0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{pctStr,14} [IN CORSO]");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{pctStr,14} [DA FARE]");
        }
        Console.ResetColor();
    }

    public static (int ExportedCount, string OutputFile) ExportBatch(
        string translationsDir,
        string sheetName,
        int batchSize,
        string? customOutPath = null)
    {
        string fullPath = TranslationPathResolver.FindFile(translationsDir, sheetName);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File di traduzione per '{sheetName}' non trovato (cercato in '{fullPath}').");
        }

        string content = File.ReadAllText(fullPath);
        var rootNode = JsonNode.Parse(content) as JsonObject;
        if (rootNode == null)
        {
            throw new InvalidOperationException($"Impossibile analizzare '{fullPath}' come JsonObject.");
        }

        var batchObj = new JsonObject();
        int exported = 0;

        foreach (var (key, value) in rootNode)
        {
            if (exported >= batchSize) break;
            if (value == null) continue;

            using var doc = JsonDocument.Parse(value.ToJsonString());
            if (!IsRowTranslated(doc.RootElement))
            {
                batchObj[key] = JsonNode.Parse(value.ToJsonString());
                exported++;
            }
        }

        if (exported == 0)
        {
            return (0, string.Empty);
        }

        string fileName = Path.GetFileNameWithoutExtension(fullPath);
        string outPath = customOutPath ?? Path.Combine(
            Directory.GetCurrentDirectory(),
            "data",
            "batches",
            $"{fileName}_batch_{DateTime.Now:yyyyMMdd_HHmmss}.json");

        var outDir = Path.GetDirectoryName(outPath);
        if (!string.IsNullOrEmpty(outDir) && !Directory.Exists(outDir))
        {
            Directory.CreateDirectory(outDir);
        }

        File.WriteAllText(outPath, batchObj.ToJsonString(JsonOptions));
        return (exported, outPath);
    }

    public static (int UpdatedCount, int SyntaxErrors, int GlossaryWarnings) ImportBatch(
        string translationsDir,
        string sheetName,
        string batchFilePath,
        GlossaryEngine? glossaryEngine = null)
    {
        if (!File.Exists(batchFilePath))
        {
            throw new FileNotFoundException($"File batch '{batchFilePath}' non trovato.");
        }

        string masterPath = TranslationPathResolver.FindFile(translationsDir, sheetName);

        if (!File.Exists(masterPath))
        {
            throw new FileNotFoundException($"File master per '{sheetName}' non trovato (cercato in '{masterPath}').");
        }

        var masterObj = JsonNode.Parse(File.ReadAllText(masterPath)) as JsonObject
            ?? throw new InvalidOperationException($"Il master JSON '{masterPath}' non è un oggetto valido.");

        var batchObj = JsonNode.Parse(File.ReadAllText(batchFilePath)) as JsonObject
            ?? throw new InvalidOperationException($"Il file batch '{batchFilePath}' non è un oggetto valido.");

        int updatedCount = 0;
        int syntaxErrors = 0;
        int glossaryWarnings = 0;
        string fileName = Path.GetFileName(masterPath);

        foreach (var (key, batchNode) in batchObj)
        {
            if (batchNode == null) continue;

            using var batchDoc = JsonDocument.Parse(batchNode.ToJsonString());
            if (!IsRowTranslated(batchDoc.RootElement))
            {
                continue;
            }

            if (!masterObj.TryGetPropertyValue(key, out var masterNode) || masterNode == null)
            {
                masterObj[key] = JsonNode.Parse(batchNode.ToJsonString());
                updatedCount++;
                continue;
            }

            if (batchNode is JsonObject batchEntry && masterNode is JsonObject masterEntry)
            {
                string origText = "";
                string transText = "";

                if (batchEntry.TryGetPropertyValue("translation", out var tProp) && !string.IsNullOrWhiteSpace(tProp?.GetValue<string>()))
                {
                    masterEntry["translation"] = tProp.GetValue<string>();
                    transText = tProp.GetValue<string>() ?? "";
                    if (masterEntry.TryGetPropertyValue("original", out var oProp)) origText = oProp?.GetValue<string>() ?? "";
                    else if (masterEntry.TryGetPropertyValue("name", out var nProp)) origText = nProp?.GetValue<string>() ?? "";
                }

                if (batchEntry.TryGetPropertyValue("translation_name", out var tnProp) && !string.IsNullOrWhiteSpace(tnProp?.GetValue<string>()))
                {
                    masterEntry["translation_name"] = tnProp.GetValue<string>();
                    transText = tnProp.GetValue<string>() ?? "";
                    if (masterEntry.TryGetPropertyValue("name", out var nProp)) origText = nProp?.GetValue<string>() ?? "";
                }

                if (batchEntry.TryGetPropertyValue("translation_description", out var tdProp) && !string.IsNullOrWhiteSpace(tdProp?.GetValue<string>()))
                {
                    masterEntry["translation_description"] = tdProp.GetValue<string>();
                    string dTrans = tdProp.GetValue<string>() ?? "";
                    string dOrig = "";
                    if (masterEntry.TryGetPropertyValue("description", out var dProp)) dOrig = dProp?.GetValue<string>() ?? "";
                    ValidateEntry(fileName, $"{key} (desc)", dOrig, dTrans, glossaryEngine, ref syntaxErrors, ref glossaryWarnings);
                }

                if (batchEntry.TryGetPropertyValue("translation_name_masculine", out var tmMasc) && !string.IsNullOrWhiteSpace(tmMasc?.GetValue<string>()))
                {
                    masterEntry["translation_name_masculine"] = tmMasc.GetValue<string>();
                }

                if (batchEntry.TryGetPropertyValue("translation_name_feminine", out var tmFem) && !string.IsNullOrWhiteSpace(tmFem?.GetValue<string>()))
                {
                    masterEntry["translation_name_feminine"] = tmFem.GetValue<string>();
                }

                ValidateEntry(fileName, key, origText, transText, glossaryEngine, ref syntaxErrors, ref glossaryWarnings);
                updatedCount++;
            }
        }

        File.WriteAllText(masterPath, masterObj.ToJsonString(JsonOptions));
        return (updatedCount, syntaxErrors, glossaryWarnings);
    }

    private static void ValidateEntry(
        string file,
        string rowId,
        string orig,
        string trans,
        GlossaryEngine? engine,
        ref int syntaxErrors,
        ref int glossaryWarnings)
    {
        if (string.IsNullOrWhiteSpace(trans)) return;

        var seResult = SeStringValidator.Validate(orig, trans);
        foreach (var err in seResult.Errors)
        {
            syntaxErrors++;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [ERRORE SINTASSI] {file} Riga {rowId}: {err}");
            Console.ResetColor();
        }

        if (engine != null)
        {
            var glResult = engine.ValidateTranslation(orig, trans);
            foreach (var pro in glResult.ProhibitedUsages)
            {
                glossaryWarnings++;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  [FORMA VIETATA] {file} Riga {rowId}: {pro}");
                Console.ResetColor();
            }

            foreach (var warn in glResult.Warnings)
            {
                glossaryWarnings++;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  [AVVISO GLOSSARIO] {file} Riga {rowId}: {warn}");
                Console.ResetColor();
            }
        }
    }

    public static int AutoFillGlossary(string translationsDir, string sheetName, GlossaryEngine engine)
    {
        string fullPath = TranslationPathResolver.FindFile(translationsDir, sheetName);

        if (!File.Exists(fullPath)) return 0;

        var rootObj = JsonNode.Parse(File.ReadAllText(fullPath)) as JsonObject;
        if (rootObj == null) return 0;

        var termMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in engine.Entries)
        {
            if (!string.IsNullOrWhiteSpace(entry.EnglishTerm) && !string.IsNullOrWhiteSpace(entry.ItalianTerm))
            {
                termMap[entry.EnglishTerm.Trim()] = entry.ItalianTerm.Trim();
            }
        }

        int filledCount = 0;

        foreach (var (_, node) in rootObj)
        {
            if (node is not JsonObject obj) continue;

            if (obj.TryGetPropertyValue("translation", out var tProp) &&
                string.IsNullOrWhiteSpace(tProp?.GetValue<string>()))
            {
                string orig = "";
                if (obj.TryGetPropertyValue("original", out var oProp)) orig = oProp?.GetValue<string>() ?? "";
                else if (obj.TryGetPropertyValue("name", out var nProp)) orig = nProp?.GetValue<string>() ?? "";

                if (!string.IsNullOrWhiteSpace(orig) && termMap.TryGetValue(orig.Trim(), out var itTerm))
                {
                    obj["translation"] = itTerm;
                    filledCount++;
                }
            }
            else if (obj.TryGetPropertyValue("translation_name", out var tnProp) &&
                     string.IsNullOrWhiteSpace(tnProp?.GetValue<string>()))
            {
                string name = "";
                if (obj.TryGetPropertyValue("name", out var nProp)) name = nProp?.GetValue<string>() ?? "";

                if (!string.IsNullOrWhiteSpace(name) && termMap.TryGetValue(name.Trim(), out var itTerm))
                {
                    obj["translation_name"] = itTerm;
                    filledCount++;
                }
            }
        }

        if (filledCount > 0)
        {
            File.WriteAllText(fullPath, rootObj.ToJsonString(JsonOptions));
        }

        return filledCount;
    }
}
