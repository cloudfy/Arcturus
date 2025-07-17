# Arcturus.Extensions.Patchable.AspNetCore

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.Extensions.Patchable.AspNetCore.svg)](https://www.nuget.org/packages/Arcturus.Extensions.Patchable.AspNetCore) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.Extensions.Patchable.AspNetCore.svg)](https://www.nuget.org/packages/Arcturus.Extensions.Patchable.AspNetCore)

---

Arcturus.Extensions.Patchable.AspNetCore is a .NET library that enables efficient partial updates of resources in ASP.NET Core applications using JSON Patch. It provides extension methods and utilities for applying RFC 6902-compliant patch operations to your models, simplifying the implementation of PATCH endpoints and improving API flexibility.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.Extensions.Patchable.AspNetCore
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.Extensions.Patchable.AspNetCore
```

## Prerequisites

- .NET SDK 8 or later

## Features

- **JSON Patch Support**: Apply RFC 6902-compliant JSON Patch operations to your ASP.NET Core models.
- **Extension Methods for Controllers**: Simplifies PATCH endpoint implementation with controller extensions for patching resources.
- **Model Validation Integration**: Automatically validates patched models and integrates with ASP.NET Core's model validation pipeline.
- **Error Handling**: Provides clear error responses for invalid patch documents or failed patch operations.
- **Flexible Patch Application**: Supports patching of complex object graphs and nested properties.
- **Seamless Integration**: Designed to work with ASP.NET Core's dependency injection and middleware pipeline.
- **Documentation and Examples**: Includes usage documentation and code samples for quick adoption.

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).

