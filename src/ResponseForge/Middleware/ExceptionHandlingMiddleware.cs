using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ResponseForge.Models;
using ResponseForge.Options;

namespace ResponseForge.Middleware;

/// <summary>
/// Middleware that catches unhandled exceptions and returns them
/// in the standardized <see cref="ApiResponse"/> format.
/// <para>
/// In Development environments, the full stack trace is included in the response
/// when <see cref="ResponseForgeOptions.IncludeStackTraceInDevelopment"/> is enabled.
/// In all other environments, the stack trace is always null.
/// </para>
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly ResponseForgeOptions _options;
    private readonly IWebHostEnvironment _environment;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IOptions<ResponseForgeOptions> options,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
        _environment = environment;
        _jsonOptions = _options.JsonSerializerOptions ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred while processing {Method} {Path}",
            context.Request.Method, context.Request.Path);

        var (statusCode, message, errors) = MapException(exception);
        var includeStackTrace = _environment.IsDevelopment() && _options.IncludeStackTraceInDevelopment;

        var response = new ApiResponse
        {
            Success = false,
            Message = message,
            Data = null,
            Errors = errors,
            StackTrace = includeStackTrace ? exception.ToString() : null
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(response, _jsonOptions);
        await context.Response.WriteAsync(json);
    }

    /// <summary>
    /// Maps known exception types to appropriate HTTP status codes and messages.
    /// </summary>
    private (int StatusCode, string Message, Dictionary<string, List<string>>? Errors) MapException(Exception exception)
    {
        return exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                validationEx.Message,
                ExtractValidationErrors(validationEx)
            ),

            ArgumentException argumentEx => (
                StatusCodes.Status400BadRequest,
                argumentEx.Message,
                null
            ),

            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "The requested resource was not found.",
                null
            ),

            FileNotFoundException => (
                StatusCodes.Status404NotFound,
                "The requested resource was not found.",
                null
            ),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "You are not authorized to perform this action.",
                null
            ),

            InvalidOperationException invalidOpEx => (
                StatusCodes.Status409Conflict,
                invalidOpEx.Message,
                null
            ),

            NotImplementedException => (
                StatusCodes.Status501NotImplemented,
                "This feature is not yet implemented.",
                null
            ),

            OperationCanceledException => (
                StatusCodes.Status400BadRequest,
                "The request was cancelled by the client.",
                null
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                _options.DefaultErrorMessage,
                null
            )
        };
    }

    /// <summary>
    /// Extracts structured validation error details from a <see cref="ValidationException"/>.
    /// </summary>
    private static Dictionary<string, List<string>>? ExtractValidationErrors(ValidationException exception)
    {
        if (exception.ValidationResult?.MemberNames?.Any() == true)
        {
            var errors = new Dictionary<string, List<string>>();

            foreach (var memberName in exception.ValidationResult.MemberNames)
            {
                errors[memberName] = [exception.ValidationResult.ErrorMessage ?? "Validation failed."];
            }

            return errors;
        }

        return new Dictionary<string, List<string>>
        {
            ["General"] = [exception.Message]
        };
    }
}
