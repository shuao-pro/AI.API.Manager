namespace AI.API.Manager.Domain.Providers;

/// <summary>
/// 提供商配置
/// </summary>
public sealed record ProviderConfiguration
{
    /// <summary>
    /// 提供商类型
    /// </summary>
    public required Enums.AIProviderType ProviderType { get; init; }

    /// <summary>
    /// 基础URL
    /// </summary>
    public required string BaseUrl { get; init; }

    /// <summary>
    /// API密钥
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// 超时秒数
    /// </summary>
    public int TimeoutSeconds { get; init; } = 30;

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetries { get; init; } = 3;

    /// <summary>
    /// 自定义头信息
    /// </summary>
    public Dictionary<string, string>? CustomHeaders { get; init; }

    /// <summary>
    /// 自定义参数
    /// </summary>
    public Dictionary<string, object>? CustomParameters { get; init; }
}