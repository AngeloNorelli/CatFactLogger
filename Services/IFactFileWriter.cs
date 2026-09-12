using CatFactLogger.Models;

namespace CatFactLogger.Services;

public interface IFactFileWriter
{
  string FilePath { get; set; }

  Task AppendFactAsync(CatFact fact, CancellationToken cancellationToken = default);
  Task<IReadOnlyList<string>> ReadAllLinesAsync(CancellationToken cancellationToken = default);
}