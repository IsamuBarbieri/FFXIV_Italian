using Lumina;
using Lumina.Data;
using System;
using System.Buffers.Binary;
using System.Text.Json.Nodes;

namespace FFXIVItalian.Extractor.Extractors;

public sealed class MainCommandCategoryExtractor : BaseSheetExtractor
{
    public override string SheetName => "MainCommandCategory";
    public override string DefaultJsonFileName => "maincommandcategory.json";
    public override string Description => "Macro-categorie del menu principale (Duty, Character, System...).";

    public override int ExtractAndSave(GameData lumina, string targetJsonPath)
    {
        var exd = lumina.GetFile("exd/maincommandcategory_0_en.exd");
        if (exd == null) throw new InvalidOperationException("exd/maincommandcategory_0_en.exd non trovato.");

        var existing = LoadExistingJson(targetJsonPath);
        var root = new JsonObject();

        uint idx = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(0x08, 4));
        int count = (int)(idx / 8);
        int fixedSize = 8;
        int extracted = 0;

        for (int i = 0; i < count; i++)
        {
            int entryPos = 0x20 + (i * 8);
            uint rId = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(entryPos, 4));
            uint off = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(entryPos + 4, 4));
            int dataSize = (int)BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan((int)off, 4));

            string text = ReadRowString(exd.Data, (int)off + 6, fixedSize, 0, dataSize);
            if (string.IsNullOrWhiteSpace(text)) continue;

            string key = rId.ToString();
            string translation = string.Empty;

            if (existing != null && existing.TryGetPropertyValue(key, out var oldNode) && oldNode is JsonObject oldObj)
            {
                if (oldObj.TryGetPropertyValue("translation", out var transNode))
                {
                    translation = transNode?.GetValue<string>() ?? string.Empty;
                }
            }

            var entryObj = new JsonObject
            {
                ["original"] = text,
                ["translation"] = translation
            };

            root[key] = entryObj;
            extracted++;
        }

        SaveJsonObject(targetJsonPath, root);
        return extracted;
    }
}

