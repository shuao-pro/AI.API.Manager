using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Domain.Entities;

namespace AI.API.Manager.Tests.IntegrationTests;

/// <summary>
/// 测试数据生成器
/// </summary>
public static class TestDataGenerator
{
    /// <summary>
    /// 生成测试租户
    /// </summary>
    public static Tenant CreateTestTenant(string name = "Test Tenant", string? description = "Test Description", bool isActive = true)
    {
        return Tenant.Create(name, description, isActive);
    }

    /// <summary>
    /// 生成创建租户请求
    /// </summary>
    public static CreateTenantRequest CreateCreateTenantRequest(string name = "Test Tenant", string? description = "Test Description")
    {
        return new CreateTenantRequest
        {
            Name = name,
            Description = description
        };
    }

    /// <summary>
    /// 生成更新租户请求
    /// </summary>
    public static UpdateTenantRequest CreateUpdateTenantRequest(string name = "Updated Tenant", string? description = "Updated Description", bool isActive = false)
    {
        return new UpdateTenantRequest
        {
            Name = name,
            Description = description,
            IsActive = isActive
        };
    }

    /// <summary>
    /// 生成测试AI提供商
    /// </summary>
    public static AIProvider CreateTestAIProvider(
        string name = "Test Provider",
        string providerType = "OpenAI",
        string baseUrl = "https://api.openai.com/v1",
        bool isActive = true,
        int maxConcurrentRequests = 10,
        int requestTimeoutSeconds = 30)
    {
        var providerTypeEnum = Enum.Parse<AI.API.Manager.Domain.Enums.AIProviderType>(providerType);
        return AIProvider.Create(name, baseUrl, providerTypeEnum, isActive, maxConcurrentRequests, requestTimeoutSeconds);
    }

    /// <summary>
    /// 生成创建AI提供商请求
    /// </summary>
    public static CreateAIProviderRequest CreateCreateAIProviderRequest(
        string name = "Test Provider",
        string providerType = "OpenAI",
        string baseUrl = "https://api.openai.com/v1",
        string apiKey = "test-api-key",
        int maxTokens = 4096,
        int timeoutSeconds = 30)
    {
        var providerTypeEnum = Enum.Parse<AI.API.Manager.Domain.Enums.AIProviderType>(providerType);
        return new CreateAIProviderRequest
        {
            Name = name,
            ProviderType = providerTypeEnum,
            BaseUrl = baseUrl,
            ApiKey = apiKey,
            MaxTokens = maxTokens,
            TimeoutSeconds = timeoutSeconds
        };
    }

    /// <summary>
    /// 生成更新AI提供商请求
    /// </summary>
    public static UpdateAIProviderRequest CreateUpdateAIProviderRequest(
        string name = "Updated Provider",
        string providerType = "Anthropic",
        string baseUrl = "https://api.anthropic.com/v1",
        string apiKey = "updated-api-key",
        bool isActive = false,
        int maxTokens = 4096,
        int timeoutSeconds = 30)
    {
        var providerTypeEnum = Enum.Parse<AI.API.Manager.Domain.Enums.AIProviderType>(providerType);
        return new UpdateAIProviderRequest
        {
            Name = name,
            ProviderType = providerTypeEnum,
            BaseUrl = baseUrl,
            ApiKey = apiKey,
            IsActive = isActive,
            MaxTokens = maxTokens,
            TimeoutSeconds = timeoutSeconds
        };
    }

    /// <summary>
    /// 生成测试API密钥
    /// </summary>
    public static ApiKey CreateTestApiKey(
        Guid tenantId,
        Guid providerId,
        string keyName = "Test API Key",
        string apiKeyValue = "test-api-key-value",
        bool isActive = true,
        int rateLimitPerMinute = 100,
        DateTime? expiresAt = null)
    {
        return ApiKey.Create(tenantId, providerId, keyName, apiKeyValue, isActive, rateLimitPerMinute, expiresAt);
    }

    /// <summary>
    /// 生成创建API密钥请求
    /// </summary>
    public static CreateApiKeyRequest CreateCreateApiKeyRequest(
        Guid tenantId,
        string name = "Test API Key",
        string? description = "Test API Key Description",
        bool isEnabled = true,
        DateTime? expiresAt = null)
    {
        return new CreateApiKeyRequest
        {
            TenantId = tenantId,
            Name = name,
            Description = description,
            IsEnabled = isEnabled,
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// 生成更新API密钥请求
    /// </summary>
    public static UpdateApiKeyRequest CreateUpdateApiKeyRequest(
        string name = "Updated API Key",
        string? description = "Updated API Key Description",
        bool isEnabled = false,
        DateTime? expiresAt = null)
    {
        return new UpdateApiKeyRequest
        {
            Name = name,
            Description = description,
            IsEnabled = isEnabled,
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// 生成测试请求日志
    /// </summary>
    public static RequestLog CreateTestRequestLog(
        Guid tenantId,
        Guid providerId,
        Guid apiKeyId,
        string endpoint = "/v1/chat/completions",
        string method = "POST",
        int statusCode = 200,
        int durationMs = 100,
        int? requestSizeBytes = null,
        int? responseSizeBytes = null,
        string? userAgent = null,
        string? ipAddress = null)
    {
        return RequestLog.Create(tenantId, providerId, apiKeyId, endpoint, method, statusCode, durationMs, requestSizeBytes, responseSizeBytes, userAgent, ipAddress);
    }

    /// <summary>
    /// 生成创建请求日志请求
    /// </summary>
    public static CreateRequestLogRequest CreateCreateRequestLogRequest(
        Guid tenantId,
        Guid providerId,
        Guid apiKeyId,
        string endpoint = "/v1/chat/completions",
        string method = "POST",
        int statusCode = 200,
        int durationMs = 100,
        int? requestSizeBytes = null,
        int? responseSizeBytes = null,
        string? userAgent = null,
        string? ipAddress = null)
    {
        return new CreateRequestLogRequest
        {
            TenantId = tenantId,
            ProviderId = providerId,
            ApiKeyId = apiKeyId,
            Endpoint = endpoint,
            Method = method,
            StatusCode = statusCode,
            DurationMs = durationMs,
            RequestSizeBytes = requestSizeBytes,
            ResponseSizeBytes = responseSizeBytes,
            UserAgent = userAgent,
            IpAddress = ipAddress
        };
    }
}