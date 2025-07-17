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

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).

