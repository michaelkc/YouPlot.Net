namespace YouPlotNet.YouPlot;

public static class YouPlotModule
{
    public static bool RunAsExecutable { get; set; }

    public static string HelpText =>
        $"""

         Program: YouPlot.NET (Tools for plotting on the terminal)
         Version: {Version.Value} (based on Ruby YouPlot v0.4.6)
         Source:  https://github.com/red-data-tools/YouPlot (original)

         Usage:   uplot <command> [options] <in.tsv>

         Commands:
             barplot    bar           draw a horizontal barplot
             histogram  hist          draw a horizontal histogram
             lineplot   line          draw a line chart
             lineplots  lines         draw a line chart with multiple series
             scatter    s             draw a scatter plot
             density    d             draw a density plot
             boxplot    box           draw a horizontal boxplot
             count      c             draw a barplot based on the number of
                                      occurrences (slow)
             colors     color         show the list of available colors

         General options:
             --config                 print config file info
             --help                   print command specific help menu
             --version                print the version of YouPlot

         """;

    public static string VersionText => $"YouPlot.NET {Version.Value}";
}
