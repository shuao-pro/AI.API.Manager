using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Repositories;
using FluentAssertions;
using Xunit;

namespace AI.API.Manager.Tests.Domain.Repositories;

public class SpecificRepositoryTests
{
    [Fact]
    public void ITenantRepository_ShouldExtendIRepository()
    {
        // Arrange
        var type = typeof(ITenantRepository);

        // Assert
        type.GetInterfaces().Should().Contain(typeof(IRepository<Tenant>));
    }

    [Fact]
    public void ITenantRepository_ShouldDefineSpecificMethods()
    {
        // Arrange
        var type = typeof(ITenantRepository);

        // Assert
        type.GetMethod("GetByNameAsync").Should().NotBeNull();
        type.GetMethod("GetActiveTenantsAsync").Should().NotBeNull();
        type.GetMethod("ExistsByNameAsync").Should().NotBeNull();
    }

    [Fact]
    public void IAIProviderRepository_ShouldExtendIRepository()
    {
        // Arrange
        var type = typeof(IAIProviderRepository);

        // Assert
        type.GetInterfaces().Should().Contain(typeof(IRepository<AIProvider>));
    }

    [Fact]
    public void IAIProviderRepository_ShouldDefineSpecificMethods()
    {
        // Arrange
        var type = typeof(IAIProviderRepository);

        // Assert
        type.GetMethod("GetByNameAsync").Should().NotBeNull();
        type.GetMethod("GetByTypeAsync").Should().NotBeNull();
        type.GetMethod("GetActiveProvidersAsync").Should().NotBeNull();
        type.GetMethod("GetByTenantIdAsync").Should().NotBeNull();
    }

    [Fact]
    public void IApiKeyRepository_ShouldExtendIRepository()
    {
        // Arrange
        var type = typeof(IApiKeyRepository);

        // Assert
        type.GetInterfaces().Should().Contain(typeof(IRepository<ApiKey>));
    }

    [Fact]
    public void IApiKeyRepository_ShouldDefineSpecificMethods()
    {
        // Arrange
        var type = typeof(IApiKeyRepository);

        // Assert
        type.GetMethod("GetByKeyValueAsync").Should().NotBeNull();
        type.GetMethod("GetByTenantIdAsync").Should().NotBeNull();
        type.GetMethod("GetActiveKeysAsync").Should().NotBeNull();
        type.GetMethod("GetExpiredKeysAsync").Should().NotBeNull();
        type.GetMethod("ExistsByKeyValueAsync").Should().NotBeNull();
    }

    [Fact]
    public void IRequestLogRepository_ShouldExtendIRepository()
    {
        // Arrange
        var type = typeof(IRequestLogRepository);

        // Assert
        type.GetInterfaces().Should().Contain(typeof(IRepository<RequestLog>));
    }

    [Fact]
    public void IRequestLogRepository_ShouldDefineSpecificMethods()
    {
        // Arrange
        var type = typeof(IRequestLogRepository);

        // Assert
        type.GetMethod("GetByTenantIdAsync").Should().NotBeNull();
        type.GetMethod("GetByApiKeyIdAsync").Should().NotBeNull();
        type.GetMethod("GetByProviderIdAsync").Should().NotBeNull();
        type.GetMethod("GetByTimeRangeAsync").Should().NotBeNull();
        type.GetMethod("GetFailedRequestsAsync").Should().NotBeNull();
        type.GetMethod("GetTenantStatisticsAsync").Should().NotBeNull();
    }
}