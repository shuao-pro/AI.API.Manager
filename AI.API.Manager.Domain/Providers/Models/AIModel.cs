namespace AI.API.Manager.Domain.Providers.Models;

/// <summary>
/// AI模型
/// </summary>
public sealed record AIModel
{
    /// <summary>
    /// 模型ID
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// 模型名称
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// 模型类型
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// 最大令牌数
    /// </summary>
    public int? MaxTokens { get; init; }

    /// <summary>
    /// 是否支持聊天
    /// </summary>
    public bool SupportsChat { get; init; }

    /// <summary>
    /// 是否支持嵌入
    /// </summary>
    public bool SupportsEmbedding { get; init; }

    /// <summary>
    /// 提供商元数据
    /// </summary>
    public Dictionary<string, object>? ProviderMetadata { get; init; }
}