namespace YouPlotNet.UnicodePlot;

public static class ValueTransformer
{
    public static readonly Dictionary<string, Func<double, double>> PredefinedTransforms = new(StringComparer.OrdinalIgnoreCase)
    {
        ["log"] = Math.Log,
        ["ln"] = Math.Log,
        ["log10"] = Math.Log10,
        ["lg"] = Math.Log10,
        ["log2"] = Math.Log2,
        ["lb"] = Math.Log2,
    };

    public static double TransformValue(string? func, double value)
    {
        if (string.IsNullOrWhiteSpace(func))
        {
            return value;
        }

        return PredefinedTransforms.TryGetValue(func, out var transformer)
            ? transformer(value)
            : value;
    }

    public static double[] TransformValues(string? func, IEnumerable<double> values)
    {
        if (string.IsNullOrWhiteSpace(func))
        {
            return values.ToArray();
        }

        return values.Select(value => TransformValue(func, value)).ToArray();
    }

    public static string TransformName(string? func, string basename = "")
    {
        if (string.IsNullOrWhiteSpace(func))
        {
            return basename;
        }

        return $"{basename} [{func}]";
    }
}
