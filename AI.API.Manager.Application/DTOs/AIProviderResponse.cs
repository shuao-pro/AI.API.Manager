using AI.API.Manager.Domain.Enums;

namespace AI.API.Manager.Application.DTOs;

/// <summary>
/// AI提供商响应DTO
/// </summary>
public sealed record AIProviderResponse
{
    /// <summary>
    /// 提供商ID
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// 提供商名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 提供商类型
    /// </summary>
    public required AIProviderType ProviderType { get; init; }

    /// <summary>
    /// 基础URL
    /// </summary>
    public required string BaseUrl { get; init; }

    /// <summary>
    /// 是否活跃
    /// </summary>
    public required bool IsActive { get; init; }

    /// <summary>
    /// 最大并发请求数
    /// </summary>
    public required int MaxConcurrentRequests { get; init; }

    /// <summary>
    /// 请求超时秒数
    /// </summary>
    public required int RequestTimeoutSeconds { get; init; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}