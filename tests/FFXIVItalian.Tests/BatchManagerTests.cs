using FFXIVItalian.Extractor;
using System.IO;
using System.Text.Json.Nodes;
using Xunit;

namespace FFXIVItalian.Tests;

public class BatchManagerTests
{
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
}

