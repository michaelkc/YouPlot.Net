namespace YouPlotNet.UnicodePlot;

public static class Histogram
{
    public static (double[] Edges, int[] Counts, string Closed) ComputeHistogram(double[] values, int? nbins, string closed = "left")
    {
        if (values.Length == 0)
        {
            throw new ArgumentException("empty");
        }

        var n = nbins ?? (int)Math.Ceiling(Math.Log2(values.Length) + 1);
        var min = values.Min();
        var max = values.Max();
        if (min == max)
        {
            min -= 0.5;
            max += 0.5;
        }

        var rawBinWidth = (max - min) / n;
        var binWidth = NiceBinWidth(rawBinWidth);
        var start = Math.Floor(min / binWidth) * binWidth;
        var stop = Math.Ceiling(max / binWidth) * binWidth;
        var actualBins = Math.Max((int)Math.Round((stop - start) / binWidth, MidpointRounding.AwayFromZero), 1);
        var edges = new double[actualBins + 1];
        for (var i = 0; i <= actualBins; i++)
        {
            edges[i] = start + (i * binWidth);
        }

        var counts = new int[actualBins];
        foreach (var value in values)
        {
            for (var j = 0; j < actualBins; j++)
            {
                var inBin = closed.Equals("right", StringComparison.OrdinalIgnoreCase)
                    ? ((value > edges[j] && value <= edges[j + 1]) || (j == 0 && value == edges[0]))
                    : ((value >= edges[j] && value < edges[j + 1]) || (j == actualBins - 1 && value == edges[actualBins]));
                if (!inBin) continue;
                counts[j]++;
                break;
            }
        }

        return (edges, counts, closed);
    }

    public static string[] BuildLabels(double[] edges, int[] counts, string closed)
    {
        var labels = new string[counts.Length];
        var binWidth = edges[1] - edges[0];
        var padLeft = 0;
        var padRight = 0;
        for (var i = 0; i < edges.Length; i++)
        {
            var val1 = Utils.FloatRoundLog10(edges[i], binWidth);
            var val2 = Utils.FloatRoundLog10(val1 + binWidth, binWidth);
            var a1 = val1.ToString("G", CultureInfo.InvariantCulture).Split('.');
            var a2 = val2.ToString("G", CultureInfo.InvariantCulture).Split('.');
            padLeft = Math.Max(padLeft, Math.Max(a1[0].Length, a2[0].Length));
            padRight = Math.Max(padRight, Math.Max(a1.Length > 1 ? a1[1].Length : 0, a2.Length > 1 ? a2[1].Length : 0));
        }

        var l = closed.Equals("right", StringComparison.OrdinalIgnoreCase) ? "(" : "[";
        var r = closed.Equals("right", StringComparison.OrdinalIgnoreCase) ? "]" : ")";
        for (var i = 0; i < counts.Length; i++)
        {
            var val1 = Utils.FloatRoundLog10(edges[i], binWidth);
            var val2 = Utils.FloatRoundLog10(val1 + binWidth, binWidth);
            labels[i] = $"\x1b[90m{l}\x1b[0m{PadNumber(val1, padLeft, padRight)}\x1b[90m, \x1b[0m{PadNumber(val2, padLeft, padRight)}\x1b[90m{r}\x1b[0m";
        }

        return labels;
    }

    private static string PadNumber(double value, int padLeft, int padRight)
    {
        var text = value.ToString("G", CultureInfo.InvariantCulture);
        if (padRight > 0 && !text.Contains('.', StringComparison.Ordinal) && !text.Contains('E', StringComparison.OrdinalIgnoreCase))
        {
            text += ".0";
        }

        var parts = text.Split('.');
        var whole = new string(' ', padLeft - parts[0].Length) + parts[0];
        var frac = parts.Length > 1 ? parts[1] : string.Empty;
        return whole + (padRight > 0 ? "." + frac.PadRight(padRight) : string.Empty);
    }

    private static double NiceBinWidth(double rawBinWidth)
    {
        if (rawBinWidth <= 0)
        {
            return 1;
        }

        var exponent = Math.Floor(Math.Log10(rawBinWidth));
        var magnitude = Math.Pow(10, exponent);
        var fraction = rawBinWidth / magnitude;
        var niceFraction = fraction < 1.5
            ? 1
            : fraction < 3
                ? 2
                : fraction < 7
                    ? 5
                    : 10;
        return niceFraction * magnitude;
    }
}
