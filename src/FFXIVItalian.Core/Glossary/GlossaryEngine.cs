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
    private readonly Dictionary<string, VoiceProfile> _voiceProfiles = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<GlossaryEntry> Entries => _entries;
    public IReadOnlyDictionary<string, VoiceProfile> VoiceProfiles => _voiceProfiles;

    public void AddEntry(GlossaryEntry entry)
    {
        _entries.Add(entry);
    }

    public void AddVoiceProfile(VoiceProfile profile)
    {
        _voiceProfiles[profile.CharacterName] = profile;
    }

    public GlossaryComplianceResult ValidateTranslation(string originalEn, string translatedIt)
    {
        var result = new GlossaryComplianceResult();

        if (string.IsNullOrWhiteSpace(translatedIt))
            return result;

        // 1. Check prohibited forms (terms that should never appear in Italian text)
        foreach (var entry in _entries)
        {
            foreach (var prohibited in entry.ProhibitedForms)
            {
                if (Regex.IsMatch(translatedIt, $@"\b{Regex.Escape(prohibited)}\b", RegexOptions.IgnoreCase))
                {
                    result.ProhibitedUsages.Add($"Forma vietata rilevata [{entry.RuleId}]: '{prohibited}'. Usare '{entry.ItalianTerm}'.");
                }
            }
        }

        // 2. Check for units of measurement violations (G28)
        // Eorzean units: yalm, fulm, ilm, malm, ponze, onze, tonze must remain invariable and never translated to metric/imperial
        var forbiddenUnitRegexes = new Dictionary<string, string>
        {
            { @"\biarde?\b", "G28: Non tradurre in 'iarda/iarde'; usa 'yalm' (invariabile)." },
            { @"(?<!in\s+)\bpied[ei]\b", "G28: Non tradurre in 'piede/piedi'; usa 'fulm' (invariabile)." },
            { @"\bmigli[ao]\b", "G28: Non tradurre in 'miglio/miglia'; usa 'malm' (invariabile)." },
            { @"\blibbr[ae]\b", "G28: Non tradurre in 'libbra/libbre'; usa 'ponze' (invariabile)." },
            { @"\byalms\b", "G28: 'yalm' è invariabile al plurale (usa 'yalm', non 'yalms')." },
            { @"\bfulms\b", "G28: 'fulm' è invariabile al plurale (usa 'fulm', non 'fulms')." },
            { @"\bmalms\b", "G28: 'malm' è invariabile al plurale (usa 'malm', non 'malms')." }
        };

        foreach (var (pattern, message) in forbiddenUnitRegexes)
        {
            if (Regex.IsMatch(translatedIt, pattern, RegexOptions.IgnoreCase))
            {
                result.Warnings.Add(message);
            }
        }

        // 3. Check for English names that were mistakenly left untranslated
        // when the English text contained a mandatory translated term (e.g. "Warrior of Light", "Scions of the Seventh Dawn", "Vesper Bay")
        foreach (var entry in _entries.Where(e => e.Category != GlossaryCategory.KeptUntranslated))
        {
            // If the term is also registered as KeptUntranslated (e.g. Sharlayan), it is valid to retain it as a proper noun
            if (_entries.Any(e => e.Category == GlossaryCategory.KeptUntranslated && e.EnglishTerm.Equals(entry.EnglishTerm, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            // If English source contained the term...
            if (Regex.IsMatch(originalEn, $@"\b{Regex.Escape(entry.EnglishTerm)}\b", RegexOptions.IgnoreCase))
            {
                // But Italian text still contains the pure English term verbatim...
                if (Regex.IsMatch(translatedIt, $@"\b{Regex.Escape(entry.EnglishTerm)}\b", RegexOptions.IgnoreCase)
                    && !entry.EnglishTerm.Equals(entry.ItalianTerm, StringComparison.OrdinalIgnoreCase))
                {
                    result.Warnings.Add($"Il termine '{entry.EnglishTerm}' [{entry.RuleId}] sembra essere stato lasciato in inglese. Forma italiana richiesta: '{entry.ItalianTerm}'.");
                }
            }
        }

        return result;
    }
}

