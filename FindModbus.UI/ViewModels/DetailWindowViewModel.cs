using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FindModbus.Core.Models;
using System;
using System.Globalization;
using System.Linq;

namespace FindModbus.UI.ViewModels;

public partial class DetailWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private byte _slaveId;
    
    [ObservableProperty]
    private string _status = string.Empty;
    
    [ObservableProperty]
    private byte _functionCode;
    
    [ObservableProperty]
    private ushort _startAddress;
    
    [ObservableProperty]
    private ushort _count;
    
    [ObservableProperty]
    private DateTime _scanTime;
    
    [ObservableProperty]
    private string _requestMessage = string.Empty;
    
    [ObservableProperty]
    private string _responseMessage = string.Empty;
    
    [ObservableProperty]
    private string _connectionMode = string.Empty;
    
    [ObservableProperty]
    private string _connectionParams = string.Empty;
    
    [ObservableProperty]
    private string _dataLength = string.Empty;
    
    [ObservableProperty]
    private string _responseTime = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    [ObservableProperty]
    private bool _isSuccess;
    
    [ObservableProperty]
    private bool _isError;
    
    [ObservableProperty]
    private string _statusBrush = string.Empty;
    
    [ObservableProperty]
    private string _statusBackground = string.Empty;
    
    public DetailWindowViewModel(ScanResult result)
    {
        if (result != null)
        {
            SlaveId = result.SlaveId;
            Status = result.Success ? "成功" : "失败";
            FunctionCode = result.FunctionCode;
            StartAddress = result.StartAddress;
            Count = result.Count;
            ScanTime = result.ScanTime;
            
            // 生成请求和响应报文
            GenerateMessages(result);
            
            // 连接模式和参数
            ConnectionMode = result.ConnectionParams is TcpConnectionParams ? "Modbus TCP" : "Modbus RTU";
            
            if (result.ConnectionParams is TcpConnectionParams tcpParams)
            {
                ConnectionParams = $"{tcpParams.IpAddress}:{tcpParams.Port}";
            }
            else if (result.ConnectionParams is RtuConnectionParams rtuParams)
            {
                ConnectionParams = $"{rtuParams.PortName}, {rtuParams.BaudRate} bps";
            }
            else
            {
                ConnectionParams = "未知";
            }
            
            // 数据长度
            DataLength = result.Data != null ? $"{result.Data.Length} 字节" : "无";
            
            // 响应时间（模拟值，实际应从扫描结果中获取）
            ResponseTime = "< 100 ms";
            
            // 错误信息
            ErrorMessage = result.ErrorMessage ?? "";
            
            // 状态标志
            IsSuccess = result.Success;
            IsError = !result.Success && !string.IsNullOrEmpty(result.ErrorMessage);
            
            // 状态颜色
            if (result.Success)
            {
                StatusBrush = "#10b981";
                StatusBackground = "#d1fae5";
            }
            else
            {
                StatusBrush = "#ef4444";
                StatusBackground = "#fee2e2";
            }
        }
    }
    
    [RelayCommand]
    private void Close()
    {
        // 关闭窗口的命令，将由窗口代码处理
    }
    
    /// <summary>
    /// 生成请求和响应报文
    /// </summary>
    /// <param name="result">扫描结果</param>
    private void GenerateMessages(ScanResult result)
    {
        // 生成请求报文
        // 格式：SlaveID FunctionCode StartAddress RegisterCount
        RequestMessage = $"{result.SlaveId:X2} {result.FunctionCode:X2} {result.StartAddress:X4} {result.Count:X4}";
        
        // 生成响应报文
        if (result.Success)
        {
            if (result.Data != null)
            {
                // 格式：SlaveID FunctionCode ByteCount Data...
                var dataHex = string.Join(" ", result.Data.Select(b => b.ToString("X2")));
                ResponseMessage = $"{result.SlaveId:X2} {result.FunctionCode:X2} {result.Data.Length:X2} {dataHex}";
            }
            else
            {
                // 即使没有数据，也生成成功响应
                ResponseMessage = $"{result.SlaveId:X2} {result.FunctionCode:X2} 00";
            }
        }
        else
        {
            // 错误响应
            ResponseMessage = $"{result.SlaveId:X2} {(result.FunctionCode | 0x80):X2} {result.ErrorMessage ?? "01"}"; // 01 = Illegal Function
        }
    }
}
