using AI.API.Manager.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AI.API.Manager.Tests.Domain.Entities;

public class RequestLogTests
{
    [Fact]
    public void Create_ShouldCreateRequestLogWithValidProperties()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var apiKeyId = Guid.NewGuid();
        var endpoint = "/v1/chat/completions";
        var method = "POST";
        var statusCode = 200;
        var durationMs = 1500;
        var requestSizeBytes = 1024;
        var responseSizeBytes = 2048;
        var userAgent = "TestClient/1.0";
        var ipAddress = "192.168.1.1";

        // Act
        var requestLog = RequestLog.Create(
            tenantId,
            providerId,
            apiKeyId,
            endpoint,
            method,
            statusCode,
            durationMs,
            requestSizeBytes,
            responseSizeBytes,
            userAgent,
            ipAddress);

        // Assert
        requestLog.Should().NotBeNull();
        requestLog.Id.Should().NotBeEmpty();
        requestLog.TenantId.Should().Be(tenantId);
        requestLog.ProviderId.Should().Be(providerId);
        requestLog.ApiKeyId.Should().Be(apiKeyId);
        requestLog.Endpoint.Should().Be(endpoint);
        requestLog.Method.Should().Be(method);
        requestLog.StatusCode.Should().Be(statusCode);
        requestLog.DurationMs.Should().Be(durationMs);
        requestLog.RequestSizeBytes.Should().Be(requestSizeBytes);
        requestLog.ResponseSizeBytes.Should().Be(responseSizeBytes);
        requestLog.UserAgent.Should().Be(userAgent);
        requestLog.IpAddress.Should().Be(ipAddress);
        requestLog.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenEndpointIsNull()
    {
        // Arrange
        var action = () => RequestLog.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            null!,
            "POST",
            200,
            1500,
            1024,
            2048,
            "TestClient/1.0",
            "192.168.1.1");

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Endpoint cannot be null or whitespace (Parameter 'endpoint')");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowArgumentException_WhenEndpointIsEmptyOrWhiteSpace(string invalidEndpoint)
    {
        // Arrange
        var action = () => RequestLog.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            invalidEndpoint,
            "POST",
            200,
            1500,
            1024,
            2048,
            "TestClient/1.0",
            "192.168.1.1");

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Endpoint cannot be null or whitespace (Parameter 'endpoint')");
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenMethodIsNull()
    {
        // Arrange
        var action = () => RequestLog.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/v1/chat/completions",
            null!,
            200,
            1500,
            1024,
            2048,
            "TestClient/1.0",
            "192.168.1.1");

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Method cannot be null or whitespace (Parameter 'method')");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldThrowArgumentException_WhenMethodIsEmptyOrWhiteSpace(string invalidMethod)
    {
        // Arrange
        var action = () => RequestLog.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/v1/chat/completions",
            invalidMethod,
            200,
            1500,
            1024,
            2048,
            "TestClient/1.0",
            "192.168.1.1");

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Method cannot be null or whitespace (Parameter 'method')");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(99)]
    [InlineData(600)]
    public void Create_ShouldThrowArgumentException_WhenStatusCodeIsInvalid(int invalidStatusCode)
    {
        // Arrange
        var action = () => RequestLog.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/v1/chat/completions",
            "POST",
            invalidStatusCode,
            1500,
            1024,
            2048,
            "TestClient/1.0",
            "192.168.1.1");

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("StatusCode must be between 100 and 599 (Parameter 'statusCode')");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Create_ShouldThrowArgumentException_WhenDurationMsIsInvalid(int invalidDuration)
    {
        // Arrange
        var action = () => RequestLog.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/v1/chat/completions",
            "POST",
            200,
            invalidDuration,
            1024,
            2048,
            "TestClient/1.0",
            "192.168.1.1");

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("DurationMs must be greater than 0 (Parameter 'durationMs')");
    }

    [Theory]
    [InlineData(-1)]
    public void Create_ShouldThrowArgumentException_WhenRequestSizeBytesIsInvalid(int invalidSize)
    {
        // Arrange
        var action = () => RequestLog.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/v1/chat/completions",
            "POST",
            200,
            1500,
            invalidSize,
            2048,
            "TestClient/1.0",
            "192.168.1.1");

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("RequestSizeBytes must be greater than or equal to 0 (Parameter 'requestSizeBytes')");
    }

    [Theory]
    [InlineData(-1)]
    public void Create_ShouldThrowArgumentException_WhenResponseSizeBytesIsInvalid(int invalidSize)
    {
        // Arrange
        var action = () => RequestLog.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/v1/chat/completions",
            "POST",
            200,
            1500,
            1024,
            invalidSize,
            "TestClient/1.0",
            "192.168.1.1");

        // Act & Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("ResponseSizeBytes must be greater than or equal to 0 (Parameter 'responseSizeBytes')");
    }

    [Fact]
    public void Create_ShouldSetOptionalPropertiesToNull_WhenNotProvided()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var apiKeyId = Guid.NewGuid();
        var endpoint = "/v1/chat/completions";
        var method = "POST";
        var statusCode = 200;
        var durationMs = 1500;

        // Act
        var requestLog = RequestLog.Create(
            tenantId,
            providerId,
            apiKeyId,
            endpoint,
            method,
            statusCode,
            durationMs,
            null,
            null,
            null,
            null);

        // Assert
        requestLog.RequestSizeBytes.Should().BeNull();
        requestLog.ResponseSizeBytes.Should().BeNull();
        requestLog.UserAgent.Should().BeNull();
        requestLog.IpAddress.Should().BeNull();
    }

    [Fact]
    public void IsSuccessful_ShouldReturnTrue_WhenStatusCodeIs2xx()
    {
        // Arrange
        var requestLog = RequestLog.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/v1/chat/completions",
            "POST",
            200,
            1500,
            1024,
            2048,
            "TestClient/1.0",
            "192.168.1.1");

        // Act
        var isSuccessful = requestLog.IsSuccessful();

        // Assert
        isSuccessful.Should().BeTrue();
    }

    [Fact]
    public void IsSuccessful_ShouldReturnFalse_WhenStatusCodeIs4xx()
    {
        // Arrange
        var requestLog = RequestLog.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/v1/chat/completions",
            "POST",
            400,
            1500,
            1024,
            2048,
            "TestClient/1.0",
            "192.168.1.1");

        // Act
        var isSuccessful = requestLog.IsSuccessful();

        // Assert
        isSuccessful.Should().BeFalse();
    }

    [Fact]
    public void IsSuccessful_ShouldReturnFalse_WhenStatusCodeIs5xx()
    {
        // Arrange
        var requestLog = RequestLog.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/v1/chat/completions",
            "POST",
            500,
            1500,
            1024,
            2048,
            "TestClient/1.0",
            "192.168.1.1");

        // Act
        var isSuccessful = requestLog.IsSuccessful();

        // Assert
        isSuccessful.Should().BeFalse();
    }
}