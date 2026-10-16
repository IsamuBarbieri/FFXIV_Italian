using System.Numerics;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;

namespace FFXIVItalian.TextInspector;

public sealed class Plugin : IDalamudPlugin
{
    private const string CommandName = "/itinspect";
    private static readonly TimeSpan ScanInterval = TimeSpan.FromMilliseconds(75);
    private static readonly Vector2 OverlayOffset = new(18, 18);

    [PluginService]
    internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

    [PluginService]
    internal static ICommandManager CommandManager { get; private set; } = null!;

    [PluginService]
    internal static IPluginLog PluginLog { get; private set; } = null!;

    [PluginService]
    internal static IGameInteropProvider GameInterop { get; private set; } = null!;

    [PluginService]
    internal static IDataManager DataManager { get; private set; } = null!;

    [PluginService]
    internal static IFramework Framework { get; private set; } = null!;

    private readonly UiTextInspector inspector = new();
    private readonly UldTextResolver uldResolver;
    private readonly LiveTextPreview preview;
    private readonly ClientAddonText clientText;
    private InlineTranslation? inlineEditor;
    private HoveredUiText? editTarget;
    private string draft = string.Empty;
    private string? inlineStatus;
    private bool livePreview;
    private sealed record Inspection(HoveredUiText Node, string Origin, UldTextReference? Uld, IReadOnlyList<TranslationReference> References);
    private Inspection? lastInspection;
    private UldTextReference? uldReference;
    private string? declaredOrigin;
    private bool panelHovered;
    private string? editorError;
    private readonly TextProvenanceTracker? provenance;
    private readonly string? provenanceError;
    private uint? observedAddonRow;
    private bool abbreviatedMatch;
    private bool enabled;
    private bool panelOpen = true;
    private Task<TranslationIndex>? indexTask;
    private CancellationTokenSource? indexCancellation;
    private TranslationIndex? index;
    private string? indexError;
    private DateTime lastScan = DateTime.MinValue;
    private HoveredUiText? hoveredText;
    private IReadOnlyList<TranslationReference> matches = Array.Empty<TranslationReference>();

    public Plugin()
    {
        uldResolver = new UldTextResolver(DataManager);
        preview = new LiveTextPreview(Framework);
        clientText = new ClientAddonText(Framework);
        try { provenance = new TextProvenanceTracker(GameInterop); }
        catch (Exception exception)
        {
            provenanceError = exception.Message;
            PluginLog.Warning($"Tracciamento delle righe Addon non disponibile: {exception.Message}");
        }
        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Attiva/disattiva l'ispezione del testo. Usa /itinspect reload per ricaricare il corpus.",
        });

        PluginInterface.UiBuilder.Draw += Draw;
        PluginInterface.UiBuilder.OpenMainUi += OpenPanel;
        PluginInterface.UiBuilder.OpenConfigUi += OpenPanel;
        StartIndexLoad();
    }

    public void Dispose()
    {
        preview.Dispose();
        clientText.Dispose();
        provenance?.Dispose();
        indexCancellation?.Cancel();
        indexCancellation?.Dispose();
        PluginInterface.UiBuilder.Draw -= Draw;
        PluginInterface.UiBuilder.OpenMainUi -= OpenPanel;
        PluginInterface.UiBuilder.OpenConfigUi -= OpenPanel;
        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        var argument = args.Trim();
        if (argument.Equals("reload", StringComparison.OrdinalIgnoreCase))
        {
            StartIndexLoad();
            PluginLog.Information("Ricarico l'indice delle traduzioni.");
            return;
        }

        if (argument.Equals("on", StringComparison.OrdinalIgnoreCase))
            enabled = true;
        else if (argument.Equals("off", StringComparison.OrdinalIgnoreCase))
            enabled = false;
        else
            enabled = !enabled;

        PluginLog.Information($"Ispezione del testo {(enabled ? "attivata" : "disattivata")}.");
    }

    private void StartIndexLoad()
    {
        indexCancellation?.Cancel();
        indexCancellation?.Dispose();
        indexCancellation = new CancellationTokenSource();
        var cancellationToken = indexCancellation.Token;
        index = null;
        indexError = null;
        hoveredText = null;
        matches = Array.Empty<TranslationReference>();
        observedAddonRow = null;
        lastInspection = null;
        declaredOrigin = null;
        uldReference = null;
        abbreviatedMatch = false;

        try
        {
            var translationsDirectory = FindTranslationsDirectory();
            indexTask = Task.Run(() => TranslationIndex.Load(translationsDirectory, cancellationToken), cancellationToken);
        }
        catch (Exception exception)
        {
            indexError = exception.Message;
            indexTask = null;
            PluginLog.Error($"Impossibile trovare data/translations: {exception.Message}");
        }
    }

    private static string FindTranslationsDirectory()
    {
        var directory = PluginInterface.AssemblyLocation.Directory
            ?? throw new DirectoryNotFoundException("Percorso dell'assembly del plugin non disponibile.");

        for (var current = directory; current is not null; current = current.Parent)
        {
            var candidate = Path.Combine(current.FullName, "data", "translations");
            if (Directory.Exists(candidate))
                return candidate;
        }

        throw new DirectoryNotFoundException("Il plugin deve essere avviato da una build dentro il repository FFXIV_Italian.");
    }

    private void Draw()
    {
        CompleteIndexLoad();
        DrawPanel();
        if (!enabled || panelHovered)
            return;

        var mouse = ImGui.GetIO().MousePos;
        var gameMouse = mouse - ImGui.GetMainViewport().Pos;
        if (!float.IsFinite(mouse.X) || !float.IsFinite(mouse.Y) || mouse.X < 0 || mouse.Y < 0)
        {
            hoveredText = null;
            matches = Array.Empty<TranslationReference>();
            return;
        }

        var now = DateTime.UtcNow;
        if (now - lastScan >= ScanInterval)
        {
            hoveredText = inspector.FindAt(gameMouse.X, gameMouse.Y);
            if (hoveredText is not null && editTarget is not null && hoveredText.Identity != editTarget.Identity)
                ClearInlineEditor();
            observedAddonRow = null;
            declaredOrigin = null;
            uldReference = null;
            abbreviatedMatch = false;
            if (hoveredText is not null && provenance is not null && provenance.TryResolve(hoveredText, out var rowId))
                observedAddonRow = rowId;
            matches = hoveredText is null || index is null ? Array.Empty<TranslationReference>() :
                observedAddonRow is uint observed ? index.FindAddonRow(observed) :
                index.Find(hoveredText.Text, out abbreviatedMatch);
            if (hoveredText is not null)
            {
                uldReference = uldResolver.Find(hoveredText);
                if (index is not null && observedAddonRow is null)
                {
                    var sheet = hoveredText.SheetType switch { 0 => "Addon", 1 => "Lobby", _ => null };
                    var declared = sheet is null ? Array.Empty<TranslationReference>() :
                        index.FindDeclaredRow(sheet, hoveredText.TextId, hoveredText.Text, clientText.GetOrRequest(sheet, hoveredText.TextId));
                    if (hoveredText.TextId != 0 && declared.Count > 0)
                    {
                        matches = declared;
                        declaredOrigin = $"Riferimento del nodo: {sheet}#{hoveredText.TextId} (testo compatibile)";
                    }
                    else if (uldReference is not null && (hoveredText.TextId == 0 ||
                             (hoveredText.TextId == uldReference.RowId && sheet == uldReference.Sheet)))
                    {
                        declared = index.FindDeclaredRow(uldReference.Sheet, uldReference.RowId, hoveredText.Text,
                            clientText.GetOrRequest(uldReference.Sheet, uldReference.RowId));
                        if (declared.Count > 0)
                        {
                            matches = declared;
                            declaredOrigin = $"Definizione ULD: {uldReference.Sheet}#{uldReference.RowId} (testo compatibile)";
                        }
                    }
                }
                lastInspection = new Inspection(hoveredText, OriginSummary(), uldReference, matches);
            }
            lastScan = now;
        }

        if (hoveredText is null && indexTask is null && indexError is null)
            return;

        DrawOverlay(mouse);
    }

    private void OpenPanel() => panelOpen = true;

    private string OriginSummary() => observedAddonRow is uint row ? $"Tracciato GetAddonText → SetText: Addon#{row}" :
        declaredOrigin ?? (abbreviatedMatch ? "Candidati dal prefisso; nessun riferimento strutturale verificato." :
            "Candidati testuali; nessun riferimento strutturale verificato.");

    private void DrawPanel()
    {
        panelHovered = false;
        if (!panelOpen)
            return;

        ImGui.SetNextWindowSize(new Vector2(430, 0), ImGuiCond.FirstUseEver);
        if (ImGui.Begin("FFXIV Italiano · Text Inspector", ref panelOpen, ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.Checkbox("Ispeziona il testo sotto il mouse", ref enabled);
            ImGui.TextWrapped("Attiva l'ispezione e passa il mouse su un testo del gioco: il riquadro mostra file, riga, ID e campo del corpus.");
            if (lastInspection is { } snapshot && ImGui.CollapsingHeader("Ultimo nodo ispezionato", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.TextWrapped($"{snapshot.Node.AddonName} → nodo {snapshot.Node.NodeId}");
                ImGui.TextWrapped(snapshot.Origin);
                ImGui.TextWrapped($"TextId: {snapshot.Node.TextId} · SheetType: {snapshot.Node.SheetType}");
                if (snapshot.Uld is { } definition)
                    ImGui.TextWrapped($"{definition.Path} → nodo {snapshot.Node.NodeId} → {definition.Sheet}#{definition.RowId}");
                ImGui.TextWrapped($"Testo: {Limit(snapshot.Node.Text, 220)}");
                if (ImGui.Button("Apri Addon Inspector"))
                {
                    ImGui.SetClipboardText(snapshot.Node.AddonName);
                    if (!CommandManager.ProcessCommand("/xldata ai"))
                        PluginLog.Warning("Impossibile aprire Addon Inspector tramite /xldata ai.");
                }
                ImGui.SameLine();
                if (ImGui.Button("Copia diagnosi"))
                    ImGui.SetClipboardText($"{snapshot.Node.AddonName} → nodo {snapshot.Node.NodeId} (0x{snapshot.Node.NodeAddress:X})\n" +
                        $"TextId={snapshot.Node.TextId}; SheetType={snapshot.Node.SheetType}\n{snapshot.Origin}\n" +
                        $"ULD: {snapshot.Uld}\nTesto: {snapshot.Node.Text}\n" +
                        string.Join('\n', snapshot.References.Select(x => $"{x.FilePath}#{x.RowId}:{x.Field} (riga {x.LineNumber})")));
                ImGui.TextWrapped($"Nell'Addon Inspector cerca {snapshot.Node.AddonName} e apri il nodo {snapshot.Node.NodeId}. Il pulsante copia il nome della finestra.");
                foreach (var reference in snapshot.References.Take(3))
                {
                    ImGui.TextWrapped($"{reference.FilePath}:{reference.LineNumber} · ID {reference.RowId} · {reference.Field}");
                    if (ImGui.SmallButton($"Copia riferimento##{reference.FilePath}/{reference.RowId}/{reference.Field}"))
                        ImGui.SetClipboardText($"{reference.FilePath}#{reference.RowId}:{reference.Field}");
                    ImGui.SameLine();
                    if (ImGui.SmallButton($"Modifica in VS Code##{reference.FilePath}/{reference.RowId}/{reference.Field}"))
                    {
                        try
                        {
                            TranslationEditor.Open(FindTranslationsDirectory(), reference);
                            editorError = null;
                        }
                        catch (Exception exception)
                        {
                            editorError = exception.Message;
                            PluginLog.Warning($"Apertura editor fallita: {exception.Message}");
                        }
                    }
                    if (ImGui.SmallButton($"Modifica in gioco##{reference.FilePath}/{reference.RowId}/{reference.Field}"))
                        OpenInlineEditor(snapshot.Node, reference);
                }
                if (editorError is not null)
                    ImGui.TextWrapped($"Editor: {editorError}");
                ImGui.TextWrapped("VS Code apre il campo della traduzione. Dopo il salvataggio premi Ricarica le traduzioni. Per vedere la modifica nel gioco occorre aggiornare anche la patch.");
                if (snapshot.References.Count > 3)
                    ImGui.TextWrapped($"Altri {snapshot.References.Count - 3} candidati nella diagnosi copiabile.");
            }
            DrawInlineEditor();
            ImGui.Separator();
            if (ImGui.Button("Ricarica le traduzioni"))
                StartIndexLoad();
            if (indexTask is not null)
                ImGui.TextWrapped("Caricamento delle traduzioni in corso…");
            else if (indexError is not null)
                ImGui.TextWrapped($"Errore: {indexError}");
            else if (index is not null)
            {
                ImGui.TextWrapped($"Indice pronto: {index.FileCount:N0} file, {index.EntryCount:N0} campi.");
                if (index.LoadErrors.Count > 0)
                    ImGui.TextWrapped($"Attenzione: {index.LoadErrors.Count} file non letti. Consulta il log Dalamud.");
            }
            ImGui.Separator();
            if (provenance is not null)
            {
                ImGui.TextWrapped($"Tracciamento Addon attivo: {provenance.Count:N0} puntatori osservati.");
                if (provenance.Failures > 0)
                    ImGui.TextWrapped($"Osservazioni non riuscite: {provenance.Failures:N0}.");
            }
            else
                ImGui.TextWrapped($"Tracciamento non disponibile: {provenanceError}");
            ImGui.TextWrapped("La ricerca usa prima il tracciamento runtime, poi TextId e SheetType del nodo e la definizione ULD, verificando il testo. Se mancano questi riferimenti mostra candidati testuali. Le definizioni ULD descrivono il layout iniziale; il gioco può aggiornare una scritta dinamicamente.");
            ImGui.TextWrapped("Una riga di origine può essere usata in più finestre: modificarla nel corpus cambia tutte le sue utilizzazioni.");
            ImGui.Separator();
            ImGui.TextWrapped("La ricerca confronta il testo con il corpus locale. Testi dinamici o assenti dal corpus possono non avere corrispondenze; testi duplicati possono indicare più voci.");
            ImGui.TextWrapped("Puoi riaprire questo pannello dal gestore plugin di Dalamud, tramite il pulsante di configurazione del plugin.");
        }
        panelHovered = ImGui.IsWindowHovered(ImGuiHoveredFlags.RootAndChildWindows);
        ImGui.End();
        if (!panelOpen)
        {
            preview.Stop();
            livePreview = false;
        }
    }

    private void OpenInlineEditor(HoveredUiText node, TranslationReference reference)
    {
        if (editTarget is not null && node.Identity != editTarget.Identity)
            ClearInlineEditor();
        if (inlineEditor is not null && draft != inlineEditor.SavedValue)
        {
            inlineStatus = "Hai una bozza non salvata. Salvala o annulla prima di cambiare voce.";
            return;
        }
        try
        {
            var document = new InlineTranslation(FindTranslationsDirectory(), reference);
            preview.Stop();
            inlineEditor = document;
            editTarget = node;
            draft = document.SavedValue;
            inlineStatus = null;
            livePreview = InlineTranslation.CanPreview(draft);
            if (livePreview) preview.Request(node, draft);
        }
        catch (Exception exception) { editorError = exception.Message; }
    }

    private void ClearInlineEditor()
    {
        preview.Stop();
        inlineEditor = null;
        editTarget = null;
        livePreview = false;
        draft = string.Empty;
        inlineStatus = null;
        editorError = null;
    }

    private void DrawInlineEditor()
    {
        if (inlineEditor is not { } editor || editTarget is null) return;
        ImGui.Separator();
        ImGui.TextWrapped($"Editor: {editor.Reference.FilePath}#{editor.Reference.RowId}:{editor.Field}");
        ImGui.TextWrapped($"Nodo fissato: {editTarget.AddonName} → {editTarget.NodeId}");
        ImGui.TextWrapped($"Sorgente: {Limit(editor.Source, 350)}");
        var changed = ImGui.InputTextMultiline("Traduzione", ref draft, 65536, new Vector2(490, 130));
        var validation = editor.Validate(draft);
        if (validation is not null) ImGui.TextWrapped(validation);
        if (ImGui.Checkbox("Anteprima live sul nodo selezionato", ref livePreview)) changed = true;
        if (changed)
        {
            inlineStatus = null;
            if (livePreview && validation is null) preview.Request(editTarget, draft);
            else preview.Stop();
        }
        ImGui.TextWrapped(preview.Status);
        ImGui.BeginDisabled(validation is not null);
        if (ImGui.Button("Salva nel JSON"))
        {
            try
            {
                var backup = editor.Save(draft, Path.Combine(PluginInterface.GetPluginConfigDirectory(), "translation-backups"));
                inlineStatus = backup is null ? "Nessuna modifica da salvare." : "Traduzione salvata; backup creato e indice in aggiornamento.";
                StartIndexLoad();
            }
            catch (Exception exception)
            {
                inlineStatus = $"Salvataggio non riuscito: {exception.Message}";
                PluginLog.Warning(inlineStatus);
            }
        }
        ImGui.EndDisabled();
        ImGui.SameLine();
        if (ImGui.Button("Annulla / chiudi editor"))
            ClearInlineEditor();
        if (inlineStatus is not null) ImGui.TextWrapped(inlineStatus);
        ImGui.TextWrapped("Annulla scarta la bozza e ripristina l'anteprima; le modifiche già salvate nel JSON restano. Per renderle definitive nel client occorre aggiornare la patch.");
    }

    private void CompleteIndexLoad()
    {
        if (indexTask is not { IsCompleted: true } completed)
            return;

        indexTask = null;
        if (completed.IsCompletedSuccessfully)
        {
            index = completed.Result;
            PluginLog.Information($"Indice pronto: {index.EntryCount} campi da {index.FileCount} file.");
            foreach (var error in index.LoadErrors)
                PluginLog.Warning($"Indicizzazione incompleta: {error}");
        }
        else
        {
            indexError = completed.Exception?.GetBaseException().Message ?? "Errore durante la lettura del corpus.";
            PluginLog.Error($"Caricamento dell'indice fallito: {indexError}");
        }
    }

    private void DrawOverlay(Vector2 mouse)
    {
        ImGui.SetNextWindowPos(mouse + OverlayOffset, ImGuiCond.Always);
        ImGui.SetNextWindowBgAlpha(0.94f);
        const ImGuiWindowFlags flags = ImGuiWindowFlags.NoDecoration
            | ImGuiWindowFlags.AlwaysAutoResize
            | ImGuiWindowFlags.NoInputs
            | ImGuiWindowFlags.NoNav
            | ImGuiWindowFlags.NoSavedSettings
            | ImGuiWindowFlags.NoFocusOnAppearing;

        if (ImGui.Begin("##FFXIVItalianTextInspector", flags))
        {
            if (hoveredText is not null)
            {
                ImGui.TextDisabled($"{hoveredText.AddonName} · addon {hoveredText.AddonId} · nodo {hoveredText.NodeId}");
                ImGui.TextWrapped($"Testo: {Limit(hoveredText.Text, 220)}");
                ImGui.TextWrapped(OriginSummary());
                if (uldReference is { } definition)
                    ImGui.TextWrapped($"{definition.Path} → nodo {hoveredText.NodeId} → {definition.Sheet}#{definition.RowId}");
                if (index is { LoadErrors.Count: > 0 })
                    ImGui.TextWrapped($"Indice incompleto: {index.LoadErrors.Count} file non letti; dettagli nel log Dalamud.");

                if (indexTask is not null)
                {
                    ImGui.TextDisabled("Caricamento del corpus…");
                }
                else if (indexError is not null)
                {
                    ImGui.TextWrapped(indexError);
                }
                else if (matches.Count == 0)
                {
                    ImGui.TextDisabled(observedAddonRow.HasValue ? "La riga osservata non è presente nel corpus locale." :
                        "Nessuna corrispondenza in data/translations.");
                }
                else
                {
                    ImGui.Separator();
                    ImGui.Text(observedAddonRow.HasValue || declaredOrigin is not null ? "Riga associata al riferimento UI" : $"Candidati: {matches.Count}");
                    foreach (var match in matches.Take(3))
                    {
                        ImGui.TextWrapped($"{match.FilePath}:{match.LineNumber}");
                        ImGui.TextDisabled($"ID {match.RowId} · campo {match.Field}");
                        if (match.Original.Length > 0)
                            ImGui.TextWrapped($"Originale: {Limit(match.Original, 220)}");
                        if (match.Translation.Length > 0)
                            ImGui.TextWrapped($"Italiano: {Limit(match.Translation, 220)}");
                        ImGui.Separator();
                    }

                    if (matches.Count > 3)
                        ImGui.TextDisabled($"Altre {matches.Count - 3} corrispondenze; diagnosi completa nel pannello.");
                }
            }
            else if (indexTask is not null)
            {
                ImGui.TextDisabled("Caricamento del corpus…");
            }
            else if (indexError is not null)
            {
                ImGui.TextWrapped(indexError);
            }
        }

        ImGui.End();
    }

    private static string Limit(string text, int maxLength) =>
        text.Length <= maxLength ? text : text[..maxLength] + "…";
}
