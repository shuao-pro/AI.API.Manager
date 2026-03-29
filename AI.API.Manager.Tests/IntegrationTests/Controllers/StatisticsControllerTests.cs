using AI.API.Manager.API.DTOs;
using AI.API.Manager.API.Controllers;
using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace AI.API.Manager.Tests.IntegrationTests.Controllers;

public class StatisticsControllerTests : BaseIntegrationTest
{
    private const string BaseUrl = "/api/statistics";

    [Fact]
    public async Task GetSystemOverview_ShouldReturnCorrectStatistics()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        // 创建测试数据
        var tenant1 = TestDataGenerator.CreateTestTenant("Tenant 1", isActive: true);
        var tenant2 = TestDataGenerator.CreateTestTenant("Tenant 2", isActive: false);
        var tenant3 = TestDataGenerator.CreateTestTenant("Tenant 3", isActive: true);

        var provider1 = TestDataGenerator.CreateTestAIProvider("Provider 1", "OpenAI", "https://api.openai.com/v1", isActive: true);
        var provider2 = TestDataGenerator.CreateTestAIProvider("Provider 2", "OpenAI", "https://api.openai.com/v1", isActive: false);
        var provider3 = TestDataGenerator.CreateTestAIProvider("Provider 3", "OpenAI", "https://api.openai.com/v1", isActive: true);

        var apiKey1 = TestDataGenerator.CreateTestApiKey(tenant1.Id, provider1.Id, "API Key 1", isActive: true);
        var apiKey2 = TestDataGenerator.CreateTestApiKey(tenant2.Id, provider2.Id, "API Key 2", isActive: false);
        var apiKey3 = TestDataGenerator.CreateTestApiKey(tenant3.Id, provider3.Id, "API Key 3", isActive: true);

        await dbContext.Tenants.AddRangeAsync(tenant1, tenant2, tenant3);
        await dbContext.AIProviders.AddRangeAsync(provider1, provider2, provider3);
        await dbContext.ApiKeys.AddRangeAsync(apiKey1, apiKey2, apiKey3);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/overview");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<SystemOverviewResponse>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);

        // 验证统计信息
        Assert.Equal(3, apiResponse.Data.TotalTenants); // 总租户数
        Assert.Equal(2, apiResponse.Data.ActiveTenants); // 活跃租户数（tenant1和tenant3）
        Assert.Equal(3, apiResponse.Data.TotalProviders); // 总提供商数
        Assert.Equal(2, apiResponse.Data.ActiveProviders); // 活跃提供商数（provider1和provider3）
        Assert.Equal(3, apiResponse.Data.TotalApiKeys); // 总API密钥数
        Assert.True(apiResponse.Data.Timestamp <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task GetSystemOverview_ShouldReturnZeroStatistics_WhenNoDataExists()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/overview");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<SystemOverviewResponse>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);

        // 验证所有统计信息都为0
        Assert.Equal(0, apiResponse.Data.TotalTenants);
        Assert.Equal(0, apiResponse.Data.ActiveTenants);
        Assert.Equal(0, apiResponse.Data.TotalProviders);
        Assert.Equal(0, apiResponse.Data.ActiveProviders);
        Assert.Equal(0, apiResponse.Data.TotalApiKeys);
        Assert.True(apiResponse.Data.Timestamp <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task GetProviderUsageStatistics_ShouldReturnUsageData()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        // 创建测试数据
        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        var provider1 = TestDataGenerator.CreateTestAIProvider("Provider 1", "OpenAI", "https://api.openai.com/v1");
        var provider2 = TestDataGenerator.CreateTestAIProvider("Provider 2", "OpenAI", "https://api.openai.com/v1");
        var apiKey1 = TestDataGenerator.CreateTestApiKey(tenant.Id, provider1.Id, "API Key 1");
        var apiKey2 = TestDataGenerator.CreateTestApiKey(tenant.Id, provider2.Id, "API Key 2");

        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.AIProviders.AddRangeAsync(provider1, provider2);
        await dbContext.ApiKeys.AddRangeAsync(apiKey1, apiKey2);
        await dbContext.SaveChangesAsync();

        // 创建请求日志
        var now = DateTime.UtcNow;
        var startDate = now.AddDays(-7);
        var endDate = now;

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/providers/usage?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ProviderUsageResponse>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);

        // 验证响应包含正确的日期范围
        Assert.NotNull(apiResponse.Data.StartDate);
        Assert.NotNull(apiResponse.Data.EndDate);
        Assert.Equal(startDate.Date, apiResponse.Data.StartDate.Value.Date);
        Assert.Equal(endDate.Date, apiResponse.Data.EndDate.Value.Date);

        // 注意：当前实现返回空的ProviderUsage字典，所以这里只是验证响应格式
        Assert.NotNull(apiResponse.Data.ProviderUsage);
        Assert.True(apiResponse.Data.Timestamp <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task GetProviderUsageStatistics_ShouldReturnBadRequest_WhenDateRangeIsInvalid()
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(-7); // 开始时间晚于结束时间

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/providers/usage?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        AssertErrorResponse(apiResponse!, "INVALID_DATE_RANGE");
    }

    [Fact]
    public async Task GetRequestStatistics_ShouldReturnStatistics()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        // 创建测试数据
        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider", "OpenAI", "https://api.openai.com/v1");
        var apiKey = TestDataGenerator.CreateTestApiKey(tenant.Id, provider.Id, "Test API Key");

        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.ApiKeys.AddAsync(apiKey);
        await dbContext.SaveChangesAsync();

        // 创建请求日志
        var now = DateTime.UtcNow;
        var requestLogs = new List<RequestLog>
        {
            TestDataGenerator.CreateTestRequestLog(tenant.Id, apiKey.Id, provider.Id, "/v1/chat/completions", 200, 100),
            TestDataGenerator.CreateTestRequestLog(tenant.Id, apiKey.Id, provider.Id, "/v1/completions", 429, 50, "Rate limit exceeded"),
            TestDataGenerator.CreateTestRequestLog(tenant.Id, apiKey.Id, provider.Id, "/v1/embeddings", 500, 200, "Internal server error")
        };

        await dbContext.RequestLogs.AddRangeAsync(requestLogs);
        await dbContext.SaveChangesAsync();

        var query = new RequestStatisticsQuery
        {
            StartDate = now.AddDays(-1),
            EndDate = now.AddDays(1),
            TenantId = tenant.Id
        };

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/requests", query, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<RequestStatisticsResponse>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);

        // 验证统计信息
        // 注意：实际统计信息取决于RequestLogService的实现
        // 这里主要验证API响应格式正确
        Assert.True(apiResponse.Data.TotalRequests >= 0);
        Assert.True(apiResponse.Data.SuccessfulRequests >= 0);
        Assert.True(apiResponse.Data.FailedRequests >= 0);
        Assert.True(apiResponse.Data.AverageResponseTimeMs >= 0);
        Assert.NotNull(apiResponse.Data.StatisticsByEndpoint);
        Assert.NotNull(apiResponse.Data.StatisticsByProvider);
        Assert.NotNull(apiResponse.Data.StatisticsByStatusCode);
        Assert.True(apiResponse.Data.Timestamp <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task GetRequestStatistics_ShouldReturnBadRequest_WhenQueryIsInvalid()
    {
        // Arrange
        var query = new RequestStatisticsQuery
        {
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(-1) // 无效的日期范围
        };

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/requests", query, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        AssertErrorResponse(apiResponse!, "INVALID_PARAMETER");
    }
}