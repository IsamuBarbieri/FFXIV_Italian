namespace FFXIVItalian.Core.Models;

public enum TranslationStatus
{
    Untranslated = 0,
    Draft = 1,
    Approved = 2,
    Gold = 3,
    Deprecated = 4
}

public enum CharacterGender
{
    Unknown = 0,
    Male = 1,
    Female = 2,
    Player = 3,
    Group = 4,
    None = 5
}

public class TranslationRow
{
    public required string Key { get; set; }
    public required string SheetName { get; set; }
    public uint RowId { get; set; }
    public ushort SubRowId { get; set; }
    public int ColumnIndex { get; set; }

    public required string OriginalEn { get; set; }
    public string CurrentIt { get; set; } = string.Empty;
    public TranslationStatus Status { get; set; } = TranslationStatus.Untranslated;
    public string SourceHash { get; set; } = string.Empty;

    // Context & Speaker metadata for gender resolution
    public uint? SpeakerId { get; set; }
    public string? SpeakerName { get; set; }
    public CharacterGender SpeakerGender { get; set; } = CharacterGender.Unknown;

    public uint? TargetId { get; set; }
    public string? TargetName { get; set; }
    public CharacterGender TargetGender { get; set; } = CharacterGender.Unknown;

    public string? QuestId { get; set; }
    public string? QuestName { get; set; }
    public string? WikiUrl { get; set; }
    public string? TranslatorNotes { get; set; }

    public bool SeStringValid { get; set; } = true;
    public List<string> ValidationWarnings { get; set; } = [];
}
