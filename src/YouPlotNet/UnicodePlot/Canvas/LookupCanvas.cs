namespace YouPlotNet.UnicodePlot.Canvas;

public abstract class LookupCanvas : Canvas
{
    protected readonly int[] Grid;

    protected LookupCanvas(int width, int height, int xPixelPerChar, int yPixelPerChar, int fillChar = 0, double originX = 0, double originY = 0, double plotWidth = 1, double plotHeight = 1)
        : base(width, height, width * xPixelPerChar, height * yPixelPerChar, originX, originY, plotWidth, plotHeight, xPixelPerChar, yPixelPerChar)
    {
        Grid = Enumerable.Repeat(fillChar, width * height).ToArray();
    }

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
            Grid[index] |= LookupEncode(charXOff - 1, charYOff - 1);
            Colors[index] |= GetColorCode(color);
        }

        return color;
    }

    public int CharAt(int x, int y) => Grid[IndexAt(x, y)];

    public override void PrintRow(IOContext output, int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= Height)
        {
            throw new ArgumentException("row_index out of bounds");
        }

        for (var x = 0; x < Width; x++)
        {
            StyledPrinter.PrintColor(output, ColorAt(x, rowIndex), LookupDecode(CharAt(x, rowIndex)));
        }
    }

    protected abstract int LookupEncode(int x, int y);
    protected abstract string LookupDecode(int code);
}
