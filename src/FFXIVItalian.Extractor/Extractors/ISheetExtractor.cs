using Lumina;

namespace FFXIVItalian.Extractor.Extractors;

public interface ISheetExtractor
{
    string SheetName { get; }
    string DefaultJsonFileName { get; }
    string Description { get; }

    /// <summary>
    /// Extracts the sheet from Lumina into the target JSON file.
    /// If targetJsonPath already exists, existing translations are preserved.
    /// </summary>
    int ExtractAndSave(GameData lumina, string targetJsonPath);

    /// <summary>
    /// Inspects and prints detailed diagnostics for the sheet and an optional row.
    /// </summary>
    void Inspect(GameData lumina, uint? targetRowId = null);
}

