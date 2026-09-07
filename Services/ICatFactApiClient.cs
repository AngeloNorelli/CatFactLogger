using CatFactLogger.Models;

namespace CatFactLogger.Services;

public interface ICatFactApiClient
{
  Task<CatFact?> GetRandomFactAsync(CancellationToken cancellationToken = default);
}