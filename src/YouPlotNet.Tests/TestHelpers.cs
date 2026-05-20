using System.Collections.Generic;
using System.Text;

namespace YouPlotNet.Tests;

/// <summary>
/// Shared helpers mirroring the Ruby test fixtures/with_term infrastructure.
/// </summary>
public static class TestHelpers
{
    private static readonly string FixturesDir = Path.Combine(AppContext.BaseDirectory, "Fixtures");

    /// <summary>Normalise CRLF → LF so fixture files round-trip on Windows.</summary>
    public static string Nl(string s) => s.Replace("\r\n", "\n");

    /// <summary>Read a fixture file and normalise line endings.</summary>
    public static string Fixture(string name) => Nl(File.ReadAllText(Path.Combine(FixturesDir, name)));

    /// <summary>
    /// Run a command against simple.tsv and return the stderr output (normalised).
    /// Mirrors the Ruby setup where $stdin = simple.tsv, $stderr = tempfile.
    /// </summary>
    public static string RunSimple(params string[] args)
    {
        var input = File.ReadAllText(Path.Combine(FixturesDir, "simple.tsv"));
        return RunInput(input, args);
    }

    /// <summary>Run with simpleT.tsv as input.</summary>
    public static string RunSimpleT(params string[] args)
    {
        var input = File.ReadAllText(Path.Combine(FixturesDir, "simpleT.tsv"));
        return RunInput(input, args);
    }

    /// <summary>Run with iris.csv as input.</summary>
    public static string RunIris(params string[] args)
    {
        var input = File.ReadAllText(Path.Combine(FixturesDir, "iris.csv"));
        return RunInput(input, args);
    }

    /// <summary>
    /// Run with iris_utf16.csv as raw bytes (Latin-1 round-trip), for --encoding tests.
    /// </summary>
    public static string RunIrisUtf16(params string[] args)
    {
        var rawBytes = File.ReadAllBytes(Path.Combine(FixturesDir, "iris_utf16.csv"));
        var input = Encoding.Latin1.GetString(rawBytes);
        return RunInput(input, args);
    }

    /// <summary>Run a command and return both stderr and stdout outputs.</summary>
    public static (string Stderr, string Stdout) RunSimpleCaptureBoth(params string[] args)
    {
        var input = File.ReadAllText(Path.Combine(FixturesDir, "simple.tsv"));
        return RunInputCaptureBoth(input, args);
    }

    public static (string Stderr, string Stdout) RunIrisCaptureBoth(params string[] args)
    {
        var input = File.ReadAllText(Path.Combine(FixturesDir, "iris.csv"));
        return RunInputCaptureBoth(input, args);
    }

    /// <summary>Run a command with explicit text input and return stderr (normalised).</summary>
    public static string RunInput(string input, string[] args)
    {
        var (stderr, _) = RunInputCaptureBoth(input, args);
        return stderr;
    }

    private static (string Stderr, string Stdout) RunInputCaptureBoth(string input, string[] args)
    {
        var stdin = new StringReader(input);
        var stdout = new StringWriter();
        var stderr = new StringWriter();
        new Command(args, stdin, stdout, stderr).Run();
        return (Nl(stderr.ToString()), Nl(stdout.ToString()));
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // UnicodePlot helpers (mirror Ruby's with_term / StringIO render pattern)
    // ──────────────────────────────────────────────────────────────────────────────

    /// <summary>Render a plot with color=true (mirrors Ruby's with_term { plot.render($stdout, newline: false) }).</summary>
    public static string RenderColor(IRenderable plot)
    {
        var sw = new StringWriter();
        plot.Render(sw, newline: false, color: true);
        return Nl(sw.ToString());
    }

    /// <summary>Render a plot with color=false (mirrors Ruby's StringIO.open { plot.render(sio, newline: false) }).</summary>
    public static string RenderNoColor(IRenderable plot)
    {
        var sw = new StringWriter();
        plot.Render(sw, newline: false, color: false);
        return Nl(sw.ToString());
    }

    /// <summary>
    /// Render with color=true and a trailing newline (mirrors with_term { plot.render($stdout) }).
    /// The Ruby tests check output[-1] == "\n" then compare output.chomp.
    /// </summary>
    public static string RenderColorWithNewline(IRenderable plot)
    {
        var sw = new StringWriter();
        plot.Render(sw, newline: true, color: true);
        return Nl(sw.ToString());
    }

    public static string RenderNoColorWithNewline(IRenderable plot)
    {
        var sw = new StringWriter();
        plot.Render(sw, newline: true, color: false);
        return Nl(sw.ToString());
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // DSV helper
    // ──────────────────────────────────────────────────────────────────────────────

    /// <summary>Convert jagged int[][] to List<List<string?>> for DSV method calls.</summary>
    public static List<List<string?>> ToSeries(int?[][] data)
        => data.Select(row => row.Select(v => (string?)(v?.ToString())).ToList()).ToList();
}
