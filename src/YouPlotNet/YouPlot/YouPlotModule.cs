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

    public static string CommandHelpText(string command)
    {
        string? cmdSpecific = command switch
        {
            "barplot" or "bar" => string.Join("\n",
                "        --xscale STR        axis scaling (log, ln, log10, lg, log2, lb)",
                "        --fmt STR           xy : header is like x, y...",
                "                            yx : header is like y, x...",
                "        --symbol STR        character to be used to plot the bars"),
            "count" or "c" => string.Join("\n",
                "        --xscale STR        axis scaling (log, ln, log10, lg, log2, lb)",
                "        --symbol STR        character to be used to plot the bars",
                "    -r, --reverse           reverse the result of comparisons"),
            "histogram" or "hist" => string.Join("\n",
                "    -n, --nbins INT         approximate number of bins",
                "        --closed STR        side of the intervals to be closed [left]",
                "        --symbol STR        character to be used to plot the bars"),
            "line" or "lineplot" or "l" => string.Join("\n",
                "        --xlim FLOAT,FLOAT  plotting range for the x coordinate",
                "        --ylim FLOAT,FLOAT  plotting range for the y coordinate",
                "        --fmt STR           xy : header is like x, y...",
                "                            yx : header is like y, x...",
                "        --[no-]grid         draws grid-lines at the origin",
                "        --canvas STR        type of canvas",
                "                            (ascii, block, braille, density, dot)"),
            "lines" or "lineplots" or "ls" or "scatter" or "s" or "density" or "d" => string.Join("\n",
                "        --xlim FLOAT,FLOAT  plotting range for the x coordinate",
                "        --ylim FLOAT,FLOAT  plotting range for the y coordinate",
                "        --fmt STR           xyxy : header is like x1, y1, x2, y2, x3, y3...",
                "                            xyy  : header is like x, y1, y2, y2, y3...",
                "        --[no-]grid         draws grid-lines at the origin",
                "        --canvas STR        type of canvas",
                "                            (ascii, block, braille, density, dot)"),
            "boxplot" or "box" =>
                "        --xlim FLOAT,FLOAT  plotting range for the x coordinate",
            "colors" or "color" or "colours" or "colour" =>
                "    -n, --names             show color names only",
            _ => null
        };

        if (cmdSpecific is null)
            return HelpText;

        var commonOptions = string.Join("\n",
            "    -O, --pass [FILE]       file to output input data to [stdout]",
            "                            for inserting YouPlot in the middle of Unix pipes",
            "    -o, --output [FILE]     file to output plots to [stdout]",
            "                            If no option is specified, plot will print to stderr",
            "    -d, --delimiter DELIM   use DELIM instead of [TAB] for field delimiter",
            "    -H, --headers           specify that the input has header row",
            "    -T, --transpose         transpose the axes of the input data",
            "    -t, --title STR         print string on the top of plot",
            "        --xlabel STR        print string on the bottom of the plot",
            "        --ylabel STR        print string on the far left of the plot",
            "    -w, --width INT         number of characters per row",
            "    -h, --height INT        number of rows",
            "    -b, --border STR        specify the style of the bounding box",
            "                            (solid, corners, barplot)",
            "    -m, --margin INT        number of spaces to the left of the plot",
            "        --padding INT       space of the left and right of the plot",
            "    -c, --color VAL         color of the drawing",
            "        --[no-]labels       hide the labels",
            "    -p, --progress          progressive mode [experimental]",
            "    -C, --color-output      colorize even if writing to a pipe",
            "    -M, --monochrome        no colouring even if writing to a tty",
            "        --encoding STR      specify the input encoding",
            "        --help              print sub-command help menu",
            "        --config FILE       specify a config file",
            "        --debug             print preprocessed data");

        return string.Join("\n",
            $"Usage: YouPlot {command} [options] <in.tsv>",
            "",
            $"Options for {command}:",
            cmdSpecific,
            "",
            "Common options:",
            commonOptions,
            "");
    }
}
