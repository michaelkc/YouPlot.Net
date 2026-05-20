namespace YouPlotNet.UnicodePlot.Canvas;

public abstract class Canvas
{
    protected readonly int[] Colors;

    protected Canvas(int width, int height, int pixelWidth, int pixelHeight, double originX = 0, double originY = 0, double plotWidth = 1, double plotHeight = 1, int xPixelPerChar = 1, int yPixelPerChar = 1)
    {
        Width = width;
        Height = height;
        PixelWidth = CheckPositive(pixelWidth, nameof(pixelWidth));
        PixelHeight = CheckPositive(pixelHeight, nameof(pixelHeight));
        OriginX = originX;
        OriginY = originY;
        PlotWidth = plotWidth;
        PlotHeight = plotHeight;
        XPixelPerChar = xPixelPerChar;
        YPixelPerChar = yPixelPerChar;
        Colors = Enumerable.Repeat(StyledPrinter.ColorEncode["normal"], width * height).ToArray();
    }

    public int Width { get; }
    public int Height { get; }
    public int PixelWidth { get; }
    public int PixelHeight { get; }
    public double OriginX { get; }
    public double OriginY { get; }
    public double PlotWidth { get; }
    public double PlotHeight { get; }
    public int XPixelPerChar { get; }
    public int YPixelPerChar { get; }

    public static Canvas Create(string canvasType, int width, int height, double originX = 0, double originY = 0, double plotWidth = 1, double plotHeight = 1) =>
        canvasType.ToLowerInvariant() switch
        {
            "ascii" => new AsciiCanvas(width, height, originX, originY, plotWidth, plotHeight),
            "block" => new BlockCanvas(width, height, originX, originY, plotWidth, plotHeight),
            "braille" => new BrailleCanvas(width, height, originX, originY, plotWidth, plotHeight),
            "density" => new DensityCanvas(width, height, originX, originY, plotWidth, plotHeight),
            "dot" => new DotCanvas(width, height, originX, originY, plotWidth, plotHeight),
            _ => throw new ArgumentException($"unknown canvas type: {canvasType}")
        };

    public int IndexAt(int x, int y) => x < 0 || x >= Width || y < 0 || y >= Height ? -1 : (y * Width) + x;

    public int ColorAt(int x, int y) => Colors[IndexAt(x, y)];

    public void Show(IOContext output)
    {
        var border = BorderMaps.Solid;
        StyledPrinter.PrintStyled(output, false, "light_black", border.Tl, new string(border.T[0], Width), border.Tr);
        output.Puts();
        for (var row = 0; row < Height; row++)
        {
            StyledPrinter.PrintStyled(output, false, "light_black", border.L);
            PrintRow(output, row);
            StyledPrinter.PrintStyled(output, false, "light_black", border.R);
            output.Puts();
        }

        StyledPrinter.PrintStyled(output, false, "light_black", border.Bl, new string(border.B[0], Width), border.Br);
    }

    public void Print(IOContext output)
    {
        for (var row = 0; row < Height; row++)
        {
            PrintRow(output, row);
            if (row < Height - 1)
            {
                output.Puts();
            }
        }
    }

    public string Point(double x, double y, string color = "normal")
    {
        if (!(OriginX <= x && x <= OriginX + PlotWidth && OriginY <= y && y <= OriginY + PlotHeight))
        {
            return color;
        }

        var plotOffsetX = x - OriginX;
        var pixelX = plotOffsetX / PlotWidth * PixelWidth;
        var plotOffsetY = y - OriginY;
        var pixelY = PixelHeight - (plotOffsetY / PlotHeight * PixelHeight);
        return Pixel((int)Math.Floor(pixelX), (int)Math.Floor(pixelY), color);
    }

    public void Points(IEnumerable<double> x, IEnumerable<double> y, string color = "normal")
    {
        var xs = x.ToArray();
        var ys = y.ToArray();
        if (xs.Length != ys.Length)
        {
            throw new ArgumentException("x and y must be the same length");
        }

        if (xs.Length == 0)
        {
            throw new ArgumentException("x and y must not be empty");
        }

        for (var i = 0; i < xs.Length; i++)
        {
            Point(xs[i], ys[i], color);
        }
    }

    public string Line(double x1, double y1, double x2, double y2, string color = "normal")
    {
        if ((x1 < OriginX && x2 < OriginX) || (x1 > OriginX + PlotWidth && x2 > OriginX + PlotWidth))
        {
            return color;
        }

        if ((y1 < OriginY && y2 < OriginY) || (y1 > OriginY + PlotHeight && y2 > OriginY + PlotHeight))
        {
            return color;
        }

        var px1 = ((x1 - OriginX) / PlotWidth) * PixelWidth;
        var px2 = ((x2 - OriginX) / PlotWidth) * PixelWidth;
        var py1 = PixelHeight - (((y1 - OriginY) / PlotHeight) * PixelHeight);
        var py2 = PixelHeight - (((y2 - OriginY) / PlotHeight) * PixelHeight);

        var dx = px2 - px1;
        var dy = py2 - py1;
        var nsteps = Math.Max(Math.Abs(dx), Math.Abs(dy));
        if (nsteps == 0)
        {
            Pixel((int)Math.Floor(px1), (int)Math.Floor(py1), color);
            return color;
        }

        var incX = dx / nsteps;
        var incY = dy / nsteps;
        var curX = px1;
        var curY = py1;
        Pixel((int)Math.Floor(curX), (int)Math.Floor(curY), color);
        for (var i = 1; i <= (int)nsteps; i++)
        {
            curX += incX;
            curY += incY;
            Pixel((int)Math.Floor(curX), (int)Math.Floor(curY), color);
        }

        return color;
    }

    public void Lines(IEnumerable<double> x, IEnumerable<double> y, string color = "normal")
    {
        var xs = x.ToArray();
        var ys = y.ToArray();
        if (xs.Length != ys.Length)
        {
            throw new ArgumentException("x and y must be the same length");
        }

        if (xs.Length == 0)
        {
            throw new ArgumentException("x and y must not be empty");
        }

        for (var i = 0; i < xs.Length - 1; i++)
        {
            Line(xs[i], ys[i], xs[i + 1], ys[i + 1], color);
        }
    }

    public abstract string Pixel(int pixelX, int pixelY, string color = "normal");
    public abstract void PrintRow(IOContext output, int rowIndex);

    protected static int CheckPositive(int value, string name) => value > 0 ? value : throw new ArgumentException($"{name} has to be positive");
    protected static int GetColorCode(string color) => StyledPrinter.ColorEncode.TryGetValue(color, out var code) ? code : StyledPrinter.ColorEncode["normal"];
}
