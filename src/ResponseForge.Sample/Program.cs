using ResponseForge.Extensions;
using ResponseForge.Sample.Services;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Register Mock Service
builder.Services.AddSingleton<IMockDataAppService, MockDataAppService>();

// Add ResponseForge with custom configuration
builder.Services.AddResponseForge(options =>
{
    options.DefaultSuccessMessage = "Operation completed successfully.";
    options.DefaultErrorMessage = "An unexpected error occurred. Please try again later.";
    options.IncludeStackTraceInDevelopment = true;
    options.ExcludedPaths = ["/swagger", "/health", "/_framework"];
});

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ResponseForge Sample API",
        Version = "v1",
        Description = "A sample API demonstrating ResponseForge middleware integration. " +
                      "All responses are automatically wrapped in a standardized envelope format.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "ResponseForge",
            Url = new Uri("https://github.com/jollydogn/response_forge")
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
});

var app = builder.Build();

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ResponseForge Sample API v1");
        options.RoutePrefix = "swagger";
    });
}

// Add ResponseForge exception handling middleware (should be early in the pipeline)
app.UseResponseForge();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
