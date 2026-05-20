namespace YouPlotNet.UnicodePlot;

public sealed class Scatterplot : GridPlot
{
    public Scatterplot(IEnumerable<double> x, IEnumerable<double> y, string canvas = "braille", int width = DefaultWidth, int height = DefaultHeight, IReadOnlyList<double>? xlim = null, IReadOnlyList<double>? ylim = null, bool grid = true, string? title = null, string? xlabel = null, string? ylabel = null, string border = DefaultBorder, int margin = DefaultMargin, int padding = DefaultPadding, bool labels = true)
        : base(x, y, canvas, width, height, xlim, ylim, grid, title, xlabel, ylabel, border, margin, padding, labels)
    {
    }
}
