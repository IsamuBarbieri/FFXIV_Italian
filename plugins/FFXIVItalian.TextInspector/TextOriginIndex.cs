using System.Collections.Concurrent;

namespace FFXIVItalian.TextInspector;

internal sealed class TextOriginIndex
{
    private sealed record Origin(uint? RowId, string Text);
    private readonly ConcurrentDictionary<nint, Origin> origins = new();
    private readonly ConcurrentDictionary<nint, (nint Pointer, uint RowId)> nodes = new();
    public int Count => origins.Count;

    public bool HasSeen(nint pointer, uint rowId) =>
        origins.TryGetValue(pointer, out var origin) && (origin.RowId is null || origin.RowId == rowId);

    public void ClearNode(nint node) => nodes.TryRemove(node, out _);

    public void BindNode(nint node, nint pointer, uint rowId)
    {
        if (node != 0 && nodes.Count < 8192 && origins.TryGetValue(pointer, out var origin) && origin.RowId == rowId)
            nodes[node] = (pointer, rowId);
    }

    public bool TryResolveNode(nint node, nint pointer, string visibleText, string? currentOriginalText, out uint rowId)
    {
        rowId = 0;
        return nodes.TryGetValue(node, out var binding) && binding.Pointer == pointer &&
            TryResolve(pointer, visibleText, currentOriginalText, out rowId) && rowId == binding.RowId;
    }

    public void Observe(nint pointer, uint rowId, string? text)
    {
        if (pointer == 0 || string.IsNullOrEmpty(text) || (origins.Count >= 8192 && !origins.ContainsKey(pointer)))
            return;
        origins.AddOrUpdate(pointer, new Origin(rowId, text), (_, previous) =>
            previous.RowId == rowId && previous.Text == text ? previous : new Origin(null, string.Empty));
    }

    public bool TryResolve(nint pointer, string visibleText, string? currentOriginalText, out uint rowId)
    {
        rowId = 0;
        if (!origins.TryGetValue(pointer, out var origin) || origin.RowId is not uint id ||
            currentOriginalText != origin.Text)
            return false;
        var matches = visibleText == origin.Text ||
            (TranslationIndex.TryGetTruncatedPrefix(visibleText, out var prefix) &&
             origin.Text.ToUpperInvariant().StartsWith(prefix, StringComparison.Ordinal));
        if (!matches) return false;
        rowId = id;
        return true;
    }
}
