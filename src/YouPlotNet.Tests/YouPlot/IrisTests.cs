// Port of ruby_source/YouPlot/test/youplot/iris_test.rb
namespace YouPlotNet.Tests.YouPlot;

public class IrisTests
{
    // ── plot type aliases ─────────────────────────────────────────────────────

    [Fact]
    public void Barplot()
        => Assert.Equal(Fixture("iris-barplot.txt"), RunIris("barplot", "-H", "-d,", "-t", "IRIS-BARPLOT"));

    [Fact]
    public void Bar()
        => Assert.Equal(Fixture("iris-barplot.txt"), RunIris("bar", "-H", "-d,", "-t", "IRIS-BARPLOT"));

    [Fact]
    public void Histogram()
        => Assert.Equal(Fixture("iris-histogram.txt"), RunIris("histogram", "-H", "-d,", "-t", "IRIS-HISTOGRAM"));

    [Fact]
    public void Hist()
        => Assert.Equal(Fixture("iris-histogram.txt"), RunIris("hist", "-H", "-d,", "-t", "IRIS-HISTOGRAM"));

    [Fact]
    public void Lineplot()
        => Assert.Equal(Fixture("iris-lineplot.txt"), RunIris("lineplot", "-H", "-d,", "-t", "IRIS-LINEPLOT"));

    [Fact]
    public void Line()
        => Assert.Equal(Fixture("iris-lineplot.txt"), RunIris("line", "-H", "-d,", "-t", "IRIS-LINEPLOT"));

    [Fact]
    public void L()
        => Assert.Equal(Fixture("iris-lineplot.txt"), RunIris("l", "-H", "-d,", "-t", "IRIS-LINEPLOT"));

    [Fact]
    public void Lineplots()
        => Assert.Equal(Fixture("iris-lineplots.txt"), RunIris("lineplots", "-H", "-d,", "-t", "IRIS-LINEPLOTS"));

    [Fact]
    public void Lines()
        => Assert.Equal(Fixture("iris-lineplots.txt"), RunIris("lines", "-H", "-d,", "-t", "IRIS-LINEPLOTS"));

    [Fact]
    public void Ls()
        => Assert.Equal(Fixture("iris-lineplots.txt"), RunIris("lines", "-H", "-d,", "-t", "IRIS-LINEPLOTS"));

    [Fact]
    public void Scatter()
        => Assert.Equal(Fixture("iris-scatter.txt"), RunIris("scatter", "-H", "-d,", "-t", "IRIS-SCATTER"));

    [Fact]
    public void S()
        => Assert.Equal(Fixture("iris-scatter.txt"), RunIris("s", "-H", "-d,", "-t", "IRIS-SCATTER"));

    [Fact]
    public void Density()
        => Assert.Equal(Fixture("iris-density.txt"), RunIris("density", "-H", "-d,", "-t", "IRIS-DENSITY"));

    [Fact]
    public void D()
        => Assert.Equal(Fixture("iris-density.txt"), RunIris("d", "-H", "-d,", "-t", "IRIS-DENSITY"));

    [Fact]
    public void Boxplot()
        => Assert.Equal(Fixture("iris-boxplot.txt"), RunIris("boxplot", "-H", "-d,", "-t", "IRIS-BOXPLOT"));

    [Fact]
    public void Box()
        => Assert.Equal(Fixture("iris-boxplot.txt"), RunIris("box", "-H", "-d,", "-t", "IRIS-BOXPLOT"));

    [Fact]
    public void Count_C()
        => Assert.Equal(Fixture("iris-count.txt"), RunIris("count", "-H", "-d,"));

    [Fact]
    public void C_Count()
        => Assert.Equal(Fixture("iris-count.txt"), RunIris("c", "-H", "-d,"));

    // ── output redirection ─────────────────────────────────────────────────────

    [Fact]
    public void PlotOutputStdout()
    {
        var (stderr, stdout) = RunIrisCaptureBoth("bar", "-o", "-H", "-d,", "-t", "IRIS-BARPLOT");
        Assert.Equal("", stderr);
        Assert.Equal(Fixture("iris-barplot.txt"), stdout);
    }

    [Fact]
    public void DataOutputStdout()
    {
        var (stderr, stdout) = RunIrisCaptureBoth("bar", "-O", "-H", "-d,", "-t", "IRIS-BARPLOT");
        Assert.Equal(Fixture("iris-barplot.txt"), stderr);
        Assert.Equal(Nl(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "iris.csv"))), stdout);
    }

    // ── colors commands ────────────────────────────────────────────────────────

    [Theory]
    [InlineData("colors")]
    [InlineData("color")]
    [InlineData("colours")]
    [InlineData("colour")]
    public void ColorsCommand(string cmd)
    {
        var (stderr, stdout) = RunIrisCaptureBoth(cmd);
        Assert.Equal(Fixture("colors.txt"), stderr);
        Assert.Equal("", stdout);
    }

    [Fact]
    public void Colors_OutputStdout()
    {
        var (stderr, stdout) = RunIrisCaptureBoth("colors", "-o");
        Assert.Equal("", stderr);
        Assert.Equal(Fixture("colors.txt"), stdout);
    }

    // ── error handling ─────────────────────────────────────────────────────────

    [Fact]
    public void UnrecognizedCommand_Throws()
    {
        var (stderr, stdout) = ("", "");
        Assert.ThrowsAny<Exception>(() =>
        {
            var result = RunIrisCaptureBoth("abracadabra", "--hadley", "--wickham");
            stderr = result.Stderr;
            stdout = result.Stdout;
        });
        Assert.Equal("", stderr);
        Assert.Equal("", stdout);
    }

    // ── encoding ───────────────────────────────────────────────────────────────

    [Fact]
    public void Encoding_Utf16()
        => Assert.Equal(Fixture("iris-scatter.txt"), RunIrisUtf16("s", "--encoding", "UTF-16", "-H", "-d,", "-t", "IRIS-SCATTER"));
}
