namespace FindModbus.Core.Models;

/// <summary>
/// Modbus TCP 连接参数
/// </summary>
public class TcpConnectionParams
{
    /// <summary>
    /// IP 地址或 IP 段（CIDR 格式）
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// 端口号，默认 502
    /// </summary>
    public int Port { get; set; } = 502;
    
    /// <summary>
    /// 连接超时（毫秒）
    /// </summary>
    public int Timeout { get; set; } = 3000;
}


