using FFXIVItalian.Core.Packaging;
using System.Text;

Console.WriteLine("==================================================");
Console.WriteLine(" FFXIV Italian - Penumbra Mod Packager (.pmp)");
Console.WriteLine("==================================================");

string outputPath = "FFXIV_Italian.pmp";
if (args.Length > 0)
{
    outputPath = args[0];
}

var meta = new PenumbraMeta
{
    Name = "FFXIV Italiano",
    Author = "FFXIV Italian Community",
    Description = "Localizzazione italiana professionale di Final Fantasy XIV (Interfaccia, MSQ, Dialoghi e Sistemi).",
    Version = "0.1.0-alpha",
    Website = "https://github.com/barbi/FFXIV_Italian"
};

Console.WriteLine($"Creazione pacchetto Penumbra: {outputPath}");

// Demo test assets to verify Penumbra mod package creation
var files = new Dictionary<string, byte[]>
{
    ["info.txt"] = Encoding.UTF8.GetBytes("FFXIV Italian Localization - Generato con successo per Penumbra.")
};

await PenumbraPackager.CreatePmpPackageAsync(outputPath, meta, files);

Console.WriteLine($"Pacchetto .pmp generato con successo in: {Path.GetFullPath(outputPath)}");
Console.WriteLine("Il file può essere importato direttamente in Penumbra tramite XIVLauncher/Dalamud.");
