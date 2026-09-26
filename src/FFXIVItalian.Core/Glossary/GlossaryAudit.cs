using System.Text.Json;

namespace FFXIVItalian.Core.Glossary;

public sealed record GlossaryFinding(string RowId, string Field, string Message);
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
                    source.Value.ValueKind != JsonValueKind.String) continue;

                string targetName = "translation_" + source.Name;
                if ((source.Name == "original" || source.Name == "name") &&
                    !row.Value.TryGetProperty(targetName, out _) &&
                    row.Value.TryGetProperty("translation", out _)) targetName = "translation";
                if (!row.Value.TryGetProperty(targetName, out var target)) continue;
                string original = source.Value.GetString() ?? "";
                if (string.IsNullOrWhiteSpace(original)) continue;
                checkedFields++;
                string translated = target.ValueKind == JsonValueKind.String ? target.GetString() ?? "" : "";
                if (string.IsNullOrWhiteSpace(translated))
                {
                    complete = false;
                    continue;
                }
                var result = engine.ValidateTranslation(original, translated);
                foreach (var warning in result.Warnings)
                    findings.Add(new GlossaryFinding(row.Name, targetName, warning));
            }
        }
        return new GlossaryFileAudit(complete && checkedFields > 0, checkedFields, findings);
    }
}
