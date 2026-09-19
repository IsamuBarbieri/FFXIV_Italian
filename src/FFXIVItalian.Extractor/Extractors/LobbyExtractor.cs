using Lumina;
using Lumina.Data;
using System;
using System.Buffers.Binary;
using System.Text.Json.Nodes;

namespace FFXIVItalian.Extractor.Extractors;

public sealed class LobbyExtractor : BaseSheetExtractor
{
    public override string SheetName => "Lobby";
    public override string DefaultJsonFileName => "lobby.json";
    public override string Description => "Testi della Schermata del Titolo, Connessione, Creazione e Selezione Personaggio.";

    public override int ExtractAndSave(GameData lumina, string targetJsonPath)
    {
        var exd = lumina.GetFile("exd/lobby_0_en.exd");
        if (exd == null) throw new InvalidOperationException("exd/lobby_0_en.exd non trovato.");

        var existing = LoadExistingJson(targetJsonPath);
        var root = new JsonObject();

        uint idx = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(0x08, 4));
        int count = (int)(idx / 8);
        int fixedSize = 24;
        int extracted = 0;

        for (int i = 0; i < count; i++)
        {
            int entryPos = 0x20 + (i * 8);
            uint rId = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(entryPos, 4));
            uint off = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(entryPos + 4, 4));
            int dataSize = (int)BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan((int)off, 4));

            string text = ReadRowString(exd.Data, (int)off + 6, fixedSize, 0, dataSize);
            string desc = ReadRowString(exd.Data, (int)off + 6, fixedSize, 8, dataSize);

            if (string.IsNullOrWhiteSpace(text) && string.IsNullOrWhiteSpace(desc)) continue;

            string key = rId.ToString();
            string translation = string.Empty;
            string transDesc = string.Empty;

            if (existing != null && existing.TryGetPropertyValue(key, out var oldNode) && oldNode is JsonObject oldObj)
            {
                if (oldObj.TryGetPropertyValue("translation_name", out var tNameNode))
                {
                    translation = tNameNode?.GetValue<string>() ?? string.Empty;
                }
                else if (oldObj.TryGetPropertyValue("translation", out var transNode))
                {
                    translation = transNode?.GetValue<string>() ?? string.Empty;
                }

                if (oldObj.TryGetPropertyValue("translation_description", out var tDescNode))
                {
                    transDesc = tDescNode?.GetValue<string>() ?? string.Empty;
                }
            }

            JsonObject entryObj;
            if (!string.IsNullOrWhiteSpace(desc))
            {
                entryObj = new JsonObject
                {
                    ["name"] = text,
                    ["translation_name"] = translation,
                    ["description"] = desc,
                    ["translation_description"] = transDesc
                };
            }
            else
            {
                entryObj = new JsonObject
                {
                    ["original"] = text,
                    ["translation"] = translation
                };
            }

            root[key] = entryObj;
            extracted++;
        }

        SaveJsonObject(targetJsonPath, root);
        return extracted;
    }
}

