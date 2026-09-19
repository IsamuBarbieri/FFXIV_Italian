using System;
using System.Collections.Generic;
using System.Linq;

namespace FFXIVItalian.Extractor.Extractors;

public static class ExtractorRegistry
{
    private static readonly Dictionary<string, ISheetExtractor> Extractors = new(StringComparer.OrdinalIgnoreCase)
    {
        ["lobby"] = new LobbyExtractor(),
        ["addon"] = new AddonExtractor(),
        ["maincommand"] = new MainCommandExtractor(),
        ["maincommandcategory"] = new MainCommandCategoryExtractor(),
        ["error"] = new ErrorExtractor(),
        ["classjob"] = new ClassJobExtractor(),
        ["placename"] = new PlaceNameExtractor(),
        ["tribe"] = new TribeExtractor(),
        ["race"] = new RaceExtractor()
    };

    public static ISheetExtractor? Get(string sheetName)
    {
        return Extractors.TryGetValue(sheetName, out var extractor) ? extractor : null;
    }

    public static IReadOnlyList<ISheetExtractor> GetAll()
    {
        return Extractors.Values.ToList();
    }

    public static IReadOnlyList<string> GetNames()
    {
        return Extractors.Keys.ToList();
    }
}

