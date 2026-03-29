using AI.API.Manager.Application.DTOs;

namespace AI.API.Manager.Application.Services;

/// <summary>
/// 请求日志服务接口
/// </summary>
public interface IRequestLogService
{
    /// <summary>
    /// 根据ID获取请求日志
    /// </summary>
    Task<RequestLogResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有请求日志
    /// </summary>
    Task<IReadOnlyList<RequestLogResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据租户ID获取请求日志
    /// </summary>
    Task<IReadOnlyList<RequestLogResponse>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据API密钥ID获取请求日志
    /// </summary>
    Task<IReadOnlyList<RequestLogResponse>> GetByApiKeyIdAsync(Guid apiKeyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建请求日志
    /// </summary>
    Task<RequestLogResponse> CreateAsync(CreateRequestLogRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新请求日志状态
    /// </summary>
    Task<RequestLogResponse> UpdateStatusAsync(Guid id, UpdateRequestLogStatusRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取请求统计
    /// </summary>
    Task<RequestStatisticsResponse> GetStatisticsAsync(RequestStatisticsQuery query, CancellationToken cancellationToken = default);
}