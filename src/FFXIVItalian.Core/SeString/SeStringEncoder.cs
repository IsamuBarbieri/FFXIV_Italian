using Lumina.Text;
using Lumina.Text.Payloads;
using System.Text;
using System.Text.RegularExpressions;

namespace FFXIVItalian.Core.SeString;

public static partial class SeStringEncoder
{
    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex TagRegex();

    [GeneratedRegex(@"^<string\(lstr(\d+)\)>$", RegexOptions.IgnoreCase)]
    private static partial Regex LstrRegex();

    [GeneratedRegex(@"^<string\(gstr(\d+)\)>$", RegexOptions.IgnoreCase)]
    private static partial Regex GstrRegex();

    [GeneratedRegex(@"^<colortype\((\d+)\)>$", RegexOptions.IgnoreCase)]
    private static partial Regex ColorTypeRegex();

    [GeneratedRegex(@"^<edgecolortype\((\d+)\)>$", RegexOptions.IgnoreCase)]
    private static partial Regex EdgeColorTypeRegex();

    [GeneratedRegex(@"^<hex:([0-9a-fA-F]+)>$", RegexOptions.IgnoreCase)]
    private static partial Regex HexRegex();

    /// <summary>
    /// Encodes a text containing optional macro tags into raw FFXIV SeString binary bytes.
    /// Supports tags such as:
    /// - &lt;string(lstr1)&gt;, &lt;string(lstr2)&gt;
    /// - &lt;string(gstr1)&gt;
    /// - &lt;br&gt;
    /// - &lt;colortype(N)&gt; (where 0 pops color)
    /// - &lt;edgecolortype(N)&gt; (where 0 pops edge color)
    /// - &lt;hex:AABBCC...&gt;
    /// </summary>
    public static byte[] Encode(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return [];
        }

        if (!input.Contains('<'))
        {
            return Encoding.UTF8.GetBytes(input);
        }

        using var ms = new MemoryStream();
        int lastIndex = 0;

        foreach (Match match in TagRegex().Matches(input))
        {
            // Append plain text before this tag
            if (match.Index > lastIndex)
            {
                string textSegment = input[lastIndex..match.Index];
                ms.Write(Encoding.UTF8.GetBytes(textSegment));
            }

            string tag = match.Value;

            if (tag.Equals("<br>", StringComparison.OrdinalIgnoreCase))
            {
                ms.Write([0x02, 0x10, 0x01, 0x03]);
            }
            else if (LstrRegex().Match(tag) is { Success: true } lstrMatch &&
                     int.TryParse(lstrMatch.Groups[1].Value, out int lstrId))
            {
                byte exprByte = (byte)(0xE9 + lstrId);
                ms.Write([0x02, 0x29, 0x03, exprByte, 0x02, 0x03]);
            }
            else if (HexRegex().Match(tag) is { Success: true } hexMatch)
            {
                byte[] rawHex = Convert.FromHexString(hexMatch.Groups[1].Value);
                ms.Write(rawHex);
            }
            else if (ColorTypeRegex().Match(tag) is { Success: true } colorMatch &&
                     uint.TryParse(colorMatch.Groups[1].Value, out uint colorId))
            {
                var sb = new SeStringBuilder();
                if (colorId == 0)
                {
                    sb.PopColorType();
                }
                else
                {
                    sb.PushColorType(colorId);
                }
                ms.Write(sb.ToArray());
            }
            else if (EdgeColorTypeRegex().Match(tag) is { Success: true } edgeMatch &&
                     uint.TryParse(edgeMatch.Groups[1].Value, out uint edgeId))
            {
                var sb = new SeStringBuilder();
                if (edgeId == 0)
                {
                    sb.PopEdgeColorType();
                }
                else
                {
                    sb.PushEdgeColorType(edgeId);
                }
                ms.Write(sb.ToArray());
            }
            else if (GstrRegex().Match(tag) is { Success: true } gstrMatch &&
                     int.TryParse(gstrMatch.Groups[1].Value, out int gstrId))
            {
                var sb = new SeStringBuilder();
                sb.BeginMacro(MacroCode.String);
                sb.AppendGlobalStringExpression(gstrId);
                sb.EndMacro();
                ms.Write(sb.ToArray());
            }
            else
            {
                // Unrecognized tag: treat as literal text
                ms.Write(Encoding.UTF8.GetBytes(tag));
            }

            lastIndex = match.Index + match.Length;
        }

        // Append any remaining text after the last tag
        if (lastIndex < input.Length)
        {
            ms.Write(Encoding.UTF8.GetBytes(input[lastIndex..]));
        }

        return ms.ToArray();
    }
}
