namespace AI.API.Manager.Application.DTOs;

/// <summary>
/// 更新API密钥请求DTO
/// </summary>
public sealed record UpdateApiKeyRequest
{
    /// <summary>
    /// 密钥名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 密钥描述
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime? ExpiresAt { get; init; }
}