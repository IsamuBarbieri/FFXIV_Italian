using System.Text.Json;
using System.Text.RegularExpressions;
using FFXIVItalian.Core.Glossary;

namespace FFXIVItalian.Tests;

public class GlossaryTests
{
    private readonly GlossaryCatalog _catalog = GlossaryLoader.LoadCanonical();

    [Fact]
    public void EveryEntryIsPresentInAnApprovedSource()
    {
        Assert.Equal(10, _catalog.ApprovedFiles.Count);
        Assert.NotEmpty(_catalog.Engine.Entries);
        string root = FindRepoRoot();
        foreach (var entry in _catalog.Engine.Entries)
        {
            Assert.Contains(entry.SourceFile, _catalog.ApprovedFiles);
            using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "data", "translations", entry.SourceFile)));
            var row = doc.RootElement.GetProperty(entry.RowId);
            string targetField = "translation_" + entry.SourceField;
            if (!row.TryGetProperty(targetField, out _) &&
                (entry.SourceField == "original" || entry.SourceField == "name")) targetField = "translation";
            Assert.Equal(entry.EnglishTerm, row.GetProperty(entry.SourceField).GetString());
            Assert.Equal(entry.ItalianTerm, row.GetProperty(targetField).GetString());
        }
    }

    [Fact]
    public void VariantsAreAcceptedAndOtherExactValuesAreFlagged()
    {
        Assert.True(_catalog.Engine.ValidateTranslation("Return", "Ritorna").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation("Return", "Indietro").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation("Duty", "Incarico").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation("Duty", "Incarichi").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation("Duty", "Missione").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation("Abandon your current duty?", "Abbandonare l'incarico?").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation("Abandon your current duty?", "Abbandonare la missione?").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation("Return", "Back").IsCompliant);
    }

    [Fact]
    public void ApprovedFilesDoNotRetainDutyInItalian()
    {
        string root = FindRepoRoot();
        foreach (var file in _catalog.ApprovedFiles)
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "data", "translations", file)));
            foreach (var row in doc.RootElement.EnumerateObject())
            {
                if (row.Value.ValueKind != JsonValueKind.Object) continue;
                foreach (var field in row.Value.EnumerateObject())
                    if (field.Name.StartsWith("translation", StringComparison.Ordinal) && field.Value.ValueKind == JsonValueKind.String)
                        Assert.False(Regex.IsMatch(field.Value.GetString() ?? "", @"\bDut(?:y|ies)\b", RegexOptions.IgnoreCase),
                            $"{file}#{row.Name}:{field.Name}");
            }
        }
    }

    [Theory]
    [InlineData("{\"1\":{\"original\":\"Cancel\",\"translation\":\"Abort\"}}", true, 1)]
    [InlineData("{\"1\":{\"name\":\"Grand Company\",\"translation_name\":\"Grande Compagnia\",\"description\":\"Return\",\"translation_description\":\"Back\"}}", true, 1)]
    [InlineData("{\"1\":{\"original\":\"Cancel\",\"translation\":\"\"}}", false, 0)]
    public void AuditChecksAllTextFieldsOnlyWhenComplete(string json, bool complete, int findings)
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, json);
            var result = GlossaryAudit.AuditFile(path, _catalog.Engine);
            Assert.Equal(complete, result.IsComplete);
            Assert.Equal(findings, result.Findings.Count);
        }
        finally { File.Delete(path); }
    }

    private static string FindRepoRoot()
    {
        for (DirectoryInfo? dir = new(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
            if (File.Exists(Path.Combine(dir.FullName, "data", "glossary", "Glossary.md"))) return dir.FullName;
        throw new DirectoryNotFoundException("Repository root not found.");
    }
}
