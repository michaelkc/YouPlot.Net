using YouPlotNet.UnicodePlot;
using YouPlotNet.YouPlot.Backends;

namespace YouPlotNet.YouPlot;

public sealed class Command
{
    private readonly List<string> _argv;
    private readonly TextReader _stdin;
    private readonly TextWriter _stdout;
    private readonly TextWriter _stderr;
    private readonly Parser _parser = new();

    public Command(IEnumerable<string> argv, TextReader? stdin = null, TextWriter? stdout = null, TextWriter? stderr = null)
    {
        _argv = argv.ToList();
        _stdin = stdin ?? Console.In;
        _stdout = stdout ?? Console.Out;
        _stderr = stderr ?? Console.Error;
    }

    public void Run()
    {
        _parser.ParseOptions(_argv);
        var rawCommand = _parser.Command;
        var command = (rawCommand ?? "help").ToLowerInvariant();
        var options = _parser.Options;
        var parameters = _parser.Params;
        if (ReferenceEquals(options.Output, Console.Error) || options.Output is null) options.Output = _stderr;
        if (ReferenceEquals(options.Output, Console.Out)) options.Output = _stdout;
        if (ReferenceEquals(options.Pass, Console.Out)) options.Pass = _stdout;

        // Handle help: null command (no args) → stderr; explicit "help" → stdout
        if (command == "help")
        {
            var helpOut = rawCommand is null ? _stderr : _stdout;
            helpOut.WriteLine(YouPlotModule.HelpText);
            return;
        }

        if (command == "version")
        {
            _stdout.WriteLine(YouPlotModule.VersionText);
            return;
        }

        if (command is "colors" or "color" or "colours" or "colour")
        {
            OutputPlot(UnicodePlotBackend.Colors(options.ColorNames), options);
            return;
        }

        var input = _stdin.ReadToEnd();
        OutputData(input, options);
        var data = ParseDsv(input, options);
        var plot = CreatePlot(command, data, parameters, options);
        OutputPlot(plot, options);
    }

    private Data ParseDsv(string input, Options options)
    {
        if (!string.IsNullOrEmpty(options.Encoding))
        {
            // Re-interpret the raw bytes of the input as the specified encoding.
            // Since stdin was read by default as UTF-8, we re-encode from Latin-1
            // (which preserves byte values unchanged) back to bytes, then decode
            // with the target encoding as the Ruby --encoding flag intends.
            var rawBytes = Encoding.Latin1.GetBytes(input);
            var targetEncoding = Encoding.GetEncoding(options.Encoding);
            input = targetEncoding.GetString(rawBytes);
        }

        return DSV.Parse(input, options.Delimiter, options.Headers, options.Transpose);
    }

    private static IRenderable CreatePlot(string command, Data data, Parameters parameters, Options options) => command switch
    {
        "bar" or "barplot" => UnicodePlotBackend.Barplot(data, parameters, options.Fmt),
        "count" or "c" => UnicodePlotBackend.Barplot(data, parameters, count: true, reverse: options.Reverse),
        "hist" or "histogram" => UnicodePlotBackend.Histogram(data, parameters),
        "line" or "lineplot" or "l" => UnicodePlotBackend.Line(data, parameters, options.Fmt),
        "lines" or "lineplots" or "ls" => UnicodePlotBackend.Lines(data, parameters, options.Fmt),
        "scatter" or "s" => UnicodePlotBackend.Scatter(data, parameters, options.Fmt),
        "density" or "d" => UnicodePlotBackend.Density(data, parameters, options.Fmt),
        "box" or "boxplot" => UnicodePlotBackend.Boxplot(data, parameters),
        _ => throw new InvalidOperationException($"unrecognized plot_type: {command}")
    };

    private void OutputData(string input, Options options)
    {
        switch (options.Pass)
        {
            case TextWriter writer: writer.Write(input); break;
            case string path when !string.IsNullOrEmpty(path): File.WriteAllText(path, input); break;
            case true: _stdout.Write(input); break;
        }
    }

    private static void OutputPlot(IRenderable plot, Options options)
    {
        var color = options.ForceColor;
        switch (options.Output)
        {
            case TextWriter writer:
                plot.Render(writer, true, color);
                break;
            case string path:
                using (var file = new StreamWriter(path, false, Encoding.UTF8))
                {
                    plot.Render(file, true, color);
                }
                break;
            default:
                plot.Render(Console.Error, true, color);
                break;
        }
    }
}
