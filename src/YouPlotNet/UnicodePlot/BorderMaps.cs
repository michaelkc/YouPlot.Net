namespace YouPlotNet.UnicodePlot;

public sealed record BorderStyle(string Tl, string Tr, string Bl, string Br, string T, string L, string B, string R);

public static class BorderMaps
{
    public static readonly BorderStyle Solid = new("┌", "┐", "└", "┘", "─", "│", "─", "│");
    public static readonly BorderStyle Ascii = new("+", "+", "+", "+", "-", "|", "-", "|");
    public static readonly BorderStyle Corners = new("┌", "┐", "└", "┘", " ", " ", " ", " ");
    public static readonly BorderStyle Barplot = new("┌", "┐", "└", "┘", " ", "┤", " ", " ");

    public static readonly Dictionary<string, BorderStyle> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["solid"] = Solid,
        ["ascii"] = Ascii,
        ["corners"] = Corners,
        ["barplot"] = Barplot,
    };

    public static IEnumerable<string> BorderTypes => Map.Keys;
}
