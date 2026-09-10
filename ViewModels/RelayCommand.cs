
using System.Windows.Input;

namespace CatFactLogger.ViewModels;

public class RelayCommand(Func<Task> executeAsync) : ICommand 
{
  private bool _isExecuting;

  public event EventHandler? CanExecuteChanged;
  public bool CanExecute(object? parameter) => !_isExecuting;

  public async void Execute(object? parameter)
  {
    _isExecuting = true;
    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    try {
      await executeAsync();
    } finally {
      _isExecuting = false;
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}