using System.IO.Compression;
using System.Text.Json;

namespace FFXIVItalian.Core.Packaging;

public class PenumbraMeta
{
    public int FileVersion { get; set; } = 3;
    public string Name { get; set; } = "FFXIV Italiano";
    public string Author { get; set; } = "FFXIV Italian Localization Team";
    public string Description { get; set; } = "Localizzazione italiana professionale di Final Fantasy XIV.";
    public string Version { get; set; } = "0.1.0";
    public string Website { get; set; } = "https://github.com/barbi/FFXIV_Italian";
    public int ModPackVersion { get; set; } = 3;
}

public class PenumbraDefaultMod
{
    public Dictionary<string, string> Files { get; set; } = new();
    public Dictionary<string, string> FileSwaps { get; set; } = new();
    public List<object> Manipulations { get; set; } = [];
}

public static class PenumbraPackager
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public static async Task CreatePmpPackageAsync(
        string outputPmpPath,
        PenumbraMeta meta,
        Dictionary<string, byte[]> fileContents)
    {
        var outputDir = Path.GetDirectoryName(outputPmpPath);
        if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        if (File.Exists(outputPmpPath))
        {
            File.Delete(outputPmpPath);
        }

        using var zipToCreate = new FileStream(outputPmpPath, FileMode.Create);
        using var archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create);

        // 1. Write meta.json
        var metaEntry = archive.CreateEntry("meta.json", CompressionLevel.Optimal);
        await using (var entryStream = metaEntry.Open())
        {
            await JsonSerializer.SerializeAsync(entryStream, meta, JsonOptions);
        }

        // 2. Build default_mod.json
        var defaultMod = new PenumbraDefaultMod();
        foreach (var (gamePath, _) in fileContents)
        {
            // The file path inside the mod package
            defaultMod.Files[gamePath] = gamePath;
        }

        var defaultModEntry = archive.CreateEntry("default_mod.json", CompressionLevel.Optimal);
        await using (var entryStream = defaultModEntry.Open())
        {
            await JsonSerializer.SerializeAsync(entryStream, defaultMod, JsonOptions);
        }

        // 3. Write individual game files into the package
        foreach (var (gamePath, content) in fileContents)
        {
            var fileEntry = archive.CreateEntry(gamePath, CompressionLevel.Optimal);
            await using var entryStream = fileEntry.Open();
            await entryStream.WriteAsync(content);
        }
    }
}

