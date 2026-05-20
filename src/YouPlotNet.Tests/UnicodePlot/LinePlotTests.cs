// Port of ruby_source/unicode_plot.rb/test/test-lineplot.rb
// Note: "stairs" tests are omitted (UnicodePlot.stairs is not ported)
namespace YouPlotNet.Tests.UnicodePlot;

public class LinePlotTests
{
    private static readonly double[] X = [-1, 1, 3, 3, -1];
    private static readonly double[] Y = [2, 0, -5, 2, -5];

    [Fact]
    public void Errors_MismatchedLengths()
    {
        Assert.Throws<ArgumentException>(() => UnicodePlotApi.Lineplot([1.0, 2.0], [1.0, 2.0, 3.0]));
        Assert.Throws<ArgumentException>(() => UnicodePlotApi.Lineplot([1.0, 2.0, 3.0], [1.0, 2.0]));
    }

    [Fact]
    public void Errors_UnknownBorder()
    {
        Assert.Throws<ArgumentException>(() =>
            UnicodePlotApi.Lineplot(X, Y, border: "invalid_border_name"));
    }

    [Fact]
    public void Default()
    {
        var plot = UnicodePlotApi.Lineplot(X, Y);
        Assert.Equal(Fixture("lineplot/default.txt"), RenderColor(plot));

        var plot2 = UnicodePlotApi.Lineplot(X.Select(v => (double)v), Y);
        Assert.Equal(Fixture("lineplot/default.txt"), RenderColor(plot2));

        var plot3 = UnicodePlotApi.Lineplot(X, Y.Select(v => (double)v));
        Assert.Equal(Fixture("lineplot/default.txt"), RenderColor(plot3));
    }

    [Fact]
    public void YOnly()
    {
        var plot = UnicodePlotApi.Lineplot(Y);
        Assert.Equal(Fixture("lineplot/y_only.txt"), RenderColor(plot));
    }

    [Fact]
    public void Range1()
    {
        var plot = UnicodePlotApi.Lineplot(Enumerable.Range(6, 5).Select(i => (double)i));
        Assert.Equal(Fixture("lineplot/range1.txt"), RenderColor(plot));
    }

    [Fact]
    public void Range2()
    {
        var plot = UnicodePlotApi.Lineplot(
            Enumerable.Range(11, 5).Select(i => (double)i),
            Enumerable.Range(6, 5).Select(i => (double)i));
        Assert.Equal(Fixture("lineplot/range2.txt"), RenderColor(plot));
    }

    [Fact]
    public void Scale1()
    {
        var plot = UnicodePlotApi.Lineplot(
            X.Select(x => x * 1e3 + 15),
            Y.Select(y => y * 1e-3 - 15));
        Assert.Equal(Fixture("lineplot/scale1.txt"), RenderColor(plot));
    }

    [Fact]
    public void Scale2()
    {
        var plot = UnicodePlotApi.Lineplot(
            X.Select(x => x * 1e-3 + 15),
            Y.Select(y => y * 1e3 - 15));
        Assert.Equal(Fixture("lineplot/scale2.txt"), RenderColor(plot));
    }

    [Fact]
    public void Scale3()
    {
        double[] tx = [-1.0, 2, 3, 700000];
        double[] ty = [1.0, 2, 9, 4000000];
        var plot = UnicodePlotApi.Lineplot(tx, ty);
        Assert.Equal(Fixture("lineplot/scale3.txt"), RenderColor(plot));
    }

    [Fact]
    public void Scale3_Small()
    {
        double[] tx = [-1.0, 2, 3, 700000];
        double[] ty = [1.0, 2, 9, 4000000];
        var plot = UnicodePlotApi.Lineplot(tx, ty, width: 5, height: 5);
        Assert.Equal(Fixture("lineplot/scale3_small.txt"), RenderColor(plot));
    }

    [Fact]
    public void Limits()
    {
        var plot = UnicodePlotApi.Lineplot(X, Y, xlim: [-1.5, 3.5], ylim: [-5.5, 2.5]);
        Assert.Equal(Fixture("lineplot/limits.txt"), RenderColor(plot));
    }

    [Fact]
    public void NoGrid()
    {
        var plot = UnicodePlotApi.Lineplot(X, Y, grid: false);
        Assert.Equal(Fixture("lineplot/nogrid.txt"), RenderColor(plot));
    }

    [Fact]
    public void Blue()
    {
        var plot = UnicodePlotApi.Lineplot(X, Y, color: "blue", name: "points1");
        Assert.Equal(Fixture("lineplot/blue.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters1()
    {
        var plot = UnicodePlotApi.Lineplot(X, Y, name: "points1", title: "Scatter", xlabel: "x", ylabel: "y");
        Assert.Equal(Fixture("lineplot/parameters1.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters2_Append()
    {
        var plot = UnicodePlotApi.Lineplot(X, Y, name: "points1", title: "Scatter", xlabel: "x", ylabel: "y");
        var same = UnicodePlotApi.LineplotAppend(plot, [0.5, 1.0, 1.5], name: "points2");
        Assert.Same(plot, same);
        Assert.Equal(Fixture("lineplot/parameters2.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters3_AppendXY()
    {
        var plot = UnicodePlotApi.Lineplot(X, Y, name: "points1", title: "Scatter", xlabel: "x", ylabel: "y");
        UnicodePlotApi.LineplotAppend(plot, [0.5, 1.0, 1.5], name: "points2");
        var same = UnicodePlotApi.LineplotAppend(plot, [-0.5, 0.5, 1.5], [0.5, 1.0, 1.5], name: "points3");
        Assert.Same(plot, same);
        Assert.Equal(Fixture("lineplot/parameters3.txt"), RenderColor(plot));
        // nocolor variant
        Assert.Equal(Fixture("lineplot/nocolor.txt"), RenderNoColor(plot));
    }

    [Fact]
    public void CanvasSize()
    {
        var plot = UnicodePlotApi.Lineplot(X, Y, title: "Scatter", canvas: "dot", width: 10, height: 5);
        Assert.Equal(Fixture("lineplot/canvassize.txt"), RenderColor(plot));
    }

    [Fact]
    public void Issue32_FixedLineYInterpolation()
    {
        double[] ys = [261, 272, 277, 283, 289, 294, 298, 305, 309, 314, 319, 320, 322, 323, 324];
        double[] xs = Enumerable.Range(0, ys.Length).Select(i => (double)i).ToArray();
        var plot = UnicodePlotApi.Lineplot(xs, ys, height: 26, ylim: [0, 700]);
        Assert.Equal(Fixture("lineplot/issue32_fix.txt"), RenderColor(plot));
    }

    [Fact]
    public void Slope1()
    {
        var plot = UnicodePlotApi.Lineplot(Y);
        var same = UnicodePlotApi.LineplotAppend(plot, [-3.0], [1.0]);
        Assert.Same(plot, same);
        Assert.Equal(Fixture("lineplot/slope1.txt"), RenderColor(plot));
    }

    [Fact]
    public void Slope2()
    {
        var plot = UnicodePlotApi.Lineplot(Y);
        UnicodePlotApi.LineplotAppend(plot, [-3.0], [1.0]);
        var same = UnicodePlotApi.LineplotAppend(plot, [-4.0], [0.5], color: "cyan", name: "foo");
        Assert.Same(plot, same);
        Assert.Equal(Fixture("lineplot/slope2.txt"), RenderColor(plot));
    }
}
