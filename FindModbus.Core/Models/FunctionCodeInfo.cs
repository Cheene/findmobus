namespace FindModbus.Core.Models;

/// <summary>
/// 功能码信息
/// </summary>
public class FunctionCodeInfo
{
    /// <summary>
    /// 功能码编号
    /// </summary>
    public byte Code { get; set; }
    
    /// <summary>
    /// 功能码名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 功能码描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 是否选中
    /// </summary>
    public bool IsSelected { get; set; }
}

