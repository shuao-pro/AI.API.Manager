using AI.API.Manager.API.DTOs;
using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace AI.API.Manager.Tests.IntegrationTests.Controllers;

public class RequestLogsControllerTests : BaseIntegrationTest
{
    private const string BaseUrl = "/api/requestlogs";

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoRequestLogsExist()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<RequestLogResponse>>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Empty(apiResponse.Data);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllRequestLogs()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        // 创建租户、提供商和API密钥
        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");
        var apiKey = TestDataGenerator.CreateTestApiKey(tenant.Id, provider.Id, "Test API Key");

        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.ApiKeys.AddAsync(apiKey);
        await dbContext.SaveChangesAsync();

        var requestLogs = new List<RequestLog>
        {
            TestDataGenerator.CreateTestRequestLog(tenant.Id, provider.Id, apiKey.Id, "/v1/chat/completions", "POST", 200, 100),
            TestDataGenerator.CreateTestRequestLog(tenant.Id, provider.Id, apiKey.Id, "/v1/completions", "POST", 429, 50)
        };

        await dbContext.RequestLogs.AddRangeAsync(requestLogs);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync(BaseUrl);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<RequestLogResponse>>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(2, apiResponse.Data.Count);
        Assert.Contains(apiResponse.Data, r => r.Endpoint == "/v1/chat/completions");
        Assert.Contains(apiResponse.Data, r => r.Endpoint == "/v1/completions");
    }

    [Fact]
    public async Task GetById_ShouldReturnRequestLog_WhenRequestLogExists()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        // 创建租户、提供商和API密钥
        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");
        var apiKey = TestDataGenerator.CreateTestApiKey(tenant.Id, provider.Id, "Test API Key");

        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.ApiKeys.AddAsync(apiKey);
        await dbContext.SaveChangesAsync();

        var requestLog = TestDataGenerator.CreateTestRequestLog(tenant.Id, provider.Id, apiKey.Id, "/v1/chat/completions", 200, 100);
        await dbContext.RequestLogs.AddAsync(requestLog);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{requestLog.Id}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<RequestLogResponse>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(requestLog.Id, apiResponse.Data.Id);
        Assert.Equal(requestLog.Endpoint, apiResponse.Data.Endpoint);
        Assert.Equal(requestLog.StatusCode, apiResponse.Data.StatusCode);
        Assert.Equal(requestLog.ResponseTimeMs, apiResponse.Data.ResponseTimeMs);
        Assert.Equal(requestLog.ErrorMessage, apiResponse.Data.ErrorMessage);
        Assert.Equal(requestLog.TenantId, apiResponse.Data.TenantId);
        Assert.Equal(requestLog.ApiKeyId, apiResponse.Data.ApiKeyId);
        Assert.Equal(requestLog.ProviderId, apiResponse.Data.ProviderId);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenRequestLogDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{nonExistentId}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        AssertErrorResponse(apiResponse!, "REQUEST_LOG_NOT_FOUND");
    }

    [Fact]
    public async Task GetByTenantId_ShouldReturnRequestLogsForTenant()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        // 创建两个租户
        var tenant1 = TestDataGenerator.CreateTestTenant("Tenant 1");
        var tenant2 = TestDataGenerator.CreateTestTenant("Tenant 2");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");
        var apiKey1 = TestDataGenerator.CreateTestApiKey(tenant1.Id, provider.Id, "Tenant 1 API Key");
        var apiKey2 = TestDataGenerator.CreateTestApiKey(tenant2.Id, provider.Id, "Tenant 2 API Key");

        await dbContext.Tenants.AddRangeAsync(tenant1, tenant2);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.ApiKeys.AddRangeAsync(apiKey1, apiKey2);
        await dbContext.SaveChangesAsync();

        // 为租户1创建2个请求日志，为租户2创建1个请求日志
        var tenant1Logs = new List<RequestLog>
        {
            TestDataGenerator.CreateTestRequestLog(tenant1.Id, apiKey1.Id, provider.Id, "/v1/chat/completions", 200, 100),
            TestDataGenerator.CreateTestRequestLog(tenant1.Id, apiKey1.Id, provider.Id, "/v1/completions", 200, 150)
        };
        var tenant2Log = TestDataGenerator.CreateTestRequestLog(tenant2.Id, apiKey2.Id, provider.Id, "/v1/embeddings", 200, 80);

        await dbContext.RequestLogs.AddRangeAsync(tenant1Logs);
        await dbContext.RequestLogs.AddAsync(tenant2Log);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/tenant/{tenant1.Id}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<RequestLogResponse>>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(2, apiResponse.Data.Count);
        Assert.All(apiResponse.Data, r => Assert.Equal(tenant1.Id, r.TenantId));
    }

    [Fact]
    public async Task GetByApiKeyId_ShouldReturnRequestLogsForApiKey()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        // 创建租户、提供商和两个API密钥
        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");
        var apiKey1 = TestDataGenerator.CreateTestApiKey(tenant.Id, provider.Id, "API Key 1");
        var apiKey2 = TestDataGenerator.CreateTestApiKey(tenant.Id, provider.Id, "API Key 2");

        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.ApiKeys.AddRangeAsync(apiKey1, apiKey2);
        await dbContext.SaveChangesAsync();

        // 为API密钥1创建2个请求日志，为API密钥2创建1个请求日志
        var apiKey1Logs = new List<RequestLog>
        {
            TestDataGenerator.CreateTestRequestLog(tenant.Id, apiKey1.Id, provider.Id, "/v1/chat/completions", 200, 100),
            TestDataGenerator.CreateTestRequestLog(tenant.Id, apiKey1.Id, provider.Id, "/v1/completions", 200, 150)
        };
        var apiKey2Log = TestDataGenerator.CreateTestRequestLog(tenant.Id, apiKey2.Id, provider.Id, "/v1/embeddings", 200, 80);

        await dbContext.RequestLogs.AddRangeAsync(apiKey1Logs);
        await dbContext.RequestLogs.AddAsync(apiKey2Log);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/api-key/{apiKey1.Id}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<RequestLogResponse>>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(2, apiResponse.Data.Count);
        Assert.All(apiResponse.Data, r => Assert.Equal(apiKey1.Id, r.ApiKeyId));
    }

    [Fact]
    public async Task Create_ShouldCreateRequestLog_WhenRequestIsValid()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");
        var apiKey = TestDataGenerator.CreateTestApiKey(tenant.Id, provider.Id, "Test API Key");

        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.ApiKeys.AddAsync(apiKey);
        await dbContext.SaveChangesAsync();

        var request = TestDataGenerator.CreateCreateRequestLogRequest(tenant.Id, apiKey.Id, provider.Id, "/v1/chat/completions", 200, 100);

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<RequestLogResponse>>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(request.Endpoint, apiResponse.Data.Endpoint);
        Assert.Equal(request.StatusCode, apiResponse.Data.StatusCode);
        Assert.Equal(request.ResponseTimeMs, apiResponse.Data.ResponseTimeMs);
        Assert.Equal(request.ErrorMessage, apiResponse.Data.ErrorMessage);
        Assert.Equal(request.TenantId, apiResponse.Data.TenantId);
        Assert.Equal(request.ApiKeyId, apiResponse.Data.ApiKeyId);
        Assert.Equal(request.ProviderId, apiResponse.Data.ProviderId);

        // 验证请求日志确实被创建了
        var createdRequestLog = await dbContext.RequestLogs.FindAsync(apiResponse.Data.Id);
        Assert.NotNull(createdRequestLog);
        Assert.Equal(request.Endpoint, createdRequestLog.Endpoint);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenEndpointIsEmpty()
    {
        // Arrange
        var request = new CreateRequestLogRequest
        {
            TenantId = Guid.NewGuid(),
            ApiKeyId = Guid.NewGuid(),
            ProviderId = Guid.NewGuid(),
            Endpoint = "",
            StatusCode = 200,
            ResponseTimeMs = 100
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
        var request = new CreateRequestLogRequest
        {
            TenantId = Guid.NewGuid(), // 不存在的租户ID
            ApiKeyId = Guid.NewGuid(),
            ProviderId = Guid.NewGuid(),
            Endpoint = "/v1/chat/completions",
            StatusCode = 200,
            ResponseTimeMs = 100
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
    public async Task UpdateStatus_ShouldUpdateRequestLogStatus_WhenRequestLogExists()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider");
        var apiKey = TestDataGenerator.CreateTestApiKey(tenant.Id, provider.Id, "Test API Key");

        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.ApiKeys.AddAsync(apiKey);
        await dbContext.SaveChangesAsync();

        var requestLog = TestDataGenerator.CreateTestRequestLog(tenant.Id, provider.Id, apiKey.Id, "/v1/chat/completions", 200, 100);
        await dbContext.RequestLogs.AddAsync(requestLog);
        await dbContext.SaveChangesAsync();

        var request = new UpdateRequestLogStatusRequest
        {
            StatusCode = 500,
            ErrorMessage = "Internal server error"
        };

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{requestLog.Id}/status", request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<RequestLogResponse>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(request.StatusCode, apiResponse.Data.StatusCode);
        Assert.Equal(request.ErrorMessage, apiResponse.Data.ErrorMessage);

        // 验证数据库中的更新
        var updatedRequestLog = await dbContext.RequestLogs.FindAsync(requestLog.Id);
        Assert.NotNull(updatedRequestLog);
        Assert.Equal(request.StatusCode, updatedRequestLog.StatusCode);
        Assert.Equal(request.ErrorMessage, updatedRequestLog.ErrorMessage);
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnNotFound_WhenRequestLogDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new UpdateRequestLogStatusRequest
        {
            StatusCode = 500,
            ErrorMessage = "Internal server error"
        };

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{nonExistentId}/status", request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        AssertErrorResponse(apiResponse!, "REQUEST_LOG_NOT_FOUND");
    }
}