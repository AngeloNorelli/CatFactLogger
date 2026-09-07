using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CatFactLogger.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<ICatFactApiClient, CatFactApiClient>(client =>
{
  client.BaseAddress = new Uri("https://catfact.ninja/");
  client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddSingleton<IFactFileWriter, FactFileWriter>();
builder.Services.AddHostedService<CatFactAppService>();

var host = builder.Build();
await host.RunAsync();