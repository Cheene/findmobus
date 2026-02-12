using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FindModbus.Core.Models;
using FindModbus.Core.Services;
using FindModbus.Protocol;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FindModbus.UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IScanService _scanService;
    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    private ConnectionMode _selectedMode = ConnectionMode.Tcp;

    [ObservableProperty]
    private string _ipAddress = "192.168.1.100";

    [ObservableProperty]
    private int _port = 502;

    [ObservableProperty]
    private string _selectedPort = string.Empty;

    [ObservableProperty]
    private int _baudRate = 9600;

    [ObservableProperty]
    private string _selectedParity = "无校验";

    [ObservableProperty]
    private string _selectedStopBits = "1位";

    [ObservableProperty]
    private string _selectedFunctionCodeDisplay = "03 - 读取保持寄存器";

    public byte SelectedFunctionCode
    {
        get
        {
            return SelectedFunctionCodeDisplay switch
            {
                "01 - 读取线圈状态" => 1,
                "02 - 读取输入状态" => 2,
                "03 - 读取保持寄存器" => 3,
                "04 - 读取输入寄存器" => 4,
                _ => 3
            };
        }
    }

    public ObservableCollection<int> BaudRateOptions { get; } = new() { 9600, 19200, 38400, 57600, 115200 };
    
    public ObservableCollection<string> ParityOptions { get; } = new() { "无校验", "偶校验", "奇校验" };
    
    public ObservableCollection<string> StopBitsOptions { get; } = new() { "1位", "2位" };

    public ObservableCollection<string> FunctionCodeOptions { get; } = new() 
    { 
        "01 - 读取线圈状态", 
        "02 - 读取输入状态", 
        "03 - 读取保持寄存器", 
        "04 - 读取输入寄存器" 
    };

    [ObservableProperty]
    private byte _slaveIdStart = 1;

    [ObservableProperty]
    private byte _slaveIdEnd = 247;

    [ObservableProperty]
    private ushort _registerStartAddress = 0;

    [ObservableProperty]
    private ushort _registerCount = 10;

    [ObservableProperty]
    private ObservableCollection<FunctionCodeInfo> _functionCodes = new();

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private string _currentParameter = string.Empty;

    [ObservableProperty]
    private double _progressPercentage;

    [ObservableProperty]
    private string _logText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ScanResult> _scanResults = new();

    [ObservableProperty]
    private ObservableCollection<string> _availablePorts = new();

    public bool IsTcpMode => SelectedMode == ConnectionMode.Tcp;
    public bool IsRtuMode => SelectedMode == ConnectionMode.Rtu;
    public bool IsNotScanning => !IsScanning;

    partial void OnSelectedModeChanged(ConnectionMode value)
    {
        OnPropertyChanged(nameof(IsTcpMode));
        OnPropertyChanged(nameof(IsRtuMode));
    }

    partial void OnIsScanningChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotScanning));
    }

    [RelayCommand]
    private void SelectTcp()
    {
        SelectedMode = ConnectionMode.Tcp;
    }

    [RelayCommand]
    private void SelectRtu()
    {
        SelectedMode = ConnectionMode.Rtu;
    }


    public MainWindowViewModel()
    {
        _scanService = new ScanService();
        _scanService.ProgressChanged += OnProgressChanged;
        _scanService.ResultFound += OnResultFound;
        _scanService.LogMessage += OnLogMessage;

        InitializeFunctionCodes();
        RefreshPorts();
    }

    private void InitializeFunctionCodes()
    {
        FunctionCodes.Clear();
        FunctionCodes.Add(new FunctionCodeInfo { Code = 1, Name = "01", Description = "读取线圈状态", IsSelected = false });
        FunctionCodes.Add(new FunctionCodeInfo { Code = 2, Name = "02", Description = "读取输入状态", IsSelected = false });
        FunctionCodes.Add(new FunctionCodeInfo { Code = 3, Name = "03", Description = "读取保持寄存器", IsSelected = true });
        FunctionCodes.Add(new FunctionCodeInfo { Code = 4, Name = "04", Description = "读取输入寄存器", IsSelected = false });
    }

    partial void OnSelectedFunctionCodeDisplayChanged(string value)
    {
        // 更新 FunctionCodes 集合中的选中状态
        var code = SelectedFunctionCode;
        foreach (var fc in FunctionCodes)
        {
            fc.IsSelected = fc.Code == code;
        }
    }

    [RelayCommand]
    private void RefreshPorts()
    {
        AvailablePorts.Clear();
        var ports = SerialPort.GetPortNames();
        foreach (var port in ports)
        {
            AvailablePorts.Add(port);
        }
        if (AvailablePorts.Count > 0 && string.IsNullOrEmpty(SelectedPort))
        {
            SelectedPort = AvailablePorts[0];
        }
    }

    [RelayCommand]
    private async Task StartScanAsync()
    {
        if (IsScanning)
            return;

        IsScanning = true;
        ScanResults.Clear();
        LogText = string.Empty;
        AddLog("开始扫描...");

        var config = new ScanConfig
        {
            Mode = SelectedMode,
            SlaveIdStart = SlaveIdStart,
            SlaveIdEnd = SlaveIdEnd,
            FunctionCodes = new List<byte> { SelectedFunctionCode },
            RegisterStartAddress = RegisterStartAddress,
            RegisterCount = RegisterCount,
            RequestInterval = 50,
            Timeout = 1000
        };

        if (SelectedMode == ConnectionMode.Tcp)
        {
            config.TcpParams = new TcpConnectionParams
            {
                IpAddress = IpAddress,
                Port = Port,
                Timeout = 3000
            };
        }
        else
        {
            if (string.IsNullOrEmpty(SelectedPort))
            {
                AddLog("错误: 请选择串口");
                IsScanning = false;
                return;
            }

            var parity = SelectedParity switch
            {
                "无校验" => FindModbus.Core.Models.Parity.None,
                "偶校验" => FindModbus.Core.Models.Parity.Even,
                "奇校验" => FindModbus.Core.Models.Parity.Odd,
                _ => FindModbus.Core.Models.Parity.None
            };

            var stopBits = SelectedStopBits switch
            {
                "1位" => FindModbus.Core.Models.StopBits.One,
                "2位" => FindModbus.Core.Models.StopBits.Two,
                _ => FindModbus.Core.Models.StopBits.One
            };

            config.RtuParams = new RtuConnectionParams
            {
                PortName = SelectedPort,
                BaudRate = BaudRate,
                DataBits = 8,
                Parity = parity,
                StopBits = stopBits,
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };
        }

        _cancellationTokenSource = new CancellationTokenSource();
        await _scanService.StartScanAsync(config, _cancellationTokenSource.Token);
        IsScanning = false;
    }

    [RelayCommand]
    private async Task StopScanAsync()
    {
        if (!IsScanning)
            return;

        await _scanService.StopScanAsync();
        _cancellationTokenSource?.Cancel();
        IsScanning = false;
        AddLog("扫描已停止");
    }

    [RelayCommand]
    private void ClearLog()
    {
        LogText = string.Empty;
    }


    private void OnProgressChanged(object? sender, ScanProgress progress)
    {
        CurrentParameter = progress.CurrentParameter;
        ProgressPercentage = progress.Percentage;
    }

    private void OnResultFound(object? sender, ScanResult result)
    {
        ScanResults.Add(result);
    }

    private void OnLogMessage(object? sender, string message)
    {
        AddLog(message);
    }

    private void AddLog(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        LogText += $"[{timestamp}] {message}\n";
    }
}

