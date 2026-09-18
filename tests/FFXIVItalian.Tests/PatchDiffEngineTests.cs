using FFXIVItalian.Core.Diff;
using FFXIVItalian.Core.Models;
using Xunit;

namespace FFXIVItalian.Tests;

public class PatchDiffEngineTests
{
    [Fact]
    public void Compare_DetectsNewModifiedAndUnchangedRowsAccurately()
    {
        var oldSnapshot = new PatchSnapshot
        {
            GameVersion = "7.0.0",
            Rows = new Dictionary<string, SnapshotEntry>
            {
                ["Addon#1#0"] = new()
                {
                    Key = "Addon#1#0",
                    SheetName = "Addon",
                    RowId = 1,
                    TextEn = "Start Game",
                    Hash = PatchSnapshot.ComputeHash("Start Game")
                },
                ["Addon#2#0"] = new()
                {
                    Key = "Addon#2#0",
                    SheetName = "Addon",
                    RowId = 2,
                    TextEn = "Quit to Desktop",
                    Hash = PatchSnapshot.ComputeHash("Quit to Desktop")
                },
                ["Quest#100#0"] = new()
                {
                    Key = "Quest#100#0",
                    SheetName = "Quest",
                    RowId = 100,
                    TextEn = "Old Quest Text",
                    Hash = PatchSnapshot.ComputeHash("Old Quest Text")
                }
            }
        };

        var newSnapshot = new PatchSnapshot
        {
            GameVersion = "7.1.0",
            Rows = new Dictionary<string, SnapshotEntry>
            {
                // Unchanged
                ["Addon#1#0"] = new()
                {
                    Key = "Addon#1#0",
                    SheetName = "Addon",
                    RowId = 1,
                    TextEn = "Start Game",
                    Hash = PatchSnapshot.ComputeHash("Start Game")
                },
                // Modified
                ["Addon#2#0"] = new()
                {
                    Key = "Addon#2#0",
                    SheetName = "Addon",
                    RowId = 2,
                    TextEn = "Exit to Desktop",
                    Hash = PatchSnapshot.ComputeHash("Exit to Desktop")
                },
                // Brand new
                ["Quest#200#0"] = new()
                {
                    Key = "Quest#200#0",
                    SheetName = "Quest",
                    RowId = 200,
                    TextEn = "A Brand New 7.1 Storyline",
                    Hash = PatchSnapshot.ComputeHash("A Brand New 7.1 Storyline")
                }
                // Quest#100#0 is removed/deprecated
            }
        };

        var engine = new PatchDiffEngine();
        var report = engine.Compare(oldSnapshot, newSnapshot);

        Assert.Equal("7.0.0", report.OldVersion);
        Assert.Equal("7.1.0", report.NewVersion);
        Assert.Equal(1, report.UnchangedRowsCount);
        Assert.Single(report.NewRows);
        Assert.Equal("Quest#200#0", report.NewRows[0].Key);
        Assert.Single(report.ModifiedRows);
        Assert.Equal("Addon#2#0", report.ModifiedRows[0].Key);
        Assert.Single(report.RemovedRowKeys);
        Assert.Equal("Quest#100#0", report.RemovedRowKeys[0]);

        // Verify Markdown report generates cleanly
        var md = report.ToMarkdown();
        Assert.Contains("7.0.0 ➔ 7.1.0", md);
        Assert.Contains("Exit to Desktop", md);
    }

    [Fact]
    public void MergeIntoCorpus_PreservesApprovedTranslationsAndFlagsModifications()
    {
        var existingCorpus = new List<TranslationRow>
        {
            new()
            {
                Key = "Addon#1#0",
                SheetName = "Addon",
                RowId = 1,
                OriginalEn = "Start Game",
                CurrentIt = "Inizia Gioco",
                Status = TranslationStatus.Gold,
                SourceHash = PatchSnapshot.ComputeHash("Start Game")
            },
            new()
            {
                Key = "Addon#2#0",
                SheetName = "Addon",
                RowId = 2,
                OriginalEn = "Quit to Desktop",
                CurrentIt = "Torna al Desktop",
                Status = TranslationStatus.Gold,
                SourceHash = PatchSnapshot.ComputeHash("Quit to Desktop")
            }
        };

        var newSnapshot = new PatchSnapshot
        {
            GameVersion = "7.1.0",
            Rows = new Dictionary<string, SnapshotEntry>
            {
                ["Addon#1#0"] = new()
                {
                    Key = "Addon#1#0",
                    SheetName = "Addon",
                    RowId = 1,
                    TextEn = "Start Game",
                    Hash = PatchSnapshot.ComputeHash("Start Game")
                },
                ["Addon#2#0"] = new()
                {
                    Key = "Addon#2#0",
                    SheetName = "Addon",
                    RowId = 2,
                    TextEn = "Exit to Desktop", // Changed!
                    Hash = PatchSnapshot.ComputeHash("Exit to Desktop")
                },
                ["Quest#200#0"] = new()
                {
                    Key = "Quest#200#0",
                    SheetName = "Quest",
                    RowId = 200,
                    TextEn = "A Brand New 7.1 Storyline",
                    Hash = PatchSnapshot.ComputeHash("A Brand New 7.1 Storyline")
                }
            }
        };

        var oldSnapshot = new PatchSnapshot
        {
            GameVersion = "7.0.0",
            Rows = new Dictionary<string, SnapshotEntry>
            {
                ["Addon#1#0"] = new()
                {
                    Key = "Addon#1#0",
                    SheetName = "Addon",
                    RowId = 1,
                    TextEn = "Start Game",
                    Hash = PatchSnapshot.ComputeHash("Start Game")
                },
                ["Addon#2#0"] = new()
                {
                    Key = "Addon#2#0",
                    SheetName = "Addon",
                    RowId = 2,
                    TextEn = "Quit to Desktop",
                    Hash = PatchSnapshot.ComputeHash("Quit to Desktop")
                }
            }
        };

        var engine = new PatchDiffEngine();
        var report = engine.Compare(oldSnapshot, newSnapshot);
        var updated = engine.MergeIntoCorpus(existingCorpus, newSnapshot, report);

        Assert.Equal(3, updated.Count);

        var row1 = updated.First(r => r.Key == "Addon#1#0");
        Assert.Equal(TranslationStatus.Gold, row1.Status);
        Assert.Equal("Inizia Gioco", row1.CurrentIt);

        var row2 = updated.First(r => r.Key == "Addon#2#0");
        Assert.Equal(TranslationStatus.Draft, row2.Status); // Demoted to Draft for review
        Assert.Contains("Testo originale inglese modificato", row2.TranslatorNotes);

        var row3 = updated.First(r => r.Key == "Quest#200#0");
        Assert.Equal(TranslationStatus.Untranslated, row3.Status);
    }
}

