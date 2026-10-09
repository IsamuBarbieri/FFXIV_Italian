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
    private static readonly IReadOnlyDictionary<string, (string Singular, string Plural)> EorzeanMeasureForms = new Dictionary<string, (string Singular, string Plural)>(StringComparer.OrdinalIgnoreCase)
    {
        ["ilm"] = ("ilm", "ilmi"), ["ilms"] = ("ilm", "ilmi"),
        ["fulm"] = ("fulm", "fulmi"), ["fulms"] = ("fulm", "fulmi"),
        ["yalm"] = ("yalm", "yalmi"), ["yalms"] = ("yalm", "yalmi"),
        ["malm"] = ("malm", "malmi"), ["malms"] = ("malm", "malmi"),
        ["onze"] = ("onza", "onze"), ["onzes"] = ("onza", "onze"),
        ["ponze"] = ("ponza", "ponze"), ["ponzes"] = ("ponza", "ponze"),
        ["tonze"] = ("tonza", "tonze"), ["tonzes"] = ("tonza", "tonze")
    };

    private static readonly Regex PhysicalDungeonContext = new(
        @"\b(?:this|that)\s+(?:[\p{L}-]+\s+){0,2}dungeons?\b|\b(?:narrow|dark|abandoned|underground)\s+dungeons?\b|\bdungeons?\s+(?:entrance|exit|interior|depths|denizens|inhabitants)\b|\b(?:entrance|exit)\s+(?:to|of)\s+(?:(?:the|this|that)\s+)?dungeons?\b|\b(?:inside|within|in)\s+(?:the|this|that)\s+dungeons?\b(?!\s+finder\b)|\b(?:exit|exiting|leave|leaving)\s+(?:the|this|that)\s+dungeons?\b|\bcome and go from\s+(?:the\s+)?(?:[\p{L}-]+\s+)?dungeons?\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex DungeonActivityContext = new(
        @"\bdungeon-delving\b|\bdungeon finder\b|\b(?:equipment|quests?|activities|content)\s+and\s+dungeons?\b|\b(?:challenge|challenging|rechallenge|rechallenging|run|clear(?:ed)?|complete(?:d)?|queue for|explore|delve into|delving into)\s+(?:(?:the|a|an)\s+)?dungeons?\b|\b(?:variant|criterion|instanced)\s+dungeons?\b|\b(?:enter|entering)\s+dungeons?\s+and\s+other\s+duties\b|\benter\s+(?:(?:the|a|an)\s+)?dungeon\s+as\s+a\s+solo\s+player\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex DutyExitVoteContext = new(
        @"\bend duty\b.*\bmost votes\b.*\b(?:exit|leave the dungeon)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline);

    private static bool IsPluralMeasureContext(string original, Match measure)
    {
        if (measure.Value.EndsWith('s')) return true;
        string following = original[(measure.Index + measure.Length)..];
        if (Regex.IsMatch(following, @"^\s+or\s+two\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            return true;
        string preceding = original[..measure.Index];
        var numeric = Regex.Match(preceding, @"(?<number>\d[\d,]*)[-\s]*$", RegexOptions.CultureInvariant);
        if (numeric.Success && long.TryParse(numeric.Groups["number"].Value.Replace(",", ""), out long count))
            return count > 1;
        var spelledNumber = Regex.Match(preceding,
            @"\b(?<number>(?:twenty|thirty|forty|fifty|sixty|seventy|eighty|ninety)(?:\s+|-)?(?:one|two|three|four|five|six|seven|eight|nine)?|zero|one|two|three|four|five|six|seven|eight|nine|ten|eleven|twelve|thirteen|fourteen|fifteen|sixteen|seventeen|eighteen|nineteen)(?:\s+|-)+$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (spelledNumber.Success)
            return !spelledNumber.Groups["number"].Value.Equals("one", StringComparison.OrdinalIgnoreCase);
        return Regex.IsMatch(preceding,
            @"\b(?:many|several|numerous|multiple|few|various|hundreds?|thousands?)(?:\s+|-)(?:[\p{L}-]+\s+){0,3}$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    private static string RemoveKnownProperNamesFromMeasureCheck(string original)
    {
        return Regex.Replace(original,
            @"\bHell of Malms\b|\bBaldewyn Four Yalms\b|\bSix Fulms Under\b",
            "", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    private static string RemoveKnownMeasureIdioms(string original)
    {
        return Regex.Replace(original,
            @"\bevery ilm (?=(?:an|the|as)\b)|\bevery ilm of (?:these mines|the)\b|\bwithin an ilm of (?:its|one's|his|her) life\b|\bhardest yalms\b|\bI was malms away\b|\bevery last onze of\b|\bevery onze of\b|\bdon't do an onze of good\b|\ba tonze of research left to do\b|\ban ilm or two taller\b",
            "", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    private static bool UsesExpeditionForPhysicalDungeon(string originalEn, string translatedIt)
    {
        bool deicticPlace = Regex.IsMatch(originalEn,
            @"\b(?:this|that)\s+(?:[\p{L}-]+\s+){0,2}dungeons?\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (deicticPlace && Regex.IsMatch(translatedIt,
                @"\b(?:questa|questo|quella|quello|queste|questi)\s+spedizion\w*\b",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            return true;

        bool physicalThreshold = Regex.IsMatch(originalEn,
            @"\bdungeons?\s+(?:entrance|exit|interior|depths|denizens|inhabitants)\b|\b(?:entrance|exit)\s+(?:to|of)\s+(?:(?:the|this|that)\s+)?dungeons?\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (physicalThreshold && Regex.IsMatch(translatedIt,
                @"\b(?:ingresso|entrata|uscita|interno|profondit\w*|abitanti|creature)\s+(?:della|dello|del|delle|degli|dei|dell['’])\s+spedizion\w*\b",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            return true;

        bool physicalExit = Regex.IsMatch(originalEn,
            @"\b(?:exit|exiting|leave|leaving)\s+(?:the|this|that)\s+dungeons?\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (physicalExit && Regex.IsMatch(translatedIt,
                @"\b(?:esci|uscire|uscendo|lascia|lasci|lasciare|abbandona|abbandonare)\b[^.!?]{0,60}\bspedizion\w*\b",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            return true;

        bool physicalInterior = Regex.IsMatch(originalEn,
            @"\b(?:inside|within|in)\s+(?:the|this|that)\s+dungeons?\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (physicalInterior && Regex.IsMatch(translatedIt,
                @"\b(?:dentro|all['’]interno|nel|nello|nella|nei|nelle)\b[^.!?]{0,60}\bspedizion\w*\b",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            return true;

        bool descriptivePlace = Regex.IsMatch(originalEn,
            @"\b(?:narrow|dark|abandoned|underground)\s+dungeons?\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        return descriptivePlace && Regex.IsMatch(translatedIt,
            @"\bspedizion\w*[^.!?]{0,30}\b(?:strett\w*|bu[iy]\w*|oscur\w*|abbandonat\w*|sotterrane\w*)\b|\b(?:strett\w*|bu[iy]\w*|oscur\w*|abbandonat\w*|sotterrane\w*)\b[^.!?]{0,30}\bspedizion\w*\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    private string RemoveCanonicalSubterranePlaceNames(string originalEn, string translatedIt)
    {
        foreach (var place in _canonicalSubterranePlaceNames)
            if (place.Source.IsMatch(originalEn))
                translatedIt = place.Target.Replace(translatedIt, "");
        return translatedIt;
    }

    private sealed record TermGroup(string Term, string[] Variants, string RuleId, Regex? Pattern, bool EquipmentOnly, bool RoleSuffixOnly);

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
    private List<(Regex Source, Regex Target)> _canonicalSubterranePlaceNames = [];
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

        var placeNamePatterns = new List<(Regex Source, Regex Target)>();
        foreach (var place in _placeNames.Where(place =>
                     place.Key.Contains("Subterrane", StringComparison.OrdinalIgnoreCase) &&
                     place.Value.Italian.Contains("sotterrane", StringComparison.OrdinalIgnoreCase)))
        {
            var sourceForms = new[]
            {
                place.Key,
                Regex.Replace(place.Key, @"^(?:the|another|a)\s+", "", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
            }.Distinct(StringComparer.OrdinalIgnoreCase);
            var targetForms = new[]
            {
                place.Value.Italian,
                Regex.Replace(place.Value.Italian, @"^(?:il|lo|la|i|gli|le)\s+", "", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
            }.Distinct(StringComparer.OrdinalIgnoreCase);
            string sourcePattern = string.Join("|", sourceForms.OrderByDescending(form => form.Length).Select(Regex.Escape));
            string targetPattern = string.Join("|", targetForms.OrderByDescending(form => form.Length).Select(Regex.Escape));
            placeNamePatterns.Add((
                new Regex($@"(?<![\p{{L}}])(?:{sourcePattern})(?![\p{{L}}])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled),
                new Regex($@"(?<![\p{{L}}])(?:{targetPattern})(?![\p{{L}}])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled)));
        }
        _canonicalSubterranePlaceNames = placeNamePatterns;
    }

    public GlossaryComplianceResult ValidateTranslation(string originalEn, string translatedIt, string? sourceContext = null, bool checkActivityCategories = true)
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
        var originalOutsideQuotedNames = Regex.Replace(originalEn, "[“‘\\\"].*?[”’\\\"]", "");
        var translatedOutsideQuotedNames = Regex.Replace(translatedIt, "[“‘\\\"].*?[”’\\\"]", "");

        var reportedMeasureForms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var measureCheckSource = RemoveKnownMeasureIdioms(RemoveKnownProperNamesFromMeasureCheck(originalOutsideQuotedNames));
        foreach (Match measure in Regex.Matches(measureCheckSource, @"(?<![\p{L}])[\p{L}]+(?![\p{L}])"))
        {
            if (!EorzeanMeasureForms.TryGetValue(measure.Value, out var forms)) continue;
            string expectedForm = IsPluralMeasureContext(measureCheckSource, measure) ? forms.Plural : forms.Singular;
            bool hasExpectedForm = Regex.IsMatch(translatedOutsideQuotedNames,
                    $@"(?<![\p{{L}}]){Regex.Escape(expectedForm)}(?![\p{{L}}])",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            bool usesGrammaticalSingularAfterQualche = expectedForm.Equals(forms.Plural, StringComparison.OrdinalIgnoreCase) &&
                Regex.IsMatch(translatedOutsideQuotedNames,
                    $@"\bqualche\s+{Regex.Escape(forms.Singular)}\b",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            if (!hasExpectedForm && !usesGrammaticalSingularAfterQualche && reportedMeasureForms.Add(measure.Value + "=" + expectedForm))
                result.Warnings.Add($"Unità eorzeana «{measure.Value}»: in italiano usare «{expectedForm}» (verificare genere e numero).");
        }

        bool sourceHasItemLevel = Regex.IsMatch(originalOutsideQuotedNames, @"\bItem Level\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) ||
            Regex.IsMatch(originalOutsideQuotedNames, @"(?<![\p{L}])IL\s*:?\s*\d+\b", RegexOptions.CultureInvariant);
        bool translationKeepsEnglishItemLevel = Regex.IsMatch(translatedOutsideQuotedNames, @"(?<![\p{L}])IL\s*:?\s*\d+\b", RegexOptions.CultureInvariant);
        if (sourceHasItemLevel && translationKeepsEnglishItemLevel)
            result.Warnings.Add("Sigla Item Level «IL» ancora presente: usare «Liv.Ogg.» o «Livello Oggetto».");

        if (checkActivityCategories)
        {
            // This phrase is a proper orchestrion track title, not a generic dungeon reference.
            var sourceCategoryText = Regex.Replace(originalOutsideQuotedNames, @"Battle in the Dungeon(?:\s+#\d+)?", "", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            var targetCategoryText = Regex.Replace(translatedOutsideQuotedNames, @"Battaglia nel dungeon(?:\s+#\d+)?", "", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            bool dutyExitVote = DutyExitVoteContext.IsMatch(originalEn);
            bool physicalDungeon = !dutyExitVote && PhysicalDungeonContext.IsMatch(sourceCategoryText);
            bool dungeonActivity = dutyExitVote || (!physicalDungeon && DungeonActivityContext.IsMatch(sourceCategoryText));
            if (physicalDungeon && UsesExpeditionForPhysicalDungeon(sourceCategoryText, targetCategoryText))
                result.Warnings.Add("Dungeon indica un luogo fisico in questo contesto; usare una forma di «sotterraneo», non «spedizione».");
            else if (physicalDungeon && Regex.IsMatch(targetCategoryText, @"\bdungeons?\b", RegexOptions.IgnoreCase))
                result.Warnings.Add("Dungeon indica un luogo fisico in questo contesto; tradurre con una forma di «sotterraneo».");
            else if (dungeonActivity && Regex.IsMatch(RemoveCanonicalSubterranePlaceNames(sourceCategoryText, targetCategoryText), @"\bsotterrane\w*\b", RegexOptions.IgnoreCase))
                result.Warnings.Add("Dungeon indica un'attività in questo contesto; usare una forma di «spedizione», non «sotterraneo».");

            foreach (var term in new[] { "dungeon", "raid", "trial", "guildhest", "levequest", "subquest" })
            {
                if (term == "dungeon" && (physicalDungeon || dungeonActivity)) continue;
                if (index.ApprovedItalianLoanwords.Contains(term)) continue;
                var pattern = $@"\b{term}s?\b";
                // These are proper titles, even though they contain category words.
                var sourceText = sourceCategoryText.Replace("Trials of the Braves", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("Dungeons of Lyhe Ghiah", "", StringComparison.OrdinalIgnoreCase);
                var targetText = targetCategoryText.Replace("Trials of the Braves", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("Dungeons of Lyhe Ghiah", "", StringComparison.OrdinalIgnoreCase);
                if (Regex.IsMatch(sourceText, pattern) &&
                    Regex.IsMatch(targetText, pattern) &&
                    !originalEn.Trim().Equals(term, StringComparison.OrdinalIgnoreCase))
                    result.Warnings.Add($"Possibile categoria ancora in inglese: '{term}' (verificare nomi propri e comandi).");
            }
        }

        bool hasExactTerm = index.ExactTerms.TryGetValue(trimmedOriginal, out var exactTerm);
        // WAR means the Warrior job only in the class/job category sheet; racing dialogue uses the ordinary word.
        if (hasExactTerm && exactTerm!.Term.Equals("WAR", StringComparison.OrdinalIgnoreCase) &&
            normalizedContext?.EndsWith("/world/classjobcategory.json", StringComparison.OrdinalIgnoreCase) != true)
        {
            hasExactTerm = false;
            exactTerm = null;
        }
        if (hasExactTerm && !exactTerm!.Variants.Contains(translatedIt.Trim(), StringComparer.OrdinalIgnoreCase))
            result.Warnings.Add($"'{exactTerm.Term}' → {string.Join(" / ", exactTerm.Variants)} (fonte: {exactTerm.RuleId}).");

        // Other longer glossary terms can still occur inside an exact source label.
        foreach (var group in index.ResidualTerms)
        {
            if ((group.EquipmentOnly || group.RoleSuffixOnly) &&
                (normalizedContext is null ||
                 !(normalizedContext.Equals("items/item.json", StringComparison.OrdinalIgnoreCase) ||
                   normalizedContext.EndsWith("/items/item.json", StringComparison.OrdinalIgnoreCase))))
                continue;
            if (group.Term.Equals("Birdsong (gear)", StringComparison.OrdinalIgnoreCase) &&
                (Regex.IsMatch(originalOutsideQuotedNames, @"\bOrchestrion Rolls?\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) ||
                 (normalizedContext?.EndsWith("/items/item.json", StringComparison.OrdinalIgnoreCase) == true &&
                   trimmedOriginal.Equals("Birdsong", StringComparison.OrdinalIgnoreCase))))
                continue;
            if (group.Term.Equals("Rolanberry", StringComparison.OrdinalIgnoreCase) &&
                Regex.IsMatch(originalOutsideQuotedNames, @"\bRolanberry children\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                continue;
            // The IL abbreviation has a dedicated case-sensitive check above; an IgnoreCase glossary regex also sees Italian "il".
            if (group.Term.Equals("IL", StringComparison.OrdinalIgnoreCase)) continue;
            if (hasExactTerm && group.Term.Equals(trimmedOriginal, StringComparison.OrdinalIgnoreCase)) continue;
            if (group.Pattern!.IsMatch(originalOutsideQuotedNames) && group.Pattern.IsMatch(translatedOutsideQuotedNames))
                result.Warnings.Add($"Termine inglese ancora presente: '{group.Term}' → {string.Join(" / ", group.Variants)} (fonte: {group.RuleId}).");
        }

        bool sourceHasTomestonesPlural = Regex.IsMatch(originalOutsideQuotedNames, @"\btomestones\b", RegexOptions.IgnoreCase) &&
            !Regex.IsMatch(originalOutsideQuotedNames, @"\btomestone\b", RegexOptions.IgnoreCase);
        if (sourceHasTomestonesPlural && Regex.IsMatch(translatedOutsideQuotedNames, @"\btavoletta\b", RegexOptions.IgnoreCase) &&
            !Regex.IsMatch(translatedOutsideQuotedNames, @"\btavolette\b", RegexOptions.IgnoreCase))
            result.Warnings.Add("Tomestones è plurale in originale: verificare che «tavoletta» non sia al singolare.");
        if (Regex.IsMatch(translatedOutsideQuotedNames,
                @"\b(?:il|lo|un|questo|quello|al|nel|sul|dal|del|dello)\s+(?:(?:tuo|suo|mio|nostro|vostro)\s+)?tavoletta\b|\btavoletta\s+allagano\b",
                RegexOptions.IgnoreCase))
            result.Warnings.Add("Accordo di genere errato per «tavoletta»: verificare articolo, possessivo e aggettivi al femminile.");
        if (Regex.IsMatch(translatedOutsideQuotedNames, @"\btavoletta\b.{0,400}\bpuò essere scambiato\b|\btavoletta\b.{0,60}\b(?:trovati|ottenuti|scambiati|conservati)\b", RegexOptions.IgnoreCase) ||
            Regex.IsMatch(translatedOutsideQuotedNames, @"\b(?:micro)?tavolette\b.{0,60}\b(?:piccoli|ricercati|scambiati|trovati|ottenuti|conservati)\b", RegexOptions.IgnoreCase))
            result.Warnings.Add("Possibile errore di accordo con «tavoletta»: controllare genere e numero di participi e aggettivi.");
        if (index.HasDutyIncarico &&
            !originalEn.Trim().Equals("Duty", StringComparison.OrdinalIgnoreCase) &&
            !originalEn.Trim().Equals("Duties", StringComparison.OrdinalIgnoreCase))
        {
            string[] sourceSegments = Regex.Split(originalOutsideQuotedNames, @"(?i)<hex:02100103>");
            string[] translatedSegments = Regex.Split(translatedOutsideQuotedNames, @"(?i)<hex:02100103>");
            bool hasDutySegment = sourceSegments.Length == translatedSegments.Length &&
                sourceSegments.Where((segment, index) =>
                    Regex.IsMatch(segment, @"\bdut(?:y|ies)\b", RegexOptions.IgnoreCase) &&
                    !segment.Contains("Duty calls", StringComparison.OrdinalIgnoreCase) &&
                    Regex.IsMatch(translatedSegments[index], @"\bmission[ei]\b", RegexOptions.IgnoreCase)).Any();
            bool hasEnglishDutySegment = sourceSegments.Length == translatedSegments.Length &&
                sourceSegments.Where((segment, index) =>
                    Regex.IsMatch(segment, @"\bdut(?:y|ies)\b", RegexOptions.IgnoreCase) &&
                    !segment.Contains("Duty calls", StringComparison.OrdinalIgnoreCase) &&
                    Regex.IsMatch(translatedSegments[index], @"\bdut(?:y|ies)\b", RegexOptions.IgnoreCase)).Any();
            if (hasEnglishDutySegment)
                result.Warnings.Add("'Duty' rimane in inglese; usare Incarico/Incarichi per l'attività di gioco.");
            if (!Regex.IsMatch(originalEn, @"\b(?:quests?|missions?|dailies)\b", RegexOptions.IgnoreCase) && hasDutySegment)
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
                bool checkPlural = notes.Any(note => note.StartsWith("Controllo automatico: residuo plurale", StringComparison.OrdinalIgnoreCase));
                bool checkInterjection = notes.Any(note => note.StartsWith("Controllo automatico: intercalare", StringComparison.OrdinalIgnoreCase));
                bool equipmentOnly = notes.Any(note => note.StartsWith("Controllo automatico: suffisso equipaggiamento", StringComparison.OrdinalIgnoreCase));
                bool roleSuffixOnly = notes.Any(note => note.StartsWith("Controllo automatico: suffisso ruolo equipaggiamento", StringComparison.OrdinalIgnoreCase));
                string residualTerm = Regex.Escape(matchTerm).Replace("'", "['’]");
                Regex? pattern = checkInterjection
                    ? new Regex($@"(?:^|[,;!?]\s*){Regex.Escape(matchTerm)}(?=\s*(?:[!?.,;:]|$))", RegexOptions.IgnoreCase)
                    : roleSuffixOnly
                        ? new Regex($@"(?<![\p{{L}}\p{{N}}/]){Regex.Escape(matchTerm)}(?=\s*(?:<hex:[^>]+>\s*)*(?:\(IL\s*\d+\))?[.!?]?$)", RegexOptions.IgnoreCase)
                        : checkResidual || equipmentOnly
                        ? new Regex($@"(?<![\p{{L}}\p{{N}}/]){residualTerm}{(checkPlural ? "s?" : "")}(?![\p{{L}}\p{{N}}])", RegexOptions.IgnoreCase)
                        : null;
                return new TermGroup(group.Key, variants, group.First().RuleId, pattern, equipmentOnly || roleSuffixOnly, roleSuffixOnly);
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

            var marker = Regex.Match(entry.Notes, @"variant[ei]\s+ammess[ae][^:]*:\s*", RegexOptions.IgnoreCase);
            if (!marker.Success) continue;
            foreach (Match match in Regex.Matches(entry.Notes[marker.Index..], @"«([^»]+)»"))
                yield return match.Groups[1].Value.Trim();
        }
    }
}
