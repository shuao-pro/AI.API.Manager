using AI.API.Manager.API.DTOs;
using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace AI.API.Manager.Tests.IntegrationTests.Controllers;

public class AIProvidersControllerTests : BaseIntegrationTest
{
    private const string BaseUrl = "/api/aiproviders";

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoProvidersExist()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AIProviderResponse>>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Empty(apiResponse.Data);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllProviders()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var providers = new List<AIProvider>
        {
            TestDataGenerator.CreateTestAIProvider("Provider 1", "OpenAI"),
            TestDataGenerator.CreateTestAIProvider("Provider 2", "Anthropic")
        };

        await dbContext.AIProviders.AddRangeAsync(providers);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync(BaseUrl);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<AIProviderResponse>>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(2, apiResponse.Data.Count);
        Assert.Contains(apiResponse.Data, p => p.Name == "Provider 1");
        Assert.Contains(apiResponse.Data, p => p.Name == "Provider 2");
    }

    [Fact]
    public async Task GetById_ShouldReturnProvider_WhenProviderExists()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var provider = TestDataGenerator.CreateTestAIProvider("Test Provider", "OpenAI");
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{provider.Id}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AIProviderResponse>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(provider.Id, apiResponse.Data.Id);
        Assert.Equal(provider.Name, apiResponse.Data.Name);
        Assert.Equal(provider.ProviderType, apiResponse.Data.ProviderType);
        Assert.Equal(provider.BaseUrl, apiResponse.Data.BaseUrl);
        Assert.Equal(provider.IsActive, apiResponse.Data.IsActive);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenProviderDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{nonExistentId}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        AssertErrorResponse(apiResponse!, "AI_PROVIDER_NOT_FOUND");
    }

    [Fact]
    public async Task Create_ShouldCreateProvider_WhenRequestIsValid()
    {
        // Arrange
        var request = TestDataGenerator.CreateCreateAIProviderRequest("New Provider", "OpenAI");

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AIProviderResponse>>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(request.Name, apiResponse.Data.Name);
        Assert.Equal(request.ProviderType, apiResponse.Data.ProviderType);
        Assert.Equal(request.BaseUrl, apiResponse.Data.BaseUrl);
        Assert.True(apiResponse.Data.IsActive); // 默认应该是激活状态

        // 验证提供商确实被创建了
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();
        var createdProvider = await dbContext.AIProviders.FindAsync(apiResponse.Data.Id);
        Assert.NotNull(createdProvider);
        Assert.Equal(request.Name, createdProvider.Name);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CreateAIProviderRequest
        {
            Name = "",
            ProviderType = "OpenAI",
            BaseUrl = "https://api.openai.com/v1"
        };

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        AssertErrorResponse(apiResponse!, "VALIDATION_ERROR");
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenBaseUrlIsInvalid()
    {
        // Arrange
        var request = new CreateAIProviderRequest
        {
            Name = "Test Provider",
            ProviderType = "OpenAI",
            BaseUrl = "not-a-valid-url"
        };

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        AssertErrorResponse(apiResponse!, "VALIDATION_ERROR");
    }

    [Fact]
    public async Task Update_ShouldUpdateProvider_WhenProviderExists()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var provider = TestDataGenerator.CreateTestAIProvider("Old Name", "OpenAI", "https://api.openai.com/v1", isActive: true);
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.SaveChangesAsync();

        var request = TestDataGenerator.CreateUpdateAIProviderRequest("Updated Name", "Anthropic", "https://api.anthropic.com/v1", isActive: false);

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{provider.Id}", request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AIProviderResponse>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(request.Name, apiResponse.Data.Name);
        Assert.Equal(request.ProviderType, apiResponse.Data.ProviderType);
        Assert.Equal(request.BaseUrl, apiResponse.Data.BaseUrl);
        Assert.Equal(request.IsActive, apiResponse.Data.IsActive);

        // 验证数据库中的更新
        var updatedProvider = await dbContext.AIProviders.FindAsync(provider.Id);
        Assert.NotNull(updatedProvider);
        Assert.Equal(request.Name, updatedProvider.Name);
        Assert.Equal(request.ProviderType, updatedProvider.ProviderType);
        Assert.Equal(request.BaseUrl, updatedProvider.BaseUrl);
        Assert.Equal(request.IsActive, updatedProvider.IsActive);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenProviderDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = TestDataGenerator.CreateUpdateAIProviderRequest();

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{nonExistentId}", request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        AssertErrorResponse(apiResponse!, "AI_PROVIDER_NOT_FOUND");
    }

    [Fact]
    public async Task Delete_ShouldDeleteProvider_WhenProviderExists()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var provider = TestDataGenerator.CreateTestAIProvider("To Be Deleted");
        await dbContext.AIProviders.AddAsync(provider);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{provider.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // 验证提供商已被删除
        var deletedProvider = await dbContext.AIProviders.FindAsync(provider.Id);
        Assert.Null(deletedProvider);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenProviderDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{nonExistentId}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        AssertErrorResponse(apiResponse!, "AI_PROVIDER_NOT_FOUND");
    }
}