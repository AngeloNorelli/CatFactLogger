using CatFactLogger.Models;
using Microsoft.Extensions.Configuration;

namespace CatFactLogger.Services;

public class FactFileWriter : IFactFileWriter
{
  private readonly SemaphoreSlim _lock = new(1, 1);
  public string FilePath { get; set; }

  public FactFileWriter(IConfiguration configuration)
  {
    FilePath = configuration["OutputFile:Path"] ?? "cat_facts.txt";

    if (!File.Exists(FilePath))
    {
      File.Create(FilePath).Dispose();
    }
  }

  public async Task AppendFactAsync(CatFact fact, CancellationToken cancellationToken = default)
  {
    var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | length={fact.Length, -3} | fact={fact.Fact}";

    await _lock.WaitAsync(cancellationToken);
    try
    {
      await File.AppendAllTextAsync(FilePath, line + Environment.NewLine, cancellationToken);
    }
    finally
    {
      _lock.Release();
    }
  }
}