// Port of ruby_source/YouPlot/test/youplot/dsv_test.rb
namespace YouPlotNet.Tests.YouPlot;

public class DsvTests
{
    // ── Transpose2 ────────────────────────────────────────────────────────────

    [Fact]
    public void Transpose2_Square()
    {
        var input = new List<IReadOnlyList<int?>>
        {
            new int?[] { 1, 4, 7 },
            new int?[] { 2, 5, 8 },
            new int?[] { 3, 6, 9 }
        };
        var expected = new List<List<int?>>
        {
            new() { 1, 2, 3 },
            new() { 4, 5, 6 },
            new() { 7, 8, 9 }
        };
        Assert.Equal(expected, DSV.Transpose2(input));
    }

    [Fact]
    public void Transpose2_RaggedLongerFirst()
    {
        var input = new List<IReadOnlyList<int?>>
        {
            new int?[] { 1, 4, 6 },
            new int?[] { 2, 5 },
            new int?[] { 3 }
        };
        var expected = new List<List<int?>>
        {
            new() { 1, 2, 3 },
            new() { 4, 5, null },
            new() { 6, null, null }
        };
        Assert.Equal(expected, DSV.Transpose2(input));
    }

    [Fact]
    public void Transpose2_RaggedLongerLast()
    {
        var input = new List<IReadOnlyList<int?>>
        {
            new int?[] { 1 },
            new int?[] { 2, 4 },
            new int?[] { 3, 5, 6 }
        };
        var expected = new List<List<int?>>
        {
            new() { 1, 2, 3 },
            new() { null, 4, 5 },
            new() { null, null, 6 }
        };
        Assert.Equal(expected, DSV.Transpose2(input));
    }

    // ── GetHeaders ───────────────────────────────────────────────────────────

    [Fact]
    public void GetHeaders_Headers_True_Transpose_True_Square()
    {
        var arr = new List<List<string?>> { new() { "1", "2", "3" }, new() { "4", "5", "6" }, new() { "7", "8", "9" } };
        Assert.Equal(new string?[] { "1", "4", "7" }, DSV.GetHeaders(arr, true, true));
    }

    [Fact]
    public void GetHeaders_Headers_True_Transpose_True_RaggedLongerFirst()
    {
        var arr = new List<List<string?>> { new() { "1", "4", "6" }, new() { "2", "5" }, new() { "3" } };
        Assert.Equal(new string?[] { "1", "2", "3" }, DSV.GetHeaders(arr, true, true));
    }

    [Fact]
    public void GetHeaders_Headers_True_Transpose_True_RaggedLongerLast()
    {
        var arr = new List<List<string?>> { new() { "1" }, new() { "2", "4" }, new() { "3", "5", "6" } };
        Assert.Equal(new string?[] { "1", "2", "3" }, DSV.GetHeaders(arr, true, true));
    }

    [Fact]
    public void GetHeaders_Headers_True_Transpose_False_Square()
    {
        var arr = new List<List<string?>> { new() { "1", "2", "3" }, new() { "4", "5", "6" }, new() { "7", "8", "9" } };
        Assert.Equal(new string?[] { "1", "2", "3" }, DSV.GetHeaders(arr, true, false));
    }

    [Fact]
    public void GetHeaders_Headers_True_Transpose_False_RaggedLongerFirst()
    {
        var arr = new List<List<string?>> { new() { "1", "4", "6" }, new() { "2", "5" }, new() { "3" } };
        Assert.Equal(new string?[] { "1", "4", "6" }, DSV.GetHeaders(arr, true, false));
    }

    [Fact]
    public void GetHeaders_Headers_True_Transpose_False_RaggedLongerLast()
    {
        var arr = new List<List<string?>> { new() { "1" }, new() { "2", "4" }, new() { "3", "5", "6" } };
        Assert.Equal(new string?[] { "1" }, DSV.GetHeaders(arr, true, false));
    }

    [Fact]
    public void GetHeaders_Headers_False_Transpose_True()
    {
        var arr = new List<List<string?>> { new() { "1", "2", "3" }, new() { "4", "5", "6" }, new() { "7", "8", "9" } };
        Assert.Null(DSV.GetHeaders(arr, false, true));
    }

    [Fact]
    public void GetHeaders_Headers_False_Transpose_False()
    {
        var arr = new List<List<string?>> { new() { "1", "2", "3" }, new() { "4", "5", "6" }, new() { "7", "8", "9" } };
        Assert.Null(DSV.GetHeaders(arr, false, false));
    }

    [Fact]
    public void GetHeaders_SingleRow()
    {
        var arr = new List<List<string?>> { new() { "1", "2", "3" } };
        Assert.Equal(new string?[] { "1", "2", "3" }, DSV.GetHeaders(arr, true, false));
    }

    // ── GetSeries ─────────────────────────────────────────────────────────────

    [Fact]
    public void GetSeries_Headers_True_Transpose_True_Square()
    {
        var arr = new List<List<string?>> { new() { "1", "2", "3" }, new() { "4", "5", "6" }, new() { "7", "8", "9" } };
        var expected = new List<List<string?>> { new() { "2", "3" }, new() { "5", "6" }, new() { "8", "9" } };
        Assert.Equal(expected, DSV.GetSeries(arr, true, true));
    }

    [Fact]
    public void GetSeries_Headers_True_Transpose_True_RaggedLongerFirst()
    {
        var arr = new List<List<string?>> { new() { "1", "4", "6" }, new() { "2", "5" }, new() { "3" } };
        var expected = new List<List<string?>> { new() { "4", "6" }, new() { "5" }, new() { } };
        Assert.Equal(expected, DSV.GetSeries(arr, true, true));
    }

    [Fact]
    public void GetSeries_Headers_True_Transpose_True_RaggedLongerLast()
    {
        var arr = new List<List<string?>> { new() { "1" }, new() { "2", "4" }, new() { "3", "5", "6" } };
        var expected = new List<List<string?>> { new() { }, new() { "4" }, new() { "5", "6" } };
        Assert.Equal(expected, DSV.GetSeries(arr, true, true));
    }

    [Fact]
    public void GetSeries_Headers_True_Transpose_False_Square()
    {
        var arr = new List<List<string?>> { new() { "1", "2", "3" }, new() { "4", "5", "6" }, new() { "7", "8", "9" } };
        var expected = new List<List<string?>> { new() { "4", "7" }, new() { "5", "8" }, new() { "6", "9" } };
        Assert.Equal(expected, DSV.GetSeries(arr, true, false));
    }

    [Fact]
    public void GetSeries_Headers_True_Transpose_False_RaggedLongerFirst()
    {
        var arr = new List<List<string?>> { new() { "1", "4", "6" }, new() { "2", "5" }, new() { "3" } };
        var expected = new List<List<string?>> { new() { "2", "3" }, new() { "5", null } };
        Assert.Equal(expected, DSV.GetSeries(arr, true, false));
    }

    [Fact]
    public void GetSeries_Headers_True_Transpose_False_RaggedLongerLast()
    {
        var arr = new List<List<string?>> { new() { "1" }, new() { "2", "4" }, new() { "3", "5", "6" } };
        var expected = new List<List<string?>> { new() { "2", "3" }, new() { "4", "5" }, new() { null, "6" } };
        Assert.Equal(expected, DSV.GetSeries(arr, true, false));
    }

    [Fact]
    public void GetSeries_Headers_False_Transpose_True_Square()
    {
        var arr = new List<List<string?>> { new() { "1", "2", "3" }, new() { "4", "5", "6" }, new() { "7", "8", "9" } };
        var expected = new List<List<string?>> { new() { "1", "2", "3" }, new() { "4", "5", "6" }, new() { "7", "8", "9" } };
        Assert.Equal(expected, DSV.GetSeries(arr, false, true));
    }

    [Fact]
    public void GetSeries_Headers_False_Transpose_True_RaggedLongerFirst()
    {
        var arr = new List<List<string?>> { new() { "1", "4", "6" }, new() { "2", "5" }, new() { "3" } };
        var expected = new List<List<string?>> { new() { "1", "4", "6" }, new() { "2", "5" }, new() { "3" } };
        Assert.Equal(expected, DSV.GetSeries(arr, false, true));
    }

    [Fact]
    public void GetSeries_Headers_False_Transpose_True_RaggedLongerLast()
    {
        var arr = new List<List<string?>> { new() { "1" }, new() { "2", "4" }, new() { "3", "5", "6" } };
        var expected = new List<List<string?>> { new() { "1" }, new() { "2", "4" }, new() { "3", "5", "6" } };
        Assert.Equal(expected, DSV.GetSeries(arr, false, true));
    }

    [Fact]
    public void GetSeries_Headers_False_Transpose_False_Square()
    {
        var arr = new List<List<string?>> { new() { "1", "2", "3" }, new() { "4", "5", "6" }, new() { "7", "8", "9" } };
        var expected = new List<List<string?>> { new() { "1", "4", "7" }, new() { "2", "5", "8" }, new() { "3", "6", "9" } };
        Assert.Equal(expected, DSV.GetSeries(arr, false, false));
    }

    [Fact]
    public void GetSeries_Headers_False_Transpose_False_RaggedLongerFirst()
    {
        var arr = new List<List<string?>> { new() { "1", "4", "6" }, new() { "2", "5" }, new() { "3" } };
        var expected = new List<List<string?>> { new() { "1", "2", "3" }, new() { "4", "5", null }, new() { "6", null, null } };
        Assert.Equal(expected, DSV.GetSeries(arr, false, false));
    }

    [Fact]
    public void GetSeries_Headers_False_Transpose_False_RaggedLongerLast()
    {
        var arr = new List<List<string?>> { new() { "1" }, new() { "2", "4" }, new() { "3", "5", "6" } };
        var expected = new List<List<string?>> { new() { "1", "2", "3" }, new() { null, "4", "5" }, new() { null, null, "6" } };
        Assert.Equal(expected, DSV.GetSeries(arr, false, false));
    }

    [Fact]
    public void GetSeries_SingleRowWithHeaders_ReturnsEmptyLists()
    {
        var arr = new List<List<string?>> { new() { "1", "2", "3" } };
        var expected = new List<List<string?>> { new(), new(), new() };
        Assert.Equal(expected, DSV.GetSeries(arr, true, false));
    }
}
