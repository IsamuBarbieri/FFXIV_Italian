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
}
