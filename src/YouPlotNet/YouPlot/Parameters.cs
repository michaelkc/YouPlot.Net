namespace YouPlotNet.YouPlot;

public sealed class Parameters
{
    public string? Title { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? Border { get; set; }
    public int? Margin { get; set; }
    public int? Padding { get; set; }
    public string? Color { get; set; }
    public string? Xlabel { get; set; }
    public string? Ylabel { get; set; }
    public bool? Labels { get; set; }
    public string? Symbol { get; set; }
    public string? Xscale { get; set; }
    public int? Nbins { get; set; }
    public string? Closed { get; set; }
    public string? Canvas { get; set; }
    public double[]? Xlim { get; set; }
    public double[]? Ylim { get; set; }
    public bool? Grid { get; set; }
    public string? Name { get; set; }

    public Dictionary<string, object> ToDict()
    {
        var d = new Dictionary<string, object>();
        if (Title != null) d["title"] = Title;
        if (Width.HasValue) d["width"] = Width.Value;
        if (Height.HasValue) d["height"] = Height.Value;
        if (Border != null) d["border"] = Border;
        if (Margin.HasValue) d["margin"] = Margin.Value;
        if (Padding.HasValue) d["padding"] = Padding.Value;
        if (Color != null) d["color"] = Color;
        if (Xlabel != null) d["xlabel"] = Xlabel;
        if (Ylabel != null) d["ylabel"] = Ylabel;
        if (Labels.HasValue) d["labels"] = Labels.Value;
        if (Symbol != null) d["symbol"] = Symbol;
        if (Xscale != null) d["xscale"] = Xscale;
        if (Nbins.HasValue) d["nbins"] = Nbins.Value;
        if (Closed != null) d["closed"] = Closed;
        if (Canvas != null) d["canvas"] = Canvas;
        if (Xlim != null) d["xlim"] = Xlim;
        if (Ylim != null) d["ylim"] = Ylim;
        if (Grid.HasValue) d["grid"] = Grid.Value;
        if (Name != null) d["name"] = Name;
        return d;
    }
}
