using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Enums;
using AI.API.Manager.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace AI.API.Manager.Application.Services;

/// <summary>
/// AI提供商服务实现
/// </summary>
public class AIProviderService : IAIProviderService
{
    private readonly IAIProviderRepository _aiProviderRepository;
    private readonly ILogger<AIProviderService> _logger;

    public AIProviderService(
        IAIProviderRepository aiProviderRepository,
        ILogger<AIProviderService> logger)
    {
        _aiProviderRepository = aiProviderRepository ?? throw new ArgumentNullException(nameof(aiProviderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AIProviderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting AI provider by ID: {ProviderId}", id);

        var provider = await _aiProviderRepository.GetByIdAsync(id, cancellationToken);
        if (provider == null)
        {
            _logger.LogDebug("AI provider not found: {ProviderId}", id);
            return null;
        }

        return MapToResponse(provider);
    }

    public async Task<IReadOnlyList<AIProviderResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all AI providers");

        var providers = await _aiProviderRepository.GetAllAsync(cancellationToken);
        return providers.Select(MapToResponse).ToList();
    }

    public async Task<IReadOnlyList<AIProviderResponse>> GetActiveProvidersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting active AI providers");

        var providers = await _aiProviderRepository.GetActiveProvidersAsync(cancellationToken);
        return providers.Select(MapToResponse).ToList();
    }

    public async Task<IReadOnlyList<AIProviderResponse>> GetByTypeAsync(AIProviderType providerType, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting AI providers by type: {ProviderType}", providerType);

        var providers = await _aiProviderRepository.GetByTypeAsync(providerType, cancellationToken);
        return providers.Select(MapToResponse).ToList();
    }

    public async Task<AIProviderResponse> CreateAsync(CreateAIProviderRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating AI provider with name: {ProviderName}", request.Name);

        // 检查名称是否已存在
        var existingProvider = await _aiProviderRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingProvider != null)
        {
            throw new InvalidOperationException($"AI provider with name '{request.Name}' already exists.");
        }

        // 使用工厂方法创建AI提供商实体
        // 注意：API密钥需要单独处理，这里只创建基础信息
        var provider = AIProvider.Create(
            request.Name,
            request.BaseUrl,
            request.ProviderType,
            request.IsActive,
            maxConcurrentRequests: 10, // 默认值，可以从配置获取
            requestTimeoutSeconds: request.TimeoutSeconds);

        // 保存到数据库
        var createdProvider = await _aiProviderRepository.AddAsync(provider, cancellationToken);

        _logger.LogInformation("Created AI provider: {ProviderId} - {ProviderName}", createdProvider.Id, createdProvider.Name);

        return MapToResponse(createdProvider);
    }

    public async Task<AIProviderResponse> UpdateAsync(Guid id, UpdateAIProviderRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating AI provider: {ProviderId}", id);

        // 获取现有提供商
        var existingProvider = await _aiProviderRepository.GetByIdAsync(id, cancellationToken);
        if (existingProvider == null)
        {
            throw new InvalidOperationException($"AI provider with ID '{id}' not found.");
        }

        // 使用Update方法更新提供商属性
        // 注意：API密钥需要单独处理，这里只更新基础信息
        existingProvider.Update(
            request.Name,
            request.BaseUrl,
            request.ProviderType,
            request.IsActive,
            maxConcurrentRequests: 10, // 默认值，可以从配置获取
            requestTimeoutSeconds: request.TimeoutSeconds);

        // 保存更新
        var updatedProvider = await _aiProviderRepository.UpdateAsync(existingProvider, cancellationToken);

        _logger.LogInformation("Updated AI provider: {ProviderId}", updatedProvider.Id);

        return MapToResponse(updatedProvider);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting AI provider: {ProviderId}", id);

        // 检查提供商是否存在
        var existingProvider = await _aiProviderRepository.GetByIdAsync(id, cancellationToken);
        if (existingProvider == null)
        {
            throw new InvalidOperationException($"AI provider with ID '{id}' not found.");
        }

        // 删除提供商
        await _aiProviderRepository.DeleteByIdAsync(id, cancellationToken);

        _logger.LogInformation("Deleted AI provider: {ProviderId}", id);
    }

    private static AIProviderResponse MapToResponse(AIProvider provider)
    {
        return new AIProviderResponse
        {
            Id = provider.Id,
            Name = provider.Name,
            ProviderType = provider.ProviderType,
            BaseUrl = provider.BaseUrl,
            IsActive = provider.IsActive,
            MaxConcurrentRequests = provider.MaxConcurrentRequests,
            RequestTimeoutSeconds = provider.RequestTimeoutSeconds,
            CreatedAt = provider.CreatedAt,
            UpdatedAt = provider.UpdatedAt
        };
    }
}