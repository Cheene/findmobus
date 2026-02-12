namespace FindModbus.Core.Models;

/// <summary>
/// Modbus RTU 连接参数
/// </summary>
public class RtuConnectionParams
{
    /// <summary>
    /// 串口名称（如 COM1, /dev/ttyUSB0）
    /// </summary>
    public string PortName { get; set; } = string.Empty;
    
    /// <summary>
    /// 波特率
    /// </summary>
    public int BaudRate { get; set; } = 9600;
    
    /// <summary>
    /// 数据位
    /// </summary>
    public int DataBits { get; set; } = 8;
    
    /// <summary>
    /// 校验位
    /// </summary>
    public Parity Parity { get; set; } = Parity.None;
    
    /// <summary>
    /// 停止位
    /// </summary>
    public StopBits StopBits { get; set; } = StopBits.One;
    
    /// <summary>
    /// 读取超时（毫秒）
    /// </summary>
    public int ReadTimeout { get; set; } = 1000;
    
    /// <summary>
    /// 写入超时（毫秒）
    /// </summary>
    public int WriteTimeout { get; set; } = 1000;
}

/// <summary>
/// 校验位枚举
/// </summary>
public enum Parity
{
    None,
    Odd,
    Even
}

/// <summary>
/// 停止位枚举
/// </summary>
public enum StopBits
{
    One = 1,
    Two = 2
}


