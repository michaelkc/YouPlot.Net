using YouPlotNet.YouPlot;

Console.OutputEncoding = System.Text.Encoding.UTF8;
//Console.Error.AutoFlush = true;

try
{
    new Command(args).Run();
}
catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
{
    Console.Error.WriteLine($"uplot: {ex.Message}");
    Environment.Exit(1);
}
