# CatFactLogger

A web application (.NET 10, Blazor Web App) that fetches random cat facts from
`https://catfact.ninja/fact` and appends them to a local `.txt` file.
Build with Dependency Injection.

## How to run

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

``` bash
cd CatFactLogger
dotnet restore
dotnet run
```

The app will start at the address printed in the console (see also [`Properties/launchSetting.json`](Properties/launchSettings.json)).

Or open the solution in Visual Studio and press **F5** to run the application.

Once the window opens, click **Fetch Cat Fact** to send a request to the API. Each click:
- fetches a new fact from `catfact.ninja`,
- appends it to `cat_facts.txt` (one fact per line),
- adds a new row to the table on the page (columns: time, length, fact).

The output file (`cat_facts.txt`) is created automatically in the application's working directory
(the path can be changed in [`appsettings.json`](/appsettings.json), under the `OutputFile:Path` key).

Example line written to the file:
```
2026-09-07 14:32:10 | length=52 | fact=Many cats love having their forehead gently stroked.
```

## Tests
Unit tests (xUnit) covers the `Services/` layer - API communication (with a substituted `HttpMessageHandler`) 
and file read/write (against a temporary directory).

```bash
dotnet test CatFactLogger.Tests/CatFactLogger.Tests.csproj
```

## CI/CD
Every push and pull requests to `main` triggers a build + test run via 
Github Actions - see [`.github/workflows/ci.yml`](.github/workflows/ci.yml).

## Project structure

```
CatFactLogger/
├── Program.cs                      # DI composition root + app startup
├── appsettings.json                # configuration (output file path, base url)
├── Models/
│   └── CatFact.cs                  # API response model
├── Components/
│   ├── App.razor                   # main component (head, style, scrpts)
│   ├── Routes.razor                # routing
│   ├── Layout/
│   │   └── MainLayout.razor        # page layout
│   └── Pages/
│       └── Main.razor              # main page: button + log table
├── wwwroot/
│   ├── app.css                     # Bootstrap theme overrides (colors, fonts)
│   └── lib/bootstrap/              # Bootstrap 5.3.2 (CSS + JS)
├── Services/
│   ├── ICatFactApiClient.cs        # API client abstraction
│   ├── CatFactApiClient.cs         # implementation (typed HttpClient)
│   ├── IFactFileWriter.cs          # file writer abstraction
│   ├── FactFileWriter.cs           # implementation of file writer
│   └── CatFactAppService.cs        # main application loop (BackgroundService)
└── .github/workflows/ci.yml        # CI/CD workflow (build + test)
```