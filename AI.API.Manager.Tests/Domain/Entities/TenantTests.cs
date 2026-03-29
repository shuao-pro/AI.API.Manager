using AI.API.Manager.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AI.API.Manager.Tests.Domain.Entities;

public class TenantTests
{
    [Fact]
    public void Create_ShouldCreateTenantWithValidProperties()
    {
        // Arrange
        var name = "Test Tenant";
        var description = "Test Description";
        var isActive = true;

        // Act
        var tenant = Tenant.Create(name, description, isActive);

        // Assert
        tenant.Should().NotBeNull();
        tenant.Id.Should().NotBeEmpty();
        tenant.Name.Should().Be(name);
        tenant.Description.Should().Be(description);
        tenant.IsActive.Should().Be(isActive);
        tenant.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        tenant.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameIsNull()
    {
        // Arrange
        var action = () => Tenant.Create(null!, "Description", true);

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be null or whitespace (Parameter 'name')");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowArgumentException_WhenNameIsEmptyOrWhiteSpace(string invalidName)
    {
        // Arrange
        var action = () => Tenant.Create(invalidName, "Description", true);

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be null or whitespace (Parameter 'name')");
    }

    [Fact]
    public void Update_ShouldUpdateTenantProperties()
    {
        // Arrange
        var tenant = Tenant.Create("Original Name", "Original Description", true);
        var newName = "Updated Name";
        var newDescription = "Updated Description";
        var newIsActive = false;

        // Act
        tenant.Update(newName, newDescription, newIsActive);

        // Assert
        tenant.Name.Should().Be(newName);
        tenant.Description.Should().Be(newDescription);
        tenant.IsActive.Should().Be(newIsActive);
        tenant.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Update_ShouldThrowArgumentException_WhenNameIsNullOrWhiteSpace()
    {
        // Arrange
        var tenant = Tenant.Create("Valid Name", "Description", true);

        // Act
        var action = () => tenant.Update(null!, "Description", true);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be null or whitespace (Parameter 'name')");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", true);

        // Act
        tenant.Deactivate();

        // Assert
        tenant.IsActive.Should().BeFalse();
        tenant.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", "Description", false);

        // Act
        tenant.Activate();

        // Assert
        tenant.IsActive.Should().BeTrue();
        tenant.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}