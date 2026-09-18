using FFXIVItalian.Core.Diff;
using FFXIVItalian.Core.Glossary;
using FFXIVItalian.Core.Models;

Console.WriteLine("==================================================");
Console.WriteLine(" FFXIV Italian - Patch Diff & Update Tool");
Console.WriteLine("==================================================");

if (args.Length == 0)
{
    PrintUsage();
    return;
}

var command = args[0].ToLowerInvariant();

switch (command)
{
    case "diff":
        await HandleDiffAsync(args);
        break;

    case "init-glossary":
        await HandleInitGlossaryAsync(args);
        break;

    default:
        Console.WriteLine($"Comando sconosciuto: {command}");
        PrintUsage();
        break;
}

static void PrintUsage()
{
    Console.WriteLine("Utilizzo:");
    Console.WriteLine("  diff --old <old.json> --new <new.json> [--out <report.md>]");
    Console.WriteLine("  init-glossary [--out data/glossary/glossary.json]");
}

static async Task HandleDiffAsync(string[] args)
{
    string? oldPath = GetArgValue(args, "--old");
    string? newPath = GetArgValue(args, "--new");
    string outPath = GetArgValue(args, "--out") ?? "patch_diff_report.md";

    if (string.IsNullOrEmpty(oldPath) || string.IsNullOrEmpty(newPath))
    {
        Console.WriteLine("Errore: specificare sia --old che --new per il comando diff.");
        return;
    }

    Console.WriteLine($"Caricamento snapshot precedente da: {oldPath}");
    var oldSnapshot = await PatchDiffEngine.LoadSnapshotAsync(oldPath);

    Console.WriteLine($"Caricamento nuovo snapshot da: {newPath}");
    var newSnapshot = await PatchDiffEngine.LoadSnapshotAsync(newPath);

    var engine = new PatchDiffEngine();
    Console.WriteLine("Analisi comparativa del delta in corso...");
    var report = engine.Compare(oldSnapshot, newSnapshot);

    var markdown = report.ToMarkdown();
    await File.WriteAllTextAsync(outPath, markdown);

    Console.WriteLine();
    Console.WriteLine("=== Risultati dell'Analisi di Patch ===");
    Console.WriteLine($"- Versione Precedente: {report.OldVersion}");
    Console.WriteLine($"- Nuova Versione:      {report.NewVersion}");
    Console.WriteLine($"- Righe Invariate:     {report.UnchangedRowsCount:N0}");
    Console.WriteLine($"- Nuove Righe:         {report.NewRows.Count:N0}");
    Console.WriteLine($"- Righe Modificate:    {report.ModifiedRows.Count:N0}");
    Console.WriteLine($"- Righe Rimosse:       {report.RemovedRowKeys.Count:N0}");
    Console.WriteLine($"Report salvato con successo in: {outPath}");
}

static async Task HandleInitGlossaryAsync(string[] args)
{
    string outPath = GetArgValue(args, "--out") ?? "data/glossary/glossary.json";
    var dir = Path.GetDirectoryName(outPath);
    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
    {
        Directory.CreateDirectory(dir);
    }

    Console.WriteLine("Generazione del database canonico da 07_Glossary.md...");
    var engine = GlossaryLoader.CreateCanonicalEngine();
    await GlossaryLoader.SaveToJsonAsync(engine, outPath);

    Console.WriteLine($"Glossario canonico salvato con successo ({engine.Entries.Count} voci, {engine.VoiceProfiles.Count} profili vocali) in: {outPath}");
}

static string? GetArgValue(string[] args, string flag)
{
    int index = Array.IndexOf(args, flag);
    if (index >= 0 && index + 1 < args.Length)
    {
        return args[index + 1];
    }
    return null;
}
