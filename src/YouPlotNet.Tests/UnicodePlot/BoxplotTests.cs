// Port of ruby_source/unicode_plot.rb/test/test-boxplot.rb
namespace YouPlotNet.Tests.UnicodePlot;

public class BoxplotTests
{
    [Fact]
    public void Errors_UnknownBorder()
    {
        Assert.Throws<ArgumentException>(() =>
            UnicodePlotApi.Boxplot([1.0, 2.0, 3.0, 4.0, 5.0], border: "invalid_border_name"));
    }

    [Fact]
    public void Default_WithoutName()
    {
        var plot = UnicodePlotApi.Boxplot([1.0, 2.0, 3.0, 4.0, 5.0]);
        Assert.Equal(Fixture("boxplot/default.txt"), RenderColor(plot));
    }

    [Fact]
    public void Default_WithName()
    {
        var plot = UnicodePlotApi.Boxplot(["series1"], [[1.0, 2.0, 3.0, 4.0, 5.0]]);
        Assert.Equal(Fixture("boxplot/default_name.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters_ColorToTty()
    {
        var plot = UnicodePlotApi.Boxplot(["series1"], [[1.0, 2.0, 3.0, 4.0, 5.0]],
            title: "Test", xlim: [-1.0, 8.0], color: "blue", width: 50, border: "solid", xlabel: "foo");
        Assert.Equal(Fixture("boxplot/default_parameters.txt"), RenderColor(plot));
    }

    [Fact]
    public void Parameters_ColorToNonTty()
    {
        var plot = UnicodePlotApi.Boxplot(["series1"], [[1.0, 2.0, 3.0, 4.0, 5.0]],
            title: "Test", xlim: [-1.0, 8.0], color: "blue", width: 50, border: "solid", xlabel: "foo");
        Assert.Equal(Fixture("boxplot/default_parameters_nocolor.txt"), RenderNoColor(plot));
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 6)]
    [InlineData(3, 10)]
    [InlineData(4, 20)]
    [InlineData(5, 40)]
    public void Scaling(int scaleIndex, double maxX)
    {
        var plot = UnicodePlotApi.Boxplot([1.0, 2.0, 3.0, 4.0, 5.0], xlim: [0.0, maxX]);
        Assert.Equal(Fixture($"boxplot/scale{scaleIndex}.txt"), RenderColor(plot));
    }

    [Fact]
    public void MultiSeries()
    {
        var plot = UnicodePlotApi.Boxplot(
            ["one", "two"],
            [[1.0, 2.0, 3.0, 4.0, 5.0], [2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0]],
            title: "Multi-series",
            xlabel: "foo",
            color: "yellow");
        Assert.Equal(Fixture("boxplot/multi1.txt"), RenderColor(plot));

        var same = UnicodePlotApi.BoxplotAppend(plot, [-1.0, 2.0, 3.0, 4.0, 11.0], name: "one more");
        Assert.Same(plot, same);
        Assert.Equal(Fixture("boxplot/multi2.txt"), RenderColor(plot));

        var same2 = UnicodePlotApi.BoxplotAppend(plot, [4.0, 2.0, 2.5, 4.0, 14.0], name: "last one");
        Assert.Same(plot, same2);
        Assert.Equal(Fixture("boxplot/multi3.txt"), RenderColor(plot));
    }
}
