using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CatFactLogger.Services;

namespace CatFactLogger.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
  private readonly ICatFactApiClient _apiClient;
  private readonly IFactFileWriter _fileWriter;
  private string _statusText = "Ready";
  
  public MainViewModel(ICatFactApiClient apiClient, IFactFileWriter fileWriter)
  {
    _apiClient = apiClient;
    _fileWriter = fileWriter;
    FetchFactCommand = new RelayCommand(FetchFactAsync);
  }

  public ObservableCollection<string> History { get; } = new();

  public string FilePath => Path.GetFullPath(_fileWriter.FilePath);

  public string StatusText 
  { 
    get => _statusText;
    private set => SetField(ref _statusText, value);
  }

  public ICommand FetchFactCommand { get; }

  public async Task FetchFactAsync() 
  {
    StatusText = "Fetching...";

    var fact = await _apiClient.GetRandomFactAsync();
    if (fact is null) 
    {
      StatusText = "Failed to fetch fact.";
      return;
    }

    await _fileWriter.AppendFactAsync(fact);

    History.Insert(0, $"{DateTime.Now:HH:mm:ss} - {fact.Fact} ({fact.Length} characters)");
    StatusText = "Fact fetched and saved successfully.";
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  private void SetField<T>(ref T field, T value, [CallerMemberName] string? name = null) 
  {
    if (Equals(field, value)) return;
    field = value;
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
  }
}
