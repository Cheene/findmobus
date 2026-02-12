using FindModbus.Core.Models;
using FindModbus.Core.Services;
using FindModbus.Protocol.ModbusRtu;
using FindModbus.Protocol.ModbusTcp;

namespace FindModbus.Protocol;

/// <summary>
/// 扫描服务实现
/// </summary>
public class ScanService : IScanService
{
    private CancellationTokenSource? _cancellationTokenSource;
    private IModbusClient? _currentClient;

    public event EventHandler<ScanProgress>? ProgressChanged;
    public event EventHandler<ScanResult>? ResultFound;
    public event EventHandler<string>? LogMessage;

    public async Task StartScanAsync(ScanConfig config, CancellationToken cancellationToken)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        
        try
        {
            if (config.Mode == ConnectionMode.Tcp)
            {
                await ScanTcpAsync(config, _cancellationTokenSource.Token);
            }
            else
            {
                await ScanRtuAsync(config, _cancellationTokenSource.Token);
            }
        }
        catch (OperationCanceledException)
        {
            OnLogMessage("扫描已取消");
        }
        catch (Exception ex)
        {
            OnLogMessage($"扫描异常: {ex.Message}");
        }
    }

    public Task StopScanAsync()
    {
        _cancellationTokenSource?.Cancel();
        _currentClient?.Dispose();
        return Task.CompletedTask;
    }

    private async Task ScanTcpAsync(ScanConfig config, CancellationToken cancellationToken)
    {
        if (config.TcpParams == null)
            throw new ArgumentNullException(nameof(config.TcpParams));

        var totalTasks = CalculateTotalTasks(config);
        var completedTasks = 0;
        var successCount = 0;

        var progress = new ScanProgress
        {
            TotalTasks = totalTasks,
            CompletedTasks = 0,
            IsScanning = true
        };
        OnProgressChanged(progress);

        _currentClient = new ModbusTcpClient(config.TcpParams);
        
        OnLogMessage($"正在连接到 {config.TcpParams.IpAddress}:{config.TcpParams.Port}...");
        
        if (!await _currentClient.ConnectAsync())
        {
            OnLogMessage($"✗ 连接失败: 无法连接到 {config.TcpParams.IpAddress}:{config.TcpParams.Port}（超时或网络不可达）");
            progress.IsScanning = false;
            OnProgressChanged(progress);
            return;
        }

        OnLogMessage($"✓ 已连接到 {config.TcpParams.IpAddress}:{config.TcpParams.Port}");

        for (byte slaveId = config.SlaveIdStart; slaveId <= config.SlaveIdEnd; slaveId++)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            foreach (var functionCode in config.FunctionCodes)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var currentParam = $"TCP - Slave ID: {slaveId}, 功能码: {functionCode}";
                progress.CurrentParameter = currentParam;
                OnProgressChanged(progress);

                var result = await TryReadAsync(_currentClient, slaveId, functionCode, 
                    config.RegisterStartAddress, config.RegisterCount, config);

                completedTasks++;
                progress.CompletedTasks = completedTasks;
                OnProgressChanged(progress);

                // 设置连接参数
                result.ConnectionParams = config.TcpParams;
                
                // 无论成功或失败都记录结果
                OnResultFound(result);
                
                if (result.Success)
                {
                    successCount++;
                    progress.SuccessCount = successCount;
                    OnLogMessage($"✓ 成功: {currentParam}");
                }
                else
                {
                    OnLogMessage($"✗ 失败: {currentParam} - {result.ErrorMessage}");
                }

                await Task.Delay(config.RequestInterval, cancellationToken);
            }
        }

        progress.IsScanning = false;
        OnProgressChanged(progress);
        OnLogMessage($"扫描完成，共找到 {successCount} 个成功结果");
    }

    private async Task ScanRtuAsync(ScanConfig config, CancellationToken cancellationToken)
    {
        if (config.RtuParams == null)
            throw new ArgumentNullException(nameof(config.RtuParams));

        // RTU 扫描需要遍历参数组合
        var paramCombinations = GenerateRtuParameterCombinations(config.RtuParams);
        var totalTasks = paramCombinations.Count * 
                        (config.SlaveIdEnd - config.SlaveIdStart + 1) * 
                        config.FunctionCodes.Count;
        
        var completedTasks = 0;
        var successCount = 0;

        var progress = new ScanProgress
        {
            TotalTasks = totalTasks,
            CompletedTasks = 0,
            IsScanning = true
        };
        OnProgressChanged(progress);

        foreach (var rtuParams in paramCombinations)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            _currentClient?.Dispose();
            _currentClient = new ModbusRtuClient(rtuParams);

            if (!await _currentClient.ConnectAsync())
            {
                // 串口无法打开，直接跳过该参数组合
                var skippedTasks = (config.SlaveIdEnd - config.SlaveIdStart + 1) * config.FunctionCodes.Count;
                completedTasks += skippedTasks;
                progress.CompletedTasks = completedTasks;
                OnProgressChanged(progress);
                continue;
            }

            var paramDesc2 = $"{rtuParams.PortName} - {rtuParams.BaudRate}-{rtuParams.DataBits}-{rtuParams.Parity}-{rtuParams.StopBits}";
            OnLogMessage($"尝试参数: {paramDesc2}");

            for (byte slaveId = config.SlaveIdStart; slaveId <= config.SlaveIdEnd; slaveId++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                foreach (var functionCode in config.FunctionCodes)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var currentParam = $"RTU - {paramDesc2}, Slave ID: {slaveId}, 功能码: {functionCode}";
                    progress.CurrentParameter = currentParam;
                    OnProgressChanged(progress);

                    var result = await TryReadAsync(_currentClient, slaveId, functionCode,
                        config.RegisterStartAddress, config.RegisterCount, config);

                    completedTasks++;
                    progress.CompletedTasks = completedTasks;
                    OnProgressChanged(progress);

                    // 设置连接参数
                    result.ConnectionParams = rtuParams;
                    
                    // 无论成功或失败都记录结果
                    OnResultFound(result);
                    
                    if (result.Success)
                    {
                        successCount++;
                        progress.SuccessCount = successCount;
                        OnLogMessage($"✓ 成功: {currentParam}");
                    }
                    else
                    {
                        OnLogMessage($"✗ 失败: {currentParam} - {result.ErrorMessage}");
                    }

                    await Task.Delay(config.RequestInterval, cancellationToken);
                }
            }

            _currentClient.Dispose();
        }

        progress.IsScanning = false;
        OnProgressChanged(progress);
        OnLogMessage($"扫描完成，共找到 {successCount} 个成功结果");
    }

    private async Task<ScanResult> TryReadAsync(IModbusClient client, byte slaveId, byte functionCode,
        ushort startAddress, ushort count, ScanConfig config)
    {
        var result = new ScanResult
        {
            SlaveId = slaveId,
            FunctionCode = functionCode,
            StartAddress = startAddress,
            Count = count
        };

        try
        {
            byte[]? data = null;

            switch (functionCode)
            {
                case 1: // Read Coils
                    var coils = await client.ReadCoilsAsync(slaveId, startAddress, count);
                    data = coils.Select(b => (byte)(b ? 1 : 0)).ToArray();
                    break;
                case 2: // Read Inputs
                    var inputs = await client.ReadInputsAsync(slaveId, startAddress, count);
                    data = inputs.Select(b => (byte)(b ? 1 : 0)).ToArray();
                    break;
                case 3: // Read Holding Registers
                    var holdingRegs = await client.ReadHoldingRegistersAsync(slaveId, startAddress, count);
                    data = BitConverter.GetBytes(holdingRegs.Length).Concat(
                        holdingRegs.SelectMany(BitConverter.GetBytes)).ToArray();
                    break;
                case 4: // Read Input Registers
                    var inputRegs = await client.ReadInputRegistersAsync(slaveId, startAddress, count);
                    data = BitConverter.GetBytes(inputRegs.Length).Concat(
                        inputRegs.SelectMany(BitConverter.GetBytes)).ToArray();
                    break;
            }

            if (data != null && data.Length > 0)
            {
                result.Success = true;
                result.Data = data;
                // 简单可信度评分（后续可优化）
                result.ReliabilityScore = new ReliabilityScore
                {
                    ResponseConsistency = 80,
                    MultiFunctionCodeVerification = 70,
                    DataStability = 75
                };
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    private List<RtuConnectionParams> GenerateRtuParameterCombinations(RtuConnectionParams baseParams)
    {
        // 如果用户已指定完整参数，只使用用户指定的参数
        if (baseParams.BaudRate > 0)
        {
            return new List<RtuConnectionParams> { baseParams };
        }

        // 否则生成常见参数组合（优先级从高到低）
        var commonBaudRates = new[] { 9600, 19200, 38400, 57600, 115200 };
        var commonParities = new[] { Parity.None, Parity.Even, Parity.Odd };
        var commonDataBits = new[] { 8, 7 };
        var commonStopBits = new[] { StopBits.One, StopBits.Two };

        var combinations = new List<RtuConnectionParams>();

        // 生成常见组合
        foreach (var baudRate in commonBaudRates)
        {
            foreach (var dataBits in commonDataBits)
            {
                foreach (var parity in commonParities)
                {
                    foreach (var stopBits in commonStopBits)
                    {
                        combinations.Add(new RtuConnectionParams
                        {
                            PortName = baseParams.PortName,
                            BaudRate = baudRate,
                            DataBits = dataBits,
                            Parity = parity,
                            StopBits = stopBits,
                            ReadTimeout = baseParams.ReadTimeout,
                            WriteTimeout = baseParams.WriteTimeout
                        });
                    }
                }
            }
        }

        return combinations;
    }

    private int CalculateTotalTasks(ScanConfig config)
    {
        if (config.Mode == ConnectionMode.Tcp)
        {
            return (config.SlaveIdEnd - config.SlaveIdStart + 1) * config.FunctionCodes.Count;
        }
        else
        {
            // RTU 需要计算参数组合数
            var paramCount = 5 * 2 * 3 * 2; // 波特率*数据位*校验位*停止位（估算）
            return paramCount * (config.SlaveIdEnd - config.SlaveIdStart + 1) * config.FunctionCodes.Count;
        }
    }

    private void OnProgressChanged(ScanProgress progress)
    {
        ProgressChanged?.Invoke(this, progress);
    }

    private void OnResultFound(ScanResult result)
    {
        ResultFound?.Invoke(this, result);
    }

    private void OnLogMessage(string message)
    {
        LogMessage?.Invoke(this, message);
    }
}


