namespace FFXIVItalian.Core.Glossary;

public enum GlossaryCategory
{
    General = 0,
    Aether = 1,
    GameTerm = 2,
    Demonym = 3,
    Place = 4,
    Faction = 5,
    NpcSurnameOrEpithet = 6,
    DutyOrTrial = 7,
    Class = 8,
    Bestiary = 9,
    IconicPhrase = 10,
    UnitOfMeasure = 11,
    KeptUntranslated = 12
}

public class GlossaryEntry
{
    public required string EnglishTerm { get; set; }
    public required string ItalianTerm { get; set; }
    public GlossaryCategory Category { get; set; } = GlossaryCategory.General;
    public string RuleId { get; set; } = string.Empty; // e.g. "G6", "G10", "G11"
    public string Notes { get; set; } = string.Empty;
    public bool CaseSensitive { get; set; } = false;
    public List<string> ProhibitedForms { get; set; } = []; // e.g. "Maelstrom" -> prohibited, must be "La Tempesta"
}

public class VoiceProfile
{
    public required string CharacterName { get; set; }
    public required string Register { get; set; }
    public required string StyleDescription { get; set; }
    public List<string> KeyTraits { get; set; } = [];
    public List<string> CharacteristicPhrases { get; set; } = [];
}

