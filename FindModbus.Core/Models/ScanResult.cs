using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FindModbus.Core.Models;

/// <summary>
/// 扫描结果
/// </summary>
public class ScanResult : INotifyPropertyChanged
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
    
    private bool _isDetailVisible = false;
    /// <summary>
    /// 详情是否可见（UI 绑定用）
    /// </summary>
    public bool IsDetailVisible 
    {
        get { return _isDetailVisible; }
        set 
        {
            if (_isDetailVisible != value)
            {
                _isDetailVisible = value;
                OnPropertyChanged();
            }
        }
    }
    
    /// <summary>
    /// 属性变更通知事件
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    
    /// <summary>
    /// 触发属性变更通知
    /// </summary>
    /// <param name="propertyName">属性名</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


