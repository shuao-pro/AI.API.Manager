using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Domain.Entities;
using AutoMapper;

namespace AI.API.Manager.Application.Mappings;

/// <summary>
/// AutoMapper配置
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // 租户映射
        CreateMap<Tenant, TenantResponse>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // AI提供商映射
        CreateMap<AIProvider, AIProviderResponse>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // API密钥映射
        CreateMap<ApiKey, ApiKeyResponse>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // 请求日志映射
        CreateMap<RequestLog, RequestLogResponse>();

        // 创建请求到实体的映射（如果需要）
        // CreateMap<CreateTenantRequest, Tenant>()
        //     .ForMember(dest => dest.Id, opt => opt.Ignore())
        //     .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
        //     .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}