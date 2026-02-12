namespace FindModbus.Core.Models;

/// <summary>
/// 可信度评分
/// </summary>
public class ReliabilityScore
{
    /// <summary>
    /// 响应一致性得分（0-100）
    /// </summary>
    public double ResponseConsistency { get; set; }
    
    /// <summary>
    /// 多功能码验证得分（0-100）
    /// </summary>
    public double MultiFunctionCodeVerification { get; set; }
    
    /// <summary>
    /// 数据稳定性得分（0-100）
    /// </summary>
    public double DataStability { get; set; }
    
    /// <summary>
    /// 综合得分（0-100）
    /// </summary>
    public double TotalScore => 
        ResponseConsistency * 0.4 + 
        MultiFunctionCodeVerification * 0.3 + 
        DataStability * 0.3;
    
    /// <summary>
    /// 可信度等级
    /// </summary>
    public ReliabilityLevel Level
    {
        get
        {
            var score = TotalScore;
            if (score >= 80) return ReliabilityLevel.High;
            if (score >= 60) return ReliabilityLevel.Medium;
            return ReliabilityLevel.Low;
        }
    }
}

/// <summary>
/// 可信度等级
/// </summary>
public enum ReliabilityLevel
{
    /// <summary>
    /// 高可信（80-100 分）
    /// </summary>
    High,
    
    /// <summary>
    /// 中可信（60-79 分）
    /// </summary>
    Medium,
    
    /// <summary>
    /// 低可信（<60 分）
    /// </summary>
    Low
}


