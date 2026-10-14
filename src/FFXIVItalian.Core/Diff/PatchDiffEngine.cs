using System.Text.Json;
using FFXIVItalian.Core.Models;

namespace FFXIVItalian.Core.Diff;

public class PatchDiffEngine
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
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

