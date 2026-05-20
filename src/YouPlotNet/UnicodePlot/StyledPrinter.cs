namespace YouPlotNet.UnicodePlot;

public static class StyledPrinter
{
    public static readonly Dictionary<string, string> TextColors = new(StringComparer.OrdinalIgnoreCase)
    {
        ["black"] = "\x1b[30m",
        ["red"] = "\x1b[31m",
        ["green"] = "\x1b[32m",
        ["yellow"] = "\x1b[33m",
        ["blue"] = "\x1b[34m",
        ["magenta"] = "\x1b[35m",
        ["cyan"] = "\x1b[36m",
        ["white"] = "\x1b[37m",
        ["gray"] = "\x1b[90m",
        ["light_black"] = "\x1b[90m",
        ["light_red"] = "\x1b[91m",
        ["light_green"] = "\x1b[92m",
        ["light_yellow"] = "\x1b[93m",
        ["light_blue"] = "\x1b[94m",
        ["light_magenta"] = "\x1b[95m",
        ["light_cyan"] = "\x1b[96m",
        ["normal"] = "\x1b[0m",
        ["default"] = "\x1b[39m",
        ["bold"] = "\x1b[1m",
        ["underline"] = "\x1b[4m",
        ["blink"] = "\x1b[5m",
        ["reverse"] = "\x1b[7m",
        ["hidden"] = "\x1b[8m",
        ["nothing"] = string.Empty,
    };

    public static readonly Dictionary<string, string> DisableTextStyle = new(StringComparer.OrdinalIgnoreCase)
    {
        ["bold"] = "\x1b[22m",
        ["underline"] = "\x1b[24m",
        ["blink"] = "\x1b[25m",
        ["reverse"] = "\x1b[27m",
        ["hidden"] = "\x1b[28m",
        ["normal"] = string.Empty,
        ["default"] = string.Empty,
        ["nothing"] = string.Empty,
    };

    public static readonly Dictionary<string, int> ColorEncode = new(StringComparer.OrdinalIgnoreCase)
    {
        ["normal"] = 0b000,
        ["blue"] = 0b001,
        ["red"] = 0b010,
        ["magenta"] = 0b011,
        ["green"] = 0b100,
        ["cyan"] = 0b101,
        ["yellow"] = 0b110,
        ["white"] = 0b111,
        ["gray"] = 0b000,
        ["light_black"] = 0b000,
    };

    public static readonly Dictionary<int, string> ColorDecode;

    static StyledPrinter()
    {
        for (var i = 0; i <= 255; i++)
        {
            TextColors[i.ToString(CultureInfo.InvariantCulture)] = $"\x1b[38;5;{i}m";
        }

        ColorDecode = ColorEncode
            .GroupBy(static kv => kv.Value)
            .ToDictionary(static g => g.Key, static g => g.First().Key);
    }

    public static void PrintStyled(IOContext output, bool bold = false, string color = "normal", params object?[] args)
    {
        var text = string.Concat(args.Select(static a => a?.ToString() ?? string.Empty));
        if (!output.IsColor)
        {
            output.Print(text);
            return;
        }

        if (bold && color.Equals("bold", StringComparison.OrdinalIgnoreCase))
        {
            color = "nothing";
        }

        var enableAnsi = GetColorSequence(color) + (bold ? TextColors["bold"] : string.Empty);
        var disableAnsi = (bold ? DisableTextStyle["bold"] : string.Empty)
            + (DisableTextStyle.TryGetValue(color, out var disable) ? disable : TextColors["default"]);

        var builder = new StringBuilder();
        foreach (var line in SplitKeepingNewlines(text))
        {
            if (line.Length == 0)
            {
                continue;
            }

            builder.Append(enableAnsi).Append(line).Append(disableAnsi);
        }

        output.Print(builder.ToString());
    }

    public static void PrintColor(IOContext output, int encodedColor, params object?[] args)
    {
        var color = ColorDecode.TryGetValue(encodedColor, out var decoded) ? decoded : "normal";
        PrintStyled(output, color: color, args: args);
    }

    public static string StripAnsi(string text) => Regex.Replace(text, "\\x1b\\[[0-9;]*m", string.Empty);

    private static string GetColorSequence(string color) => TextColors.TryGetValue(color, out var value) ? value : TextColors["default"];

    private static IEnumerable<string> SplitKeepingNewlines(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            yield break;
        }

        var start = 0;
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] != '\n')
            {
                continue;
            }

            yield return text.Substring(start, i - start + 1);
            start = i + 1;
        }

        if (start < text.Length)
        {
            yield return text[start..];
        }
    }
}
