using AI.API.Manager.Domain.Enums;
using System;

namespace AI.API.Manager.Domain.Entities;

public class AIProvider
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string BaseUrl { get; private set; } = string.Empty;
    public AIProviderType ProviderType { get; private set; }
    public bool IsActive { get; private set; }
    public int MaxConcurrentRequests { get; private set; }
    public int RequestTimeoutSeconds { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private AIProvider() { }

    public static AIProvider Create(
        string name,
        string baseUrl,
        AIProviderType providerType,
        bool isActive,
        int maxConcurrentRequests,
        int requestTimeoutSeconds)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or whitespace", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentException("BaseUrl cannot be null or whitespace", nameof(baseUrl));
        }

        if (maxConcurrentRequests <= 0)
        {
            throw new ArgumentException("MaxConcurrentRequests must be greater than 0", nameof(maxConcurrentRequests));
        }

        if (requestTimeoutSeconds <= 0)
        {
            throw new ArgumentException("RequestTimeoutSeconds must be greater than 0", nameof(requestTimeoutSeconds));
        }

        return new AIProvider
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            BaseUrl = baseUrl.Trim(),
            ProviderType = providerType,
            IsActive = isActive,
            MaxConcurrentRequests = maxConcurrentRequests,
            RequestTimeoutSeconds = requestTimeoutSeconds,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string name,
        string baseUrl,
        AIProviderType providerType,
        bool isActive,
        int maxConcurrentRequests,
        int requestTimeoutSeconds)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or whitespace", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentException("BaseUrl cannot be null or whitespace", nameof(baseUrl));
        }

        if (maxConcurrentRequests <= 0)
        {
            throw new ArgumentException("MaxConcurrentRequests must be greater than 0", nameof(maxConcurrentRequests));
        }

        if (requestTimeoutSeconds <= 0)
        {
            throw new ArgumentException("RequestTimeoutSeconds must be greater than 0", nameof(requestTimeoutSeconds));
        }

        Name = name.Trim();
        BaseUrl = baseUrl.Trim();
        ProviderType = providerType;
        IsActive = isActive;
        MaxConcurrentRequests = maxConcurrentRequests;
        RequestTimeoutSeconds = requestTimeoutSeconds;
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
}