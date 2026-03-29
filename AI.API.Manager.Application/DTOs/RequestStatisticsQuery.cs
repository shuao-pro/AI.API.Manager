namespace AI.API.Manager.Application.DTOs;

/// <summary>
/// 请求统计查询DTO
/// </summary>
public sealed record RequestStatisticsQuery
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; init; }

    /// <summary>
    /// 提供商ID
    /// </summary>
    public Guid? ProviderId { get; init; }

    /// <summary>
    /// API密钥ID
    /// </summary>
    public Guid? ApiKeyId { get; init; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// 是否仅统计成功请求
    /// </summary>
    public bool? OnlySuccessful { get; init; }

    /// <summary>
    /// 是否仅统计失败请求
    /// </summary>
    public bool? OnlyFailed { get; init; }

    /// <summary>
    /// 端点路径
    /// </summary>
    public string? Endpoint { get; init; }

    /// <summary>
    /// HTTP方法
    /// </summary>
    public string? Method { get; init; }

    /// <summary>
    /// 状态码
    /// </summary>
    public int? StatusCode { get; init; }

    /// <summary>
    /// 最小持续时间（毫秒）
    /// </summary>
    public int? MinDurationMs { get; init; }

    /// <summary>
    /// 最大持续时间（毫秒）
    /// </summary>
    public int? MaxDurationMs { get; init; }
}