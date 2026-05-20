// Port of ruby_source/unicode_plot.rb/test/test-densityplot.rb
namespace YouPlotNet.Tests.UnicodePlot;

public class DensityplotTests
{
    private static (double[] dx, double[] dy) GetData()
    {
        var lines = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "Fixtures", "randn_1338_2000.txt"));
        var randn = lines.Select(l => double.Parse(l.Trim(), CultureInfo.InvariantCulture)).ToArray();
        return (randn[..1000], randn[1000..2000]);
    }

    [Fact]
    public void Errors_UnknownBorder()
    {
        var (dx, dy) = GetData();
        Assert.Throws<ArgumentException>(() =>
            UnicodePlotApi.Densityplot(dx, dy, border: "invalid_border_name"));
    }

    [Fact]
    public void Default()
    {
        var (dx, dy) = GetData();
        var plot = UnicodePlotApi.Densityplot(dx, dy);
        var dx2 = dx.Select(x => x + 2.0).ToArray();
        var dy2 = dy.Select(y => y + 2.0).ToArray();
        var same = UnicodePlotApi.DensityplotAppend(plot, dx2, dy2);
        Assert.Same(plot, same);
        Assert.Equal(Fixture("scatterplot/densityplot.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters()
    {
        var (dx, dy) = GetData();
        var plot = UnicodePlotApi.Densityplot(dx, dy, name: "foo", color: "red", title: "Title", xlabel: "x");
        var dx2 = dx.Select(x => x + 2.0).ToArray();
        var dy2 = dy.Select(y => y + 2.0).ToArray();
        var same = UnicodePlotApi.DensityplotAppend(plot, dx2, dy2, name: "bar");
        Assert.Same(plot, same);
        Assert.Equal(Fixture("scatterplot/densityplot_parameters.txt"), RenderColor(plot));
    }
}
