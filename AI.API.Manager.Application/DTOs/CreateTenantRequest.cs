namespace AI.API.Manager.Application.DTOs;

/// <summary>
/// 创建租户请求DTO
/// </summary>
public sealed record CreateTenantRequest
{
    /// <summary>
    /// 租户名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 租户描述
    /// </summary>
    public string? Description { get; init; }
}