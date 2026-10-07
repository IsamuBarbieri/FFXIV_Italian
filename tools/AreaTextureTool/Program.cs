using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.Json;
using Lumina;
using Lumina.Data;
using Lumina.Data.Files;
using Lumina.Excel.Sheets;

const string DefaultSqPack = @"G:\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\sqpack";
var rootInfo = new DirectoryInfo(AppContext.BaseDirectory);
while (rootInfo is not null && !Directory.Exists(Path.Combine(rootInfo.FullName, "data")))
    rootInfo = rootInfo.Parent;
var root = rootInfo?.FullName ?? throw new DirectoryNotFoundException("Non trovo la radice del progetto.");

if (args.Length == 0)
    throw new ArgumentException("Uso: AreaTextureTool catalog [--sqpack <cartella>] [--placenames <file>] [--duties <file>] | dump <percorso-gioco.tex> <output.png> [--sqpack <cartella>] | pack [--config <file>] [--output-root <cartella>]");

var mode = args[0].ToLowerInvariant();
var options = ParseOptions(args.Skip(1));

if (mode == "dump")
{
    var sqPackPath = GetOption(options, "sqpack") ?? DefaultSqPack;
    if (!Directory.Exists(sqPackPath))
        throw new DirectoryNotFoundException($"Cartella sqpack non trovata: {sqPackPath}");
    using var game = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var positional = options.Positional;
    if (positional.Count != 2)
        throw new ArgumentException("Uso: AreaTextureTool dump <percorso-gioco.tex> <output.png> [--sqpack <cartella>]");
    var texture = GetTexture(game, positional[0]);
    var output = Path.GetFullPath(positional[1]);
    SavePng(output, texture.Header.Width, texture.Header.Height, texture.ImageData);
    Console.WriteLine($"{positional[0]} -> {output} ({texture.Header.Width}x{texture.Header.Height}, {texture.Header.Format})");
    return 0;
}

if (mode == "catalog")
{
    var sqPackPath = GetOption(options, "sqpack") ?? DefaultSqPack;
    if (!Directory.Exists(sqPackPath))
        throw new DirectoryNotFoundException($"Cartella sqpack non trovata: {sqPackPath}");
    using var game = new GameData(sqPackPath, new LuminaOptions { DefaultExcelLanguage = Language.English });
    var placeNamesPath = Path.GetFullPath(GetOption(options, "placenames") ?? Path.Combine(root, "data", "translations", "world", "placename.json"));
    var dutyNamesPath = Path.GetFullPath(GetOption(options, "duties") ?? Path.Combine(root, "data", "translations", "world", "contentfindercondition.json"));
    var placeNames = ReadTranslations(placeNamesPath, "translation");
    var dutyNames = ReadTranslations(dutyNamesPath, "translation");
    var candidates = new Dictionary<(int AssetId, string Layout, int RowId, string Text, bool IsDutyLabel, bool IsNonDuty), int>();
    foreach (var territory in game.Excel.GetSheet<TerritoryType>())
    {
        var dutyRowId = (int)territory.ContentFinderCondition.RowId;
        var placeNameId = (int)territory.PlaceName.RowId;
        AddCandidate(territory.PlaceNameRegionIcon, (int)territory.PlaceNameRegion.RowId, "region", placeNames.GetValueOrDefault((int)territory.PlaceNameRegion.RowId), false, dutyRowId == 0);
        AddCandidate(territory.PlaceNameIcon, placeNameId, "zone", placeNames.GetValueOrDefault(placeNameId), false, dutyRowId == 0);
        if (dutyRowId > 0)
            AddCandidate(territory.PlaceNameIcon, dutyRowId, "zone", dutyNames.GetValueOrDefault(dutyRowId), true, false);

        void AddCandidate(int assetId, int rowId, string layout, string? sourceText, bool isDutyLabel, bool isNonDuty)
        {
            if (assetId <= 0) return;
            if (string.IsNullOrWhiteSpace(sourceText)) return;
            var group = assetId / 1000 * 1000;
            var path = $"ui/icon/{group}/en/{assetId}.tex";
            if (!game.FileExists(path)) return;
            var texture = GetTexture(game, path);
            var expectedHeight = layout == "region" ? 64 : 128;
            if (texture.Header.Width != 1024 || texture.Header.Height != expectedHeight) return;
            var text = CleanLabel(sourceText);
            if (text.Length == 0) return;
            var key = (assetId, layout, rowId, text, isDutyLabel, isNonDuty);
            candidates[key] = candidates.GetValueOrDefault(key) + 1;
        }
    }
    var specialLabels = new Dictionary<int, string>
    {
        [122016] = "Fenditura Eterea",
        [122022] = "L'Ombra di Mhach",
        [122032] = "Dungeon Variante",
        [122033] = "Dungeon Criterio"
    };
    var entries = new List<TextureEntry>();
    foreach (var group in candidates.GroupBy(pair => pair.Key.AssetId).OrderBy(group => group.Key))
    {
        var assetCandidates = group.ToList();
        var hasNonDutyZone = assetCandidates.Any(pair => pair.Key.Layout == "zone" && pair.Key.IsNonDuty && !pair.Key.IsDutyLabel);
        var hasDutyLabel = assetCandidates.Any(pair => pair.Key.Layout == "zone" && pair.Key.IsDutyLabel);
        var eligible = hasNonDutyZone
            ? assetCandidates.Where(pair => !pair.Key.IsDutyLabel).ToList()
            : hasDutyLabel
                ? assetCandidates.Where(pair => pair.Key.IsDutyLabel).ToList()
                : assetCandidates;
        var candidate = eligible.OrderByDescending(pair => pair.Value).First();
        var text = specialLabels.GetValueOrDefault(group.Key) ?? candidate.Key.Text;
        if (eligible.Select(pair => pair.Key.Text).Distinct(StringComparer.Ordinal).Count() > 1)
            Console.Error.WriteLine($"Nomi diversi per ui/icon/{group.Key / 1000 * 1000}/en/{group.Key}.tex; scelto il riferimento territorio più frequente: {text}");
        var path = $"ui/icon/{group.Key / 1000 * 1000}/en/{group.Key}.tex";
        entries.Add(new TextureEntry($"territory-{group.Key}", path, path, text, candidate.Key.Layout));
    }
    var catalogPath = Path.GetFullPath(GetOption(options, "output") ?? Path.Combine(root, "tools", "AreaTextureTool", "area_texture_labels.json"));
    Directory.CreateDirectory(Path.GetDirectoryName(catalogPath)!);
    File.WriteAllText(catalogPath, JsonSerializer.Serialize(new GeneratorConfig(entries), new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
    Console.WriteLine($"Trovate {entries.Count} texture di territorio in {catalogPath} (regioni: {entries.Count(e => e.Layout == "region")}, zone: {entries.Count(e => e.Layout == "zone")}).");
    return 0;
}

if (mode != "pack")
    throw new ArgumentException($"Comando sconosciuto: {mode}");

var configPath = Path.GetFullPath(GetOption(options, "config") ?? Path.Combine(root, "tools", "AreaTextureTool", "area_texture_labels.json"));
var config = JsonSerializer.Deserialize<GeneratorConfig>(File.ReadAllText(configPath), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
    ?? throw new InvalidDataException($"Configurazione non valida: {configPath}");
if (config.Entries.Count == 0)
    throw new InvalidDataException("La configurazione non contiene texture.");

var outputRoot = Path.GetFullPath(GetOption(options, "output-root") ?? Path.Combine(root, "data", "assets"));
var previewRoot = Path.Combine(root, "tools", "fullscreen_texture_previews");
if (mode == "pack")
{
    foreach (var entry in config.Entries)
    foreach (var highResolution in new[] { false, true })
    {
        var suffix = highResolution ? "_hr1" : string.Empty;
        var scale = highResolution ? 2 : 1;
        var width = (entry.Layout == "full" ? 1280 : 1024) * scale;
        var height = (entry.Height > 0 ? entry.Height : entry.Layout switch { "zone" => 128, "region" => 64, "full" => 360, _ => throw new InvalidDataException($"Layout non valido: {entry.Layout}") }) * scale;
        var styledPath = Path.Combine(previewRoot, $"{entry.Id}{suffix}.styled.png");
        using var source = new Bitmap(styledPath);
        if (source.Width != width * 3 || source.Height != height * 3)
            throw new InvalidDataException($"Dimensioni Photoshop inattese per {styledPath}: {source.Width}x{source.Height}.");
        using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.DrawImage(source, new Rectangle(0, 0, width, height));
        }
        var pixels = ReadPixels(bitmap);
        var targetPath = AddHr1Suffix(entry.TargetPath, suffix);
        WriteTex(GetContainedPath(outputRoot, targetPath.Replace('/', Path.DirectorySeparatorChar)), width, height, pixels);
        SavePng(Path.Combine(previewRoot, $"{entry.Id}{suffix}.png"), width, height, pixels);
        Console.WriteLine($"{entry.Id}{suffix}: Photoshop -> {targetPath} ({width}x{height})");
    }
    return 0;
}

return 0;

static Options ParseOptions(IEnumerable<string> args)
{
    var positional = new List<string>();
    var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    using var iterator = args.GetEnumerator();
    while (iterator.MoveNext())
    {
        var value = iterator.Current;
        if (!value.StartsWith("--", StringComparison.Ordinal))
        {
            positional.Add(value);
            continue;
        }
        var name = value[2..];
        if (!iterator.MoveNext())
            throw new ArgumentException($"Manca il valore per --{name}.");
        values[name] = iterator.Current;
    }
    return new Options(positional, values);
}

static Dictionary<int, string> ReadTranslations(string path, string propertyName)
{
    using var document = JsonDocument.Parse(File.ReadAllText(path));
    var translations = new Dictionary<int, string>();
    foreach (var row in document.RootElement.EnumerateObject())
        if (int.TryParse(row.Name, out var rowId) && row.Value.TryGetProperty(propertyName, out var value) && value.GetString() is { Length: > 0 } text)
            translations[rowId] = text;
    return translations;
}

static string CleanLabel(string text) => System.Text.RegularExpressions.Regex.Replace(
    text.Replace("<hex:021F0103>", "-", StringComparison.OrdinalIgnoreCase)
        .Replace("<hex:02100103>", " ", StringComparison.OrdinalIgnoreCase),
    "<hex:[0-9A-Fa-f]+>", string.Empty).Trim();

static string? GetOption(Options options, string name) => options.Values.GetValueOrDefault(name);

static string AddHr1Suffix(string path, string suffix)
{
    if (suffix.Length == 0)
        return path;
    if (!path.EndsWith(".tex", StringComparison.OrdinalIgnoreCase))
        throw new InvalidDataException($"Il percorso della texture deve terminare in .tex: {path}");
    return path[..^4] + suffix + ".tex";
}

static string GetContainedPath(string root, string relativePath)
{
    var fullPath = Path.GetFullPath(relativePath, root);
    var rootPrefix = Path.TrimEndingDirectorySeparator(root) + Path.DirectorySeparatorChar;
    if (!fullPath.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase))
        throw new InvalidDataException($"Il percorso di output esce dalla cartella scelta: {relativePath}");
    return fullPath;
}

static TexFile GetTexture(GameData game, string gamePath) => game.GetFile<TexFile>(gamePath)
    ?? throw new FileNotFoundException($"Texture non trovata nei dati di gioco: {gamePath}");

static byte[] ReadPixels(Bitmap bitmap)
{
    var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    var pixels = new byte[data.Stride * bitmap.Height];
    Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
    bitmap.UnlockBits(data);
    return pixels;
}

static void WriteTex(string path, int width, int height, byte[] pixels)
{
    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    using var stream = File.Create(path);
    using var writer = new BinaryWriter(stream);
    writer.Write(0x00800000u);
    writer.Write(0x00001450u); // TextureFormat.A8R8G8B8
    writer.Write((ushort)width);
    writer.Write((ushort)height);
    writer.Write((ushort)1);
    writer.Write((ushort)1);
    writer.Write((ushort)0);
    while (stream.Position < 28) writer.Write((byte)0);
    writer.Write(0x50u);
    while (stream.Position < 80) writer.Write((byte)0);
    writer.Write(pixels);
}

static void SavePng(string path, int width, int height, byte[] pixels)
{
    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
    var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
    Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
    bitmap.UnlockBits(data);
    bitmap.Save(path, ImageFormat.Png);
}

record Options(List<string> Positional, Dictionary<string, string> Values);
record GeneratorConfig(List<TextureEntry> Entries);
record TextureEntry(string Id, string SourcePath, string TargetPath, string Text, string Layout, int Height = 0);
