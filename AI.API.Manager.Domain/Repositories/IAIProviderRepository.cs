using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Enums;

namespace AI.API.Manager.Domain.Repositories;

/// <summary>
/// AI提供商仓储接口
/// </summary>
public interface IAIProviderRepository : IRepository<AIProvider>
{
    /// <summary>
    /// 根据名称获取AI提供商
    /// </summary>
    Task<AIProvider?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据类型获取AI提供商列表
    /// </summary>
    Task<IReadOnlyList<AIProvider>> GetByTypeAsync(AIProviderType type, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取活跃的AI提供商列表
    /// </summary>
    Task<IReadOnlyList<AIProvider>> GetActiveProvidersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取租户的AI提供商列表
    /// </summary>
    Task<IReadOnlyList<AIProvider>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}