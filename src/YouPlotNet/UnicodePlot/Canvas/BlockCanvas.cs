namespace YouPlotNet.UnicodePlot.Canvas;

public sealed class BlockCanvas : LookupCanvas
{
    private static readonly int[][] BlockSigns =
    [
        [0b1000, 0b0010],
        [0b0100, 0b0001]
    ];

    private static readonly string[] BlockDecode = [" ", "▗", "▖", "▄", "▝", "▐", "▞", "▟", "▘", "▚", "▌", "▙", "▀", "▜", "▛", "█"];

    public BlockCanvas(int width, int height, double originX = 0, double originY = 0, double plotWidth = 1, double plotHeight = 1)
        : base(width, height, 2, 2, 0, originX, originY, plotWidth, plotHeight)
    {
    }

    protected override int LookupEncode(int x, int y) => BlockSigns[x][y];
    protected override string LookupDecode(int code) => BlockDecode[code];
}
