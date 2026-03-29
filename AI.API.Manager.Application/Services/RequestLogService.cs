using AI.API.Manager.Application.DTOs;
using AI.API.Manager.Domain.Entities;
using AI.API.Manager.Domain.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AI.API.Manager.Application.Services;

/// <summary>
/// 请求日志服务实现
/// </summary>
public class RequestLogService : IRequestLogService
{
    private readonly IRequestLogRepository _requestLogRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RequestLogService> _logger;

    public RequestLogService(
        IRequestLogRepository requestLogRepository,
        IMapper mapper,
        ILogger<RequestLogService> logger)
    {
        _requestLogRepository = requestLogRepository ?? throw new ArgumentNullException(nameof(requestLogRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RequestLogResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting request log by ID: {RequestLogId}", id);

        var requestLog = await _requestLogRepository.GetByIdAsync(id, cancellationToken);
        if (requestLog == null)
        {
            _logger.LogDebug("Request log not found: {RequestLogId}", id);
            return null;
        }

        return _mapper.Map<RequestLogResponse>(requestLog);
    }

    public async Task<IReadOnlyList<RequestLogResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all request logs");

        var requestLogs = await _requestLogRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<RequestLogResponse>>(requestLogs);
    }

    public async Task<IReadOnlyList<RequestLogResponse>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting request logs for tenant: {TenantId}", tenantId);

        var requestLogs = await _requestLogRepository.GetByTenantIdAsync(tenantId, cancellationToken);
        return _mapper.Map<IReadOnlyList<RequestLogResponse>>(requestLogs);
    }

    public async Task<IReadOnlyList<RequestLogResponse>> GetByApiKeyIdAsync(Guid apiKeyId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting request logs for API key: {ApiKeyId}", apiKeyId);

        var requestLogs = await _requestLogRepository.GetByApiKeyIdAsync(apiKeyId, cancellationToken);
        return _mapper.Map<IReadOnlyList<RequestLogResponse>>(requestLogs);
    }

    public async Task<RequestLogResponse> CreateAsync(CreateRequestLogRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating request log for tenant: {TenantId}", request.TenantId);

        // 验证请求数据
        ValidateRequestLogRequest(request);

        // 使用工厂方法创建请求日志实体
        var requestLog = RequestLog.Create(
            tenantId: request.TenantId,
            providerId: request.ProviderId,
            apiKeyId: request.ApiKeyId,
            endpoint: request.Endpoint,
            method: request.Method,
            statusCode: request.StatusCode,
            durationMs: request.DurationMs,
            requestSizeBytes: request.RequestSizeBytes,
            responseSizeBytes: request.ResponseSizeBytes,
            userAgent: request.UserAgent,
            ipAddress: request.IpAddress);

        // 保存到数据库
        var createdRequestLog = await _requestLogRepository.AddAsync(requestLog, cancellationToken);

        _logger.LogInformation("Created request log: {RequestLogId} for tenant: {TenantId}", createdRequestLog.Id, createdRequestLog.TenantId);

        return _mapper.Map<RequestLogResponse>(createdRequestLog);
    }

    public async Task<RequestLogResponse> UpdateStatusAsync(Guid id, UpdateRequestLogStatusRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating request log status: {RequestLogId}", id);

        // 获取现有请求日志
        var existingRequestLog = await _requestLogRepository.GetByIdAsync(id, cancellationToken);
        if (existingRequestLog == null)
        {
            throw new InvalidOperationException($"Request log with ID '{id}' not found.");
        }

        // 注意：RequestLog实体是不可变的，没有更新方法
        // 实际项目中可能需要添加更新功能
        // 这里暂时抛出异常，因为RequestLog设计为不可变
        throw new NotSupportedException("RequestLog is immutable and cannot be updated. Create a new request log instead.");
    }

    public async Task<RequestStatisticsResponse> GetStatisticsAsync(RequestStatisticsQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting request statistics");

        // 实际项目中应该实现更复杂的统计查询
        // 这里使用简化的实现
        var allLogs = await _requestLogRepository.GetAllAsync(cancellationToken);

        // 应用过滤器
        var filteredLogs = allLogs.AsEnumerable();

        if (query.TenantId.HasValue)
        {
            filteredLogs = filteredLogs.Where(log => log.TenantId == query.TenantId.Value);
        }

        if (query.ProviderId.HasValue)
        {
            filteredLogs = filteredLogs.Where(log => log.ProviderId == query.ProviderId.Value);
        }

        if (query.ApiKeyId.HasValue)
        {
            filteredLogs = filteredLogs.Where(log => log.ApiKeyId == query.ApiKeyId.Value);
        }

        if (query.StartDate.HasValue)
        {
            filteredLogs = filteredLogs.Where(log => log.CreatedAt >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            filteredLogs = filteredLogs.Where(log => log.CreatedAt <= query.EndDate.Value);
        }

        if (query.OnlySuccessful.HasValue && query.OnlySuccessful.Value)
        {
            filteredLogs = filteredLogs.Where(log => log.IsSuccessful());
        }

        if (query.OnlyFailed.HasValue && query.OnlyFailed.Value)
        {
            filteredLogs = filteredLogs.Where(log => !log.IsSuccessful());
        }

        if (!string.IsNullOrEmpty(query.Endpoint))
        {
            filteredLogs = filteredLogs.Where(log => log.Endpoint.Contains(query.Endpoint, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(query.Method))
        {
            filteredLogs = filteredLogs.Where(log => log.Method.Equals(query.Method, StringComparison.OrdinalIgnoreCase));
        }

        if (query.StatusCode.HasValue)
        {
            filteredLogs = filteredLogs.Where(log => log.StatusCode == query.StatusCode.Value);
        }

        if (query.MinDurationMs.HasValue)
        {
            filteredLogs = filteredLogs.Where(log => log.DurationMs >= query.MinDurationMs.Value);
        }

        if (query.MaxDurationMs.HasValue)
        {
            filteredLogs = filteredLogs.Where(log => log.DurationMs <= query.MaxDurationMs.Value);
        }

        var logsList = filteredLogs.ToList();

        // 计算统计
        var totalRequests = logsList.Count;
        var successfulRequests = logsList.Count(log => log.IsSuccessful());
        var failedRequests = totalRequests - successfulRequests;

        var averageResponseTimeMs = logsList.Any() ? logsList.Average(log => log.DurationMs) : 0;
        var minResponseTimeMs = logsList.Any() ? logsList.Min(log => log.DurationMs) : 0;
        var maxResponseTimeMs = logsList.Any() ? logsList.Max(log => log.DurationMs) : 0;

        var totalRequestSizeBytes = logsList.Sum(log => log.RequestSizeBytes ?? 0);
        var totalResponseSizeBytes = logsList.Sum(log => log.ResponseSizeBytes ?? 0);

        var statusCodeCounts = logsList
            .GroupBy(log => log.StatusCode)
            .ToDictionary(g => g.Key, g => g.Count());

        var endpointCounts = logsList
            .GroupBy(log => log.Endpoint)
            .ToDictionary(g => g.Key, g => g.Count());

        var providerCounts = logsList
            .GroupBy(log => log.ProviderId)
            .ToDictionary(g => g.Key, g => g.Count());

        return new RequestStatisticsResponse
        {
            TotalRequests = totalRequests,
            SuccessfulRequests = successfulRequests,
            FailedRequests = failedRequests,
            AverageResponseTimeMs = averageResponseTimeMs,
            MinResponseTimeMs = minResponseTimeMs,
            MaxResponseTimeMs = maxResponseTimeMs,
            TotalRequestSizeBytes = totalRequestSizeBytes,
            TotalResponseSizeBytes = totalResponseSizeBytes,
            StatusCodeCounts = statusCodeCounts,
            EndpointCounts = endpointCounts,
            ProviderCounts = providerCounts
        };
    }

    private static void ValidateRequestLogRequest(CreateRequestLogRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Endpoint))
        {
            throw new ArgumentException("Endpoint cannot be null or whitespace", nameof(request.Endpoint));
        }

        if (string.IsNullOrWhiteSpace(request.Method))
        {
            throw new ArgumentException("Method cannot be null or whitespace", nameof(request.Method));
        }

        if (request.StatusCode < 100 || request.StatusCode > 599)
        {
            throw new ArgumentException("StatusCode must be between 100 and 599", nameof(request.StatusCode));
        }

        if (request.DurationMs <= 0)
        {
            throw new ArgumentException("DurationMs must be greater than 0", nameof(request.DurationMs));
        }

        if (request.RequestSizeBytes < 0)
        {
            throw new ArgumentException("RequestSizeBytes must be greater than or equal to 0", nameof(request.RequestSizeBytes));
        }

        if (request.ResponseSizeBytes < 0)
        {
            throw new ArgumentException("ResponseSizeBytes must be greater than or equal to 0", nameof(request.ResponseSizeBytes));
        }
    }
}