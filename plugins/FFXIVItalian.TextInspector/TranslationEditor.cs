using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace FFXIVItalian.TextInspector;

internal sealed record EditLocation(string FilePath, string Field, int Line, int Column);

internal static class TranslationEditor
{
    public static EditLocation Locate(string translationsDirectory, TranslationReference reference)
    {
        var file = ResolveFile(translationsDirectory, reference);
        var field = TranslationField(reference);
        var bytes = File.ReadAllBytes(file);
        var content = bytes.AsSpan();
        if (content.StartsWith(new byte[] { 0xEF, 0xBB, 0xBF })) content = content[3..];
        using var document = JsonDocument.Parse(content.ToArray());
        if (!document.RootElement.TryGetProperty(reference.RowId, out var row) ||
            !row.TryGetProperty(field, out var value) || value.ValueKind != JsonValueKind.String)
            throw new InvalidOperationException($"Il campo {reference.RowId}:{field} non esiste più o non è una stringa. Ricarica il corpus.");

        var reader = new Utf8JsonReader(content);
        var rowId = string.Empty;
        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName) continue;
            if (reader.CurrentDepth == 1) rowId = reader.GetString()!;
            else if (reader.CurrentDepth == 2 && rowId == reference.RowId && reader.ValueTextEquals(field))
            {
                reader.Read();
                var offset = checked((int)reader.TokenStartIndex + 1);
                var before = content[..offset];
                var line = 1;
                foreach (var character in before)
                    if (character == '\n') line++;
                var lineStart = before.LastIndexOf((byte)'\n') + 1;
                var column = Encoding.UTF8.GetCharCount(content[lineStart..offset]) + 1;
                return new EditLocation(file, field, line, column);
            }
        }
        throw new InvalidOperationException("Impossibile individuare il campo di traduzione.");
    }

    internal static string TranslationField(TranslationReference reference) =>
        reference.Field == "original" ? "translation" : "translation_" + reference.Field;

    internal static string ResolveFile(string translationsDirectory, TranslationReference reference)
    {
        var root = Path.GetFullPath(translationsDirectory);
        var relative = reference.FilePath.Replace('\\', '/');
        const string prefix = "data/translations/";
        if (!relative.StartsWith(prefix, StringComparison.Ordinal))
            throw new InvalidOperationException("Il riferimento non appartiene al corpus delle traduzioni.");
        var file = Path.GetFullPath(Path.Combine(root, relative[prefix.Length..]));
        var withinRoot = Path.GetRelativePath(root, file);
        if (Path.IsPathRooted(withinRoot) || withinRoot == ".." || withinRoot.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            throw new InvalidOperationException("Il percorso esce dalla cartella delle traduzioni.");

        return file;
    }

    internal static ProcessStartInfo CreateStartInfo(string executable, EditLocation location)
    {
        var start = new ProcessStartInfo(executable) { UseShellExecute = false, CreateNoWindow = true };
        start.ArgumentList.Add("--reuse-window");
        start.ArgumentList.Add("--goto");
        start.ArgumentList.Add($"{location.FilePath}:{location.Line}:{location.Column}");
        return start;
    }

    public static void Open(string translationsDirectory, TranslationReference reference)
    {
        var location = Locate(translationsDirectory, reference);
        var executable = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Microsoft VS Code", "Code.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft VS Code", "Code.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft VS Code", "Code.exe"),
        }.FirstOrDefault(File.Exists) ?? throw new FileNotFoundException("VS Code non trovato nelle cartelle di installazione standard.");
        using var process = Process.Start(CreateStartInfo(executable, location));
        if (process is null) throw new InvalidOperationException("Impossibile avviare VS Code.");
    }
}
