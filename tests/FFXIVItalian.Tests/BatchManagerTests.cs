using FFXIVItalian.Extractor;
using System.IO;
using System.Text.Json.Nodes;
using Xunit;

namespace FFXIVItalian.Tests;

public class BatchManagerTests
{
    [Fact]
    public void TechnicalKeysDoNotHideMissingVisibleTextOrPayloads()
    {
        using var complete = System.Text.Json.JsonDocument.Parse("""
            { "name": "MOVE_FORE", "translation_name": "", "description": "Move Forward", "translation_description": "Muoviti in avanti" }
            """);
        Assert.True(BatchManager.IsRowTranslated(complete.RootElement, "configkey"));
        Assert.False(BatchManager.IsRowTranslated(complete.RootElement, "another_sheet"));
        using var pending = System.Text.Json.JsonDocument.Parse("""
            { "name": "GAMEPAD", "translation_name": "", "description": "<hex:02080BE802FF054C656674FF0103>", "translation_description": "" }
            """);
        Assert.False(BatchManager.IsRowTranslated(pending.RootElement, "configkey"));
    }

    [Fact]
    public void OnlyVerifiedSoundColumnsAndSentinelSheetsAreExcluded()
    {
        using var sound = System.Text.Json.JsonDocument.Parse("""{ "col_3": "SE_Bt_Etc_ChocoboWhistle_only", "translation_col_3": "" }""");
        Assert.True(BatchManager.IsRowTranslated(sound.RootElement, "mount"));
        Assert.False(BatchManager.IsRowTranslated(sound.RootElement, "another_sheet"));
        using var visible = System.Text.Json.JsonDocument.Parse("""{ "col_3": "Whistle", "translation_col_3": "" }""");
        Assert.False(BatchManager.IsRowTranslated(visible.RootElement, "mount"));
        using var sentinel = System.Text.Json.JsonDocument.Parse("""{ "original": "0", "translation": "" }""");
        Assert.True(BatchManager.IsRowTranslated(sentinel.RootElement, "contentfinderconditiontransient"));
        Assert.False(BatchManager.IsRowTranslated(sentinel.RootElement, "another_sheet"));
    }

    [Fact]
    public void TechnicalInvariantsDoNotHideVisibleTextInAetheryteOrNotebookOrLogKind()
    {
        // aetheryte col_2 model resource is technical, but missing name is NOT hidden
        using var aethOk = System.Text.Json.JsonDocument.Parse("""{ "name": "aetheryte", "translation_name": "eterite", "col_2": "AetheryteIshgard" }""");
        Assert.True(BatchManager.IsRowTranslated(aethOk.RootElement, "aetheryte"));
        using var aethMissing = System.Text.Json.JsonDocument.Parse("""{ "name": "new aetheryte", "translation_name": "", "col_2": "AetheryteIshgard" }""");
        Assert.False(BatchManager.IsRowTranslated(aethMissing.RootElement, "aetheryte"));

        // mount sound effect typo E_Bt_ is excluded, but visible text is NOT hidden
        using var mountTypo = System.Text.Json.JsonDocument.Parse("""{ "name": "mount", "translation_name": "cavalcatura", "col_3": "E_Bt_Etc_GuuBooWhistle_only" }""");
        Assert.True(BatchManager.IsRowTranslated(mountTypo.RootElement, "mount"));

        // placename empty source name with valid translation is accepted, but missing translation is NOT
        using var placeOk = System.Text.Json.JsonDocument.Parse("""{ "name": "", "translation": "Limsa Lominsa" }""");
        Assert.True(BatchManager.IsRowTranslated(placeOk.RootElement, "placename"));
        using var placeMissing = System.Text.Json.JsonDocument.Parse("""{ "name": "New Island", "translation": "" }""");
        Assert.False(BatchManager.IsRowTranslated(placeMissing.RootElement, "placename"));

        // notebookdivision numeric range and sentinel are technical, but textual category requires translation
        using var nbRange = System.Text.Json.JsonDocument.Parse("""{ "original": "1-5", "translation": "" }""");
        Assert.True(BatchManager.IsRowTranslated(nbRange.RootElement, "notebookdivision"));
        using var nbSentinel = System.Text.Json.JsonDocument.Parse("""{ "original": "[翻訳不要] レベル区分終端", "translation": "" }""");
        Assert.True(BatchManager.IsRowTranslated(nbSentinel.RootElement, "notebookdivision"));
        using var nbText = System.Text.Json.JsonDocument.Parse("""{ "original": "Special Recipes", "translation": "" }""");
        Assert.False(BatchManager.IsRowTranslated(nbText.RootElement, "notebookdivision"));

        // emjaddon symbols are technical, but alphabetic text requires translation
        using var emjSymbol = System.Text.Json.JsonDocument.Parse("""{ "original": "●<hex:022003E80203>/<hex:022003E80303>", "translation": "" }""");
        Assert.True(BatchManager.IsRowTranslated(emjSymbol.RootElement, "emjaddon"));
        using var emjText = System.Text.Json.JsonDocument.Parse("""{ "original": "Riichi", "translation": "" }""");
        Assert.False(BatchManager.IsRowTranslated(emjText.RootElement, "emjaddon"));

        // logkind tags and GM/PvP are technical, but alphabetic text requires translation
        using var logTags = System.Text.Json.JsonDocument.Parse("""{ "original": "<hex:021303E91003>[GM]<hex:022903EA0203> >> <hex:022903EA0303><hex:021302EC03>", "translation": "" }""");
        Assert.True(BatchManager.IsRowTranslated(logTags.RootElement, "logkind"));
        using var logText = System.Text.Json.JsonDocument.Parse("""{ "original": "Battle Log", "translation": "" }""");
        Assert.False(BatchManager.IsRowTranslated(logText.RootElement, "logkind"));
    }

    [Fact]
    public void ManyColumnSheetsDoNotUseCustomTalkPromptRules()
    {
        using var row = System.Text.Json.JsonDocument.Parse("""
            { "name": "Visible name", "translation_name": "", "col_31": "", "col_32": "" }
            """);
        Assert.True(BatchManager.IsRowTranslated(row.RootElement, "customtalk"));
        Assert.False(BatchManager.IsRowTranslated(row.RootElement, "charamakename"));
    }

    [Fact]
    public void CalculateStats_CalculatesCorrectPercentagesAndCounts()
    {
        string tempFile = Path.GetTempFileName();
        try
        {
            var root = new JsonObject
            {
                ["1"] = new JsonObject { ["original"] = "Yes", ["translation"] = "Sì" },
                ["2"] = new JsonObject { ["original"] = "No", ["translation"] = "" },
                ["3"] = new JsonObject { ["original"] = "Cancel", ["translation"] = "Annulla" }
            };

            File.WriteAllText(tempFile, root.ToJsonString());

            var stats = BatchManager.CalculateStats(tempFile);

            Assert.Equal(3, stats.TotalRows);
            Assert.Equal(2, stats.TranslatedRows);
            Assert.Equal(1, stats.PendingRows);
            Assert.Equal(66.7, stats.Percentage);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public void ExportAndImportBatch_RoundTripPreservesData()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), "FFXIV_BatchTest_" + Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        try
        {
            string masterFile = Path.Combine(tempDir, "testsheet.json");
            var root = new JsonObject
            {
                ["1"] = new JsonObject { ["original"] = "Start", ["translation"] = "Inizia" },
                ["2"] = new JsonObject { ["original"] = "Exit", ["translation"] = "" },
                ["3"] = new JsonObject { ["original"] = "Options", ["translation"] = "" }
            };
            File.WriteAllText(masterFile, root.ToJsonString());

            // Export batch of size 2
            var (count, batchFile) = BatchManager.ExportBatch(tempDir, "testsheet", 2);

            Assert.Equal(2, count);
            Assert.True(File.Exists(batchFile));

            // Translate row 2 in batch file
            var batchObj = JsonNode.Parse(File.ReadAllText(batchFile)) as JsonObject;
            Assert.NotNull(batchObj);
            batchObj["2"]!["translation"] = "Esci";
            File.WriteAllText(batchFile, batchObj.ToJsonString());

            // Import batch
            var (updated, syntaxErrors, glossaryWarnings) = BatchManager.ImportBatch(tempDir, "testsheet", batchFile);

            Assert.Equal(1, updated);
            Assert.Equal(0, syntaxErrors);

            // Re-read master file
            var updatedMaster = JsonNode.Parse(File.ReadAllText(masterFile)) as JsonObject;
            Assert.NotNull(updatedMaster);
            Assert.Equal("Inizia", updatedMaster["1"]!["translation"]!.GetValue<string>());
            Assert.Equal("Esci", updatedMaster["2"]!["translation"]!.GetValue<string>());
            Assert.Equal("", updatedMaster["3"]!["translation"]!.GetValue<string>());
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void IsRowTranslated_CustomTalk_HandlesPromptColumnsCorrectly()
    {
        // Pure script hook row (col_31 and col_32 empty) -> translated
        using var doc1 = System.Text.Json.JsonDocument.Parse(@"{ ""name"": ""CmnDefMogLetter_00002"", ""col_31"": """", ""col_32"": """" }");
        Assert.True(BatchManager.IsRowTranslated(doc1.RootElement));

        // Row with col_31 untranslated -> pending
        using var doc2 = System.Text.Json.JsonDocument.Parse(@"{ ""col_31"": ""Small Talk"", ""translation_col_31"": """" }");
        Assert.False(BatchManager.IsRowTranslated(doc2.RootElement));

        // Row with col_31 translated -> translated
        using var doc3 = System.Text.Json.JsonDocument.Parse(@"{ ""col_31"": ""Small Talk"", ""translation_col_31"": ""Quattro chiacchiere"" }");
        Assert.True(BatchManager.IsRowTranslated(doc3.RootElement));
    }

    [Fact]
    public void IsRowTranslated_RequiresEveryVisibleField()
    {
        using var partial = System.Text.Json.JsonDocument.Parse(@"{ ""name"": ""Duty"", ""translation_name"": ""Incarico"", ""description"": ""A duty"", ""translation_description"": """" }");
        Assert.False(BatchManager.IsRowTranslated(partial.RootElement));
        using var complete = System.Text.Json.JsonDocument.Parse(@"{ ""tag"": ""SCRIPT_ID"", ""name"": ""Duty"", ""translation_name"": ""Incarico"", ""description"": ""A duty"", ""translation_description"": ""Un incarico"" }");
        Assert.True(BatchManager.IsRowTranslated(complete.RootElement));
    }
}
