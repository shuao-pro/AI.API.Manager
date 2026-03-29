using AI.API.Manager.API.DTOs;
using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.API.Manager.API.Controllers;

/// <summary>
/// 请求日志管理API控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RequestLogsController : ControllerBase
{
    private readonly IRequestLogService _requestLogService;
    private readonly ILogger<RequestLogsController> _logger;

    /// <summary>
    /// 初始化请求日志控制器
    /// </summary>
    public RequestLogsController(IRequestLogService requestLogService, ILogger<RequestLogsController> logger)
    {
        _requestLogService = requestLogService ?? throw new ArgumentNullException(nameof(requestLogService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 获取所有请求日志
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>请求日志列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RequestLogResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            var requestLogs = await _requestLogService.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<RequestLogResponse>>.SuccessResponse(requestLogs));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有请求日志时发生错误");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取请求日志列表时发生服务器错误", "REQUEST_LOG_LIST_ERROR"));
        }
    }

    /// <summary>
    /// 根据ID获取请求日志
    /// </summary>
    /// <param name="id">请求日志ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>请求日志信息</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RequestLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var requestLog = await _requestLogService.GetByIdAsync(id, cancellationToken);

            if (requestLog == null)
            {
                return NotFound(ApiResponse.ErrorResponse($"未找到ID为 {id} 的请求日志", "REQUEST_LOG_NOT_FOUND"));
            }

            return Ok(ApiResponse<RequestLogResponse>.SuccessResponse(requestLog));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取请求日志 {RequestLogId} 时发生错误", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取请求日志信息时发生服务器错误", "REQUEST_LOG_GET_ERROR"));
        }
    }

    /// <summary>
    /// 根据租户ID获取请求日志
    /// </summary>
    /// <param name="tenantId">租户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>租户的请求日志列表</returns>
    [HttpGet("tenant/{tenantId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RequestLogResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByTenantId(Guid tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var requestLogs = await _requestLogService.GetByTenantIdAsync(tenantId, cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<RequestLogResponse>>.SuccessResponse(requestLogs));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取租户 {TenantId} 的请求日志时发生错误", tenantId);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取租户请求日志时发生服务器错误", "REQUEST_LOG_BY_TENANT_ERROR"));
        }
    }

    /// <summary>
    /// 根据API密钥ID获取请求日志
    /// </summary>
    /// <param name="apiKeyId">API密钥ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>API密钥的请求日志列表</returns>
    [HttpGet("api-key/{apiKeyId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RequestLogResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByApiKeyId(Guid apiKeyId, CancellationToken cancellationToken = default)
    {
        try
        {
            var requestLogs = await _requestLogService.GetByApiKeyIdAsync(apiKeyId, cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<RequestLogResponse>>.SuccessResponse(requestLogs));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取API密钥 {ApiKeyId} 的请求日志时发生错误", apiKeyId);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取API密钥请求日志时发生服务器错误", "REQUEST_LOG_BY_API_KEY_ERROR"));
        }
    }

    /// <summary>
    /// 创建请求日志
    /// </summary>
    /// <param name="request">创建请求日志请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>创建的请求日志信息</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RequestLogResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(CreateRequestLogRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse("请求数据验证失败", "VALIDATION_ERROR"));
            }

            var createdRequestLog = await _requestLogService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdRequestLog.Id },
                ApiResponse<RequestLogResponse>.SuccessResponse(createdRequestLog));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "创建请求日志时参数错误");
            return BadRequest(ApiResponse.ErrorResponse(ex.Message, "INVALID_PARAMETER"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建请求日志时发生错误");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("创建请求日志时发生服务器错误", "REQUEST_LOG_CREATE_ERROR"));
        }
    }

    /// <summary>
    /// 更新请求日志状态
    /// </summary>
    /// <param name="id">请求日志ID</param>
    /// <param name="request">更新请求日志状态请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新后的请求日志信息</returns>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<RequestLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateRequestLogStatusRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse("请求数据验证失败", "VALIDATION_ERROR"));
            }

            var updatedRequestLog = await _requestLogService.UpdateStatusAsync(id, request, cancellationToken);

            return Ok(ApiResponse<RequestLogResponse>.SuccessResponse(updatedRequestLog));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "更新请求日志 {RequestLogId} 状态时参数错误", id);
            return BadRequest(ApiResponse.ErrorResponse(ex.Message, "INVALID_PARAMETER"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "更新请求日志 {RequestLogId} 状态时未找到请求日志", id);
            return NotFound(ApiResponse.ErrorResponse(ex.Message, "REQUEST_LOG_NOT_FOUND"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新请求日志 {RequestLogId} 状态时发生错误", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("更新请求日志状态时发生服务器错误", "REQUEST_LOG_UPDATE_STATUS_ERROR"));
        }
    }

    /// <summary>
    /// 获取请求统计
    /// </summary>
    /// <param name="query">统计查询条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>请求统计信息</returns>
    [HttpPost("statistics")]
    [ProducesResponseType(typeof(ApiResponse<RequestStatisticsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStatistics(RequestStatisticsQuery query, CancellationToken cancellationToken = default)
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
}