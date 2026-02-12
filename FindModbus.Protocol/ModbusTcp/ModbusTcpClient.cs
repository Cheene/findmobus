using FindModbus.Core.Models;
using NModbus;
using NModbus.IO;
using NModbus.Device;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;
using System.Linq;

namespace FindModbus.Protocol.ModbusTcp;

/// <summary>
/// Modbus TCP 客户端实现
/// </summary>
public class ModbusTcpClient : IModbusClient
{
    private TcpClient? _tcpClient;
    private IDisposable? _modbusMaster;
    private readonly TcpConnectionParams _params;
    private readonly object _lock = new();

    public ModbusTcpClient(TcpConnectionParams @params)
    {
        _params = @params;
    }

    public bool IsConnected => _tcpClient?.Connected ?? false;

    public async Task<bool> ConnectAsync()
    {
        try
        {
            lock (_lock)
            {
                if (_tcpClient?.Connected == true)
                    return true;

                _tcpClient?.Dispose();
                _tcpClient = new TcpClient();
            }

            // 使用超时连接
            using var cts = new CancellationTokenSource(_params.Timeout);
            await _tcpClient.ConnectAsync(_params.IpAddress, _params.Port).WaitAsync(cts.Token);
            
            var factory = new ModbusFactory();
            // NModbus 3.0: 使用 CreateMaster 创建 TCP master
            _modbusMaster = factory.CreateMaster(_tcpClient);
            
            return _tcpClient.Connected;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        catch (TimeoutException)
        {
            return false;
        }
        catch
        {
            return false;
        }
    }

    public Task DisconnectAsync()
    {
        lock (_lock)
        {
            _modbusMaster?.Dispose();
            _tcpClient?.Close();
            _tcpClient?.Dispose();
            _tcpClient = null;
            _modbusMaster = null;
        }
        return Task.CompletedTask;
    }

    public async Task<bool[]> ReadCoilsAsync(byte slaveId, ushort startAddress, ushort numberOfPoints)
    {
        if (_modbusMaster == null)
            throw new InvalidOperationException("未连接");

        var master = (dynamic)_modbusMaster;
        return await Task.Run(() => (bool[])master.ReadCoils(slaveId, startAddress, numberOfPoints));
    }

    public async Task<bool[]> ReadInputsAsync(byte slaveId, ushort startAddress, ushort numberOfPoints)
    {
        if (_modbusMaster == null)
            throw new InvalidOperationException("未连接");

        var master = (dynamic)_modbusMaster;
        return await Task.Run(() => (bool[])master.ReadInputs(slaveId, startAddress, numberOfPoints));
    }

    public async Task<ushort[]> ReadHoldingRegistersAsync(byte slaveId, ushort startAddress, ushort numberOfPoints)
    {
        if (_modbusMaster == null)
            throw new InvalidOperationException("未连接");

        return await Task.Run(() =>
        {
            try
            {
                // 使用反射查找正确的方法
                var masterType = _modbusMaster.GetType();
                
                // 尝试查找 ReadHoldingRegisters 方法（精确匹配）
                var method = masterType.GetMethod("ReadHoldingRegisters", 
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    new[] { typeof(byte), typeof(ushort), typeof(ushort) },
                    null);
                
                if (method != null)
                {
                    var result = method.Invoke(_modbusMaster, new object[] { slaveId, startAddress, numberOfPoints });
                    return (ushort[])result!;
                }
                
                // 如果找不到，尝试查找其他可能的方法名
                var allMethods = masterType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                var readMethod = allMethods.FirstOrDefault(m => 
                    m.Name.Contains("Holding") && m.Name.Contains("Register") && 
                    m.GetParameters().Length == 3);
                
                if (readMethod != null)
                {
                    var result = readMethod.Invoke(_modbusMaster, new object[] { slaveId, startAddress, numberOfPoints });
                    return (ushort[])result!;
                }
                
                // 如果还是找不到，列出所有方法名以便调试
                var methodNames = string.Join(", ", allMethods.Select(m => m.Name).Distinct());
                throw new InvalidOperationException(
                    $"找不到 ReadHoldingRegisters 方法。实际类型: {masterType.FullName}，可用方法: {methodNames}");
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"调用 ReadHoldingRegisters 失败: {ex.Message}", ex);
            }
        });
    }

    public async Task<ushort[]> ReadInputRegistersAsync(byte slaveId, ushort startAddress, ushort numberOfPoints)
    {
        if (_modbusMaster == null)
            throw new InvalidOperationException("未连接");

        var master = (dynamic)_modbusMaster;
        return await Task.Run(() => (ushort[])master.ReadInputRegisters(slaveId, startAddress, numberOfPoints));
    }

    public void Dispose()
    {
        DisconnectAsync().Wait();
    }
}


