using System;

namespace AI.API.Manager.Domain.Entities;

public class RequestLog
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ProviderId { get; private set; }
    public Guid ApiKeyId { get; private set; }
    public string Endpoint { get; private set; } = string.Empty;
    public string Method { get; private set; } = string.Empty;
    public int StatusCode { get; private set; }
    public int DurationMs { get; private set; }
    public int? RequestSizeBytes { get; private set; }
    public int? ResponseSizeBytes { get; private set; }
    public string? UserAgent { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private RequestLog() { }

    public static RequestLog Create(
        Guid tenantId,
        Guid providerId,
        Guid apiKeyId,
        string endpoint,
        string method,
        int statusCode,
        int durationMs,
        int? requestSizeBytes,
        int? responseSizeBytes,
        string? userAgent,
        string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new ArgumentException("Endpoint cannot be null or whitespace", nameof(endpoint));
        }

        if (string.IsNullOrWhiteSpace(method))
        {
            throw new ArgumentException("Method cannot be null or whitespace", nameof(method));
        }

        if (statusCode < 100 || statusCode > 599)
        {
            throw new ArgumentException("StatusCode must be between 100 and 599", nameof(statusCode));
        }

        if (durationMs <= 0)
        {
            throw new ArgumentException("DurationMs must be greater than 0", nameof(durationMs));
        }

        if (requestSizeBytes < 0)
        {
            throw new ArgumentException("RequestSizeBytes must be greater than or equal to 0", nameof(requestSizeBytes));
        }

        if (responseSizeBytes < 0)
        {
            throw new ArgumentException("ResponseSizeBytes must be greater than or equal to 0", nameof(responseSizeBytes));
        }

        return new RequestLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProviderId = providerId,
            ApiKeyId = apiKeyId,
            Endpoint = endpoint.Trim(),
            Method = method.Trim().ToUpperInvariant(),
            StatusCode = statusCode,
            DurationMs = durationMs,
            RequestSizeBytes = requestSizeBytes,
            ResponseSizeBytes = responseSizeBytes,
            UserAgent = userAgent?.Trim(),
            IpAddress = ipAddress?.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool IsSuccessful()
    {
        return StatusCode >= 200 && StatusCode < 300;
    }
}