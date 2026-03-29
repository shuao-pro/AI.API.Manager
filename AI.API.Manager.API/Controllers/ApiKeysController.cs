using AI.API.Manager.API.DTOs;
using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.API.Manager.API.Controllers;

/// <summary>
/// API密钥管理API控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ApiKeysController : ControllerBase
{
    private readonly IApiKeyService _apiKeyService;
    private readonly ILogger<ApiKeysController> _logger;

    /// <summary>
    /// 初始化API密钥控制器
    /// </summary>
    public ApiKeysController(IApiKeyService apiKeyService, ILogger<ApiKeysController> logger)
    {
        _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 获取所有API密钥
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>API密钥列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ApiKeyResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            var apiKeys = await _apiKeyService.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<ApiKeyResponse>>.SuccessResponse(apiKeys));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有API密钥时发生错误");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取API密钥列表时发生服务器错误", "API_KEY_LIST_ERROR"));
        }
    }

    /// <summary>
    /// 根据ID获取API密钥
    /// </summary>
    /// <param name="id">API密钥ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>API密钥信息</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ApiKeyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiKey = await _apiKeyService.GetByIdAsync(id, cancellationToken);

            if (apiKey == null)
            {
                return NotFound(ApiResponse.ErrorResponse($"未找到ID为 {id} 的API密钥", "API_KEY_NOT_FOUND"));
            }

            return Ok(ApiResponse<ApiKeyResponse>.SuccessResponse(apiKey));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取API密钥 {ApiKeyId} 时发生错误", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取API密钥信息时发生服务器错误", "API_KEY_GET_ERROR"));
        }
    }

    /// <summary>
    /// 根据租户ID获取API密钥
    /// </summary>
    /// <param name="tenantId">租户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>租户的API密钥列表</returns>
    [HttpGet("tenant/{tenantId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ApiKeyResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByTenantId(Guid tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiKeys = await _apiKeyService.GetByTenantIdAsync(tenantId, cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<ApiKeyResponse>>.SuccessResponse(apiKeys));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取租户 {TenantId} 的API密钥时发生错误", tenantId);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取租户API密钥时发生服务器错误", "API_KEY_BY_TENANT_ERROR"));
        }
    }

    /// <summary>
    /// 创建API密钥
    /// </summary>
    /// <param name="request">创建API密钥请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>创建的API密钥信息</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ApiKeyResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(CreateApiKeyRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse("请求数据验证失败", "VALIDATION_ERROR"));
            }

            var createdApiKey = await _apiKeyService.CreateAsync(request, cancellationToken);

            // 注意：出于安全考虑，返回的响应中不应包含实际的密钥值
            // 服务层应该已经处理了这一点
            return CreatedAtAction(
                nameof(GetById),
                new { id = createdApiKey.Id },
                ApiResponse<ApiKeyResponse>.SuccessResponse(createdApiKey));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "创建API密钥时参数错误");
            return BadRequest(ApiResponse.ErrorResponse(ex.Message, "INVALID_PARAMETER"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建API密钥时发生错误");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("创建API密钥时发生服务器错误", "API_KEY_CREATE_ERROR"));
        }
    }

    /// <summary>
    /// 更新API密钥
    /// </summary>
    /// <param name="id">API密钥ID</param>
    /// <param name="request">更新API密钥请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新后的API密钥信息</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ApiKeyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, UpdateApiKeyRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse("请求数据验证失败", "VALIDATION_ERROR"));
            }

            var updatedApiKey = await _apiKeyService.UpdateAsync(id, request, cancellationToken);

            return Ok(ApiResponse<ApiKeyResponse>.SuccessResponse(updatedApiKey));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "更新API密钥 {ApiKeyId} 时参数错误", id);
            return BadRequest(ApiResponse.ErrorResponse(ex.Message, "INVALID_PARAMETER"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "更新API密钥 {ApiKeyId} 时未找到API密钥", id);
            return NotFound(ApiResponse.ErrorResponse(ex.Message, "API_KEY_NOT_FOUND"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新API密钥 {ApiKeyId} 时发生错误", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("更新API密钥时发生服务器错误", "API_KEY_UPDATE_ERROR"));
        }
    }

    /// <summary>
    /// 删除API密钥
    /// </summary>
    /// <param name="id">API密钥ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _apiKeyService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "删除API密钥 {ApiKeyId} 时未找到API密钥", id);
            return NotFound(ApiResponse.ErrorResponse(ex.Message, "API_KEY_NOT_FOUND"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除API密钥 {ApiKeyId} 时发生错误", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("删除API密钥时发生服务器错误", "API_KEY_DELETE_ERROR"));
        }
    }

    /// <summary>
    /// 验证API密钥
    /// </summary>
    /// <param name="apiKey">API密钥值</param>
    /// <param name="tenantId">租户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>验证结果</returns>
    [HttpGet("validate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ValidateApiKey(
        [FromQuery] string apiKey,
        [FromQuery] Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return BadRequest(ApiResponse.ErrorResponse("API密钥不能为空", "API_KEY_EMPTY"));
            }

            if (tenantId == Guid.Empty)
            {
                return BadRequest(ApiResponse.ErrorResponse("租户ID无效", "INVALID_TENANT_ID"));
            }

            var isValid = await _apiKeyService.ValidateApiKeyAsync(apiKey, tenantId, cancellationToken);
            return Ok(ApiResponse<bool>.SuccessResponse(isValid));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证API密钥时发生错误");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("验证API密钥时发生服务器错误", "API_KEY_VALIDATE_ERROR"));
        }
    }
}