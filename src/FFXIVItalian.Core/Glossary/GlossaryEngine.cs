using System.Text.RegularExpressions;

namespace FFXIVItalian.Core.Glossary;

public class GlossaryComplianceResult
{
    public bool IsCompliant => Warnings.Count == 0 && ProhibitedUsages.Count == 0;
    public List<string> Warnings { get; } = [];
    public List<string> ProhibitedUsages { get; } = [];
}

public class GlossaryEngine
{
    private static readonly Dictionary<string, string> ActivityLabels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Subquest"] = "Missione secondaria",
        ["Subquests"] = "Missioni secondarie",
        ["Levequest"] = "Mandato",
        ["Leve"] = "Mandato",
        ["Trial"] = "Prova",
        ["Raid"] = "Incursione",
        ["Dungeon"] = "Spedizione",
        ["Guildhest"] = "Operazione di Gilda",
        ["Alliance Raid"] = "Incursione di Alleanza",
        ["Savage Raid"] = "Incursione Selvaggia",
        ["Extreme Trial"] = "Prova Estrema"
    };
    private readonly List<GlossaryEntry> _entries = [];
    private readonly Dictionary<string, (string Italian, string Reference)> _placeNames = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<GlossaryEntry> Entries => _entries;

    public void AddEntry(GlossaryEntry entry) => _entries.Add(entry);

    public void AddPlaceNames(IEnumerable<PlaceNameEntry> names)
    {
        foreach (var group in names.GroupBy(name => name.English, StringComparer.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(group.Key) || group.Key.Contains('<') || ActivityLabels.ContainsKey(group.Key) ||
                _entries.Any(entry => entry.Category != GlossaryCategory.Place &&
                    entry.EnglishTerm.Equals(group.Key, StringComparison.OrdinalIgnoreCase))) continue;
            var variants = group.Select(name => name.Italian).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            if (variants.Length == 1 && !string.IsNullOrWhiteSpace(variants[0]))
                _placeNames[group.Key] = (variants[0], $"world/placename.json#{group.First().RowId}:name");
        }
    }

    public GlossaryComplianceResult ValidateTranslation(string originalEn, string translatedIt)
    {
        var result = new GlossaryComplianceResult();
        if (string.IsNullOrWhiteSpace(originalEn) || string.IsNullOrWhiteSpace(translatedIt)) return result;

        if (_placeNames.TryGetValue(originalEn.Trim(), out var place) &&
            !translatedIt.Trim().Equals(place.Italian, StringComparison.OrdinalIgnoreCase))
            result.Warnings.Add($"Luogo '{originalEn.Trim()}' → {place.Italian} (fonte: {place.Reference}).");

        if (ActivityLabels.TryGetValue(originalEn.Trim(), out var activityLabel) &&
            !translatedIt.Trim().Equals(activityLabel, StringComparison.OrdinalIgnoreCase))
            result.Warnings.Add($"Categoria di attività '{originalEn.Trim()}' → {activityLabel}.");
        if (Regex.IsMatch(originalEn, @"\bFATEs?\b", RegexOptions.IgnoreCase) &&
            translatedIt.Contains("F.A.T.E.", StringComparison.OrdinalIgnoreCase))
            result.Warnings.Add("Sigla FATE: usare FATE senza punti.");
        foreach (var term in new[] { "dungeon", "raid", "trial", "guildhest", "levequest", "subquest" })
        {
            var pattern = $@"\b{term}s?\b";
            // These are proper titles, even though they contain category words.
            var sourceText = originalEn.Replace("Trials of the Braves", "", StringComparison.OrdinalIgnoreCase)
                .Replace("Dungeons of Lyhe Ghiah", "", StringComparison.OrdinalIgnoreCase);
            var targetText = translatedIt.Replace("Trials of the Braves", "", StringComparison.OrdinalIgnoreCase)
                .Replace("Dungeons of Lyhe Ghiah", "", StringComparison.OrdinalIgnoreCase);
            if (Regex.IsMatch(sourceText, pattern, RegexOptions.IgnoreCase) &&
                Regex.IsMatch(targetText, pattern, RegexOptions.IgnoreCase) &&
                !originalEn.Trim().Equals(term, StringComparison.OrdinalIgnoreCase))
                result.Warnings.Add($"Possibile categoria ancora in inglese: '{term}' (verificare nomi propri e comandi).");
        }

        foreach (var group in _entries.GroupBy(e => e.EnglishTerm, StringComparer.OrdinalIgnoreCase))
        {
            var term = group.Key;
            var variants = group.Select(e => e.ItalianTerm).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            var expected = string.Join(" / ", variants);
            if (originalEn.Trim().Equals(term, StringComparison.OrdinalIgnoreCase))
            {
                if (!variants.Contains(translatedIt.Trim(), StringComparer.OrdinalIgnoreCase))
                    result.Warnings.Add($"'{term}' → {expected} (fonte: {group.First().RuleId}).");
                continue;
            }

            // In longer text, only flag a retained English phrase. A contextual paraphrase
            // is valid, so the absence of a literal Italian form is never treated as an error.
            if (!term.Contains(' ') || variants.Contains(term, StringComparer.OrdinalIgnoreCase)) continue;
            string pattern = $@"(?<![\p{{L}}\p{{N}}]){Regex.Escape(term)}(?![\p{{L}}\p{{N}}])";
            if (Regex.IsMatch(originalEn, pattern, RegexOptions.IgnoreCase) &&
                Regex.IsMatch(translatedIt, pattern, RegexOptions.IgnoreCase))
                result.Warnings.Add($"Termine inglese ancora presente: '{term}' → {expected} (fonte: {group.First().RuleId}).");
        }
        if (_entries.Any(e => e.EnglishTerm == "Duty" && e.ItalianTerm == "Incarico") &&
            !originalEn.Trim().Equals("Duty", StringComparison.OrdinalIgnoreCase) &&
            !originalEn.Trim().Equals("Duties", StringComparison.OrdinalIgnoreCase) &&
            Regex.IsMatch(originalEn, @"\bdut(?:y|ies)\b", RegexOptions.IgnoreCase) &&
            !originalEn.Contains("Duty calls", StringComparison.OrdinalIgnoreCase))
        {
            if (Regex.IsMatch(translatedIt, @"\bdut(?:y|ies)\b", RegexOptions.IgnoreCase))
                result.Warnings.Add("'Duty' rimane in inglese; usare Incarico/Incarichi per l'attività di gioco.");
            if (!Regex.IsMatch(originalEn, @"\b(?:quests?|missions?|dailies)\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(translatedIt, @"\bmission[ei]\b", RegexOptions.IgnoreCase))
                result.Warnings.Add("'Duty' è reso come Missione; usare Incarico/Incarichi per l'attività di gioco.");
        }
        return result;
    }
}
