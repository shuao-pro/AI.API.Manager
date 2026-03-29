using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AI.API.Manager.Tests.Infrastructure.Data;

public class ApplicationDbContextTests
{
    [Fact]
    public void DbContext_ShouldHaveTenantsDbSet()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.Tenants);
    }

    [Fact]
    public void DbContext_ShouldHaveAIProvidersDbSet()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.AIProviders);
    }

    [Fact]
    public void DbContext_ShouldHaveApiKeysDbSet()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.ApiKeys);
    }

    [Fact]
    public void DbContext_ShouldHaveRequestLogsDbSet()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        // Act
        using var context = new ApplicationDbContext(options);

        // Assert
        Assert.NotNull(context.RequestLogs);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSetTimestamps()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        using var context = new ApplicationDbContext(options);

        var tenant = Tenant.Create("Test Tenant", "Description", true);

        // Act
        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        // Assert - timestamps are now shadow properties managed by EF Core
        var entry = context.Entry(tenant);
        var createdAt = entry.Property<DateTime>("CreatedAt").CurrentValue;
        var updatedAt = entry.Property<DateTime>("UpdatedAt").CurrentValue;

        Assert.NotEqual(default, createdAt);
        Assert.NotEqual(default, updatedAt);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldUpdateUpdatedAt()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        using var context = new ApplicationDbContext(options);

        var tenant = Tenant.Create("Test Tenant", "Description", true);
        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        var entry = context.Entry(tenant);
        var originalUpdatedAt = entry.Property<DateTime>("UpdatedAt").CurrentValue;

        // Act
        tenant.Update("Updated Name", "Updated Description", false);
        await context.SaveChangesAsync();

        // Assert
        var newUpdatedAt = entry.Property<DateTime>("UpdatedAt").CurrentValue;
        Assert.True(newUpdatedAt > originalUpdatedAt);
    }
}