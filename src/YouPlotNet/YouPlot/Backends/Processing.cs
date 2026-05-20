namespace YouPlotNet.YouPlot.Backends;

public static class Processing
{
    public static List<List<string?>> CountValues(IEnumerable<string?> values, bool reverse = false)
    {
        var grouped = values
            .GroupBy(static value => value)
            .Select(static group => new KeyValuePair<string?, int>(group.Key, group.Count()))
            .OrderByDescending(static pair => pair.Value)
            .ThenBy(static pair => pair.Key, StringComparer.Ordinal)
            .ToList();
        if (reverse)
        {
            grouped.Reverse();
        }

        return
        [
            grouped.Select(static pair => pair.Key).ToList(),
            grouped.Select(static pair => (string?)pair.Value.ToString(CultureInfo.InvariantCulture)).ToList()
        ];
    }
}
