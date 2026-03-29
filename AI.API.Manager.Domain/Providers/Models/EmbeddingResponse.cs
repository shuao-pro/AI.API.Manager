namespace AI.API.Manager.Domain.Providers.Models;

/// <summary>
/// 嵌入响应
/// </summary>
public sealed record EmbeddingResponse
{
    /// <summary>
    /// 模型名称
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// 嵌入数据列表
    /// </summary>
    public required IReadOnlyList<EmbeddingData> Data { get; init; }

    /// <summary>
    /// 使用情况
    /// </summary>
    public required Usage Usage { get; init; }
}

/// <summary>
/// 嵌入数据
/// </summary>
public sealed record EmbeddingData
{
    /// <summary>
    /// 索引
    /// </summary>
    public required int Index { get; init; }

    /// <summary>
    /// 嵌入向量
    /// </summary>
    public required IReadOnlyList<float> Embedding { get; init; }
}