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
    private readonly List<GlossaryEntry> _entries = [];
    public IReadOnlyList<GlossaryEntry> Entries => _entries;

    public void AddEntry(GlossaryEntry entry) => _entries.Add(entry);

    public GlossaryComplianceResult ValidateTranslation(string originalEn, string translatedIt)
    {
        var result = new GlossaryComplianceResult();
        if (string.IsNullOrWhiteSpace(originalEn) || string.IsNullOrWhiteSpace(translatedIt)) return result;

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
