using Grand.Module.MobileApi.Constants;

namespace Grand.Module.MobileApi.DTOs;

/// <summary>
/// Standard mobile API response wrapper
/// </summary>
/// <typeparam name="T">Response data type</typeparam>
public class MobileResponseDto<T>
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response data (null on error)
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Success message
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Error information (null on success)
    /// </summary>
    public MobileErrorDto? Error { get; set; }

    /// <summary>
    /// Response metadata
    /// </summary>
    public MobileMetaDto Meta { get; set; } = new();

    /// <summary>
    /// Create success response
    /// </summary>
    public static MobileResponseDto<T> CreateSuccess(T data, string? message = null)
    {
        return new MobileResponseDto<T>
        {
            Success = true,
            Data = data,
            Message = message,
            Meta = new MobileMetaDto
            {
                Timestamp = DateTime.UtcNow,
                Version = MobileApiConstants.ApiVersion
            }
        };
    }

    /// <summary>
    /// Create error response
    /// </summary>
    public static MobileResponseDto<T> CreateError(string code, string message, object? details = null)
    {
        return new MobileResponseDto<T>
        {
            Success = false,
            Error = new MobileErrorDto
            {
                Code = code,
                Message = message,
                Details = details
            },
            Meta = new MobileMetaDto
            {
                Timestamp = DateTime.UtcNow,
                Version = MobileApiConstants.ApiVersion
            }
        };
    }
}

/// <summary>
/// Error information for mobile API responses
/// </summary>
public class MobileErrorDto
{
    /// <summary>
    /// Error code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Additional error details (validation errors, etc.)
    /// </summary>
    public object? Details { get; set; }
}

/// <summary>
/// Response metadata
/// </summary>
public class MobileMetaDto
{
    /// <summary>
    /// Response timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// API version
    /// </summary>
    public string Version { get; set; } = MobileApiConstants.ApiVersion;
}