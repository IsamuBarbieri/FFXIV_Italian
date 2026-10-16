using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using FFXIVItalian.Core.Packaging;
using FFXIVItalian.Core.SeString;
using FFXIVItalian.Core.Translation;
using Lumina;

namespace FFXIVItalian.Patcher;

public static class TranslationBuildVerifier
{
    public sealed record Coverage(int Files, int Fields);

    public static Coverage Verify(string translationsDir, GameData gameData, IReadOnlyDictionary<string, byte[]> files)
    {
        var catalog = Encoding.UTF8.GetString(gameData.GetFile("exd/root.exl")!.Data)
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(',')[0].Trim()).Where(sheet => !sheet.StartsWith("EXLT"))
            .GroupBy(sheet => sheet.Split('/').Last(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.OrdinalIgnoreCase);
        var errors = new List<string>();
        int fieldCount = 0, fileCount = 0;
        foreach (string path in Directory.EnumerateFiles(translationsDir, "*.json", SearchOption.AllDirectories))
        {
            using var document = JsonDocument.Parse(File.ReadAllText(path));
            var translatedRows = document.RootElement.EnumerateObject()
                .Where(row => row.Value.ValueKind == JsonValueKind.Object && row.Value.EnumerateObject().Any(IsTranslation)).ToArray();
            if (translatedRows.Length == 0) continue;
            fileCount++;
            string relative = Path.GetRelativePath(translationsDir, path);
            catalog.TryGetValue(Path.GetFileNameWithoutExtension(path), out var candidates);
            string? sheet = TranslationPathResolver.ResolveSheetForFile(path, candidates ?? [])?.ToLowerInvariant();
            var exh = sheet == null ? null : gameData.GetFile($"exd/{sheet}.exh");
            if (exh == null)
            {
                errors.Add($"{relative}: foglio assente dal client.");
                continue;
            }
            int fixedSize = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(6, 2));
            int columnCount = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(8, 2));
            int pageCount = BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(10, 2));
            var offsets = new List<int>();
            for (int column = 0; column < columnCount; column++)
                if (BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(32 + column * 4, 2)) == 0)
                    offsets.Add(BinaryPrimitives.ReadUInt16BigEndian(exh.Data.AsSpan(34 + column * 4, 2)));
            var pages = new List<uint>();
            for (int page = 0; page < pageCount; page++)
                pages.Add(BinaryPrimitives.ReadUInt32BigEndian(exh.Data.AsSpan(32 + columnCount * 4 + page * 8, 4)));
            var indices = new Dictionary<string, Dictionary<uint, int>>();
            foreach (var row in translatedRows)
            {
                foreach (var field in row.Value.EnumerateObject().Where(IsTranslation))
                {
                    fieldCount++;
                    string reference = $"{relative}#{row.Name}:{field.Name}";
                    if (!uint.TryParse(row.Name, out uint rowId))
                    {
                        errors.Add($"{reference}: ID non valido.");
                        continue;
                    }
                    var matchingPages = pages.Where(start => start <= rowId).ToArray();
                    if (matchingPages.Length == 0)
                    {
                        errors.Add($"{reference}: nessuna pagina per l'ID.");
                        continue;
                    }
                    string exdPath = $"exd/{sheet}_{matchingPages[^1]}_en.exd";
                    if (!files.TryGetValue(exdPath, out byte[]? data))
                    {
                        errors.Add($"{reference}: pagina non inclusa ({exdPath}).");
                        continue;
                    }
                    if (!indices.TryGetValue(exdPath, out var index))
                    {
                        index = new Dictionary<uint, int>();
                        int indexSize = (int)BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(8, 4));
                        for (int pos = 32; pos < 32 + indexSize; pos += 8)
                            index.Add(BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(pos, 4)),
                                (int)BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(pos + 4, 4)));
                        indices[exdPath] = index;
                    }
                    if (!index.TryGetValue(rowId, out int rowOffset))
                    {
                        errors.Add($"{reference}: riga non inclusa.");
                        continue;
                    }
                    try
                    {
                        byte[] expected = SeStringEncoder.Encode(field.Value.GetString()!);
                        foreach (int offset in GetOffsets(sheet!, field.Name, offsets))
                            VerifyField(data, rowOffset, fixedSize, offset, expected);
                    }
                    catch (Exception ex) when (ex is InvalidDataException or ArgumentException)
                    {
                        errors.Add($"{reference}: {ex.Message}");
                    }
                }
            }
        }
        if (errors.Count > 0)
            throw new InvalidDataException($"Verifica traduzioni: {errors.Count} errori su {fieldCount} campi.\n" +
                string.Join('\n', errors.GroupBy(error => error.Split('#')[0]).Select(group => $"{group.Key}: {group.Count()} errori")) + "\n" +
                string.Join('\n', errors.Take(20)));
        Console.WriteLine($"[Verifica traduzioni] {fieldCount} campi in {fileCount} fogli confrontati byte per byte, inclusi i SeString.");
        return new Coverage(fileCount, fieldCount);
    }

    private static bool IsTranslation(JsonProperty field) =>
        (field.Name == "translation" || field.Name.StartsWith("translation_", StringComparison.Ordinal)) &&
        field.Value.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(field.Value.GetString());

    public static int[] GetOffsets(string sheet, string field, IReadOnlyList<int> offsets)
    {
        if (sheet == "classjob")
            return field switch
            {
                "translation" => [0, 16],
                "translation_col_4" => [4],
                _ => throw new InvalidDataException($"Campo ClassJob non gestito: {field}.")
            };
        if (sheet == "placename" && field == "translation") return [0, 4];
        int ordinal = field switch
        {
            "translation" when sheet.StartsWith("quest/", StringComparison.Ordinal) || sheet == "contentfindercondition" => 1,
            "translation" or "translation_name" or "translation_name_masculine" => 0,
            "translation_description" when sheet == "lobby" => 2,
            "translation_description" or "translation_alt" or "translation_name_feminine" => 1,
            _ when field.StartsWith("translation_col_", StringComparison.Ordinal) && int.TryParse(field[16..], out int column) => column,
            _ => throw new InvalidDataException($"Campo non gestito: {field}.")
        };
        if (ordinal >= offsets.Count)
            throw new InvalidDataException($"Colonna stringa {ordinal} assente nel foglio {sheet}.");
        return [offsets[ordinal]];
    }

    public static void VerifyField(byte[] data, int rowOffset, int fixedSize, int columnOffset, byte[] expected)
    {
        int dataSize = checked((int)BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(rowOffset, 4)));
        int stringOffset = checked((int)BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(rowOffset + 6 + columnOffset, 4)));
        int start = checked(rowOffset + 6 + fixedSize + stringOffset);
        int end = checked(rowOffset + 6 + dataSize);
        if (start < rowOffset + 6 + fixedSize || start + expected.Length >= end || end > data.Length ||
            !data.AsSpan(start, expected.Length).SequenceEqual(expected) || data[start + expected.Length] != 0)
            throw new InvalidDataException($"Testo EXD diverso dalla traduzione alla colonna offset {columnOffset}.");
    }

    public static void VerifyPackage(string path, PenumbraMeta meta, IReadOnlyDictionary<string, byte[]> files)
    {
        using var archive = ZipFile.OpenRead(path);
        var registrations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        void ReadFiles(JsonElement container)
        {
            foreach (var file in container.GetProperty("Files").EnumerateObject())
                if (!registrations.TryAdd(file.Name, file.Value.GetString()!))
                    throw new InvalidDataException($"Registrazione Penumbra duplicata: {file.Name}.");
        }
        using var metadataStream = archive.GetEntry("meta.json")!.Open();
        using var metadata = JsonDocument.Parse(metadataStream);
        if (metadata.RootElement.GetProperty("FileVersion").GetInt32() != 4)
            throw new InvalidDataException("Il verificatore richiede il formato Penumbra v4.");
        ReadFiles(metadata.RootElement.GetProperty("DefaultData"));
        var groups = metadata.RootElement.GetProperty("Groups");
        if (groups.GetArrayLength() != meta.Groups.Count)
            throw new InvalidDataException("Numero di blocchi Penumbra incoerente.");
        foreach (var group in groups.EnumerateArray())
        {
            if (group.GetProperty("DefaultSettings").GetUInt64() != 1)
                throw new InvalidDataException("Blocco Penumbra non attivo per impostazione predefinita.");
            foreach (var option in group.GetProperty("Options").EnumerateArray()) ReadFiles(option);
        }
        using var defaultsStream = archive.GetEntry("default_mod.json")!.Open();
        using var defaults = JsonDocument.Parse(defaultsStream);
        if (!defaults.RootElement.GetProperty("Files").EnumerateObject().Select(file => (file.Name, file.Value.GetString()))
            .OrderBy(file => file.Name).SequenceEqual(metadata.RootElement.GetProperty("DefaultData").GetProperty("Files")
                .EnumerateObject().Select(file => (file.Name, file.Value.GetString())).OrderBy(file => file.Name)))
            throw new InvalidDataException("default_mod.json diverso dai dati predefiniti v4.");
        if (registrations.Count != files.Count || archive.Entries.Count != files.Count + 2)
            throw new InvalidDataException("Numero di file o registrazioni del PMP incoerente.");
        foreach (var (gamePath, expected) in files)
        {
            if (!registrations.TryGetValue(gamePath, out string? target) || target != gamePath)
                throw new InvalidDataException($"File non registrato in Penumbra: {gamePath}.");
            using var stream = archive.GetEntry(target)!.Open();
            using var content = new MemoryStream();
            stream.CopyTo(content);
            if (!content.ToArray().AsSpan().SequenceEqual(expected))
                throw new InvalidDataException($"Contenuto PMP diverso dalla build: {gamePath}.");
        }
        Console.WriteLine($"[Verifica PMP] {files.Count} file registrati una sola volta e verificati nell'archivio, {meta.Groups.Count} blocchi attivi.");
    }
}
