using YamlDotNet.Serialization;

namespace YouPlotNet.YouPlot;

public sealed class Parser
{
    public string? Command { get; private set; }
    public bool ShowHelp { get; private set; }
    public Options Options { get; } = new();
    public Parameters Params { get; } = new();
    public string? ConfigFile { get; private set; }

    public void ParseOptions(IEnumerable<string> args)
    {
        var tokens = args.ToList();
        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (!token.StartsWith('-') && Command is null)
            {
                Command = token;
                continue;
            }

            var shortToken = token;
            string? attachedValue = null;
            if (token.Length > 2 && token[0] == '-' && token[1] != '-')
            {
                shortToken = token[..2];
                attachedValue = token[2..];
            }

            string Next() => ++i < tokens.Count ? tokens[i] : throw new ArgumentException($"missing value for {token}");
            string ValueOrNext() => !string.IsNullOrEmpty(attachedValue) ? attachedValue : Next();
            switch (shortToken)
            {
                case "-O": case "--pass": Options.Pass = i + 1 < tokens.Count && !tokens[i + 1].StartsWith('-') ? Next() : Console.Out; break;
                case "-o": case "--output": Options.Output = i + 1 < tokens.Count && !tokens[i + 1].StartsWith('-') ? Next() : Console.Out; break;
                case "-d": case "--delimiter": Options.Delimiter = ValueOrNext(); break;
                case "-H": case "--headers": Options.Headers = true; break;
                case "-T": case "--transpose": Options.Transpose = true; break;
                case "-t": case "--title": Params.Title = ValueOrNext(); break;
                case "--xlabel": Params.Xlabel = Next(); break;
                case "--ylabel": Params.Ylabel = Next(); break;
                case "-w": case "--width": Params.Width = int.Parse(ValueOrNext(), CultureInfo.InvariantCulture); break;
                case "-h": case "--height": Params.Height = int.Parse(ValueOrNext(), CultureInfo.InvariantCulture); break;
                case "-b": case "--border": Params.Border = ValueOrNext(); break;
                case "-m": case "--margin": Params.Margin = int.Parse(ValueOrNext(), CultureInfo.InvariantCulture); break;
                case "--padding": Params.Padding = int.Parse(Next(), CultureInfo.InvariantCulture); break;
                case "-c": case "--color": Params.Color = ValueOrNext(); break;
                case "--labels": Params.Labels = true; break;
                case "--no-labels": Params.Labels = false; break;
                case "-p": case "--progress": Options.Progressive = true; break;
                case "-C": case "--color-output": Options.ForceColor = true; break;
                case "-M": case "--monochrome": Options.ForceColor = false; break;
                case "--encoding": Options.Encoding = Next(); break;
                case "--config": ConfigFile = i + 1 < tokens.Count && !tokens[i + 1].StartsWith('-') ? Next() : null; break;
                case "--debug": Options.Debug = true; break;
                case "--symbol": Params.Symbol = Next(); break;
                case "--xscale": Params.Xscale = Next(); break;
                case "--canvas": Params.Canvas = Next(); break;
                case "--xlim": Params.Xlim = Next().Split(',').Select(static s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray(); break;
                case "--ylim": Params.Ylim = Next().Split(',').Select(static s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray(); break;
                case "--grid": Params.Grid = true; break;
                case "--no-grid": Params.Grid = false; break;
                case "--fmt": Options.Fmt = Next(); break;
                case "-r": case "--reverse": Options.Reverse = true; break;
                case "--closed": Params.Closed = Next(); break;
                case "-n":
                    if (string.Equals(Command, "colors", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(Command, "color", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(Command, "colours", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(Command, "colour", StringComparison.OrdinalIgnoreCase))
                        Options.ColorNames = true;
                    else Params.Nbins = int.Parse(ValueOrNext(), CultureInfo.InvariantCulture);
                    break;
                case "--names": Options.ColorNames = true; break;
                case "--help": ShowHelp = true; Command ??= "help"; break;
                case "--version": Command = "version"; break;
            }
        }

        ApplyConfigFile();
    }

    private void ApplyConfigFile()
    {
        var file = ConfigFile ?? FindConfigFile();
        if (string.IsNullOrEmpty(file) || !File.Exists(file)) return;
        var deserializer = new DeserializerBuilder().Build();
        var config = deserializer.Deserialize<Dictionary<string, object?>>(File.ReadAllText(file));
        foreach (var (key, raw) in config)
        {
            var value = raw?.ToString();
            switch (key)
            {
                case "delimiter" when Options.Delimiter == "\t": Options.Delimiter = value ?? "\t"; break;
                case "title" when Params.Title is null: Params.Title = value; break;
                case "width" when !Params.Width.HasValue && int.TryParse(value, out var width): Params.Width = width; break;
                case "height" when !Params.Height.HasValue && int.TryParse(value, out var height): Params.Height = height; break;
                case "border" when Params.Border is null: Params.Border = value; break;
                case "margin" when !Params.Margin.HasValue && int.TryParse(value, out var margin): Params.Margin = margin; break;
                case "padding" when !Params.Padding.HasValue && int.TryParse(value, out var padding): Params.Padding = padding; break;
                case "color" when Params.Color is null: Params.Color = value; break;
                case "xlabel" when Params.Xlabel is null: Params.Xlabel = value; break;
                case "ylabel" when Params.Ylabel is null: Params.Ylabel = value; break;
                case "symbol" when Params.Symbol is null: Params.Symbol = value; break;
                case "xscale" when Params.Xscale is null: Params.Xscale = value; break;
                case "canvas" when Params.Canvas is null: Params.Canvas = value; break;
            }
        }
    }

    private static string? FindConfigFile()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var paths = new[] { Environment.GetEnvironmentVariable("MYYOUPLOTRC"), ".youplot.yml", ".youplotrc", Path.Combine(home, ".youplotrc"), Path.Combine(home, ".youplot.yml"), Path.Combine(home, ".config", "youplot", "youplotrc"), Path.Combine(home, ".config", "youplot", "youplot.yml") };
        return paths.FirstOrDefault(static path => !string.IsNullOrEmpty(path) && File.Exists(path));
    }
}
