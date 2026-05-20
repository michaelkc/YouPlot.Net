// Port of ruby_source/YouPlot/test/youplot/simple_test.rb
namespace YouPlotNet.Tests.YouPlot;

public class SimpleTests
{
    // ── single-command tests (no options) ─────────────────────────────────────

    [Fact]
    public void Barplot_NegativeValues_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => RunSimple("barplot"));

    [Fact]
    public void Bar_NegativeValues_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => RunSimple("bar"));

    [Fact]
    public void Histogram()
        => Assert.Equal(Fixture("simple-histogram.txt"), RunSimple("histogram"));

    [Fact]
    public void Hist()
        => Assert.Equal(Fixture("simple-histogram.txt"), RunSimple("hist"));

    [Fact]
    public void Lineplot()
        => Assert.Equal(Fixture("simple-lineplot.txt"), RunSimple("lineplot"));

    [Fact]
    public void Line()
        => Assert.Equal(Fixture("simple-lineplot.txt"), RunSimple("line"));

    [Fact]
    public void Lineplots_SingleSeries_Throws()
        => Assert.Throws<InvalidOperationException>(() => RunSimple("lineplots"));

    [Fact]
    public void Lines_SingleSeries_Throws()
        => Assert.Throws<InvalidOperationException>(() => RunSimple("lines"));

    [Fact]
    public void Scatter_SingleSeries_Throws()
        => Assert.Throws<InvalidOperationException>(() => RunSimple("scatter"));

    [Fact]
    public void S_SingleSeries_Throws()
        => Assert.Throws<InvalidOperationException>(() => RunSimple("s"));

    [Fact]
    public void Density_SingleSeries_Throws()
        => Assert.Throws<InvalidOperationException>(() => RunSimple("density"));

    [Fact]
    public void D_SingleSeries_Throws()
        => Assert.Throws<InvalidOperationException>(() => RunSimple("d"));

    [Fact]
    public void Boxplot()
        => Assert.Equal(Fixture("simple-boxplot.txt"), RunSimple("boxplot"));

    [Fact]
    public void Box()
        => Assert.Equal(Fixture("simple-boxplot.txt"), RunSimple("box"));

    [Fact]
    public void Count()
        => Assert.Equal(Fixture("simple-count.txt"), RunSimple("c"));

    [Fact]
    public void C()
        => Assert.Equal(Fixture("simple-count.txt"), RunSimple("count"));

    // ── output redirection ─────────────────────────────────────────────────────

    [Fact]
    public void PlotOutputStdout()
    {
        var (stderr, stdout) = RunSimpleCaptureBoth("line", "-o");
        Assert.Equal("", stderr);
        Assert.Equal(Fixture("simple-lineplot.txt"), stdout);
    }

    [Fact]
    public void DataOutputStdout()
    {
        var (stderr, stdout) = RunSimpleCaptureBoth("box", "-O");
        Assert.Equal(Fixture("simple-boxplot.txt"), stderr);
        Assert.Equal(Nl(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "simple.tsv"))), stdout);
    }

    // ── transpose ─────────────────────────────────────────────────────────────

    [Fact]
    public void Line_Transpose()
        => Assert.Equal(Fixture("simple-lineplot.txt"), RunSimpleT("line", "--transpose"));

    [Fact]
    public void Line_T()
        => Assert.Equal(Fixture("simple-lineplot.txt"), RunSimpleT("line", "-T"));

    // ── labels ────────────────────────────────────────────────────────────────

    [Fact]
    public void Line_Xlabel()
        => Assert.Equal(Fixture("simple-lineplot-xlabel.txt"), RunSimple("line", "--xlabel", "X-LABEL"));

    [Fact]
    public void Line_Ylabel()
        => Assert.Equal(Fixture("simple-lineplot-ylabel.txt"), RunSimple("line", "--ylabel", "Y-LABEL"));

    // ── sizing ────────────────────────────────────────────────────────────────

    [Fact]
    public void Line_Width()
        => Assert.Equal(Fixture("simple-lineplot-width-17.txt"), RunSimple("line", "--width", "17"));

    [Fact]
    public void Line_W()
        => Assert.Equal(Fixture("simple-lineplot-width-17.txt"), RunSimple("line", "-w", "17"));

    [Fact]
    public void Line_Height()
        => Assert.Equal(Fixture("simple-lineplot-height-17.txt"), RunSimple("line", "--height", "17"));

    [Fact]
    public void Line_H()
        => Assert.Equal(Fixture("simple-lineplot-height-17.txt"), RunSimple("line", "-h", "17"));

    [Fact]
    public void Line_Margin()
        => Assert.Equal(Fixture("simple-lineplot-margin-17.txt"), RunSimple("line", "--margin", "17"));

    [Fact]
    public void Line_M()
        => Assert.Equal(Fixture("simple-lineplot-margin-17.txt"), RunSimple("line", "-m", "17"));

    [Fact]
    public void Line_Padding()
        => Assert.Equal(Fixture("simple-lineplot-padding-17.txt"), RunSimple("line", "--padding", "17"));

    // ── border ────────────────────────────────────────────────────────────────

    [Fact]
    public void Line_BorderCorners()
        => Assert.Equal(Fixture("simple-lineplot-border-corners.txt"), RunSimple("line", "--border", "corners"));

    [Fact]
    public void Line_B_Corners()
        => Assert.Equal(Fixture("simple-lineplot-border-corners.txt"), RunSimple("line", "-b", "corners"));

    [Fact]
    public void Line_BorderBarplot()
        => Assert.Equal(Fixture("simple-lineplot-border-barplot.txt"), RunSimple("line", "--border", "barplot"));

    [Fact]
    public void Line_B_Barplot()
        => Assert.Equal(Fixture("simple-lineplot-border-barplot.txt"), RunSimple("line", "-b", "barplot"));

    // ── canvas types ──────────────────────────────────────────────────────────

    [Fact]
    public void Line_CanvasAscii()
        => Assert.Equal(Fixture("simple-lineplot-canvas-ascii.txt"), RunSimple("line", "--canvas", "ascii"));

    [Fact]
    public void Line_CanvasBraille()
        => Assert.Equal(Fixture("simple-lineplot.txt"), RunSimple("line", "--canvas", "braille"));

    [Fact]
    public void Line_CanvasDensity()
        => Assert.Equal(Fixture("simple-lineplot-canvas-density.txt"), RunSimple("line", "--canvas", "density"));

    [Fact]
    public void Line_CanvasDot()
        => Assert.Equal(Fixture("simple-lineplot-canvas-dot.txt"), RunSimple("line", "--canvas", "dot"));

    // ── histogram-specific ────────────────────────────────────────────────────

    [Fact]
    public void Hist_SymbolAtmark()
        => Assert.Equal(Fixture("simple-histogram-symbol-@.txt"), RunSimple("hist", "--symbol", "@"));

    // ── axis limits ───────────────────────────────────────────────────────────

    [Fact]
    public void Line_Xlim()
        => Assert.Equal(Fixture("simple-lineplot-xlim--1-5.txt"), RunSimple("line", "--xlim", "-1,5"));

    [Fact]
    public void Line_Ylim()
        => Assert.Equal(Fixture("simple-lineplot-ylim--25-50.txt"), RunSimple("line", "--ylim", "-25,50"));

    [Fact]
    public void Line_XlimAndYlim()
        => Assert.Equal(Fixture("simple-lineplot-xlim--1-5-ylim--25-50.txt"), RunSimple("line", "--xlim", "-1,5", "--ylim", "-25,50"));

    // ── grid ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Line_Grid()
        => Assert.Equal(Fixture("simple-lineplot.txt"), RunSimple("line", "--grid"));

    [Fact]
    public void Line_NoGrid()
        => Assert.Equal(Fixture("simple-lineplot-no-grid.txt"), RunSimple("line", "--no-grid"));
}
