using AI.API.Manager.Application.Mappings;
using AI.API.Manager.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AI.API.Manager.Application;

/// <summary>
/// 应用层依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加应用层服务
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // 注册应用服务
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IAIProviderService, AIProviderService>();
        services.AddScoped<IApiKeyService, ApiKeyService>();
        services.AddScoped<IRequestLogService, RequestLogService>();
        // IAIGatewayService 暂时不注册，因为实现可能在后续阶段

        // 注册AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        return services;
    }
}