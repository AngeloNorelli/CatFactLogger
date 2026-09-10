using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using CatFactLogger.Services;
using CatFactLogger.ViewModels;


namespace CatFactLogger;

public partial class  App: Application
{
  private IHost? _host;

  protected override void OnStartup(StartupEventArgs e) 
  {
    base.OnStartup(e);

    var builder = Host.CreateApplicationBuilder();

    var configuredBase = builder.Configuration["CatFactApi:BaseUrl"]
      ?? throw new InvalidOperationException("Missing configuration for CatFactApi:BaseUrl");

    builder.Services.AddHttpClient<ICatFactApiClient, CatFactApiClient>(client => {
      client.BaseAddress = new Uri(configuredBase);
      client.Timeout = TimeSpan.FromSeconds(10);
    });

    builder.Services.AddSingleton<IFactFileWriter, FactFileWriter>();
    builder.Services.AddSingleton<MainViewModel>();
    builder.Services.AddSingleton<MainWindow>();

    _host = builder.Build();
    _host.Start();

    var mainWindow = _host.Services.GetRequiredService<MainWindow>();
    mainWindow.Show();
  }

  protected override void OnExit(ExitEventArgs e) 
  {
    _host?.Dispose();
    base.OnExit(e);
  }
}