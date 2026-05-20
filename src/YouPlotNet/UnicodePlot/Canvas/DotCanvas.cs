namespace YouPlotNet.UnicodePlot.Canvas;

public sealed class DotCanvas : LookupCanvas
{
    private static readonly int[][] DotSigns = [[0b10, 0b01]];
    private static readonly string[] DotDecode = [" ", ".", "'", ":"];

    public DotCanvas(int width, int height, double originX = 0, double originY = 0, double plotWidth = 1, double plotHeight = 1)
        : base(width, height, 1, 2, 0, originX, originY, plotWidth, plotHeight)
    {
    }

    protected override int LookupEncode(int x, int y) => DotSigns[x][y];
    protected override string LookupDecode(int code) => DotDecode[code];
}
