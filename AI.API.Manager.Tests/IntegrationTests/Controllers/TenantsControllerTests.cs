using AI.API.Manager.API.DTOs;
using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace AI.API.Manager.Tests.IntegrationTests.Controllers;

public class TenantsControllerTests : BaseIntegrationTest
{
    private const string BaseUrl = "/api/tenants";

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoTenantsExist()
    {
        // Act
        var response = await Client.GetAsync(BaseUrl);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<TenantResponse>>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Empty(apiResponse.Data);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllTenants()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var tenants = new List<Tenant>
        {
            TestDataGenerator.CreateTestTenant("Tenant 1"),
            TestDataGenerator.CreateTestTenant("Tenant 2")
        };

        await dbContext.Tenants.AddRangeAsync(tenants);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync(BaseUrl);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<TenantResponse>>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(2, apiResponse.Data.Count);
        Assert.Contains(apiResponse.Data, t => t.Name == "Tenant 1");
        Assert.Contains(apiResponse.Data, t => t.Name == "Tenant 2");
    }

    [Fact]
    public async Task GetById_ShouldReturnTenant_WhenTenantExists()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var tenant = TestDataGenerator.CreateTestTenant("Test Tenant");
        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{tenant.Id}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TenantResponse>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(tenant.Id, apiResponse.Data.Id);
        Assert.Equal(tenant.Name, apiResponse.Data.Name);
        Assert.Equal(tenant.IsActive, apiResponse.Data.IsActive);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenTenantDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{nonExistentId}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        AssertErrorResponse(apiResponse!, "TENANT_NOT_FOUND");
    }

    [Fact]
    public async Task Create_ShouldCreateTenant_WhenRequestIsValid()
    {
        // Arrange
        var request = TestDataGenerator.CreateCreateTenantRequest("New Tenant");

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TenantResponse>>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(request.Name, apiResponse.Data.Name);
        Assert.Equal(request.Description, apiResponse.Data.Description);
        Assert.True(apiResponse.Data.IsActive); // 默认应该是激活状态

        // 验证租户确实被创建了
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();
        var createdTenant = await dbContext.Tenants.FindAsync(apiResponse.Data.Id);
        Assert.NotNull(createdTenant);
        Assert.Equal(request.Name, createdTenant.Name);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CreateTenantRequest
        {
            Name = "",
            Description = "Test Description"
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
    public async Task Create_ShouldReturnBadRequest_WhenNameAlreadyExists()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var existingTenant = TestDataGenerator.CreateTestTenant("Existing Tenant");
        await dbContext.Tenants.AddAsync(existingTenant);
        await dbContext.SaveChangesAsync();

        var request = TestDataGenerator.CreateCreateTenantRequest("Existing Tenant");

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        AssertErrorResponse(apiResponse!, "INVALID_PARAMETER");
    }

    [Fact]
    public async Task Update_ShouldUpdateTenant_WhenTenantExists()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var tenant = TestDataGenerator.CreateTestTenant("Old Name", "Old Description", isActive: true);
        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.SaveChangesAsync();

        var request = TestDataGenerator.CreateUpdateTenantRequest("Updated Name", "Updated Description", isActive: false);

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{tenant.Id}", request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TenantResponse>>(
            GetJsonSerializerOptions());

        // Assert
        response.EnsureSuccessStatusCode();
        AssertSuccessResponse(apiResponse!);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(request.Name, apiResponse.Data.Name);
        Assert.Equal(request.Description, apiResponse.Data.Description);
        Assert.Equal(request.IsActive, apiResponse.Data.IsActive);

        // 验证数据库中的更新
        var updatedTenant = await dbContext.Tenants.FindAsync(tenant.Id);
        Assert.NotNull(updatedTenant);
        Assert.Equal(request.Name, updatedTenant.Name);
        Assert.Equal(request.Description, updatedTenant.Description);
        Assert.Equal(request.IsActive, updatedTenant.IsActive);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenTenantDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = TestDataGenerator.CreateUpdateTenantRequest();

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{nonExistentId}", request, GetJsonSerializerOptions());
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        AssertErrorResponse(apiResponse!, "TENANT_NOT_FOUND");
    }

    [Fact]
    public async Task Delete_ShouldDeleteTenant_WhenTenantExists()
    {
        // Arrange
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();

        var tenant = TestDataGenerator.CreateTestTenant("To Be Deleted");
        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{tenant.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // 验证租户已被删除
        var deletedTenant = await dbContext.Tenants.FindAsync(tenant.Id);
        Assert.Null(deletedTenant);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenTenantDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{nonExistentId}");
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
            GetJsonSerializerOptions());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        AssertErrorResponse(apiResponse!, "TENANT_NOT_FOUND");
    }
}