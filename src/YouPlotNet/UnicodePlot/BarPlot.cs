namespace YouPlotNet.UnicodePlot;

public sealed class BarPlot : Plot
{
    public const int MinWidth = 10;
    public const string DefaultColor = "green";
    public const string DefaultSymbol = "■";

    private readonly List<double> _bars;
    private readonly List<string>? _valueLabels;
    private readonly string _color;
    private readonly string _symbol;
    private readonly string? _transform;

    public BarPlot(IEnumerable<double> bars, int width, string color, string symbol, string? transform, IEnumerable<string>? valueLabels = null, string? title = null, string? xlabel = null, string? ylabel = null, string border = "barplot", int margin = DefaultMargin, int padding = DefaultPadding, bool labels = true)
        : base(title, xlabel, ylabel, border, margin, padding, labels)
    {
        if (symbol.Length > 1)
        {
            throw new ArgumentException("symbol must be a single character");
        }

        _bars = bars.ToList();
        _valueLabels = valueLabels?.ToList();
        if (_valueLabels is { Count: > 0 } && _valueLabels.Count != _bars.Count)
        {
            throw new ArgumentException("value labels must match bars");
        }

        _color = color;
        _symbol = symbol;
        _transform = transform;
        Recalculate(width);
    }

    public double MaxFreq { get; private set; }
    public int MaxLen { get; private set; }
    public int WidthValue { get; private set; }
    public override int NRows => _bars.Count;
    public override int NColumns => WidthValue;

    public void AddRow(IEnumerable<double> bars)
    {
        _bars.AddRange(bars);
        Recalculate(WidthValue);
    }

    public override void PrintRow(IOContext output, int rowIndex)
    {
        var bar = _bars[rowIndex];
        var maxBarWidth = Math.Max(WidthValue - 2 - MaxLen, 1);
        var value = ValueTransformer.TransformValue(_transform, bar);
        var barLen = MaxFreq > 0 ? (int)Math.Round(Math.Max(value, 0d) / MaxFreq * maxBarWidth, MidpointRounding.AwayFromZero) : 0;
        var barString = MaxFreq > 0 ? string.Concat(Enumerable.Repeat(_symbol, barLen)) : string.Empty;
        var label = GetLabel(rowIndex);
        StyledPrinter.PrintStyled(output, false, _color, barString);
        StyledPrinter.PrintStyled(output, false, "normal", " ", label);
        var padLen = Math.Max(maxBarWidth + 1 + MaxLen - barLen - label.Length, 0);
        output.Print(new string(' ', padLen));
    }

    private string GetLabel(int rowIndex) => _valueLabels?[rowIndex] ?? _bars[rowIndex].ToString("G", CultureInfo.InvariantCulture);

    private void Recalculate(int width)
    {
        var transformed = ValueTransformer.TransformValues(_transform, _bars);
        if (transformed.Length == 0)
        {
            MaxFreq = 0;
            MaxLen = 0;
        }
        else
        {
            var maxIndex = 0;
            var maxFreq = transformed[0];
            for (var i = 1; i < transformed.Length; i++)
            {
                if (transformed[i] <= maxFreq)
                {
                    continue;
                }

                maxFreq = transformed[i];
                maxIndex = i;
            }

            MaxFreq = maxFreq;
            MaxLen = GetLabel(maxIndex).Length;
        }

        WidthValue = Math.Max(Math.Max(width, MaxLen + 7), MinWidth);
    }
}
