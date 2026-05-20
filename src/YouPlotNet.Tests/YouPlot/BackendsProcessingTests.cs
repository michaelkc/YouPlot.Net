namespace YouPlotNet.Tests.YouPlot;

public class BackendsProcessingTests
{
    [Fact]
    public void CountValues_Sorts_By_Count_Then_Name()
    {
        var result = Processing.CountValues(["b", "a", "b", "a", "b"]);
        Assert.Equal(["b", "a"], result[0]);
        Assert.Equal(["3", "2"], result[1]);
    }
}
