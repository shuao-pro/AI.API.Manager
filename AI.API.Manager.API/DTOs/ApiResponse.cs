using System.Text.Json.Serialization;

namespace AI.API.Manager.API.DTOs;

/// <summary>
/// 统一的API响应格式
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// 是否成功
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// 响应数据
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    /// <summary>
    /// 错误代码
    /// </summary>
    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 创建成功响应
    /// </summary>
    /// <param name="data">响应数据</param>
    /// <returns>成功响应</returns>
    public static ApiResponse<T> SuccessResponse(T data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Error = null,
            ErrorCode = null
        };
    }

    /// <summary>
    /// 创建失败响应
    /// </summary>
    /// <param name="error">错误消息</param>
    /// <param name="errorCode">错误代码</param>
    /// <returns>失败响应</returns>
    public static ApiResponse<T> ErrorResponse(string error, string? errorCode = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Data = default,
            Error = error,
            ErrorCode = errorCode
        };
    }
}

/// <summary>
/// 无数据的API响应
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// 是否成功
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    /// <summary>
    /// 错误代码
    /// </summary>
    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 创建成功响应
    /// </summary>
    /// <returns>成功响应</returns>
    public static ApiResponse SuccessResponse()
    {
        return new ApiResponse
        {
            Success = true,
            Error = null,
            ErrorCode = null
        };
    }

    /// <summary>
    /// 创建失败响应
    /// </summary>
    /// <param name="error">错误消息</param>
    /// <param name="errorCode">错误代码</param>
    /// <returns>失败响应</returns>
    public static ApiResponse ErrorResponse(string error, string? errorCode = null)
    {
        return new ApiResponse
        {
            Success = false,
            Error = error,
            ErrorCode = errorCode
        };
    }
}