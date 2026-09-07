# CatFactLogger

Simple console application in .NET 10 that fetches random cat facts from
`https://catfact.ninja/fact` and appends them to a local `.txt` file.

## How to run

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

``` bash
cd CatFactLogger
dotner restore
dotner run
```

Once running:
- press **ENTER** to send a new request and append a fact to the file,
- type `exit` to stop the program.

The output file (`cat_facts.txt`) is created automatically in the application's working directory
(the path can be changed in [`appsettings.json`](/appsettings.json), under the `OutputFile:Path` key).

Example line written to the file:
```
2026-09-07 14:32:10 | length=52 | fact=Many cats love having their forehead gently stroked.
```

## Project structure

```
CatFactLogger/
├── Program.cs                      # DI composition root (Generic Host)
├── appsettings.json                # configuration (output file path, base url)
├── Models/
│   └── CatFact.cs                  # API response model
└── Services/
    ├── ICatFactApiClient.cs        # API client abstraction
    ├── CatFactApiClient.cs         # implementation (typed HttpClient)
    ├── IFactFileWriter.cs          # file writer abstraction
    ├── FactFileWriter.cs           # implementation of file writer
    └── CatFactAppService.cs        # main application loop (BackgroundService)
```