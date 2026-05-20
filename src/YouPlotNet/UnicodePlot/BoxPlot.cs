namespace YouPlotNet.UnicodePlot;

public sealed class BoxPlot : Plot
{
    public const int MinWidth = 10;
    public const string DefaultColor = "green";

    private readonly List<double[]> _data = [];
    private readonly string _color;
    private readonly int _width;

    public BoxPlot(IEnumerable<double> data, int width, string color, double minX, double maxX, string? title = null, string? xlabel = null, string? ylabel = null, string border = "corners", int margin = DefaultMargin, int padding = DefaultPadding, bool labels = true)
        : base(title, xlabel, ylabel, border, margin, padding, labels)
    {
        _width = Math.Max(width, MinWidth);
        _color = color;
        MinX = minX == maxX ? minX - 1 : minX;
        MaxX = minX == maxX ? maxX + 1 : maxX;
        _data.Add(Percentiles(data.ToArray()));
    }

    public double MinX { get; private set; }
    public double MaxX { get; private set; }
    public int NData => _data.Count;
    public override int NRows => _data.Count * 3;
    public override int NColumns => _width;

    public void AddSeries(IEnumerable<double> data)
    {
        var values = data.ToArray();
        _data.Add(Percentiles(values));
        MinX = Math.Min(MinX, values.Min());
        MaxX = Math.Max(MaxX, values.Max());
    }

    public override void PrintRow(IOContext output, int rowIndex)
    {
        var series = _data[rowIndex / 3];
        var seriesRow = rowIndex % 3;
        var minChar = new[] { '╷', '├', '╵' }[seriesRow];
        var lineChar = new[] { ' ', '─', ' ' }[seriesRow];
        var leftBoxChar = new[] { '┌', '┤', '└' }[seriesRow];
        var lineBoxChar = new[] { '─', ' ', '─' }[seriesRow];
        var medianChar = new[] { '┬', '│', '┴' }[seriesRow];
        var rightBoxChar = new[] { '┐', '├', '┘' }[seriesRow];
        var maxChar = new[] { '╷', '┤', '╵' }[seriesRow];
        var line = Enumerable.Repeat(' ', _width).ToArray();
        var transformed = Transform(series);
        line[transformed[0] - 1] = minChar;
        line[transformed[1] - 1] = leftBoxChar;
        line[transformed[2] - 1] = medianChar;
        line[transformed[3] - 1] = rightBoxChar;
        line[transformed[4] - 1] = maxChar;
        for (var i = transformed[0]; i < transformed[1] - 1; i++) line[i] = lineChar;
        for (var i = transformed[1]; i < transformed[2] - 1; i++) line[i] = lineBoxChar;
        for (var i = transformed[2]; i < transformed[3] - 1; i++) line[i] = lineBoxChar;
        for (var i = transformed[3]; i < transformed[4] - 1; i++) line[i] = lineChar;
        StyledPrinter.PrintStyled(output, false, _color, new string(line));
    }

    private int[] Transform(IEnumerable<double> values) => values
        .Select(value => (int)Math.Clamp(Math.Round((value - MinX) / (MaxX - MinX) * _width, MidpointRounding.ToEven), 1, _width))
        .ToArray();

    private static double[] Percentiles(double[] values)
    {
        Array.Sort(values);
        return [Percentile(values, 0), Percentile(values, 25), Percentile(values, 50), Percentile(values, 75), Percentile(values, 100)];
    }

    private static double Percentile(double[] sorted, double percentile)
    {
        if (sorted.Length == 0) throw new ArgumentException("empty data");
        var rank = percentile / 100d * (sorted.Length - 1);
        var lo = (int)Math.Floor(rank);
        var hi = (int)Math.Ceiling(rank);
        if (lo == hi) return sorted[lo];
        return sorted[lo] + ((rank - lo) * (sorted[hi] - sorted[lo]));
    }
}
