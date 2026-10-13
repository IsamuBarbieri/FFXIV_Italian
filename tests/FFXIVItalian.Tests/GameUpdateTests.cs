using System.Text.Json.Nodes;
using FFXIVItalian.Extractor;

namespace FFXIVItalian.Tests;

public class GameUpdateTests
{
    [Fact]
    public void NestedExtractionDoesNotOverwriteTheFlatHomonym()
    {
        string dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            string translations = Path.Combine(dir, "translations");
            Directory.CreateDirectory(Path.Combine(translations, "world"));
            string flat = Path.Combine(translations, "world", "aetheryte.json");
            File.WriteAllText(flat, "{}");
            string expected = Path.Combine(dir, "da_tradurre", "world", "transport", "aetheryte.json");
            Assert.Equal(expected, FFXIVItalian.Core.Translation.TranslationPathResolver.ResolveTargetPath(translations, "transport/aetheryte"));
            Directory.CreateDirectory(Path.GetDirectoryName(expected)!);
            File.WriteAllText(expected, "{}");
            Assert.Equal(expected, FFXIVItalian.Core.Translation.TranslationPathResolver.FindFile(translations, "transport/aetheryte"));
            Assert.Equal(flat, FFXIVItalian.Core.Translation.TranslationPathResolver.FindFile(translations, "aetheryte"));
        }
        finally { if (Directory.Exists(dir)) Directory.Delete(dir, true); }
    }
    [Theory]
    [InlineData("world/aetheryte.json", "Aetheryte", "transport/Aetheryte", "Aetheryte")]
    [InlineData("world/transport/aetheryte.json", "Aetheryte", "transport/Aetheryte", "transport/Aetheryte")]
    [InlineData("quests/leve/guildleveassignment.json", "GuildleveAssignment", "leve/GuildleveAssignment", "leve/GuildleveAssignment")]
    [InlineData("world/transport/chocobotaxistand.json", "ChocoboTaxiStand", "transport/ChocoboTaxiStand", "transport/ChocoboTaxiStand")]
    public void PatcherResolvesHomonymsUsingTheApprovedFilePath(string path, string first, string second, string expected)
    {
        Assert.Equal(expected, FFXIVItalian.Core.Translation.TranslationPathResolver.ResolveSheetForFile(
            Path.Combine("data", "translations", path.Replace('/', Path.DirectorySeparatorChar)), [first, second]));
        Assert.Throws<InvalidDataException>(() => FFXIVItalian.Core.Translation.TranslationPathResolver.ResolveSheetForFile(
            Path.Combine("data", "translations", "wrong", "aetheryte.json"), ["Aetheryte", "transport/Aetheryte"]));
    }
    [Fact]
    public void EquivalentHexSegmentationIsNotASourceChange()
    {
        var before = JsonNode.Parse("""{"1":{"original":"<hex:02100103>.","translation":"conserva"}}""")!.AsObject();
        var fresh = JsonNode.Parse("""{"1":{"original":"<hex:0210><hex:01032E>","translation":""}}""")!.AsObject();
        var changes = new List<UpdateChange>();
        var merged = GameUpdate.Merge("test.json", before, fresh, changes);
        Assert.Empty(changes);
        Assert.True(JsonNode.DeepEquals(before, merged));
        fresh["1"]!["original"] = "<hex:02100103>!";
        GameUpdate.Merge("test.json", before, fresh, changes);
        Assert.Single(changes);
    }
    [Fact]
    public void HomonymousTechnicalAndDialogueSheetsDoNotShareTheSameCorpusFile()
    {
        string file = Path.Combine("data", "translations", "activities", "guild_order", "guildorderguide.json");
        Assert.Null(GameUpdate.MatchExistingFile("GuildOrderGuide", [file], true));
        Assert.Equal(file, GameUpdate.MatchExistingFile("guild_order/GuildOrderGuide", [file], true));
        Assert.Throws<InvalidDataException>(() => GameUpdate.MatchExistingFile("guild_order/GuildOrderGuide", [file, file.Replace("translations", "da_tradurre")], true));
        string taxi = Path.Combine("data", "translations", "world", "chocobotaxistand.json");
        Assert.Equal(taxi, GameUpdate.MatchExistingFile("ChocoboTaxiStand", [taxi], true));
        Assert.Null(GameUpdate.MatchExistingFile("transport/ChocoboTaxiStand", [taxi], true));
        string leve = Path.Combine("data", "da_tradurre", "quests", "master", "guildleveassignment.json");
        Assert.Equal(leve, GameUpdate.MatchExistingFile("GuildleveAssignment", [leve], true));
        Assert.Null(GameUpdate.MatchExistingFile("leve/GuildleveAssignment", [leve], true));
    }
    [Fact]
    public void MergePreservesTranslationsMetadataAndRemovedRowsAndReportsChanges()
    {
        var before = JsonNode.Parse("""{"1":{"original":"old","translation":"italiano","note":"keep"},"2":{"original":"same","translation":"uguale"},"3":{"original":"gone","translation":"conserva"}}""")!.AsObject();
        var fresh = JsonNode.Parse("""{"1":{"original":"new","translation":""},"2":{"original":"same","translation":"uguale"},"4":{"original":"added","translation":""}}""")!.AsObject();
        var changes = new List<UpdateChange>();
        var merged = GameUpdate.Merge("file.json", before, fresh, changes);
        Assert.Equal("italiano", merged["1"]!["translation"]!.GetValue<string>());
        Assert.Equal("keep", merged["1"]!["note"]!.GetValue<string>());
        Assert.Equal("new", merged["1"]!["original"]!.GetValue<string>());
        Assert.True(JsonNode.DeepEquals(before["3"], merged["3"]));
        Assert.Equal("", merged["4"]!["translation"]!.GetValue<string>());
        Assert.Equal(new[] { "modified", "added", "removed" }, changes.Select(c => c.Kind));
        Assert.Equal("old", before["1"]!["original"]!.GetValue<string>());
        Assert.Equal(new[] { "original" }, changes[0].Fields);
    }

    [Fact]
    public void MergeRejectsAnUnmappedSourceSchema()
    {
        var before = JsonNode.Parse("""{"1":{"original":"old","translation":"conserva"}}""")!.AsObject();
        var fresh = JsonNode.Parse("""{"1":{"name":"new","translation_name":""}}""")!.AsObject();
        Assert.Throws<InvalidDataException>(() => GameUpdate.Merge("file.json", before, fresh, []));
    }

    [Fact]
    public void IntegrationAddsRowsWithoutChangingOldSourceTranslationsOrEditorialState()
    {
        string dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            string relative = Path.Combine("translations", "system", "addon.json");
            string run = Path.Combine(dir, "updates", "run");
            string old = "{\r\n  \"1\": { \"original\": \"old\", \"translation\": \"traduzione\", \"note\": \"keep\" }\r\n}\r\n";
            foreach (string folder in new[] { "translations/system", "updates/run/backup/translations/system" })
                Directory.CreateDirectory(Path.Combine(dir, folder));
            File.WriteAllText(Path.Combine(dir, relative), old);
            File.WriteAllText(Path.Combine(run, "backup", relative), old);
            var before = JsonNode.Parse(old)!.AsObject();
            var fresh = JsonNode.Parse("""{"1":{"original":"changed","translation":""},"2":{"original":"new","translation":""}}""")!.AsObject();
            var changes = new List<UpdateChange>();
            GameUpdate.Merge(relative, before, fresh, changes);
            Assert.Equal(1, GameUpdate.IntegrateAdditions(dir, run, [], changes));
            string result = File.ReadAllText(Path.Combine(dir, relative));
            Assert.Contains("  \"1\": { \"original\": \"old\", \"translation\": \"traduzione\", \"note\": \"keep\" }", result);
            var rows = JsonNode.Parse(result)!.AsObject();
            Assert.Equal("old", rows["1"]!["original"]!.GetValue<string>());
            Assert.Equal("", rows["2"]!["translation"]!.GetValue<string>());
            Assert.Throws<IOException>(() => GameUpdate.IntegrateAdditions(dir, run, [], changes));
            Assert.Equal(result, File.ReadAllText(Path.Combine(dir, relative)));
        }
        finally { if (Directory.Exists(dir)) Directory.Delete(dir, true); }
    }

    [Fact]
    public void IntegrationPreflightsAllTargetsBeforeWritingAnyFile()
    {
        string dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            string run = Path.Combine(dir, "updates", "run");
            string first = Path.Combine("da_tradurre", "world", "new.json");
            string second = Path.Combine("da_tradurre", "world", "conflict.json");
            Directory.CreateDirectory(Path.Combine(run, "updated", "da_tradurre", "world"));
            Directory.CreateDirectory(Path.Combine(dir, "da_tradurre", "world"));
            File.WriteAllText(Path.Combine(run, "updated", first), "{}");
            File.WriteAllText(Path.Combine(dir, second), "{}");
            Assert.Throws<IOException>(() => GameUpdate.IntegrateAdditions(dir, run, [first, second], []));
            Assert.False(File.Exists(Path.Combine(dir, first)));
        }
        finally { if (Directory.Exists(dir)) Directory.Delete(dir, true); }
    }

    [Fact]
    public void IntegrationHandlesEmptyBomFileAndImportsANewSheetIntoPendingState()
    {
        string dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            string run = Path.Combine(dir, "updates", "run");
            string existing = Path.Combine("da_revisionare", "system", "empty.json");
            string fresh = Path.Combine("da_tradurre", "world", "warp", "new.json");
            foreach (string folder in new[] { Path.GetDirectoryName(existing)!, Path.Combine("updates", "run", "backup", Path.GetDirectoryName(existing)!), Path.Combine("updates", "run", "updated", Path.GetDirectoryName(fresh)!) })
                Directory.CreateDirectory(Path.Combine(dir, folder));
            var encoding = new System.Text.UTF8Encoding(true);
            File.WriteAllText(Path.Combine(dir, existing), "{}\r\n", encoding);
            File.Copy(Path.Combine(dir, existing), Path.Combine(run, "backup", existing));
            string sheet = """{"1":{"original":"new","translation":""}}""";
            File.WriteAllText(Path.Combine(run, "updated", fresh), sheet);
            var row = JsonNode.Parse(sheet)!["1"]!;
            Assert.Equal(1, GameUpdate.IntegrateAdditions(dir, run, [fresh], [new(existing, "1", "added", null, row)]));
            Assert.Equal(sheet, File.ReadAllText(Path.Combine(dir, fresh)));
            Assert.True(File.ReadAllBytes(Path.Combine(dir, existing)).AsSpan().StartsWith(encoding.GetPreamble()));
            Assert.Equal("", JsonNode.Parse(File.ReadAllText(Path.Combine(dir, existing)))!["1"]!["translation"]!.GetValue<string>());
        }
        finally { if (Directory.Exists(dir)) Directory.Delete(dir, true); }
    }

    [Theory]
    [InlineData("custom/000/Menu", "dialogue/custom/000")]
    [InlineData("warp/WarpInn", "world/warp")]
    [InlineData("cut_scene/022/Scene", "dialogue/cut_scene/022")]
    public void NestedSheetsKeepTheirFamilyAndFolder(string sheet, string expected)
    {
        Assert.Equal(expected.Replace('/', Path.DirectorySeparatorChar),
            FFXIVItalian.Core.Translation.TranslationPathResolver.GetTargetSubdirectory(sheet));
        var extractor = new FFXIVItalian.Extractor.Extractors.UniversalSheetExtractor(sheet);
        Assert.Equal(Path.GetFileName(sheet).ToLowerInvariant() + ".json", extractor.DefaultJsonFileName);
    }
}
