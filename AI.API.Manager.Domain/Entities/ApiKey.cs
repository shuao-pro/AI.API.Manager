using System;

namespace AI.API.Manager.Domain.Entities;

public class ApiKey
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ProviderId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string KeyValue { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public int RateLimitPerMinute { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ApiKey() { }

    public static ApiKey Create(
        Guid tenantId,
        Guid providerId,
        string name,
        string keyValue,
        bool isActive,
        int rateLimitPerMinute,
        DateTime? expiresAt)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or whitespace", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(keyValue))
        {
            throw new ArgumentException("KeyValue cannot be null or whitespace", nameof(keyValue));
        }

        if (rateLimitPerMinute <= 0)
        {
            throw new ArgumentException("RateLimitPerMinute must be greater than 0", nameof(rateLimitPerMinute));
        }

        return new ApiKey
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProviderId = providerId,
            Name = name.Trim(),
            KeyValue = keyValue.Trim(),
            IsActive = isActive,
            RateLimitPerMinute = rateLimitPerMinute,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string name,
        string keyValue,
        bool isActive,
        int rateLimitPerMinute,
        DateTime? expiresAt)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or whitespace", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(keyValue))
        {
            throw new ArgumentException("KeyValue cannot be null or whitespace", nameof(keyValue));
        }

        if (rateLimitPerMinute <= 0)
        {
            throw new ArgumentException("RateLimitPerMinute must be greater than 0", nameof(rateLimitPerMinute));
        }

        Name = name.Trim();
        KeyValue = keyValue.Trim();
        IsActive = isActive;
        RateLimitPerMinute = rateLimitPerMinute;
        ExpiresAt = expiresAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsExpired()
    {
        if (ExpiresAt == null)
        {
            return false;
        }

        return ExpiresAt.Value < DateTime.UtcNow;
    }
}