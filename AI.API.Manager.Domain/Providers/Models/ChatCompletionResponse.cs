namespace AI.API.Manager.Domain.Providers.Models;

/// <summary>
/// 聊天完成响应
/// </summary>
public sealed record ChatCompletionResponse
{
    /// <summary>
    /// 响应ID
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// 模型名称
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// 选择列表
    /// </summary>
    public required IReadOnlyList<ChatChoice> Choices { get; init; }

    /// <summary>
    /// 使用情况
    /// </summary>
    public required Usage Usage { get; init; }

    /// <summary>
    /// 提供商元数据
    /// </summary>
    public Dictionary<string, object>? ProviderMetadata { get; init; }
}

/// <summary>
/// 聊天选择
/// </summary>
public sealed record ChatChoice
{
    /// <summary>
    /// 索引
    /// </summary>
    public required int Index { get; init; }

    /// <summary>
    /// 消息
    /// </summary>
    public required ChatMessage Message { get; init; }

    /// <summary>
    /// 完成原因
    /// </summary>
    public string? FinishReason { get; init; }
}

/// <summary>
/// 使用情况
/// </summary>
public sealed record Usage
{
    /// <summary>
    /// 提示令牌数
    /// </summary>
    public required int PromptTokens { get; init; }

    /// <summary>
    /// 完成令牌数
    /// </summary>
    public required int CompletionTokens { get; init; }

    /// <summary>
    /// 总令牌数
    /// </summary>
    public required int TotalTokens { get; init; }
}