using System.Reflection;
using System.Text.Json;

namespace FFXIVItalian.Core.Glossary;

public sealed record GlossaryCatalog(GlossaryEngine Engine)
{
    public IReadOnlyList<PlaceNameEntry> PlaceNames { get; init; } = [];
}
public sealed record PlaceNameEntry(string RowId, string English, string Italian);

public static class GlossaryLoader
{
    public static GlossaryEngine CreateCanonicalEngine() => LoadCanonical().Engine;

    public static GlossaryCatalog LoadCanonical()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FFXIVItalian.Core.Glossary.md")
            ?? throw new InvalidOperationException("Glossary.md incorporato non trovato.");
        using var reader = new StreamReader(stream);
        var catalog = LoadFromMarkdown(reader);
        using var places = Assembly.GetExecutingAssembly().GetManifestResourceStream("FFXIVItalian.Core.PlaceNames.json")
            ?? throw new InvalidOperationException("placename.json incorporato non trovato.");
        using var doc = JsonDocument.Parse(places);
        var names = doc.RootElement.EnumerateObject()
            .Select(row => new PlaceNameEntry(row.Name,
                row.Value.GetProperty("name").GetString() ?? "",
                row.Value.GetProperty("translation").GetString() ?? ""))
            .ToArray();
        catalog.Engine.AddPlaceNames(names);
        return catalog with { PlaceNames = names };
    }

    public static GlossaryCatalog LoadFromMarkdown(TextReader reader)
    {
        var engine = new GlossaryEngine();
        string section = "";
        string category = "";
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                section = line[3..].Trim();
                continue;
            }
            if (line.StartsWith("### ", StringComparison.Ordinal))
            {
                category = line[4..].Trim();
                continue;
            }
            if (section != "Voci" || !line.StartsWith("| ", StringComparison.Ordinal) ||
                line.StartsWith("| Inglese |", StringComparison.Ordinal) || line.StartsWith("| --- |", StringComparison.Ordinal))
                continue;

            var cells = line.Trim('|').Split('|').Select(value => value.Trim().Trim('`')).ToArray();
            if (cells.Length != 4 || cells[0].Length == 0 || cells[1].Length == 0)
                throw new FormatException($"Riga glossario non valida: {line}");
            var source = cells[2].Split(['#', ':'], 3);
            bool userDecision = cells[2] == "Decisione dell'utente";
            if (!userDecision && (source.Length != 3 || source[0].Length == 0 || source[1].Length == 0 || source[2].Length == 0))
                throw new FormatException($"Fonte glossario non valida: {cells[2]}");

            engine.AddEntry(new GlossaryEntry
            {
                EnglishTerm = cells[0],
                ItalianTerm = cells[1],
                Category = category switch
                {
                    "Classi e mestieri" => GlossaryCategory.Class,
                    "Luoghi" => GlossaryCategory.Place,
                    "Termini di gioco" => GlossaryCategory.GameTerm,
                    _ => GlossaryCategory.General
                },
                RuleId = cells[2],
                Notes = cells[3],
                SourceFile = source[0],
                RowId = userDecision ? "" : source[1],
                SourceField = userDecision ? "" : source[2]
            });
        }
        if (engine.Entries.Count == 0)
            throw new FormatException("Il glossario non contiene voci.");
        return new GlossaryCatalog(engine);
    }
}
