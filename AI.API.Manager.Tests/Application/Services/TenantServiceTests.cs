using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Application.Services;
using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AI.API.Manager.Tests.Application.Services;

public class TenantServiceTests
{
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<TenantService>> _loggerMock;
    private readonly TenantService _tenantService;

    public TenantServiceTests()
    {
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _loggerMock = new Mock<ILogger<TenantService>>();

        // 创建真实的Mapper用于测试
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AI.API.Manager.Application.Mappings.MappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        _tenantService = new TenantService(_tenantRepositoryMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTenant_WhenTenantExists()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenant = Tenant.Create("Test Tenant", "Test Description", isActive: true);

        _tenantRepositoryMock
            .Setup(r => r.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        // Act
        var result = await _tenantService.GetByIdAsync(tenantId);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Tenant");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTenantDoesNotExist()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _tenantRepositoryMock
            .Setup(r => r.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tenant?)null);

        // Act
        var result = await _tenantService.GetByIdAsync(tenantId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTenant_WhenNameIsUnique()
    {
        // Arrange
        var request = new CreateTenantRequest
        {
            Name = "New Tenant",
            Description = "New Description"
        };

        _tenantRepositoryMock
            .Setup(r => r.ExistsByNameAsync(request.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Tenant? capturedTenant = null;
        _tenantRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()))
            .Callback<Tenant, CancellationToken>((tenant, _) => capturedTenant = tenant)
            .ReturnsAsync((Tenant tenant, CancellationToken _) => tenant);

        // Act
        var result = await _tenantService.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Description.Should().Be(request.Description);
        result.IsActive.Should().BeTrue();

        capturedTenant.Should().NotBeNull();
        capturedTenant!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenNameAlreadyExists()
    {
        // Arrange
        var request = new CreateTenantRequest
        {
            Name = "Existing Tenant",
            Description = "Description"
        };

        _tenantRepositoryMock
            .Setup(r => r.ExistsByNameAsync(request.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tenantService.CreateAsync(request));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTenant_WhenTenantExists()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var existingTenant = Tenant.Create("Old Name", "Old Description", isActive: true);

        var request = new UpdateTenantRequest
        {
            Name = "Updated Name",
            Description = "Updated Description",
            IsActive = false
        };

        _tenantRepositoryMock
            .Setup(r => r.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTenant);

        Tenant? capturedTenant = null;
        _tenantRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()))
            .Callback<Tenant, CancellationToken>((tenant, _) => capturedTenant = tenant)
            .ReturnsAsync((Tenant tenant, CancellationToken _) => tenant);

        // Act
        var result = await _tenantService.UpdateAsync(tenantId, request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Description.Should().Be(request.Description);
        result.IsActive.Should().Be(request.IsActive);

        capturedTenant.Should().NotBeNull();
        capturedTenant!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenTenantDoesNotExist()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new UpdateTenantRequest
        {
            Name = "Updated Name",
            Description = "Updated Description",
            IsActive = false
        };

        _tenantRepositoryMock
            .Setup(r => r.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tenant?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tenantService.UpdateAsync(tenantId, request));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteTenant_WhenTenantExists()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var existingTenant = Tenant.Create("Test Tenant", null, isActive: true);

        _tenantRepositoryMock
            .Setup(r => r.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTenant);

        _tenantRepositoryMock
            .Setup(r => r.DeleteByIdAsync(tenantId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _tenantService.DeleteAsync(tenantId);

        // Assert
        _tenantRepositoryMock.Verify(
            r => r.DeleteByIdAsync(tenantId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenTenantDoesNotExist()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _tenantRepositoryMock
            .Setup(r => r.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tenant?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tenantService.DeleteAsync(tenantId));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTenants()
    {
        // Arrange
        var tenants = new List<Tenant>
        {
            Tenant.Create("Tenant 1", null, isActive: true),
            Tenant.Create("Tenant 2", null, isActive: false)
        };

        _tenantRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenants);

        // Act
        var result = await _tenantService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Name == "Tenant 1");
        result.Should().Contain(t => t.Name == "Tenant 2");
    }

    [Fact]
    public async Task GetActiveTenantsAsync_ShouldReturnOnlyActiveTenants()
    {
        // Arrange
        var activeTenants = new List<Tenant>
        {
            Tenant.Create("Active Tenant 1", null, isActive: true),
            Tenant.Create("Active Tenant 2", null, isActive: true)
        };

        _tenantRepositoryMock
            .Setup(r => r.GetActiveTenantsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeTenants);

        // Act
        var result = await _tenantService.GetActiveTenantsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.IsActive == true);
    }
}