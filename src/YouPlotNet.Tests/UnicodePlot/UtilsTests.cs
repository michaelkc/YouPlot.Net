// Port of ruby_source/unicode_plot.rb/test/test-utils.rb  
// and canvas_types / border_types from test-utils.rb
namespace YouPlotNet.Tests.UnicodePlot;

public class UtilsTests
{
    [Fact]
    public void CanvasTypes_AllExpected()
    {
        var expected = new[] { "ascii", "block", "braille", "density", "dot" };
        foreach (var type in expected)
            Assert.NotNull(Canvas.Create(type, 10, 5, 0, 0, 1, 1));
    }

    [Fact]
    public void BorderTypes_AllExpected()
    {
        var expected = new[] { "solid", "ascii", "corners", "barplot" };
        foreach (var type in expected)
            Assert.True(BorderMaps.Map.ContainsKey(type), $"Missing border type: {type}");
    }

    [Fact]
    public void RoundToDecimalPlaces_PositiveDecimals()
    {
        Assert.Equal(1.23, Utils.RoundToDecimalPlaces(1.234, 2), 10);
        Assert.Equal(0.0, Utils.RoundToDecimalPlaces(0.0, 2), 10);
    }

    [Fact]
    public void RoundToDecimalPlaces_NegativeDecimals()
    {
        Assert.Equal(130.0, Utils.RoundToDecimalPlaces(134.9, -1), 5);
        Assert.Equal(100.0, Utils.RoundToDecimalPlaces(149.9, -2), 5);
    }

    [Fact]
    public void ExtendLimits_DataRange_WhenZeroLimits()
    {
        var data = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
        var (min, max) = Utils.ExtendLimits(data, [0.0, 0.0]);
        Assert.True(min <= 1.0);
        Assert.True(max >= 5.0);
    }

    [Fact]
    public void ExtendLimits_UsesExplicitLimits()
    {
        var data = new[] { 1.0, 2.0, 3.0 };
        var (min, max) = Utils.ExtendLimits(data, [0.0, 10.0]);
        Assert.Equal(0.0, min, 5);
        Assert.Equal(10.0, max, 5);
    }
}
