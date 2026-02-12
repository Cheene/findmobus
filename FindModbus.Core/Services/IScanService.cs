using FindModbus.Core.Models;

namespace FindModbus.Core.Services;

/// <summary>
/// 扫描服务接口
/// </summary>
public interface IScanService
{
    /// <summary>
    /// 开始扫描
    /// </summary>
    Task StartScanAsync(ScanConfig config, CancellationToken cancellationToken);
    
    /// <summary>
    /// 停止扫描
    /// </summary>
    Task StopScanAsync();
    
    /// <summary>
    /// 扫描进度事件
    /// </summary>
    event EventHandler<ScanProgress>? ProgressChanged;
    
    /// <summary>
    /// 扫描结果事件
    /// </summary>
    event EventHandler<ScanResult>? ResultFound;
    
    /// <summary>
    /// 日志事件
    /// </summary>
    event EventHandler<string>? LogMessage;
}


