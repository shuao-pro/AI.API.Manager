using AI.API.Manager.API.DTOs;
using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.API.Manager.API.Controllers;

/// <summary>
/// 系统统计API控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IRequestLogService _requestLogService;
    private readonly ITenantService _tenantService;
    private readonly IAIProviderService _aiProviderService;
    private readonly IApiKeyService _apiKeyService;
    private readonly ILogger<StatisticsController> _logger;

    /// <summary>
    /// 初始化统计控制器
    /// </summary>
    public StatisticsController(
        IRequestLogService requestLogService,
        ITenantService tenantService,
        IAIProviderService aiProviderService,
        IApiKeyService apiKeyService,
        ILogger<StatisticsController> logger)
    {
        _requestLogService = requestLogService ?? throw new ArgumentNullException(nameof(requestLogService));
        _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
        _aiProviderService = aiProviderService ?? throw new ArgumentNullException(nameof(aiProviderService));
        _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 获取请求统计
    /// </summary>
    /// <param name="query">统计查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>请求统计信息</returns>
    [HttpPost("requests")]
    [ProducesResponseType(typeof(ApiResponse<RequestStatisticsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRequestStatistics(RequestStatisticsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse("查询条件验证失败", "VALIDATION_ERROR"));
            }

            var statistics = await _requestLogService.GetStatisticsAsync(query, cancellationToken);
            return Ok(ApiResponse<RequestStatisticsResponse>.SuccessResponse(statistics));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "获取请求统计时参数错误");
            return BadRequest(ApiResponse.ErrorResponse(ex.Message, "INVALID_PARAMETER"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取请求统计时发生错误");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取请求统计时发生服务器错误", "REQUEST_STATISTICS_ERROR"));
        }
    }

    /// <summary>
    /// 获取系统概览统计
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>系统概览统计信息</returns>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(ApiResponse<SystemOverviewResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSystemOverview(CancellationToken cancellationToken = default)
    {
        try
        {
            // 并行获取各项统计
            var tenantsTask = _tenantService.GetAllAsync(cancellationToken);
            var providersTask = _aiProviderService.GetAllAsync(cancellationToken);
            var apiKeysTask = _apiKeyService.GetAllAsync(cancellationToken);
            var activeProvidersTask = _aiProviderService.GetActiveProvidersAsync(cancellationToken);
            var activeTenantsTask = _tenantService.GetActiveTenantsAsync(cancellationToken);

            await Task.WhenAll(tenantsTask, providersTask, apiKeysTask, activeProvidersTask, activeTenantsTask);

            var tenants = await tenantsTask;
            var providers = await providersTask;
            var apiKeys = await apiKeysTask;
            var activeProviders = await activeProvidersTask;
            var activeTenants = await activeTenantsTask;

            var overview = new SystemOverviewResponse
            {
                TotalTenants = tenants.Count,
                ActiveTenants = activeTenants.Count,
                TotalProviders = providers.Count,
                ActiveProviders = activeProviders.Count,
                TotalApiKeys = apiKeys.Count,
                Timestamp = DateTimeOffset.UtcNow
            };

            return Ok(ApiResponse<SystemOverviewResponse>.SuccessResponse(overview));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取系统概览统计时发生错误");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取系统概览统计时发生服务器错误", "SYSTEM_OVERVIEW_ERROR"));
        }
    }

    /// <summary>
    /// 获取提供商使用统计
    /// </summary>
    /// <param name="startDate">开始时间</param>
    /// <param name="endDate">结束时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>提供商使用统计</returns>
    [HttpGet("providers/usage")]
    [ProducesResponseType(typeof(ApiResponse<ProviderUsageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProviderUsageStatistics(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 验证日期范围
            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
            {
                return BadRequest(ApiResponse.ErrorResponse("开始时间不能晚于结束时间", "INVALID_DATE_RANGE"));
            }

            // 这里可以调用专门的服务获取提供商使用统计
            // 目前返回一个简单响应，实际实现需要查询请求日志
            var usage = new ProviderUsageResponse
            {
                StartDate = startDate,
                EndDate = endDate,
                ProviderUsage = new Dictionary<Guid, ProviderUsageInfo>(),
                Timestamp = DateTimeOffset.UtcNow
            };

            // 暂时返回空数据，后续可以在此处实现实际逻辑
            return Ok(ApiResponse<ProviderUsageResponse>.SuccessResponse(usage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取提供商使用统计时发生错误");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取提供商使用统计时发生服务器错误", "PROVIDER_USAGE_STATISTICS_ERROR"));
        }
    }
}

/// <summary>
/// 系统概览统计响应
/// </summary>
public sealed record SystemOverviewResponse
{
    /// <summary>
    /// 总租户数
    /// </summary>
    public required int TotalTenants { get; init; }

    /// <summary>
    /// 活跃租户数
    /// </summary>
    public required int ActiveTenants { get; init; }

    /// <summary>
    /// 总提供商数
    /// </summary>
    public required int TotalProviders { get; init; }

    /// <summary>
    /// 活跃提供商数
    /// </summary>
    public required int ActiveProviders { get; init; }

    /// <summary>
    /// 总API密钥数
    /// </summary>
    public required int TotalApiKeys { get; init; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public required DateTimeOffset Timestamp { get; init; }
}

/// <summary>
/// 提供商使用统计响应
/// </summary>
public sealed record ProviderUsageResponse
{
    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// 提供商使用情况
    /// </summary>
    public required Dictionary<Guid, ProviderUsageInfo> ProviderUsage { get; init; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public required DateTimeOffset Timestamp { get; init; }
}

/// <summary>
/// 提供商使用信息
/// </summary>
public sealed record ProviderUsageInfo
{
    /// <summary>
    /// 提供商名称
    /// </summary>
    public required string ProviderName { get; init; }

    /// <summary>
    /// 总请求数
    /// </summary>
    public required int TotalRequests { get; init; }

    /// <summary>
    /// 成功请求数
    /// </summary>
    public required int SuccessfulRequests { get; init; }

    /// <summary>
    /// 失败请求数
    /// </summary>
    public required int FailedRequests { get; init; }

    /// <summary>
    /// 平均响应时间（毫秒）
    /// </summary>
    public required double AverageResponseTimeMs { get; init; }

    /// <summary>
    /// 总消耗令牌数（如果可用）
    /// </summary>
    public long? TotalTokensUsed { get; init; }
}