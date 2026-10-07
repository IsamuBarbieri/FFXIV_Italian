using System.Text.Json;
using System.Text.RegularExpressions;
using FFXIVItalian.Core.Glossary;

namespace FFXIVItalian.Tests;

public class GlossaryTests
{
    private readonly GlossaryCatalog _catalog = GlossaryLoader.LoadCanonical();

    [Fact]
    public void AllApprovedPlaceNamesAreAvailableAndUniqueNamesAreChecked()
    {
        string root = FindRepoRoot();
        using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "data", "translations", "world", "placename.json")));
        Assert.Equal(doc.RootElement.EnumerateObject().Count(), _catalog.PlaceNames.Count);
        foreach (var place in _catalog.PlaceNames)
        {
            var row = doc.RootElement.GetProperty(place.RowId);
            Assert.Equal(row.GetProperty("name").GetString(), place.English);
            Assert.Equal(row.GetProperty("translation").GetString(), place.Italian);
        }
        Assert.True(_catalog.Engine.ValidateTranslation("New Gridania", "Nuova Gridania").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation("New Gridania", "Gridania Nuova").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation("Dzemael Darkhold", "Fortezza Oscura di Dzemael").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation("Dzemael Darkhold", "Bastione Sotterraneo di Dzemael").IsCompliant);
    }

    [Fact]
    public void FileReferencesResolveToApprovedOrReviewSources()
    {
        Assert.NotEmpty(_catalog.Engine.Entries);
        string root = FindRepoRoot();
        int fileSources = 0;
        foreach (var entry in _catalog.Engine.Entries)
        {
            if (entry.SourceFile == "Decisione dell'utente")
            {
                Assert.Empty(entry.RowId);
                Assert.Empty(entry.SourceField);
                continue;
            }

            bool reviewSource = entry.SourceFile.StartsWith("@review/", StringComparison.Ordinal);
            string relativePath = reviewSource ? entry.SourceFile["@review/".Length..] : entry.SourceFile;
            string sourceRoot = Path.GetFullPath(Path.Combine(root, "data", reviewSource ? "da_revisionare" : "translations"));
            string sourcePath = Path.GetFullPath(Path.Combine(sourceRoot, relativePath.Replace('/', Path.DirectorySeparatorChar)));
            Assert.StartsWith(sourceRoot + Path.DirectorySeparatorChar, sourcePath, StringComparison.OrdinalIgnoreCase);
            Assert.True(File.Exists(sourcePath), entry.SourceFile);
            using var doc = JsonDocument.Parse(File.ReadAllText(sourcePath));
            var row = doc.RootElement.GetProperty(entry.RowId);
            string targetField = "translation_" + entry.SourceField;
            if (!row.TryGetProperty(targetField, out _) &&
                (entry.SourceField == "original" || entry.SourceField == "name")) targetField = "translation";
            string original = row.GetProperty(entry.SourceField).GetString() ?? "";
            string translation = row.GetProperty(targetField).GetString() ?? "";
            Assert.False(string.IsNullOrWhiteSpace(original), entry.SourceFile);
            Assert.False(string.IsNullOrWhiteSpace(translation), entry.SourceFile);
            fileSources++;
        }
        Assert.NotEqual(0, fileSources);
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
        Assert.False(_catalog.Engine.ValidateTranslation("Dungeon", "Dungeon").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation("Dungeon", "Spedizione").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation("Enter the dungeon", "Entra nel dungeon").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation("Raid the Dungeons of Lyhe Ghiah", "Compi un'incursione nei Dungeons of Lyhe Ghiah").IsCompliant);
    }

    [Fact]
    public void ApprovedTranslationFilesDoNotRetainDutyInItalian()
    {
        string root = FindRepoRoot();
        string translations = Path.Combine(root, "data", "translations");
        var files = Directory.EnumerateFiles(translations, "*.json", SearchOption.AllDirectories).ToArray();
        Assert.NotEmpty(files);
        foreach (var path in files)
        {
            string file = Path.GetRelativePath(translations, path).Replace('\\', '/');
            using var doc = JsonDocument.Parse(File.ReadAllText(path));
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
    [InlineData("{\"1\":{\"name\":\"Duty\",\"translation_name\":\"Incarico\",\"description\":\"Return\",\"translation_description\":\"\"}}", false, 0)]
    [InlineData("{\"1\":{\"original\":\"Cancel\"}}", false, 0)]
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
