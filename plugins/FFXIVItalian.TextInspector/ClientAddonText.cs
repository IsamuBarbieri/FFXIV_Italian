using System.Collections.Concurrent;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace FFXIVItalian.TextInspector;

// The running client can still use an older patch than the JSON being edited.
internal sealed unsafe class ClientAddonText : IDisposable
{
    private readonly IFramework framework;
    private readonly ConcurrentDictionary<uint, string> rows = new();
    private readonly ConcurrentDictionary<uint, byte> pending = new();
    private readonly ConcurrentQueue<uint> requests = new();

    public ClientAddonText(IFramework framework)
    {
        this.framework = framework;
        framework.Update += Update;
    }

    public string? GetOrRequest(string sheet, uint rowId)
    {
        if (sheet != "Addon" || rowId == 0) return null;
        if (rows.TryGetValue(rowId, out var text)) return text;
        if (pending.TryAdd(rowId, 0)) requests.Enqueue(rowId);
        return null;
    }

    private void Update(IFramework frameworkInstance)
    {
        if (requests.IsEmpty) return;
        var module = RaptureTextModule.Instance();
        if (module == null) return;
        for (var i = 0; i < 8 && requests.TryDequeue(out var rowId); i++)
        {
            try
            {
                var pointer = module->GetAddonText(rowId);
                rows[rowId] = NativeText.Read((nint)pointer.Value) ?? string.Empty;
            }
            catch (Exception exception)
            {
                rows[rowId] = string.Empty;
                Plugin.PluginLog.Warning($"Lettura Addon#{rowId} dal client fallita: {exception.Message}");
            }
            finally { pending.TryRemove(rowId, out _); }
        }
    }

    public void Dispose() => framework.Update -= Update;
}
