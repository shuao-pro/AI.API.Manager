using AI.API.Manager.Domain.Enums;

namespace AI.API.Manager.Application.DTOs;

/// <summary>
/// 创建AI提供商请求DTO
/// </summary>
public sealed record CreateAIProviderRequest
{
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
    /// API密钥
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// 是否活跃
    /// </summary>
    public bool IsActive { get; init; } = true;

    /// <summary>
    /// 最大令牌数
    /// </summary>
    public int MaxTokens { get; init; } = 4096;

    /// <summary>
    /// 超时秒数
    /// </summary>
    public int TimeoutSeconds { get; init; } = 30;
}