using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AI.API.Manager.Application.Services;

/// <summary>
/// 租户服务实现
/// </summary>
public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TenantService> _logger;

    public TenantService(
        ITenantRepository tenantRepository,
        IMapper mapper,
        ILogger<TenantService> logger)
    {
        _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TenantResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting tenant by ID: {TenantId}", id);

        var tenant = await _tenantRepository.GetByIdAsync(id, cancellationToken);
        if (tenant == null)
        {
            _logger.LogDebug("Tenant not found: {TenantId}", id);
            return null;
        }

        return _mapper.Map<TenantResponse>(tenant);
    }

    public async Task<IReadOnlyList<TenantResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all tenants");

        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<TenantResponse>>(tenants);
    }

    public async Task<IReadOnlyList<TenantResponse>> GetActiveTenantsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting active tenants");

        var tenants = await _tenantRepository.GetActiveTenantsAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<TenantResponse>>(tenants);
    }

    public async Task<TenantResponse> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating tenant with name: {TenantName}", request.Name);

        // 检查名称是否已存在
        var exists = await _tenantRepository.ExistsByNameAsync(request.Name, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException($"Tenant with name '{request.Name}' already exists.");
        }

        // 使用工厂方法创建租户实体
        var tenant = Tenant.Create(request.Name, request.Description, isActive: true);

        // 保存到数据库
        var createdTenant = await _tenantRepository.AddAsync(tenant, cancellationToken);

        _logger.LogInformation("Created tenant: {TenantId} - {TenantName}", createdTenant.Id, createdTenant.Name);

        return _mapper.Map<TenantResponse>(createdTenant);
    }

    public async Task<TenantResponse> UpdateAsync(Guid id, UpdateTenantRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating tenant: {TenantId}", id);

        // 获取现有租户
        var existingTenant = await _tenantRepository.GetByIdAsync(id, cancellationToken);
        if (existingTenant == null)
        {
            throw new InvalidOperationException($"Tenant with ID '{id}' not found.");
        }

        // 使用Update方法更新租户属性
        existingTenant.Update(request.Name, request.Description, request.IsActive);

        // 保存更新
        var updatedTenant = await _tenantRepository.UpdateAsync(existingTenant, cancellationToken);

        _logger.LogInformation("Updated tenant: {TenantId}", updatedTenant.Id);

        return _mapper.Map<TenantResponse>(updatedTenant);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting tenant: {TenantId}", id);

        // 检查租户是否存在
        var existingTenant = await _tenantRepository.GetByIdAsync(id, cancellationToken);
        if (existingTenant == null)
        {
            throw new InvalidOperationException($"Tenant with ID '{id}' not found.");
        }

        // 删除租户
        await _tenantRepository.DeleteByIdAsync(id, cancellationToken);

        _logger.LogInformation("Deleted tenant: {TenantId}", id);
    }

}