using Lumina;
using Lumina.Data;
using Lumina.Excel.Sheets;

Console.WriteLine("==================================================");
Console.WriteLine(" FFXIV Italian - Game Data Extractor");
Console.WriteLine("==================================================");

// Detect game path
string defaultSqPack = @"G:\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack";

if (args.Length > 0 && Directory.Exists(args[0]))
{
    defaultSqPack = args[0];
}

Console.WriteLine($"Verifica percorso SqPack: {defaultSqPack}");

if (!Directory.Exists(defaultSqPack))
{
    Console.WriteLine($"[ERRORE] Directory non trovata: {defaultSqPack}");
    return;
}

try
{
    Console.WriteLine("Inizializzazione motore Lumina in corso...");
    var lumina = new GameData(defaultSqPack, new LuminaOptions
    {
        DefaultExcelLanguage = Language.English
    });

    Console.WriteLine("Connessione ai dati di gioco riuscita!");

    // Test reading sheets
    var addonSheet = lumina.GetExcelSheet<Addon>();
    Console.WriteLine($"- Foglio Addon: {addonSheet?.Count ?? 0} righe caricate.");

    var actionSheet = lumina.GetExcelSheet<Lumina.Excel.Sheets.Action>();
    Console.WriteLine($"- Foglio Action: {actionSheet?.Count ?? 0} righe caricate.");

    var placeNameSheet = lumina.GetExcelSheet<PlaceName>();
    Console.WriteLine($"- Foglio PlaceName: {placeNameSheet?.Count ?? 0} righe caricate.");

    var npcBaseSheet = lumina.GetExcelSheet<ENpcBase>();
    Console.WriteLine($"- Foglio ENpcBase: {npcBaseSheet?.Count ?? 0} righe caricate.");

    var npcResidentSheet = lumina.GetExcelSheet<ENpcResident>();
    Console.WriteLine($"- Foglio ENpcResident: {npcResidentSheet?.Count ?? 0} righe caricate.");

    // Sample inspection: test NPC gender reading
    if (npcResidentSheet != null && npcBaseSheet != null)
    {
        int checkedNpc = 0;
        foreach (var resident in npcResidentSheet)
        {
            var name = resident.Singular.ExtractText();
            if (string.IsNullOrEmpty(name)) continue;

            // Check if it matches a known NPC
            if (name.Contains("Urianger") || name.Contains("Thancred") || name.Contains("Y'shtola") || name.Contains("Tataru") || name.Contains("Kan-E-Senna"))
            {
                var baseNpc = npcBaseSheet.GetRowOrDefault(resident.RowId);
                var gender = baseNpc?.Gender ?? 255;
                string genderStr = gender switch
                {
                    0 => "Maschio (0)",
                    1 => "Femmina (1)",
                    _ => $"Sconosciuto ({gender})"
                };
                Console.WriteLine($"  * NPC: {name} (ID: {resident.RowId}) -> Genere: {genderStr}");
                checkedNpc++;
                if (checkedNpc >= 5) break;
            }
        }
    }

    Console.WriteLine();
    Console.WriteLine("Test lettura file raw EXD e EXH tramite Lumina:");
    var placeNameExh = lumina.GetFile("exd/placename.exh");
    if (placeNameExh != null)
    {
        var fixedDataSize = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(placeNameExh.Data.AsSpan(0x06, 2));
        var colCount = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(placeNameExh.Data.AsSpan(0x08, 2));
        Console.WriteLine($"- PlaceName.exh: fixedDataSize={fixedDataSize}, columns={colCount}");
    }

    var placeNameExd = lumina.GetFile("exd/placename_0_en.exd");
    Console.WriteLine($"- File exd/placename_0_en.exd caricato: {placeNameExd?.Data.Length ?? 0} byte.");

    var addonExh = lumina.GetFile("exd/addon.exh");
    if (addonExh != null)
    {
        var fixedDataSize = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(addonExh.Data.AsSpan(0x06, 2));
        var colCount = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(addonExh.Data.AsSpan(0x08, 2));
        Console.WriteLine($"- Addon.exh: fixedDataSize={fixedDataSize}, columns={colCount}");
    }

    var addonExd = lumina.GetFile("exd/addon_0_en.exd");
    Console.WriteLine($"- File exd/addon_0_en.exd caricato: {addonExd?.Data.Length ?? 0} byte.");

    Console.WriteLine();
    Console.WriteLine("Colonne foglio Lobby.exh:");
    var lobbyExh = lumina.GetFile("exd/lobby.exh");
    if (lobbyExh != null)
    {
        var fixedDataSize = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(lobbyExh.Data.AsSpan(0x06, 2));
        var colCount = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(lobbyExh.Data.AsSpan(0x08, 2));
        Console.WriteLine($"- Lobby.exh: fixedDataSize={fixedDataSize}, columns={colCount}");

        for (int c = 0; c < colCount; c++)
        {
            int colPos = 0x20 + (c * 4);
            var colType = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(lobbyExh.Data.AsSpan(colPos, 2));
            var colOffset = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(lobbyExh.Data.AsSpan(colPos + 2, 2));
            Console.WriteLine($"  Colonna {c}: Tipo=0x{colType:X4}, Offset={colOffset}");
        }
    }

    // Export templates
    string translationsDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "data", "translations");
    if (!Directory.Exists(translationsDir))
    {
        translationsDir = Path.Combine(Directory.GetCurrentDirectory(), "data", "translations");
    }

    var lobbySheet = lumina.GetExcelSheet<Lobby>();
    Console.WriteLine($"- Foglio Lobby: {lobbySheet?.Count ?? 0} righe caricate.");
    if (lobbySheet != null)
    {
        var allLobby = new Dictionary<string, string>();
        foreach (var row in lobbySheet)
        {
            var text = row.Text.ExtractText();
            if (!string.IsNullOrEmpty(text))
            {
                allLobby[row.RowId.ToString()] = text;
            }
        }
        string lobbyAllPath = Path.Combine(translationsDir, "lobby_all_extracted.json");
        File.WriteAllText(lobbyAllPath, System.Text.Json.JsonSerializer.Serialize(allLobby, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"Salvate tutte le {allLobby.Count} stringhe di Lobby in '{Path.GetFileName(lobbyAllPath)}'.");
    }

    Console.WriteLine();
    Console.WriteLine("Esplorazione fogli UI:");
    var mainCommandSheet = lumina.GetExcelSheet<MainCommand>();
    Console.WriteLine($"- Foglio MainCommand: {mainCommandSheet?.Count ?? 0} righe.");

    var mainCommandCatSheet = lumina.GetExcelSheet<MainCommandCategory>();
    if (mainCommandCatSheet != null)
    {
        var catDict = new Dictionary<string, object>();
        foreach (var cat in mainCommandCatSheet)
        {
            var name = cat.Name.ExtractText();
            if (!string.IsNullOrEmpty(name))
            {
                catDict[cat.RowId.ToString()] = new
                {
                    original = name,
                    translation = ""
                };
            }
        }
        var catJson = System.Text.Json.JsonSerializer.Serialize(catDict, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(translationsDir, "maincommandcategory_template.json"), catJson);
        Console.WriteLine($"Esportato maincommandcategory_template.json ({catDict.Count} voci)");
    }

    if (mainCommandSheet != null)
    {
        var cmdDict = new Dictionary<string, object>();
        foreach (var cmd in mainCommandSheet)
        {
            var name = cmd.Name.ExtractText();
            var desc = cmd.Description.ExtractText();
            if (!string.IsNullOrEmpty(name))
            {
                cmdDict[cmd.RowId.ToString()] = new
                {
                    name = name,
                    translation_name = "",
                    description = desc,
                    translation_description = ""
                };
            }
        }
        var cmdJson = System.Text.Json.JsonSerializer.Serialize(cmdDict, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(translationsDir, "maincommand_template.json"), cmdJson);
        Console.WriteLine($"Esportato maincommand_template.json ({cmdDict.Count} comandi)");
    }

    if (addonSheet != null)
    {
        var addonDict = new Dictionary<string, string>();
        for (uint i = 1; i <= 500; i++)
        {
            var row = addonSheet.GetRowOrDefault(i);
            if (row != null)
            {
                var text = row.Value.Text.ExtractText();
                if (!string.IsNullOrWhiteSpace(text) && text.Length > 1 && !text.StartsWith("--"))
                {
                    addonDict[i.ToString()] = text;
                }
            }
        }
        var addonJson = System.Text.Json.JsonSerializer.Serialize(addonDict, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(translationsDir, "addon_sample.json"), addonJson);
        Console.WriteLine($"Esportato addon_sample.json ({addonDict.Count} righe Addon)");
    }

    var mainCommandExh = lumina.GetFile("exd/maincommand.exh");
    if (mainCommandExh != null)
    {
        var fixedDataSize = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(mainCommandExh.Data.AsSpan(0x06, 2));
        var colCount = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(mainCommandExh.Data.AsSpan(0x08, 2));
        Console.WriteLine($"- MainCommand.exh: fixedDataSize={fixedDataSize}, columns={colCount}");
        for (int c = 0; c < colCount; c++)
        {
            int colPos = 0x20 + (c * 4);
            var colType = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(mainCommandExh.Data.AsSpan(colPos, 2));
            var colOffset = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(mainCommandExh.Data.AsSpan(colPos + 2, 2));
            Console.WriteLine($"  MainCommand Colonna {c}: Tipo=0x{colType:X4}, Offset={colOffset}");
        }
    }

    Console.WriteLine();
    Console.WriteLine("Dettaglio file error_0_en.exd:");
    var errorExh = lumina.GetFile("exd/error.exh");
    var errorExd = lumina.GetFile("exd/error_0_en.exd");
    if (errorExh != null && errorExd != null)
    {
        var fixedDataSize = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(errorExh.Data.AsSpan(0x06, 2));
        var colCount = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(errorExh.Data.AsSpan(0x08, 2));
        Console.WriteLine($"- Error.exh: fixedDataSize={fixedDataSize}, columns={colCount}");
        for (int c = 0; c < colCount; c++)
        {
            int colPos = 0x20 + (c * 4);
            var colType = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(errorExh.Data.AsSpan(colPos, 2));
            var colOffset = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(errorExh.Data.AsSpan(colPos + 2, 2));
            Console.WriteLine($"  Error Colonna {c}: Tipo=0x{colType:X4}, Offset={colOffset}");
        }

        uint indexSize = System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(errorExd.Data.AsSpan(0x08, 4));
        int rowCount = (int)(indexSize / 8);
        for (int i = 0; i < rowCount; i++)
        {
            int entryPos = 0x20 + (i * 8);
            uint rowId = System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(errorExd.Data.AsSpan(entryPos, 4));
            uint offset = System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(errorExd.Data.AsSpan(entryPos + 4, 4));
            if (rowId == 13206)
            {
                int dataSize = (int)System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(errorExd.Data.AsSpan((int)offset, 4));
                int strStart = (int)offset + 6 + 4;
                int strLen = dataSize - 4;
                var raw = errorExd.Data.AsSpan(strStart, strLen).ToArray();
                Console.WriteLine($"Row 13206 raw bytes ({raw.Length}):");
                Console.WriteLine(Convert.ToHexString(raw));
                break;
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[ECCEZIONE] {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}
