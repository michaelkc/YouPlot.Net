namespace YouPlotNet.UnicodePlot.Canvas;

public sealed class BrailleCanvas : Canvas
{
    private static readonly int[][] BrailleSigns =
    [
        [0x2801, 0x2802, 0x2804, 0x2840],
        [0x2808, 0x2810, 0x2820, 0x2880]
    ];

    private readonly char[] _grid;

    public BrailleCanvas(int width, int height, double originX = 0, double originY = 0, double plotWidth = 1, double plotHeight = 1)
        : base(width, height, width * 2, height * 4, originX, originY, plotWidth, plotHeight, 2, 4)
    {
        _grid = Enumerable.Repeat('\u2800', width * height).ToArray();
    }

    public char CharAt(int x, int y) => _grid[IndexAt(x, y)];

    public override string Pixel(int pixelX, int pixelY, string color = "normal")
    {
        if (!(0 <= pixelX && pixelX <= PixelWidth && 0 <= pixelY && pixelY <= PixelHeight))
        {
            return color;
        }

        if (pixelX >= PixelWidth)
        {
            pixelX -= 1;
        }

        if (pixelY >= PixelHeight)
        {
            pixelY -= 1;
        }

        var tx = (double)pixelX / PixelWidth * Width;
        var charX = (int)Math.Floor(tx) + 1;
        var charXOff = (pixelX % XPixelPerChar) + 1;
        if (charX < (int)Math.Round(tx, MidpointRounding.AwayFromZero) + 1 && charXOff == 1)
        {
            charX += 1;
        }

        var charYOff = (pixelY % YPixelPerChar) + 1;
        var charY = ((pixelY - (charYOff - 1)) / YPixelPerChar) + 1;
        var index = IndexAt(charX - 1, charY - 1);
        if (index >= 0)
        {
            _grid[index] = (char)(_grid[index] | BrailleSigns[charXOff - 1][charYOff - 1]);
            Colors[index] |= GetColorCode(color);
        }

        return color;
    }

    public override void PrintRow(IOContext output, int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= Height)
        {
            throw new ArgumentException("row_index out of bounds");
        }

        for (var x = 0; x < Width; x++)
        {
            StyledPrinter.PrintColor(output, ColorAt(x, rowIndex), CharAt(x, rowIndex).ToString());
        }
    }
}
