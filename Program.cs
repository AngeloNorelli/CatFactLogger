using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CatFactLogger.Services;

var builder = Host.CreateApplicationBuilder(args);
var apiBaseUrl = builder.Configuration["CatFactApi:BaseUrl"] 
  ?? throw new InvalidOperationException("CatFactApi:BaseUrl is not configured in appsettings.json.");

builder.Services.AddHttpClient<ICatFactApiClient, CatFactApiClient>(client =>
{
  client.BaseAddress = new Uri(apiBaseUrl);
  client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddSingleton<IFactFileWriter, FactFileWriter>();
builder.Services.AddHostedService<CatFactAppService>();

var host = builder.Build();
await host.RunAsync();