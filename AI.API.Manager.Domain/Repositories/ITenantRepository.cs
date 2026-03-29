using AI.API.Manager.Domain.Entities;

namespace AI.API.Manager.Domain.Repositories;

/// <summary>
/// 租户仓储接口
/// </summary>
public interface ITenantRepository : IRepository<Tenant>
{
    /// <summary>
    /// 根据名称获取租户
    /// </summary>
    Task<Tenant?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取活跃的租户列表
    /// </summary>
    Task<IReadOnlyList<Tenant>> GetActiveTenantsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查租户名称是否已存在
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}