using Lumina;
using Lumina.Data;
using System;
using System.Buffers.Binary;
using System.Text.Json.Nodes;

namespace FFXIVItalian.Extractor.Extractors;

public sealed class AddonExtractor : BaseSheetExtractor
{
    public override string SheetName => "Addon";
    public override string DefaultJsonFileName => "addon.json";
    public override string Description => "Etichette UI, pulsanti standard (OK, Annulla), intestazioni finestre ed elementi di sistema.";

    public override int ExtractAndSave(GameData lumina, string targetJsonPath)
    {
        var exd = lumina.GetFile("exd/addon_0_en.exd");
        if (exd == null) throw new InvalidOperationException("exd/addon_0_en.exd non trovato.");

        var existing = LoadExistingJson(targetJsonPath);
        var root = new JsonObject();

        uint idx = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(0x08, 4));
        int count = (int)(idx / 8);
        int fixedSize = 4;
        int extracted = 0;

        for (int i = 0; i < count; i++)
        {
            int entryPos = 0x20 + (i * 8);
            uint rId = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(entryPos, 4));
            uint off = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(entryPos + 4, 4));
            int dataSize = (int)BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan((int)off, 4));

            string text = ReadRowString(exd.Data, (int)off + 6, fixedSize, 0, dataSize);
            string key = rId.ToString();

            // If file exists, we preserve any rows already present or translated
            string translation = string.Empty;
            bool hasExisting = false;

            if (existing != null && existing.TryGetPropertyValue(key, out var oldNode) && oldNode is JsonObject oldObj)
            {
                hasExisting = true;
                if (oldObj.TryGetPropertyValue("translation", out var transNode))
                {
                    translation = transNode?.GetValue<string>() ?? string.Empty;
                }
            }

            // For Addon (19,000+ entries), only include non-empty rows or already-translated rows
            if (string.IsNullOrWhiteSpace(text) && !hasExisting) continue;

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

