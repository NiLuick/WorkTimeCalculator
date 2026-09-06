using System.Configuration;
using System.Data;
using System.Windows;
using WorkTimeCalculator.ViewModel;

namespace WorkTimeCalculator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    // Manual Start of the Main Window
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var viewModel = new MainWindowViewModel();
        var window = new MainWindow(viewModel);
        window.Show();
    }
}