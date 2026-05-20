using PlotCanvas = YouPlotNet.UnicodePlot.Canvas.Canvas;

namespace YouPlotNet.UnicodePlot;

public class GridPlot : Plot
{
    public const int MinWidth = 5;
    public const int MinHeight = 2;
    public const int DefaultHeight = 15;

    public GridPlot(IEnumerable<double> x, IEnumerable<double> y, string canvas, int width = DefaultWidth, int height = DefaultHeight, IReadOnlyList<double>? xlim = null, IReadOnlyList<double>? ylim = null, bool grid = true, string? title = null, string? xlabel = null, string? ylabel = null, string border = DefaultBorder, int margin = DefaultMargin, int padding = DefaultPadding, bool labels = true)
        : base(title, xlabel, ylabel, border, margin, padding, labels)
    {
        var xs = x.ToArray();
        var ys = y.ToArray();
        if (xs.Length != ys.Length)
        {
            throw new ArgumentException("x and y must be the same length");
        }

        if (xs.Length == 0)
        {
            throw new ArgumentException("x and y must not be empty");
        }

        xlim ??= [0d, 0d];
        ylim ??= [0d, 0d];
        if (xlim.Count != 2 || ylim.Count != 2)
        {
            throw new ArgumentException("xlim and ylim must be 2-length arrays");
        }

        width = Math.Max(width, MinWidth);
        height = Math.Max(height, MinHeight);
        var (minX, maxX) = Utils.ExtendLimits(xs, xlim);
        var (minY, maxY) = Utils.ExtendLimits(ys, ylim);
        Canvas = PlotCanvas.Create(canvas, width, height, minX, minY, maxX - minX, maxY - minY);
        AnnotateRow("l", 0, Utils.FormatNumber(maxY), "light_black");
        AnnotateRow("l", height - 1, Utils.FormatNumber(minY), "light_black");
        Annotate("bl", Utils.FormatNumber(minX), "light_black");
        Annotate("br", Utils.FormatNumber(maxX), "light_black");

        if (grid)
        {
            if (minY < 0 && 0 < maxY)
            {
                var count = width * Canvas.XPixelPerChar - 1;
                var step = count > 0 ? (maxX - minX) / count : maxX - minX;
                for (var value = minX; value <= maxX + (step / 2d); value += step == 0 ? maxX - minX + 1 : step)
                {
                    Canvas.Point(value, 0, "normal");
                    if (step == 0) break;
                }
            }

            if (minX < 0 && 0 < maxX)
            {
                var count = height * Canvas.YPixelPerChar - 1;
                var step = count > 0 ? (maxY - minY) / count : maxY - minY;
                for (var value = minY; value <= maxY + (step / 2d); value += step == 0 ? maxY - minY + 1 : step)
                {
                    Canvas.Point(0, value, "normal");
                    if (step == 0) break;
                }
            }
        }
    }

    public PlotCanvas Canvas { get; }
    public double OriginX => Canvas.OriginX;
    public double OriginY => Canvas.OriginY;
    public double PlotWidth => Canvas.PlotWidth;
    public double PlotHeight => Canvas.PlotHeight;
    public override int NRows => Canvas.Height;
    public override int NColumns => Canvas.Width;
    public void Points(IEnumerable<double> x, IEnumerable<double> y, string color) => Canvas.Points(x, y, color);
    public void Lines(IEnumerable<double> x, IEnumerable<double> y, string color) => Canvas.Lines(x, y, color);
    public override void PrintRow(IOContext output, int rowIndex) => Canvas.PrintRow(output, rowIndex);
}
