namespace AI.API.Manager.Application.DTOs;

/// <summary>
/// 更新请求日志状态请求DTO
/// </summary>
public sealed record UpdateRequestLogStatusRequest
{
    /// <summary>
    /// 状态码
    /// </summary>
    public required int StatusCode { get; init; }

    /// <summary>
    /// 持续时间（毫秒）
    /// </summary>
    public int? DurationMs { get; init; }

    /// <summary>
    /// 响应大小（字节）
    /// </summary>
    public int? ResponseSizeBytes { get; init; }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMessage { get; init; }
}