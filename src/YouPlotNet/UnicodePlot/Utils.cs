namespace YouPlotNet.UnicodePlot;

public static class Utils
{
    public static (double Min, double Max) ExtendLimits(IEnumerable<double> values, IReadOnlyList<double> limits)
    {
        var (mi, ma) = (limits.Min(), limits.Max());
        if (mi == 0 && ma == 0)
        {
            var list = values.ToArray();
            mi = list.Min();
            ma = list.Max();
        }

        var diff = ma - mi;
        if (diff == 0)
        {
            ma = mi + 1;
            mi -= 1;
        }

        return limits.Count >= 2 && limits[0] == 0 && limits[1] == 0
            ? PlottingRangeNarrow(mi, ma)
            : (mi, ma);
    }

    public static (double Min, double Max) PlottingRangeNarrow(double xmin, double xmax)
    {
        var diff = xmax - xmin;
        xmax = RoundUpSubtick(xmax, diff);
        xmin = RoundDownSubtick(xmin, diff);
        return (xmin, xmax);
    }

    public static double FloatRoundLog10(double x, double m)
    {
        if (x == 0)
        {
            return 0.0;
        }

        var digits = CeilNegLog10(m) + 1;
        return x > 0
            ? RoundToDecimalPlaces(x, digits)
            : -RoundToDecimalPlaces(-x, digits);
    }

    public static double RoundUpSubtick(double x, double m)
    {
        if (x == 0)
        {
            return 0.0;
        }

        var digits = CeilNegLog10(m) + 1;
        return x > 0
            ? CeilToDecimalPlaces(x, digits)
            : -FloorToDecimalPlaces(-x, digits);
    }

    public static double RoundDownSubtick(double x, double m)
    {
        if (x == 0)
        {
            return 0.0;
        }

        var digits = CeilNegLog10(m) + 1;
        return x > 0
            ? FloorToDecimalPlaces(x, digits)
            : -CeilToDecimalPlaces(-x, digits);
    }

    public static int CeilNegLog10(double x)
    {
        if (x == 0)
        {
            return 0;
        }

        var value = -Math.Log10(Math.Abs(x));
        return Roundable(value) ? (int)Math.Ceiling(value) : (int)Math.Floor(value);
    }

    public static bool Roundable(double x) => Math.Truncate(x) == x && x >= long.MinValue && x < long.MaxValue;

    public static double RoundToDecimalPlaces(double x, int n)
    {
        if (n >= 0)
        {
            return Math.Round(x, n, MidpointRounding.AwayFromZero);
        }

        var factor = Math.Pow(10, -n);
        return Math.Round(x / factor, MidpointRounding.AwayFromZero) * factor;
    }

    public static double CeilToDecimalPlaces(double x, int n)
    {
        if (n >= 0)
        {
            var factor = Math.Pow(10, n);
            return Math.Ceiling(x * factor) / factor;
        }

        var negativeFactor = Math.Pow(10, -n);
        return Math.Ceiling(x / negativeFactor) * negativeFactor;
    }

    public static double FloorToDecimalPlaces(double x, int n)
    {
        if (n >= 0)
        {
            var factor = Math.Pow(10, n);
            return Math.Floor(x * factor) / factor;
        }

        var negativeFactor = Math.Pow(10, -n);
        return Math.Floor(x / negativeFactor) * negativeFactor;
    }

    public static string FormatNumber(double value)
    {
        if (Roundable(value))
        {
            return Math.Round(value).ToString(CultureInfo.InvariantCulture);
        }

        return value.ToString("g", CultureInfo.InvariantCulture);
    }
}
