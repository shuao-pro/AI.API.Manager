using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AI.API.Manager.Infrastructure.Data.Repositories;

/// <summary>
/// API密钥仓储实现
/// </summary>
public class ApiKeyRepository : Repository<ApiKey>, IApiKeyRepository
{
    public ApiKeyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ApiKey?> GetByKeyValueAsync(string keyValue, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(k => k.KeyValue == keyValue, cancellationToken);
    }

    public async Task<IReadOnlyList<ApiKey>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(k => k.TenantId == tenantId)
            .OrderBy(k => k.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ApiKey>> GetActiveKeysAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(k => k.IsActive && (!k.ExpiresAt.HasValue || k.ExpiresAt > now))
            .OrderBy(k => k.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ApiKey>> GetExpiredKeysAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(k => k.ExpiresAt.HasValue && k.ExpiresAt <= now)
            .OrderBy(k => k.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByKeyValueAsync(string keyValue, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(k => k.KeyValue == keyValue, cancellationToken);
    }
}