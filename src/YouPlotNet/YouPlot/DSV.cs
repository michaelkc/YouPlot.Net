namespace YouPlotNet.YouPlot;

public static class DSV
{
    public static Data Parse(string input, string delimiter, bool headers, bool transpose)
    {
        var rows = ParseRows(input, delimiter);
        rows.RemoveAll(static row => row.Count == 0 || row.All(static value => value is null or ""));
        var headerRow = GetHeaders(rows, headers, transpose);
        var series = GetSeries(rows, headers, transpose);
        return new Data(headerRow, series);
    }

    public static List<List<T?>> Transpose2<T>(IReadOnlyList<IReadOnlyList<T?>> arr)
    {
        var max = arr.Count == 0 ? 0 : arr.Max(static row => row.Count);
        var result = new List<List<T?>>(max);
        for (var i = 0; i < max; i++)
        {
            result.Add(arr.Select(row => i < row.Count ? row[i] : default).ToList());
        }
        return result;
    }

    public static string?[]? GetHeaders(List<List<string?>> arr, bool headers, bool transpose)
    {
        if (!headers) return null;
        return transpose ? arr.Select(static row => row.FirstOrDefault()).ToArray() : arr[0].ToArray();
    }

    public static List<List<string?>> GetSeries(List<List<string?>> arr, bool headers, bool transpose)
    {
        if (!headers)
        {
            return transpose ? arr.Select(static row => row.ToList()).ToList() : Transpose2<string>(arr).Select(static row => row.ToList()).ToList();
        }

        if (arr.Count == 1)
        {
            return Enumerable.Range(0, arr[0].Count).Select(static _ => new List<string?>()).ToList();
        }

        return transpose
            ? arr.Select(static row => row.Skip(1).ToList()).ToList()
            : Transpose2<string>(arr.Skip(1).Select(static row => (IReadOnlyList<string?>)row).ToList()).Select(static row => row.ToList()).ToList();
    }

    private static List<List<string?>> ParseRows(string input, string delimiter)
    {
        var rows = new List<List<string?>>();
        var row = new List<string?>();
        var field = new StringBuilder();
        var inQuotes = false;
        var i = 0;
        while (i < input.Length)
        {
            var c = input[i];
            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < input.Length && input[i + 1] == '"')
                    {
                        field.Append('"');
                        i += 2;
                        continue;
                    }
                    inQuotes = false;
                    i++;
                    continue;
                }

                field.Append(c);
                i++;
                continue;
            }

            if (c == '"')
            {
                inQuotes = true;
            }
            else if (input.AsSpan(i).StartsWith(delimiter, StringComparison.Ordinal))
            {
                row.Add(field.ToString());
                field.Clear();
                i += delimiter.Length;
                continue;
            }
            else if (c == '\r')
            {
            }
            else if (c == '\n')
            {
                row.Add(field.ToString());
                field.Clear();
                rows.Add(row);
                row = new List<string?>();
                i++;
                continue;
            }
            else
            {
                field.Append(c);
            }

            i++;
        }

        if (field.Length > 0 || row.Count > 0)
        {
            row.Add(field.ToString());
            rows.Add(row);
        }

        return rows;
    }
}
