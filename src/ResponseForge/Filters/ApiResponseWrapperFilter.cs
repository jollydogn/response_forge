using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using ResponseForge.Attributes;
using ResponseForge.Models;
using ResponseForge.Options;

namespace ResponseForge.Filters;

/// <summary>
/// An async result filter that wraps all <see cref="ObjectResult"/> responses
/// in the standardized <see cref="ApiResponse"/> envelope format.
/// <para>
/// This filter executes after the action method has produced its result
/// but before the result is serialized and sent to the client.
/// </para>
/// </summary>
/// <remarks>
/// The filter respects:
/// <list type="bullet">
///   <item><see cref="ResponseMessageAttribute"/> for per-endpoint message customization</item>
///   <item><see cref="ResponseForgeOptions.ExcludedPaths"/> for skipping specific routes</item>
///   <item>Non-object results (FileResult, RedirectResult, etc.) are passed through unchanged</item>
/// </list>
/// </remarks>
public class ApiResponseWrapperFilter : IAsyncResultFilter
{
    private readonly ResponseForgeOptions _options;

    public ApiResponseWrapperFilter(IOptions<ResponseForgeOptions> options)
    {
        _options = options.Value;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var requestPath = context.HttpContext.Request.Path.Value ?? string.Empty;

        // Skip wrapping for excluded paths (Swagger, health checks, etc.)
        if (IsExcludedPath(requestPath))
        {
            await next();
            return;
        }

        // Only wrap ObjectResult responses (Ok(), BadRequest(), NotFound(), etc.)
        // File downloads, redirects, and other special results pass through unchanged
        if (context.Result is ObjectResult objectResult)
        {
            // If the result is already an ApiResponse, don't double-wrap it
            if (objectResult.Value is ApiResponse)
            {
                await next();
                return;
            }

            var statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
            var isSuccess = statusCode >= 200 && statusCode < 300;

            // Resolve the message from the ResponseMessage attribute or fall back to defaults
            var message = ResolveMessage(context, isSuccess);

            ApiResponse envelope;

            if (isSuccess)
            {
                envelope = ApiResponse.Ok(objectResult.Value, message);
            }
            else
            {
                // For error ObjectResults, extract validation errors if present
                var errors = ExtractErrors(objectResult.Value);
                envelope = ApiResponse.Fail(message, errors);
            }

            context.Result = new ObjectResult(envelope)
            {
                StatusCode = statusCode
            };
        }
        else if (context.Result is StatusCodeResult statusCodeResult)
        {
            // Handle empty status code results (e.g., NoContent(), NotFound without body)
            var statusCode = statusCodeResult.StatusCode;
            var isSuccess = statusCode >= 200 && statusCode < 300;
            var message = ResolveMessage(context, isSuccess);

            var envelope = isSuccess
                ? ApiResponse.Ok(message: message)
                : ApiResponse.Fail(message);

            context.Result = new ObjectResult(envelope)
            {
                StatusCode = statusCode
            };
        }

        await next();
    }

    /// <summary>
    /// Resolves the response message by checking for a <see cref="ResponseMessageAttribute"/>
    /// on the action, falling back to the configured defaults.
    /// </summary>
    private string ResolveMessage(ResultExecutingContext context, bool isSuccess)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        var attribute = endpoint?.Metadata.GetMetadata<ResponseMessageAttribute>();

        if (attribute is not null)
        {
            return isSuccess
                ? attribute.SuccessMessage
                : attribute.ErrorMessage ?? _options.DefaultErrorMessage;
        }

        return isSuccess ? _options.DefaultSuccessMessage : _options.DefaultErrorMessage;
    }

    /// <summary>
    /// Extracts validation errors from common error response types
    /// such as <see cref="ValidationProblemDetails"/> or <see cref="ProblemDetails"/>.
    /// </summary>
    private static Dictionary<string, List<string>>? ExtractErrors(object? value)
    {
        if (value is ValidationProblemDetails validationProblem)
        {
            return validationProblem.Errors
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.ToList()
                );
        }

        if (value is SerializableError serializableError)
        {
            return serializableError
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value is string[] messages
                        ? messages.ToList()
                        : [kvp.Value?.ToString() ?? string.Empty]
                );
        }

        return null;
    }

    /// <summary>
    /// Checks if the request path matches any of the configured excluded path prefixes.
    /// </summary>
    private bool IsExcludedPath(string path)
    {
        return _options.ExcludedPaths.Any(excluded =>
            path.StartsWith(excluded, StringComparison.OrdinalIgnoreCase));
    }
}
