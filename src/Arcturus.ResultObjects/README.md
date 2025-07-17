# Arcturus.ResultObjects

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.ResultObjects.svg)](https://www.nuget.org/packages/Arcturus.ResultObjects) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.ResultObjects.svg)](https://www.nuget.org/packages/Arcturus.ResultObjects)

---

Arcturus.ResultObjects is a .NET library that provides a robust pattern for representing the outcome of operations, encapsulating both success and failure states. It enables clear handling of results, error information, and validation, making your code more expressive and reliable for both synchronous and asynchronous workflows.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.ResultObjects
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.ResultObjects
```

## Prerequisites

- .NET SDK 8 or later

## Features

| Feature                        | Description                                                                                          |
|--------------------------------|------------------------------------------------------------------------------------------------------|
| Result Pattern                 | Encapsulates operation outcomes as `Result` and `Result<T>`, distinguishing success and failure.     |
| Fault Object                   | Provides a `Fault` record for detailed error codes and messages.                                     |
| Exception Handling             | Attach exceptions to results for advanced error reporting.                                           |
| HTTP Status Integration        | Assign HTTP status codes to results for web API scenarios.                                           |
| Help Link Support              | Add help/documentation URLs to results for troubleshooting.                                          |
| Value Projection               | `Result<T>` supports carrying a value on success or error details on failure.                        |
| Implicit Conversion            | Easily convert values and faults to results using implicit operators.                                |
| Extension Methods              | Fluent API for attaching exceptions, status codes, and help links to results.                        |
| Async Compatibility            | Designed for use in both synchronous and asynchronous workflows.                                     |
| Validation & Error Propagation | Consistent propagation of validation errors and failure reasons.                                     |
| Integration Ready              | Works seamlessly with ASP.NET Core and other .NET platforms.                                         |

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).

