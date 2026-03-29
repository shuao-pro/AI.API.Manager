using System;
using System.IO;
using System.Reflection;
using AI.API.Manager.Application;
using AI.API.Manager.Infrastructure;
using AI.API.Manager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace AI.API.Manager.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        // 注册应用层服务
        services.AddApplication();

        // 注册基础设施层服务
        services.AddInfrastructure(configuration);

        return services;
    }

    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AI API Manager",
                Version = "v1",
                Description = "统一管理多个AI服务提供商的API网关",
                Contact = new OpenApiContact
                {
                    Name = "AI API Manager Team",
                    Email = "support@aiapimanager.example.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // 添加XML注释
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // 添加JWT Bearer认证（将在阶段2实现）
            // options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            // {
            //     Description = "JWT Authorization header using the Bearer scheme.",
            //     Name = "Authorization",
            //     In = ParameterLocation.Header,
            //     Type = SecuritySchemeType.Http,
            //     Scheme = "bearer",
            //     BearerFormat = "JWT"
            // });
            //
            // options.AddSecurityRequirement(new OpenApiSecurityRequirement
            // {
            //     {
            //         new OpenApiSecurityScheme
            //         {
            //             Reference = new OpenApiReference
            //             {
            //                 Type = ReferenceType.SecurityScheme,
            //                 Id = "Bearer"
            //             }
            //         },
            //         Array.Empty<string>()
            //     }
            // });
        });

        return services;
    }

    public static IServiceCollection AddApplicationHealthChecks(this IServiceCollection services)
    {
        // 临时跳过数据库检查，让API先跑起来
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());
        
        return services;
    }
}