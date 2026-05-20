namespace YouPlotNet.UnicodePlot;

public sealed class IOContext
{
    private readonly TextWriter _writer;
    private readonly bool? _colorOverride;

    public IOContext(TextWriter writer, bool? color = null)
    {
        _writer = writer;
        _colorOverride = color;
    }

    public bool IsColor
    {
        get
        {
            if (_colorOverride.HasValue)
            {
                return _colorOverride.Value;
            }

            if (ReferenceEquals(_writer, Console.Out))
            {
                return !Console.IsOutputRedirected;
            }

            if (ReferenceEquals(_writer, Console.Error))
            {
                return !Console.IsErrorRedirected;
            }

            return false;
        }
    }

    public void Print(params object?[] values) => _writer.Write(string.Concat(values.Select(static v => v?.ToString() ?? string.Empty)));

    public void Puts(params object?[] values)
    {
        if (values.Length == 0)
        {
            _writer.WriteLine();
            return;
        }

        _writer.WriteLine(string.Concat(values.Select(static v => v?.ToString() ?? string.Empty)));
    }
}
