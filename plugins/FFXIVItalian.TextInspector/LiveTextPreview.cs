using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;

namespace FFXIVItalian.TextInspector;

internal sealed unsafe class LiveTextPreview : IDisposable
{
    private readonly IFramework framework;
    private readonly object gate = new();
    private (HoveredUiText Target, string Value)? desired;
    private HoveredUiText? active;
    private byte[]? originalRendered;
    private byte[]? originalFull;
    private byte[]? applied;
    private string? appliedValue;
    private long lastTick;
    private string status = "Anteprima disattivata.";
    public string Status => Volatile.Read(ref status);

    public LiveTextPreview(IFramework framework)
    {
        this.framework = framework;
        framework.Update += Update;
    }

    public void Request(HoveredUiText target, string value)
    {
        lock (gate) desired = (target, value);
    }

    public void Stop()
    {
        lock (gate)
        {
            desired = null;
            status = "Anteprima disattivata.";
        }
    }

    private void Update(IFramework _)
    {
        if (Environment.TickCount64 - lastTick < 100) return;
        lastTick = Environment.TickCount64;
        lock (gate)
        {
            try { Apply(); }
            catch (Exception exception)
            {
                desired = null;
                status = $"Anteprima sospesa: {exception.Message}";
                Plugin.PluginLog.Warning(status);
            }
        }
    }

    private void Apply()
    {
        if (desired is not { } request)
        {
            if (active is not null) Restore();
            return;
        }
        if (active is not null && active != request.Target)
            Restore();
        if (!InlineTranslation.CanPreview(request.Value))
        {
            Restore();
            status = "Anteprima disponibile solo per testo semplice; tag e macro si possono salvare preservandoli.";
            return;
        }
        var node = UiTextInspector.FindLiveNode(request.Target);
        if (node == null)
        {
            desired = null;
            active = null;
            status = "Nodo non più disponibile. Riapri la finestra e selezionalo di nuovo.";
            return;
        }
        var bytes = node->NodeText.AsSpan().ToArray();
        if (active is null)
        {
            if (bytes.Contains((byte)2))
            {
                status = "Questo nodo contiene payload: anteprima non disponibile; il salvataggio resta disponibile.";
                return;
            }
            var actualText = TranslationIndex.CleanForDisplay(node->NodeText.ToString());
            if (actualText != request.Target.Text && !(TranslationIndex.TryGetTruncatedPrefix(actualText, out var prefix) &&
                request.Target.Text.ToUpperInvariant().StartsWith(prefix, StringComparison.Ordinal)))
            {
                desired = null;
                status = "Il testo del nodo è cambiato: selezionalo di nuovo.";
                return;
            }
            active = request.Target;
            originalRendered = bytes;
            originalFull = NativeText.ReadBytes(request.Target.SourcePointer) ?? bytes;
            // Fall back to the owned node text if the caller's buffer has changed.
            if (NativeText.Read(request.Target.SourcePointer) != request.Target.Text)
                originalFull = bytes;
            if (originalFull.Contains((byte)2)) originalFull = bytes;
        }
        else if (!bytes.AsSpan().SequenceEqual(applied) && !bytes.AsSpan().SequenceEqual(originalRendered))
        {
            desired = null;
            active = null;
            status = "Il gioco ha assegnato un altro testo al nodo: anteprima sospesa.";
            return;
        }
        if (appliedValue != request.Value || !bytes.AsSpan().SequenceEqual(applied))
        {
            // This overload copies the string into NodeText and uses its owned buffer.
            node->SetText(request.Value);
            applied = node->NodeText.AsSpan().ToArray();
            appliedValue = request.Value;
        }
        status = "Anteprima live attiva sul nodo selezionato. La patch del gioco non viene modificata.";
    }

    private void Restore()
    {
        if (active is not null && applied is not null)
        {
            var node = UiTextInspector.FindLiveNode(active);
            if (node != null && node->NodeText.AsSpan().SequenceEqual(applied) && originalFull is not null)
                node->SetText(originalFull.AsSpan());
        }
        active = null;
        applied = null;
        appliedValue = null;
        originalRendered = null;
        originalFull = null;
        status = "Anteprima disattivata; ripristinato il testo precedente se il nodo è ancora disponibile.";
    }

    public void Dispose()
    {
        framework.Update -= Update;
        if (framework.IsFrameworkUnloading) return;
        _ = framework.RunOnFrameworkThread(() =>
        {
            lock (gate) { desired = null; Restore(); }
        }).ContinueWith(task =>
        {
            if (task.IsFaulted) Plugin.PluginLog.Warning($"Ripristino anteprima fallito: {task.Exception?.GetBaseException().Message}");
        });
    }
}
