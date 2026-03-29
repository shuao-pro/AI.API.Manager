using AI.API.Manager.API.DTOs;
using AI.API.Manager.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;
using System.Text.Json;

namespace AI.API.Manager.Tests.IntegrationTests;

/// <summary>
/// 基础集成测试类，提供测试服务器和HTTP客户端
/// </summary>
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    protected HttpClient Client { get; private set; } = null!;
    protected IServiceProvider ServiceProvider { get; private set; } = null!;

    protected BaseIntegrationTest()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // 配置测试环境
                builder.UseSetting("Environment", "Test");

                // 替换真实数据库为内存数据库
                builder.ConfigureServices(services =>
                {
                    // 移除现有的DbContext配置
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // 添加内存数据库
                    services.AddDbContext<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                    });

                    // 可以在此处添加其他测试服务替换
                });
            });
    }

    public async Task InitializeAsync()
    {
        Client = _factory.CreateClient();
        ServiceProvider = _factory.Services;

        // 确保数据库已创建
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        // 清理数据库
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AI.API.Manager.Infrastructure.Data.ApplicationDbContext>();
        await dbContext.Database.EnsureDeletedAsync();

        Client.Dispose();
        _factory.Dispose();
    }

    /// <summary>
    /// 验证API响应是否成功
    /// </summary>
    protected static void AssertSuccessResponse<T>(ApiResponse<T> response)
    {
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Null(response.Error);
        Assert.NotNull(response.Data);
    }

    /// <summary>
    /// 验证API响应是否包含错误
    /// </summary>
    protected static void AssertErrorResponse(ApiResponse response, string expectedErrorCode)
    {
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.NotNull(response.Error);
        Assert.Equal(expectedErrorCode, response.ErrorCode);
    }

    /// <summary>
    /// 验证API响应是否包含错误（泛型版本）
    /// </summary>
    protected static void AssertErrorResponse<T>(ApiResponse<T> response, string expectedErrorCode)
    {
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.NotNull(response.Error);
        Assert.Equal(expectedErrorCode, response.ErrorCode);
        Assert.Null(response.Data);
    }

    /// <summary>
    /// 获取JSON序列化选项（使用不区分大小写的属性名匹配）
    /// </summary>
    protected static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}