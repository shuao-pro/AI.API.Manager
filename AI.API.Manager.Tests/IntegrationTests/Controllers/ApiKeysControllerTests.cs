using AI.API.Manager.API.DTOs;
using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace AI.API.Manager.Tests.IntegrationTests.Controllers;

public class ApiKeysControllerTests : BaseIntegrationTest
{
    private const string BaseUrl = "/api/apikeys";

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoApiKeysExist()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<ApiKeyResponse>>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Empty(apiResponse.Data);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllApiKeys()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        // 先创建租户和提供商
        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");
        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.SaveChangesAsync();

        var apiKeys = new List<ApiKey>
        {
            TestDataGenerator.CreateTestApiKey(tenant.Id, provider.Id, "API Key 1"),
            TestDataGenerator.CreateTestApiKey(tenant.Id, provider.Id, "API Key 2")
        };

        await dbContext.ApiKeys.AddRangeAsync(apiKeys);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync(BaseUrl);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<ApiKeyResponse>>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(2, apiResponse.Data.Count);
        Assert.Contains(apiResponse.Data, k => k.Name == "API Key 1");
        Assert.Contains(apiResponse.Data, k => k.Name == "API Key 2");
    }

    [Fact]
    public async Task GetById_ShouldReturnApiKey_WhenApiKeyExists()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        // 先创建租户和提供商
        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");
        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.SaveChangesAsync();

        var apiKey = TestDataGenerator.CreateTestApiKey(tenant.Id, provider.Id, "Test API Key");
        await dbContext.ApiKeys.AddAsync(apiKey);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{apiKey.Id}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ApiKeyResponse>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(apiKey.Id, apiResponse.Data.Id);
        Assert.Equal(apiKey.Name, apiResponse.Data.Name);
        Assert.Equal(apiKey.TenantId, apiResponse.Data.TenantId);
        Assert.Equal(apiKey.ProviderId, apiResponse.Data.ProviderId);
        Assert.Equal(apiKey.IsActive, apiResponse.Data.IsActive);
        // 注意：API密钥值不应在响应中返回
        Assert.Null(apiResponse.Data.ApiKeyValue);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenApiKeyDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{nonExistentId}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        AssertErrorResponse(apiResponse!, "API_KEY_NOT_FOUND");
    }

    [Fact]
    public async Task GetByTenantId_ShouldReturnApiKeysForTenant()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        // 创建两个租户
        var tenant1 = TestDataGenerator.CreateTestTenant("Tenant 1");
        var tenant2 = TestDataGenerator.CreateTestTenant("Tenant 2");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");

        await dbContext.Tenants.AddRangeAsync(tenant1, tenant2);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.SaveChangesAsync();

        // 为租户1创建2个API密钥，为租户2创建1个API密钥
        var tenant1ApiKeys = new List<ApiKey>
        {
            TestDataGenerator.CreateTestApiKey(tenant1.Id, provider.Id, "Tenant 1 Key 1"),
            TestDataGenerator.CreateTestApiKey(tenant1.Id, provider.Id, "Tenant 1 Key 2")
        };
        var tenant2ApiKey = TestDataGenerator.CreateTestApiKey(tenant2.Id, provider.Id, "Tenant 2 Key 1");

        await dbContext.ApiKeys.AddRangeAsync(tenant1ApiKeys);
        await dbContext.ApiKeys.AddAsync(tenant2ApiKey);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/tenant/{tenant1.Id}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<ApiKeyResponse>>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(2, apiResponse.Data.Count);
        Assert.All(apiResponse.Data, k => Assert.Equal(tenant1.Id, k.TenantId));
    }

    [Fact]
    public async Task Create_ShouldCreateApiKey_WhenRequestIsValid()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");
        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.SaveChangesAsync();

        var request = TestDataGenerator.CreateCreateApiKeyRequest(tenant.Id, provider.Id, "New API Key");

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ApiKeyResponse>>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(request.Name, apiResponse.Data.Name);
        Assert.Equal(request.TenantId, apiResponse.Data.TenantId);
        Assert.Equal(request.ProviderId, apiResponse.Data.ProviderId);
        Assert.True(apiResponse.Data.IsActive); // 默认应该是激活状态
        // API密钥值不应在响应中返回
        Assert.Null(apiResponse.Data.ApiKeyValue);

        // 验证API密钥确实被创建了
        var createdApiKey = await dbContext.ApiKeys.FindAsync(apiResponse.Data.Id);
        Assert.NotNull(createdApiKey);
        Assert.Equal(request.Name, createdApiKey.KeyName);
        // 验证API密钥值已被哈希存储（不应是明文）
        Assert.NotEqual(request.ApiKeyValue, createdApiKey.ApiKeyValue);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenKeyNameIsEmpty()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");
        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.SaveChangesAsync();

        var request = new CreateApiKeyRequest
        {
            TenantId = tenant.Id,
            ProviderId = provider.Id,
            KeyName = "",
            ApiKeyValue = "test-key"
        };

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        AssertErrorResponse(apiResponse!, "VALIDATION_ERROR");
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenTenantDoesNotExist()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.SaveChangesAsync();

        var request = new CreateApiKeyRequest
        {
            TenantId = Guid.NewGuid(), // 不存在的租户ID
            ProviderId = provider.Id,
            KeyName = "Test Key",
            ApiKeyValue = "test-key"
        };

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        AssertErrorResponse(apiResponse!, "INVALID_PARAMETER");
    }

    [Fact]
    public async Task Update_ShouldUpdateApiKey_WhenApiKeyExists()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");
        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.SaveChangesAsync();

        var apiKey = TestDataGenerator.CreateTestApiKey(tenant.Id, provider.Id, "Old Name", isActive: true);
        await dbContext.ApiKeys.AddAsync(apiKey);
        await dbContext.SaveChangesAsync();

        var request = TestDataGenerator.CreateUpdateApiKeyRequest("Updated Name", isActive: false);

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{apiKey.Id}", request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ApiKeyResponse>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(request.Name, apiResponse.Data.Name);
        Assert.Equal(request.IsActive, apiResponse.Data.IsActive);

        // 验证数据库中的更新
        var updatedApiKey = await dbContext.ApiKeys.FindAsync(apiKey.Id);
        Assert.NotNull(updatedApiKey);
        Assert.Equal(request.Name, updatedApiKey.KeyName);
        Assert.Equal(request.IsActive, updatedApiKey.IsActive);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenApiKeyDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = TestDataGenerator.CreateUpdateApiKeyRequest();

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{nonExistentId}", request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        AssertErrorResponse(apiResponse!, "API_KEY_NOT_FOUND");
    }

    [Fact]
    public async Task Delete_ShouldDeleteApiKey_WhenApiKeyExists()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");
        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.SaveChangesAsync();

        var apiKey = TestDataGenerator.CreateTestApiKey(tenant.Id, provider.Id, "To Be Deleted");
        await dbContext.ApiKeys.AddAsync(apiKey);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{apiKey.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // 验证API密钥已被删除
        var deletedApiKey = await dbContext.ApiKeys.FindAsync(apiKey.Id);
        Assert.Null(deletedApiKey);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenApiKeyDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{nonExistentId}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        AssertErrorResponse(apiResponse!, "API_KEY_NOT_FOUND");
    }

    [Fact]
    public async Task ValidateApiKey_ShouldReturnTrue_WhenApiKeyIsValid()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");
        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.SaveChangesAsync();

        var apiKeyValue = "valid-api-key-value";
        var apiKey = TestDataGenerator.CreateTestApiKey(tenant.Id, provider.Id, "Valid Key", apiKeyValue, isActive: true);
        await dbContext.ApiKeys.AddAsync(apiKey);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/validate?apiKey={apiKeyValue}&tenantId={tenant.Id}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.True(apiResponse.Data);
    }

    [Fact]
    public async Task ValidateApiKey_ShouldReturnFalse_WhenApiKeyIsInvalid()
    {
        // Arrange
        var invalidApiKey = "invalid-api-key";
        var tenantId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/validate?apiKey={invalidApiKey}&tenantId={tenantId}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.False(apiResponse.Data);
    }
}