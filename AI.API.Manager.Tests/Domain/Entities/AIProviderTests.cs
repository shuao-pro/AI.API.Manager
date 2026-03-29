using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace AI.API.Manager.Tests.Domain.Entities;

public class AIProviderTests
{
    [Fact]
    public void Create_ShouldCreateAIProviderWithValidProperties()
    {
        // Arrange
        var name = "OpenAI";
        var baseUrl = "https://api.openai.com/v1";
        var providerType = AIProviderType.OpenAI;
        var isActive = true;
        var maxConcurrentRequests = 10;
        var requestTimeoutSeconds = 30;

        // Act
        var provider = AIProvider.Create(
            name,
            baseUrl,
            providerType,
            isActive,
            maxConcurrentRequests,
            requestTimeoutSeconds);

        // Assert
        provider.Should().NotBeNull();
        provider.Id.Should().NotBeEmpty();
        provider.Name.Should().Be(name);
        provider.BaseUrl.Should().Be(baseUrl);
        provider.ProviderType.Should().Be(providerType);
        provider.IsActive.Should().Be(isActive);
        provider.MaxConcurrentRequests.Should().Be(maxConcurrentRequests);
        provider.RequestTimeoutSeconds.Should().Be(requestTimeoutSeconds);
        provider.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        provider.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameIsNull()
    {
        // Arrange
        var action = () => AIProvider.Create(
            null!,
            "https://api.openai.com/v1",
            AIProviderType.OpenAI,
            true,
            10,
            30);

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
        var action = () => AIProvider.Create(
            invalidName!,
            "https://api.openai.com/v1",
            AIProviderType.OpenAI,
            true,
            10,
            30);

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Name cannot be null or whitespace (Parameter 'name')");
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenBaseUrlIsNull()
    {
        // Arrange
        var action = () => AIProvider.Create(
            "OpenAI",
            null!,
            AIProviderType.OpenAI,
            true,
            10,
            30);

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("BaseUrl cannot be null or whitespace (Parameter 'baseUrl')");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowArgumentException_WhenBaseUrlIsEmptyOrWhiteSpace(string invalidBaseUrl)
    {
        // Arrange
        var action = () => AIProvider.Create(
            "OpenAI",
            invalidBaseUrl!,
            AIProviderType.OpenAI,
            true,
            10,
            30);

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("BaseUrl cannot be null or whitespace (Parameter 'baseUrl')");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldThrowArgumentException_WhenMaxConcurrentRequestsIsInvalid(int invalidMaxRequests)
    {
        // Arrange
        var action = () => AIProvider.Create(
            "OpenAI",
            "https://api.openai.com/v1",
            AIProviderType.OpenAI,
            true,
            invalidMaxRequests,
            30);

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("MaxConcurrentRequests must be greater than 0 (Parameter 'maxConcurrentRequests')");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldThrowArgumentException_WhenRequestTimeoutSecondsIsInvalid(int invalidTimeout)
    {
        // Arrange
        var action = () => AIProvider.Create(
            "OpenAI",
            "https://api.openai.com/v1",
            AIProviderType.OpenAI,
            true,
            10,
            invalidTimeout);

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("RequestTimeoutSeconds must be greater than 0 (Parameter 'requestTimeoutSeconds')");
    }

    [Fact]
    public void Update_ShouldUpdateAIProviderProperties()
    {
        // Arrange
        var provider = AIProvider.Create(
            "Original Name",
            "https://original.com",
            AIProviderType.OpenAI,
            true,
            10,
            30);

        var newName = "Updated Name";
        var newBaseUrl = "https://updated.com";
        var newProviderType = AIProviderType.Anthropic;
        var newIsActive = false;
        var newMaxConcurrentRequests = 20;
        var newRequestTimeoutSeconds = 60;

        // Act
        provider.Update(
            newName,
            newBaseUrl,
            newProviderType,
            newIsActive,
            newMaxConcurrentRequests,
            newRequestTimeoutSeconds);

        // Assert
        provider.Name.Should().Be(newName);
        provider.BaseUrl.Should().Be(newBaseUrl);
        provider.ProviderType.Should().Be(newProviderType);
        provider.IsActive.Should().Be(newIsActive);
        provider.MaxConcurrentRequests.Should().Be(newMaxConcurrentRequests);
        provider.RequestTimeoutSeconds.Should().Be(newRequestTimeoutSeconds);
        provider.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var provider = AIProvider.Create(
            "OpenAI",
            "https://api.openai.com/v1",
            AIProviderType.OpenAI,
            true,
            10,
            30);

        // Act
        provider.Deactivate();

        // Assert
        provider.IsActive.Should().BeFalse();
        provider.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var provider = AIProvider.Create(
            "OpenAI",
            "https://api.openai.com/v1",
            AIProviderType.OpenAI,
            false,
            10,
            30);

        // Act
        provider.Activate();

        // Assert
        provider.IsActive.Should().BeTrue();
        provider.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}