using FFXIVItalian.Core.Packaging;
using FFXIVItalian.Core.Translation;
using Lumina;
using Lumina.Data;
using System.Text;

Console.WriteLine("==================================================");
Console.WriteLine(" FFXIV Italian - Penumbra Mod Packager (.pmp)");
Console.WriteLine("==================================================");

string baseDir = AppContext.BaseDirectory;
// Find project root (looking for data/translations)
string projectRoot = Directory.GetCurrentDirectory();
if (!Directory.Exists(Path.Combine(projectRoot, "data", "translations")))
{
    var parent = Directory.GetParent(projectRoot);
    while (parent != null && !Directory.Exists(Path.Combine(parent.FullName, "data", "translations")))
    {
        parent = parent.Parent;
    }
    if (parent != null)
    {
        projectRoot = parent.FullName;
    }
}

string translationsDir = Path.Combine(projectRoot, "data", "translations");
string sqPackPath = @"G:\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack";
string penumbraModDir = @"G:\SquareEnix\FFXIV_Mod\FFXIV Italiano (Test In-Game)";
string outputPmp = Path.Combine(projectRoot, "FFXIV_Italian.pmp");

if (args.Length > 0 && Directory.Exists(args[0]))
{
    sqPackPath = args[0];
}

Console.WriteLine($"Cartella Traduzioni: {translationsDir}");
Console.WriteLine($"Percorso SqPack gioco: {sqPackPath}");
Console.WriteLine($"Percorso Mod Penumbra: {penumbraModDir}");

// Verifica e configura Dalamud per sospendere il gioco finché i plugin (Penumbra) non sono caricati
Console.WriteLine();
Console.WriteLine("Verifica configurazione Dalamud (IsResumeGameAfterPluginLoad)...");
var dalamudCheck = DalamudConfigService.EnsureResumeGameAfterPluginLoad();
Console.WriteLine($"  [Dalamud] {dalamudCheck.Message}");

var meta = new PenumbraMeta
{
    Name = "FFXIV Italiano (Test In-Game)",
    Author = "FFXIV Italian Community",
    Description = "Localizzazione italiana professionale: Schermata del Titolo (START, EXIT, CONFIGURAZIONE), Creazione Personaggio e Interfaccia di Sistema.",
    Version = "0.2.0-title-test",
    Website = "https://github.com/IsamuBarbieri/FFXIV_Italian"
};

var fileMap = new Dictionary<string, byte[]>();

if (Directory.Exists(sqPackPath))
{
    try
    {
        var lumina = new GameData(sqPackPath, new LuminaOptions
        {
            DefaultExcelLanguage = Language.English
        });

        // 1. PATCH LOBBY (Schermata del Titolo e Creazione PG)
        string lobbyJsonPath = Path.Combine(translationsDir, "lobby.json");
        var lobbyReplacements = TranslationFileReader.LoadReplacements(lobbyJsonPath);
        if (lobbyReplacements.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"Caricate {lobbyReplacements.Count} traduzioni da '{Path.GetFileName(lobbyJsonPath)}'.");
            var originalLobbyExd = lumina.GetFile("exd/lobby_0_en.exd");
            if (originalLobbyExd != null)
            {
                var lobbyMulti = lobbyReplacements.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (IReadOnlyDictionary<int, string>)new Dictionary<int, string> { [0] = kvp.Value });

                byte[] patchedLobbyExd = ExdPatcher.PatchMultiColumnStringSheet(
                    originalLobbyExd.Data,
                    fixedDataSize: 24,
                    stringColumnOffsets: [0, 4, 8],
                    lobbyMulti);

                fileMap["exd/lobby_0_en.exd"] = patchedLobbyExd;
                Console.WriteLine($"  * 'exd/lobby_0_en.exd' rigenerato ({patchedLobbyExd.Length:N0} byte).");
            }
        }

        // 2. PATCH ADDON (Popup di conferma, pulsanti e menu generali)
        string addonJsonPath = Path.Combine(translationsDir, "addon.json");
        var addonReplacements = TranslationFileReader.LoadReplacements(addonJsonPath);
        if (addonReplacements.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"Caricate {addonReplacements.Count} traduzioni da '{Path.GetFileName(addonJsonPath)}'.");
            var originalAddonExd = lumina.GetFile("exd/addon_0_en.exd");
            if (originalAddonExd != null)
            {
                byte[] patchedAddonExd = ExdPatcher.PatchSimpleStringSheet(
                    originalAddonExd.Data,
                    fixedDataSize: 4,
                    stringColumnOffset: 0,
                    addonReplacements);

                fileMap["exd/addon_0_en.exd"] = patchedAddonExd;
                Console.WriteLine($"  * 'exd/addon_0_en.exd' rigenerato ({patchedAddonExd.Length:N0} byte).");
            }
        }
        // 3. PATCH MAINCOMMANDCATEGORY (Le macro-categorie del menu principale)
        string catJsonPath = Path.Combine(translationsDir, "maincommandcategory.json");
        var catReplacements = TranslationFileReader.LoadReplacements(catJsonPath);
        if (catReplacements.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"Caricate {catReplacements.Count} traduzioni da '{Path.GetFileName(catJsonPath)}'.");
            var originalCatExd = lumina.GetFile("exd/maincommandcategory_0_en.exd");
            if (originalCatExd != null)
            {
                byte[] patchedCatExd = ExdPatcher.PatchSimpleStringSheet(
                    originalCatExd.Data,
                    fixedDataSize: 8,
                    stringColumnOffset: 0,
                    catReplacements);

                fileMap["exd/maincommandcategory_0_en.exd"] = patchedCatExd;
                Console.WriteLine($"  * 'exd/maincommandcategory_0_en.exd' rigenerato ({patchedCatExd.Length:N0} byte).");
            }
        }

        // 4. PATCH MAINCOMMAND (I 100 comandi e descrizioni del menu principale)
        string cmdJsonPath = Path.Combine(translationsDir, "maincommand.json");
        var cmdReplacements = TranslationFileReader.LoadTwoStringReplacements(cmdJsonPath);
        if (cmdReplacements.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"Caricate {cmdReplacements.Count} traduzioni da '{Path.GetFileName(cmdJsonPath)}'.");
            var originalCmdExd = lumina.GetFile("exd/maincommand_0_en.exd");
            if (originalCmdExd != null)
            {
                byte[] patchedCmdExd = ExdPatcher.PatchTwoStringSheet(
                    originalCmdExd.Data,
                    fixedDataSize: 16,
                    string1ColumnOffset: 0,
                    string2ColumnOffset: 4,
                    cmdReplacements);

                fileMap["exd/maincommand_0_en.exd"] = patchedCmdExd;
                Console.WriteLine($"  * 'exd/maincommand_0_en.exd' rigenerato ({patchedCmdExd.Length:N0} byte).");
            }
        }

        // 5. PATCH ERROR (Messaggi di connessione, server, coda e lobby)
        string errJsonPath = Path.Combine(translationsDir, "error.json");
        var errReplacements = TranslationFileReader.LoadReplacements(errJsonPath);
        if (errReplacements.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"Caricate {errReplacements.Count} traduzioni da '{Path.GetFileName(errJsonPath)}'.");
            var originalErrExd = lumina.GetFile("exd/error_0_en.exd");
            if (originalErrExd != null)
            {
                byte[] patchedErrExd = ExdPatcher.PatchSimpleStringSheet(
                    originalErrExd.Data,
                    fixedDataSize: 4,
                    stringColumnOffset: 0,
                    errReplacements);

                fileMap["exd/error_0_en.exd"] = patchedErrExd;
                Console.WriteLine($"  * 'exd/error_0_en.exd' rigenerato ({patchedErrExd.Length:N0} byte).");
            }
        }

        // 6. PATCH CLASSJOB (Nomi di classi e job, inclusa la schermata di selezione personaggio)
        string classJobJsonPath = Path.Combine(translationsDir, "classjob.json");
        var classJobReplacements = TranslationFileReader.LoadReplacements(classJobJsonPath);
        if (classJobReplacements.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"Caricate {classJobReplacements.Count} traduzioni da '{Path.GetFileName(classJobJsonPath)}'.");
            var originalClassJobExd = lumina.GetFile("exd/classjob_0_en.exd");
            if (originalClassJobExd != null)
            {
                var classJobMulti = classJobReplacements.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (IReadOnlyDictionary<int, string>)new Dictionary<int, string> { [16] = kvp.Value });

                byte[] patchedClassJobExd = ExdPatcher.PatchMultiColumnStringSheet(
                    originalClassJobExd.Data,
                    fixedDataSize: 112,
                    stringColumnOffsets: [0, 4, 8, 16],
                    classJobMulti);

                fileMap["exd/classjob_0_en.exd"] = patchedClassJobExd;
                Console.WriteLine($"  * 'exd/classjob_0_en.exd' rigenerato ({patchedClassJobExd.Length:N0} byte).");
            }
        }

        // 7. PATCH PLACENAME (Nomi dei luoghi e zone, inclusa la schermata di selezione personaggio)
        string placeJsonPath = Path.Combine(translationsDir, "placename.json");
        var placeReplacements = TranslationFileReader.LoadReplacements(placeJsonPath);
        if (placeReplacements.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"Caricate {placeReplacements.Count} traduzioni da '{Path.GetFileName(placeJsonPath)}'.");
            var originalPlaceExd = lumina.GetFile("exd/placename_0_en.exd");
            if (originalPlaceExd != null)
            {
                var placeMulti = placeReplacements.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (IReadOnlyDictionary<int, string>)new Dictionary<int, string> { [0] = kvp.Value, [4] = kvp.Value });

                byte[] patchedPlaceExd = ExdPatcher.PatchMultiColumnStringSheet(
                    originalPlaceExd.Data,
                    fixedDataSize: 24,
                    stringColumnOffsets: [0, 4, 8],
                    placeMulti);

                fileMap["exd/placename_0_en.exd"] = patchedPlaceExd;
                Console.WriteLine($"  * 'exd/placename_0_en.exd' rigenerato ({patchedPlaceExd.Length:N0} byte).");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERRORE durante il patching]: {ex.Message}");
    }
}

fileMap["readme_ita.txt"] = Encoding.UTF8.GetBytes("FFXIV Italiano - Compilato da file JSON in data/translations.");

// A. Compilazione pacchetto .pmp per distribuzione
Console.WriteLine();
Console.WriteLine($"Generazione archivio Penumbra ({Path.GetFileName(outputPmp)})...");
await PenumbraPackager.CreatePmpPackageAsync(outputPmp, meta, fileMap);

// B. Scrittura DIRETTA nella cartella attiva di Penumbra dell'utente (Hot-Reload istantaneo!)
if (Directory.Exists(penumbraModDir))
{
    Console.WriteLine($"Aggiornamento DIRETTO nella cartella attiva di Penumbra: {penumbraModDir}...");
    var exdDir = Path.Combine(penumbraModDir, "exd");
    if (!Directory.Exists(exdDir))
    {
        Directory.CreateDirectory(exdDir);
    }

    foreach (var (gamePath, data) in fileMap)
    {
        var destFile = Path.Combine(penumbraModDir, gamePath.Replace('/', Path.DirectorySeparatorChar));
        var parentDir = Path.GetDirectoryName(destFile);
        if (!string.IsNullOrEmpty(parentDir) && !Directory.Exists(parentDir))
        {
            Directory.CreateDirectory(parentDir);
        }
        await File.WriteAllBytesAsync(destFile, data);
        Console.WriteLine($"  * Scritto: {gamePath} ({data.Length:N0} byte)");
    }

    // Preserve existing Identifier GUID from Penumbra
    string? existingIdentifier = null;
    string existingMetaPath = Path.Combine(penumbraModDir, "meta.json");
    if (File.Exists(existingMetaPath))
    {
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(existingMetaPath));
            if (doc.RootElement.TryGetProperty("Identifier", out var idProp) && idProp.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                existingIdentifier = idProp.GetString();
            }
        }
        catch { }
    }

    meta.Identifier = existingIdentifier;
    meta.FileVersion = 4;
    meta.DefaultData.Files.Clear();
    var defaultMod = new PenumbraDefaultMod();

    foreach (var key in fileMap.Keys)
    {
        string normKey = key.Replace('\\', '/');
        meta.DefaultData.Files[normKey] = normKey;
        defaultMod.Files[normKey] = normKey;
    }

    var jsonOptions = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
    string penumbraMetaJson = System.Text.Json.JsonSerializer.Serialize(meta, jsonOptions);
    await File.WriteAllTextAsync(Path.Combine(penumbraModDir, "meta.json"), penumbraMetaJson);
    Console.WriteLine($"  * Scritto: meta.json ({meta.DefaultData.Files.Count} file registrati)");

    string defaultModJson = System.Text.Json.JsonSerializer.Serialize(defaultMod, jsonOptions);
    await File.WriteAllTextAsync(Path.Combine(penumbraModDir, "default_mod.json"), defaultModJson);
    Console.WriteLine($"  * Scritto: default_mod.json ({defaultMod.Files.Count} file registrati)");
}

Console.WriteLine();
Console.WriteLine("==================================================");
Console.WriteLine("REBUILD COMPLETATO CON SUCCESSO!");
Console.WriteLine("Tutti i 7 fogli EXD e i metadati Penumbra v4 sono pronti.");
Console.WriteLine("IMPORTANTE: Riavvia il gioco FINAL FANTASY XIV per applicare i fogli EXD.");
Console.WriteLine("(I file EXD vengono memorizzati nella RAM del processo FFXIV al caricamento)");
Console.WriteLine("==================================================");

