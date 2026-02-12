namespace FindModbus.Core.Models;

/// <summary>
/// 扫描结果
/// </summary>
public class ScanResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Slave ID
    /// </summary>
    public byte SlaveId { get; set; }
    
    /// <summary>
    /// 功能码
    /// </summary>
    public byte FunctionCode { get; set; }
    
    /// <summary>
    /// 寄存器起始地址
    /// </summary>
    public ushort StartAddress { get; set; }
    
    /// <summary>
    /// 寄存器数量
    /// </summary>
    public ushort Count { get; set; }
    
    /// <summary>
    /// 连接参数（TCP 或 RTU）
    /// </summary>
    public object ConnectionParams { get; set; } = null!;
    
    /// <summary>
    /// 读取到的数据
    /// </summary>
    public byte[]? Data { get; set; }
    
    /// <summary>
    /// 可信度评分
    /// </summary>
    public ReliabilityScore ReliabilityScore { get; set; } = new();
    
    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// 扫描时间
    /// </summary>
    public DateTime ScanTime { get; set; } = DateTime.Now;
}


