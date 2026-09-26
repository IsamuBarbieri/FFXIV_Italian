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
        string lobbyJsonPath = TranslationPathResolver.FindFile(translationsDir, "lobby");
        var lobbyMulti = TranslationFileReader.LoadLobbyReplacements(lobbyJsonPath);
        if (lobbyMulti.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"Caricate {lobbyMulti.Count} traduzioni da '{Path.GetFileName(lobbyJsonPath)}'.");
            var originalLobbyExd = lumina.GetFile("exd/lobby_0_en.exd");
            if (originalLobbyExd != null)
            {
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
        string addonJsonPath = TranslationPathResolver.FindFile(translationsDir, "addon");
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

        // 2b. PATCH GRAND COMPANY (nomi risolti dal macro GrandCompany)
        string grandCompanyJsonPath = TranslationPathResolver.FindFile(translationsDir, "grandcompany");
        var grandCompanyReplacements = TranslationFileReader.LoadReplacements(grandCompanyJsonPath);
        if (grandCompanyReplacements.Count > 0)
        {
            var originalGrandCompanyExd = lumina.GetFile("exd/grandcompany_0_en.exd");
            if (originalGrandCompanyExd != null)
            {
                var grandCompanyColumns = grandCompanyReplacements.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (IReadOnlyDictionary<int, string>)new Dictionary<int, string> { [0] = kvp.Value });
                byte[] patchedGrandCompanyExd = ExdPatcher.PatchMultiColumnStringSheet(
                    originalGrandCompanyExd.Data,
                    fixedDataSize: 24,
                    stringColumnOffsets: [0, 4, 8],
                    grandCompanyColumns);
                fileMap["exd/grandcompany_0_en.exd"] = patchedGrandCompanyExd;
                Console.WriteLine($"  * 'exd/grandcompany_0_en.exd' rigenerato ({patchedGrandCompanyExd.Length:N0} byte).");
            }
        }

        // 2c. PATCH FC REPUTATION (gradi risolti dal macro FCReputation)
        string fcReputationJsonPath = TranslationPathResolver.FindFile(translationsDir, "fcreputation");
        var fcReputationReplacements = TranslationFileReader.LoadReplacements(fcReputationJsonPath);
        if (fcReputationReplacements.Count > 0)
        {
            var originalFcReputationExd = lumina.GetFile("exd/fcreputation_0_en.exd");
            if (originalFcReputationExd != null)
            {
                var fcReputationRows = fcReputationReplacements.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (IReadOnlyDictionary<int, string>)new Dictionary<int, string> { [0] = kvp.Value });
                byte[] patchedFcReputationExd = ExdPatcher.PatchMultiColumnStringSheet(
                    originalFcReputationExd.Data,
                    fixedDataSize: 20,
                    stringColumnOffsets: [0],
                    fcReputationRows);
                fileMap["exd/fcreputation_0_en.exd"] = patchedFcReputationExd;
                Console.WriteLine($"  * 'exd/fcreputation_0_en.exd' rigenerato ({patchedFcReputationExd.Length:N0} byte).");
            }
        }

        // 2d. PATCH BEAST REPUTATION RANK (gradi delle società alleate)
        string beastReputationJsonPath = TranslationPathResolver.FindFile(translationsDir, "beastreputationrank");
        var beastReputationReplacements = TranslationFileReader.LoadTwoStringReplacements(beastReputationJsonPath);
        if (beastReputationReplacements.Count > 0)
        {
            var originalBeastReputationExd = lumina.GetFile("exd/beastreputationrank_0_en.exd");
            if (originalBeastReputationExd != null)
            {
                var beastReputationRows = beastReputationReplacements.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (kvp.Value.Name, kvp.Value.Description));
                byte[] patchedBeastReputationExd = ExdPatcher.PatchTwoStringSheet(
                    originalBeastReputationExd.Data,
                    fixedDataSize: 16,
                    string1ColumnOffset: 0,
                    string2ColumnOffset: 4,
                    beastReputationRows);
                fileMap["exd/beastreputationrank_0_en.exd"] = patchedBeastReputationExd;
                Console.WriteLine($"  * 'exd/beastreputationrank_0_en.exd' rigenerato ({patchedBeastReputationExd.Length:N0} byte).");
            }
        }

        // 2e. PATCH GC RANK TEXT (i titoli sono suddivisi per compagnia e genere)
        string[] gcRankSheets =
        [
            "gcranklimsamaletext", "gcranklimsafemaletext",
            "gcrankgridaniamaletext", "gcrankgridaniafemaletext",
            "gcrankuldahmaletext", "gcrankuldahfemaletext"
        ];
        foreach (string sheet in gcRankSheets)
        {
            string rankJsonPath = TranslationPathResolver.FindFile(translationsDir, sheet);
            var replacements = TranslationFileReader.LoadReplacements(rankJsonPath);
            var rankNouns = TranslationFileReader.LoadTextCommandReplacements(rankJsonPath, 8);
            if (replacements.Count == 0 && rankNouns.Count == 0) continue;

            string exdPath = $"exd/{sheet}_0_en.exd";
            var originalExd = lumina.GetFile(exdPath);
            if (originalExd == null) continue;

            var columns = replacements.ToDictionary(
                kvp => kvp.Key,
                kvp => (IReadOnlyDictionary<int, string>)new Dictionary<int, string> { [0] = kvp.Value });
            foreach (var (rowId, nounColumns) in rankNouns)
            {
                var row = columns.TryGetValue(rowId, out var existing)
                    ? new Dictionary<int, string>(existing)
                    : new Dictionary<int, string>();
                row[8] = nounColumns[8];
                columns[rowId] = row;
            }
            byte[] patchedExd = ExdPatcher.PatchMultiColumnStringSheet(
                originalExd.Data,
                fixedDataSize: 24,
                stringColumnOffsets: [0, 4, 8, 12],
                columns);
            fileMap[exdPath] = patchedExd;
            Console.WriteLine($"  * '{exdPath}' rigenerato ({patchedExd.Length:N0} byte).");
        }
        // 3. PATCH MAINCOMMANDCATEGORY (Le macro-categorie del menu principale)
        string catJsonPath = TranslationPathResolver.FindFile(translationsDir, "maincommandcategory");
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
        string cmdJsonPath = TranslationPathResolver.FindFile(translationsDir, "maincommand");
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
        string errJsonPath = TranslationPathResolver.FindFile(translationsDir, "error");
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
        string classJobJsonPath = TranslationPathResolver.FindFile(translationsDir, "classjob");
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
        string placeJsonPath = TranslationPathResolver.FindFile(translationsDir, "placename");
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

// 8. PATCH TRIBE (Nomi dei clan maschile/femminile - schermata di creazione PG)
string tribeJsonPath = TranslationPathResolver.FindFile(translationsDir, "tribe");
var tribeReplacements = TranslationFileReader.LoadTwoColumnNameReplacements(tribeJsonPath);
if (tribeReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {tribeReplacements.Count} traduzioni da '{Path.GetFileName(tribeJsonPath)}'.");
    var lumina8 = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalTribeExd = lumina8.GetFile("exd/tribe_0_en.exd");
    if (originalTribeExd != null)
    {
        var tribeDict = tribeReplacements.ToDictionary(
            kvp => kvp.Key,
            kvp => (kvp.Value.Masculine, kvp.Value.Feminine));

        byte[] patchedTribeExd = ExdPatcher.PatchTwoStringSheet(
            originalTribeExd.Data,
            fixedDataSize: 16,
            string1ColumnOffset: 0,
            string2ColumnOffset: 4,
            tribeDict);

        fileMap["exd/tribe_0_en.exd"] = patchedTribeExd;
        Console.WriteLine($"  * 'exd/tribe_0_en.exd' rigenerato ({patchedTribeExd.Length:N0} byte).");
    }
}

// 9. PATCH RACE (Nomi delle razze maschile/femminile - schermata di creazione PG)
string raceJsonPath = TranslationPathResolver.FindFile(translationsDir, "race");
var raceReplacements = TranslationFileReader.LoadTwoColumnNameReplacements(raceJsonPath);
if (raceReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {raceReplacements.Count} traduzioni da '{Path.GetFileName(raceJsonPath)}'.");
    var lumina9 = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalRaceExd = lumina9.GetFile("exd/race_0_en.exd");
    if (originalRaceExd != null)
    {
        var raceDict = raceReplacements.ToDictionary(
            kvp => kvp.Key,
            kvp => (kvp.Value.Masculine, kvp.Value.Feminine));

        byte[] patchedRaceExd = ExdPatcher.PatchTwoStringSheet(
            originalRaceExd.Data,
            fixedDataSize: 44,
            string1ColumnOffset: 0,
            string2ColumnOffset: 4,
            raceDict);

        fileMap["exd/race_0_en.exd"] = patchedRaceExd;
        Console.WriteLine($"  * 'exd/race_0_en.exd' rigenerato ({patchedRaceExd.Length:N0} byte).");
    }
}

// 10. PATCH HOWTOCATEGORY (Categorie dei tutorial e guide per principianti)
string howToCatJsonPath = TranslationPathResolver.FindFile(translationsDir, "howtocategory");
var howToCatReplacements = TranslationFileReader.LoadReplacements(howToCatJsonPath);
if (howToCatReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {howToCatReplacements.Count} traduzioni da '{Path.GetFileName(howToCatJsonPath)}'.");
    var luminaHowToCat = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalHowToCatExd = luminaHowToCat.GetFile("exd/howtocategory_0_en.exd");
    if (originalHowToCatExd != null)
    {
        byte[] patchedHowToCatExd = ExdPatcher.PatchSimpleStringSheet(
            originalHowToCatExd.Data,
            fixedDataSize: 4,
            stringColumnOffset: 0,
            howToCatReplacements);

        fileMap["exd/howtocategory_0_en.exd"] = patchedHowToCatExd;
        Console.WriteLine($"  * 'exd/howtocategory_0_en.exd' rigenerato ({patchedHowToCatExd.Length:N0} byte).");
    }
}

// 11. PATCH ITEMUICATEGORY (Categorie dell'inventario e dell'armeria)
string itemCatJsonPath = TranslationPathResolver.FindFile(translationsDir, "itemuicategory");
var itemCatReplacements = TranslationFileReader.LoadReplacements(itemCatJsonPath);
if (itemCatReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {itemCatReplacements.Count} traduzioni da '{Path.GetFileName(itemCatJsonPath)}'.");
    var luminaItemCat = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalItemCatExd = luminaItemCat.GetFile("exd/itemuicategory_0_en.exd");
    if (originalItemCatExd != null)
    {
        byte[] patchedItemCatExd = ExdPatcher.PatchSimpleStringSheet(
            originalItemCatExd.Data,
            fixedDataSize: 12,
            stringColumnOffset: 0,
            itemCatReplacements);

        fileMap["exd/itemuicategory_0_en.exd"] = patchedItemCatExd;
        Console.WriteLine($"  * 'exd/itemuicategory_0_en.exd' rigenerato ({patchedItemCatExd.Length:N0} byte).");
    }
}

// 12. PATCH WEATHER (Condizioni meteorologiche di tutte le zone di Eorzea)
string weatherJsonPath = TranslationPathResolver.FindFile(translationsDir, "weather");
var weatherReplacements = TranslationFileReader.LoadTwoStringReplacements(weatherJsonPath);
if (weatherReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {weatherReplacements.Count} traduzioni da '{Path.GetFileName(weatherJsonPath)}'.");
    var luminaWeather = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalWeatherExd = luminaWeather.GetFile("exd/weather_0_en.exd");
    if (originalWeatherExd != null)
    {
        byte[] patchedWeatherExd = ExdPatcher.PatchTwoStringSheet(
            originalWeatherExd.Data,
            fixedDataSize: 28,
            string1ColumnOffset: 0,
            string2ColumnOffset: 4,
            weatherReplacements);

        fileMap["exd/weather_0_en.exd"] = patchedWeatherExd;
        Console.WriteLine($"  * 'exd/weather_0_en.exd' rigenerato ({patchedWeatherExd.Length:N0} byte).");
    }
}

// 13. PATCH HOWTO (Guide e tutorial per principianti)
string howToJsonPath = TranslationPathResolver.FindFile(translationsDir, "howto");
var howToReplacements = TranslationFileReader.LoadReplacements(howToJsonPath);
if (howToReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {howToReplacements.Count} traduzioni da '{Path.GetFileName(howToJsonPath)}'.");
    var luminaHowTo = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalHowToExd = luminaHowTo.GetFile("exd/howto_0_en.exd");
    if (originalHowToExd != null)
    {
        byte[] patchedHowToExd = ExdPatcher.PatchSimpleStringSheet(
            originalHowToExd.Data,
            fixedDataSize: 28,
            stringColumnOffset: 0,
            howToReplacements);

        fileMap["exd/howto_0_en.exd"] = patchedHowToExd;
        Console.WriteLine($"  * 'exd/howto_0_en.exd' rigenerato ({patchedHowToExd.Length:N0} byte).");
    }
}

// 14. PATCH TEXTCOMMAND (Spiegazioni e manuali d'uso dei comandi chat slash)
string textCmdJsonPath = TranslationPathResolver.FindFile(translationsDir, "textcommand");
if (Directory.Exists(sqPackPath))
{
    var luminaTextCmd = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var textCmdExh = luminaTextCmd.GetFile("exd/textcommand.exh");
    if (textCmdExh != null)
    {
        ushort fixedDataSize = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(textCmdExh.Data.AsSpan(0x06, 2));
        ushort columnCount = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(textCmdExh.Data.AsSpan(0x08, 2));
        var stringColumnOffsets = new List<int>();
        for (int column = 0; column < columnCount; column++)
        {
            int columnPos = 0x20 + column * 4;
            ushort type = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(textCmdExh.Data.AsSpan(columnPos, 2));
            ushort offset = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(textCmdExh.Data.AsSpan(columnPos + 2, 2));
            if (type == 0)
                stringColumnOffsets.Add(offset);
        }

        if (stringColumnOffsets.Count < 3)
            throw new InvalidDataException("Il foglio TextCommand non contiene la colonna stringa col_2 attesa.");

        var textCmdReplacements = TranslationFileReader.LoadTextCommandReplacements(textCmdJsonPath, stringColumnOffsets[2]);
        if (textCmdReplacements.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"Caricate {textCmdReplacements.Count} traduzioni da '{Path.GetFileName(textCmdJsonPath)}'.");
            var originalTextCmdExd = luminaTextCmd.GetFile("exd/textcommand_0_en.exd");
            if (originalTextCmdExd != null)
            {
                byte[] patchedTextCmdExd = ExdPatcher.PatchMultiColumnStringSheet(
                    originalTextCmdExd.Data,
                    fixedDataSize,
                    stringColumnOffsets,
                    textCmdReplacements);

                fileMap["exd/textcommand_0_en.exd"] = patchedTextCmdExd;
                Console.WriteLine($"  * 'exd/textcommand_0_en.exd' rigenerato ({patchedTextCmdExd.Length:N0} byte).");
            }
        }

    }
}

// PATCH LOGMESSAGE (Messaggi di sistema e notifiche)
string logMessageJsonPath = TranslationPathResolver.FindFile(translationsDir, "logmessage");
var logMessageReplacements = TranslationFileReader.LoadReplacements(logMessageJsonPath);
if (logMessageReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {logMessageReplacements.Count} traduzioni da '{Path.GetFileName(logMessageJsonPath)}'.");
    var luminaLogMessage = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalLogMessageExd = luminaLogMessage.GetFile("exd/logmessage_0_en.exd");
    if (originalLogMessageExd != null)
    {
        byte[] patchedLogMessageExd = ExdPatcher.PatchSimpleStringSheet(
            originalLogMessageExd.Data,
            // LogMessage EXH: 12-byte fixed row data; its string column is at byte offset 0.
            fixedDataSize: 12,
            stringColumnOffset: 0,
            logMessageReplacements);

        fileMap["exd/logmessage_0_en.exd"] = patchedLogMessageExd;
        Console.WriteLine($"  * 'exd/logmessage_0_en.exd' rigenerato ({patchedLogMessageExd.Length:N0} byte).");
    }
}

// 15. PATCH TRAIT (Nomi dei tratti passivi di classe e job)
string traitJsonPath = TranslationPathResolver.FindFile(translationsDir, "trait");
var traitReplacements = TranslationFileReader.LoadReplacements(traitJsonPath);
if (traitReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {traitReplacements.Count} traduzioni da '{Path.GetFileName(traitJsonPath)}'.");
    var luminaTrait = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalTraitExd = luminaTrait.GetFile("exd/trait_0_en.exd");
    if (originalTraitExd != null)
    {
        byte[] patchedTraitExd = ExdPatcher.PatchSimpleStringSheet(
            originalTraitExd.Data,
            fixedDataSize: 20,
            stringColumnOffset: 0,
            traitReplacements);

        fileMap["exd/trait_0_en.exd"] = patchedTraitExd;
        Console.WriteLine($"  * 'exd/trait_0_en.exd' rigenerato ({patchedTraitExd.Length:N0} byte).");
    }
}

// PATCH ACTIONTRANSIENT (Descrizioni delle azioni nei tooltip)
string actionTransientJsonPath = TranslationPathResolver.FindFile(translationsDir, "actiontransient");
var actionTransientReplacements = TranslationFileReader.LoadReplacements(actionTransientJsonPath);
if (actionTransientReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {actionTransientReplacements.Count} traduzioni da '{Path.GetFileName(actionTransientJsonPath)}'.");
    var luminaActionTransient = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalActionTransientExd = luminaActionTransient.GetFile("exd/actiontransient_0_en.exd");
    if (originalActionTransientExd != null)
    {
        byte[] patchedActionTransientExd = ExdPatcher.PatchSimpleStringSheet(
            originalActionTransientExd.Data,
            fixedDataSize: 4,
            stringColumnOffset: 0,
            actionTransientReplacements);

        fileMap["exd/actiontransient_0_en.exd"] = patchedActionTransientExd;
        Console.WriteLine($"  * 'exd/actiontransient_0_en.exd' rigenerato ({patchedActionTransientExd.Length:N0} byte).");
    }
}

// 16. PATCH TRAITTRANSIENT (Descrizioni e tooltip dei tratti passivi)
string traitTransientJsonPath = TranslationPathResolver.FindFile(translationsDir, "traittransient");
var traitTransientReplacements = TranslationFileReader.LoadReplacements(traitTransientJsonPath);
if (traitTransientReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {traitTransientReplacements.Count} traduzioni da '{Path.GetFileName(traitTransientJsonPath)}'.");
    var luminaTraitTrans = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalTraitTransExd = luminaTraitTrans.GetFile("exd/traittransient_0_en.exd");
    if (originalTraitTransExd != null)
    {
        byte[] patchedTraitTransExd = ExdPatcher.PatchSimpleStringSheet(
            originalTraitTransExd.Data,
            fixedDataSize: 4,
            stringColumnOffset: 0,
            traitTransientReplacements);

        fileMap["exd/traittransient_0_en.exd"] = patchedTraitTransExd;
        Console.WriteLine($"  * 'exd/traittransient_0_en.exd' rigenerato ({patchedTraitTransExd.Length:N0} byte).");
    }
}

// 17. PATCH TITLE (Titoli onorifici dei personaggi - maschile e femminile)
string titleJsonPath = TranslationPathResolver.FindFile(translationsDir, "title");
var titleReplacements = TranslationFileReader.LoadTwoStringReplacements(titleJsonPath);
if (titleReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {titleReplacements.Count} traduzioni da '{Path.GetFileName(titleJsonPath)}'.");
    var luminaTitle = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalTitleExd = luminaTitle.GetFile("exd/title_0_en.exd");
    if (originalTitleExd != null)
    {
        byte[] patchedTitleExd = ExdPatcher.PatchTwoStringSheet(
            originalTitleExd.Data,
            fixedDataSize: 16,
            string1ColumnOffset: 0,
            string2ColumnOffset: 4,
            titleReplacements);

        fileMap["exd/title_0_en.exd"] = patchedTitleExd;
        Console.WriteLine($"  * 'exd/title_0_en.exd' rigenerato ({patchedTitleExd.Length:N0} byte).");
    }
}

// 18. PATCH CUSTOMTALK (Dialoghi brevi e opzioni menu NPC)
string customTalkJsonPath = TranslationPathResolver.FindFile(translationsDir, "customtalk");
var customTalkReplacements = TranslationFileReader.LoadCustomTalkReplacements(customTalkJsonPath);
if (customTalkReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {customTalkReplacements.Count} traduzioni da '{Path.GetFileName(customTalkJsonPath)}'.");
    var luminaCustomTalk = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalCustomTalkExd = luminaCustomTalk.GetFile("exd/customtalk_720896_en.exd");
    if (originalCustomTalkExd != null)
    {
        byte[] patchedCustomTalkExd = ExdPatcher.PatchMultiColumnStringSheet(
            originalCustomTalkExd.Data,
            fixedDataSize: 268,
            stringColumnOffsets: [
                248, 0, 8, 16, 24, 32, 40, 48, 56, 64, 72, 80, 88, 96, 104, 112, 120, 128, 136, 144, 152, 160, 168, 176, 184, 192, 200, 208, 216, 224, 232, 240, 244
            ],
            customTalkReplacements);

        fileMap["exd/customtalk_720896_en.exd"] = patchedCustomTalkExd;
        Console.WriteLine($"  * 'exd/customtalk_720896_en.exd' rigenerato ({patchedCustomTalkExd.Length:N0} byte).");
    }
}

// 19. PATCH STATUS (Effetti di stato, buff e debuff di combattimento)
string statusJsonPath = TranslationPathResolver.FindFile(translationsDir, "status");
var statusReplacements = TranslationFileReader.LoadTwoStringReplacements(statusJsonPath);
if (statusReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {statusReplacements.Count} traduzioni da '{Path.GetFileName(statusJsonPath)}'.");
    var luminaStatus = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalStatusExd = luminaStatus.GetFile("exd/status_0_en.exd");
    if (originalStatusExd != null)
    {
        byte[] patchedStatusExd = ExdPatcher.PatchTwoStringSheet(
            originalStatusExd.Data,
            fixedDataSize: 36,
            string1ColumnOffset: 0,
            string2ColumnOffset: 4,
            statusReplacements);

        fileMap["exd/status_0_en.exd"] = patchedStatusExd;
        Console.WriteLine($"  * 'exd/status_0_en.exd' rigenerato ({patchedStatusExd.Length:N0} byte).");
    }
}

// 20. PATCH FATE (Eventi a tempo F.A.T.E. del mondo aperto)
string fateJsonPath = TranslationPathResolver.FindFile(translationsDir, "fate");
var fateReplacements = TranslationFileReader.LoadTwoStringReplacements(fateJsonPath);
if (fateReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {fateReplacements.Count} traduzioni da '{Path.GetFileName(fateJsonPath)}'.");
    var luminaFate = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalFateExd = luminaFate.GetFile("exd/fate_0_en.exd");
    if (originalFateExd != null)
    {
        byte[] patchedFateExd = ExdPatcher.PatchTwoStringSheet(
            originalFateExd.Data,
            fixedDataSize: 388,
            string1ColumnOffset: 0,
            string2ColumnOffset: 4,
            fateReplacements);

        fileMap["exd/fate_0_en.exd"] = patchedFateExd;
        Console.WriteLine($"  * 'exd/fate_0_en.exd' rigenerato ({patchedFateExd.Length:N0} byte).");
    }
}

// 21. PATCH ACHIEVEMENT (Obiettivi e trofei del personaggio)
string achievementJsonPath = TranslationPathResolver.FindFile(translationsDir, "achievement");
var achievementReplacements = TranslationFileReader.LoadTwoStringReplacements(achievementJsonPath);
if (achievementReplacements.Count > 0 && Directory.Exists(sqPackPath))
{
    Console.WriteLine();
    Console.WriteLine($"Caricate {achievementReplacements.Count} traduzioni da '{Path.GetFileName(achievementJsonPath)}'.");
    var luminaAchievement = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var originalAchievementExd = luminaAchievement.GetFile("exd/achievement_0_en.exd");
    if (originalAchievementExd != null)
    {
        byte[] patchedAchievementExd = ExdPatcher.PatchTwoStringSheet(
            originalAchievementExd.Data,
            fixedDataSize: 68,
            string1ColumnOffset: 0,
            string2ColumnOffset: 4,
            achievementReplacements);

        fileMap["exd/achievement_0_en.exd"] = patchedAchievementExd;
        Console.WriteLine($"  * 'exd/achievement_0_en.exd' rigenerato ({patchedAchievementExd.Length:N0} byte).");
    }
}

// 22. PATCH TRANSLATED QUESTS (se presenti in data/translations/quests/)
string questsDir = Path.Combine(translationsDir, "quests");
if (Directory.Exists(questsDir) && Directory.Exists(sqPackPath))
{
    var luminaQuest = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    int patchedQuests = 0;

    foreach (var qFile in Directory.GetFiles(questsDir, "*.json", SearchOption.AllDirectories))
    {
        try
        {
            var content = File.ReadAllText(qFile);
            using var doc = System.Text.Json.JsonDocument.Parse(content);
            var replacements = new Dictionary<uint, (string Tag, string Text)>();

            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (!uint.TryParse(prop.Name, out uint rowId)) continue;
                if (prop.Value.ValueKind != System.Text.Json.JsonValueKind.Object) continue;

                var obj = prop.Value;
                string tag = obj.TryGetProperty("tag", out var tp) ? tp.GetString() ?? "" : "";
                string orig = obj.TryGetProperty("original", out var op) ? op.GetString() ?? "" : "";
                string trans = obj.TryGetProperty("translation", out var trp) ? trp.GetString() ?? "" : "";

                if (!string.IsNullOrWhiteSpace(trans))
                {
                    replacements[rowId] = (tag, trans);
                }
            }

            if (replacements.Count > 0)
            {
                string qName = Path.GetFileNameWithoutExtension(qFile);
                var parentFolder = Directory.GetParent(qFile)?.Name; // e.g. 000
                if (!string.IsNullOrEmpty(parentFolder))
                {
                    string exdGamePath = $"exd/quest/{parentFolder}/{qName.ToLowerInvariant()}_0_en.exd";
                    var originalExd = luminaQuest.GetFile(exdGamePath);
                    if (originalExd != null)
                    {
                        var dict = replacements.ToDictionary(k => k.Key, v => ((string?)v.Value.Tag, (string?)v.Value.Text));
                        byte[] patchedQuestExd = ExdPatcher.PatchTwoStringSheet(
                            originalExd.Data,
                            fixedDataSize: 8,
                            string1ColumnOffset: 0,
                            string2ColumnOffset: 4,
                            dict);

                        fileMap[exdGamePath] = patchedQuestExd;
                        patchedQuests++;
                    }
                }
            }
        }
        catch { }
    }

    if (patchedQuests > 0)
    {
        Console.WriteLine();
        Console.WriteLine($"[Quests] {patchedQuests} missioni con traduzioni patchate nel pacchetto Penumbra.");
    }
}

// PATCH ASSET UI (texture localizzate per la creazione del personaggio)
string assetsDir = Path.Combine(projectRoot, "data", "assets");
if (Directory.Exists(assetsDir))
{
    var assetFiles = Directory.EnumerateFiles(assetsDir, "*", SearchOption.AllDirectories).ToArray();
    foreach (var assetPath in assetFiles)
    {
        string gamePath = Path.GetRelativePath(assetsDir, assetPath).Replace(Path.DirectorySeparatorChar, '/');
        fileMap[gamePath] = await File.ReadAllBytesAsync(assetPath);
    }

    if (assetFiles.Length > 0)
    {
        Console.WriteLine();
        Console.WriteLine($"Caricate {assetFiles.Length} risorse UI localizzate da '{assetsDir}'.");
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
Console.WriteLine($"Tutti i {fileMap.Count(f => f.Key.EndsWith(".exd", StringComparison.OrdinalIgnoreCase))} fogli EXD e i metadati Penumbra v4 sono pronti.");
Console.WriteLine("IMPORTANTE: Riavvia il gioco FINAL FANTASY XIV per applicare i fogli EXD.");
Console.WriteLine("(I file EXD vengono memorizzati nella RAM del processo FFXIV al caricamento)");
Console.WriteLine("==================================================");

