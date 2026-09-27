using FFXIVItalian.Core.Translation;
using FFXIVItalian.Extractor.Extractors;
using Lumina;
using Lumina.Data;
using System;
using System.IO;
using System.Linq;

namespace FFXIVItalian.Extractor;

public static class Program
{
    private const string DefaultSqPackPath = @"G:\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack";

    public static int Main(string[] args)
    {
        Console.WriteLine("==================================================");
        Console.WriteLine(" FFXIV Italian - Estrattore e Ispettore Fogli EXD");
        Console.WriteLine("==================================================");

        if (args.Length == 0 || args[0].Equals("--help", StringComparison.OrdinalIgnoreCase) || args[0].Equals("-h", StringComparison.OrdinalIgnoreCase))
        {
            PrintUsage();
            return 0;
        }

        string command = args[0].ToLowerInvariant();
        string sqpackPath = FindSqPackPath(args);

        switch (command)
        {
            case "status":
                return RunStatus(args);

            case "export-batch":
                return RunExportBatch(args);

            case "import-batch":
                return RunImportBatch(args);

            case "autofill":
                return RunAutoFill(args);

            case "reorganize":
                return RunReorganize();

            case "extract-quests":
                return RunExtractQuests(args, sqpackPath);

            case "discover":
                return RunDiscover(sqpackPath);

            case "list":
                ListExtractors();
                return 0;

            case "inspect":
                return RunInspect(args, sqpackPath);

            case "extract":
                return RunExtract(args, sqpackPath);

            case "search":
                return RunSearch(args, sqpackPath);

            case "validate":
                return args.Length > 1 && args[1].Equals("--review", StringComparison.OrdinalIgnoreCase)
                    ? RunGlossaryReview(args)
                    : RunValidate();

            case "apply-cc":
                return RunApplyCharacterCreation();

            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Comando non riconosciuto: '{args[0]}'");
                Console.ResetColor();
                PrintUsage();
                return 1;
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine(@"
USO:
  dotnet run --project src/FFXIVItalian.Extractor -- <comando> [opzioni]

COMANDI DISPONIBILI:
  status [filtro]               Mostra il cruscotto di avanzamento (es. status, status pending, status system)
  export-batch <foglio> [opt]   Esporta un blocco (batch) di righe pendenti pronte per la traduzione
  import-batch <foglio> <file>  Importa e convalida un batch tradotto nel master JSON
  autofill <foglio|all>         Pre-popola termini identici dal glossario canonico
  reorganize                    Riorganizza i file JSON in sottocartelle tematiche (system/, world/, combat/, items/...)
  extract-quests [esp|all]      Estrae tutti i dialoghi delle quest (all, arr, heavensward, stormblood, etc.)
  list                          Mostra tutti gli estrattori di fogli registrati
  inspect <foglio> [rowId]      Ispeziona la struttura EXH/EXD di un foglio (es. lobby 1704)
  extract <foglio|all>          Estrae il testo originale in JSON preservando le traduzioni esistenti
  search <query>                Cerca un testo in inglese in tutti i fogli supportati
  validate                      Verifica le traduzioni con Glossary.md e SeString
  validate --review [file]      Controlla le categorie nei file completi, oppure in un file specifico

OPZIONI:
  --sqpack <percorso>           Specifica il percorso della cartella sqpack del gioco
  --out <percorso>              Specifica un file o cartella di output personalizzata
  --size <numero>               Dimensione del batch per export-batch (default: 50)

ESEMPI:
  dotnet run --project src/FFXIVItalian.Extractor -- status
  dotnet run --project src/FFXIVItalian.Extractor -- reorganize
  dotnet run --project src/FFXIVItalian.Extractor -- extract-quests arr
  dotnet run --project src/FFXIVItalian.Extractor -- export-batch addon --size 50
  dotnet run --project src/FFXIVItalian.Extractor -- import-batch addon data/batches/addon_batch.json
  dotnet run --project src/FFXIVItalian.Extractor -- autofill placename
  dotnet run --project src/FFXIVItalian.Extractor -- extract all
");
    }

    private static void ListExtractors()
    {
        Console.WriteLine();
        Console.WriteLine($"{"Foglio",-22} {"File JSON Default",-26} {"Descrizione"}");
        Console.WriteLine(new string('-', 85));

        foreach (var ext in ExtractorRegistry.GetAll())
        {
            Console.WriteLine($"{ext.SheetName,-22} {ext.DefaultJsonFileName,-26} {ext.Description}");
        }
        Console.WriteLine();
    }

    private static int RunInspect(string[] args, string sqpackPath)
    {
        if (args.Length < 2)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Specifica il nome del foglio da ispezionare. Es: inspect lobby 1704");
            Console.ResetColor();
            return 1;
        }

        string sheetName = args[1];
        uint? targetRowId = null;

        if (args.Length >= 3 && uint.TryParse(args[2], out uint rId))
        {
            targetRowId = rId;
        }

        var extractor = ExtractorRegistry.Get(sheetName);
        if (extractor == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Estrattore per '{sheetName}' non trovato. Usa 'list' per vedere i fogli supportati.");
            Console.ResetColor();
            return 1;
        }

        if (!Directory.Exists(sqpackPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Percorso sqpack non valido: {sqpackPath}");
            Console.ResetColor();
            return 1;
        }

        var lumina = CreateLumina(sqpackPath);
        extractor.Inspect(lumina, targetRowId);
        return 0;
    }

    private static int RunExtract(string[] args, string sqpackPath)
    {
        if (args.Length < 2)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Specifica il nome del foglio o 'all' per estrarre tutto. Es: extract lobby");
            Console.ResetColor();
            return 1;
        }

        string target = args[1];
        string translationsDir = FindTranslationsDir();

        if (!Directory.Exists(sqpackPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Percorso sqpack non valido: {sqpackPath}");
            Console.ResetColor();
            return 1;
        }

        var lumina = CreateLumina(sqpackPath);

        if (target.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Estrazione di TUTTI i {ExtractorRegistry.GetAll().Count} fogli supportati...");
            int totalExtracted = 0;

            foreach (var ext in ExtractorRegistry.GetAll())
            {
                string jsonPath = TranslationPathResolver.ResolveTargetPath(translationsDir, ext.SheetName, ext.DefaultJsonFileName);
                string relPath = Path.GetRelativePath(translationsDir, jsonPath);
                Console.Write($"  * Estrazione {ext.SheetName,-20} -> {relPath}... ");
                try
                {
                    int count = ext.ExtractAndSave(lumina, jsonPath);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"OK ({count} righe)");
                    Console.ResetColor();
                    totalExtracted += count;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"ERRORE: {ex.Message}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Completato! Totale righe estratte/aggiornate: {totalExtracted:N0}");
            Console.ResetColor();
            return 0;
        }
        else
        {
            var ext = ExtractorRegistry.Get(target);
            if (ext == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Estrattore per '{target}' non trovato. Usa 'list' per vedere i fogli disponibili.");
                Console.ResetColor();
                return 1;
            }

            string jsonPath = TranslationPathResolver.ResolveTargetPath(translationsDir, ext.SheetName, ext.DefaultJsonFileName);
            string relPath = Path.GetRelativePath(translationsDir, jsonPath);
            Console.WriteLine($"Estrazione '{ext.SheetName}' -> {relPath}...");
            try
            {
                int count = ext.ExtractAndSave(lumina, jsonPath);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Completato con successo! {count} righe estratte/aggiornate.");
                Console.ResetColor();
                return 0;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERRORE durante l'estrazione: {ex.Message}");
                Console.ResetColor();
                return 1;
            }
        }
    }

    private static int RunSearch(string[] args, string sqpackPath)
    {
        if (args.Length < 2)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Specifica il testo da cercare. Es: search \"INSTALLATION DETAILS\"");
            Console.ResetColor();
            return 1;
        }

        string query = args[1];
        if (!Directory.Exists(sqpackPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Percorso sqpack non valido: {sqpackPath}");
            Console.ResetColor();
            return 1;
        }

        var lumina = CreateLumina(sqpackPath);
        Console.WriteLine($"Ricerca di \"{query}\" in tutti i fogli master testuali...");
        int totalMatches = 0;
        var rootExl = lumina.GetFile("exd/root.exl");
        if (rootExl == null) return 1;
        var sheetNames = System.Text.Encoding.UTF8.GetString(rootExl.Data)
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(',')[0].Trim())
            .Where(name => name.Length > 0 && !name.Contains('/') && !name.StartsWith("EXLT", StringComparison.OrdinalIgnoreCase));

        foreach (string sheetName in sheetNames)
        {
            var exh = lumina.GetFile($"exd/{sheetName.ToLowerInvariant()}.exh");
            if (exh == null || exh.Data.Length < 0x20) continue;
            var header = exh.Data.AsSpan();
            ushort fixedSize = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(header.Slice(0x06, 2));
            ushort colCount = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(header.Slice(0x08, 2));
            ushort pageCount = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(header.Slice(0x0A, 2));
            var stringOffsets = new List<ushort>();
            for (int c = 0; c < colCount && 0x20 + c * 4 + 4 <= header.Length; c++)
            {
                int pos = 0x20 + c * 4;
                if (System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(header.Slice(pos, 2)) == 0)
                    stringOffsets.Add(System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(header.Slice(pos + 2, 2)));
            }
            int pageTable = 0x20 + colCount * 4;
            for (int p = 0; p < pageCount && pageTable + p * 8 + 4 <= header.Length; p++)
            {
                uint pageId = System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(header.Slice(pageTable + p * 8, 4));
                var exd = lumina.GetFile($"exd/{sheetName.ToLowerInvariant()}_{pageId}_en.exd");
                if (exd == null || exd.Data.Length < 0x20) continue;
                var data = exd.Data.AsSpan();
                int count = (int)(System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(data.Slice(0x08, 4)) / 8);
                for (int i = 0; i < count && 0x20 + i * 8 + 8 <= data.Length; i++)
                {
                    int index = 0x20 + i * 8;
                    uint rId = System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(data.Slice(index, 4));
                    int row = (int)System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(data.Slice(index + 4, 4));
                    if (row < 0 || row + 6 + fixedSize > data.Length) continue;
                    int size = (int)System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(data.Slice(row, 4));
                    int end = Math.Min(data.Length, row + 6 + size);
                    foreach (ushort colOffset in stringOffsets)
                    {
                        int fixedPos = row + 6 + colOffset;
                        if (fixedPos + 4 > row + 6 + fixedSize) continue;
                        int start = row + 6 + fixedSize + (int)System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(data.Slice(fixedPos, 4));
                        if (start < row + 6 + fixedSize || start >= end) continue;
                        int stop = start;
                        while (stop < end && data[stop] != 0) stop++;
                        string value = BaseSheetExtractor.DecodeSeStringPayload(data.Slice(start, stop - start));
                        if (!value.Contains(query, StringComparison.OrdinalIgnoreCase)) continue;
                        totalMatches++;
                        Console.WriteLine($"  [{sheetName}] Riga {rId}, offset {colOffset}: {value.Replace('\r', ' ').Replace('\n', ' ')}");
                    }
                }
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Ricerca completata: {totalMatches} occorrenze trovate.");
        return 0;
    }

    private static int RunValidate()
    {
        string translationsDir = FindTranslationsDir();
        if (!Directory.Exists(translationsDir))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Cartella traduzioni non trovata: {translationsDir}");
            Console.ResetColor();
            return 1;
        }

        var engine = FFXIVItalian.Core.Glossary.GlossaryLoader.CreateCanonicalEngine();
        Console.WriteLine($"Motore Glossario inizializzato ({engine.Entries.Count} regole canoniche).");
        Console.WriteLine($"Validazione di tutti i file JSON in: {translationsDir}");
        Console.WriteLine();

        int totalEntries = 0;
        int totalIssues = 0;

        foreach (var file in Directory.GetFiles(translationsDir, "*.json", SearchOption.AllDirectories))
        {
            string fileName = Path.GetRelativePath(translationsDir, file);
            Console.WriteLine($"--- Analisi: {fileName} ---");
            string content = File.ReadAllText(file);
            using var doc = System.Text.Json.JsonDocument.Parse(content);

            int fileEntries = 0;
            int fileIssues = 0;

            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                string rowId = prop.Name;
                if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.String)
                {
                    string trans = prop.Value.GetString() ?? "";
                    fileEntries++;
                    fileIssues += ValidateSingleEntry(fileName, rowId, "", trans, engine);
                }
                else if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    fileEntries++;
                    var obj = prop.Value;
                    string orig = "";
                    string trans = "";

                    if (obj.TryGetProperty("name", out var np)) orig = np.GetString() ?? "";
                    if (obj.TryGetProperty("original", out var op)) orig = op.GetString() ?? "";
                    if (obj.TryGetProperty("translation", out var tp)) trans = tp.GetString() ?? "";
                    if (obj.TryGetProperty("translation_name", out var tnp)) trans = tnp.GetString() ?? "";

                    fileIssues += ValidateSingleEntry(fileName, rowId, orig, trans, engine);

                    if (obj.TryGetProperty("description", out var dp) && obj.TryGetProperty("translation_description", out var tdp))
                    {
                        string dOrig = dp.GetString() ?? "";
                        string dTrans = tdp.GetString() ?? "";
                        fileIssues += ValidateSingleEntry(fileName, $"{rowId} (desc)", dOrig, dTrans, engine);
                    }
                }
            }

            if (fileIssues == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  [OK] {fileEntries} righe analizzate - Nessun errore riscontrato.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  [ATTENZIONE] {fileIssues} problemi rilevati su {fileEntries} righe.");
                Console.ResetColor();
            }

            totalEntries += fileEntries;
            totalIssues += fileIssues;
            Console.WriteLine();
        }

        Console.WriteLine("==================================================");
        if (totalIssues == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"CONVALIDA SUPERATA CON SUCCESSO! {totalEntries} righe conformi a Glossary.md e SeString.");
            Console.ResetColor();
            return 0;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"CONVALIDA COMPLETATA: Trovati {totalIssues} avvisi su {totalEntries} righe.");
            Console.ResetColor();
            return 0;
        }
    }

    private static int RunGlossaryReview(string[] args)
    {
        var translationsDir = FindTranslationsDir();
        var catalog = FFXIVItalian.Core.Glossary.GlossaryLoader.LoadCanonical();
        string[] files;
        if (args.Length > 2)
        {
            var requested = args[2];
            var path = File.Exists(requested) ? requested : Path.Combine(translationsDir, requested);
            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"File non trovato: {requested}");
                return 1;
            }
            files = [path];
        }
        else files = Directory.GetFiles(translationsDir, "*.json", SearchOption.AllDirectories);

        int scanned = 0, complete = 0, findings = 0;
        foreach (var file in files)
        {
            var relative = Path.GetRelativePath(translationsDir, file).Replace('\\', '/');
            scanned++;
            FFXIVItalian.Core.Glossary.GlossaryFileAudit audit;
            try { audit = FFXIVItalian.Core.Glossary.GlossaryAudit.AuditFile(file, catalog.Engine); }
            catch (Exception ex) when (ex is IOException or System.Text.Json.JsonException or FormatException)
            {
                Console.Error.WriteLine($"{relative}: {ex.Message}");
                return 1;
            }
            if (!audit.IsComplete)
            {
                if (args.Length > 2) Console.WriteLine($"{relative}: traduzione incompleta o senza testo originale verificabile.");
                continue;
            }
            complete++;
            foreach (var issue in audit.Findings)
            {
                findings++;
                static string Preview(string value) => value.Length <= 180 ? value : value[..180] + "…";
                Console.WriteLine($"{relative}#{issue.RowId}:{issue.Field}: originale={Preview(issue.Original)} | traduzione={Preview(issue.Translation)} | proposta={issue.Message}");
            }
        }
        Console.WriteLine($"Revisione terminologica: {complete} file completi su {scanned} esaminati; {findings} suggerimenti da verificare.");
        return 0;
    }

    private static int ValidateSingleEntry(string file, string rowId, string orig, string trans, FFXIVItalian.Core.Glossary.GlossaryEngine engine)
    {
        if (string.IsNullOrWhiteSpace(trans)) return 0;
        int issues = 0;

        // 1. Check SeString Syntax
        var seResult = FFXIVItalian.Core.SeString.SeStringValidator.Validate(orig, trans);
        foreach (var err in seResult.Errors)
        {
            issues++;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [ERRORE SINTASSI] {file} Riga {rowId}: {err}");
            Console.ResetColor();
        }

        // 2. Check Glossary Compliance
        var glResult = engine.ValidateTranslation(orig, trans);
        foreach (var pro in glResult.ProhibitedUsages)
        {
            issues++;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [FORMA VIETATA] {file} Riga {rowId}: {pro}");
            Console.ResetColor();
        }

        foreach (var warn in glResult.Warnings)
        {
            issues++;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  [AVVISO GLOSSARIO] {file} Riga {rowId}: {warn}");
            Console.ResetColor();
        }

        return issues;
    }

    private static GameData CreateLumina(string sqpackPath)
    {
        return new GameData(sqpackPath, new LuminaOptions
        {
            DefaultExcelLanguage = Language.English
        });
    }

    private static string FindSqPackPath(string[] args)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i].Equals("--sqpack", StringComparison.OrdinalIgnoreCase))
            {
                return args[i + 1];
            }
        }

        if (Directory.Exists(DefaultSqPackPath))
        {
            return DefaultSqPackPath;
        }

        // Common alternative locations
        string[] fallbacks =
        [
            @"C:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack",
            @"D:\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack"
        ];

        foreach (var fb in fallbacks)
        {
            if (Directory.Exists(fb)) return fb;
        }

        return DefaultSqPackPath;
    }

    private static string FindTranslationsDir()
    {
        string dir = Directory.GetCurrentDirectory();
        while (dir != null && !Directory.Exists(Path.Combine(dir, "data", "translations")))
        {
            var parent = Directory.GetParent(dir);
            if (parent == null) break;
            dir = parent.FullName;
        }

        if (dir != null && Directory.Exists(Path.Combine(dir, "data", "translations")))
        {
            return Path.Combine(dir, "data", "translations");
        }

        // Fallback relative
        return Path.Combine(Directory.GetCurrentDirectory(), "data", "translations");
    }

    private static int RunApplyCharacterCreation()
    {
        string transDir = FindTranslationsDir();
        string lobbyJson = TranslationPathResolver.FindFile(transDir, "lobby");
        Console.WriteLine($"Applicazione traduzioni Creazione del Personaggio a: {lobbyJson}");
        try
        {
            CharacterCreationTranslations.ApplyTo(lobbyJson);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Traduzioni della Creazione del Personaggio applicate con successo!");
            Console.ResetColor();
            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Errore: {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }

    private static int RunStatus(string[] args)
    {
        string translationsDir = FindTranslationsDir();
        string? filter = args.Length > 1 ? args[1] : null;
        BatchManager.PrintStatus(translationsDir, filter);
        return 0;
    }

    private static int RunExportBatch(string[] args)
    {
        if (args.Length < 2)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Uso: export-batch <foglio> [--size <N>] [--out <percorso>]");
            Console.WriteLine("Es:  export-batch addon --size 50");
            Console.ResetColor();
            return 1;
        }

        string sheetName = args[1];
        int batchSize = 50;
        string? customOut = null;

        for (int i = 2; i < args.Length; i++)
        {
            if (args[i].Equals("--size", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length && int.TryParse(args[i + 1], out int s))
            {
                batchSize = s;
                i++;
            }
            else if (args[i].Equals("--out", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                customOut = args[i + 1];
                i++;
            }
        }

        string translationsDir = FindTranslationsDir();
        try
        {
            var (count, outFile) = BatchManager.ExportBatch(translationsDir, sheetName, batchSize, customOut);
            if (count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Nessuna riga pendente trovata per il foglio '{sheetName}'. Il foglio è già interamente tradotto!");
                Console.ResetColor();
                return 0;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Esportato batch di {count} righe in: {outFile}");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("SUGGERIMENTO: Fornisci questo file JSON al prompt di traduzione assieme a docs/TRANSLATION_PROMPT.md.");
            Console.WriteLine($"Al termine, importa le traduzioni con: dotnet run --project src/FFXIVItalian.Extractor -- import-batch {sheetName} \"{outFile}\"");
            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Errore durante l'esportazione del batch: {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }

    private static int RunImportBatch(string[] args)
    {
        if (args.Length < 3)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Uso: import-batch <foglio> <percorso_file_batch.json>");
            Console.WriteLine("Es:  import-batch addon data/batches/addon_batch_1.json");
            Console.ResetColor();
            return 1;
        }

        string sheetName = args[1];
        string batchPath = args[2];
        string translationsDir = FindTranslationsDir();

        try
        {
            var engine = FFXIVItalian.Core.Glossary.GlossaryLoader.CreateCanonicalEngine();
            var (updated, syntaxErrors, glossaryWarnings) = BatchManager.ImportBatch(translationsDir, sheetName, batchPath, engine);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Importazione completata con successo! Aggiornate {updated} righe nel master JSON.");
            Console.ResetColor();

            if (syntaxErrors > 0 || glossaryWarnings > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Avvisi rilevati durante l'importazione: {syntaxErrors} errori SeString, {glossaryWarnings} avvisi glossario.");
                Console.ResetColor();
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Errore durante l'importazione del batch: {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }

    private static int RunAutoFill(string[] args)
    {
        if (args.Length < 2)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Uso: autofill <foglio|all>");
            Console.ResetColor();
            return 1;
        }

        string target = args[1];
        string translationsDir = FindTranslationsDir();
        var engine = FFXIVItalian.Core.Glossary.GlossaryLoader.CreateCanonicalEngine();

        if (target.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            int total = 0;
            foreach (var ext in ExtractorRegistry.GetAll())
            {
                int count = BatchManager.AutoFillGlossary(translationsDir, ext.SheetName, engine);
                if (count > 0)
                {
                    Console.WriteLine($"  * {ext.SheetName}: {count} termini canonici pre-popolati dal glossario.");
                    total += count;
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Autocompilazione completata! Totale termini pre-popolati: {total}");
            Console.ResetColor();
            return 0;
        }
        else
        {
            int count = BatchManager.AutoFillGlossary(translationsDir, target, engine);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Completato! {count} termini canonici pre-popolati dal glossario per '{target}'.");
            Console.ResetColor();
            return 0;
        }
    }

    private static int RunReorganize()
    {
        string translationsDir = FindTranslationsDir();
        Console.WriteLine($"Riorganizzazione file JSON in sottocartelle tematiche in: {translationsDir}");
        int count = TranslationPathResolver.Reorganize(translationsDir);
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Riorganizzazione completata! {count} file spostati nelle rispettive cartelle.");
        Console.ResetColor();
        return 0;
    }

    private static int RunExtractQuests(string[] args, string sqpackPath)
    {
        string filter = args.Length > 1 ? args[1] : "all";
        string translationsDir = FindTranslationsDir();

        if (!Directory.Exists(sqpackPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Percorso sqpack non valido: {sqpackPath}");
            Console.ResetColor();
            return 1;
        }

        var lumina = CreateLumina(sqpackPath);
        Console.WriteLine($"Estrazione Quest narrative (filtro: '{filter}')...");
        Console.WriteLine($"Destinazione: {Path.Combine(translationsDir, "quests")}");
        Console.WriteLine();

        var sw = System.Diagnostics.Stopwatch.StartNew();

        int lastReportedPercent = -1;
        var (quests, rows) = QuestExtractor.ExtractQuests(lumina, filter, translationsDir, (done, total, current) =>
        {
            int pct = (int)((double)done / total * 100);
            if (pct != lastReportedPercent && pct % 5 == 0)
            {
                lastReportedPercent = pct;
                Console.Write($"\rProgresso: [{pct,3}%] ({done}/{total} missioni estratte)...      ");
            }
        });

        sw.Stop();
        Console.WriteLine();
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Estrazione Quest completata con successo!");
        Console.WriteLine($"  * Missioni estratte: {quests:N0}");
        Console.WriteLine($"  * Righe di dialogo:  {rows:N0}");
        Console.WriteLine($"  * Tempo impiegato:   {sw.Elapsed.TotalSeconds:F1}s");
        Console.ResetColor();
        return 0;
    }

    private static int RunDiscover(string sqpackPath)
    {
        var lumina = CreateLumina(sqpackPath);
        var rootExl = lumina.GetFile("exd/root.exl");
        if (rootExl == null)
        {
            Console.WriteLine("exd/root.exl non trovato.");
            return 1;
        }

        string text = System.Text.Encoding.UTF8.GetString(rootExl.Data);
        var lines = text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine($"Trovate {lines.Length} definizioni di fogli in exd/root.exl.");

        int textSheets = 0;
        var textSheetNames = new List<(string Name, int StrCols, int TotalCols)>();

        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length < 1) continue;
            string sheetName = parts[0].Trim();
            if (string.IsNullOrEmpty(sheetName) || sheetName.StartsWith("EXLT", StringComparison.OrdinalIgnoreCase)) continue;

            var exh = lumina.GetFile($"exd/{sheetName.ToLowerInvariant()}.exh");
            if (exh == null || exh.Data.Length < 0x20) continue;

            ushort colCount = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(0x08, 2));
            ushort langCount = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(0x0C, 2));

            // Check how many string columns
            int strCols = 0;
            for (int c = 0; c < colCount; c++)
            {
                int colPos = 0x20 + (c * 4);
                if (colPos + 4 > exh.Data.Length) break;
                ushort type = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(colPos, 2));
                if (type == 0) strCols++;
            }

            if (strCols > 0 && langCount > 1)
            {
                textSheets++;
                textSheetNames.Add((sheetName, strCols, colCount));
            }
        }

        int questSheets = textSheetNames.Count(s => s.Name.StartsWith("quest/", StringComparison.OrdinalIgnoreCase));
        int talkSheets = textSheetNames.Count(s => s.Name.Contains("talk", StringComparison.OrdinalIgnoreCase) || s.Name.StartsWith("balloon", StringComparison.OrdinalIgnoreCase));
        int warpRaidSheets = textSheetNames.Count(s => s.Name.StartsWith("warp/", StringComparison.OrdinalIgnoreCase) || s.Name.StartsWith("raid/", StringComparison.OrdinalIgnoreCase) || s.Name.StartsWith("transport/", StringComparison.OrdinalIgnoreCase) || s.Name.StartsWith("shop/", StringComparison.OrdinalIgnoreCase));
        int generalSheets = textSheets - questSheets - talkSheets - warpRaidSheets;

        Console.WriteLine();
        Console.WriteLine("=========================================================================");
        Console.WriteLine(" CENSIMENTO COMPLETO DEI FOGLI CON TESTO IN FFXIV (root.exl)");
        Console.WriteLine("=========================================================================");
        Console.WriteLine($"Totale fogli con testo multilingua nel client: {textSheets}");
        Console.WriteLine($"  * Fogli di Dialogo delle Quest (MSQ, Job, Secondarie): {questSheets}");
        Console.WriteLine($"  * Fogli di Dialogo Ambientale e Fumetti (Talk/Balloon):  {talkSheets}");
        Console.WriteLine($"  * Fogli di Interazione / Negozi / Viaggi (Warp/Shop):     {warpRaidSheets}");
        Console.WriteLine($"  * Fogli Master di Gioco (UI, Oggetti, Abilità, Zone...):  {generalSheets}");
        Console.WriteLine("=========================================================================");
        Console.WriteLine();
        Console.WriteLine("Esempi di Fogli Master importanti:");
        foreach (var s in textSheetNames.Where(s => !s.Name.Contains('/')).Take(35))
        {
            Console.WriteLine($"  - {s.Name,-30} ({s.StrCols} col. testo su {s.TotalCols})");
        }

        var knownSheets = ExtractorRegistry.GetNames().ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missingMasterSheets = textSheetNames
            .Where(s => !s.Name.Contains('/') && !knownSheets.Contains(s.Name))
            .OrderBy(s => s.Name)
            .ToList();
        Console.WriteLine($"Fogli master testuali non registrati: {missingMasterSheets.Count}");
        foreach (var s in missingMasterSheets)
            Console.WriteLine($"  + {s.Name,-30} ({s.StrCols} col. testo su {s.TotalCols})");

        // Quest folder breakdown
        var questFolders = textSheetNames
            .Where(s => s.Name.StartsWith("quest/", StringComparison.OrdinalIgnoreCase))
            .GroupBy(s => s.Name.Split('/')[1])
            .OrderBy(g => g.Key)
            .ToList();

        Console.WriteLine($"Sottocartelle Quest trovate: {questFolders.Count}");
        foreach (var qf in questFolders)
        {
            var firstSample = qf.First().Name;
            Console.WriteLine($"  - quest/{qf.Key,-6} ({qf.Count(),3} missioni)  Es: {firstSample}");
        }
        Console.WriteLine("=========================================================================");

        return 0;
    }
}
