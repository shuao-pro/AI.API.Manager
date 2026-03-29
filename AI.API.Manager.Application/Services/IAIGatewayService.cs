namespace AI.API.Manager.Application.Services;

/// <summary>
/// AI网关服务接口
/// </summary>
public interface IAIGatewayService
{
    // 暂时注释掉，先让编译通过
    /*
    /// <summary>
    /// 处理聊天完成请求
    /// </summary>
    Task<ChatCompletionResponse> ProcessChatCompletionAsync(ChatCompletionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 处理嵌入请求
    /// </summary>
    Task<EmbeddingResponse> ProcessEmbeddingAsync(EmbeddingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取可用的AI模型列表
    /// </summary>
    Task<IReadOnlyList<AIModelResponse>> ListModelsAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证租户和API密钥
    /// </summary>
    Task<bool> ValidateAccessAsync(Guid tenantId, string apiKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 记录请求日志
    /// </summary>
    Task LogRequestAsync(LogRequestRequest request, CancellationToken cancellationToken = default);
    */
}