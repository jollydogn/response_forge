using System.Text.Json;

namespace ResponseForge.Options;

/// <summary>
/// Configuration options for the ResponseForge middleware pipeline.
/// Use these options to customize default messages, stack trace behavior, and path exclusions.
/// </summary>
/// <example>
/// <code>
/// builder.Services.AddResponseForge(options =>
/// {
///     options.DefaultSuccessMessage = "Request processed successfully.";
///     options.DefaultErrorMessage = "Something went wrong.";
///     options.IncludeStackTraceInDevelopment = true;
///     options.ExcludedPaths.Add("/custom-endpoint");
/// });
/// </code>
/// </example>
public class ResponseForgeOptions
{
    /// <summary>
    /// The default message returned for successful responses (2xx status codes)
    /// when no <see cref="Attributes.ResponseMessageAttribute"/> is applied to the endpoint.
    /// Default: "Operation completed successfully."
    /// </summary>
    public string DefaultSuccessMessage { get; set; } = "Operation completed successfully.";

    /// <summary>
    /// The default message returned for error responses (non-2xx status codes)
    /// when no <see cref="Attributes.ResponseMessageAttribute"/> is applied to the endpoint.
    /// Default: "An error occurred."
    /// </summary>
    public string DefaultErrorMessage { get; set; } = "An error occurred.";

    /// <summary>
    /// When set to true, includes the full exception stack trace in the response
    /// when running in the Development environment. Stack traces are never included
    /// in non-Development environments regardless of this setting.
    /// Default: true
    /// </summary>
    public bool IncludeStackTraceInDevelopment { get; set; } = true;

    /// <summary>
    /// A list of URL path prefixes to exclude from response wrapping.
    /// Requests matching these prefixes will pass through without modification.
    /// Default: ["/swagger", "/health", "/_framework"]
    /// </summary>
    public List<string> ExcludedPaths { get; set; } =
    [
        "/swagger",
        "/health",
        "/_framework"
    ];

    /// <summary>
    /// Custom JSON serializer options for the response output.
    /// If null, the default options with camelCase naming policy are used.
    /// </summary>
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }
}
