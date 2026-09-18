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
    Console.WriteLine("Campione righe PlaceName sheet:");
    uint[] placeIds = [1, 2, 22, 23, 24, 25, 26, 27, 28, 39, 40, 50, 51];
    foreach (var id in placeIds)
    {
        var row = placeNameSheet?.GetRowOrDefault(id);
        var text = row?.Name.ExtractText();
        if (!string.IsNullOrEmpty(text))
        {
            Console.WriteLine($"  [Place #{id}]: \"{text}\"");
        }
    }

    Console.WriteLine();
    Console.WriteLine("Test di estrazione completato con SUCCESSO!");
}
catch (Exception ex)
{
    Console.WriteLine($"[ECCEZIONE] {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}
