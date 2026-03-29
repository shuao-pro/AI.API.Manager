using AI.API.Manager.API.DTOs;
using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.API.Manager.API.Controllers;

/// <summary>
/// 租户管理API控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly ILogger<TenantsController> _logger;

    /// <summary>
    /// 初始化租户控制器
    /// </summary>
    public TenantsController(ITenantService tenantService, ILogger<TenantsController> logger)
    {
        _tenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 获取所有租户
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>租户列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TenantResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            var tenants = await _tenantService.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<TenantResponse>>.SuccessResponse(tenants));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有租户时发生错误");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取租户列表时发生服务器错误", "TENANT_LIST_ERROR"));
        }
    }

    /// <summary>
    /// 根据ID获取租户
    /// </summary>
    /// <param name="id">租户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>租户信息</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TenantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenant = await _tenantService.GetByIdAsync(id, cancellationToken);

            if (tenant == null)
            {
                return NotFound(ApiResponse.ErrorResponse($"未找到ID为 {id} 的租户", "TENANT_NOT_FOUND"));
            }

            return Ok(ApiResponse<TenantResponse>.SuccessResponse(tenant));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取租户 {TenantId} 时发生错误", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取租户信息时发生服务器错误", "TENANT_GET_ERROR"));
        }
    }

    /// <summary>
    /// 获取活跃租户列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>活跃租户列表</returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TenantResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActiveTenants(CancellationToken cancellationToken = default)
    {
        try
        {
            var activeTenants = await _tenantService.GetActiveTenantsAsync(cancellationToken);
            return Ok(ApiResponse<IReadOnlyList<TenantResponse>>.SuccessResponse(activeTenants));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取活跃租户时发生错误");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("获取活跃租户列表时发生服务器错误", "ACTIVE_TENANTS_ERROR"));
        }
    }

    /// <summary>
    /// 创建租户
    /// </summary>
    /// <param name="request">创建租户请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>创建的租户信息</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TenantResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(CreateTenantRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse("请求数据验证失败", "VALIDATION_ERROR"));
            }

            var createdTenant = await _tenantService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdTenant.Id },
                ApiResponse<TenantResponse>.SuccessResponse(createdTenant));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "创建租户时参数错误");
            return BadRequest(ApiResponse.ErrorResponse(ex.Message, "INVALID_PARAMETER"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建租户时发生错误");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("创建租户时发生服务器错误", "TENANT_CREATE_ERROR"));
        }
    }

    /// <summary>
    /// 更新租户
    /// </summary>
    /// <param name="id">租户ID</param>
    /// <param name="request">更新租户请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新后的租户信息</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TenantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, UpdateTenantRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse("请求数据验证失败", "VALIDATION_ERROR"));
            }

            var updatedTenant = await _tenantService.UpdateAsync(id, request, cancellationToken);

            return Ok(ApiResponse<TenantResponse>.SuccessResponse(updatedTenant));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "更新租户 {TenantId} 时参数错误", id);
            return BadRequest(ApiResponse.ErrorResponse(ex.Message, "INVALID_PARAMETER"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "更新租户 {TenantId} 时未找到租户", id);
            return NotFound(ApiResponse.ErrorResponse(ex.Message, "TENANT_NOT_FOUND"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新租户 {TenantId} 时发生错误", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("更新租户时发生服务器错误", "TENANT_UPDATE_ERROR"));
        }
    }

    /// <summary>
    /// 删除租户
    /// </summary>
    /// <param name="id">租户ID</param>
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
            await _tenantService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "删除租户 {TenantId} 时未找到租户", id);
            return NotFound(ApiResponse.ErrorResponse(ex.Message, "TENANT_NOT_FOUND"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除租户 {TenantId} 时发生错误", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse.ErrorResponse("删除租户时发生服务器错误", "TENANT_DELETE_ERROR"));
        }
    }
}