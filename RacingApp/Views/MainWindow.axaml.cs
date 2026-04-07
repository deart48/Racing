using Avalonia.Controls;
using RacingApp.ViewModels;

namespace RacingApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Closing += (_, _) =>
        {
            if (DataContext is MainWindowViewModel vm)
                vm.Dispose();
        };
    }
}
