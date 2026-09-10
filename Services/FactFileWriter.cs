using CatFactLogger.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CatFactLogger.Services;

public class FactFileWriter : IFactFileWriter
{
  private readonly SemaphoreSlim _lock = new(1, 1);
  public string FilePath { get; set; }

  public FactFileWriter(IConfiguration configuration)
  {
    var configured = configuration["OutputFile:Path"];
    if (string.IsNullOrEmpty(configured))
    {
      FilePath = GetDefaultFilePath();
    }
    else 
    {
      FilePath = ResolveConfiguredPath(configured);
    }

    EnsureFileExists(FilePath);
  }

  private static string GetDefaultFilePath() 
  {
    var projectRoot = Path.GetFullPath(
      Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

    return Path.Combine(projectRoot, "cat_facts.txt");
  }

  private static string ResolveConfiguredPath(string configuredPath) 
  {
    configuredPath = Environment.ExpandEnvironmentVariables(configuredPath);

    try 
    {
      var path = Path.IsPathRooted(configuredPath)
        ? configuredPath
        : Path.Combine(
          Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..")),
          configuredPath);

      return Path.GetFullPath(path);
    }
    catch (Exception ex) when (
      ex is ArgumentException ||
      ex is NotSupportedException) 
    {
      throw new InvalidOperationException(
        $"Invalid configuration 'OutputFile:Path': '{configuredPath}'.",
        ex);
    }
  }

  private static void EnsureFileExists(string filePath) 
  {
    try 
    {
      var directory = Path.GetDirectoryName(filePath);
      if (string.IsNullOrWhiteSpace(directory))
      {
        throw new InvalidOperationException(
          $"Invalid configuration 'OutputFile:Path': '{filePath}'." +
          "The path does not contain a valid directory."
          );    
      }

      if (!Directory.Exists(directory)) 
      {
        throw new InvalidOperationException(
          $"Invalid configuration 'OutputFile:Path':" +
          $"The directory '{directory}' does not exist.");
      }

      if (!File.Exists(filePath)) 
      {
        File.Create(filePath).Dispose();
      }
    } 
    catch (UnauthorizedAccessException ex) 
    {
      throw new InvalidOperationException(
        $"Cannot write to configured output file '{filePath}'." +
        "The application does not have sufficient permissions.",
        ex);
    }
    catch (IOException ex) 
    {
      throw new InvalidOperationException(
        $"Cannot create or access output file '{filePath}'." +
        "Check that the path is valid and accessible",
        ex);
    }
  }

  public async Task AppendFactAsync(CatFact fact, CancellationToken cancellationToken = default)
  {
    var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | length={fact.Length} | fact={fact.Fact}";

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