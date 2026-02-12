namespace FindModbus.Core.Models;

/// <summary>
/// 扫描配置
/// </summary>
public class ScanConfig
{
    /// <summary>
    /// 连接模式
    /// </summary>
    public ConnectionMode Mode { get; set; }
    
    /// <summary>
    /// TCP 连接参数（Mode = Tcp 时使用）
    /// </summary>
    public TcpConnectionParams? TcpParams { get; set; }
    
    /// <summary>
    /// RTU 连接参数（Mode = Rtu 时使用）
    /// </summary>
    public RtuConnectionParams? RtuParams { get; set; }
    
    /// <summary>
    /// Slave ID 扫描范围（起始）
    /// </summary>
    public byte SlaveIdStart { get; set; } = 1;
    
    /// <summary>
    /// Slave ID 扫描范围（结束）
    /// </summary>
    public byte SlaveIdEnd { get; set; } = 247;
    
    /// <summary>
    /// 要扫描的功能码列表
    /// </summary>
    public List<byte> FunctionCodes { get; set; } = new() { 1, 2, 3, 4 };
    
    /// <summary>
    /// 寄存器起始地址
    /// </summary>
    public ushort RegisterStartAddress { get; set; } = 0;
    
    /// <summary>
    /// 寄存器数量
    /// </summary>
    public ushort RegisterCount { get; set; } = 10;
    
    /// <summary>
    /// 请求间隔（毫秒）
    /// </summary>
    public int RequestInterval { get; set; } = 50;
    
    /// <summary>
    /// 超时时间（毫秒）
    /// </summary>
    public int Timeout { get; set; } = 1000;
}


