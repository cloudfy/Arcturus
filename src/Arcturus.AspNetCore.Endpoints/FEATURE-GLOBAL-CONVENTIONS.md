# Global Endpoint Convention Configuration Feature

## Overview

This feature extends the `IEndPointModule` interface and `UseEndpointModules` method to support global endpoint conventions, allowing developers to apply common configurations like authorization, metadata, or other conventions to all endpoints registered by endpoint modules.

## New Capabilities

### 1. Optional Convention Delegate in IEndPointModule

The `IEndPointModule` interface now includes an overload with an optional `Action<IEndpointConventionBuilder>?` parameter:

```csharp
void AddRoute(IEndpointRouteBuilder app, Action<IEndpointConventionBuilder>? configure = null)
```

This overload has a default implementation that calls the existing `AddRoute(IEndpointRouteBuilder)` method, ensuring **backward compatibility** with existing endpoint modules.

### 2. UseEndpointModules with Convention Support

The `UseEndpointModules` extension method now accepts an optional convention delegate:

```csharp
app.UseEndpointModules(endpoint => endpoint.RequireAuthorization());
```

This delegate is passed to each registered endpoint module, which can then apply it to individual endpoints.

### 3. MapEndpointModules (Obsolete) Support

The obsolete `MapEndpointModules` method also supports the new convention parameter for backward compatibility.

## Usage Examples

### Apply Global Authorization

```csharp
app.UseEndpointModules(endpoint => endpoint.RequireAuthorization());
```

All endpoints that invoke the convention delegate will require authorization.

### Apply Multiple Conventions

```csharp
app.UseEndpointModules(endpoint =>
{
	endpoint.RequireAuthorization("AdminPolicy");
	endpoint.WithMetadata("ApiVersion", "v1");
	endpoint.WithDisplayName("Global API Endpoint");
});
```

### Selective Application in Modules

Modules can choose which endpoints receive the global conventions:

```csharp
public class MyEndpointModule : IEndPointModule
{
	public void AddRoute(IEndpointRouteBuilder app)
	{
		AddRoute(app, null);
	}

	public void AddRoute(IEndpointRouteBuilder app, Action<IEndpointConventionBuilder>? configure = null)
	{
		// Public endpoint - no conventions
		app.MapGet("/api/public", () => "Public data");

		// Protected endpoint - apply conventions
		var secureEndpoint = app.MapGet("/api/secure", () => "Secure data");
		configure?.Invoke(secureEndpoint);
	}
}
```

## Backward Compatibility

- **Existing modules** that only implement `AddRoute(IEndpointRouteBuilder)` continue to work without modification.
- The new overload with the convention parameter has a default implementation that delegates to the original method.
- If `UseEndpointModules()` is called without a convention parameter, modules receive `null` and can handle it gracefully.

## Benefits

1. **Centralized Configuration**: Apply common conventions (authorization, metadata, rate limiting, etc.) in one place.
2. **Flexibility**: Modules can selectively apply or ignore global conventions on a per-endpoint basis.
3. **Clean Code**: Reduces repetition of common endpoint configurations across multiple modules.
4. **Testability**: Convention delegates can be easily tested and swapped for different environments.

## Migration Guide

### For Existing Code (No Changes Required)

Existing endpoint modules will continue to work without modification:

```csharp
public class LegacyModule : IEndPointModule
{
	public void AddRoute(IEndpointRouteBuilder app)
	{
		app.MapGet("/api/legacy", () => "Works as before");
	}
}
```

### For New Modules (Recommended Pattern)

Implement both overloads to support convention configuration:

```csharp
public class ModernModule : IEndPointModule
{
	public void AddRoute(IEndpointRouteBuilder app)
	{
		AddRoute(app, null);
	}

	public void AddRoute(IEndpointRouteBuilder app, Action<IEndpointConventionBuilder>? configure = null)
	{
		var endpoint = app.MapGet("/api/modern", () => "Modern endpoint");
		configure?.Invoke(endpoint);
	}
}
```

## Files Changed

- `IEndPointModule.cs` - Added convention parameter overload
- `ServiceCollectionExtensions.cs` - Updated UseEndpointModules and MapEndpointModules to support conventions
- `EndpointModuleConfiguration.cs` - Added XML documentation
- `README.md` - Updated with usage examples
- `Examples/ExampleEndpointModule.cs` - Demonstration implementation
- `Samples/SampleEndpointModules.cs` - Multiple sample patterns
- `Samples/Program.cs.sample` - Usage examples in a Program.cs context

## Related ASP.NET Core Types

- `IEndpointConventionBuilder` - Base interface for applying conventions
- `RouteHandlerBuilder` - Implements IEndpointConventionBuilder for minimal APIs
- Common convention methods:
  - `RequireAuthorization()`
  - `AllowAnonymous()`
  - `WithMetadata()`
  - `WithName()`
  - `WithDisplayName()`
  - `WithGroupName()`
  - `WithTags()`
