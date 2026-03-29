namespace AI.API.Manager.Application.DTOs;

/// <summary>
/// API密钥响应DTO
/// </summary>
public sealed record ApiKeyResponse
{
    /// <summary>
    /// API密钥ID
    /// </summary>
    public required Guid Id { get; init; }

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
    public required bool IsEnabled { get; init; }

    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime? ExpiresAt { get; init; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}