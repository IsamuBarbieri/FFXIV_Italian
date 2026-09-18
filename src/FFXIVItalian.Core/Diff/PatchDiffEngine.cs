using System.Text.Json;
using FFXIVItalian.Core.Models;

namespace FFXIVItalian.Core.Diff;

public class PatchDiffEngine
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public PatchDiffReport Compare(PatchSnapshot oldSnapshot, PatchSnapshot newSnapshot)
    {
        var report = new PatchDiffReport
        {
            OldVersion = oldSnapshot.GameVersion,
            NewVersion = newSnapshot.GameVersion,
            GeneratedAt = DateTimeOffset.UtcNow
        };

        int unchanged = 0;

        foreach (var (key, newEntry) in newSnapshot.Rows)
        {
            if (!oldSnapshot.Rows.TryGetValue(key, out var oldEntry))
            {
                // Key is brand new in the new version
                report.NewRows.Add(newEntry);
            }
            else if (oldEntry.Hash == newEntry.Hash)
            {
                // Text is identical between versions
                unchanged++;
            }
            else
            {
                // Text was modified in the new version!
                report.ModifiedRows.Add(new RowDiff
                {
                    Key = key,
                    SheetName = newEntry.SheetName,
                    RowId = newEntry.RowId,
                    OldText = oldEntry.TextEn,
                    NewText = newEntry.TextEn,
                    OldHash = oldEntry.Hash,
                    NewHash = newEntry.Hash
                });
            }
        }

        foreach (var key in oldSnapshot.Rows.Keys)
        {
            if (!newSnapshot.Rows.ContainsKey(key))
            {
                report.RemovedRowKeys.Add(key);
            }
        }

        report.UnchangedRowsCount = unchanged;
        return report;
    }

    public List<TranslationRow> MergeIntoCorpus(
        List<TranslationRow> currentCorpus,
        PatchSnapshot newSnapshot,
        PatchDiffReport diffReport)
    {
        var corpusDict = currentCorpus.ToDictionary(r => r.Key, r => r);
        var updatedCorpus = new List<TranslationRow>();

        // 1. Process all entries in the new snapshot
        foreach (var (key, snapshotEntry) in newSnapshot.Rows)
        {
            if (corpusDict.TryGetValue(key, out var existingRow))
            {
                // Check if it was modified
                var modification = diffReport.ModifiedRows.FirstOrDefault(m => m.Key == key);
                if (modification != null)
                {
                    // Text changed in English! Needs review!
                    existingRow.OriginalEn = snapshotEntry.TextEn;
                    existingRow.SourceHash = snapshotEntry.Hash;
                    existingRow.Status = TranslationStatus.Draft;
                    existingRow.TranslatorNotes = $"[PATCH UPDATE {diffReport.NewVersion}] Testo originale inglese modificato da Square Enix. Testo precedente: \"{modification.OldText}\"";
                    updatedCorpus.Add(existingRow);
                }
                else
                {
                    // Unchanged! Keep existing translation intact
                    updatedCorpus.Add(existingRow);
                }
            }
            else
            {
                // Brand new row!
                updatedCorpus.Add(new TranslationRow
                {
                    Key = key,
                    SheetName = snapshotEntry.SheetName,
                    RowId = snapshotEntry.RowId,
                    SubRowId = snapshotEntry.SubRowId,
                    ColumnIndex = snapshotEntry.ColumnIndex,
                    OriginalEn = snapshotEntry.TextEn,
                    CurrentIt = string.Empty,
                    Status = TranslationStatus.Untranslated,
                    SourceHash = snapshotEntry.Hash,
                    TranslatorNotes = $"[PATCH NEW {diffReport.NewVersion}] Inserito nella patch {diffReport.NewVersion}"
                });
            }
        }

        return updatedCorpus;
    }

    public static async Task<PatchSnapshot> LoadSnapshotAsync(string filePath)
    {
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<PatchSnapshot>(json, JsonOptions)
            ?? throw new InvalidOperationException($"Impossibile deserializzare lo snapshot da {filePath}");
    }

    public static async Task SaveSnapshotAsync(PatchSnapshot snapshot, string filePath)
    {
        var json = JsonSerializer.Serialize(snapshot, JsonOptions);
        await File.WriteAllTextAsync(filePath, json);
    }
}

