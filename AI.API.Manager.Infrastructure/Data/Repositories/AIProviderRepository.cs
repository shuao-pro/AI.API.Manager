using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Enums;
using AI.API.Manager.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AI.API.Manager.Infrastructure.Data.Repositories;

/// <summary>
/// AI提供商仓储实现
/// </summary>
public class AIProviderRepository : Repository<AIProvider>, IAIProviderRepository
{
    public AIProviderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<AIProvider?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<AIProvider>> GetByTypeAsync(AIProviderType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.ProviderType == type && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AIProvider>> GetActiveProvidersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AIProvider>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        // AIProvider实体当前没有TenantId属性
        // 这个方法暂时返回空列表，后续可以根据需要实现
        return await Task.FromResult<IReadOnlyList<AIProvider>>(new List<AIProvider>());
    }
}