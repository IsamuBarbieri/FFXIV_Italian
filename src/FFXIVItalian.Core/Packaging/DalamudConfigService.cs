using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FFXIVItalian.Core.Packaging;

public enum DalamudConfigState
{
    NotFound,
    AlreadyConfigured,
    SuccessfullyUpdated,
    UpdateFailed
}

public sealed record DalamudConfigResult(
    DalamudConfigState State,
    string? ConfigPath,
    string Message);

public static class DalamudConfigService
{
    private const string PropertyName = "IsResumeGameAfterPluginLoad";

    public static string? FindConfigPath()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string defaultPath = Path.Combine(appData, "XIVLauncher", "dalamudConfig.json");
        if (File.Exists(defaultPath))
        {
            return defaultPath;
        }

        // Check alternate known paths (e.g. Linux/SteamDeck or XIV on Mac if ported)
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string[] alternates =
        [
            Path.Combine(home, ".xlcore", "dalamudConfig.json"),
            Path.Combine(home, ".var", "app", "dev.goats.xivlauncher", "config", "dalamudConfig.json")
        ];

        foreach (var alt in alternates)
        {
            if (File.Exists(alt))
            {
                return alt;
            }
        }

        return null;
    }

    public static (bool Found, bool IsResumeEnabled, string? Path) Inspect(string? customPath = null)
    {
        string? path = customPath ?? FindConfigPath();
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            return (false, false, null);
        }

        try
        {
            using var doc = JsonDocument.Parse(File.ReadAllBytes(path));
            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty(PropertyName, out var prop))
            {
                return (true, prop.ValueKind == JsonValueKind.True, path);
            }

            return (true, false, path);
        }
        catch
        {
            return (true, false, path);
        }
    }

    public static DalamudConfigResult EnsureResumeGameAfterPluginLoad(string? customPath = null)
    {
        string? path = customPath ?? FindConfigPath();
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            return new DalamudConfigResult(
                DalamudConfigState.NotFound,
                null,
                "File dalamudConfig.json non trovato nei percorsi predefiniti di XIVLauncher.");
        }

        try
        {
            byte[] originalBytes = File.ReadAllBytes(path);
            var root = JsonNode.Parse(originalBytes) as JsonObject;
            if (root == null)
            {
                return new DalamudConfigResult(
                    DalamudConfigState.UpdateFailed,
                    path,
                    "Impossibile analizzare il file dalamudConfig.json come oggetto JSON.");
            }

            if (root.TryGetPropertyValue(PropertyName, out var val) && val?.GetValue<bool>() == true)
            {
                return new DalamudConfigResult(
                    DalamudConfigState.AlreadyConfigured,
                    path,
                    $"{PropertyName} è già attivo (true). Dalamud attende correttamente il caricamento dei plugin prima di avviare il gioco.");
            }

            // Create backup if not present
            string backupPath = path + ".bak";
            if (!File.Exists(backupPath))
            {
                File.WriteAllBytes(backupPath, originalBytes);
            }

            // Update property
            root[PropertyName] = true;

            var options = new JsonSerializerOptions { WriteIndented = true };
            string updatedJson = root.ToJsonString(options);

            // Atomic write
            string tempPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            File.WriteAllText(tempPath, updatedJson);
            File.Move(tempPath, path, overwrite: true);

            return new DalamudConfigResult(
                DalamudConfigState.SuccessfullyUpdated,
                path,
                $"Configurato {PropertyName} = true con successo in '{path}'. FFXIV ora attenderà Penumbra prima di caricare le tabelle EXD!");
        }
        catch (Exception ex)
        {
            return new DalamudConfigResult(
                DalamudConfigState.UpdateFailed,
                path,
                $"Errore durante l'aggiornamento di dalamudConfig.json: {ex.Message}");
        }
    }
}

