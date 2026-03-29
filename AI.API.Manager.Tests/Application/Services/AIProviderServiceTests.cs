using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Application.Services;
using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Enums;
using AI.API.Manager.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AI.API.Manager.Tests.Application.Services;

public class AIProviderServiceTests
{
    private readonly Mock<IAIProviderRepository> _aiProviderRepositoryMock;
    private readonly Mock<ILogger<AIProviderService>> _loggerMock;
    private readonly AIProviderService _aiProviderService;

    public AIProviderServiceTests()
    {
        _aiProviderRepositoryMock = new Mock<IAIProviderRepository>();
        _loggerMock = new Mock<ILogger<AIProviderService>>();
        _aiProviderService = new AIProviderService(_aiProviderRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAIProvider_WhenProviderExists()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        var provider = AIProvider.Create(
            "Test Provider",
            "https://api.openai.com",
            AIProviderType.OpenAI,
            isActive: true,
            maxConcurrentRequests: 10,
            requestTimeoutSeconds: 30);

        _aiProviderRepositoryMock
            .Setup(r => r.GetByIdAsync(providerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(provider);

        // Act
        var result = await _aiProviderService.GetByIdAsync(providerId);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Provider");
        result.ProviderType.Should().Be(AIProviderType.OpenAI);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProviderDoesNotExist()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        _aiProviderRepositoryMock
            .Setup(r => r.GetByIdAsync(providerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AIProvider?)null);

        // Act
        var result = await _aiProviderService.GetByIdAsync(providerId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAIProvider_WhenDataIsValid()
    {
        // Arrange
        var request = new CreateAIProviderRequest
        {
            Name = "New Provider",
            ProviderType = AIProviderType.Anthropic,
            BaseUrl = "https://api.anthropic.com",
            ApiKey = "sk-ant-test-key",
            IsActive = true,
            MaxTokens = 4096,
            TimeoutSeconds = 30
        };

        _aiProviderRepositoryMock
            .Setup(r => r.GetByNameAsync(request.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AIProvider?)null);

        AIProvider? capturedProvider = null;
        _aiProviderRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<AIProvider>(), It.IsAny<CancellationToken>()))
            .Callback<AIProvider, CancellationToken>((provider, _) => capturedProvider = provider)
            .ReturnsAsync((AIProvider provider, CancellationToken _) => provider);

        // Act
        var result = await _aiProviderService.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.ProviderType.Should().Be(request.ProviderType);
        result.BaseUrl.Should().Be(request.BaseUrl);
        result.IsActive.Should().Be(request.IsActive);
        result.MaxConcurrentRequests.Should().Be(10); // 默认值
        result.RequestTimeoutSeconds.Should().Be(request.TimeoutSeconds);

        capturedProvider.Should().NotBeNull();
        capturedProvider!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenNameAlreadyExists()
    {
        // Arrange
        var request = new CreateAIProviderRequest
        {
            Name = "Existing Provider",
            ProviderType = AIProviderType.OpenAI,
            BaseUrl = "https://api.openai.com",
            ApiKey = "sk-test-key"
        };

        var existingProvider = AIProvider.Create(
            "Existing Provider",
            "https://api.openai.com",
            AIProviderType.OpenAI,
            isActive: true,
            maxConcurrentRequests: 10,
            requestTimeoutSeconds: 30);

        _aiProviderRepositoryMock
            .Setup(r => r.GetByNameAsync(request.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProvider);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _aiProviderService.CreateAsync(request));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAIProvider_WhenProviderExists()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        var existingProvider = AIProvider.Create(
            "Old Provider",
            "https://old.api.com",
            AIProviderType.OpenAI,
            isActive: true,
            maxConcurrentRequests: 10,
            requestTimeoutSeconds: 30);

        var request = new UpdateAIProviderRequest
        {
            Name = "Updated Provider",
            ProviderType = AIProviderType.Anthropic,
            BaseUrl = "https://updated.api.com",
            ApiKey = "sk-updated-key",
            IsActive = false,
            MaxTokens = 8192,
            TimeoutSeconds = 60
        };

        _aiProviderRepositoryMock
            .Setup(r => r.GetByIdAsync(providerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProvider);

        AIProvider? capturedProvider = null;
        _aiProviderRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<AIProvider>(), It.IsAny<CancellationToken>()))
            .Callback<AIProvider, CancellationToken>((provider, _) => capturedProvider = provider)
            .ReturnsAsync((AIProvider provider, CancellationToken _) => provider);

        // Act
        var result = await _aiProviderService.UpdateAsync(providerId, request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.ProviderType.Should().Be(request.ProviderType);
        result.BaseUrl.Should().Be(request.BaseUrl);
        result.IsActive.Should().Be(request.IsActive);
        result.MaxConcurrentRequests.Should().Be(10); // 默认值
        result.RequestTimeoutSeconds.Should().Be(request.TimeoutSeconds);

        capturedProvider.Should().NotBeNull();
        capturedProvider!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenProviderDoesNotExist()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        var request = new UpdateAIProviderRequest
        {
            Name = "Updated Provider",
            ProviderType = AIProviderType.OpenAI,
            BaseUrl = "https://api.openai.com",
            ApiKey = "sk-test-key"
        };

        _aiProviderRepositoryMock
            .Setup(r => r.GetByIdAsync(providerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AIProvider?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _aiProviderService.UpdateAsync(providerId, request));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteAIProvider_WhenProviderExists()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        var existingProvider = AIProvider.Create(
            "Test Provider",
            "https://api.openai.com",
            AIProviderType.OpenAI,
            isActive: true,
            maxConcurrentRequests: 10,
            requestTimeoutSeconds: 30);

        _aiProviderRepositoryMock
            .Setup(r => r.GetByIdAsync(providerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProvider);

        _aiProviderRepositoryMock
            .Setup(r => r.DeleteByIdAsync(providerId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _aiProviderService.DeleteAsync(providerId);

        // Assert
        _aiProviderRepositoryMock.Verify(
            r => r.DeleteByIdAsync(providerId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenProviderDoesNotExist()
    {
        // Arrange
        var providerId = Guid.NewGuid();
        _aiProviderRepositoryMock
            .Setup(r => r.GetByIdAsync(providerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AIProvider?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _aiProviderService.DeleteAsync(providerId));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllAIProviders()
    {
        // Arrange
        var providers = new List<AIProvider>
        {
            AIProvider.Create("Provider 1", "https://api1.com", AIProviderType.OpenAI, isActive: true, maxConcurrentRequests: 10, requestTimeoutSeconds: 30),
            AIProvider.Create("Provider 2", "https://api2.com", AIProviderType.Anthropic, isActive: false, maxConcurrentRequests: 10, requestTimeoutSeconds: 30)
        };

        _aiProviderRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(providers);

        // Act
        var result = await _aiProviderService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Provider 1");
        result.Should().Contain(p => p.Name == "Provider 2");
    }

    [Fact]
    public async Task GetActiveProvidersAsync_ShouldReturnOnlyActiveProviders()
    {
        // Arrange
        var activeProviders = new List<AIProvider>
        {
            AIProvider.Create("Active Provider 1", "https://api1.com", AIProviderType.OpenAI, isActive: true, maxConcurrentRequests: 10, requestTimeoutSeconds: 30),
            AIProvider.Create("Active Provider 2", "https://api2.com", AIProviderType.Anthropic, isActive: true, maxConcurrentRequests: 10, requestTimeoutSeconds: 30)
        };

        _aiProviderRepositoryMock
            .Setup(r => r.GetActiveProvidersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeProviders);

        // Act
        var result = await _aiProviderService.GetActiveProvidersAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.IsActive == true);
    }

    [Fact]
    public async Task GetByTypeAsync_ShouldReturnProvidersByType()
    {
        // Arrange
        var openAIProviders = new List<AIProvider>
        {
            AIProvider.Create("OpenAI 1", "https://api.openai.com", AIProviderType.OpenAI, isActive: true, maxConcurrentRequests: 10, requestTimeoutSeconds: 30),
            AIProvider.Create("OpenAI 2", "https://api.openai.com", AIProviderType.OpenAI, isActive: true, maxConcurrentRequests: 10, requestTimeoutSeconds: 30)
        };

        _aiProviderRepositoryMock
            .Setup(r => r.GetByTypeAsync(AIProviderType.OpenAI, It.IsAny<CancellationToken>()))
            .ReturnsAsync(openAIProviders);

        // Act
        var result = await _aiProviderService.GetByTypeAsync(AIProviderType.OpenAI);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.ProviderType == AIProviderType.OpenAI);
    }
}