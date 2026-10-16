using Dalamud.Plugin.Services;
using Lumina.Data.Files;
using Lumina.Data.Parsing.Uld;

namespace FFXIVItalian.TextInspector;

internal sealed record UldTextReference(string Path, string Sheet, uint RowId);

internal sealed class UldTextResolver(IDataManager dataManager)
{
    private readonly Dictionary<string, UldFile?> files = new(StringComparer.Ordinal);

    public UldTextReference? Find(HoveredUiText text)
    {
        // IDs inside components are not unique in the widget's node list.
        if (!text.IsAddonNode || !text.AddonName.All(c => char.IsAsciiLetterOrDigit(c) || c == '_'))
            return null;
        var path = $"ui/uld/{text.AddonName.ToLowerInvariant()}.uld";
        if (!files.TryGetValue(path, out var file))
        {
            try { file = dataManager.GetFile<UldFile>(path); }
            catch (Exception exception)
            {
                Plugin.PluginLog.Warning($"Impossibile leggere {path}: {exception.Message}");
                file = null;
            }
            files[path] = file;
        }
        if (file?.WidgetData.Nodes is not { } nodes)
            return null;
        var candidates = nodes.Where(node => node.NodeId == text.NodeId).ToArray();
        if (candidates.Length != 1 || candidates[0].Node is not NodeData.TextNode definition || definition.TextId == 0)
            return null;
        var sheet = (byte)definition.SheetType switch { 0 => "Addon", 1 => "Lobby", _ => null };
        return sheet is null ? null : new UldTextReference(path, sheet, definition.TextId);
    }
}
