namespace AI.API.Manager.Application.DTOs;

/// <summary>
/// 请求统计响应DTO
/// </summary>
public sealed record RequestStatisticsResponse
{
    /// <summary>
    /// 总请求数
    /// </summary>
    public required int TotalRequests { get; init; }

    /// <summary>
    /// 成功请求数
    /// </summary>
    public required int SuccessfulRequests { get; init; }

    /// <summary>
    /// 失败请求数
    /// </summary>
    public required int FailedRequests { get; init; }

    /// <summary>
    /// 成功率
    /// </summary>
    public double SuccessRate => TotalRequests > 0 ? (double)SuccessfulRequests / TotalRequests * 100 : 0;

    /// <summary>
    /// 平均响应时间（毫秒）
    /// </summary>
    public required double AverageResponseTimeMs { get; init; }

    /// <summary>
    /// 最小响应时间（毫秒）
    /// </summary>
    public required int MinResponseTimeMs { get; init; }

    /// <summary>
    /// 最大响应时间（毫秒）
    /// </summary>
    public required int MaxResponseTimeMs { get; init; }

    /// <summary>
    /// 总请求大小（字节）
    /// </summary>
    public long? TotalRequestSizeBytes { get; init; }

    /// <summary>
    /// 总响应大小（字节）
    /// </summary>
    public long? TotalResponseSizeBytes { get; init; }

    /// <summary>
    /// 按状态码统计
    /// </summary>
    public Dictionary<int, int>? StatusCodeCounts { get; init; }

    /// <summary>
    /// 按端点统计
    /// </summary>
    public Dictionary<string, int>? EndpointCounts { get; init; }

    /// <summary>
    /// 按提供商统计
    /// </summary>
    public Dictionary<Guid, int>? ProviderCounts { get; init; }
}