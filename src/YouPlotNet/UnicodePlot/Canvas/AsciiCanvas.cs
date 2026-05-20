using System.Numerics;

namespace YouPlotNet.UnicodePlot.Canvas;

public sealed class AsciiCanvas : LookupCanvas
{
    private static readonly int[][] AsciiSigns =
    [
        [0b100_000_000, 0b000_100_000, 0b000_000_100],
        [0b010_000_000, 0b000_010_000, 0b000_000_010],
        [0b001_000_000, 0b000_001_000, 0b000_000_001]
    ];

    private static readonly Dictionary<int, string> AsciiLookup = new()
    {
        [0b101_000_000] = "\"", [0b111_111_111] = "@", [0b010_000_000] = "'", [0b010_100_010] = "(", [0b010_001_010] = ")",
        [0b000_010_000] = "*", [0b010_111_010] = "+", [0b000_010_010] = ",", [0b000_100_100] = ",", [0b000_001_001] = ",",
        [0b000_111_000] = "-", [0b000_000_010] = ".", [0b000_000_100] = ".", [0b000_000_001] = ".", [0b001_010_100] = "/",
        [0b010_100_000] = "/", [0b001_010_110] = "/", [0b011_010_010] = "/", [0b001_010_010] = "/", [0b110_010_111] = "1",
        [0b010_000_010] = ":", [0b111_000_111] = "=", [0b111_010_111] = "I", [0b100_100_111] = "L", [0b111_010_010] = "T",
        [0b101_101_010] = "V", [0b101_010_101] = "X", [0b101_010_010] = "Y", [0b110_100_110] = "[", [0b010_001_000] = "\\",
        [0b100_010_001] = "\\", [0b110_010_010] = "\\", [0b100_010_011] = "\\", [0b100_010_010] = "\\", [0b011_001_011] = "]",
        [0b010_101_000] = "^", [0b000_000_111] = "_", [0b100_000_000] = "`", [0b110_010_011] = "l", [0b000_111_100] = "r",
        [0b000_101_010] = "v", [0b011_110_011] = "{", [0b010_010_010] = "|", [0b100_100_100] = "|", [0b001_001_001] = "|",
        [0b110_011_110] = "}"
    };

    private static readonly int[] AsciiLookupKeyOrder = [0x0002, 0x00d2, 0x0113, 0x00a0, 0x0088, 0x002a, 0x0100, 0x0197, 0x0012, 0x0193, 0x0092, 0x0082, 0x008a, 0x0054, 0x0004, 0x01d2, 0x01ff, 0x0124, 0x00a8, 0x0056, 0x0001, 0x01c7, 0x0052, 0x0080, 0x0009, 0x00cb, 0x0007, 0x003c, 0x0111, 0x0140, 0x0024, 0x0127, 0x0192, 0x0010, 0x019e, 0x01a6, 0x01d7, 0x0155, 0x00a2, 0x00ba, 0x0112, 0x0049, 0x00f3, 0x0152, 0x0038, 0x016a];
    private static readonly string[] AsciiDecode = BuildAsciiDecode();

    public AsciiCanvas(int width, int height, double originX = 0, double originY = 0, double plotWidth = 1, double plotHeight = 1)
        : base(width, height, 3, 3, 0, originX, originY, plotWidth, plotHeight)
    {
    }

    protected override int LookupEncode(int x, int y) => AsciiSigns[x][y];
    protected override string LookupDecode(int code) => AsciiDecode[code];

    private static string[] BuildAsciiDecode()
    {
        var decode = new string[0b111_111_111 + 1];
        decode[0] = " ";
        for (var i = 1; i < decode.Length; i++)
        {
            var minKey = AsciiLookupKeyOrder.MinBy(key => BitOperations.PopCount((uint)(i ^ key)));
            decode[i] = AsciiLookup[minKey];
        }

        return decode;
    }
}
