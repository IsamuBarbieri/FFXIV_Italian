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
                else if (!SourceEquals(old[field], value)) updated[field] = value?.DeepClone();
            }
            var fields = fresh.Where(p => p.Key != "translation" && !p.Key.StartsWith("translation_")
                && !SourceEquals(old[p.Key], p.Value)).Select(p => p.Key).ToList();
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

    private static bool SourceEquals(JsonNode? before, JsonNode? after)
    {
        if (JsonNode.DeepEquals(before, after)) return true;
        if (before is JsonValue left && after is JsonValue right &&
            left.TryGetValue<string>(out var oldText) && right.TryGetValue<string>(out var newText) &&
            (oldText.Contains("<hex:") || newText.Contains("<hex:")))
            return FFXIVItalian.Core.SeString.SeStringEncoder.Encode(oldText)
                .SequenceEqual(FFXIVItalian.Core.SeString.SeStringEncoder.Encode(newText));
        return false;
    }

    public static int Run(GameData game, string translationsDir, string sqpackPath, bool integrate = false)
    {
        string dataDir = Directory.GetParent(Path.GetFullPath(translationsDir))!.FullName;
        string runDir = Path.Combine(dataDir, "updates", DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N")[..6]);
        Directory.CreateDirectory(runDir);
        var files = TranslationPathResolver.GetCorpusFiles(translationsDir)
            .GroupBy(Path.GetFileNameWithoutExtension, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key!, g => g.ToArray(), StringComparer.OrdinalIgnoreCase);
        var root = game.GetFile("exd/root.exl") ?? throw new FileNotFoundException("exd/root.exl non trovato.");
        var sheets = Encoding.UTF8.GetString(root.Data).Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(',')[0].Trim()).Where(s => s.Length > 0 && !s.StartsWith("EXLT"))
            .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var sheetCounts = sheets.GroupBy(s => s.Split('/').Last(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);
        var changes = new List<UpdateChange>();
        var newFiles = new List<string>();
        var errors = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int checkedFiles = 0;
        foreach (string sheet in sheets)
        {
            string name = sheet.Split('/').Last();
            files.TryGetValue(name, out string[]? candidates);
            string? existing = MatchExistingFile(sheet, candidates ?? [], sheetCounts[name] > 1);
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
                if (BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(16, 2)) != 1)
                    throw new InvalidDataException("Variante EXH non supportata: confronto sospeso per evitare falsi risultati.");
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
        foreach (string missing in files.Values.SelectMany(f => f).Where(f => !seen.Contains(f)))
            errors.Add($"File del corpus non associato a root.exl: {Path.GetRelativePath(dataDir, missing)}");
        string? versionPath = Path.Combine(Directory.GetParent(sqpackPath)!.FullName, "ffxivgame.ver");
        var report = new { GameVersion = File.Exists(versionPath) ? File.ReadAllText(versionPath).Trim() : "unknown",
            CheckedFiles = checkedFiles, NewFiles = newFiles, Changes = changes, Errors = errors };
        File.WriteAllText(Path.Combine(runDir, "report.json"), JsonSerializer.Serialize(report, Options));
        var existingChanges = changes.Where(c => !newFiles.Contains(c.File)).ToList();
        var md = new StringBuilder($"# Aggiornamento {report.GameVersion}\n\nFile verificati: {checkedFiles}. File nuovi: {newFiles.Count}. Righe aggiunte a file esistenti: {existingChanges.Count(c => c.Kind == "added")}. Righe modificate: {existingChanges.Count(c => c.Kind == "modified")}. Righe rimosse: {existingChanges.Count(c => c.Kind == "removed")}. Errori/limiti: {errors.Count}.\n\nLe copie in `updated/` conservano traduzioni e metadati. Le righe modificate richiedono revisione; le righe rimosse restano conservate e non vanno applicate alla cieca. `backup/` contiene gli originali dei file interessati.\n\n## File nuovi\n\n");
        foreach (string file in newFiles) md.AppendLine($"- `{file}`");
        md.AppendLine("\n## Pianificazione per file esistente\n\n| File | Righe nuove | Righe modificate | Righe rimosse |\n| --- | ---: | ---: | ---: |");
        foreach (var group in existingChanges.GroupBy(c => c.File))
            md.AppendLine($"| `{group.Key}` | {group.Count(c => c.Kind == "added")} | {group.Count(c => c.Kind == "modified")} | {group.Count(c => c.Kind == "removed")} |");
        md.AppendLine("\n## Righe da tradurre o ricontrollare\n\n| File | ID | Tipo | Campi modificati |\n| --- | --- | --- | --- |");
        foreach (var c in existingChanges) md.AppendLine($"| `{c.File}` | {c.Id} | {c.Kind} | {string.Join(", ", c.Fields ?? [])} |");
        var review = new StringBuilder("# Sorgenti modificate da revisionare\n\nLa traduzione attuale è conservata. Il corpus mantiene la sorgente precedente finché la voce non viene ricontrollata.\n\n");
        foreach (var c in existingChanges.Where(c => c.Kind == "modified"))
        {
            review.AppendLine($"## {c.File}#{c.Id}\n");
            foreach (string field in c.Fields ?? [])
            {
                string translation = field == "original" ? "translation" : "translation_" + field;
                review.AppendLine($"### {field}\n\nPrima:\n\n```text\n{c.Before?[field]}\n```\n\nDopo:\n\n```text\n{c.After?[field]}\n```\n\nTraduzione attuale:\n\n```text\n{c.Before?[translation]}\n```\n");
            }
        }
        File.WriteAllText(Path.Combine(runDir, "review.md"), review.ToString());
        md.AppendLine("\n## Errori e limiti\n");
        foreach (string error in errors) md.AppendLine($"- {error}");
        File.WriteAllText(Path.Combine(runDir, "report.md"), md.ToString());
        if (integrate && errors.Count == 0)
        {
            int added = IntegrateAdditions(dataDir, runDir, newFiles, changes);
            File.AppendAllText(Path.Combine(runDir, "report.md"), $"\nIntegrazione completata: {newFiles.Count} file nuovi in da_tradurre e {added} righe nuove nei file esistenti. Sorgenti modificate e righe rimosse lasciate intatte nel corpus.\n");
        }
        else if (integrate) Console.Error.WriteLine("Integrazione sospesa: risolvere prima gli errori del rapporto. Nessun file del corpus modificato.");
        Console.WriteLine($"Rapporto e copie aggiornate: {runDir}");
        Console.WriteLine($"File nuovi: {newFiles.Count}; righe aggiunte a file esistenti: {existingChanges.Count(c => c.Kind == "added")}; modificate: {existingChanges.Count(c => c.Kind == "modified")}; rimosse: {existingChanges.Count(c => c.Kind == "removed")}; errori/limiti: {errors.Count}.");
        return errors.Count == 0 ? 0 : 1;
    }

    public static string? MatchExistingFile(string sheet, string[] candidates, bool ambiguousSheetName)
    {
        string suffix = Path.Combine(TranslationPathResolver.GetTargetSubdirectory(sheet), sheet.Split('/').Last().ToLowerInvariant() + ".json");
        var matches = candidates.Where(file => file.EndsWith(Path.DirectorySeparatorChar + suffix, StringComparison.OrdinalIgnoreCase)).ToArray();
        if (matches.Length == 1) return matches[0];
        if (matches.Length > 1 || (!ambiguousSheetName && candidates.Length > 1))
            throw new InvalidDataException($"Più file editoriali associati al foglio {sheet}: risolvere il duplicato prima dell'aggiornamento.");
        return ambiguousSheetName ? null : candidates.SingleOrDefault();
    }

    public static int IntegrateAdditions(string dataDir, string runDir, IReadOnlyCollection<string> newFiles, IEnumerable<UpdateChange> changes)
    {
        var plans = new List<(string Path, byte[] Bytes)>();
        int added = 0;
        foreach (string relative in newFiles.Concat(changes.Where(c => c.Kind == "added").Select(c => c.File)).Distinct())
        {
            string target = Path.GetFullPath(Path.Combine(dataDir, relative));
            string root = Path.GetFullPath(dataDir) + Path.DirectorySeparatorChar;
            if (!target.StartsWith(root, StringComparison.OrdinalIgnoreCase) ||
                !new[] { "translations", "da_revisionare", "da_tradurre" }.Contains(relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)[0]))
                throw new InvalidDataException($"Destinazione non valida: {relative}");
            if (newFiles.Contains(relative))
            {
                if (File.Exists(target)) throw new IOException($"File comparso dopo il confronto: {relative}");
                plans.Add((target, File.ReadAllBytes(Path.Combine(runDir, "updated", relative))));
                continue;
            }
            byte[] before = File.ReadAllBytes(Path.Combine(runDir, "backup", relative));
            if (!File.ReadAllBytes(target).SequenceEqual(before))
                throw new IOException($"File modificato dopo il confronto: {relative}. Ripetere update.");
            bool bom = before.AsSpan().StartsWith(new byte[] { 0xef, 0xbb, 0xbf });
            string text = Encoding.UTF8.GetString(before, bom ? 3 : 0, before.Length - (bom ? 3 : 0));
            var old = JsonNode.Parse(text)!.AsObject();
            var additions = new JsonObject();
            foreach (var change in changes.Where(c => c.File == relative && c.Kind == "added"))
            {
                if (old.ContainsKey(change.Id)) throw new InvalidDataException($"ID già presente: {relative}#{change.Id}");
                additions.Add(change.Id, change.After?.DeepClone());
                added++;
            }
            // Append only: keep every existing row and its formatting byte-for-byte.
            int end = text.LastIndexOf('}');
            string newline = text.Contains("\r\n") ? "\r\n" : "\n";
            string fragment = additions.ToJsonString(Options).Replace("\r\n", "\n");
            fragment = fragment[(fragment.IndexOf('\n') + 1)..fragment.LastIndexOf('\n')].Replace("\n", newline);
            string prefix = text[..end].TrimEnd();
            string result = prefix + (old.Count > 0 ? "," : "") + newline + fragment + newline + text[end..];
            JsonNode.Parse(result);
            var encoding = new UTF8Encoding(bom);
            plans.Add((target, [.. encoding.GetPreamble(), .. encoding.GetBytes(result)]));
        }
        foreach (var plan in plans)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(plan.Path)!);
            string temporary = plan.Path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try { File.WriteAllBytes(temporary, plan.Bytes); File.Move(temporary, plan.Path, true); }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
        return added;
    }
}
