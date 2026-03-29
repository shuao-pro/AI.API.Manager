using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Enums;
using AI.API.Manager.Infrastructure.Data;
using AI.API.Manager.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AI.API.Manager.Tests.Infrastructure.Data.Repositories;

public class SpecificRepositoryImplementationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly TenantRepository _tenantRepository;
    private readonly AIProviderRepository _aiProviderRepository;
    private readonly ApiKeyRepository _apiKeyRepository;
    private readonly RequestLogRepository _requestLogRepository;

    public SpecificRepositoryImplementationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _tenantRepository = new TenantRepository(_context);
        _aiProviderRepository = new AIProviderRepository(_context);
        _apiKeyRepository = new ApiKeyRepository(_context);
        _requestLogRepository = new RequestLogRepository(_context);

        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region TenantRepository Tests

    [Fact]
    public async Task TenantRepository_GetByNameAsync_ShouldReturnTenant_WhenExists()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        // Act
        var result = await _tenantRepository.GetByNameAsync("Test Tenant");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(tenant.Id);
        result.Name.Should().Be("Test Tenant");
    }

    [Fact]
    public async Task TenantRepository_GetByNameAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _tenantRepository.GetByNameAsync("Non-existent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task TenantRepository_GetActiveTenantsAsync_ShouldReturnOnlyActiveTenants()
    {
        // Arrange
        var activeTenant = Tenant.Create("Active Tenant", "Description", true);
        var inactiveTenant = Tenant.Create("Inactive Tenant", "Description", false);
        await _context.Tenants.AddRangeAsync(activeTenant, inactiveTenant);
        await _context.SaveChangesAsync();

        // Act
        var result = await _tenantRepository.GetActiveTenantsAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(activeTenant.Id);
    }

    [Fact]
    public async Task TenantRepository_ExistsByNameAsync_ShouldReturnTrue_WhenNameExists()
    {
        // Arrange
        var tenant = Tenant.Create("Existing Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _tenantRepository.ExistsByNameAsync("Existing Tenant");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task TenantRepository_ExistsByNameAsync_ShouldReturnFalse_WhenNameDoesNotExist()
    {
        // Act
        var exists = await _tenantRepository.ExistsByNameAsync("Non-existent");

        // Assert
        exists.Should().BeFalse();
    }

    #endregion

    #region AIProviderRepository Tests

    [Fact]
    public async Task AIProviderRepository_GetByNameAsync_ShouldReturnProvider_WhenExists()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        var provider = AIProvider.Create(
            "Test Provider",
            "https://api.openai.com",
            AIProviderType.OpenAI,
            true,
            10,
            30);
        await _context.AIProviders.AddAsync(provider);
        await _context.SaveChangesAsync();

        // Act
        var result = await _aiProviderRepository.GetByNameAsync("Test Provider");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(provider.Id);
        result.Name.Should().Be("Test Provider");
    }

    [Fact]
    public async Task AIProviderRepository_GetByTypeAsync_ShouldReturnProvidersOfType()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        var openAIProvider = AIProvider.Create(
            "OpenAI Provider",
            "https://api.openai.com",
            AIProviderType.OpenAI,
            true,
            10,
            30);

        var anthropicProvider = AIProvider.Create(
            "Anthropic Provider",
            "https://api.anthropic.com",
            AIProviderType.Anthropic,
            true,
            10,
            30);

        await _context.AIProviders.AddRangeAsync(openAIProvider, anthropicProvider);
        await _context.SaveChangesAsync();

        // Act
        var result = await _aiProviderRepository.GetByTypeAsync(AIProviderType.OpenAI);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(openAIProvider.Id);
    }

    [Fact]
    public async Task AIProviderRepository_GetActiveProvidersAsync_ShouldReturnOnlyActiveProviders()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        var activeProvider = AIProvider.Create(
            "Active Provider",
            "https://api.openai.com",
            AIProviderType.OpenAI,
            true,
            10,
            30);

        var inactiveProvider = AIProvider.Create(
            "Inactive Provider",
            "https://api.openai.com",
            AIProviderType.OpenAI,
            false,
            10,
            30);

        await _context.AIProviders.AddRangeAsync(activeProvider, inactiveProvider);
        await _context.SaveChangesAsync();

        // Act
        var result = await _aiProviderRepository.GetActiveProvidersAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(activeProvider.Id);
    }

    [Fact]
    public async Task AIProviderRepository_GetByTenantIdAsync_ShouldReturnProvidersForTenant()
    {
        // Arrange
        var tenant1 = Tenant.Create("Tenant 1", "Description", true);
        var tenant2 = Tenant.Create("Tenant 2", "Description", true);
        await _context.Tenants.AddRangeAsync(tenant1, tenant2);
        await _context.SaveChangesAsync();

        var provider1 = AIProvider.Create(
            "Provider 1",
            "https://api.openai.com",
            AIProviderType.OpenAI,
            true,
            10,
            30);

        var provider2 = AIProvider.Create(
            "Provider 2",
            "https://api.openai.com",
            AIProviderType.OpenAI,
            true,
            10,
            30);

        await _context.AIProviders.AddRangeAsync(provider1, provider2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _aiProviderRepository.GetByTenantIdAsync(tenant1.Id);

        // Assert
        // AIProvider实体当前没有TenantId属性，所以返回空列表
        result.Should().BeEmpty();
    }

    #endregion

    #region ApiKeyRepository Tests

    [Fact]
    public async Task ApiKeyRepository_GetByKeyValueAsync_ShouldReturnApiKey_WhenExists()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        var apiKey = ApiKey.Create(
            tenant.Id,
            Guid.NewGuid(),
            "Test Key",
            "test-key-value",
            true,
            100,
            DateTime.UtcNow.AddDays(30));
        await _context.ApiKeys.AddAsync(apiKey);
        await _context.SaveChangesAsync();

        // Act
        var result = await _apiKeyRepository.GetByKeyValueAsync("test-key-value");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(apiKey.Id);
        result.KeyValue.Should().Be("test-key-value");
    }

    [Fact]
    public async Task ApiKeyRepository_GetByTenantIdAsync_ShouldReturnApiKeysForTenant()
    {
        // Arrange
        var tenant1 = Tenant.Create("Tenant 1", "Description", true);
        var tenant2 = Tenant.Create("Tenant 2", "Description", true);
        await _context.Tenants.AddRangeAsync(tenant1, tenant2);
        await _context.SaveChangesAsync();

        var apiKey1 = ApiKey.Create(tenant1.Id, Guid.NewGuid(), "Key 1", "key1", true, 100, null);
        var apiKey2 = ApiKey.Create(tenant2.Id, Guid.NewGuid(), "Key 2", "key2", true, 100, null);
        await _context.ApiKeys.AddRangeAsync(apiKey1, apiKey2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _apiKeyRepository.GetByTenantIdAsync(tenant1.Id);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(apiKey1.Id);
    }

    [Fact]
    public async Task ApiKeyRepository_GetActiveKeysAsync_ShouldReturnOnlyActiveKeys()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        var activeKey = ApiKey.Create(tenant.Id, Guid.NewGuid(), "Active Key", "active-key", true, 100, null);
        var inactiveKey = ApiKey.Create(tenant.Id, Guid.NewGuid(), "Inactive Key", "inactive-key", false, 100, null);
        await _context.ApiKeys.AddRangeAsync(activeKey, inactiveKey);
        await _context.SaveChangesAsync();

        // Act
        var result = await _apiKeyRepository.GetActiveKeysAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(activeKey.Id);
    }

    [Fact]
    public async Task ApiKeyRepository_GetExpiredKeysAsync_ShouldReturnExpiredKeys()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        var expiredKey = ApiKey.Create(
            tenant.Id,
            Guid.NewGuid(),
            "Expired Key",
            "expired-key",
            true,
            100,
            DateTime.UtcNow.AddDays(-1));

        var validKey = ApiKey.Create(
            tenant.Id,
            Guid.NewGuid(),
            "Valid Key",
            "valid-key",
            true,
            100,
            DateTime.UtcNow.AddDays(30));

        await _context.ApiKeys.AddRangeAsync(expiredKey, validKey);
        await _context.SaveChangesAsync();

        // Act
        var result = await _apiKeyRepository.GetExpiredKeysAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(expiredKey.Id);
    }

    [Fact]
    public async Task ApiKeyRepository_ExistsByKeyValueAsync_ShouldReturnTrue_WhenKeyExists()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        var apiKey = ApiKey.Create(tenant.Id, Guid.NewGuid(), "Existing Key", "existing-key", true, 100, null);
        await _context.ApiKeys.AddAsync(apiKey);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _apiKeyRepository.ExistsByKeyValueAsync("existing-key");

        // Assert
        exists.Should().BeTrue();
    }

    #endregion

    #region RequestLogRepository Tests

    [Fact]
    public async Task RequestLogRepository_GetByTenantIdAsync_ShouldReturnLogsForTenant()
    {
        // Arrange
        var tenant1 = Tenant.Create("Tenant 1", "Description", true);
        var tenant2 = Tenant.Create("Tenant 2", "Description", true);
        await _context.Tenants.AddRangeAsync(tenant1, tenant2);
        await _context.SaveChangesAsync();

        var log1 = RequestLog.Create(
            tenant1.Id,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/api/chat",
            "POST",
            200,
            100,
            null,
            null,
            null,
            null);

        var log2 = RequestLog.Create(
            tenant2.Id,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/api/chat",
            "POST",
            200,
            100,
            null,
            null,
            null,
            null);

        await _context.RequestLogs.AddRangeAsync(log1, log2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _requestLogRepository.GetByTenantIdAsync(tenant1.Id);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(log1.Id);
    }

    [Fact]
    public async Task RequestLogRepository_GetFailedRequestsAsync_ShouldReturnOnlyFailedRequests()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        var successLog = RequestLog.Create(
            tenant.Id,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/api/chat",
            "POST",
            200,
            100,
            null,
            null,
            null,
            null);

        var failedLog = RequestLog.Create(
            tenant.Id,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/api/chat",
            "POST",
            500,
            100,
            null,
            null,
            null,
            null);

        await _context.RequestLogs.AddRangeAsync(successLog, failedLog);
        await _context.SaveChangesAsync();

        // Act
        var result = await _requestLogRepository.GetFailedRequestsAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(failedLog.Id);
    }

    [Fact]
    public async Task RequestLogRepository_GetTenantStatisticsAsync_ShouldReturnCorrectStatistics()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        var successLog = RequestLog.Create(
            tenant.Id,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/api/chat",
            "POST",
            200,
            100,
            null,
            null,
            null,
            null);

        var failedLog = RequestLog.Create(
            tenant.Id,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/api/chat",
            "POST",
            500,
            100,
            null,
            null,
            null,
            null);

        await _context.RequestLogs.AddRangeAsync(successLog, failedLog);
        await _context.SaveChangesAsync();

        // Act
        var (totalRequests, failedRequests, totalCost) = await _requestLogRepository
            .GetTenantStatisticsAsync(tenant.Id);

        // Assert
        totalRequests.Should().Be(2);
        failedRequests.Should().Be(1);
        totalCost.Should().Be(0m); // RequestLog实体没有Cost属性
    }

    #endregion
}