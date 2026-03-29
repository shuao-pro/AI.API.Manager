using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AI.API.Manager.Infrastructure.Data.Repositories;

/// <summary>
/// 请求日志仓储实现
/// </summary>
public class RequestLogRepository : Repository<RequestLog>, IRequestLogRepository
{
    public RequestLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<RequestLog>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.TenantId == tenantId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RequestLog>> GetByApiKeyIdAsync(Guid apiKeyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.ApiKeyId == apiKeyId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RequestLog>> GetByProviderIdAsync(Guid providerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.ProviderId == providerId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RequestLog>> GetByTimeRangeAsync(
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.CreatedAt >= startTime && l.CreatedAt <= endTime)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RequestLog>> GetFailedRequestsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.StatusCode >= 400)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(int totalRequests, int failedRequests, decimal totalCost)> GetTenantStatisticsAsync(
        Guid tenantId,
        DateTime? startTime = null,
        DateTime? endTime = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(l => l.TenantId == tenantId);

        if (startTime.HasValue)
        {
            query = query.Where(l => l.CreatedAt >= startTime.Value);
        }

        if (endTime.HasValue)
        {
            query = query.Where(l => l.CreatedAt <= endTime.Value);
        }

        var totalRequests = await query.CountAsync(cancellationToken);
        var failedRequests = await query.CountAsync(l => l.StatusCode >= 400, cancellationToken);

        // RequestLog实体当前没有Cost属性，暂时返回0
        var totalCost = 0m;

        return (totalRequests, failedRequests, totalCost);
    }
}