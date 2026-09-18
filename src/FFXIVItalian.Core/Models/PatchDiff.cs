using System.Security.Cryptography;
using System.Text;

namespace FFXIVItalian.Core.Models;

public class SnapshotEntry
{
    public required string Key { get; set; }
    public required string SheetName { get; set; }
    public uint RowId { get; set; }
    public ushort SubRowId { get; set; }
    public int ColumnIndex { get; set; }
    public required string TextEn { get; set; }
    public required string Hash { get; set; }
}

public class PatchSnapshot
{
    public required string GameVersion { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public Dictionary<string, SnapshotEntry> Rows { get; set; } = new();

    public static string ComputeHash(string text)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}

public class RowDiff
{
    public required string Key { get; set; }
    public required string SheetName { get; set; }
    public uint RowId { get; set; }
    public required string OldText { get; set; }
    public required string NewText { get; set; }
    public required string OldHash { get; set; }
    public required string NewHash { get; set; }
}

public class PatchDiffReport
{
    public required string OldVersion { get; set; }
    public required string NewVersion { get; set; }
    public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;

    public int UnchangedRowsCount { get; set; }
    public List<SnapshotEntry> NewRows { get; set; } = [];
    public List<RowDiff> ModifiedRows { get; set; } = [];
    public List<string> RemovedRowKeys { get; set; } = [];

    public string ToMarkdown()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# FFXIV Patch Diff Report: {OldVersion} ➔ {NewVersion}");
        sb.AppendLine();
        sb.AppendLine($"- **Generato il**: {GeneratedAt:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"- **Righe Invariate (Preservate)**: {UnchangedRowsCount:N0}");
        sb.AppendLine($"- **Nuove Righe (`[NEW]`)**: {NewRows.Count:N0}");
        sb.AppendLine($"- **Righe Modificate (`[MODIFIED]`)**: {ModifiedRows.Count:N0}");
        sb.AppendLine($"- **Righe Rimosse (`[DEPRECATED]`)**: {RemovedRowKeys.Count:N0}");
        sb.AppendLine();

        if (NewRows.Count > 0)
        {
            sb.AppendLine("## Nuove Righe da Tradurre (Esempio prime 20)");
            sb.AppendLine("| Sheet | RowId | Testo Originale (EN) |");
            sb.AppendLine("| :--- | :--- | :--- |");
            foreach (var r in NewRows.Take(20))
            {
                var escaped = r.TextEn.Replace("\n", " ").Replace("|", "\\|");
                if (escaped.Length > 80) escaped = escaped[..77] + "...";
                sb.AppendLine($"| `{r.SheetName}` | `{r.RowId}` | {escaped} |");
            }
            if (NewRows.Count > 20)
            {
                sb.AppendLine($"*...altre {NewRows.Count - 20:N0} righe.*");
            }
            sb.AppendLine();
        }

        if (ModifiedRows.Count > 0)
        {
            sb.AppendLine("## Righe Modificate da Revisionare (Esempio prime 20)");
            sb.AppendLine("| Sheet | RowId | Testo Precedente (EN) | Nuovo Testo (EN) |");
            sb.AppendLine("| :--- | :--- | :--- | :--- |");
            foreach (var m in ModifiedRows.Take(20))
            {
                var oldEsc = m.OldText.Replace("\n", " ").Replace("|", "\\|");
                var newEsc = m.NewText.Replace("\n", " ").Replace("|", "\\|");
                if (oldEsc.Length > 50) oldEsc = oldEsc[..47] + "...";
                if (newEsc.Length > 50) newEsc = newEsc[..47] + "...";
                sb.AppendLine($"| `{m.SheetName}` | `{m.RowId}` | {oldEsc} | {newEsc} |");
            }
            if (ModifiedRows.Count > 20)
            {
                sb.AppendLine($"*...altre {ModifiedRows.Count - 20:N0} righe.*");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}

