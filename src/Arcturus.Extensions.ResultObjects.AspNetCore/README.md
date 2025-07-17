# Arcturus.Extensions.ResultObjects.AspNetCore

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.Extensions.ResultObjects.AspNetCore.svg)](https://www.nuget.org/packages/Arcturus.Extensions.ResultObjects.AspNetCore) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.Extensions.ResultObjects.AspNetCore.svg)](https://www.nuget.org/packages/Arcturus.Extensions.ResultObjects.AspNetCore)

---

Arcturus.Extensions.ResultObjects.AspNetCore is a .NET library that integrates the Arcturus.ResultObjects pattern with ASP.NET Core, providing standardized error handling and response formatting. It enables seamless conversion of operation results into HTTP responses using Problem Details, supports correlation and trace IDs, and enriches error responses with exception details in development environments.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.Extensions.ResultObjects.AspNetCore
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.Extensions.ResultObjects.AspNetCore
```

## Prerequisites

- .NET SDK 8 or later

## Features

| Feature                                 | Description                                                                                                         |
|------------------------------------------|---------------------------------------------------------------------------------------------------------------------|
| ProblemDetails Integration               | Converts Arcturus.ResultObjects to RFC7807-compliant ProblemDetails responses for standardized error handling.       |
| Correlation and Trace ID Support         | Automatically includes correlation and trace identifiers in error responses for improved diagnostics.                |
| Exception Details in Development         | Enriches ProblemDetails with exception information when running in development environments.                         |
| Customizable Error Mapping               | Utilizes ASP.NET Core's ApiBehaviorOptions for mapping HTTP status codes to error titles and types.                  |
| ActionResult Extensions                  | Provides ProblemDetailsActionResult for returning operation results directly from controller actions.                |
| Fault Object Mapping                     | Maps Arcturus Fault objects to ProblemDetails fields for clear error communication.                                 |
| HTTP Status Code Propagation             | Propagates custom HTTP status codes from Result objects to HTTP responses.                                          |
| Help Link and Instance URI Support       | Includes help links and request instance URIs in error responses for troubleshooting.                               |
| Seamless ASP.NET Core Integration        | Designed to work out-of-the-box with ASP.NET Core middleware and controllers.                                       |

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).

