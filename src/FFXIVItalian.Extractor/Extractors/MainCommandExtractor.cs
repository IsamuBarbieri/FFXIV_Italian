using Lumina;
using Lumina.Data;
using System;
using System.Buffers.Binary;
using System.Text.Json.Nodes;

namespace FFXIVItalian.Extractor.Extractors;

public sealed class MainCommandExtractor : BaseSheetExtractor
{
    public override string SheetName => "MainCommand";
    public override string DefaultJsonFileName => "maincommand.json";
    public override string Description => "Comandi del menu principale con nomi e descrizioni (Journal, Duty Finder, Azioni...).";

    public override int ExtractAndSave(GameData lumina, string targetJsonPath)
    {
        var exd = lumina.GetFile("exd/maincommand_0_en.exd");
        if (exd == null) throw new InvalidOperationException("exd/maincommand_0_en.exd non trovato.");

        var existing = LoadExistingJson(targetJsonPath);
        var root = new JsonObject();

        uint idx = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(0x08, 4));
        int count = (int)(idx / 8);
        int fixedSize = 16;
        int extracted = 0;

        for (int i = 0; i < count; i++)
        {
            int entryPos = 0x20 + (i * 8);
            uint rId = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(entryPos, 4));
            uint off = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(entryPos + 4, 4));
            int dataSize = (int)BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan((int)off, 4));

            string name = ReadRowString(exd.Data, (int)off + 6, fixedSize, 0, dataSize);
            string desc = ReadRowString(exd.Data, (int)off + 6, fixedSize, 4, dataSize);

            string key = rId.ToString();
            bool hasExisting = false;
            string transName = string.Empty;
            string transDesc = string.Empty;

            if (existing != null && existing.TryGetPropertyValue(key, out var oldNode) && oldNode is JsonObject oldObj)
            {
                hasExisting = true;
                if (oldObj.TryGetPropertyValue("translation_name", out var tn))
                {
                    transName = tn?.GetValue<string>() ?? string.Empty;
                }
                else if (oldObj.TryGetPropertyValue("translation", out var t))
                {
                    transName = t?.GetValue<string>() ?? string.Empty;
                }

                if (oldObj.TryGetPropertyValue("translation_description", out var td))
                {
                    transDesc = td?.GetValue<string>() ?? string.Empty;
                }
            }

            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(desc) && !hasExisting)
            {
                continue;
            }

            var entryObj = new JsonObject
            {
                ["name"] = name,
                ["translation_name"] = transName,
                ["description"] = desc,
                ["translation_description"] = transDesc
            };

            root[key] = entryObj;
            extracted++;
        }

        SaveJsonObject(targetJsonPath, root);
        return extracted;
    }
}

