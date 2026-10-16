using Dalamud.Hooking;
using System.Diagnostics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVItalian.TextInspector;

internal sealed unsafe class TextProvenanceTracker : IDisposable
{
    private delegate nint GetAddonTextDelegate(nint module, uint rowId);
    private delegate void SetTextDelegate(nint node, nint text);
    private sealed record Pending(nint Pointer, uint RowId, long Timestamp);
    private readonly Hook<GetAddonTextDelegate> hook;
    private readonly Hook<SetTextDelegate> setTextHook;
    private readonly ThreadLocal<Pending?> pending = new();
    private readonly TextOriginIndex origins = new();
    private int failures;
    public int Count => origins.Count;
    public int Failures => Volatile.Read(ref failures);

    public TextProvenanceTracker(IGameInteropProvider interop)
    {
        hook = interop.HookFromAddress<GetAddonTextDelegate>(
            (nint)RaptureTextModule.MemberFunctionPointers.GetAddonText, OnGetAddonText);
        try
        {
            setTextHook = interop.HookFromAddress<SetTextDelegate>(
                (nint)AtkTextNode.MemberFunctionPointers.SetText, OnSetText);
            setTextHook.Enable();
            hook.Enable();
        }
        catch
        {
            setTextHook?.Dispose();
            hook.Dispose();
            pending.Dispose();
            throw;
        }
    }

    private nint OnGetAddonText(nint module, uint rowId)
    {
        var pointer = hook.Original(module, rowId);
        try
        {
            if (!origins.HasSeen(pointer, rowId))
                origins.Observe(pointer, rowId, NativeText.Read(pointer));
            pending.Value = new Pending(pointer, rowId, Stopwatch.GetTimestamp());
        }
        catch (Exception)
        {
            Interlocked.Increment(ref failures);
        }
        return pointer;
    }

    private void OnSetText(nint node, nint text)
    {
        Pending? source = null;
        try
        {
            source = pending.Value;
            pending.Value = null;
            origins.ClearNode(node);
        }
        catch (Exception) { Interlocked.Increment(ref failures); }

        setTextHook.Original(node, text);

        try
        {
            // Only correlate an observed getter/setter sequence on the same thread.
            // A pointer lookup alone would incorrectly attribute nodes created before tracing.
            if (source is not null && source.Pointer == text &&
                Stopwatch.GetElapsedTime(source.Timestamp) <= TimeSpan.FromMilliseconds(100))
                origins.BindNode(node, text, source.RowId);
        }
        catch (Exception) { Interlocked.Increment(ref failures); }
    }

    public bool TryResolve(HoveredUiText text, out uint rowId) =>
        origins.TryResolveNode(text.NodeAddress, text.SourcePointer, text.Text, NativeText.Read(text.SourcePointer), out rowId);

    public void Dispose()
    {
        hook.Dispose();
        setTextHook.Dispose();
        pending.Dispose();
    }
}
