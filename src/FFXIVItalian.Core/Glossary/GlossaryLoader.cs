using System.Reflection;

namespace FFXIVItalian.Core.Glossary;

public sealed record GlossaryCatalog(GlossaryEngine Engine, IReadOnlySet<string> ApprovedFiles);

public static class GlossaryLoader
{
    public static GlossaryEngine CreateCanonicalEngine() => LoadCanonical().Engine;

    public static GlossaryCatalog LoadCanonical()
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FFXIVItalian.Core.Glossary.md")
            ?? throw new InvalidOperationException("Glossary.md incorporato non trovato.");
        using var reader = new StreamReader(stream);
        return LoadFromMarkdown(reader);
    }

    public static GlossaryCatalog LoadFromMarkdown(TextReader reader)
    {
        var engine = new GlossaryEngine();
        var approved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
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
            if (section == "File approvati" && line.StartsWith("- `", StringComparison.Ordinal))
            {
                var path = line[3..].Trim('`', ' ');
                if (!approved.Add(path)) throw new FormatException($"File approvato duplicato: {path}");
                continue;
            }
            if (section != "Voci" || !line.StartsWith("| ", StringComparison.Ordinal) ||
                line.StartsWith("| Inglese |", StringComparison.Ordinal) || line.StartsWith("| --- |", StringComparison.Ordinal))
                continue;

            var cells = line.Trim('|').Split('|').Select(value => value.Trim().Trim('`')).ToArray();
            if (cells.Length != 4 || cells[0].Length == 0 || cells[1].Length == 0)
                throw new FormatException($"Riga glossario non valida: {line}");
            var source = cells[2].Split(['#', ':'], 3);
            if (source.Length != 3 || !approved.Contains(source[0]) || source[1].Length == 0 || source[2].Length == 0)
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
                RowId = source[1],
                SourceField = source[2]
            });
        }
        if (approved.Count == 0 || engine.Entries.Count == 0)
            throw new FormatException("Il glossario non contiene file approvati o voci.");
        return new GlossaryCatalog(engine, approved);
    }
}
