using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using FFXIVItalian.Core.SeString;

namespace FFXIVItalian.TextInspector;

internal sealed class InlineTranslation
{
    private static readonly Regex Tags = new("<[^>]+>", RegexOptions.CultureInvariant);
    public string FilePath { get; }
    public TranslationReference Reference { get; }
    public string Field { get; }
    public string Source { get; }
    public string SavedValue { get; private set; }

    public InlineTranslation(string translationsDirectory, TranslationReference reference)
    {
        Reference = reference;
        FilePath = TranslationEditor.ResolveFile(translationsDirectory, reference);
        Field = TranslationEditor.TranslationField(reference);
        var current = Read(File.ReadAllBytes(FilePath));
        Source = current.Source;
        SavedValue = current.Value;
    }

    private (string Source, string Value, int Start, int Length) Read(byte[] bytes)
    {
        var bom = bytes.AsSpan().StartsWith(new byte[] { 0xEF, 0xBB, 0xBF }) ? 3 : 0;
        var content = bytes.AsSpan(bom);
        using var document = JsonDocument.Parse(content.ToArray());
        var rows = document.RootElement.EnumerateObject().Where(x => x.Name == Reference.RowId).ToArray();
        if (rows.Length != 1 || rows[0].Value.ValueKind != JsonValueKind.Object)
            throw new InvalidOperationException("Voce assente o ID duplicato. Ricarica il corpus.");
        var fields = rows[0].Value.EnumerateObject().Where(x => x.Name == Field).ToArray();
        var sources = rows[0].Value.EnumerateObject().Where(x => x.Name == Reference.Field).ToArray();
        if (fields.Length != 1 || fields[0].Value.ValueKind != JsonValueKind.String ||
            sources.Length != 1 || sources[0].Value.ValueKind != JsonValueKind.String)
            throw new InvalidOperationException("Campo sorgente o traduzione assente, duplicato o non testuale.");
        var reader = new Utf8JsonReader(content);
        var row = string.Empty;
        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName) continue;
            if (reader.CurrentDepth == 1) row = reader.GetString()!;
            else if (reader.CurrentDepth == 2 && row == Reference.RowId && reader.ValueTextEquals(Field))
            {
                reader.Read();
                var start = checked((int)reader.TokenStartIndex);
                return (sources[0].Value.GetString()!, fields[0].Value.GetString()!, start + bom, checked((int)reader.BytesConsumed - start));
            }
        }
        throw new InvalidOperationException("Campo non individuato.");
    }

    public string? Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return "La traduzione è vuota: il salvataggio è bloccato.";
        if (value.Length > 65535 || value.Contains('\0')) return "Testo troppo lungo o contenente un carattere nullo.";
        var validation = SeStringValidator.Validate(Source, value);
        if (!validation.IsValid) return string.Join("\n", validation.Errors);
        var before = Tags.Matches(SavedValue).Select(x => x.Value);
        var after = Tags.Matches(value).Select(x => x.Value);
        if (!before.SequenceEqual(after, StringComparer.Ordinal))
            return "Nell'editor in gioco mantieni invariati i tag e i payload esistenti. Per modificarli usa VS Code.";
        return null;
    }

    public string? Save(string value, string backupDirectory)
    {
        if (Validate(value) is { } error) throw new InvalidOperationException(error);
        var bytes = File.ReadAllBytes(FilePath);
        var current = Read(bytes);
        if (current.Value != SavedValue || current.Source != Source)
            throw new InvalidOperationException("Il campo è stato modificato da un altro editor. Riapri la voce prima di salvare; la bozza resta nel pannello.");
        if (value == SavedValue) return null;
        var encoded = JsonSerializer.SerializeToUtf8Bytes(value, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        var updated = new byte[bytes.Length - current.Length + encoded.Length];
        bytes.AsSpan(0, current.Start).CopyTo(updated);
        encoded.CopyTo(updated.AsSpan(current.Start));
        bytes.AsSpan(current.Start + current.Length).CopyTo(updated.AsSpan(current.Start + encoded.Length));
        _ = Read(updated);
        Directory.CreateDirectory(backupDirectory);
        var backup = Path.Combine(backupDirectory, Path.GetFileName(FilePath) + "." + Guid.NewGuid().ToString("N") + ".bak");
        var temporary = Path.Combine(Path.GetDirectoryName(FilePath)!, ".textinspector-" + Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            File.WriteAllBytes(temporary, updated);
            if (!File.ReadAllBytes(FilePath).AsSpan().SequenceEqual(bytes))
                throw new IOException("Il file è cambiato durante il salvataggio. Riprova.");
            File.WriteAllBytes(backup, bytes);
            if (!File.ReadAllBytes(FilePath).AsSpan().SequenceEqual(bytes))
                throw new IOException("Il file è cambiato durante il backup. Riprova.");
            File.Replace(temporary, FilePath, null);
            SavedValue = value;
            return backup;
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }

    public static bool CanPreview(string value) => !value.Contains('\0') && !Tags.IsMatch(value) &&
        !value.Any(c => char.IsControl(c) && c is not '\n' and not '\r' and not '\t');
}
