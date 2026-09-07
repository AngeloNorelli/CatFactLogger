using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using CatFactLogger.Models;

namespace CatFactLogger.Services;

public class CatFactApiClient : ICatFactApiClient
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<CatFactApiClient> _logger;

  public CatFactApiClient(HttpClient httpClient, ILogger<CatFactApiClient> logger)
  {
    _httpClient = httpClient;
    _logger = logger;
  }

  public async Task<CatFact?> GetRandomFactAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      var response = await _httpClient.GetFromJsonAsync<CatFact>("fact", cancellationToken);
      return response;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error fetching cat fact from API.");
      return null;
    }
  }
}