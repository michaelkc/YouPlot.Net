namespace YouPlotNet.YouPlot;

public sealed record Data(string?[]? Headers, List<List<string?>> Series);
