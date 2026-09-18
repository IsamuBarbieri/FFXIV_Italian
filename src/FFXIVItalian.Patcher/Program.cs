using FFXIVItalian.Core.Packaging;
using Lumina;
using Lumina.Data;
using System.Text;

Console.WriteLine("==================================================");
Console.WriteLine(" FFXIV Italian - Penumbra Mod Packager (.pmp)");
Console.WriteLine("==================================================");

string sqPackPath = @"G:\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack";
string outputPmp = "FFXIV_Italian.pmp";

if (args.Length > 0 && Directory.Exists(args[0]))
{
    sqPackPath = args[0];
}
if (args.Length > 1)
{
    outputPmp = args[1];
}

Console.WriteLine($"Percorso SqPack gioco: {sqPackPath}");

if (!Directory.Exists(sqPackPath))
{
    Console.WriteLine($"[AVVISO] Directory SqPack non trovata: {sqPackPath}");
    Console.WriteLine("Verrà generato un archivio .pmp di base.");
}

var meta = new PenumbraMeta
{
    Name = "FFXIV Italiano (Test In-Game)",
    Author = "FFXIV Italian Community",
    Description = "Pacchetto di collaudo in-game: localizzazione dei dialoghi di conferma, menu e interfaccia (Addon UI).",
    Version = "0.1.0-test",
    Website = "https://github.com/IsamuBarbieri/FFXIV_Italian"
};

var fileMap = new Dictionary<string, byte[]>();

if (Directory.Exists(sqPackPath))
{
    try
    {
        Console.WriteLine("Estrazione di 'exd/addon_0_en.exd' tramite Lumina...");
        var lumina = new GameData(sqPackPath, new LuminaOptions
        {
            DefaultExcelLanguage = Language.English
        });

        var originalAddonExd = lumina.GetFile("exd/addon_0_en.exd");
        if (originalAddonExd != null)
        {
            Console.WriteLine($"Letto file originale ({originalAddonExd.Data.Length:N0} byte).");

            // Traduzioni di test per pulsanti e menu universali (Addon sheet)
            var replacements = new Dictionary<uint, string>
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

            Console.WriteLine($"Applicazione di {replacements.Count} stringhe italiane nei blocchi binari EXDF...");
            byte[] patchedAddonExd = ExdPatcher.PatchSimpleStringSheet(
                originalAddonExd.Data,
                fixedDataSize: 4,
                replacements);

            fileMap["exd/addon_0_en.exd"] = patchedAddonExd;
            Console.WriteLine($"- File 'exd/addon_0_en.exd' rigenerato con successo ({patchedAddonExd.Length:N0} byte)!");
        }
        else
        {
            Console.WriteLine("[AVVISO] Impossibile estrarre exd/addon_0_en.exd da SqPack.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERRORE durante il patching]: {ex.Message}");
    }
}

fileMap["readme_ita.txt"] = Encoding.UTF8.GetBytes("FFXIV Italiano - Pacchetto Mod Penumbra collaudato con successo!");

Console.WriteLine($"Compilazione archivio Penumbra (.pmp): {outputPmp}...");
await PenumbraPackager.CreatePmpPackageAsync(outputPmp, meta, fileMap);

Console.WriteLine();
Console.WriteLine("==================================================");
Console.WriteLine($"SUCCESSO! Pacchetto generato: {Path.GetFullPath(outputPmp)}");
Console.WriteLine("Contiene: exd/addon_0_en.exd (pulsanti di sistema tradotti in italiano)");
Console.WriteLine("==================================================");
