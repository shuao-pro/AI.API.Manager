using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Infrastructure.Data;
using AI.API.Manager.Infrastructure.Data.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Xunit;

namespace AI.API.Manager.Tests.Infrastructure.Data.Repositories;

public class RepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Repository<Tenant> _repository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new Repository<Tenant>(_context);

        // 确保数据库已创建
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(tenant.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(tenant.Id);
        result.Name.Should().Be(tenant.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var tenant1 = Tenant.Create("Tenant 1", "Description 1", true);
        var tenant2 = Tenant.Create("Tenant 2", "Description 2", false);
        await _context.Tenants.AddRangeAsync(tenant1, tenant2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Id == tenant1.Id);
        result.Should().Contain(t => t.Id == tenant2.Id);
    }

    [Fact]
    public async Task GetAsync_WithPredicate_ShouldReturnFilteredEntities()
    {
        // Arrange
        var activeTenant = Tenant.Create("Active Tenant", "Description", true);
        var inactiveTenant = Tenant.Create("Inactive Tenant", "Description", false);
        await _context.Tenants.AddRangeAsync(activeTenant, inactiveTenant);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(t => t.IsActive, cancellationToken: default);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(activeTenant.Id);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ShouldReturnFirstMatchingEntity()
    {
        // Arrange
        var tenant1 = Tenant.Create("Tenant A", "Description", true);
        var tenant2 = Tenant.Create("Tenant B", "Description", true);
        await _context.Tenants.AddRangeAsync(tenant1, tenant2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FirstOrDefaultAsync(t => t.Name.Contains("A"));

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(tenant1.Id);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ShouldReturnNull_WhenNoMatchingEntity()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FirstOrDefaultAsync(t => t.Name == "Non-existent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityToDatabase()
    {
        // Arrange
        var tenant = Tenant.Create("New Tenant", "Description", true);

        // Act
        var result = await _repository.AddAsync(tenant);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(tenant.Id);

        var savedTenant = await _context.Tenants.FindAsync(tenant.Id);
        savedTenant.Should().NotBeNull();
        savedTenant!.Name.Should().Be("New Tenant");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntityInDatabase()
    {
        // Arrange
        var tenant = Tenant.Create("Original Name", "Original Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        tenant.Update("Updated Name", "Updated Description", false);

        // Act
        var result = await _repository.UpdateAsync(tenant);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name");
        result.IsActive.Should().BeFalse();

        var updatedTenant = await _context.Tenants.FindAsync(tenant.Id);
        updatedTenant.Should().NotBeNull();
        updatedTenant!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntityFromDatabase()
    {
        // Arrange
        var tenant = Tenant.Create("To Delete", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(tenant);

        // Assert
        var deletedTenant = await _context.Tenants.FindAsync(tenant.Id);
        deletedTenant.Should().BeNull();
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldRemoveEntity_WhenEntityExists()
    {
        // Arrange
        var tenant = Tenant.Create("To Delete", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteByIdAsync(tenant.Id);

        // Assert
        var deletedTenant = await _context.Tenants.FindAsync(tenant.Id);
        deletedTenant.Should().BeNull();
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldDoNothing_WhenEntityDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        await _repository.DeleteByIdAsync(nonExistentId);

        // Assert
        // 不应该抛出异常
        Assert.True(true);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenEntityExists()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsAsync(t => t.Name == "Test Tenant");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);
        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsAsync(t => t.Name == "Non-existent");

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ShouldReturnCorrectCount()
    {
        // Arrange
        var tenant1 = Tenant.Create("Active Tenant", "Description", true);
        var tenant2 = Tenant.Create("Inactive Tenant", "Description", false);
        await _context.Tenants.AddRangeAsync(tenant1, tenant2);
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.CountAsync(t => t.IsActive);

        // Assert
        count.Should().Be(1);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnTotalCount()
    {
        // Arrange
        var tenant1 = Tenant.Create("Tenant 1", "Description", true);
        var tenant2 = Tenant.Create("Tenant 2", "Description", false);
        await _context.Tenants.AddRangeAsync(tenant1, tenant2);
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.CountAsync();

        // Assert
        count.Should().Be(2);
    }
}