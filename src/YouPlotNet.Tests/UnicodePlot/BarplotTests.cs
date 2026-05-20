// Port of ruby_source/unicode_plot.rb/test/test-barplot.rb
namespace YouPlotNet.Tests.UnicodePlot;

public class BarplotTests
{
    private static readonly double[] Heights1 = [23, 37];
    private static readonly string[] Labels1 = ["bar", "foo"];

    [Fact]
    public void Errors_NegativeValues()
    {
        Assert.Throws<ArgumentException>(() => UnicodePlotApi.Barplot(["a"], [-1]));
        Assert.Throws<ArgumentException>(() => UnicodePlotApi.Barplot(["a", "b"], [-1, 2]));
    }

    [Fact]
    public void Errors_UnknownBorder()
    {
        Assert.Throws<ArgumentException>(() =>
            UnicodePlotApi.Barplot(Labels1, Heights1, border: "invalid_border_name"));
    }

    [Fact]
    public void Colored()
    {
        var plot = UnicodePlotApi.Barplot(Labels1, Heights1);
        Assert.Equal(Fixture("barplot/default.txt"), RenderColor(plot));

        // positional overload
        var plot2 = UnicodePlotApi.Barplot(Labels1, Heights1);
        Assert.Equal(Fixture("barplot/default.txt"), RenderColor(plot2));
    }

    [Fact]
    public void NotColored()
    {
        var plot = UnicodePlotApi.Barplot(Labels1, Heights1);
        Assert.Equal(Fixture("barplot/nocolor.txt"), RenderNoColor(plot));
    }

    [Fact]
    public void Mixed()
    {
        var labels = new[] { "bar", "2.1", "foo" };
        var heights = new[] { 23.0, 10.0, 37.0 };
        var plot = UnicodePlotApi.Barplot(labels, heights);
        Assert.Equal(Fixture("barplot/default_mixed.txt"), RenderColor(plot));
    }

    [Fact]
    public void Xscale_Log10_Default()
    {
        var labels = new[] { "a", "b", "c", "d", "e" };
        var heights = new[] { 0.0, 1.0, 10.0, 100.0, 1000.0 };
        var plot = UnicodePlotApi.Barplot(labels, heights, title: "Logscale Plot", xscale: "log10");
        Assert.Equal(Fixture("barplot/log10.txt"), RenderColor(plot));
    }

    [Fact]
    public void Xscale_Log10_CustomLabel()
    {
        var labels = new[] { "a", "b", "c", "d", "e" };
        var heights = new[] { 0.0, 1.0, 10.0, 100.0, 1000.0 };
        var plot = UnicodePlotApi.Barplot(labels, heights, title: "Logscale Plot", xlabel: "custom label", xscale: "log10");
        Assert.Equal(Fixture("barplot/log10_label.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters1()
    {
        var labels = new[] { "Paris", "New York", "Moskau", "Madrid" };
        var heights = new[] { 2.244, 8.406, 11.92, 3.165 };
        var plot = UnicodePlotApi.Barplot(labels, heights,
            title: "Relative sizes of cities",
            xlabel: "population [in mil]",
            color: "blue",
            margin: 7,
            padding: 3);
        Assert.Equal(Fixture("barplot/parameters1.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters1_NoLabels()
    {
        var labels = new[] { "Paris", "New York", "Moskau", "Madrid" };
        var heights = new[] { 2.244, 8.406, 11.92, 3.165 };
        var plot = UnicodePlotApi.Barplot(labels, heights,
            title: "Relative sizes of cities",
            xlabel: "population [in mil]",
            color: "blue",
            margin: 7,
            padding: 3,
            labelsVisible: false);
        Assert.Equal(Fixture("barplot/parameters1_nolabels.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters2()
    {
        var labels = new[] { "Paris", "New York", "Moskau", "Madrid" };
        var heights = new[] { 2.244, 8.406, 11.92, 3.165 };
        var plot = UnicodePlotApi.Barplot(labels, heights,
            title: "Relative sizes of cities",
            xlabel: "population [in mil]",
            color: "yellow",
            border: "solid",
            symbol: "=",
            width: 60);
        Assert.Equal(Fixture("barplot/parameters2.txt"), RenderColor(plot));
    }

    [Fact]
    public void Ranges()
    {
        var labels = Enumerable.Range(2, 5).Select(i => i.ToString(CultureInfo.InvariantCulture));
        var heights = Enumerable.Range(11, 5).Select(i => (double)i);
        var plot = UnicodePlotApi.Barplot(labels, heights);
        Assert.Equal(Fixture("barplot/ranges.txt"), RenderColor(plot));
    }

    [Fact]
    public void EdgeCase_AllZeros()
    {
        var labels = new[] { "5", "4", "3", "2", "1" };
        var heights = new[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
        var plot = UnicodePlotApi.Barplot(labels, heights);
        Assert.Equal(Fixture("barplot/edgecase_zeros.txt"), RenderColor(plot));
    }

    [Fact]
    public void EdgeCase_OneLarge()
    {
        var labels = new[] { "a", "b", "c", "d" };
        var heights = new[] { 1.0, 1.0, 1.0, 1000000.0 };
        var plot = UnicodePlotApi.Barplot(labels, heights);
        Assert.Equal(Fixture("barplot/edgecase_onelarge.txt"), RenderColor(plot));
    }

    // ── BarplotAppend ─────────────────────────────────────────────────────────

    [Fact]
    public void BarplotAppend_Errors()
    {
        var plot = UnicodePlotApi.Barplot(Labels1, Heights1);
        Assert.Throws<ArgumentException>(() => UnicodePlotApi.BarplotAppend(plot, ["zoom"], [90, 80]));
        Assert.Throws<ArgumentException>(() => UnicodePlotApi.BarplotAppend(plot, ["zoom", "boom"], [90]));
        // valid append should not throw
        UnicodePlotApi.BarplotAppend(plot, ["zoom"], [90.1]);
    }

    [Fact]
    public void BarplotAppend_ReturnValue()
    {
        var plot = UnicodePlotApi.Barplot(Labels1, Heights1);
        var same = UnicodePlotApi.BarplotAppend(plot, ["zoom"], [90]);
        Assert.Same(plot, same);
        Assert.Equal(Fixture("barplot/default2.txt"), RenderColor(plot));

        var plot2 = UnicodePlotApi.Barplot(Labels1, Heights1);
        UnicodePlotApi.BarplotAppend(plot2, ["zoom"], [90]);
        Assert.Equal(Fixture("barplot/default2.txt"), RenderColor(plot2));
    }

    [Fact]
    public void BarplotAppend_Ranges()
    {
        var labels = Enumerable.Range(2, 5).Select(i => i.ToString(CultureInfo.InvariantCulture));
        var heights = Enumerable.Range(11, 5).Select(i => (double)i);
        var plot = UnicodePlotApi.Barplot(labels, heights);
        UnicodePlotApi.BarplotAppend(plot, Enumerable.Range(9, 2).Select(i => i.ToString(CultureInfo.InvariantCulture)), Enumerable.Range(20, 2).Select(i => (double)i));
        Assert.Equal(Fixture("barplot/ranges2.txt"), RenderColor(plot));
    }
}
