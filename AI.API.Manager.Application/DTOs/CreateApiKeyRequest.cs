namespace AI.API.Manager.Application.DTOs;

/// <summary>
/// 创建API密钥请求DTO
/// </summary>
public sealed record CreateApiKeyRequest
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public required Guid TenantId { get; init; }

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