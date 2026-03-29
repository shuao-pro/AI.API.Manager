using AI.API.Manager.Application.DTOs;

namespace AI.API.Manager.Application.Services;

/// <summary>
/// 租户服务接口
/// </summary>
public interface ITenantService
{
    /// <summary>
    /// 根据ID获取租户
    /// </summary>
    Task<TenantResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有租户
    /// </summary>
    Task<IReadOnlyList<TenantResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取活跃租户列表
    /// </summary>
    Task<IReadOnlyList<TenantResponse>> GetActiveTenantsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建租户
    /// </summary>
    Task<TenantResponse> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新租户
    /// </summary>
    Task<TenantResponse> UpdateAsync(Guid id, UpdateTenantRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除租户
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}