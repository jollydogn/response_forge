# ResponseForge ⚒️

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-ResponseForge-blue?style=flat-square&logo=nuget)](https://www.nuget.org/packages/ResponseForge)

A lightweight, zero-boilerplate API response wrapper for **ASP.NET Core**. ResponseForge automatically wraps all your API responses in a consistent envelope format — no need to manually return wrapper objects from every endpoint.

---

## ✨ Features

- 🎯 **Automatic Response Wrapping** — All controller responses are wrapped in a standardized envelope
- 🛡️ **Global Exception Handling** — Unhandled exceptions are caught and formatted consistently
- 🏷️ **Custom Message Attribute** — Customize success/error messages per endpoint with `[ResponseMessage]`
- 🔧 **Configurable** — Default messages, excluded paths, and JSON options via `ResponseForgeOptions`
- 🐛 **Environment-Aware Stack Traces** — Stack traces included only in Development, always `null` in Production
- ⚡ **Lightweight** — Uses `IAsyncResultFilter` for wrapping (no response body buffering overhead)
- 🔌 **Non-Invasive** — Two lines to integrate: `AddResponseForge()` + `UseResponseForge()`

---

## 📦 Installation

```bash
dotnet add package ResponseForge
```

Or via Package Manager:

```powershell
Install-Package ResponseForge
```

---

## 🚀 Quick Start

### 1. Register Services

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddResponseForge(); // 👈 Add this

var app = builder.Build();

app.UseResponseForge(); // 👈 Add this (before MapControllers)
app.MapControllers();
app.Run();
```

### 2. That's It!

Your existing controllers will automatically return wrapped responses:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Product>> GetAll()
    {
        var products = _repository.GetAll();
        return Ok(products);
    }
}
```

**Response:**

```json
{
    "success": true,
    "message": "Operation completed successfully.",
    "data": [
        { "id": "...", "name": "Keyboard", "price": 149.99 }
    ],
    "errors": null,
    "stack_trace": null
}
```

---

## 🏷️ Custom Messages with `[ResponseMessage]`

Use the `[ResponseMessage]` attribute to set custom messages for specific endpoints:

```csharp
[HttpGet]
[ResponseMessage("Products retrieved successfully.", "Failed to retrieve products.")]
public ActionResult<List<Product>> GetAll()
{
    return Ok(products);
}
```

**Success Response:**

```json
{
    "success": true,
    "message": "Products retrieved successfully.",
    "data": [...],
    "errors": null,
    "stack_trace": null
}
```

The first parameter sets the **success message**, and the optional second parameter sets the **error message**.

---

## 🔧 Configuration

Customize ResponseForge behavior through `ResponseForgeOptions`:

```csharp
builder.Services.AddResponseForge(options =>
{
    options.DefaultSuccessMessage = "Request processed successfully.";
    options.DefaultErrorMessage = "Something went wrong. Please try again.";
    options.IncludeStackTraceInDevelopment = true;
    options.ExcludedPaths = ["/swagger", "/health", "/metrics"];
});
```

### Available Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `DefaultSuccessMessage` | `string` | `"Operation completed successfully."` | Default message for 2xx responses |
| `DefaultErrorMessage` | `string` | `"An error occurred."` | Default message for non-2xx responses |
| `IncludeStackTraceInDevelopment` | `bool` | `true` | Include stack traces in Development env |
| `ExcludedPaths` | `List<string>` | `["/swagger", "/health", "/_framework"]` | URL prefixes to skip wrapping |
| `JsonSerializerOptions` | `JsonSerializerOptions?` | `null` | Custom JSON serializer settings |

---

## 🛡️ Exception Handling

ResponseForge maps common exception types to appropriate HTTP status codes:

| Exception Type | HTTP Status | Default Message |
|---------------|-------------|-----------------|
| `ValidationException` | 400 Bad Request | Exception message + field errors |
| `ArgumentException` | 400 Bad Request | Exception message |
| `KeyNotFoundException` | 404 Not Found | "The requested resource was not found." |
| `FileNotFoundException` | 404 Not Found | "The requested resource was not found." |
| `UnauthorizedAccessException` | 401 Unauthorized | "You are not authorized to perform this action." |
| `InvalidOperationException` | 409 Conflict | Exception message |
| `NotImplementedException` | 501 Not Implemented | "This feature is not yet implemented." |
| Other exceptions | 500 Internal Error | Configured `DefaultErrorMessage` |

### Error Response Example

```json
{
    "success": false,
    "message": "The requested resource was not found.",
    "data": null,
    "errors": null,
    "stack_trace": "System.Collections.Generic.KeyNotFoundException: ..."
}
```

> **Note:** `stack_trace` is only populated in the **Development** environment. In Production, it is always `null`.

---

## 📋 Validation Errors

Model validation errors are automatically extracted and structured in the `errors` field:

```json
{
    "success": false,
    "message": "An error occurred.",
    "data": null,
    "errors": {
        "Name": ["Product name is required."],
        "Price": ["Price must be between 0.01 and 999,999.99."]
    },
    "stack_trace": null
}
```

---

## 🏗️ Response Format

Every response follows this structure:

```typescript
interface ApiResponse {
    success: boolean;        // Whether the operation succeeded
    message: string;         // Human-readable result description
    data: any | null;        // Response payload (null on errors)
    errors: object | null;   // Validation/field-specific errors
    stack_trace: string | null; // Exception trace (Development only)
}
```

---

## 📁 Project Structure

```
ResponseForge/
├── src/
│   ├── ResponseForge/                    # NuGet library
│   │   ├── Attributes/
│   │   │   └── ResponseMessageAttribute  # Per-endpoint message customization
│   │   ├── Extensions/
│   │   │   └── ResponseForgeExtensions   # DI registration helpers
│   │   ├── Filters/
│   │   │   └── ApiResponseWrapperFilter  # IAsyncResultFilter implementation
│   │   ├── Middleware/
│   │   │   └── ExceptionHandlingMiddleware # Global exception handler
│   │   ├── Models/
│   │   │   └── ApiResponse               # Response envelope model
│   │   └── Options/
│   │       └── ResponseForgeOptions      # Configuration options
│   │
│   └── ResponseForge.Sample/            # Sample API project
│       ├── Controllers/
│       │   ├── ProductsController        # CRUD success scenarios
│       │   └── OrdersController          # Error scenario demonstrations
│       ├── Models/
│       └── Program.cs                    # Integration example
│
├── ResponseForge.sln
├── README.md
└── LICENSE
```

---

## 🤝 Contributing

Contributions are welcome! Feel free to open issues and submit pull requests.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'feat: add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 🔗 Links

- [GitHub Repository](https://github.com/jollydogn/response_forge)
- [Report an Issue](https://github.com/jollydogn/response_forge/issues)

---

Made with ⚒️ by [ResponseForge](https://github.com/jollydogn)
