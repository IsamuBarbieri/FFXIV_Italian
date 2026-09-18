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
                    result[rowId] = property.Value.GetString() ?? string.Empty;
                }
                else if (property.Value.ValueKind == JsonValueKind.Object)
                {
                    if (property.Value.TryGetProperty("translation", out var transProp) &&
                        transProp.ValueKind == JsonValueKind.String)
                    {
                        result[rowId] = transProp.GetString() ?? string.Empty;
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

                if (property.Value.TryGetProperty("translation_name", out var tName) && tName.ValueKind == JsonValueKind.String)
                {
                    name = tName.GetString();
                }
                else if (property.Value.TryGetProperty("name", out var n) && n.ValueKind == JsonValueKind.String)
                {
                    name = n.GetString();
                }

                if (property.Value.TryGetProperty("translation_description", out var tDesc) && tDesc.ValueKind == JsonValueKind.String)
                {
                    desc = tDesc.GetString();
                }
                else if (property.Value.TryGetProperty("description", out var d) && d.ValueKind == JsonValueKind.String)
                {
                    desc = d.GetString();
                }

                if (name != null || desc != null)
                {
                    result[rowId] = (name, desc);
                }
            }
        }

        return result;
    }
}

