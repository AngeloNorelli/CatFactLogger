
using System.Windows.Input;

namespace CatFactLogger.ViewModels;

public class RelayCommand : ICommand 
{
  private readonly Func<Task> _executeAsync;
  private bool _isExecuting;

  public RelayCommand(Func<Task> executeAsync) => _executeAsync = executeAsync;

  public event EventHandler? CanExecuteChanged;
  public bool CanExecute(object? parameter) => !_isExecuting;

  public async void Execute(object? parameter)
  {
    _isExecuting = true;
    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    try {
      await _executeAsync();
    } finally {
      _isExecuting = false;
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}