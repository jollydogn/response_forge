using System.Text.Json.Serialization;

namespace ResponseForge.Models;

/// <summary>
/// Represents the standardized API response envelope that wraps all API responses.
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// A human-readable message describing the result of the operation.
    /// Can be customized using the <see cref="Attributes.ResponseMessageAttribute"/>.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The response payload. Contains the actual data returned by the endpoint.
    /// Will be null for error responses or void endpoints.
    /// </summary>
    [JsonPropertyName("data")]
    public object? Data { get; set; }

    /// <summary>
    /// Validation errors or detailed error information.
    /// Keys represent field names, values contain the associated error messages.
    /// </summary>
    [JsonPropertyName("errors")]
    public Dictionary<string, List<string>>? Errors { get; set; }

    /// <summary>
    /// The exception stack trace. Only populated in Development environment
    /// when <see cref="Options.ResponseForgeOptions.IncludeStackTraceInDevelopment"/> is enabled.
    /// </summary>
    [JsonPropertyName("stack_trace")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// Creates a successful response with the specified data and message.
    /// </summary>
    /// <param name="data">The response payload.</param>
    /// <param name="message">The success message.</param>
    /// <returns>A new <see cref="ApiResponse"/> indicating success.</returns>
    public static ApiResponse Ok(object? data = null, string message = "Operation completed successfully.")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = null,
            StackTrace = null
        };
    }

    /// <summary>
    /// Creates a failed response with the specified message and optional error details.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">Optional dictionary of field-specific validation errors.</param>
    /// <returns>A new <see cref="ApiResponse"/> indicating failure.</returns>
    public static ApiResponse Fail(string message = "An error occurred.", Dictionary<string, List<string>>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Data = null,
            Errors = errors,
            StackTrace = null
        };
    }

    /// <summary>
    /// Creates an error response from an exception, optionally including the stack trace.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="message">The error message to display.</param>
    /// <param name="includeStackTrace">Whether to include the stack trace in the response.</param>
    /// <returns>A new <see cref="ApiResponse"/> containing exception details.</returns>
    public static ApiResponse Error(Exception exception, string message = "An unexpected error occurred.", bool includeStackTrace = false)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Data = null,
            Errors = null,
            StackTrace = includeStackTrace ? exception.ToString() : null
        };
    }
}
