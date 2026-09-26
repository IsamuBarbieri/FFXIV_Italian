using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Drawing.Text;

var rootInfo = new DirectoryInfo(AppContext.BaseDirectory);
while (rootInfo is not null && !Directory.Exists(Path.Combine(rootInfo.FullName, "data")))
    rootInfo = rootInfo.Parent;
var root = rootInfo?.FullName ?? throw new DirectoryNotFoundException("Non trovo la radice del progetto.");
var configPath = Path.Combine(root, "tools", "clan_texture_labels.json");
var config = JsonSerializer.Deserialize<GeneratorConfig>(File.ReadAllText(configPath), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
    ?? throw new InvalidDataException($"Configurazione non valida: {configPath}");
var requestedIds = args.ToHashSet(StringComparer.Ordinal);
var unknownIds = requestedIds.Except(config.Labels.Keys, StringComparer.Ordinal).ToArray();
if (unknownIds.Length > 0)
    throw new ArgumentException($"ID non presenti nella configurazione: {string.Join(", ", unknownIds)}");
if (config.Labels.Count == 0)
    throw new InvalidDataException("Nessuna etichetta configurata.");
var labelsToGenerate = config.Labels.Where(pair => requestedIds.Count == 0 || requestedIds.Contains(pair.Key)).ToArray();
var fontPath = new[]
    {
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows", "Fonts"),
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts")
    }
    .Where(Directory.Exists)
    .SelectMany(path => Directory.EnumerateFiles(path, "Cinzel*.ttf"))
    .FirstOrDefault() ?? throw new FileNotFoundException("Font Cinzel non trovato nei font installati.");
using var fontCollection = new PrivateFontCollection();
fontCollection.AddFontFile(fontPath);
var cinzel = fontCollection.Families.First(family => family.Name == "Cinzel");

var outputDir = Path.Combine(root, "data", "assets", "ui", "icon", "126000", "en");
Directory.CreateDirectory(outputDir);

foreach (var (id, text) in labelsToGenerate)
{
    var full = Compose(RenderLabel(text, cinzel, config), config.GlowColor);
    WriteTex(Path.Combine(outputDir, $"{id}_hr1.tex"), full);

    using var half = new Bitmap(800, 68, PixelFormat.Format32bppArgb);
    using (var graphics = Graphics.FromImage(half))
    {
        graphics.Clear(Color.Transparent);
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        graphics.DrawImage(full, new Rectangle(0, 0, 800, 68));
    }
    WriteTex(Path.Combine(outputDir, $"{id}.tex"), half);
    Console.WriteLine($"{id}: {text}");
}

return 0;

static Bitmap RenderLabel(string text, FontFamily family, GeneratorConfig config)
{
    // GraphicsPath.AddString produces broken contours with Cinzel's variable font.
    using var source = new Bitmap(6400, 800, PixelFormat.Format32bppArgb);
    using var font = new Font(family, 400, FontStyle.Regular, GraphicsUnit.Pixel);
    using (var graphics = Graphics.FromImage(source))
    {
        graphics.Clear(Color.FromArgb(0, 255, 255, 255));
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        graphics.DrawString(text.ToUpperInvariant(), font, Brushes.White, new PointF(64, 64), StringFormat.GenericTypographic);
    }

    var sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
    var pixels = new byte[sourceData.Stride * source.Height];
    Marshal.Copy(sourceData.Scan0, pixels, 0, pixels.Length);
    source.UnlockBits(sourceData);
    var left = source.Width; var top = source.Height; var right = -1; var bottom = -1;
    for (var y = 0; y < source.Height; y++)
    for (var x = 0; x < source.Width; x++)
    {
        if (pixels[y * sourceData.Stride + x * 4 + 3] == 0) continue;
        left = Math.Min(left, x); top = Math.Min(top, y);
        right = Math.Max(right, x); bottom = Math.Max(bottom, y);
    }
    if (right < left)
        throw new InvalidDataException($"Nessun glifo visibile per '{text}'.");

    var inkWidth = right - left + 1;
    var inkHeight = bottom - top + 1;
    var scale = Math.Min((double)config.MaxWidth / inkWidth, (double)config.MaxHeight / inkHeight);
    var outputWidth = (int)Math.Round(inkWidth * scale);
    var outputHeight = (int)Math.Round(inkHeight * scale);
    var bitmap = new Bitmap(1600, 136, PixelFormat.Format32bppArgb);
    using var target = Graphics.FromImage(bitmap);
    target.Clear(Color.FromArgb(0, 255, 255, 255));
    target.InterpolationMode = InterpolationMode.HighQualityBicubic;
    target.PixelOffsetMode = PixelOffsetMode.HighQuality;
    target.DrawImage(source,
        new Rectangle(config.LeftInset, (136 - outputHeight) / 2, outputWidth, outputHeight),
        new Rectangle(left, top, inkWidth, inkHeight), GraphicsUnit.Pixel);
    return bitmap;
}

static Bitmap Compose(Bitmap maskBitmap, int[] glowColor)
{
    var mask = new byte[136, 1600];
    for (var y = 0; y < 136; y++)
    for (var x = 0; x < 1600; x++)
        mask[y, x] = maskBitmap.GetPixel(x, y).A;
    maskBitmap.Dispose();

    var glow = Blur(mask, 16, 5);

    var bitmap = new Bitmap(1600, 136, PixelFormat.Format32bppArgb);
    var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
    var output = new byte[1600 * 136 * 4];
    for (var y = 0; y < 136; y++)
    for (var x = 0; x < 1600; x++)
    {
        var i = (y * 1600 + x) * 4;
        var glyphAlpha = mask[y, x];
        var glowAlpha = Math.Min(255, (int)Math.Round(glow[y, x] * 2.0)) * (255 - glyphAlpha) / 255;
        var alpha = glyphAlpha + glowAlpha;
        output[i] = alpha == 0 ? (byte)255 : (byte)((glowColor[2] * glowAlpha + 255 * glyphAlpha) / alpha);
        output[i + 1] = alpha == 0 ? (byte)255 : (byte)((glowColor[1] * glowAlpha + 255 * glyphAlpha) / alpha);
        output[i + 2] = alpha == 0 ? (byte)255 : (byte)((glowColor[0] * glowAlpha + 255 * glyphAlpha) / alpha);
        output[i + 3] = (byte)alpha;
    }
    Marshal.Copy(output, 0, data.Scan0, output.Length);
    bitmap.UnlockBits(data);
    return bitmap;
}

static double[,] Blur(byte[,] input, int radius, double sigma)
{
    var height = input.GetLength(0);
    var width = input.GetLength(1);
    var weights = Enumerable.Range(-radius, 2 * radius + 1)
        .Select(offset => Math.Exp(-offset * offset / (2 * sigma * sigma))).ToArray();
    var sum = weights.Sum();
    var horizontal = new double[height, width];
    var result = new double[height, width];
    for (var y = 0; y < height; y++)
    for (var x = 0; x < width; x++)
    for (var offset = -radius; offset <= radius; offset++)
    {
        var sourceX = x + offset;
        if ((uint)sourceX < (uint)width)
            horizontal[y, x] += input[y, sourceX] * weights[offset + radius] / sum;
    }
    for (var y = 0; y < height; y++)
    for (var x = 0; x < width; x++)
    for (var offset = -radius; offset <= radius; offset++)
    {
        var sourceY = y + offset;
        if ((uint)sourceY < (uint)height)
            result[y, x] += horizontal[sourceY, x] * weights[offset + radius] / sum;
    }
    return result;
}

static void WriteTex(string path, Bitmap bitmap)
{
    var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
    var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
    var pixels = new byte[bitmap.Width * bitmap.Height * 4];
    Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
    bitmap.UnlockBits(data);
    using var stream = File.Create(path);
    using var writer = new BinaryWriter(stream);
    writer.Write(0x00800000u); writer.Write(0x00001450u);
    writer.Write((ushort)bitmap.Width); writer.Write((ushort)bitmap.Height);
    writer.Write((ushort)1); writer.Write((ushort)1); writer.Write((ushort)0);
    // FFXIV stores the first mip's byte offset at header position 28.
    while (stream.Position < 28) writer.Write((byte)0);
    writer.Write(0x50u);
    while (stream.Position < 80) writer.Write((byte)0);
    writer.Write(pixels);
}

record GeneratorConfig(int MaxWidth, int MaxHeight, int LeftInset, int[] GlowColor, Dictionary<string, string> Labels);
