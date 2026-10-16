using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FFXIVItalian.TextInspector;

internal sealed record TranslationReference(
    string FilePath,
    string RowId,
    string Field,
    int LineNumber,
    string Original,
    string Translation);

internal sealed class TranslationIndex
{
    private static readonly Regex HexTag = new("<hex:[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private readonly IReadOnlyDictionary<string, IReadOnlyList<TranslationReference>> byText;
    private readonly string[] sortedKeys;

    private TranslationIndex(
        IReadOnlyDictionary<string, IReadOnlyList<TranslationReference>> byText,
        int fileCount,
        int entryCount)
    {
        this.byText = byText;
        sortedKeys = byText.Keys.Order(StringComparer.Ordinal).ToArray();
        FileCount = fileCount;
        EntryCount = entryCount;
    }

    public int FileCount { get; }
    public int EntryCount { get; }
    public List<string> LoadErrors { get; } = new();

    public IReadOnlyList<TranslationReference> Find(string text) => Find(text, out _);

    public IReadOnlyList<TranslationReference> Find(string text, out bool abbreviated)
    {
        abbreviated = false;
        var key = Normalize(text);
        if (key.Length > 0 && byText.TryGetValue(key, out var references))
            return references;
        if (!TryGetTruncatedPrefix(text, out var prefix))
            return Array.Empty<TranslationReference>();

        abbreviated = true;
        var start = Array.BinarySearch(sortedKeys, prefix, StringComparer.Ordinal);
        if (start < 0) start = ~start;
        var candidates = new HashSet<TranslationReference>();
        for (var i = start; i < sortedKeys.Length && sortedKeys[i].StartsWith(prefix, StringComparison.Ordinal); i++)
            candidates.UnionWith(byText[sortedKeys[i]]);
        return candidates.OrderBy(x => x.FilePath, StringComparer.Ordinal)
            .ThenBy(x => x.RowId, StringComparer.Ordinal).ThenBy(x => x.Field, StringComparer.Ordinal).ToArray();
    }

    public IReadOnlyList<TranslationReference> FindAddonRow(uint rowId) =>
        FindSheetRow("Addon", rowId);

    private readonly Dictionary<(string Sheet, uint RowId), IReadOnlyList<TranslationReference>> sheetRows = new();

    public IReadOnlyList<TranslationReference> FindSheetRow(string sheet, uint rowId) =>
        sheetRows.GetValueOrDefault((sheet, rowId)) ?? Array.Empty<TranslationReference>();

    public IReadOnlyList<TranslationReference> FindDeclaredRow(string sheet, uint rowId, string visibleText, string? clientText = null)
    {
        var key = Normalize(visibleText);
        if (key.Length == 0) return Array.Empty<TranslationReference>();
        var truncated = TryGetTruncatedPrefix(visibleText, out var prefix);
        var client = clientText is null ? string.Empty : Normalize(clientText);
        var matchesClient = client.Length > 0 && (client == key || (truncated && client.StartsWith(prefix, StringComparison.Ordinal)));
        return FindSheetRow(sheet, rowId).Where(reference =>
        {
            var original = Normalize(reference.Original);
            var translation = Normalize(reference.Translation);
            return matchesClient || original == key || translation == key || (truncated &&
                (original.StartsWith(prefix, StringComparison.Ordinal) || translation.StartsWith(prefix, StringComparison.Ordinal)));
        }).ToArray();
    }

    internal static bool TryGetTruncatedPrefix(string text, out string prefix)
    {
        var key = Normalize(text);
        prefix = key.EndsWith("...", StringComparison.Ordinal) ? key[..^3].TrimEnd() : string.Empty;
        return prefix.Length >= 4 && prefix.Any(char.IsLetter);
    }

    public static TranslationIndex Load(string translationsDirectory, CancellationToken cancellationToken = default)
    {
        var entriesByText = new Dictionary<string, HashSet<TranslationReference>>(StringComparer.Ordinal);
        var fileCount = 0;
        var entryCount = 0;
        var errors = new List<string>();

        foreach (var filePath in Directory.EnumerateFiles(translationsDirectory, "*.json", SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var bytes = File.ReadAllBytes(filePath);
                var content = bytes.AsSpan();
                if (content.StartsWith(new byte[] { 0xEF, 0xBB, 0xBF }))
                    content = content[3..];
                using var document = JsonDocument.Parse(content.ToArray());
                var lines = ReadFieldLines(content);
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                    continue;

                fileCount++;
                var relativePath = "data/translations/" + Path.GetRelativePath(translationsDirectory, filePath).Replace('\\', '/');
                foreach (var row in document.RootElement.EnumerateObject())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (row.Value.ValueKind != JsonValueKind.Object)
                        continue;

                    var fields = new Dictionary<string, string>(StringComparer.Ordinal);
                    foreach (var field in row.Value.EnumerateObject())
                    {
                        if (field.Value.ValueKind == JsonValueKind.String)
                            fields[field.Name] = field.Value.GetString() ?? string.Empty;
                    }

                    foreach (var field in fields)
                    {
                        if (!TryGetSourceField(field.Key, out var sourceField))
                            continue;

                        fields.TryGetValue(sourceField, out var original);
                        var reference = new TranslationReference(
                            relativePath,
                            row.Name,
                            sourceField,
                            lines.GetValueOrDefault((row.Name, sourceField)),
                            CleanForDisplay(original ?? string.Empty),
                            CleanForDisplay(field.Value));

                        if (reference.Original.Length == 0 && reference.Translation.Length == 0)
                            continue;

                        Add(entriesByText, reference.Original, reference);
                        Add(entriesByText, reference.Translation, reference);
                        entryCount++;
                    }
                }
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException)
            {
                errors.Add($"{filePath}: {exception.Message}");
            }
        }

        var finalized = entriesByText.ToDictionary(
            pair => pair.Key,
            pair => (IReadOnlyList<TranslationReference>)pair.Value
                .OrderBy(reference => reference.FilePath, StringComparer.Ordinal)
                .ThenBy(reference => reference.RowId, StringComparer.Ordinal)
                .ThenBy(reference => reference.Field, StringComparer.Ordinal)
                .ToArray(),
            StringComparer.Ordinal);

        var result = new TranslationIndex(finalized, fileCount, entryCount);
        foreach (var row in finalized.Values.SelectMany(x => x).Distinct()
                     .Where(x => x.FilePath is "data/translations/system/addon.json" or "data/translations/system/lobby.json")
                     .GroupBy(x => (x.FilePath, x.RowId)))
            if (uint.TryParse(row.Key.RowId, out var rowId))
                result.sheetRows[(row.Key.FilePath.EndsWith("/addon.json", StringComparison.Ordinal) ? "Addon" : "Lobby", rowId)] = row.ToArray();
        result.LoadErrors.AddRange(errors);
        return result;
    }

    private static Dictionary<(string Row, string Field), int> ReadFieldLines(ReadOnlySpan<byte> bytes)
    {
        var result = new Dictionary<(string, string), int>();
        var reader = new Utf8JsonReader(bytes);
        var row = string.Empty;
        var offset = 0;
        var line = 1;
        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;
            while (offset < reader.TokenStartIndex)
                if (bytes[offset++] == '\n') line++;
            if (reader.CurrentDepth == 1)
                row = reader.GetString()!;
            else if (reader.CurrentDepth == 2)
                result[(row, reader.GetString()!)] = line;
        }
        return result;
    }

    public static string CleanForDisplay(string value)
    {
        var withoutHexTags = HexTag.Replace(value, string.Empty);
        var characters = withoutHexTags.Select(character => char.IsControl(character) ? ' ' : character).ToArray();
        return string.Join(' ', new string(characters).Normalize(NormalizationForm.FormKC)
            .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }

    private static string Normalize(string value) => CleanForDisplay(value).ToUpperInvariant();

    private static bool TryGetSourceField(string translatedField, out string sourceField)
    {
        if (translatedField.Equals("translation", StringComparison.Ordinal))
        {
            sourceField = "original";
            return true;
        }

        const string prefix = "translation_";
        if (translatedField.StartsWith(prefix, StringComparison.Ordinal))
        {
            sourceField = translatedField[prefix.Length..];
            return true;
        }

        sourceField = string.Empty;
        return false;
    }

    private static void Add(
        Dictionary<string, HashSet<TranslationReference>> entriesByText,
        string text,
        TranslationReference reference)
    {
        var key = Normalize(text);
        if (key.Length == 0)
            return;

        if (!entriesByText.TryGetValue(key, out var references))
        {
            references = new HashSet<TranslationReference>();
            entriesByText.Add(key, references);
        }

        references.Add(reference);
    }
}
