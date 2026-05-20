namespace YouPlotNet.UnicodePlot;

public interface IRenderable
{
    void Render(TextWriter writer, bool newline = true, bool? color = null);
}

public abstract class Plot : IRenderable
{
    public const int DefaultWidth = 40;
    public const string DefaultBorder = "solid";
    public const int DefaultMargin = 3;
    public const int DefaultPadding = 1;

    private int _autoColor;

    protected Plot(string? title = null, string? xlabel = null, string? ylabel = null, string border = DefaultBorder, int margin = DefaultMargin, int padding = DefaultPadding, bool labels = true)
    {
        Title = title;
        Xlabel = xlabel;
        Ylabel = ylabel;
        Border = BorderMaps.Map.ContainsKey(border) ? border : throw new ArgumentException($"unknown border type: {border}");
        Margin = margin >= 0 ? margin : throw new ArgumentException("margin must be >= 0");
        Padding = padding;
        ShowLabels = labels;
    }

    public string? Title { get; set; }
    public string? Xlabel { get; set; }
    public string? Ylabel { get; set; }
    public string Border { get; set; }
    public int Margin { get; set; }
    public int Padding { get; set; }
    public bool ShowLabels { get; set; }
    public Dictionary<int, string> LabelsLeft { get; } = new();
    public Dictionary<int, string> ColorsLeft { get; } = new();
    public Dictionary<int, string> LabelsRight { get; } = new();
    public Dictionary<int, string> ColorsRight { get; } = new();
    public Dictionary<string, string> Decorations { get; } = new();
    public Dictionary<string, string> ColorsDeco { get; } = new();
    public static readonly string[] ColorCycle = ["green", "blue", "red", "magenta", "yellow", "cyan"];

    public bool TitleGiven => !string.IsNullOrEmpty(Title);
    public bool XlabelGiven => !string.IsNullOrEmpty(Xlabel);
    public bool YlabelGiven => !string.IsNullOrEmpty(Ylabel);
    public int YlabelLength => Ylabel?.Length ?? 0;

    public string NextColor()
    {
        var color = ColorCycle[_autoColor];
        _autoColor = (_autoColor + 1) % ColorCycle.Length;
        return color;
    }

    public abstract int NRows { get; }
    public abstract int NColumns { get; }
    public abstract void PrintRow(IOContext output, int rowIndex);

    public void Render(TextWriter writer, bool newline = true, bool? color = null) => Renderer.Render(new IOContext(writer, color), this, newline);

    public override string ToString()
    {
        using var writer = new StringWriter(CultureInfo.InvariantCulture);
        Render(writer, false, false);
        return writer.ToString();
    }

    public void AnnotateRow(string loc, int rowIndex, string value, string color = "normal")
    {
        switch (loc)
        {
            case "l": LabelsLeft[rowIndex] = value; ColorsLeft[rowIndex] = color; break;
            case "r": LabelsRight[rowIndex] = value; ColorsRight[rowIndex] = color; break;
            default: throw new ArgumentException($"unknown location `{loc}`, try :l or :r instead");
        }
    }

    public void Annotate(string loc, string value, string color = "normal")
    {
        switch (loc)
        {
            case "l":
                for (var row = 0; row < NRows; row++)
                {
                    if (!LabelsLeft.TryGetValue(row, out var s) || string.IsNullOrEmpty(s))
                    {
                        LabelsLeft[row] = value;
                        ColorsLeft[row] = color;
                        return;
                    }
                }
                break;
            case "r":
                for (var row = 0; row < NRows; row++)
                {
                    if (!LabelsRight.TryGetValue(row, out var s) || string.IsNullOrEmpty(s))
                    {
                        LabelsRight[row] = value;
                        ColorsRight[row] = color;
                        return;
                    }
                }
                break;
            case "t": case "b": case "tl": case "tr": case "bl": case "br":
                Decorations[loc] = value;
                ColorsDeco[loc] = color;
                break;
            default:
                throw new ArgumentException($"unknown location to annotate ({loc})");
        }
    }
}
