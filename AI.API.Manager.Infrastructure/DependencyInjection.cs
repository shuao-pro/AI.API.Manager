using AI.API.Manager.Domain.Repositories;
using AI.API.Manager.Infrastructure.Data;
using AI.API.Manager.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.API.Manager.Infrastructure;

/// <summary>
/// 基础设施层依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加基础设施层服务
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        // 使用内存数据库（不需要安装 SQL Server，用于开发测试）
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("AIAPIManagerDb"));
        
        // 如果需要使用 SQL Server（原配置），可以注释掉上面，取消下面的注释
        // var connectionString = configuration.GetConnectionString("DefaultConnection");
        // if (string.IsNullOrEmpty(connectionString))
        // {
        //     throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        // }
        // services.AddDbContext<ApplicationDbContext>(options =>
        //     options.UseSqlServer(connectionString));

        // 注册仓储实现
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IAIProviderRepository, AIProviderRepository>();
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddScoped<IRequestLogRepository, RequestLogRepository>();

        return services;
    }
}