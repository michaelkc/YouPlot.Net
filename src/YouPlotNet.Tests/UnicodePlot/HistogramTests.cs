// Port of ruby_source/unicode_plot.rb/test/test-histogram.rb
namespace YouPlotNet.Tests.UnicodePlot;

public class HistogramTests
{
    private static double[] LoadRandn() =>
        File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "Fixtures", "randn.txt"))
            .Select(l => double.Parse(l.Trim(), CultureInfo.InvariantCulture))
            .ToArray();

    [Fact]
    public void Errors_UnknownBorder()
    {
        var x = LoadRandn();
        Assert.Throws<ArgumentException>(() =>
            UnicodePlotApi.HistogramPlot(x, border: "invalid_border_name"));
    }

    [Fact]
    public void Default()
    {
        var x = LoadRandn();
        var plot = UnicodePlotApi.HistogramPlot(x);
        // Ruby: render($stdout) then check output[-1] == "\n" and compare output.chomp
        var output = RenderColorWithNewline(plot);
        Assert.Equal("\n", output[^1..]);
        Assert.Equal(Fixture("histogram/default.txt"), output.TrimEnd('\n'));
    }

    [Fact]
    public void Nocolor()
    {
        var x = LoadRandn();
        var plot = UnicodePlotApi.HistogramPlot(x);
        var output = RenderNoColorWithNewline(plot);
        Assert.Equal("\n", output[^1..]);
        Assert.Equal(Fixture("histogram/default_nocolor.txt"), output.TrimEnd('\n'));
    }

    [Fact]
    public void Closed_Left()
    {
        var x = LoadRandn();
        var plot = UnicodePlotApi.HistogramPlot(x, closed: "left");
        Assert.Equal(Fixture("histogram/default.txt"), RenderColor(plot));
    }

    [Fact]
    public void Scale_1e2()
    {
        var x = LoadRandn().Select(a => a * 100).ToArray();
        var plot = UnicodePlotApi.HistogramPlot(x);
        Assert.Equal(Fixture("histogram/default_1e2.txt"), RenderColor(plot));
    }

    [Fact]
    public void Scale_1e_Minus2()
    {
        var x = LoadRandn().Select(a => a * 0.01).ToArray();
        var plot = UnicodePlotApi.HistogramPlot(x);
        Assert.Equal(Fixture("histogram/default_1e-2.txt"), RenderColor(plot));
    }

    [Fact]
    public void Xscale_Log10()
    {
        var x = LoadRandn();
        var plot = UnicodePlotApi.HistogramPlot(x, xscale: "log10");
        Assert.Equal(Fixture("histogram/log10.txt"), RenderColor(plot));
    }

    [Fact]
    public void Xscale_Log10_CustomLabel()
    {
        var x = LoadRandn();
        var plot = UnicodePlotApi.HistogramPlot(x, xscale: "log10", xlabel: "custom label");
        Assert.Equal(Fixture("histogram/log10_label.txt"), RenderColor(plot));
    }

    [Fact]
    public void Nbins5_ClosedRight()
    {
        var x = LoadRandn();
        var plot = UnicodePlotApi.HistogramPlot(x, nbins: 5, closed: "right");
        Assert.Equal(Fixture("histogram/hist_params.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters1_WithTitleXlabelColorMarginPadding()
    {
        var x = LoadRandn();
        var plot = UnicodePlotApi.HistogramPlot(x,
            title: "My Histogram",
            xlabel: "Absolute Frequency",
            color: "blue",
            margin: 7,
            padding: 3);
        Assert.Equal(Fixture("histogram/parameters1.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters1_NoLabels()
    {
        var x = LoadRandn();
        var plot = UnicodePlotApi.HistogramPlot(x,
            title: "My Histogram",
            xlabel: "Absolute Frequency",
            color: "blue",
            margin: 7,
            padding: 3,
            labelsVisible: false);
        Assert.Equal(Fixture("histogram/parameters1_nolabels.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters2_WithBorderSymbolWidth()
    {
        var x = LoadRandn();
        var plot = UnicodePlotApi.HistogramPlot(x,
            title: "My Histogram",
            xlabel: "Absolute Frequency",
            color: "yellow",
            border: "solid",
            symbol: "=",
            width: 50);
        Assert.Equal(Fixture("histogram/parameters2.txt"), RenderColor(plot));
    }

    [Fact]
    public void Issue24_TwoElementArray_DoesNotThrow()
    {
        // should not throw
        UnicodePlotApi.HistogramPlot([1.0, 2.0]);
    }
}
