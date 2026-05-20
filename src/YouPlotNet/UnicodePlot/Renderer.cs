namespace YouPlotNet.UnicodePlot;

public sealed class Renderer
{
    private readonly Plot _plot;
    private IOContext? _output;
    private BorderStyle _border = BorderMaps.Solid;
    private int _borderLength;
    private int _maxLenL;
    private int _maxLenR;
    private string _plotPadding = string.Empty;
    private string _borderPadding = string.Empty;
    private int _ylabelRow;

    private Renderer(Plot plot)
    {
        _plot = plot;
    }

    public static void Render(IOContext output, Plot plot, bool newline) => new Renderer(plot).RenderCore(output, newline);

    private void RenderCore(IOContext output, bool newline)
    {
        _output = output;
        InitRender();
        RenderTop();
        for (var row = 0; row < _plot.NRows; row++)
        {
            RenderRow(row);
        }

        RenderBottom();
        if (newline)
        {
            output.Puts();
        }
    }

    private IOContext Out => _output ?? throw new InvalidOperationException();

    private void InitRender()
    {
        _border = BorderMaps.Map[_plot.Border];
        _borderLength = _plot.NColumns;
        _maxLenL = _plot.ShowLabels && _plot.LabelsLeft.Count > 0 ? _plot.LabelsLeft.Values.Max(static value => StyledPrinter.StripAnsi(value).Length) : 0;
        _maxLenR = _plot.ShowLabels && _plot.LabelsRight.Count > 0 ? _plot.LabelsRight.Values.Max(static value => StyledPrinter.StripAnsi(value).Length) : 0;
        if (_plot.ShowLabels && _plot.YlabelGiven)
        {
            _maxLenL += _plot.YlabelLength + 1;
        }

        var plotOffset = _maxLenL + _plot.Margin + _plot.Padding;
        _plotPadding = new string(' ', _plot.Padding);
        _borderPadding = new string(' ', plotOffset);
        _ylabelRow = (int)Math.Round(_plot.NRows / 2d, MidpointRounding.AwayFromZero) - 1;
    }

    private void RenderTop()
    {
        PrintTitle(_borderPadding, _plot.Title, _borderLength, true);
        if (_plot.TitleGiven)
        {
            Out.Puts();
        }

        if (_plot.ShowLabels)
        {
            var tl = _plot.Decorations.GetValueOrDefault("tl", string.Empty);
            var t = _plot.Decorations.GetValueOrDefault("t", string.Empty);
            var tr = _plot.Decorations.GetValueOrDefault("tr", string.Empty);
            if (tl.Length > 0 || t.Length > 0 || tr.Length > 0)
            {
                var tlCol = _plot.ColorsDeco.GetValueOrDefault("tl", "light_black");
                var tCol = _plot.ColorsDeco.GetValueOrDefault("t", "light_black");
                var trCol = _plot.ColorsDeco.GetValueOrDefault("tr", "light_black");
                StyledPrinter.PrintStyled(Out, false, tlCol, _borderPadding, tl);
                var cnt = (int)Math.Round(_borderLength / 2d - t.Length / 2d - tl.Length, MidpointRounding.AwayFromZero);
                StyledPrinter.PrintStyled(Out, false, tCol, new string(' ', Math.Max(cnt, 0)), t);
                cnt = _borderLength - tr.Length - tl.Length - t.Length + 2 - cnt;
                StyledPrinter.PrintStyled(Out, false, trCol, new string(' ', Math.Max(cnt, 0)), tr, "\n");
            }
        }

        StyledPrinter.PrintStyled(Out, false, "light_black", _borderPadding, _border.Tl, _border.T.Length > 0 ? string.Concat(Enumerable.Repeat(_border.T, _borderLength)) : string.Empty, _border.Tr);
        Out.Print(new string(' ', _maxLenR), _plotPadding, "\n");
    }

    private void RenderRow(int row)
    {
        var left = _plot.LabelsLeft.GetValueOrDefault(row, string.Empty);
        var right = _plot.LabelsRight.GetValueOrDefault(row, string.Empty);
        var leftColor = _plot.ColorsLeft.GetValueOrDefault(row, "light_black");
        var rightColor = _plot.ColorsRight.GetValueOrDefault(row, "light_black");
        var leftLen = StyledPrinter.StripAnsi(left).Length;
        var rightLen = StyledPrinter.StripAnsi(right).Length;
        if (!Out.IsColor)
        {
            left = StyledPrinter.StripAnsi(left);
            right = StyledPrinter.StripAnsi(right);
        }

        Out.Print(new string(' ', _plot.Margin));
        if (_plot.ShowLabels)
        {
            if (row == _ylabelRow)
            {
                StyledPrinter.PrintStyled(Out, false, "normal", _plot.Ylabel ?? string.Empty);
                Out.Print(new string(' ', Math.Max(_maxLenL - _plot.YlabelLength - leftLen, 0)));
            }
            else
            {
                Out.Print(new string(' ', Math.Max(_maxLenL - leftLen, 0)));
            }

            StyledPrinter.PrintStyled(Out, false, leftColor, left);
        }

        StyledPrinter.PrintStyled(Out, false, "light_black", _plotPadding, _border.L);
        _plot.PrintRow(Out, row);
        StyledPrinter.PrintStyled(Out, false, "light_black", _border.R);
        if (_plot.ShowLabels)
        {
            Out.Print(_plotPadding);
            StyledPrinter.PrintStyled(Out, false, rightColor, right);
            Out.Print(new string(' ', Math.Max(_maxLenR - rightLen, 0)));
        }

        Out.Puts();
    }

    private void RenderBottom()
    {
        StyledPrinter.PrintStyled(Out, false, "light_black", _borderPadding, _border.Bl, _border.B.Length > 0 ? string.Concat(Enumerable.Repeat(_border.B, _borderLength)) : string.Empty, _border.Br);
        Out.Print(new string(' ', _maxLenR), _plotPadding);
        if (_plot.ShowLabels)
        {
            var bl = _plot.Decorations.GetValueOrDefault("bl", string.Empty);
            var b = _plot.Decorations.GetValueOrDefault("b", string.Empty);
            var br = _plot.Decorations.GetValueOrDefault("br", string.Empty);
            if (bl.Length > 0 || b.Length > 0 || br.Length > 0)
            {
                Out.Puts();
                StyledPrinter.PrintStyled(Out, false, _plot.ColorsDeco.GetValueOrDefault("bl", "light_black"), _borderPadding, bl);
                var cnt = (int)Math.Round(_borderLength / 2d - b.Length / 2d - bl.Length, MidpointRounding.AwayFromZero);
                StyledPrinter.PrintStyled(Out, false, _plot.ColorsDeco.GetValueOrDefault("b", "light_black"), new string(' ', Math.Max(cnt, 0)), b);
                cnt = _borderLength - br.Length - bl.Length - b.Length + 2 - cnt;
                StyledPrinter.PrintStyled(Out, false, _plot.ColorsDeco.GetValueOrDefault("br", "light_black"), new string(' ', Math.Max(cnt, 0)), br);
            }

            if (_plot.XlabelGiven)
            {
                Out.Puts();
            }

            PrintTitle(_borderPadding, _plot.Xlabel, _borderLength, false);
        }
    }

    private void PrintTitle(string padding, string? title, int plotWidth, bool bold)
    {
        if (string.IsNullOrEmpty(title))
        {
            return;
        }

        var offset = (int)Math.Round(plotWidth / 2d - title.Length / 2d, MidpointRounding.AwayFromZero);
        StyledPrinter.PrintStyled(Out, false, bold ? "bold" : "normal", padding, new string(' ', Math.Max(offset, 0)), title);
    }
}
