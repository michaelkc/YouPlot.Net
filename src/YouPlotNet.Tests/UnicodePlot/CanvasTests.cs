// Port of ruby_source/unicode_plot.rb/test/test-canvas.rb
namespace YouPlotNet.Tests.UnicodePlot;

public class CanvasTests
{
    // Deterministic test data (from Ruby test seed 1337)
    private static readonly double[] X1 = [0.226582, 0.504629, 0.933372, 0.522172, 0.505208, 0.0997825, 0.0443222, 0.722906, 0.812814, 0.245457, 0.11202, 0.000341996, 0.380001, 0.505277, 0.841177, 0.326561, 0.810857, 0.850456, 0.478053, 0.179066];
    private static readonly double[] Y1 = [0.44701, 0.219519, 0.677372, 0.746407, 0.735727, 0.574789, 0.538086, 0.848053, 0.110351, 0.796793, 0.987618, 0.801862, 0.365172, 0.469959, 0.306373, 0.704691, 0.540434, 0.405842, 0.805117, 0.014829];
    private static readonly double[] X2 = [0.486366, 0.911547, 0.900818, 0.641951, 0.546221, 0.036135, 0.931522, 0.196704, 0.710775, 0.969291, 0.32546, 0.632833, 0.815576, 0.85278, 0.577286, 0.887004, 0.231596, 0.288337, 0.881386, 0.0952668, 0.609881, 0.393795, 0.84808, 0.453653, 0.746048, 0.924725, 0.100012, 0.754283, 0.769802, 0.997368, 0.0791693, 0.234334, 0.361207, 0.1037, 0.713739, 0.510725, 0.649145, 0.233949, 0.812092, 0.914384, 0.106925, 0.570467, 0.594956, 0.118498, 0.699827, 0.380363, 0.843282, 0.28761, 0.541469, 0.568466];
    private static readonly double[] Y2 = [0.417777, 0.774845, 0.00230619, 0.907031, 0.971138, 0.0524795, 0.957415, 0.328894, 0.530493, 0.193359, 0.768422, 0.783238, 0.607772, 0.0261113, 0.0849032, 0.461164, 0.613067, 0.785021, 0.988875, 0.131524, 0.0657328, 0.466453, 0.560878, 0.925428, 0.238691, 0.692385, 0.203687, 0.441146, 0.229352, 0.332706, 0.113543, 0.537354, 0.965718, 0.437026, 0.960983, 0.372294, 0.0226533, 0.593514, 0.657878, 0.450696, 0.436169, 0.445539, 0.0534673, 0.0882236, 0.361795, 0.182991, 0.156862, 0.734805, 0.166076, 0.1172];

    // ── utility / types tests ────────────────────────────────────────────────

    [Fact]
    public void CanvasTypes()
    {
        foreach (var t in new[] { "ascii", "block", "braille", "density", "dot" })
            Assert.NotNull(Canvas.Create(t, 40, 10, 0, 0, 1, 1));
    }

    [Fact]
    public void UnknownCanvasType_Throws()
        => Assert.Throws<ArgumentException>(() => Canvas.Create("invalid", 40, 10, 0, 0, 1, 1));

    // ── empty show ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("braille", "canvas/empty_braille_show.txt")]
    [InlineData("ascii",   "canvas/empty_show.txt")]
    [InlineData("density", "canvas/empty_show.txt")]
    [InlineData("dot",     "canvas/empty_show.txt")]
    [InlineData("block",   "canvas/empty_show.txt")]
    public void Empty_Show(string canvasType, string fixtureName)
    {
        var canvas = Canvas.Create(canvasType, 40, 10, 0, 0, 1, 1);
        var sw = new StringWriter();
        canvas.Show(new IOContext(sw, true));
        Assert.Equal(Fixture(fixtureName), Nl(sw.ToString()));
    }

    // ── drawing helper (mirrors Ruby setup + drawing operations) ─────────────

    private static Canvas DrawCanvas(string canvasType)
    {
        var canvas = Canvas.Create(canvasType, 40, 10, 0, 0, 1, 1);
        canvas.Line(0, 0, 1, 1, "blue");
        canvas.Points(X1, Y1, "white");
        canvas.Pixel(2, 4, "cyan");   // Ruby: canvas.pixel!(2, 4, :cyan)
        canvas.Points(X2, Y2, "red");
        canvas.Line(0, 1, 0.5, 0, "green");
        canvas.Point(0.05, 0.3, "cyan");
        canvas.Lines([1.0, 2.0], [2.0, 1.0]);
        canvas.Line(0, 0, 9.0, 9999.0, "yellow");
        canvas.Line(0, 0, 1, 1, "blue");
        canvas.Line(0.1, 0.7, 0.9, 0.6, "red");
        return canvas;
    }

    // ── PrintRow ─────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("braille")]
    [InlineData("ascii")]
    [InlineData("density")]
    [InlineData("dot")]
    [InlineData("block")]
    public void PrintRow_Row2(string canvasType)
    {
        var canvas = DrawCanvas(canvasType);
        var sw = new StringWriter();
        canvas.PrintRow(new IOContext(sw, true), 2);
        Assert.Equal(Fixture($"canvas/{canvasType}_printrow.txt"), Nl(sw.ToString()));
    }

    // ── Print (all rows, no border) ───────────────────────────────────────────

    [Theory]
    [InlineData("braille")]
    [InlineData("ascii")]
    [InlineData("density")]
    [InlineData("dot")]
    [InlineData("block")]
    public void Print_Color(string canvasType)
    {
        var canvas = DrawCanvas(canvasType);
        var sw = new StringWriter();
        canvas.Print(new IOContext(sw, true));
        Assert.Equal(Fixture($"canvas/{canvasType}_print.txt"), Nl(sw.ToString()));
    }

    [Theory]
    [InlineData("braille")]
    [InlineData("ascii")]
    [InlineData("density")]
    [InlineData("dot")]
    [InlineData("block")]
    public void Print_NoColor(string canvasType)
    {
        var canvas = DrawCanvas(canvasType);
        var sw = new StringWriter();
        canvas.Print(new IOContext(sw, false));
        Assert.Equal(Fixture($"canvas/{canvasType}_print_nocolor.txt"), Nl(sw.ToString()));
    }

    // ── Show (with border) ────────────────────────────────────────────────────

    [Theory]
    [InlineData("braille")]
    [InlineData("ascii")]
    [InlineData("density")]
    [InlineData("dot")]
    [InlineData("block")]
    public void Show_Color(string canvasType)
    {
        var canvas = DrawCanvas(canvasType);
        var sw = new StringWriter();
        canvas.Show(new IOContext(sw, true));
        Assert.Equal(Fixture($"canvas/{canvasType}_show.txt"), Nl(sw.ToString()));
    }

    [Theory]
    [InlineData("braille")]
    [InlineData("ascii")]
    [InlineData("density")]
    [InlineData("dot")]
    [InlineData("block")]
    public void Show_NoColor(string canvasType)
    {
        var canvas = DrawCanvas(canvasType);
        var sw = new StringWriter();
        canvas.Show(new IOContext(sw, false));
        Assert.Equal(Fixture($"canvas/{canvasType}_show_nocolor.txt"), Nl(sw.ToString()));
    }
}
