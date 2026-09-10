# CatFactLogger

A .NET 10 WPF application that fetches random cat facts from
`https://catfact.ninja/fact` and appends them to a local `.txt` file.
Build with Dependency Injection and the MVVM pattern.

## How to run

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download) and **Windows** (WPF is a Windows-only UI framework).

``` bash
cd CatFactLogger
dotner restore
dotner run
```

Or open the solution in Visual Studio and press **F5** to run the application.

Once the windwo opens, click **Fetch Cat Fact** to send a request to the API. Each click:
- fetches a new fact from `catfact.ninja`,
- appends it to `cat_facts.txt` (one fact per line),
- adds it ot the on-screen history list.

By hovering over the item in the list you can see the full fact, also by clicking on it, you can see it under the list.

The output file (`cat_facts.txt`) is created automatically in the application's working directory
(the path can be changed in [`appsettings.json`](/appsettings.json), under the `OutputFile:Path` key).

Example line written to the file:
```
2026-09-07 14:32:10 | length=52 | fact=Many cats love having their forehead gently stroked.
```

## Project structure

```
CatFactLogger/
├── App.xaml / App.xaml.cs          # DI composition root + app startup
├── MainWindow.xaml / .xaml.cs      # main view (XAML UI)
├── appsettings.json                # configuration (output file path, base url)
├── Models/
│   └── CatFact.cs                  # API response model
├── ViewModels/
│   ├── MainViewModel.cs            # UI logic (MVM)
│   └── RelayCommand.cs             # ICommand implementation
└── Services/
    ├── ICatFactApiClient.cs        # API client abstraction
    ├── CatFactApiClient.cs         # implementation (typed HttpClient)
    ├── IFactFileWriter.cs          # file writer abstraction
    ├── FactFileWriter.cs           # implementation of file writer
    └── CatFactAppService.cs        # main application loop (BackgroundService)
```