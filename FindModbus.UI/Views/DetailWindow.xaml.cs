using Avalonia; 
using Avalonia.Controls; 
using Avalonia.Markup.Xaml; 
using FindModbus.UI.ViewModels; 

namespace FindModbus.UI.Views; 

public partial class DetailWindow : Window 
{
    public DetailWindow() 
    {
        InitializeComponent();
    }
    
    public DetailWindow(DetailWindowViewModel viewModel) 
    {
        InitializeComponent();
        DataContext = viewModel;
    }
    
    private void InitializeComponent() 
    {
        AvaloniaXamlLoader.Load(this);
    }
}
