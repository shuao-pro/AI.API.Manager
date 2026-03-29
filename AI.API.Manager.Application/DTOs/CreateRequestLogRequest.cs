namespace AI.API.Manager.Application.DTOs;

/// <summary>
/// 创建请求日志请求DTO
/// </summary>
public sealed record CreateRequestLogRequest
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public required Guid TenantId { get; init; }

    /// <summary>
    /// 提供商ID
    /// </summary>
    public required Guid ProviderId { get; init; }

    /// <summary>
    /// API密钥ID
    /// </summary>
    public required Guid ApiKeyId { get; init; }

    /// <summary>
    /// 端点路径
    /// </summary>
    public required string Endpoint { get; init; }

    /// <summary>
    /// HTTP方法
    /// </summary>
    public required string Method { get; init; }

    /// <summary>
    /// 状态码
    /// </summary>
    public required int StatusCode { get; init; }

    /// <summary>
    /// 持续时间（毫秒）
    /// </summary>
    public required int DurationMs { get; init; }

    /// <summary>
    /// 请求大小（字节）
    /// </summary>
    public int? RequestSizeBytes { get; init; }

    /// <summary>
    /// 响应大小（字节）
    /// </summary>
    public int? ResponseSizeBytes { get; init; }

    /// <summary>
    /// 用户代理
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// IP地址
    /// </summary>
    public string? IpAddress { get; init; }
}