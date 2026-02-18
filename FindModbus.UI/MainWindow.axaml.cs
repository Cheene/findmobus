using Avalonia.Controls;
using Avalonia.Threading;
using FindModbus.UI.ViewModels;
using System.Threading.Tasks;

namespace FindModbus.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        
        // 窗口加载完成后强制布局更新，修复下拉框问题
        // 这是因为窗口首次打开时布局可能未完全计算，导致 ComboBox 的 Popup 无法正确定位
        this.Opened += OnWindowOpened;
    }
    
    private async void OnWindowOpened(object? sender, System.EventArgs e)
    {
        // 等待窗口完全显示和渲染
        await Task.Delay(100);
        
        // 使用窗口状态变化来强制触发布局重新计算（这是最可靠的方法，模拟全屏操作）
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var originalState = this.WindowState;
            
            // 临时最大化再恢复（完全模拟全屏操作）
            this.WindowState = WindowState.Maximized;
            
            // 立即恢复，这会强制所有布局重新计算
            Dispatcher.UIThread.Post(() =>
            {
                this.WindowState = originalState;
                
                // 强制所有控件重新布局
                this.InvalidateMeasure();
                this.InvalidateArrange();
                this.UpdateLayout();
            }, DispatcherPriority.Render);
        });
    }
}


