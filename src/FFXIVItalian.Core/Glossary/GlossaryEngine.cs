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
    private sealed record TermGroup(string Term, string[] Variants, string RuleId, Regex? Pattern, bool EquipmentOnly);

    private sealed record ValidationIndex(
        Dictionary<string, TermGroup> ExactTerms,
        TermGroup[] ResidualTerms,
        HashSet<string> ApprovedItalianLoanwords,
        bool HasDutyIncarico);

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
    private ValidationIndex? _validationIndex;
    public IReadOnlyList<GlossaryEntry> Entries => _entries;

    public void AddEntry(GlossaryEntry entry)
    {
        _entries.Add(entry);
        _validationIndex = null;
    }

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

    public GlossaryComplianceResult ValidateTranslation(string originalEn, string translatedIt, string? sourceContext = null)
    {
        var result = new GlossaryComplianceResult();
        if (string.IsNullOrWhiteSpace(originalEn) || string.IsNullOrWhiteSpace(translatedIt)) return result;

        var index = GetValidationIndex();
        string trimmedOriginal = originalEn.Trim();
        var normalizedContext = sourceContext?.Replace('\\', '/');
        bool isPlaceNameSheet = string.IsNullOrWhiteSpace(sourceContext) ||
            (normalizedContext is not null &&
             (normalizedContext.Equals("placename.json", StringComparison.OrdinalIgnoreCase) ||
              normalizedContext.Equals("world/placename.json", StringComparison.OrdinalIgnoreCase) ||
              normalizedContext.EndsWith("/world/placename.json", StringComparison.OrdinalIgnoreCase)));
        // The full place catalog contains common words that are valid action/status names too.
        if (isPlaceNameSheet && _placeNames.TryGetValue(trimmedOriginal, out var place) &&
            !translatedIt.Trim().Equals(place.Italian, StringComparison.OrdinalIgnoreCase) &&
            (!index.ExactTerms.TryGetValue(trimmedOriginal, out var placeTerm) ||
             !placeTerm.Variants.Contains(translatedIt.Trim(), StringComparer.OrdinalIgnoreCase)))
            result.Warnings.Add($"Luogo '{trimmedOriginal}' → {place.Italian} (fonte: {place.Reference}).");

        if (ActivityLabels.TryGetValue(trimmedOriginal, out var activityLabel) &&
            !translatedIt.Trim().Equals(activityLabel, StringComparison.OrdinalIgnoreCase))
            result.Warnings.Add($"Categoria di attività '{originalEn.Trim()}' → {activityLabel}.");
        if (Regex.IsMatch(originalEn, @"\bFATEs?\b", RegexOptions.IgnoreCase) &&
            translatedIt.Contains("F.A.T.E.", StringComparison.OrdinalIgnoreCase))
            result.Warnings.Add("Sigla FATE: usare FATE senza punti.");
        foreach (var term in new[] { "dungeon", "raid", "trial", "guildhest", "levequest", "subquest" })
        {
            if (index.ApprovedItalianLoanwords.Contains(term)) continue;
            var pattern = $@"\b{term}s?\b";
            // These are proper titles, even though they contain category words.
            var sourceText = originalEn.Replace("Trials of the Braves", "", StringComparison.OrdinalIgnoreCase)
                .Replace("Dungeons of Lyhe Ghiah", "", StringComparison.OrdinalIgnoreCase);
            var targetText = translatedIt.Replace("Trials of the Braves", "", StringComparison.OrdinalIgnoreCase)
                .Replace("Dungeons of Lyhe Ghiah", "", StringComparison.OrdinalIgnoreCase);
            if (Regex.IsMatch(sourceText, pattern) &&
                Regex.IsMatch(targetText, pattern) &&
                !originalEn.Trim().Equals(term, StringComparison.OrdinalIgnoreCase))
                result.Warnings.Add($"Possibile categoria ancora in inglese: '{term}' (verificare nomi propri e comandi).");
        }

        bool hasExactTerm = index.ExactTerms.TryGetValue(trimmedOriginal, out var exactTerm);
        if (hasExactTerm && !exactTerm!.Variants.Contains(translatedIt.Trim(), StringComparer.OrdinalIgnoreCase))
            result.Warnings.Add($"'{exactTerm.Term}' → {string.Join(" / ", exactTerm.Variants)} (fonte: {exactTerm.RuleId}).");

        // Other longer glossary terms can still occur inside an exact source label.
        foreach (var group in index.ResidualTerms)
        {
            if (group.EquipmentOnly &&
                (normalizedContext is null ||
                 !(normalizedContext.Equals("items/item.json", StringComparison.OrdinalIgnoreCase) ||
                   normalizedContext.EndsWith("/items/item.json", StringComparison.OrdinalIgnoreCase))))
                continue;
            if (hasExactTerm && group.Term.Equals(trimmedOriginal, StringComparison.OrdinalIgnoreCase)) continue;
            if (group.Pattern!.IsMatch(originalEn) && group.Pattern.IsMatch(translatedIt))
                result.Warnings.Add($"Termine inglese ancora presente: '{group.Term}' → {string.Join(" / ", group.Variants)} (fonte: {group.RuleId}).");
        }
        var originalOutsideQuotedNames = Regex.Replace(originalEn, "[“‘\\\"].*?[”’\\\"]", "");
        var translatedOutsideQuotedNames = Regex.Replace(translatedIt, "[“‘\\\"].*?[”’\\\"]", "");
        if (index.HasDutyIncarico &&
            !originalEn.Trim().Equals("Duty", StringComparison.OrdinalIgnoreCase) &&
            !originalEn.Trim().Equals("Duties", StringComparison.OrdinalIgnoreCase) &&
            Regex.IsMatch(originalOutsideQuotedNames, @"\bdut(?:y|ies)\b", RegexOptions.IgnoreCase) &&
            !originalOutsideQuotedNames.Contains("Duty calls", StringComparison.OrdinalIgnoreCase))
        {
            if (Regex.IsMatch(translatedOutsideQuotedNames, @"\bdut(?:y|ies)\b", RegexOptions.IgnoreCase))
                result.Warnings.Add("'Duty' rimane in inglese; usare Incarico/Incarichi per l'attività di gioco.");
            if (!Regex.IsMatch(originalEn, @"\b(?:quests?|missions?|dailies)\b", RegexOptions.IgnoreCase) &&
                Regex.IsMatch(translatedOutsideQuotedNames, @"\bmission[ei]\b", RegexOptions.IgnoreCase))
                result.Warnings.Add("'Duty' è reso come Missione; usare Incarico/Incarichi per l'attività di gioco.");
        }
        return result;
    }

    private ValidationIndex GetValidationIndex()
    {
        if (_validationIndex is not null) return _validationIndex;

        var groups = _entries
            .GroupBy(entry => entry.EnglishTerm, StringComparer.OrdinalIgnoreCase)
            .Select(group =>
            {
                var variants = GetItalianVariants(group)
                    .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
                string matchTerm = Regex.Replace(group.Key, @"\s+\([^()]*\)$", "").Trim();
                var notes = group.Select(entry => entry.Notes).ToArray();
                bool checkResidual = notes.Any(note => note.StartsWith("Controllo automatico: residuo", StringComparison.OrdinalIgnoreCase));
                bool checkInterjection = notes.Any(note => note.StartsWith("Controllo automatico: intercalare", StringComparison.OrdinalIgnoreCase));
                bool equipmentOnly = notes.Any(note => note.StartsWith("Controllo automatico: suffisso equipaggiamento", StringComparison.OrdinalIgnoreCase));
                Regex? pattern = checkInterjection
                    ? new Regex($@"(?:^|[,;!?]\s*){Regex.Escape(matchTerm)}(?=\s*(?:[!?.,;:]|$))", RegexOptions.IgnoreCase)
                    : checkResidual || equipmentOnly
                        ? new Regex($@"(?<![\p{{L}}\p{{N}}/]){Regex.Escape(matchTerm)}(?![\p{{L}}\p{{N}}])", RegexOptions.IgnoreCase)
                        : null;
                return new TermGroup(group.Key, variants, group.First().RuleId, pattern, equipmentOnly);
            })
            .ToArray();

        var exactTerms = groups.ToDictionary(group => group.Term, StringComparer.OrdinalIgnoreCase);
        var residualTerms = groups.Where(group => group.Pattern is not null).ToArray();
        var approvedLoanwords = _entries
            .Where(entry => entry.Category == GlossaryCategory.GameTerm &&
                entry.EnglishTerm.Equals(entry.ItalianTerm, StringComparison.OrdinalIgnoreCase))
            .Select(entry => entry.EnglishTerm)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        bool hasDutyIncarico = _entries.Any(entry => entry.EnglishTerm == "Duty" && entry.ItalianTerm == "Incarico");

        return _validationIndex = new ValidationIndex(exactTerms, residualTerms, approvedLoanwords, hasDutyIncarico);
    }

    private static IEnumerable<string> GetItalianVariants(IGrouping<string, GlossaryEntry> group)
    {
        foreach (var entry in group)
        {
            foreach (var variant in entry.ItalianTerm.Split(" / ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                yield return variant;

            const string marker = "Varianti ammesse in prosa:";
            int start = entry.Notes.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (start < 0) continue;
            foreach (Match match in Regex.Matches(entry.Notes[start..], @"«([^»]+)»"))
                yield return match.Groups[1].Value.Trim();
        }
    }
}
