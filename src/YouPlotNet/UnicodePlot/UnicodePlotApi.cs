namespace YouPlotNet.UnicodePlot;

public sealed class ColorsOutput : IRenderable
{
    private readonly string _text;

    public ColorsOutput(string text)
    {
        _text = text;
    }

    public void Render(TextWriter writer, bool newline = true, bool? color = null)
    {
        var text = color == false ? StyledPrinter.StripAnsi(_text) : _text;
        writer.Write(text);
        if (newline && !text.EndsWith(Environment.NewLine, StringComparison.Ordinal) && !text.EndsWith("\n", StringComparison.Ordinal))
        {
            writer.WriteLine();
        }
    }

    public override string ToString() => _text;
}

public static class UnicodePlotApi
{
    public static BarPlot Barplot(IEnumerable<string> labels, IEnumerable<double> heights, string color = BarPlot.DefaultColor, string symbol = BarPlot.DefaultSymbol, string border = "barplot", string? xscale = null, string? title = null, string? xlabel = null, string? ylabel = null, bool labelsVisible = true, int width = Plot.DefaultWidth, int margin = Plot.DefaultMargin, int padding = Plot.DefaultPadding, IEnumerable<string>? valueLabels = null)
    {
        var labelArray = labels.Select(static value => value.ToString()).ToArray();
        var heightArray = heights.ToArray();
        var valueLabelArray = valueLabels?.ToArray();
        if (labelArray.Length != heightArray.Length)
        {
            throw new ArgumentException("The given vectors must be of the same length");
        }

        if (valueLabelArray is { Length: > 0 } && valueLabelArray.Length != heightArray.Length)
        {
            throw new ArgumentException("value labels must match heights");
        }

        if (heightArray.Min() < 0)
        {
            throw new ArgumentException("All values have to be positive. Negative bars are not supported.");
        }

        xlabel ??= ValueTransformer.TransformName(xscale);
        var plot = new BarPlot(heightArray, width, color, symbol, xscale, valueLabelArray, title, xlabel, ylabel, border, margin, padding, labelsVisible);
        for (var i = 0; i < labelArray.Length; i++)
        {
            plot.AnnotateRow("l", i, labelArray[i]);
        }

        return plot;
    }

    public static BarPlot BarplotAppend(BarPlot plot, IEnumerable<string> labels, IEnumerable<double> heights)
    {
        var labelArray = labels.Select(static value => value.ToString()).ToArray();
        var heightArray = heights.ToArray();
        if (labelArray.Length != heightArray.Length)
        {
            throw new ArgumentException("The given vectors must be of the same length");
        }

        if (labelArray.Length == 0)
        {
            throw new ArgumentException("Can't append empty array to barplot");
        }

        var offset = plot.NRows;
        plot.AddRow(heightArray);
        for (var i = 0; i < labelArray.Length; i++)
        {
            plot.AnnotateRow("l", offset + i, labelArray[i]);
        }

        return plot;
    }

    public static BarPlot HistogramPlot(IEnumerable<double> values, int? nbins = null, string closed = "left", string symbol = "▇", string? xscale = null, string? title = null, string? xlabel = null, string? ylabel = null, bool labelsVisible = true, string border = "barplot", string color = BarPlot.DefaultColor, int width = Plot.DefaultWidth, int margin = Plot.DefaultMargin, int padding = Plot.DefaultPadding)
    {
        var data = values.ToArray();
        var (edges, counts, intervalSide) = Histogram.ComputeHistogram(data, nbins, closed);
        var labels = Histogram.BuildLabels(edges, counts, intervalSide);
        xlabel ??= ValueTransformer.TransformName(xscale, "Frequency");
        return Barplot(labels, counts.Select(static c => (double)c), color, symbol, border, xscale, title, xlabel, ylabel, labelsVisible, width, margin, padding);
    }

    public static Lineplot Lineplot(IEnumerable<double> y, string canvas = "braille", string color = "auto", string name = "", string? title = null, string? xlabel = null, string? ylabel = null, bool labelsVisible = true, string border = Plot.DefaultBorder, int margin = Plot.DefaultMargin, int padding = Plot.DefaultPadding, int width = Plot.DefaultWidth, int height = GridPlot.DefaultHeight, IReadOnlyList<double>? xlim = null, IReadOnlyList<double>? ylim = null, bool grid = true)
        => Lineplot(Enumerable.Range(1, y.Count()).Select(static i => (double)i), y, canvas, color, name, title, xlabel, ylabel, labelsVisible, border, margin, padding, width, height, xlim, ylim, grid);

    public static Lineplot Lineplot(IEnumerable<double> x, IEnumerable<double> y, string canvas = "braille", string color = "auto", string name = "", string? title = null, string? xlabel = null, string? ylabel = null, bool labelsVisible = true, string border = Plot.DefaultBorder, int margin = Plot.DefaultMargin, int padding = Plot.DefaultPadding, int width = Plot.DefaultWidth, int height = GridPlot.DefaultHeight, IReadOnlyList<double>? xlim = null, IReadOnlyList<double>? ylim = null, bool grid = true)
    {
        var xs = x.ToArray();
        var ys = y.ToArray();
        var plot = new Lineplot(xs, ys, canvas, width, height, xlim, ylim, grid, title, xlabel, ylabel, border, margin, padding, labelsVisible);
        return LineplotAppend(plot, xs, ys, color, name);
    }

    public static Lineplot LineplotAppend(Lineplot plot, IEnumerable<double> y, string color = "auto", string name = "")
        => LineplotAppend(plot, Enumerable.Range(1, y.Count()).Select(static i => (double)i), y, color, name);

    public static Lineplot LineplotAppend(Lineplot plot, IEnumerable<double> x, IEnumerable<double> y, string color = "auto", string name = "")
    {
        var xs = x.ToArray();
        var ys = y.ToArray();
        if (xs.Length == 1 && ys.Length == 1)
        {
            var intercept = xs[0];
            var slope = ys[0];
            xs = [plot.OriginX, plot.OriginX + plot.PlotWidth];
            ys = [intercept + (xs[0] * slope), intercept + (xs[1] * slope)];
        }

        var actualColor = color == "auto" ? plot.NextColor() : color;
        if (!string.IsNullOrEmpty(name)) plot.Annotate("r", name, actualColor);
        plot.Lines(xs, ys, actualColor);
        return plot;
    }

    public static Scatterplot Scatterplot(IEnumerable<double> y, string canvas = "braille", string color = "auto", string name = "", string? title = null, string? xlabel = null, string? ylabel = null, bool labelsVisible = true, string border = Plot.DefaultBorder, int margin = Plot.DefaultMargin, int padding = Plot.DefaultPadding, int width = Plot.DefaultWidth, int height = GridPlot.DefaultHeight, IReadOnlyList<double>? xlim = null, IReadOnlyList<double>? ylim = null, bool grid = true)
        => Scatterplot(Enumerable.Range(1, y.Count()).Select(static i => (double)i), y, canvas, color, name, title, xlabel, ylabel, labelsVisible, border, margin, padding, width, height, xlim, ylim, grid);

    public static Scatterplot Scatterplot(IEnumerable<double> x, IEnumerable<double> y, string canvas = "braille", string color = "auto", string name = "", string? title = null, string? xlabel = null, string? ylabel = null, bool labelsVisible = true, string border = Plot.DefaultBorder, int margin = Plot.DefaultMargin, int padding = Plot.DefaultPadding, int width = Plot.DefaultWidth, int height = GridPlot.DefaultHeight, IReadOnlyList<double>? xlim = null, IReadOnlyList<double>? ylim = null, bool grid = true)
    {
        var xs = x.ToArray();
        var ys = y.ToArray();
        var plot = new Scatterplot(xs, ys, canvas, width, height, xlim, ylim, grid, title, xlabel, ylabel, border, margin, padding, labelsVisible);
        return ScatterplotAppend(plot, xs, ys, color, name);
    }

    public static Scatterplot ScatterplotAppend(Scatterplot plot, IEnumerable<double> y, string color = "auto", string name = "")
        => ScatterplotAppend(plot, Enumerable.Range(1, y.Count()).Select(static i => (double)i), y, color, name);

    public static Scatterplot ScatterplotAppend(Scatterplot plot, IEnumerable<double> x, IEnumerable<double> y, string color = "auto", string name = "")
    {
        var actualColor = color == "auto" ? plot.NextColor() : color;
        if (!string.IsNullOrEmpty(name)) plot.Annotate("r", name, actualColor);
        plot.Points(x, y, actualColor);
        return plot;
    }

    public static Densityplot Densityplot(IEnumerable<double> x, IEnumerable<double> y, string color = "auto", string name = "", string? title = null, string? xlabel = null, string? ylabel = null, bool labelsVisible = true, string border = Plot.DefaultBorder, int margin = Plot.DefaultMargin, int padding = Plot.DefaultPadding, int width = Plot.DefaultWidth, int height = GridPlot.DefaultHeight, IReadOnlyList<double>? xlim = null, IReadOnlyList<double>? ylim = null, bool grid = false)
    {
        var xs = x.ToArray();
        var ys = y.ToArray();
        var plot = new Densityplot(xs, ys, width, height, xlim, ylim, grid, title, xlabel, ylabel, border, margin, padding, labelsVisible);
        return DensityplotAppend(plot, xs, ys, color, name);
    }

    public static Densityplot DensityplotAppend(Densityplot plot, IEnumerable<double> x, IEnumerable<double> y, string color = "auto", string name = "")
    {
        var actualColor = color == "auto" ? plot.NextColor() : color;
        if (!string.IsNullOrEmpty(name)) plot.Annotate("r", name, actualColor);
        plot.Points(x, y, actualColor);
        return plot;
    }

    public static BoxPlot Boxplot(IEnumerable<double> data, string? title = null, string? xlabel = null, string? ylabel = null, string border = "corners", string color = BoxPlot.DefaultColor, int width = Plot.DefaultWidth, IReadOnlyList<double>? xlim = null, int margin = Plot.DefaultMargin, int padding = Plot.DefaultPadding, bool labelsVisible = true)
        => Boxplot(Array.Empty<string>(), [data.ToArray()], title, xlabel, ylabel, border, color, width, xlim, margin, padding, labelsVisible);

    public static BoxPlot Boxplot(IEnumerable<string> names, IEnumerable<IEnumerable<double>> series, string? title = null, string? xlabel = null, string? ylabel = null, string border = "corners", string color = BoxPlot.DefaultColor, int width = Plot.DefaultWidth, IReadOnlyList<double>? xlim = null, int margin = Plot.DefaultMargin, int padding = Plot.DefaultPadding, bool labelsVisible = true)
    {
        var data = series.Select(static item => item.ToArray()).ToArray();
        var labelArray = names.ToArray();
        if (labelArray.Length == 0) labelArray = Enumerable.Repeat(string.Empty, data.Length).ToArray();
        if (labelArray.Length != data.Length) throw new ArgumentException("wrong number of text");
        xlim ??= [0d, 0d];
        if (xlim.Count != 2) throw new ArgumentException("xlim must be a length 2 array");
        var flattened = data.SelectMany(static values => values).ToArray();
        var (minX, maxX) = Utils.ExtendLimits(flattened, xlim);
        var plot = new BoxPlot(data[0], width, color, minX, maxX, title, xlabel, ylabel, border, margin, padding, labelsVisible);
        for (var i = 1; i < data.Length; i++) plot.AddSeries(data[i]);
        AnnotateBoxplotAxis(plot);
        for (var i = 0; i < labelArray.Length; i++) if (!string.IsNullOrEmpty(labelArray[i])) plot.AnnotateRow("l", (i * 3) + 1, labelArray[i]);
        return plot;
    }

    public static BoxPlot BoxplotAppend(BoxPlot plot, IEnumerable<double> data, string name = "")
    {
        var values = data.ToArray();
        if (values.Length == 0) throw new ArgumentException("Can't append empty array to boxplot");
        plot.AddSeries(values);
        if (!string.IsNullOrEmpty(name)) plot.AnnotateRow("l", ((plot.NData - 1) * 3) + 1, name);
        AnnotateBoxplotAxis(plot);
        return plot;
    }

    public static ColorsOutput Colors(bool colorNames = false)
    {
        var builder = new StringBuilder();
        foreach (var (key, value) in StyledPrinter.TextColors)
        {
            builder.Append(value).Append(key);
            if (!colorNames) builder.Append('\t').Append("  ●");
            builder.Append("\x1b[0m\t");
        }

        builder.Append('\n');
        return new ColorsOutput(builder.ToString());
    }

    private static void AnnotateBoxplotAxis(BoxPlot plot)
    {
        var meanX = (plot.MinX + plot.MaxX) / 2d;
        plot.Annotate("bl", Utils.FormatNumber(plot.MinX), "light_black");
        plot.Annotate("b", Utils.FormatNumber(meanX), "light_black");
        plot.Annotate("br", Utils.FormatNumber(plot.MaxX), "light_black");
    }
}
