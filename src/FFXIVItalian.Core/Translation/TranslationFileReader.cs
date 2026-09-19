using System.Text.Json;

namespace FFXIVItalian.Core.Translation;

public static class TranslationFileReader
{
    public static Dictionary<uint, string> LoadReplacements(string jsonFilePath)
    {
        var result = new Dictionary<uint, string>();

        if (!File.Exists(jsonFilePath))
        {
            return result;
        }

        var json = File.ReadAllText(jsonFilePath);
        using var doc = JsonDocument.Parse(json);

        foreach (var property in doc.RootElement.EnumerateObject())
        {
            if (uint.TryParse(property.Name, out uint rowId))
            {
                if (property.Value.ValueKind == JsonValueKind.String)
                {
                    var val = property.Value.GetString();
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        result[rowId] = val;
                    }
                }
                else if (property.Value.ValueKind == JsonValueKind.Object)
                {
                    if (property.Value.TryGetProperty("translation", out var transProp) &&
                        transProp.ValueKind == JsonValueKind.String)
                    {
                        var val = transProp.GetString();
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            result[rowId] = val;
                        }
                    }
                    else if (property.Value.TryGetProperty("translation_name", out var transNameProp) &&
                             transNameProp.ValueKind == JsonValueKind.String)
                    {
                        var val = transNameProp.GetString();
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            result[rowId] = val;
                        }
                    }
                }
            }
        }

        return result;
    }

    public static Dictionary<uint, (string? Name, string? Description)> LoadTwoStringReplacements(string jsonFilePath)
    {
        var result = new Dictionary<uint, (string? Name, string? Description)>();

        if (!File.Exists(jsonFilePath))
        {
            return result;
        }

        var json = File.ReadAllText(jsonFilePath);
        using var doc = JsonDocument.Parse(json);

        foreach (var property in doc.RootElement.EnumerateObject())
        {
            if (uint.TryParse(property.Name, out uint rowId) && property.Value.ValueKind == JsonValueKind.Object)
            {
                string? name = null;
                string? desc = null;
                bool hasTranslation = false;

                if (property.Value.TryGetProperty("translation_name", out var tName) && tName.ValueKind == JsonValueKind.String)
                {
                    var val = tName.GetString();
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        name = val;
                        hasTranslation = true;
                    }
                }

                if (property.Value.TryGetProperty("translation_description", out var tDesc) && tDesc.ValueKind == JsonValueKind.String)
                {
                    var val = tDesc.GetString();
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        desc = val;
                        hasTranslation = true;
                    }
                }

                if (hasTranslation)
                {
                    result[rowId] = (name, desc);
                }
            }
        }

        return result;
    }

    public static Dictionary<uint, IReadOnlyDictionary<int, string>> LoadLobbyReplacements(string jsonFilePath)
    {
        var result = new Dictionary<uint, IReadOnlyDictionary<int, string>>();

        if (!File.Exists(jsonFilePath))
        {
            return result;
        }

        var json = File.ReadAllText(jsonFilePath);
        using var doc = JsonDocument.Parse(json);

        foreach (var property in doc.RootElement.EnumerateObject())
        {
            if (uint.TryParse(property.Name, out uint rowId))
            {
                var colMap = new Dictionary<int, string>();

                if (property.Value.ValueKind == JsonValueKind.String)
                {
                    string val = property.Value.GetString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(val)) colMap[0] = val;
                }
                else if (property.Value.ValueKind == JsonValueKind.Object)
                {
                    // Col 0: translation_name or translation
                    if (property.Value.TryGetProperty("translation_name", out var tName) &&
                        tName.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(tName.GetString()))
                    {
                        colMap[0] = tName.GetString()!;
                    }
                    else if (property.Value.TryGetProperty("translation", out var tSimple) &&
                             tSimple.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(tSimple.GetString()))
                    {
                        colMap[0] = tSimple.GetString()!;
                    }

                    // Col 4: translation_alt (if present)
                    if (property.Value.TryGetProperty("translation_alt", out var tAlt) &&
                        tAlt.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(tAlt.GetString()))
                    {
                        colMap[4] = tAlt.GetString()!;
                    }

                    // Col 8: translation_description
                    if (property.Value.TryGetProperty("translation_description", out var tDesc) &&
                        tDesc.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(tDesc.GetString()))
                    {
                        colMap[8] = tDesc.GetString()!;
                    }
                }

                if (colMap.Count > 0)
                {
                    result[rowId] = colMap;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Carica un file JSON con nomi maschili e femminili per il clan/razza.
    /// Supporta: translation_name_masculine (offset 0) e translation_name_feminine (offset 4).
    /// </summary>
    public static Dictionary<uint, (string? Masculine, string? Feminine)> LoadTwoColumnNameReplacements(string jsonFilePath)
    {
        var result = new Dictionary<uint, (string? Masculine, string? Feminine)>();

        if (!File.Exists(jsonFilePath))
        {
            return result;
        }

        var json = File.ReadAllText(jsonFilePath);
        using var doc = JsonDocument.Parse(json);

        foreach (var property in doc.RootElement.EnumerateObject())
        {
            if (uint.TryParse(property.Name, out uint rowId) && property.Value.ValueKind == JsonValueKind.Object)
            {
                string? masc = null;
                string? fem  = null;

                if (property.Value.TryGetProperty("translation_name_masculine", out var tm) &&
                    tm.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(tm.GetString()))
                {
                    masc = tm.GetString();
                }

                if (property.Value.TryGetProperty("translation_name_feminine", out var tf) &&
                    tf.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(tf.GetString()))
                {
                    fem = tf.GetString();
                }

                if (masc != null || fem != null)
                {
                    result[rowId] = (masc, fem);
                }
            }
        }

        return result;
    }

    public static Dictionary<uint, IReadOnlyDictionary<int, string>> LoadTextCommandReplacements(string jsonFilePath)
    {
        var result = new Dictionary<uint, IReadOnlyDictionary<int, string>>();

        if (!File.Exists(jsonFilePath))
        {
            return result;
        }

        var json = File.ReadAllText(jsonFilePath);
        using var doc = JsonDocument.Parse(json);

        foreach (var property in doc.RootElement.EnumerateObject())
        {
            if (uint.TryParse(property.Name, out uint rowId) && property.Value.ValueKind == JsonValueKind.Object)
            {
                var colMap = new Dictionary<int, string>();

                // Col 8 is col_2 in TextCommand EXD (usage / help description)
                if (property.Value.TryGetProperty("translation_col_2", out var tCol2) &&
                    tCol2.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(tCol2.GetString()))
                {
                    colMap[8] = tCol2.GetString()!;
                }

                if (colMap.Count > 0)
                {
                    result[rowId] = colMap;
                }
            }
        }

        return result;
    }

    public static Dictionary<uint, IReadOnlyDictionary<int, string>> LoadCustomTalkReplacements(string jsonFilePath)
    {
        var result = new Dictionary<uint, IReadOnlyDictionary<int, string>>();

        if (!File.Exists(jsonFilePath))
        {
            return result;
        }

        var json = File.ReadAllText(jsonFilePath);
        using var doc = JsonDocument.Parse(json);

        foreach (var property in doc.RootElement.EnumerateObject())
        {
            if (uint.TryParse(property.Name, out uint rowId) && property.Value.ValueKind == JsonValueKind.Object)
            {
                var colMap = new Dictionary<int, string>();

                // Col 64 in EXH is col_31 (Offset 240)
                if (property.Value.TryGetProperty("translation_col_31", out var tCol31) &&
                    tCol31.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(tCol31.GetString()))
                {
                    colMap[240] = tCol31.GetString()!;
                }

                // Col 65 in EXH is col_32 (Offset 244)
                if (property.Value.TryGetProperty("translation_col_32", out var tCol32) &&
                    tCol32.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(tCol32.GetString()))
                {
                    colMap[244] = tCol32.GetString()!;
                }

                if (colMap.Count > 0)
                {
                    result[rowId] = colMap;
                }
            }
        }

        return result;
    }
}
