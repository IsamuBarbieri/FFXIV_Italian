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
                byte[] patchedLobbyExd = ExdPatcher.PatchSimpleStringSheet(
                    originalLobbyExd.Data,
                    fixedDataSize: 24,
                    stringColumnOffset: 12,
                    lobbyReplacements);

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

    var penumbraMetaJson = $$"""
    {
        "FileVersion": 4,
        "Name": "{{meta.Name}}",
        "Author": "{{meta.Author}}",
        "Description": "{{meta.Description}}",
        "Version": "{{meta.Version}}",
        "Website": "{{meta.Website}}",
        "DefaultData": {
            "Files": {
                "exd/lobby_0_en.exd": "exd\\lobby_0_en.exd",
                "exd/addon_0_en.exd": "exd\\addon_0_en.exd",
                "readme_ita.txt": "readme_ita.txt"
            }
        }
    }
    """;
    await File.WriteAllTextAsync(Path.Combine(penumbraModDir, "meta.json"), penumbraMetaJson);
}

Console.WriteLine();
Console.WriteLine("==================================================");
Console.WriteLine("REBUILD COMPLETATO CON SUCCESSO!");
Console.WriteLine("1. Modifica i file in: data/translations/ (*.json)");
Console.WriteLine("2. Ricarica in gioco su Penumbra (/penumbra) cliccando 🔄 Reload.");
Console.WriteLine("==================================================");
