using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Domain.Enums;

namespace AI.API.Manager.Application.Services;

/// <summary>
/// AI提供商服务接口
/// </summary>
public interface IAIProviderService
{
    /// <summary>
    /// 根据ID获取AI提供商
    /// </summary>
    Task<AIProviderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有AI提供商
    /// </summary>
    Task<IReadOnlyList<AIProviderResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取活跃的AI提供商列表
    /// </summary>
    Task<IReadOnlyList<AIProviderResponse>> GetActiveProvidersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据类型获取AI提供商
    /// </summary>
    Task<IReadOnlyList<AIProviderResponse>> GetByTypeAsync(AIProviderType providerType, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建AI提供商
    /// </summary>
    Task<AIProviderResponse> CreateAsync(CreateAIProviderRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新AI提供商
    /// </summary>
    Task<AIProviderResponse> UpdateAsync(Guid id, UpdateAIProviderRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除AI提供商
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}