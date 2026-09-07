using System.Text.Json.Serialization;

namespace CatFactLogger.Models;

public class CatFact
{
  [JsonPropertyName("fact")]
  public string Fact { get; set; } = string.Empty;

  [JsonPropertyName("length")]
  public int Length { get; set; }
}