using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AI.API.Manager.Application.Services;

/// <summary>
/// API密钥服务实现
/// </summary>
public class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ApiKeyService> _logger;

    public ApiKeyService(
        IApiKeyRepository apiKeyRepository,
        IMapper mapper,
        ILogger<ApiKeyService> logger)
    {
        _apiKeyRepository = apiKeyRepository ?? throw new ArgumentNullException(nameof(apiKeyRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApiKeyResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting API key by ID: {ApiKeyId}", id);

        var apiKey = await _apiKeyRepository.GetByIdAsync(id, cancellationToken);
        if (apiKey == null)
        {
            _logger.LogDebug("API key not found: {ApiKeyId}", id);
            return null;
        }

        return _mapper.Map<ApiKeyResponse>(apiKey);
    }

    public async Task<IReadOnlyList<ApiKeyResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all API keys");

        var apiKeys = await _apiKeyRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ApiKeyResponse>>(apiKeys);
    }

    public async Task<IReadOnlyList<ApiKeyResponse>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting API keys for tenant: {TenantId}", tenantId);

        var apiKeys = await _apiKeyRepository.GetByTenantIdAsync(tenantId, cancellationToken);
        return _mapper.Map<IReadOnlyList<ApiKeyResponse>>(apiKeys);
    }

    public async Task<ApiKeyResponse?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting API key by key value");

        var apiKey = await _apiKeyRepository.GetByKeyValueAsync(key, cancellationToken);
        if (apiKey == null)
        {
            _logger.LogDebug("API key not found for key value");
            return null;
        }

        return _mapper.Map<ApiKeyResponse>(apiKey);
    }

    public async Task<ApiKeyResponse> CreateAsync(CreateApiKeyRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating API key for tenant: {TenantId}", request.TenantId);

        // TODO: 检查租户是否存在
        // TODO: 检查提供商是否存在（需要ProviderId）

        // 生成密钥值（实际项目中应该使用安全的生成方式）
        string keyValue = GenerateApiKey();

        // 检查密钥值是否已存在（极小概率但需要检查）
        var exists = await _apiKeyRepository.ExistsByKeyValueAsync(keyValue, cancellationToken);
        if (exists)
        {
            // 重试生成（实际项目中应该有更完善的逻辑）
            keyValue = GenerateApiKey();
            exists = await _apiKeyRepository.ExistsByKeyValueAsync(keyValue, cancellationToken);
            if (exists)
            {
                throw new InvalidOperationException("Failed to generate unique API key. Please try again.");
            }
        }

        // 使用工厂方法创建API密钥实体
        // 注意：当前CreateApiKeyRequest缺少ProviderId，需要调整
        // 暂时使用默认值或从配置获取
        Guid defaultProviderId = Guid.Empty; // 实际项目中应该从配置或数据库获取

        var apiKey = ApiKey.Create(
            tenantId: request.TenantId,
            providerId: defaultProviderId,
            name: request.Name,
            keyValue: keyValue,
            isActive: request.IsEnabled,
            rateLimitPerMinute: 100, // 默认值，可以从配置获取
            expiresAt: request.ExpiresAt);

        // 保存到数据库
        var createdApiKey = await _apiKeyRepository.AddAsync(apiKey, cancellationToken);

        _logger.LogInformation("Created API key: {ApiKeyId} for tenant: {TenantId}", createdApiKey.Id, createdApiKey.TenantId);

        return _mapper.Map<ApiKeyResponse>(createdApiKey);
    }

    public async Task<ApiKeyResponse> UpdateAsync(Guid id, UpdateApiKeyRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating API key: {ApiKeyId}", id);

        // 获取现有API密钥
        var existingApiKey = await _apiKeyRepository.GetByIdAsync(id, cancellationToken);
        if (existingApiKey == null)
        {
            throw new InvalidOperationException($"API key with ID '{id}' not found.");
        }

        // 使用Update方法更新API密钥属性
        existingApiKey.Update(
            name: request.Name,
            keyValue: existingApiKey.KeyValue, // 密钥值通常不允许修改
            isActive: request.IsEnabled,
            rateLimitPerMinute: 100, // 默认值，可以从配置获取
            expiresAt: request.ExpiresAt);

        // 保存更新
        var updatedApiKey = await _apiKeyRepository.UpdateAsync(existingApiKey, cancellationToken);

        _logger.LogInformation("Updated API key: {ApiKeyId}", updatedApiKey.Id);

        return _mapper.Map<ApiKeyResponse>(updatedApiKey);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting API key: {ApiKeyId}", id);

        // 检查API密钥是否存在
        var existingApiKey = await _apiKeyRepository.GetByIdAsync(id, cancellationToken);
        if (existingApiKey == null)
        {
            throw new InvalidOperationException($"API key with ID '{id}' not found.");
        }

        // 删除API密钥
        await _apiKeyRepository.DeleteByIdAsync(id, cancellationToken);

        _logger.LogInformation("Deleted API key: {ApiKeyId}", id);
    }

    public async Task<bool> ValidateApiKeyAsync(string apiKey, Guid tenantId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Validating API key for tenant: {TenantId}", tenantId);

        var keyEntity = await _apiKeyRepository.GetByKeyValueAsync(apiKey, cancellationToken);
        if (keyEntity == null)
        {
            _logger.LogDebug("API key not found");
            return false;
        }

        // 检查密钥是否属于指定租户
        if (keyEntity.TenantId != tenantId)
        {
            _logger.LogDebug("API key does not belong to tenant: {TenantId}", tenantId);
            return false;
        }

        // 检查密钥是否启用
        if (!keyEntity.IsActive)
        {
            _logger.LogDebug("API key is not active");
            return false;
        }

        // 检查密钥是否过期
        if (keyEntity.IsExpired())
        {
            _logger.LogDebug("API key is expired");
            return false;
        }

        _logger.LogDebug("API key validation successful");
        return true;
    }

    private static string GenerateApiKey()
    {
        // 实际项目中应该使用更安全的生成方式
        // 这里使用简化的示例
        return $"sk_{Guid.NewGuid():N}";
    }
}