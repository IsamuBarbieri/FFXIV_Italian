using System.Text.Json;

namespace FFXIVItalian.Core.Glossary;

public sealed record GlossaryFinding(string RowId, string Field, string Original, string Translation, string Message);
public sealed record GlossaryFileAudit(bool IsComplete, int CheckedFields, IReadOnlyList<GlossaryFinding> Findings);

public static class GlossaryAudit
{
    public static GlossaryFileAudit AuditFile(string filePath, GlossaryEngine engine)
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(filePath));
        if (doc.RootElement.ValueKind != JsonValueKind.Object)
            throw new FormatException($"JSON di traduzione non valido: {filePath}");

        var findings = new List<GlossaryFinding>();
        int checkedFields = 0;
        bool complete = true;
        foreach (var row in doc.RootElement.EnumerateObject())
        {
            if (row.Value.ValueKind == JsonValueKind.String)
            {
                complete = false; // No source text is available for a reliable audit.
                continue;
            }
            if (row.Value.ValueKind != JsonValueKind.Object) continue;
            foreach (var source in row.Value.EnumerateObject())
            {
                if (source.Name.StartsWith("translation", StringComparison.Ordinal) ||
                    source.Name == "tag" || source.Value.ValueKind != JsonValueKind.String) continue;
                // CustomTalk's name and columns 2-30 are engine identifiers.
                if (Path.GetFileName(filePath).Equals("customtalk.json", StringComparison.OrdinalIgnoreCase) &&
                    (source.Name == "name" || (source.Name.StartsWith("col_", StringComparison.Ordinal) &&
                     int.TryParse(source.Name[4..], out var column) && column < 31))) continue;

                string targetName = "translation_" + source.Name;
                if ((source.Name == "original" || source.Name == "name") &&
                    !row.Value.TryGetProperty(targetName, out _) &&
                    row.Value.TryGetProperty("translation", out _)) targetName = "translation";
                string original = source.Value.GetString() ?? "";
                if (string.IsNullOrWhiteSpace(original)) continue;
                row.Value.TryGetProperty(targetName, out var target);
                checkedFields++;
                string translated = target.ValueKind == JsonValueKind.String ? target.GetString() ?? "" : "";
                if (string.IsNullOrWhiteSpace(translated))
                {
                    complete = false;
                    continue;
                }
                if (Path.GetFileName(filePath).Equals("placename.json", StringComparison.OrdinalIgnoreCase) &&
                    source.Name == "name" && (original == "Dungeon" || original == "Raid")) continue;
                if (Path.GetFileName(filePath).Equals("achievement.json", StringComparison.OrdinalIgnoreCase) &&
                    source.Name == "name") continue; // Achievement titles are proper names.
                var result = engine.ValidateTranslation(original, translated, filePath);
                foreach (var warning in result.Warnings)
                    findings.Add(new GlossaryFinding(row.Name, targetName, original, translated, warning));
            }
        }
        return new GlossaryFileAudit(complete && checkedFields > 0, checkedFields, findings);
    }
}
