namespace YouPlotNet.YouPlot;

public sealed class Options
{
    public string Delimiter { get; set; } = "\t";
    public bool Transpose { get; set; }
    public bool Headers { get; set; }
    public object? Pass { get; set; } = false;
    public object Output { get; set; } = Console.Error;
    public string Fmt { get; set; } = "xyy";
    public bool Progressive { get; set; }
    public string? Encoding { get; set; }
    public bool Reverse { get; set; }
    public bool ColorNames { get; set; }
    public bool Debug { get; set; }
    public bool? ForceColor { get; set; }
}
