using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CatFactLogger.Services;

public class CatFactAppService : BackgroundService
{
  private readonly ICatFactApiClient _apiClient;
  private readonly IFactFileWriter _fileWriter;
  private readonly IHostApplicationLifetime _lifetime;

  public CatFactAppService(
    ICatFactApiClient apiClient,
    IFactFileWriter fileWriter,
    IHostApplicationLifetime lifetime,
    ILogger<CatFactAppService> logger)
  {
    _apiClient = apiClient;
    _fileWriter = fileWriter;
    _lifetime = lifetime;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    Console.WriteLine("=== Cat Fact Logger ===");
    Console.WriteLine($"Output file: {Path.GetFullPath(_fileWriter.FilePath)}");
    Console.WriteLine();

    while (!stoppingToken.IsCancellationRequested)
    {
      Console.WriteLine("Press ENTER to fetch a new cat fact or type 'exit' to quit.");
      var input = Console.ReadLine();

      if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
      {
        break;
      }

      var fact = await _apiClient.GetRandomFactAsync(stoppingToken);
      if (fact is null)
      {
        Console.WriteLine("Failed to fetch a cat fact.");
        Console.WriteLine();
        continue;
      }

      await _fileWriter.AppendFactAsync(fact, stoppingToken);

      Console.WriteLine($"[OK] \"{fact.Fact}\" (length: {fact.Length})");
      Console.WriteLine();
    }

    Console.WriteLine("Exiting...");
    _lifetime.StopApplication();
  }
}