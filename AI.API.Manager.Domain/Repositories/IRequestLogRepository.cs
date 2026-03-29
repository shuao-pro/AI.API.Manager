using AI.API.Manager.Domain.Entities;

namespace AI.API.Manager.Domain.Repositories;

/// <summary>
/// 请求日志仓储接口
/// </summary>
public interface IRequestLogRepository : IRepository<RequestLog>
{
    /// <summary>
    /// 获取租户的请求日志列表
    /// </summary>
    Task<IReadOnlyList<RequestLog>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取API密钥的请求日志列表
    /// </summary>
    Task<IReadOnlyList<RequestLog>> GetByApiKeyIdAsync(Guid apiKeyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取AI提供商的请求日志列表
    /// </summary>
    Task<IReadOnlyList<RequestLog>> GetByProviderIdAsync(Guid providerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取时间范围内的请求日志列表
    /// </summary>
    Task<IReadOnlyList<RequestLog>> GetByTimeRangeAsync(
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取失败的请求日志列表
    /// </summary>
    Task<IReadOnlyList<RequestLog>> GetFailedRequestsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取租户的请求统计
    /// </summary>
    Task<(int totalRequests, int failedRequests, decimal totalCost)> GetTenantStatisticsAsync(
        Guid tenantId,
        DateTime? startTime = null,
        DateTime? endTime = null,
        CancellationToken cancellationToken = default);
}