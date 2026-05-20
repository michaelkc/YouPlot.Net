// Port of ruby_source/unicode_plot.rb/test/test-scatterplot.rb
namespace YouPlotNet.Tests.UnicodePlot;

public class ScatterplotTests
{
    private static readonly double[] X = [-1, 1, 3, 3, -1];
    private static readonly double[] Y = [2, 0, -5, 2, -5];

    [Fact]
    public void Errors_MismatchedLengths()
    {
        Assert.Throws<ArgumentException>(() => UnicodePlotApi.Scatterplot([1.0, 2.0], [1.0, 2.0, 3.0]));
        Assert.Throws<ArgumentException>(() => UnicodePlotApi.Scatterplot([1.0, 2.0, 3.0], [1.0, 2.0]));
    }

    [Fact]
    public void Errors_UnknownBorder()
    {
        Assert.Throws<ArgumentException>(() =>
            UnicodePlotApi.Scatterplot(X, Y, border: "invalid_border_name"));
    }

    [Fact]
    public void Default()
    {
        var plot = UnicodePlotApi.Scatterplot(X, Y);
        Assert.Equal(Fixture("scatterplot/default.txt"), RenderColor(plot));
    }

    [Fact]
    public void YOnly()
    {
        var plot = UnicodePlotApi.Scatterplot(Y);
        Assert.Equal(Fixture("scatterplot/y_only.txt"), RenderColor(plot));
    }

    [Fact]
    public void Range1()
    {
        var plot = UnicodePlotApi.Scatterplot(Enumerable.Range(6, 5).Select(i => (double)i));
        Assert.Equal(Fixture("scatterplot/range1.txt"), RenderColor(plot));
    }

    [Fact]
    public void Range2()
    {
        var plot = UnicodePlotApi.Scatterplot(
            Enumerable.Range(11, 5).Select(i => (double)i),
            Enumerable.Range(6, 5).Select(i => (double)i));
        Assert.Equal(Fixture("scatterplot/range2.txt"), RenderColor(plot));
    }

    [Fact]
    public void Scale1()
    {
        var plot = UnicodePlotApi.Scatterplot(
            X.Select(x => x * 1e3 + 15),
            Y.Select(y => y * 1e-3 - 15));
        Assert.Equal(Fixture("scatterplot/scale1.txt"), RenderColor(plot));
    }

    [Fact]
    public void Scale2()
    {
        var plot = UnicodePlotApi.Scatterplot(
            X.Select(x => x * 1e-3 + 15),
            Y.Select(y => y * 1e3 - 15));
        Assert.Equal(Fixture("scatterplot/scale2.txt"), RenderColor(plot));
    }

    [Fact]
    public void Scale3()
    {
        double miny = -1.2796649117521434e218;
        double maxy = -miny;
        var plot = UnicodePlotApi.Scatterplot([1.0], [miny], xlim: [1.0, 1.0], ylim: [miny, maxy]);
        Assert.Equal(Fixture("scatterplot/scale3.txt"), RenderColor(plot));
    }

    [Fact]
    public void Limits()
    {
        var plot = UnicodePlotApi.Scatterplot(X, Y, xlim: [-1.5, 3.5], ylim: [-5.5, 2.5]);
        Assert.Equal(Fixture("scatterplot/limits.txt"), RenderColor(plot));
    }

    [Fact]
    public void NoGrid()
    {
        var plot = UnicodePlotApi.Scatterplot(X, Y, grid: false);
        Assert.Equal(Fixture("scatterplot/nogrid.txt"), RenderColor(plot));
    }

    [Fact]
    public void Blue()
    {
        var plot = UnicodePlotApi.Scatterplot(X, Y, color: "blue", name: "points1");
        Assert.Equal(Fixture("scatterplot/blue.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters1()
    {
        var plot = UnicodePlotApi.Scatterplot(X, Y, name: "points1", title: "Scatter", xlabel: "x", ylabel: "y");
        Assert.Equal(Fixture("scatterplot/parameters1.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters2_Append()
    {
        var plot = UnicodePlotApi.Scatterplot(X, Y, name: "points1", title: "Scatter", xlabel: "x", ylabel: "y");
        var same = UnicodePlotApi.ScatterplotAppend(plot, [0.5, 1.0, 1.5], name: "points2");
        Assert.Same(plot, same);
        Assert.Equal(Fixture("scatterplot/parameters2.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters3_AppendXY_NoColor()
    {
        var plot = UnicodePlotApi.Scatterplot(X, Y, name: "points1", title: "Scatter", xlabel: "x", ylabel: "y");
        UnicodePlotApi.ScatterplotAppend(plot, [0.5, 1.0, 1.5], name: "points2");
        var same = UnicodePlotApi.ScatterplotAppend(plot, [-0.5, 0.5, 1.5], [0.5, 1.0, 1.5], name: "points3");
        Assert.Same(plot, same);
        Assert.Equal(Fixture("scatterplot/parameters3.txt"), RenderColor(plot));
        Assert.Equal(Fixture("scatterplot/nocolor.txt"), RenderNoColor(plot));
    }

    [Fact]
    public void CanvasSize()
    {
        var plot = UnicodePlotApi.Scatterplot(X, Y, title: "Scatter", canvas: "dot", width: 10, height: 5);
        Assert.Equal(Fixture("scatterplot/canvassize.txt"), RenderColor(plot));
    }
}
