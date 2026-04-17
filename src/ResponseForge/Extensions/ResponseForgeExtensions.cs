using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ResponseForge.Filters;
using ResponseForge.Middleware;
using ResponseForge.Options;

namespace ResponseForge.Extensions;

/// <summary>
/// Extension methods for integrating ResponseForge into the ASP.NET Core pipeline.
/// </summary>
public static class ResponseForgeExtensions
{
    /// <summary>
    /// Registers ResponseForge services, including the global response wrapper filter
    /// and configuration options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional action to configure <see cref="ResponseForgeOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// builder.Services.AddResponseForge(options =>
    /// {
    ///     options.DefaultSuccessMessage = "Request processed successfully.";
    ///     options.IncludeStackTraceInDevelopment = true;
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddResponseForge(
        this IServiceCollection services,
        Action<ResponseForgeOptions>? configure = null)
    {
        // Register and configure options
        if (configure is not null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<ResponseForgeOptions>(_ => { });
        }

        // Register the wrapper filter globally without overriding existing MVC configuration
        services.PostConfigure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
        {
            options.Filters.Add<ApiResponseWrapperFilter>();
        });

        return services;
    }

    /// <summary>
    /// Adds the ResponseForge exception handling middleware to the application pipeline.
    /// <para>
    /// This should be called early in the middleware pipeline to catch all unhandled exceptions.
    /// </para>
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    /// <example>
    /// <code>
    /// var app = builder.Build();
    /// app.UseResponseForge();
    /// app.MapControllers();
    /// app.Run();
    /// </code>
    /// </example>
    public static IApplicationBuilder UseResponseForge(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
}
