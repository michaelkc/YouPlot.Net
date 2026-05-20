namespace YouPlotNet.UnicodePlot;

public sealed class Densityplot : GridPlot
{
    public Densityplot(IEnumerable<double> x, IEnumerable<double> y, int width = DefaultWidth, int height = DefaultHeight, IReadOnlyList<double>? xlim = null, IReadOnlyList<double>? ylim = null, bool grid = false, string? title = null, string? xlabel = null, string? ylabel = null, string border = DefaultBorder, int margin = DefaultMargin, int padding = DefaultPadding, bool labels = true)
        : base(x, y, "density", width, height, xlim, ylim, grid, title, xlabel, ylabel, border, margin, padding, labels)
    {
    }
}
