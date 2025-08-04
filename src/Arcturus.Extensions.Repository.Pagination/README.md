# Arcturus.Extensions.Repository.Pagination

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.Extensions.Repository.Pagination.svg)](https://www.nuget.org/packages/Arcturus.Extensions.Repository.Pagination) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.Extensions.Repository.Pagination.svg)](https://www.nuget.org/packages/Arcturus.Extensions.Repository.Pagination)

---

A .NET library providing robust, extensible pagination support for repository patterns. Designed for use in modern .NET applications, it enables efficient, consistent, and customizable data paging for queries and repositories.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.Extensions.Repository.Pagination
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.Extensions.Repository.Pagination
```

## Prerequisites

- .NET SDK 8 or later

## Features

- **Generic Pagination Models**: Strongly-typed request and response models for paginated data.
- **Repository Extensions**: Extension methods for common repository patterns to easily add pagination.
- **LINQ Support**: Works seamlessly with `IQueryable<T>` and `IEnumerable<T>`.
- **Customizable**: Easily extend or override pagination logic to fit your domain.
- **Async Support**: Asynchronous pagination for EF Core and other async data sources.
- **.NET 8 and .NET 9**: Built for the latest .NET runtimes.

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).