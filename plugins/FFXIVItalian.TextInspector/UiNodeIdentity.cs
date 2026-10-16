namespace FFXIVItalian.TextInspector;

// Text buffers and displayed text can change during preview; these identify the selected node.
internal readonly record struct UiNodeIdentity(string AddonName, ushort AddonId, nint Address, uint NodeId, uint TextId, byte SheetType);
