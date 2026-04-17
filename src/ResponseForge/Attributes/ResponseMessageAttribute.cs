namespace ResponseForge.Attributes;

/// <summary>
/// Specifies custom success and error messages for an API endpoint.
/// When applied to a controller action, the middleware uses the provided messages
/// instead of the default ones configured in <see cref="Options.ResponseForgeOptions"/>.
/// </summary>
/// <example>
/// <code>
/// [HttpGet]
/// [ResponseMessage("Products retrieved successfully.", "Failed to retrieve products.")]
/// public async Task&lt;List&lt;Product&gt;&gt; GetAll() { ... }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ResponseMessageAttribute : Attribute
{
    /// <summary>
    /// The message to return when the operation completes successfully (2xx status codes).
    /// </summary>
    public string SuccessMessage { get; }

    /// <summary>
    /// The message to return when the operation fails (non-2xx status codes).
    /// If not specified, the default error message from <see cref="Options.ResponseForgeOptions"/> is used.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseMessageAttribute"/> class.
    /// </summary>
    /// <param name="successMessage">The message to display on successful responses.</param>
    /// <param name="errorMessage">Optional message to display on error responses.</param>
    public ResponseMessageAttribute(string successMessage, string? errorMessage = null)
    {
        SuccessMessage = successMessage;
        ErrorMessage = errorMessage;
    }
}
