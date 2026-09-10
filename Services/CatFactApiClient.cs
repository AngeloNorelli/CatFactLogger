using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using CatFactLogger.Models;

namespace CatFactLogger.Services;

public class CatFactApiClient(
  HttpClient httpClient, 
  ILogger<CatFactApiClient> logger) : ICatFactApiClient
{

  public async Task<CatFact?> GetRandomFactAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      var response = await httpClient.GetFromJsonAsync<CatFact>("fact", cancellationToken);
      return response;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error fetching cat fact from API.");
      return null;
    }
  }
}