namespace AI.API.Manager.Domain.Providers.Models;

/// <summary>
/// 嵌入请求
/// </summary>
public sealed record EmbeddingRequest
{
    /// <summary>
    /// 模型名称
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// 输入文本列表
    /// </summary>
    public required IReadOnlyList<string> Input { get; init; }
}