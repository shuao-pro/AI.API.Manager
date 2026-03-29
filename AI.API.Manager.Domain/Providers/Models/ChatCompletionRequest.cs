namespace AI.API.Manager.Domain.Providers.Models;

/// <summary>
/// 聊天完成请求
/// </summary>
public sealed record ChatCompletionRequest
{
    /// <summary>
    /// 模型名称
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// 消息列表
    /// </summary>
    public required IReadOnlyList<ChatMessage> Messages { get; init; }

    /// <summary>
    /// 最大令牌数
    /// </summary>
    public int? MaxTokens { get; init; }

    /// <summary>
    /// 温度
    /// </summary>
    public float? Temperature { get; init; }

    /// <summary>
    /// Top P
    /// </summary>
    public float? TopP { get; init; }

    /// <summary>
    /// 是否流式输出
    /// </summary>
    public bool Stream { get; init; }
}

/// <summary>
/// 聊天消息
/// </summary>
public sealed record ChatMessage
{
    /// <summary>
    /// 角色
    /// </summary>
    public required string Role { get; init; }

    /// <summary>
    /// 内容
    /// </summary>
    public required string Content { get; init; }
}