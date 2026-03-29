using AI.API.Manager.Domain.Providers.Models;

namespace AI.API.Manager.Domain.Providers;

/// <summary>
/// 统一AI提供商接口
/// </summary>
public interface IAIProvider
{
    /// <summary>
    /// 提供商类型
    /// </summary>
    Enums.AIProviderType ProviderType { get; }

    /// <summary>
    /// 提供商名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 创建聊天完成
    /// </summary>
    Task<ChatCompletionResponse> CreateChatCompletionAsync(ChatCompletionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建嵌入
    /// </summary>
    Task<EmbeddingResponse> CreateEmbeddingAsync(EmbeddingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取模型列表
    /// </summary>
    Task<IReadOnlyList<AIModel>> ListModelsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 测试连接
    /// </summary>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);
}