# Arcturus.Data.Repository.Abstracts

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.Data.Repository.Abstracts.svg)](https://www.nuget.org/packages/Arcturus.Data.Repository.Abstracts) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.Data.Repository.Abstracts.svg)](https://www.nuget.org/packages/Arcturus.Data.Repository.Abstracts)

---

A brief description of what this .NET NuGet package does and its purpose.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.Data.Repository.Abstracts
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.Data.Repository.Abstracts
```

## Prerequisites

- .NET SDK 8 or later

## Features

| Feature                        | Description                                                                                          |
|---------------------------------|------------------------------------------------------------------------------------------------------|
| Generic Repository Abstraction  | Provides `IRepository<T, TKey>` for CRUD operations, querying, and projection over any entity type.  |
| Asynchronous Operations         | Supports async methods for querying, adding, updating, and deleting entities.                        |
| LINQ Expression Support         | Enables flexible querying and projection using LINQ expressions.                                     |
| Specification Pattern           | Allows advanced querying via `ISpecification<T>` and `ISpecification<TEntity, TResult>`.             |
| Entity Abstraction              | Defines `IEntity<TKey>` for consistent entity identification.                                        |
| Projection and Ordering         | Supports result projection, ordering, limiting, and tracking options in queries.                     |
| Cancellation Support            | All async operations accept `CancellationToken` for cooperative cancellation.                        |

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).

