using FindModbus.Core.Models;
using NModbus;
using NModbus.IO;
using System.IO.Ports;
using System.IO;

namespace FindModbus.Protocol.ModbusRtu;

/// <summary>
/// Modbus RTU 客户端实现
/// </summary>
public class ModbusRtuClient : IModbusClient
{
    private SerialPort? _serialPort;
    private IDisposable? _modbusMaster;
    private readonly RtuConnectionParams _params;
    private readonly object _lock = new();

    public ModbusRtuClient(RtuConnectionParams @params)
    {
        _params = @params;
    }

    public bool IsConnected => _serialPort?.IsOpen ?? false;

    public async Task<bool> ConnectAsync()
    {
        try
        {
            return await Task.Run(() =>
            {
                lock (_lock)
                {
                    if (_serialPort?.IsOpen == true)
                        return true;

                    _serialPort?.Dispose();
                    
                    _serialPort = new SerialPort(
                        _params.PortName,
                        _params.BaudRate,
                        ConvertParity(_params.Parity),
                        _params.DataBits,
                        ConvertStopBits(_params.StopBits))
                    {
                        ReadTimeout = _params.ReadTimeout,
                        WriteTimeout = _params.WriteTimeout
                    };

                    _serialPort.Open();

                    var factory = new ModbusFactory();
                    // NModbus 3.0 需要 IStreamResource，SerialPort 实现了它
                    _modbusMaster = factory.CreateRtuMaster(_serialPort as IStreamResource ?? throw new InvalidOperationException("SerialPort 不支持"));
                    
                    return _serialPort.IsOpen;
                }
            });
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
            _serialPort?.Close();
            _serialPort?.Dispose();
            _serialPort = null;
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

        var master = (dynamic)_modbusMaster;
        return await Task.Run(() => (ushort[])master.ReadHoldingRegisters(slaveId, startAddress, numberOfPoints));
    }

    public async Task<ushort[]> ReadInputRegistersAsync(byte slaveId, ushort startAddress, ushort numberOfPoints)
    {
        if (_modbusMaster == null)
            throw new InvalidOperationException("未连接");

        var master = (dynamic)_modbusMaster;
        return await Task.Run(() => (ushort[])master.ReadInputRegisters(slaveId, startAddress, numberOfPoints));
    }

    private static System.IO.Ports.Parity ConvertParity(FindModbus.Core.Models.Parity parity)
    {
        return parity switch
        {
            FindModbus.Core.Models.Parity.None => System.IO.Ports.Parity.None,
            FindModbus.Core.Models.Parity.Odd => System.IO.Ports.Parity.Odd,
            FindModbus.Core.Models.Parity.Even => System.IO.Ports.Parity.Even,
            _ => System.IO.Ports.Parity.None
        };
    }

    private static System.IO.Ports.StopBits ConvertStopBits(FindModbus.Core.Models.StopBits stopBits)
    {
        return stopBits switch
        {
            FindModbus.Core.Models.StopBits.One => System.IO.Ports.StopBits.One,
            FindModbus.Core.Models.StopBits.Two => System.IO.Ports.StopBits.Two,
            _ => System.IO.Ports.StopBits.One
        };
    }

    public void Dispose()
    {
        DisconnectAsync().Wait();
    }
}


