namespace AI.API.Manager.Application.DTOs;

/// <summary>
/// 请求日志响应DTO
/// </summary>
public sealed record RequestLogResponse
{
    /// <summary>
    /// 请求日志ID
    /// </summary>
    public required Guid Id { get; init; }

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
    /// 是否成功
    /// </summary>
    public bool IsSuccessful => StatusCode >= 200 && StatusCode < 300;

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

    /// <summary>
    /// 创建时间
    /// </summary>
    public required DateTime CreatedAt { get; init; }
}