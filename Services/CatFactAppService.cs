using Microsoft.Extensions.Hosting;
using CatFactLogger.Models;

namespace CatFactLogger.Services;

public class CatFactAppService(
    ICatFactApiClient apiClient,
    IFactFileWriter fileWriter,
    IHostApplicationLifetime lifetime) : BackgroundService
{
  private readonly List<CatFact> _facts = [];

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      Render();
      
      var input = Console.ReadLine();
      if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
      {
        break;
      }

      var fact = await apiClient.GetRandomFactAsync(stoppingToken);
      if (fact is null)
      {
        continue;
      }

      await fileWriter.AppendFactAsync(fact, stoppingToken);

      _facts.Add(fact);
    }

    Console.Clear();
    Console.WriteLine("Exiting Cat Fact Logger...");
    lifetime.StopApplication();
  }

  private void Render()
  {
    Console.Clear();

    const int width = 70;
    
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╭" + new string('─', width) + "╮");
    Console.WriteLine("│" + Center("CAT FACT LOGGER", width) + "│");
    Console.WriteLine("├" + new string('─', width) + "┤");
    Console.ResetColor();
    Console.WriteLine("│" + new string(' ', width) + "│");

    if (_facts.Count == 0)
    {
      Console.WriteLine(
        "│" + Center("No cat facts yet...", width) + "│"
      );
    }
    else
    {
      foreach (var fact in _facts)
      {
        PrintFact(fact, width);
      }
    }

    Console.WriteLine("│" + new string(' ', width) + "│");

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine(
      "|" + Pad($"Output: {Path.GetFullPath(fileWriter.FilePath)}", width) + "|"
    );
    Console.WriteLine("|" + new string(' ', width) + "|");

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine(
      "|" + Pad("Press ENTER to fetch a new cat fact", width) + "|"
    );
    Console.WriteLine(
      "|" + Pad("Type 'exit' to quit the application", width) + "|"
    );

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("╰" + new string('─', width) + "╯");
    Console.ResetColor();
  }

  private void PrintFact(CatFact fact, int width)
  {
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("|" + Pad(fact.Fact, width) + "|");

    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine(
      "|" + Pad($"length: {fact.Length}", width) + "|"
    );
    Console.WriteLine(
      "|" + Pad("      " + new string('-', Math.Min(width - 6, 55)), width) + "|"
    );
    
    Console.ResetColor();
  }

  private static string Pad(string v, int width)
  {
    if(v.Length > width)
    {
      v = v[..(width - 5)] + "...";
    }
    return " " + v.PadRight(width - 1);
  }

  private static string Center(string v, int width)
  {
    if(v.Length >= width)
    {
      return v[..width];
    }
    
    var left = (width - v.Length) / 2;

    return new string(' ', left) + v + new string(' ', width - v.Length - left);
  }
}