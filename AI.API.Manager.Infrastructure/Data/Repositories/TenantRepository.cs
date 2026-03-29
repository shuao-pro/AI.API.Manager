using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AI.API.Manager.Infrastructure.Data.Repositories;

/// <summary>
/// 租户仓储实现
/// </summary>
public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Tenant?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Tenant>> GetActiveTenantsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.IsActive)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(t => t.Name == name, cancellationToken);
    }
}