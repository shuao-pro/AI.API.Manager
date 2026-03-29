namespace AI.API.Manager.Application.DTOs;

/// <summary>
/// 更新租户请求DTO
/// </summary>
public sealed record UpdateTenantRequest
{
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
    public bool IsActive { get; init; } = true;
}