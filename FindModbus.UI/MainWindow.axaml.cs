using Avalonia.Controls;
using FindModbus.UI.ViewModels;

namespace FindModbus.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}


