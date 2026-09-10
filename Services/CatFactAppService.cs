using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace CatFactLogger.Services;

public class CatFactAppService(
    ICatFactApiClient apiClient,
    IFactFileWriter fileWriter,
    IHostApplicationLifetime lifetime) : BackgroundService
{

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    Console.WriteLine("=== Cat Fact Logger ===");
    Console.WriteLine($"Output file: {Path.GetFullPath(fileWriter.FilePath)}");
    Console.WriteLine();

    while (!stoppingToken.IsCancellationRequested)
    {
      Console.WriteLine("Press ENTER to fetch a new cat fact or type 'exit' to quit.");
      var input = Console.ReadLine();

      if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
      {
        break;
      }

      var fact = await apiClient.GetRandomFactAsync(stoppingToken);
      if (fact is null)
      {
        Console.WriteLine("Failed to fetch a cat fact.");
        Console.WriteLine();
        continue;
      }

      await fileWriter.AppendFactAsync(fact, stoppingToken);

      Console.WriteLine($"[OK] \"{fact.Fact}\" (length: {fact.Length})");
      Console.WriteLine();
    }

    Console.WriteLine("Exiting...");
    lifetime.StopApplication();
  }
}