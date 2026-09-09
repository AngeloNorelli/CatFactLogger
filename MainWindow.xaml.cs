using System.Windows;
using CatFactLogger.ViewModels;

namespace CatFactLogger;

public partial class MainWindow : Window
{
	public MainWindow(MainViewModel viewModel)
	{
	  InitializeComponent();
	  DataContext = viewModel;
	}
}
