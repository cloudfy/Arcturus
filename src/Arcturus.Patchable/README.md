# Arcturus.Patchable

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.Patchable.svg)](https://www.nuget.org/packages/Arcturus.Patchable) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.Patchable.svg)](https://www.nuget.org/packages/Arcturus.Patchable)

---

Arcturus.Patchable is a .NET library that provides a flexible and type-safe framework for applying partial updates ("patches") to objects, commonly used in RESTful APIs and data processing scenarios. It supports JSON Patch-like operations, custom patch logic, and validation, enabling efficient and controlled modifications to your domain models.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.Patchable
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.Patchable
```

## Prerequisites

- .NET SDK 8 or later

## Features

| Feature                        | Description                                                                                          |
|--------------------------------|------------------------------------------------------------------------------------------------------|
| Object Patch Abstraction       | Provides `IObjectPatch<T>` and `ObjectPatch<T>` for defining and applying patch operations to objects.|
| Patch Operations               | Supports standard operations like add, remove, replace, and custom patch logic via `PatchOperation`. |
| Patch Requests & Results       | Encapsulates patch requests (`PatchRequest<T>`) and results (`PatchResult<T>`) for robust workflows. |
| Options & Validation           | Configurable patch options (`PatchOptions`) and built-in validation for safe updates.                |
| JSON Patch Compatibility       | Designed to work with JSON Patch document formats for API scenarios.                                 |
| Type Safety                    | Strongly-typed patching ensures compile-time safety and reduces runtime errors.                      |
| Extensibility                  | Easily extendable for custom patch behaviors and domain-specific requirements.                       |
| Error Reporting                | Detailed error information for failed patch operations.                                              |

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).

