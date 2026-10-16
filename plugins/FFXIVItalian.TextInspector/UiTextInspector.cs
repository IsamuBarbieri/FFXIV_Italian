using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVItalian.TextInspector;

internal sealed record HoveredUiText(
    string Text,
    string AddonName,
    nint NodeAddress,
    nint SourcePointer,
    ushort AddonId,
    uint NodeId,
    uint TextId,
    byte SheetType,
    bool IsAddonNode,
    uint DepthLayer,
    ushort DrawOrderIndex,
    long Area)
{
    public UiNodeIdentity Identity => new(AddonName, AddonId, NodeAddress, NodeId, TextId, SheetType);
}

internal sealed unsafe class UiTextInspector
{
    private const int MaxNodesPerScan = 30000;
    private readonly Stack<nint> stack = new();
    private readonly HashSet<nint> visited = new();

    internal static AtkTextNode* FindLiveNode(HoveredUiText target)
    {
        var manager = (AtkUnitManager*)RaptureAtkUnitManager.Instance();
        if (manager == null) return null;
        var units = &manager->AllLoadedUnitsList;
        for (var i = 0; i < units->Count; i++)
        {
            var addon = units->Entries[i].Value;
            if (addon == null || !addon->IsReady || addon->Id != target.AddonId || addon->NameString != target.AddonName)
                continue;
            var stack = new Stack<nint>();
            var visited = new HashSet<nint>();
            if (addon->RootNode != null) stack.Push((nint)addon->RootNode);
            while (stack.TryPop(out var address) && visited.Count < MaxNodesPerScan)
            {
                if (!visited.Add(address)) continue;
                var node = (AtkResNode*)address;
                if (address == target.NodeAddress && node->Type == NodeType.Text && node->NodeId == target.NodeId)
                {
                    var text = (AtkTextNode*)node;
                    return text->TextId == target.TextId && text->SheetType == target.SheetType ? text : null;
                }
                if (node->ChildNode != null) stack.Push((nint)node->ChildNode);
                if (node->NextSiblingNode != null) stack.Push((nint)node->NextSiblingNode);
                if (node->PrevSiblingNode != null) stack.Push((nint)node->PrevSiblingNode);
                if ((int)node->Type >= 1000)
                {
                    var component = ((AtkComponentNode*)node)->Component;
                    if (component != null && component->UldManager.RootNode != null)
                        stack.Push((nint)component->UldManager.RootNode);
                }
            }
            break;
        }
        return null;
    }

    public HoveredUiText? FindAt(float mouseX, float mouseY)
    {
        var manager = (AtkUnitManager*)RaptureAtkUnitManager.Instance();
        if (manager == null)
            return null;

        var loadedUnits = &manager->AllLoadedUnitsList;
        var best = (HoveredUiText?)null;
        var scanned = 0;
        var mousePointX = (int)mouseX;
        var mousePointY = (int)mouseY;

        for (var addonIndex = 0; addonIndex < loadedUnits->Count; addonIndex++)
        {
            var addon = loadedUnits->Entries[addonIndex].Value;
            if (addon == null || !addon->IsReady || !addon->IsVisible || addon->RootNode == null)
                continue;

            stack.Clear();
            visited.Clear();
            stack.Push((nint)addon->RootNode);

            while (stack.TryPop(out var address))
            {
                if (!visited.Add(address))
                    continue;
                if (++scanned > MaxNodesPerScan)
                    return best;

                var node = (AtkResNode*)address;
                if (node->NextSiblingNode != null)
                    stack.Push((nint)node->NextSiblingNode);
                if (node->PrevSiblingNode != null)
                    stack.Push((nint)node->PrevSiblingNode);

                var visible = !node->IsDrawDisabled && node->NodeFlags.HasFlag(NodeFlags.Visible);
                if (!visible)
                    continue;

                if (node->ChildNode != null)
                    stack.Push((nint)node->ChildNode);

                if ((int)node->Type >= 1000)
                {
                    var component = ((AtkComponentNode*)node)->Component;
                    if (component != null && component->UldManager.RootNode != null)
                        stack.Push((nint)component->UldManager.RootNode);
                }

                if (node->Type != NodeType.Text || !IsVisibleThroughParents(node, addon->RootNode))
                    continue;

                var bounds = default(Bounds);
                node->GetBounds(&bounds);
                if (bounds.Width <= 0 || bounds.Height <= 0 || !bounds.ContainsPoint(mousePointX, mousePointY))
                    continue;

                var textNode = (AtkTextNode*)node;
                var text = TranslationIndex.CleanForDisplay(ReadText(textNode));
                if (text.Length == 0)
                    continue;

                if (TranslationIndex.TryGetTruncatedPrefix(text, out var prefix))
                {
                    var fullText = NativeText.Read((nint)textNode->OriginalTextPointer.Value);
                    if (fullText is not null && fullText.ToUpperInvariant().StartsWith(prefix, StringComparison.Ordinal))
                        text = fullText;
                }

                var candidate = new HoveredUiText(
                    text,
                    addon->NameString,
                    (nint)node,
                    (nint)textNode->OriginalTextPointer.Value,
                    addon->Id,
                    node->NodeId,
                    textNode->TextId,
                    textNode->SheetType,
                    IsAddonNode(node, addon),
                    addon->DepthLayer,
                    addon->DrawOrderIndex,
                    (long)bounds.Width * bounds.Height);

                if (IsBetter(candidate, best))
                    best = candidate;
            }
        }

        return best;
    }

    private static bool IsAddonNode(AtkResNode* node, AtkUnitBase* addon)
    {
        var manager = &addon->UldManager;
        for (var i = 0; manager->NodeList != null && i < manager->NodeListCount; i++)
            if (manager->NodeList[i] == node)
                return true;
        return false;
    }

    private static string ReadText(AtkTextNode* textNode)
    {
        var bytes = textNode->NodeText.AsSpan();
        if (bytes.IsEmpty)
            return string.Empty;

        try
        {
            fixed (byte* pointer = bytes)
                return SeString.Parse(pointer, bytes.Length).TextValue;
        }
        catch (Exception)
        {
            return textNode->NodeText.ToString();
        }
    }

    private static bool IsVisibleThroughParents(AtkResNode* node, AtkResNode* root)
    {
        for (var depth = 0; node != null && depth < 128; depth++, node = node->ParentNode)
        {
            if (node->IsDrawDisabled || !node->NodeFlags.HasFlag(NodeFlags.Visible))
                return false;
            if (node == root)
                return true;
        }

        // Component roots can terminate their parent chain before the addon root.
        return node == null;
    }

    private static bool IsBetter(HoveredUiText candidate, HoveredUiText? current)
    {
        if (current is null)
            return true;

        if (candidate.DepthLayer != current.DepthLayer)
            return candidate.DepthLayer > current.DepthLayer;
        if (candidate.DrawOrderIndex != current.DrawOrderIndex)
            return candidate.DrawOrderIndex > current.DrawOrderIndex;
        return candidate.Area < current.Area;
    }
}
