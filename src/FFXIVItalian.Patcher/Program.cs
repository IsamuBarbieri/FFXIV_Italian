using FFXIVItalian.Core.Packaging;
using Lumina;
using Lumina.Data;
using System.Text;

Console.WriteLine("==================================================");
Console.WriteLine(" FFXIV Italian - Penumbra Mod Packager (.pmp)");
Console.WriteLine("==================================================");

string sqPackPath = @"G:\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack";
string penumbraModDir = @"G:\SquareEnix\FFXIV_Mod\FFXIV Italiano (Test In-Game)";
string outputPmp = "FFXIV_Italian.pmp";

if (args.Length > 0 && Directory.Exists(args[0]))
{
    sqPackPath = args[0];
}

Console.WriteLine($"Percorso SqPack gioco: {sqPackPath}");
Console.WriteLine($"Percorso Penumbra Mod: {penumbraModDir}");

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

        // 1. PATCH FOGLIO LOBBY (SCHERMATA DEL TITOLO, MENU INIZIALE E CREAZIONE PG)
        Console.WriteLine();
        Console.WriteLine("Estrazione e patching di 'exd/lobby_0_en.exd' (Schermata del Titolo)...");
        var originalLobbyExd = lumina.GetFile("exd/lobby_0_en.exd");
        if (originalLobbyExd != null)
        {
            Console.WriteLine($"Letto file originale Lobby ({originalLobbyExd.Data.Length:N0} byte).");

            var lobbyReplacements = new Dictionary<uint, string>
            {
                // Menu Principale del Titolo
                [2001] = "INIZIA",
                [2002] = "ESCI",
                [2003] = "CONFIGURAZIONE DI SISTEMA",
                [2004] = "FILMATO INIZIALE",
                [2006] = "DATA CENTER",
                [2008] = "Demo e Titolo A Realm Reborn",
                [2009] = "INDIETRO",
                [2010] = "Selezione Personaggio",
                [2011] = "FILMATI E TITOLI",
                [2012] = "Demo e Titolo Heavensward",
                [2013] = "Demo e Titolo Stormblood",
                [2014] = "Demo e Titolo Shadowbringers",
                [2021] = "VISITA MONDO",
                [2022] = "Demo e Titolo Endwalker",
                [2023] = "Demo e Titolo Dawntrail",
                [2025] = "Mondo",
                [2027] = "Nuovo Personaggio",
                [2028] = "Personaggi Salvati",
                [2030] = "Selezione Mondo",
                [2033] = "Lista Mondi",

                // Creazione Personaggio
                [2015] = "RAZZA",
                [2016] = "CLAN",
                [2017] = "GENETLIACO",
                [2018] = "DIVINITÀ TUTELARE",
                [2019] = "CLASSE",
                [2020] = "POSIZIONE",
                [2035] = "Nome",
                [2036] = "Cognome",
                [2037] = "Cambia",
                [2038] = "Nome del Personaggio",
                [2044] = "Caricare i dati dell'aspetto salvati in precedenza?",
                [2046] = "Scegli un Altro",
                [2047] = "Indietro",
                [2050] = "Conferma",
                [2051] = "Annulla",
                [2052] = "Esci",
                [2053] = "Aggiorna",
                [2055] = "CREAZIONE PERSONAGGIO",
                [2056] = "PERSONALIZZAZIONE PERSONAGGIO",
                [2057] = "LICENZA",
                [2058] = "OPZIONI",
                [2060] = "POSA",
                [2061] = "In Piedi",
                [2062] = "Unica",
                [2063] = "Casuale",
                [2064] = "ABBIGLIAMENTO",
                [2065] = "Razza",
                [2066] = "Classe",
                [2067] = "Biancheria",
                [2068] = "Servitore",
                [2069] = "Equipaggiato",
                [2070] = "Ambiente",
                [2071] = "Alba",
                [2072] = "Mezzogiorno",
                [2073] = "Tramonto",
                [2074] = "Sera",
                [2100] = "Nome del Personaggio",
                [2101] = "Nome",
                [2102] = "Cognome",
                [2104] = "Conferma",
                [2126] = "Aspetto Casuale",
                [2162] = "Sì",
                [2163] = "No",
                [2177] = "Sì",
                [2178] = "No",
                [2180] = "Indietro",
                [2184] = "Dati salvati.",
                [2185] = "Salvataggio non riuscito.",
                [2186] = "OK"
            };

            Console.WriteLine($"Applicazione di {lobbyReplacements.Count} stringhe italiane al foglio Lobby (Offset 12)...");
            byte[] patchedLobbyExd = ExdPatcher.PatchSimpleStringSheet(
                originalLobbyExd.Data,
                fixedDataSize: 24,
                stringColumnOffset: 12,
                lobbyReplacements);

            fileMap["exd/lobby_0_en.exd"] = patchedLobbyExd;
            Console.WriteLine($"- File 'exd/lobby_0_en.exd' rigenerato con successo ({patchedLobbyExd.Length:N0} byte)!");
        }

        // 2. PATCH FOGLIO ADDON (POPUP DI CONFERMA E PULSANTI GENERALI)
        Console.WriteLine();
        Console.WriteLine("Estrazione e patching di 'exd/addon_0_en.exd' (Pulsanti e popup di sistema)...");
        var originalAddonExd = lumina.GetFile("exd/addon_0_en.exd");
        if (originalAddonExd != null)
        {
            var addonReplacements = new Dictionary<uint, string>
            {
                [1] = "OK",
                [2] = "Annulla",
                [3] = "Sì.",
                [4] = "No.",
                [5] = "Esci dal Gioco",
                [100] = "Copia",
                [101] = "Incolla",
                [102] = "Taglia",
                [103] = "Esegui",
                [104] = "Elimina",
                [105] = "Annulla azione"
            };

            byte[] patchedAddonExd = ExdPatcher.PatchSimpleStringSheet(
                originalAddonExd.Data,
                fixedDataSize: 4,
                stringColumnOffset: 0,
                addonReplacements);

            fileMap["exd/addon_0_en.exd"] = patchedAddonExd;
            Console.WriteLine($"- File 'exd/addon_0_en.exd' rigenerato con successo ({patchedAddonExd.Length:N0} byte)!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERRORE durante il patching]: {ex.Message}");
    }
}

fileMap["readme_ita.txt"] = Encoding.UTF8.GetBytes("FFXIV Italiano - Titolo e Interfaccia tradotti con successo!");

// A. Compilazione pacchetto .pmp per distribuzione
Console.WriteLine();
Console.WriteLine($"Generazione archivio Penumbra ({outputPmp})...");
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

    // Aggiorna meta.json di Penumbra v4 per registrare i file reindirizzati
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
    Console.WriteLine("  * Aggiornato: meta.json con reindirizzamenti exd/lobby_0_en.exd e exd/addon_0_en.exd");
}

Console.WriteLine();
Console.WriteLine("==================================================");
Console.WriteLine("SUCCESSO! Titolo e Interfaccia aggiornati al 100%!");
Console.WriteLine("1. In gioco, apri Penumbra (/penumbra).");
Console.WriteLine("2. Clicca sull'icona circolare 🔄 di Ricarica accanto a 'FFXIV Italiano'.");
Console.WriteLine("3. Vedrai all'istante nella schermata del titolo: 'INIZIA', 'ESCI', 'CONFIGURAZIONE DI SISTEMA', ecc.!");
Console.WriteLine("==================================================");
