# Arcturus.AspNetCore.Endpoints

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.AspNetCore.Endpoints.svg)](https://www.nuget.org/packages/Arcturus.AspNetCore.Endpoints) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.AspNetCore.Endpoints.svg)](https://www.nuget.org/packages/Arcturus.AspNetCore.Endpoints)

---

Arcturus.AspNetCore.Endpoints is a .NET library that provides a flexible abstraction for defining HTTP API endpoints in ASP.NET Core applications. It enables developers to implement endpoints using a consistent pattern, supporting both synchronous and asynchronous operations, with or without request objects, and integrates seamlessly with ASP.NET Core routing and controller infrastructure.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.AspNetCore.Endpoints
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.AspNetCore.Endpoints
```

## Prerequisites

- .NET SDK 8 or later

## Features

- Abstraction for single endpoints using `AbstractEndpoint` base class.
- Automatic application of `[ApiController]` and `[Route]` attributes for endpoint classes.
- Builder pattern via `EndpointsBuilder` for defining endpoints with or without request objects.
- Support for both synchronous and asynchronous endpoint handlers.
- Flexible return types: plain results, `ActionResult<T>`, `IActionResult`, and `IAsyncEnumerable<T>`.
- Endpoint definitions for:
  - Endpoints with request and response types.
  - Endpoints with request but no response.
  - Endpoints without request, with or without response.
- Integration with ASP.NET Core controller routing using the `[controller]` template.
- Clean separation of endpoint logic from controller boilerplate.
- Extensible for custom endpoint behaviors and patterns.
- **Global endpoint conventions**: Apply common conventions like `RequireAuthorization()`, `AllowAnonymous()`, `WithMetadata()`, etc. to all endpoints registered by a module.

## Usage

### Basic Endpoint Module

Implement the `IEndPointModule` interface to create an endpoint module:

```csharp
using Arcturus.AspNetCore.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

public class UsersEndpointModule : IEndPointModule
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", () => "List of users");
        app.MapGet("/api/users/{id}", (int id) => $"User {id}");
    }
}
```

### Applying Global Conventions

To apply conventions to all endpoints in a module, implement the `IConfigurableEndPointModule` interface:

```csharp
public class SecureEndpointModule : IConfigurableEndPointModule
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        // Basic implementation - can delegate to the configurable overload
        AddRoute(app, null);
    }

    public void AddRoute(IEndpointRouteBuilder app, Action<IEndpointConventionBuilder>? configure = null)
    {
        var getEndpoint = app.MapGet("/api/secure/data", () => "Secure data");
        configure?.Invoke(getEndpoint);

        var postEndpoint = app.MapPost("/api/secure/data", (object data) => Results.Ok());
        configure?.Invoke(postEndpoint);
    }
}
```

**Note:** Modules implementing only `IEndPointModule` (without the configurable overload) will continue to work for backward compatibility.

### Registration and Configuration

Register endpoint modules in your `Program.cs`:

```csharp
using Arcturus.AspNetCore.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Register all endpoint modules from the calling assembly
builder.Services.AddEndpointModules();

var app = builder.Build();

// Option 1: Use endpoints without global conventions
app.UseEndpointModules();

// Option 2: Apply global authorization to all endpoints
app.UseEndpointModules(endpoint => endpoint.RequireAuthorization());

// Option 3: Apply multiple conventions
app.UseEndpointModules(endpoint =>
{
    endpoint.RequireAuthorization("AdminPolicy");
    endpoint.WithMetadata("GlobalConvention", true);
});

// Option 4: Mix authorization with specific endpoints allowing anonymous access
app.UseEndpointModules(endpoint => endpoint.RequireAuthorization());
// Individual modules can still override by adding .AllowAnonymous() to specific endpoints

app.Run();
```

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).

