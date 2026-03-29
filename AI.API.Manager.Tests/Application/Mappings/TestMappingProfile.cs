using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Domain.Entities;
using AutoMapper;

namespace AI.API.Manager.Tests.Application.Mappings;

public class TestMappingProfile : Profile
{
    public TestMappingProfile()
    {
        // 租户映射
        CreateMap<Tenant, TenantResponse>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // AI提供商映射
        CreateMap<AIProvider, AIProviderResponse>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
    }
}