using System.Text;
using System.Buffers.Binary;
using System.Text.Json;
using System.Text.Json.Nodes;
using FFXIVItalian.Core.Translation;
using FFXIVItalian.Extractor.Extractors;
using Lumina;

namespace FFXIVItalian.Extractor;

public record UpdateChange(string File, string Id, string Kind, JsonNode? Before, JsonNode? After, string[]? Fields = null);

public static class GameUpdate
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    // Keep translations and editorial metadata, including removed rows, for human review.
    public static JsonObject Merge(string file, JsonObject before, JsonObject extracted, List<UpdateChange> changes)
    {
        var merged = (JsonObject)before.DeepClone();
        foreach (var (id, node) in extracted)
        {
            if (node is not JsonObject fresh) throw new InvalidDataException($"{file}#{id}: riga non strutturata.");
            if (before[id] is not JsonObject old)
            {
                merged[id] = fresh.DeepClone();
                changes.Add(new(file, id, "added", null, fresh.DeepClone()));
                continue;
            }
            var updated = (JsonObject)old.DeepClone();
            if (old.ContainsKey("original") && !fresh.ContainsKey("original"))
                throw new InvalidDataException($"{file}#{id}: schema sorgente cambiato; serve un allineamento esplicito delle colonne.");
            foreach (var (field, value) in fresh)
            {
                if (field == "translation" || field.StartsWith("translation_"))
                {
                    if (!updated.ContainsKey(field)) updated[field] = value?.DeepClone();
                }
                else updated[field] = value?.DeepClone();
            }
            var fields = fresh.Where(p => p.Key != "translation" && !p.Key.StartsWith("translation_")
                && !JsonNode.DeepEquals(old[p.Key], p.Value)).Select(p => p.Key).ToList();
            // A removed source column is a schema change; retain its translation for review.
            foreach (var (field, _) in old)
                if (old.ContainsKey("translation_" + field) && !fresh.ContainsKey(field)) fields.Add(field);
            if (fields.Count > 0)
                changes.Add(new(file, id, "modified", old.DeepClone(), updated.DeepClone(), fields.ToArray()));
            merged[id] = updated;
        }
        foreach (var (id, old) in before)
            if (!extracted.ContainsKey(id)) changes.Add(new(file, id, "removed", old?.DeepClone(), null));
        return merged;
    }

    public static int Run(GameData game, string translationsDir, string sqpackPath)
    {
        string dataDir = Directory.GetParent(Path.GetFullPath(translationsDir))!.FullName;
        string runDir = Path.Combine(dataDir, "updates", DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N")[..6]);
        Directory.CreateDirectory(runDir);
        var files = TranslationPathResolver.GetCorpusFiles(translationsDir)
            .GroupBy(Path.GetFileNameWithoutExtension, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key!, g => g.Single(), StringComparer.OrdinalIgnoreCase);
        var root = game.GetFile("exd/root.exl") ?? throw new FileNotFoundException("exd/root.exl non trovato.");
        var sheets = Encoding.UTF8.GetString(root.Data).Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(',')[0].Trim()).Where(s => s.Length > 0 && !s.StartsWith("EXLT"))
            .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var changes = new List<UpdateChange>();
        var newFiles = new List<string>();
        var errors = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int checkedFiles = 0;
        foreach (string sheet in sheets)
        {
            string name = sheet.Split('/').Last();
            files.TryGetValue(name, out string? existing);
            if (existing != null) seen.Add(existing);
            string relative = existing != null ? Path.GetRelativePath(dataDir, existing)
                : Path.Combine("da_tradurre", TranslationPathResolver.GetTargetSubdirectory(sheet), name.ToLowerInvariant() + ".json");
            string scratch = Path.Combine(runDir, "scratch", "translations");
            string extractedPath = Path.Combine(scratch, TranslationPathResolver.GetTargetSubdirectory(sheet), name.ToLowerInvariant() + ".json");
            int changeStart = changes.Count;
            try
            {
                var exh = game.GetFile($"exd/{sheet.ToLowerInvariant()}.exh")
                    ?? throw new FileNotFoundException($"EXH mancante: {sheet}");
                var bytes = exh.Data;
                ushort columns = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(8, 2));
                ushort pages = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(10, 2));
                ushort languages = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(12, 2));
                bool hasStrings = Enumerable.Range(0, columns).Any(c => BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(32 + c * 4, 2)) == 0);
                int languageStart = 32 + columns * 4 + pages * 8;
                bool hasEnglish = Enumerable.Range(0, languages).Any(l => bytes[languageStart + l * 2] == 2);
                if (!hasStrings || !hasEnglish)
                {
                    if (existing != null) errors.Add($"{sheet}: foglio del corpus senza colonne testuali o lingua inglese.");
                    continue;
                }
                // Existing extractors skip missing pages: an update must fail instead of reporting false removals.
                for (int p = 0; p < pages; p++)
                {
                    uint startRow = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(32 + columns * 4 + p * 8, 4));
                    if (game.GetFile($"exd/{sheet.ToLowerInvariant()}_{startRow}_en.exd") == null)
                        throw new FileNotFoundException($"Pagina inglese mancante: {sheet}_{startRow}_en.exd");
                }
                // Seed only the scratch file: extractors also retain translated empty source rows.
                Directory.CreateDirectory(Path.GetDirectoryName(extractedPath)!);
                var before = existing == null ? new JsonObject() : JsonNode.Parse(File.ReadAllText(existing)) as JsonObject
                    ?? throw new InvalidDataException($"JSON non strutturato: {existing}");
                if (existing != null) File.Copy(existing, extractedPath, true);
                if (sheet.StartsWith("quest/", StringComparison.OrdinalIgnoreCase))
                    QuestExtractor.ExtractQuest(game, sheet, scratch);
                else if (sheet.Equals("GuardianDeity", StringComparison.OrdinalIgnoreCase) && before.Any(p => p.Value is JsonObject row && row.ContainsKey("original")))
                    // Canonical GuardianDeity and its patcher use only the first string column.
                    new UniversalSheetExtractor(sheet, selectedStringColumn: 0).ExtractAndSave(game, extractedPath);
                else
                    ExtractorRegistry.Get(sheet)!.ExtractAndSave(game, extractedPath);
                if (!File.Exists(extractedPath)) continue;
                var fresh = JsonNode.Parse(File.ReadAllText(extractedPath)) as JsonObject
                    ?? throw new InvalidDataException($"Estrazione non strutturata: {sheet}");
                if (existing == null && fresh.Count == 0) continue;
                int start = changes.Count;
                var merged = Merge(relative, before, fresh, changes);
                checkedFiles++;
                if (existing == null) newFiles.Add(relative);
                if (changes.Count > start || existing == null)
                {
                    string output = Path.Combine(runDir, "updated", relative);
                    Directory.CreateDirectory(Path.GetDirectoryName(output)!);
                    File.WriteAllText(output, merged.ToJsonString(Options));
                    if (existing != null)
                    {
                        string backup = Path.Combine(runDir, "backup", relative);
                        Directory.CreateDirectory(Path.GetDirectoryName(backup)!);
                        File.Copy(existing, backup);
                    }
                }
            }
            catch (Exception ex)
            {
                changes.RemoveRange(changeStart, changes.Count - changeStart);
                newFiles.Remove(relative);
                errors.Add($"{sheet}: {ex.Message}");
            }
            // Scratch contents are not a baseline; each sheet is extracted independently.
            if (File.Exists(extractedPath)) File.Delete(extractedPath);
            if (checkedFiles % 250 == 0) Console.WriteLine($"Verificati {checkedFiles} file, {changes.Count} differenze...");
        }
        foreach (string missing in files.Values.Where(f => !seen.Contains(f)))
            errors.Add($"File del corpus non associato a root.exl: {Path.GetRelativePath(dataDir, missing)}");
        string? versionPath = Path.Combine(Directory.GetParent(sqpackPath)!.FullName, "ffxivgame.ver");
        var report = new { GameVersion = File.Exists(versionPath) ? File.ReadAllText(versionPath).Trim() : "unknown",
            CheckedFiles = checkedFiles, NewFiles = newFiles, Changes = changes, Errors = errors };
        File.WriteAllText(Path.Combine(runDir, "report.json"), JsonSerializer.Serialize(report, Options));
        var md = new StringBuilder($"# Aggiornamento {report.GameVersion}\n\nFile verificati: {checkedFiles}. File nuovi: {newFiles.Count}. Righe aggiunte: {changes.Count(c => c.Kind == "added")}. Righe modificate: {changes.Count(c => c.Kind == "modified")}. Righe rimosse: {changes.Count(c => c.Kind == "removed")}. Errori/limiti: {errors.Count}.\n\nLe copie in `updated/` conservano traduzioni e metadati. Le righe modificate richiedono revisione; le righe rimosse restano conservate e non vanno applicate alla cieca. Il corpus e gli stati editoriali non sono stati modificati. `backup/` contiene gli originali dei file interessati.\n\n## File nuovi\n\n");
        foreach (string file in newFiles) md.AppendLine($"- `{file}`");
        md.AppendLine("\n## Righe da tradurre o ricontrollare\n\n| File | ID | Tipo | Campi modificati |\n| --- | --- | --- | --- |");
        foreach (var c in changes) md.AppendLine($"| `{c.File}` | {c.Id} | {c.Kind} | {string.Join(", ", c.Fields ?? [])} |");
        md.AppendLine("\n## Errori e limiti\n");
        foreach (string error in errors) md.AppendLine($"- {error}");
        File.WriteAllText(Path.Combine(runDir, "report.md"), md.ToString());
        Console.WriteLine($"Rapporto e copie aggiornate: {runDir}");
        Console.WriteLine($"File nuovi: {newFiles.Count}; righe aggiunte: {changes.Count(c => c.Kind == "added")}; modificate: {changes.Count(c => c.Kind == "modified")}; rimosse: {changes.Count(c => c.Kind == "removed")}; errori/limiti: {errors.Count}.");
        return errors.Count == 0 ? 0 : 1;
    }
}
