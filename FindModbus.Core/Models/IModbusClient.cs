namespace FindModbus.Core.Models;

/// <summary>
/// Modbus 客户端接口
/// </summary>
public interface IModbusClient : IDisposable
{
    /// <summary>
    /// 是否已连接
    /// </summary>
    bool IsConnected { get; }
    
    /// <summary>
    /// 连接
    /// </summary>
    Task<bool> ConnectAsync();
    
    /// <summary>
    /// 断开连接
    /// </summary>
    Task DisconnectAsync();
    
    /// <summary>
    /// 读取线圈状态（功能码 01）
    /// </summary>
    Task<bool[]> ReadCoilsAsync(byte slaveId, ushort startAddress, ushort numberOfPoints);
    
    /// <summary>
    /// 读取输入状态（功能码 02）
    /// </summary>
    Task<bool[]> ReadInputsAsync(byte slaveId, ushort startAddress, ushort numberOfPoints);
    
    /// <summary>
    /// 读取保持寄存器（功能码 03）
    /// </summary>
    Task<ushort[]> ReadHoldingRegistersAsync(byte slaveId, ushort startAddress, ushort numberOfPoints);
    
    /// <summary>
    /// 读取输入寄存器（功能码 04）
    /// </summary>
    Task<ushort[]> ReadInputRegistersAsync(byte slaveId, ushort startAddress, ushort numberOfPoints);
}


