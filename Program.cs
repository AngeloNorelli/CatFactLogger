using CatFactLogger.Components;
using CatFactLogger.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddRazorComponents()
  .AddInteractiveServerComponents();

var baseURL = builder.Configuration["CatFactApi:BaseUrl"];
if (string.IsNullOrEmpty(baseURL)) 
{
  throw new InvalidOperationException("Base URL for CatFact API is not configured.");
}

builder.Services.AddHttpClient<ICatFactApiClient, CatFactApiClient>(client => 
{
  client.BaseAddress = new Uri(baseURL);
});

builder.Services.AddSingleton<IFactFileWriter, FactFileWriter>();

var app = builder.Build();
if (!app.Environment.IsDevelopment()) 
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
