using System.Text.RegularExpressions;

namespace FFXIVItalian.Core.SeString;

public class SeStringValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = [];
    public List<string> Warnings { get; } = [];
}

public static partial class SeStringValidator
{
    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex TagRegex();

    public static SeStringValidationResult Validate(string originalEn, string translatedIt)
    {
        var result = new SeStringValidationResult();

        if (string.IsNullOrWhiteSpace(translatedIt))
        {
            return result;
        }

        // Check for unbalanced angle brackets
        int openCount = translatedIt.Count(c => c == '<');
        int closeCount = translatedIt.Count(c => c == '>');
        if (openCount != closeCount)
        {
            result.Errors.Add($"Parentesi angolari sbilanciate: trovati {openCount} '<' e {closeCount} '>'.");
        }

        var originalTags = TagRegex().Matches(originalEn).Select(m => m.Value).ToList();
        var translatedTags = TagRegex().Matches(translatedIt).Select(m => m.Value).ToList();

        // Check for placeholder tags that must be preserved exactly
        // e.g. <FullName>, <Forename>, <Surname>, <PlayerParameter(...)>, <Sheet(...)>, <SheetEn(...)>
        var essentialTags = originalTags
            .Where(t => t.StartsWith("<Full", StringComparison.OrdinalIgnoreCase)
                     || t.StartsWith("<Fore", StringComparison.OrdinalIgnoreCase)
                     || t.StartsWith("<Sur", StringComparison.OrdinalIgnoreCase)
                     || t.StartsWith("<Sheet", StringComparison.OrdinalIgnoreCase)
                     || t.StartsWith("<Clickable", StringComparison.OrdinalIgnoreCase)
                     || t.StartsWith("</Clickable", StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var essential in essentialTags)
        {
            if (!translatedTags.Contains(essential))
            {
                result.Errors.Add($"Tag SeString obbligatorio mancante nella traduzione: '{essential}'.");
            }
        }

        // Check paired tags: Highlight, Color, Emphasis
        CheckPairedTag(translatedTags, "<Highlight>", "</Highlight>", "Highlight", result);
        CheckPairedTag(translatedTags, "<Emphasis>", "</Emphasis>", "Emphasis", result);

        return result;
    }

    private static void CheckPairedTag(List<string> tags, string openTag, string closeTag, string tagName, SeStringValidationResult result)
    {
        int opens = tags.Count(t => t.Equals(openTag, StringComparison.OrdinalIgnoreCase));
        int closes = tags.Count(t => t.Equals(closeTag, StringComparison.OrdinalIgnoreCase));
        if (opens != closes)
        {
            result.Errors.Add($"Tag accoppiato sbilanciato per {tagName}: {opens} aperture vs {closes} chiusure.");
        }
    }
}

