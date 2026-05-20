namespace YouPlotNet.UnicodePlot.Canvas;

public sealed class DensityCanvas : Canvas
{
    private static readonly string[] DensitySigns = [" ", "░", "▒", "▓", "█"];
    private readonly int[] _grid;
    private int _maxDensity = 1;

    public DensityCanvas(int width, int height, double originX = 0, double originY = 0, double plotWidth = 1, double plotHeight = 1)
        : base(Math.Max(width, 5), Math.Max(height, 5), Math.Max(width, 5), Math.Max(height, 5) * 2, originX, originY, plotWidth, plotHeight, 1, 2)
    {
        _grid = new int[Width * Height];
    }

    public int CharAt(int x, int y) => _grid[IndexAt(x, y)];

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

        var charX = (int)Math.Floor((double)pixelX / PixelWidth * Width);
        var charY = (int)Math.Floor((double)pixelY / PixelHeight * Height);
        var index = IndexAt(charX, charY);
        if (index >= 0)
        {
            _grid[index] += 1;
            _maxDensity = Math.Max(_maxDensity, _grid[index]);
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

        var scale = (DensitySigns.Length - 1d) / _maxDensity;
        for (var x = 0; x < Width; x++)
        {
            var densityIndex = (int)Math.Round(CharAt(x, rowIndex) * scale, MidpointRounding.AwayFromZero);
            StyledPrinter.PrintColor(output, ColorAt(x, rowIndex), DensitySigns[densityIndex]);
        }
    }
}
