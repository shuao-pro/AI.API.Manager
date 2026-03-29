using AI.API.Manager.API.DTOs;
using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Application.Services;
using AI.API.Manager.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AI.API.Manager.API.Controllers;

/// <summary>
/// AI提供商管理API控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AIProvidersController : ControllerBase
{
    private readonly IAIProviderService _aiProviderService;
    private readonly ILogger<AIProvidersController> _logger;

    /// <summary>
    /// 初始化AI提供商控制器
    /// </summary>
    public AIProvidersController(IAIProviderService aiProviderService, ILogger<AIProvidersController> logger)
    {
        _aiProviderService = aiProviderService ?? throw new ArgumentNullException(nameof(aiProviderService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 获取所有AI提供商
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>AI提供商列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AIProviderResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            var providers = await _aiProviderService.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<AIProviderResponse>>.SuccessResponse(providers));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有AI提供商时发生错误");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取AI提供商列表时发生服务器错误", "PROVIDER_LIST_ERROR"));
        }
    }

    /// <summary>
    /// 根据ID获取AI提供商
    /// </summary>
    /// <param name="id">提供商ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>AI提供商信息</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AIProviderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var provider = await _aiProviderService.GetByIdAsync(id, cancellationToken);

            if (provider == null)
            {
                return NotFound(ApiResponse.ErrorResponse($"未找到ID为 {id} 的AI提供商", "PROVIDER_NOT_FOUND"));
            }

            return Ok(ApiResponse<AIProviderResponse>.SuccessResponse(provider));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取AI提供商 {ProviderId} 时发生错误", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取AI提供商信息时发生服务器错误", "PROVIDER_GET_ERROR"));
        }
    }

    /// <summary>
    /// 获取活跃的AI提供商列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>活跃的AI提供商列表</returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AIProviderResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActiveProviders(CancellationToken cancellationToken = default)
    {
        try
        {
            var activeProviders = await _aiProviderService.GetActiveProvidersAsync(cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<AIProviderResponse>>.SuccessResponse(activeProviders));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取活跃AI提供商时发生错误");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取活跃AI提供商列表时发生服务器错误", "ACTIVE_PROVIDERS_ERROR"));
        }
    }

    /// <summary>
    /// 根据类型获取AI提供商
    /// </summary>
    /// <param name="providerType">提供商类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>指定类型的AI提供商列表</returns>
    [HttpGet("type/{providerType}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AIProviderResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByType(AIProviderType providerType, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Enum.IsDefined(typeof(AIProviderType), providerType))
            {
                return BadRequest(ApiResponse.ErrorResponse($"无效的提供商类型: {providerType}", "INVALID_PROVIDER_TYPE"));
            }

            var providers = await _aiProviderService.GetByTypeAsync(providerType, cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<AIProviderResponse>>.SuccessResponse(providers));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根据类型 {ProviderType} 获取AI提供商时发生错误", providerType);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("根据类型获取AI提供商时发生服务器错误", "PROVIDERS_BY_TYPE_ERROR"));
        }
    }

    /// <summary>
    /// 创建AI提供商
    /// </summary>
    /// <param name="request">创建AI提供商请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>创建的AI提供商信息</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AIProviderResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(CreateAIProviderRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse("请求数据验证失败", "VALIDATION_ERROR"));
            }

            var createdProvider = await _aiProviderService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdProvider.Id },
                ApiResponse<AIProviderResponse>.SuccessResponse(createdProvider));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "创建AI提供商时参数错误");
            return BadRequest(ApiResponse.ErrorResponse(ex.Message, "INVALID_PARAMETER"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建AI提供商时发生错误");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("创建AI提供商时发生服务器错误", "PROVIDER_CREATE_ERROR"));
        }
    }

    /// <summary>
    /// 更新AI提供商
    /// </summary>
    /// <param name="id">提供商ID</param>
    /// <param name="request">更新AI提供商请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新后的AI提供商信息</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AIProviderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, UpdateAIProviderRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse("请求数据验证失败", "VALIDATION_ERROR"));
            }

            var updatedProvider = await _aiProviderService.UpdateAsync(id, request, cancellationToken);

            return Ok(ApiResponse<AIProviderResponse>.SuccessResponse(updatedProvider));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "更新AI提供商 {ProviderId} 时参数错误", id);
            return BadRequest(ApiResponse.ErrorResponse(ex.Message, "INVALID_PARAMETER"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "更新AI提供商 {ProviderId} 时未找到提供商", id);
            return NotFound(ApiResponse.ErrorResponse(ex.Message, "PROVIDER_NOT_FOUND"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新AI提供商 {ProviderId} 时发生错误", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("更新AI提供商时发生服务器错误", "PROVIDER_UPDATE_ERROR"));
        }
    }

    /// <summary>
    /// 删除AI提供商
    /// </summary>
    /// <param name="id">提供商ID</param>
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
            await _aiProviderService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "删除AI提供商 {ProviderId} 时未找到提供商", id);
            return NotFound(ApiResponse.ErrorResponse(ex.Message, "PROVIDER_NOT_FOUND"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除AI提供商 {ProviderId} 时发生错误", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("删除AI提供商时发生服务器错误", "PROVIDER_DELETE_ERROR"));
        }
    }
}