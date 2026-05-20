using System.Text.RegularExpressions;
using YouPlotNet.UnicodePlot;

namespace YouPlotNet.YouPlot.Backends;

public static class UnicodePlotBackend
{
    public static IRenderable Barplot(Data data, Parameters parameters, string? fmt = null, bool count = false, bool reverse = false)
    {
        var headers = data.Headers;
        var series = data.Series.Select(static item => item.ToList()).ToList();
        if (count)
        {
            var counted = Processing.CountValues(series[0], reverse);
            headers ??= data.Headers;
            if (headers is { Length: > 0 }) parameters.Title ??= headers[0];
            var values = counted[1].Select(static value => double.Parse(value!, CultureInfo.InvariantCulture)).ToArray();
            var valueLabels = values.Select(static value => value.ToString("0.0###############", CultureInfo.InvariantCulture)).ToArray();
            return UnicodePlotApi.Barplot(counted[0].Select(static value => value ?? string.Empty), values, parameters.Color ?? BarPlot.DefaultColor, parameters.Symbol ?? BarPlot.DefaultSymbol, parameters.Border ?? "barplot", parameters.Xscale, parameters.Title, parameters.Xlabel, parameters.Ylabel, parameters.Labels ?? true, parameters.Width ?? Plot.DefaultWidth, parameters.Margin ?? Plot.DefaultMargin, parameters.Padding ?? Plot.DefaultPadding, valueLabels);
        }

        if (series.Count == 1)
        {
            if (headers is { Length: > 0 }) parameters.Title ??= headers[0];
            var labels = Enumerable.Range(1, series[0].Count).Select(static i => i.ToString(CultureInfo.InvariantCulture));
            var values = series[0].Select(ParseDouble).ToArray();
            var valueLabels = series[0].Select(static value => value ?? string.Empty).ToArray();
            return UnicodePlotApi.Barplot(labels, values, parameters.Color ?? BarPlot.DefaultColor, parameters.Symbol ?? BarPlot.DefaultSymbol, parameters.Border ?? "barplot", parameters.Xscale, parameters.Title, parameters.Xlabel, parameters.Ylabel, parameters.Labels ?? true, parameters.Width ?? Plot.DefaultWidth, parameters.Margin ?? Plot.DefaultMargin, parameters.Padding ?? Plot.DefaultPadding, valueLabels);
        }

        var xCol = fmt == "yx" ? 1 : 0;
        var yCol = fmt == "yx" ? 0 : 1;
        if (headers is { Length: > 0 }) parameters.Title ??= headers[yCol];
        var barValues = series[yCol].Select(ParseDouble).ToArray();
        var barValueLabels = series[yCol].Select(static value => value ?? string.Empty).ToArray();
        return UnicodePlotApi.Barplot(series[xCol].Select(static value => value ?? string.Empty), barValues, parameters.Color ?? BarPlot.DefaultColor, parameters.Symbol ?? BarPlot.DefaultSymbol, parameters.Border ?? "barplot", parameters.Xscale, parameters.Title, parameters.Xlabel, parameters.Ylabel, parameters.Labels ?? true, parameters.Width ?? Plot.DefaultWidth, parameters.Margin ?? Plot.DefaultMargin, parameters.Padding ?? Plot.DefaultPadding, barValueLabels);
    }

    public static IRenderable Histogram(Data data, Parameters parameters)
    {
        if (data.Headers is { Length: > 0 }) parameters.Title ??= data.Headers[0];
        return UnicodePlotApi.HistogramPlot(data.Series[0].Select(ParseDouble), parameters.Nbins, parameters.Closed ?? "left", parameters.Symbol ?? "▇", parameters.Xscale, parameters.Title, parameters.Xlabel, parameters.Ylabel, parameters.Labels ?? true, parameters.Border ?? "barplot", parameters.Color ?? BarPlot.DefaultColor, parameters.Width ?? Plot.DefaultWidth, parameters.Margin ?? Plot.DefaultMargin, parameters.Padding ?? Plot.DefaultPadding);
    }

    public static IRenderable Line(Data data, Parameters parameters, string? fmt = null)
    {
        var headers = data.Headers;
        var series = data.Series;
        if (series.Count == 1)
        {
            if (headers is { Length: > 0 }) parameters.Ylabel ??= headers[0];
            return UnicodePlotApi.Lineplot(series[0].Select(ParseDouble), parameters.Canvas ?? "braille", parameters.Color ?? "auto", parameters.Name ?? string.Empty, parameters.Title, parameters.Xlabel, parameters.Ylabel, parameters.Labels ?? true, parameters.Border ?? Plot.DefaultBorder, parameters.Margin ?? Plot.DefaultMargin, parameters.Padding ?? Plot.DefaultPadding, parameters.Width ?? Plot.DefaultWidth, parameters.Height ?? GridPlot.DefaultHeight, parameters.Xlim, parameters.Ylim, parameters.Grid ?? true);
        }

        var xCol = fmt == "yx" ? 1 : 0;
        var yCol = fmt == "yx" ? 0 : 1;
        if (headers is { Length: > 0 })
        {
            parameters.Xlabel ??= headers[xCol];
            parameters.Ylabel ??= headers[yCol];
        }

        return UnicodePlotApi.Lineplot(series[xCol].Select(ParseDouble), series[yCol].Select(ParseDouble), parameters.Canvas ?? "braille", parameters.Color ?? "auto", parameters.Name ?? string.Empty, parameters.Title, parameters.Xlabel, parameters.Ylabel, parameters.Labels ?? true, parameters.Border ?? Plot.DefaultBorder, parameters.Margin ?? Plot.DefaultMargin, parameters.Padding ?? Plot.DefaultPadding, parameters.Width ?? Plot.DefaultWidth, parameters.Height ?? GridPlot.DefaultHeight, parameters.Xlim, parameters.Ylim, parameters.Grid ?? true);
    }

    public static IRenderable Lines(Data data, Parameters parameters, string fmt = "xyy") => PlotFmt(data, fmt, parameters, isLine: true, isDensity: false);
    public static IRenderable Scatter(Data data, Parameters parameters, string fmt = "xyy") => PlotFmt(data, fmt, parameters, isLine: false, isDensity: false);
    public static IRenderable Density(Data data, Parameters parameters, string fmt = "xyy") => PlotFmt(data, fmt, parameters, isLine: false, isDensity: true);

    public static IRenderable Boxplot(Data data, Parameters parameters)
    {
        var headers = data.Headers?.Select(static h => h ?? string.Empty).ToArray() ?? Enumerable.Range(1, data.Series.Count).Select(static i => i.ToString(CultureInfo.InvariantCulture)).ToArray();
        return UnicodePlotApi.Boxplot(headers, data.Series.Select(static s => s.Select(ParseDouble)), parameters.Title, parameters.Xlabel, parameters.Ylabel, parameters.Border ?? "corners", parameters.Color ?? BoxPlot.DefaultColor, parameters.Width ?? Plot.DefaultWidth, parameters.Xlim, parameters.Margin ?? Plot.DefaultMargin, parameters.Padding ?? Plot.DefaultPadding, parameters.Labels ?? true);
    }

    public static IRenderable Colors(bool colorNames = false) => UnicodePlotApi.Colors(colorNames);

    private static IRenderable PlotFmt(Data data, string fmt, Parameters parameters, bool isLine, bool isDensity)
    {
        if (data.Series.Count == 1) throw new InvalidOperationException("There is only one series of input data. Please check the delimiter.");
        if (fmt == "xyxy" && data.Series.Count % 2 != 0) throw new InvalidOperationException("In the xyxy format, the number of series must be even.");
        if (fmt == "xyxy")
        {
            var groups = data.Series.Select(static s => s.Select(ParseDouble).ToArray()).Chunk(2).ToArray();
            var plot = CreatePlot(groups[0][0], groups[0][1], parameters, isLine, isDensity, data.Headers?.ElementAtOrDefault(0));
            for (var i = 1; i < groups.Length; i++) AppendPlot(plot, groups[i][0], groups[i][1], data.Headers?.ElementAtOrDefault(i * 2), isLine, isDensity);
            return plot;
        }
        else
        {
            var x = data.Series[0].Select(ParseDouble).ToArray();
            var ys = data.Series.Skip(1).Select(static s => s.Select(ParseDouble).ToArray()).ToArray();
            var plot = CreatePlot(x, ys[0], parameters, isLine, isDensity, data.Headers?.ElementAtOrDefault(1));
            for (var i = 1; i < ys.Length; i++) AppendPlot(plot, x, ys[i], data.Headers?.ElementAtOrDefault(i + 1), isLine, isDensity);
            return plot;
        }
    }

    private static Plot CreatePlot(double[] x, double[] y, Parameters p, bool isLine, bool isDensity, string? name)
    {
        p.Name ??= name;
        return isDensity
            ? UnicodePlotApi.Densityplot(x, y, p.Color ?? "auto", p.Name ?? string.Empty, p.Title, p.Xlabel, p.Ylabel, p.Labels ?? true, p.Border ?? Plot.DefaultBorder, p.Margin ?? Plot.DefaultMargin, p.Padding ?? Plot.DefaultPadding, p.Width ?? Plot.DefaultWidth, p.Height ?? GridPlot.DefaultHeight, p.Xlim ?? [x.Min(), x.Max()], p.Ylim ?? [y.Min(), y.Max()], p.Grid ?? false)
            : isLine
                ? UnicodePlotApi.Lineplot(x, y, p.Canvas ?? "braille", p.Color ?? "auto", p.Name ?? string.Empty, p.Title, p.Xlabel, p.Ylabel, p.Labels ?? true, p.Border ?? Plot.DefaultBorder, p.Margin ?? Plot.DefaultMargin, p.Padding ?? Plot.DefaultPadding, p.Width ?? Plot.DefaultWidth, p.Height ?? GridPlot.DefaultHeight, p.Xlim ?? [x.Min(), x.Max()], p.Ylim ?? [y.Min(), y.Max()], p.Grid ?? true)
                : UnicodePlotApi.Scatterplot(x, y, p.Canvas ?? "braille", p.Color ?? "auto", p.Name ?? string.Empty, p.Title, p.Xlabel, p.Ylabel, p.Labels ?? true, p.Border ?? Plot.DefaultBorder, p.Margin ?? Plot.DefaultMargin, p.Padding ?? Plot.DefaultPadding, p.Width ?? Plot.DefaultWidth, p.Height ?? GridPlot.DefaultHeight, p.Xlim ?? [x.Min(), x.Max()], p.Ylim ?? [y.Min(), y.Max()], p.Grid ?? true);
    }

    private static void AppendPlot(Plot plot, double[] x, double[] y, string? name, bool isLine, bool isDensity)
    {
        switch (plot)
        {
            case Lineplot linePlot when isLine:
                UnicodePlotApi.LineplotAppend(linePlot, x, y, name: name ?? string.Empty);
                break;
            case Scatterplot scatterPlot when !isDensity:
                UnicodePlotApi.ScatterplotAppend(scatterPlot, x, y, name: name ?? string.Empty);
                break;
            case Densityplot densityPlot:
                UnicodePlotApi.DensityplotAppend(densityPlot, x, y, name: name ?? string.Empty);
                break;
        }
    }

    private static readonly Regex LeadingFloatPattern = new(@"^[\s]*[+-]?(?:(?:\d+\.?\d*)|(?:\.\d+))(?:[eE][+-]?\d+)?", RegexOptions.Compiled);

    private static double ParseDouble(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0d;
        }

        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        var match = LeadingFloatPattern.Match(value);
        return match.Success && double.TryParse(match.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out result)
            ? result
            : 0d;
    }
}
