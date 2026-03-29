namespace AI.API.Manager.Application.DTOs;

/// <summary>
/// 租户响应DTO
/// </summary>
public sealed record TenantResponse
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// 租户名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 租户描述
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// 是否活跃
    /// </summary>
    public required bool IsActive { get; init; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}