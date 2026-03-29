using AI.API.Manager.Domain.Entities;

namespace AI.API.Manager.Domain.Repositories;

/// <summary>
/// API密钥仓储接口
/// </summary>
public interface IApiKeyRepository : IRepository<ApiKey>
{
    /// <summary>
    /// 根据密钥值获取API密钥
    /// </summary>
    Task<ApiKey?> GetByKeyValueAsync(string keyValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取租户的API密钥列表
    /// </summary>
    Task<IReadOnlyList<ApiKey>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取活跃的API密钥列表
    /// </summary>
    Task<IReadOnlyList<ApiKey>> GetActiveKeysAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取过期的API密钥列表
    /// </summary>
    Task<IReadOnlyList<ApiKey>> GetExpiredKeysAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查密钥值是否已存在
    /// </summary>
    Task<bool> ExistsByKeyValueAsync(string keyValue, CancellationToken cancellationToken = default);
}