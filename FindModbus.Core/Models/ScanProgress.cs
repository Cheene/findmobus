namespace FindModbus.Core.Models;

/// <summary>
/// 扫描进度
/// </summary>
public class ScanProgress
{
    /// <summary>
    /// 总任务数
    /// </summary>
    public int TotalTasks { get; set; }
    
    /// <summary>
    /// 已完成任务数
    /// </summary>
    public int CompletedTasks { get; set; }
    
    /// <summary>
    /// 当前扫描参数描述
    /// </summary>
    public string CurrentParameter { get; set; } = string.Empty;
    
    /// <summary>
    /// 进度百分比（0-100）
    /// </summary>
    public double Percentage => TotalTasks > 0 ? (CompletedTasks * 100.0 / TotalTasks) : 0;
    
    /// <summary>
    /// 已找到的成功结果数
    /// </summary>
    public int SuccessCount { get; set; }
    
    /// <summary>
    /// 是否正在扫描
    /// </summary>
    public bool IsScanning { get; set; }
}


