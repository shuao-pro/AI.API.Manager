using AI.API.Manager.Application.DTOs;

namespace AI.API.Manager.Application.Services;

/// <summary>
/// API密钥服务接口
/// </summary>
public interface IApiKeyService
{
    /// <summary>
    /// 根据ID获取API密钥
    /// </summary>
    Task<ApiKeyResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有API密钥
    /// </summary>
    Task<IReadOnlyList<ApiKeyResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据租户ID获取API密钥
    /// </summary>
    Task<IReadOnlyList<ApiKeyResponse>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据密钥值获取API密钥
    /// </summary>
    Task<ApiKeyResponse?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建API密钥
    /// </summary>
    Task<ApiKeyResponse> CreateAsync(CreateApiKeyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新API密钥
    /// </summary>
    Task<ApiKeyResponse> UpdateAsync(Guid id, UpdateApiKeyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除API密钥
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证API密钥
    /// </summary>
    Task<bool> ValidateApiKeyAsync(string apiKey, Guid tenantId, CancellationToken cancellationToken = default);
}