using AI.API.Manager.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AI.API.Manager.Tests.Domain.Entities;

public class ApiKeyTests
{
    [Fact]
    public void Create_ShouldCreateApiKeyWithValidProperties()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var name = "Production API Key";
        var keyValue = "sk-test-1234567890";
        var isActive = true;
        var rateLimitPerMinute = 100;
        var expiresAt = DateTime.UtcNow.AddDays(30);

        // Act
        var apiKey = ApiKey.Create(
            tenantId,
            providerId,
            name,
            keyValue,
            isActive,
            rateLimitPerMinute,
            expiresAt);

        // Assert
        apiKey.Should().NotBeNull();
        apiKey.Id.Should().NotBeEmpty();
        apiKey.TenantId.Should().Be(tenantId);
        apiKey.ProviderId.Should().Be(providerId);
        apiKey.Name.Should().Be(name);
        apiKey.KeyValue.Should().Be(keyValue);
        apiKey.IsActive.Should().Be(isActive);
        apiKey.RateLimitPerMinute.Should().Be(rateLimitPerMinute);
        apiKey.ExpiresAt.Should().Be(expiresAt);
        apiKey.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        apiKey.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameIsNull()
    {
        // Arrange
        var action = () => ApiKey.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null!,
            "sk-test-1234567890",
            true,
            100,
            DateTime.UtcNow.AddDays(30));

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
        var action = () => ApiKey.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            invalidName,
            "sk-test-1234567890",
            true,
            100,
            DateTime.UtcNow.AddDays(30));

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be null or whitespace (Parameter 'name')");
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenKeyValueIsNull()
    {
        // Arrange
        var action = () => ApiKey.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Key",
            null!,
            true,
            100,
            DateTime.UtcNow.AddDays(30));

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("KeyValue cannot be null or whitespace (Parameter 'keyValue')");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowArgumentException_WhenKeyValueIsEmptyOrWhiteSpace(string invalidKeyValue)
    {
        // Arrange
        var action = () => ApiKey.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Key",
            invalidKeyValue,
            true,
            100,
            DateTime.UtcNow.AddDays(30));

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("KeyValue cannot be null or whitespace (Parameter 'keyValue')");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldThrowArgumentException_WhenRateLimitPerMinuteIsInvalid(int invalidRateLimit)
    {
        // Arrange
        var action = () => ApiKey.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Key",
            "sk-test-1234567890",
            true,
            invalidRateLimit,
            DateTime.UtcNow.AddDays(30));

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("RateLimitPerMinute must be greater than 0 (Parameter 'rateLimitPerMinute')");
    }

    [Fact]
    public void Create_ShouldSetExpiresAtToNull_WhenNotProvided()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var name = "Test Key";
        var keyValue = "sk-test-1234567890";

        // Act
        var apiKey = ApiKey.Create(
            tenantId,
            providerId,
            name,
            keyValue,
            true,
            100,
            null);

        // Assert
        apiKey.ExpiresAt.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldUpdateApiKeyProperties()
    {
        // Arrange
        var apiKey = ApiKey.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Original Name",
            "sk-original-123",
            true,
            100,
            DateTime.UtcNow.AddDays(30));

        var newName = "Updated Name";
        var newKeyValue = "sk-updated-456";
        var newIsActive = false;
        var newRateLimitPerMinute = 200;
        var newExpiresAt = DateTime.UtcNow.AddDays(60);

        // Act
        apiKey.Update(
            newName,
            newKeyValue,
            newIsActive,
            newRateLimitPerMinute,
            newExpiresAt);

        // Assert
        apiKey.Name.Should().Be(newName);
        apiKey.KeyValue.Should().Be(newKeyValue);
        apiKey.IsActive.Should().Be(newIsActive);
        apiKey.RateLimitPerMinute.Should().Be(newRateLimitPerMinute);
        apiKey.ExpiresAt.Should().Be(newExpiresAt);
        apiKey.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Update_ShouldSetExpiresAtToNull_WhenNotProvided()
    {
        // Arrange
        var apiKey = ApiKey.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Key",
            "sk-test-123",
            true,
            100,
            DateTime.UtcNow.AddDays(30));

        // Act
        apiKey.Update(
            "Updated Name",
            "sk-updated-456",
            false,
            200,
            null);

        // Assert
        apiKey.ExpiresAt.Should().BeNull();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var apiKey = ApiKey.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Key",
            "sk-test-123",
            true,
            100,
            DateTime.UtcNow.AddDays(30));

        // Act
        apiKey.Deactivate();

        // Assert
        apiKey.IsActive.Should().BeFalse();
        apiKey.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var apiKey = ApiKey.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Key",
            "sk-test-123",
            false,
            100,
            DateTime.UtcNow.AddDays(30));

        // Act
        apiKey.Activate();

        // Assert
        apiKey.IsActive.Should().BeTrue();
        apiKey.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void IsExpired_ShouldReturnTrue_WhenExpiresAtIsInPast()
    {
        // Arrange
        var apiKey = ApiKey.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Key",
            "sk-test-123",
            true,
            100,
            DateTime.UtcNow.AddDays(-1));

        // Act
        var isExpired = apiKey.IsExpired();

        // Assert
        isExpired.Should().BeTrue();
    }

    [Fact]
    public void IsExpired_ShouldReturnFalse_WhenExpiresAtIsInFuture()
    {
        // Arrange
        var apiKey = ApiKey.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Key",
            "sk-test-123",
            true,
            100,
            DateTime.UtcNow.AddDays(1));

        // Act
        var isExpired = apiKey.IsExpired();

        // Assert
        isExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_ShouldReturnFalse_WhenExpiresAtIsNull()
    {
        // Arrange
        var apiKey = ApiKey.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Key",
            "sk-test-123",
            true,
            100,
            null);

        // Act
        var isExpired = apiKey.IsExpired();

        // Assert
        isExpired.Should().BeFalse();
    }
}