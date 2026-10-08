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
        Assert.False(_catalog.Engine.ValidateTranslation(
            "No one comes and goes from this dungeon without permission.",
            "Nessuno può entrare o uscire da questa spedizione senza permesso.").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "No one comes and goes from this dungeon without permission.",
            "Nessuno può entrare o uscire da questo sotterraneo senza permesso.").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation(
            "You can enter a variant dungeon via the V&C Dungeon Finder and interact with the dungeon entrance.",
            "Puoi accedere a una spedizione variante dalla Ricerca Spedizioni V&C e interagire con l'ingresso della spedizione.").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "You can enter a variant dungeon via the V&C Dungeon Finder and interact with the dungeon entrance.",
            "Puoi accedere a una spedizione variante dalla Ricerca Spedizioni V&C e interagire con l'ingresso del sotterraneo.").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "Enter dungeons and other duties via the Duty Finder.",
            "Accedi a spedizioni e altri incarichi tramite la Ricerca Incarichi.").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation(
            "Enter dungeons and other duties via the Duty Finder.",
            "Accedi a sotterranei e altri incarichi tramite la Ricerca Incarichi.").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "These actions are available during the dungeon and can be changed in the Dungeon Finder window.",
            "Queste azioni sono disponibili durante la spedizione e modificabili dalla finestra Ricerca Spedizioni V&C.").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "Complete the criterion dungeon Another Sil'dihn Subterrane.",
            "Completa la spedizione criterio «Altri Sotterranei di Sil'dih».").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "Battle in the Dungeon orchestrion roll",
            "Battaglia nel dungeon rullo orchestrion").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "Save your progress and leave the dungeon. Speak with the expedition bishop.",
            "Salva i progressi e lascia il sotterraneo. Parla con il vescovo della spedizione.").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "“End duty.” has received the most votes. Proceed to the exit to leave the dungeon.",
            "«Termina incarico» ha ricevuto più voti. Raggiungi l'uscita per lasciare la spedizione.").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation(
            "“End duty.” has received the most votes. Proceed to the exit to leave the dungeon.",
            "«Termina incarico» ha ricevuto più voti. Raggiungi l'uscita per lasciare il sotterraneo.").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation(
            "You spend most of your time traipsing about dark dungeons.",
            "Passi la maggior parte del tempo nelle spedizioni più oscure.").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "You spend most of your time traipsing about dark dungeons.",
            "Passi la maggior parte del tempo nei sotterranei più oscuri.").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "We will keep coming up with interesting new equipment and dungeons!",
            "Continueremo a ideare equipaggiamenti e spedizioni interessanti!").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation(
            "We will keep coming up with interesting new equipment and dungeons!",
            "Continueremo a ideare equipaggiamenti e sotterranei interessanti!").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation(
            "Lovingly tap your tomestone.", "Tocca affettuosamente il tuo tavoletta.").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "Lovingly tap your tomestone.", "Tocca affettuosamente la tua tavoletta.").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation(
            "Collecting tomestones with the old crowd.", "Raccogliere tavoletta con i vecchi compagni.").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation(
            "An Allagan tomestone can be exchanged.", "Questa tavoletta allagana può essere scambiato.").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "An Allagan tomestone can be exchanged.", "Questa tavoletta allagana può essere scambiata.").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation(
            "micro tomestones are highly sought after.", "Le microtavolette sono molto ricercati.").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "Researchers constructed this variety of tomestone and abandoned these tomestones.",
            "I ricercatori costruirono questo tipo di tavoletta e poi abbandonarono il progetto.").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation(
            "Little Ladies' Day festivities", "festeggiamenti del Little Ladies’ Day").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "Little Ladies' Day festivities", "festeggiamenti della Giornata delle Piccole Dame").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation("Raid the Dungeons of Lyhe Ghiah", "Compi un'incursione nei Dungeons of Lyhe Ghiah").IsCompliant);
    }

    [Fact]
    public void ValidationIndexRefreshesWhenEntriesAreAdded()
    {
        var engine = new GlossaryEngine();
        engine.AddEntry(new GlossaryEntry { EnglishTerm = "Duty", ItalianTerm = "Incarico" });

        Assert.False(engine.ValidateTranslation("Duty", "Compito").IsCompliant);

        engine.AddEntry(new GlossaryEntry { EnglishTerm = "Duty", ItalianTerm = "Compito" });

        Assert.True(engine.ValidateTranslation("Duty", "Compito").IsCompliant);
    }

    [Fact]
    public void ExplicitResidualRulesCheckTermsInPhrasesAndPreserveKupoCompounds()
    {
        Assert.False(_catalog.Engine.ValidateTranslation(
            "An Allagan tomestone was found.", "È stata trovata una tomestone allagana.").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation(
            "Allagan tomestones", "tomestones allagane").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "An Allagan tomestone was found.", "È stata trovata una tavoletta allagana.").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation(
            "dated bronze gladius", "gladio di bronzo dated", "items/item.json").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation(
            "falchion of War", "falchione of War", "items/item.json").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "In the Arms of War orchestrion roll", "In the Arms of War rullo orchestrion", "items/item.json").IsCompliant);
        Assert.False(_catalog.Engine.ValidateTranslation(
            "Welcome, kupo!", "Benvenuti, kupo!").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "Hair Raid", "Assalto di Capelli").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "Wanderlust", "Spirito Vagabondo").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "A kupo nut is on the table.", "C'è una noce kupo sul tavolo.").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "His duties were unfinished.<hex:02100103>He pursued his goal.",
            "I suoi doveri erano incompiuti.<hex:02100103>Seguì la sua missione.").IsCompliant);
        Assert.True(_catalog.Engine.ValidateTranslation(
            "dungeon tomato", "pomodoro del dungeon", "world/bnpcname.json", checkActivityCategories: false).IsCompliant);
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
                    {
                        if (file.Equals("system/textcommandparam.json", StringComparison.OrdinalIgnoreCase) &&
                            row.Value.TryGetProperty("original", out var command) &&
                            command.GetString()?.Equals("duty", StringComparison.OrdinalIgnoreCase) == true)
                            continue; // È un identificatore letterale del comando, non testo localizzato.
                        if (file.Equals("system/textcommand.json", StringComparison.OrdinalIgnoreCase) &&
                            field.Name == "translation_col_2" &&
                            row.Value.TryGetProperty("col_2", out var source) &&
                            Regex.IsMatch(source.GetString() ?? "",
                                @"(?is)^ALIAS(?:ES)?:.*?(?:USAGE|USO):.*?/search\s+\[condition\]"))
                            continue; // La parola è un parametro letterale della sintassi /search.
                        string translated = Regex.Replace(field.Value.GetString() ?? "", "[“‘\\\"].*?[”’\\\"]", "");
                        Assert.False(Regex.IsMatch(translated, @"\bDut(?:y|ies)\b", RegexOptions.IgnoreCase),
                            $"{file}#{row.Name}:{field.Name}");
                    }
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
