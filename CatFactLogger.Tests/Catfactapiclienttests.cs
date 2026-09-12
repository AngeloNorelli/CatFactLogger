using System.Net;
using System.Text;
using System.Text.Json;
using CatFactLogger.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CatFactLogger.Tests;

public class CatFactApiClientTests	
{
	private sealed class FakeHttpMessageHandler(
		HttpStatusCode statusCode,
		string? content,
		bool throwException = false) : HttpMessageHandler 
	{
		protected override Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request,
			CancellationToken cancellationToken) 
		{
			if (throwException) 
			{
				throw new HttpRequestException("Fake web error.");
			}

			var response = new HttpResponseMessage(statusCode) 
			{
				Content = content is null
					? null
					: new StringContent(content, Encoding.UTF8, "application/json")
			};
			return Task.FromResult(response);
		}
	}

	private static CatFactApiClient CreateClient(HttpMessageHandler handler) 
	{
		var httpClient = new HttpClient(handler)
		{
			BaseAddress = new Uri("https://catfact.ninja")
		};
		return new CatFactApiClient(httpClient, NullLogger<CatFactApiClient>.Instance);
	}

	[Theory]
	[InlineData("Cats sleep a lot.", 17)]
	[InlineData("Cats love sleeping", 19)]
	[InlineData("Meow", 5)]
	public async Task GetRandomFactAsync_ReturnsDeserializedFact_OnSuccessResponse(
		string fact,
		int length) 
	{
		var json = JsonSerializer.Serialize(new {fact, length});
		var client = CreateClient(new FakeHttpMessageHandler(HttpStatusCode.OK, json));

		var result = await client.GetRandomFactAsync();

		Assert.NotNull(result);
		Assert.Equal(length, result.Length);
		Assert.Equal(fact, result.Fact);
	}

	[Fact]
	public async Task GetRandomFactAsync_ReturnsNull_WhenHttpRequestFails() 
	{
		var client = CreateClient(new FakeHttpMessageHandler(HttpStatusCode.OK, null, throwException: true));

		var result = await client.GetRandomFactAsync();
		Assert.Null(result);
	}
}
