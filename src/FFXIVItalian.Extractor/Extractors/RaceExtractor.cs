using Lumina;
using Lumina.Data;
using System;
using System.Buffers.Binary;
using System.Text.Json.Nodes;

namespace FFXIVItalian.Extractor.Extractors;

/// <summary>
/// Estrae i nomi delle razze dalla scheda exd/race_0_en.exd.
/// Ogni riga ha fixedSize=44 con due colonne stringa:
///   Col 0 (offset 0): nome maschile
///   Col 1 (offset 4): nome femminile
/// </summary>
public sealed class RaceExtractor : BaseSheetExtractor
{
    public override string SheetName => "Race";
    public override string DefaultJsonFileName => "race.json";
    public override string Description => "Nomi delle razze (maschile/femminile) per la schermata di creazione personaggio.";

    public override int ExtractAndSave(GameData lumina, string targetJsonPath)
    {
        var exd = lumina.GetFile("exd/race_0_en.exd");
        if (exd == null) throw new InvalidOperationException("exd/race_0_en.exd non trovato.");

        var existing = LoadExistingJson(targetJsonPath);
        var root = new JsonObject();

        uint idx = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(0x08, 4));
        int count = (int)(idx / 8);
        int fixedSize = 44;
        int extracted = 0;

        for (int i = 0; i < count; i++)
        {
            int entryPos = 0x20 + (i * 8);
            uint rId = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(entryPos, 4));
            uint off = BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan(entryPos + 4, 4));
            int dataSize = (int)BinaryPrimitives.ReadUInt32BigEndian(exd.Data.AsSpan((int)off, 4));

            string nameMasc = ReadRowString(exd.Data, (int)off + 6, fixedSize, 0, dataSize);
            string nameFem  = ReadRowString(exd.Data, (int)off + 6, fixedSize, 4, dataSize);

            string key = rId.ToString();
            bool hasExisting = false;
            string transMasc = string.Empty;
            string transFem  = string.Empty;

            if (existing != null && existing.TryGetPropertyValue(key, out var oldNode) && oldNode is JsonObject oldObj)
            {
                hasExisting = true;
                if (oldObj.TryGetPropertyValue("translation_name_masculine", out var tm))
                    transMasc = tm?.GetValue<string>() ?? string.Empty;
                if (oldObj.TryGetPropertyValue("translation_name_feminine", out var tf))
                    transFem = tf?.GetValue<string>() ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(nameMasc) && string.IsNullOrWhiteSpace(nameFem) && !hasExisting)
                continue;

            var entryObj = new JsonObject
            {
                ["name_masculine"]            = nameMasc,
                ["translation_name_masculine"] = transMasc,
                ["name_feminine"]             = nameFem,
                ["translation_name_feminine"]  = transFem
            };

            root[key] = entryObj;
            extracted++;
        }

        SaveJsonObject(targetJsonPath, root);
        return extracted;
    }
}

