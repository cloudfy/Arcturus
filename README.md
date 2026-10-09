# Arcturus

[![Version](https://img.shields.io/github/v/release/cloudfy/Arcturus)](https://github.com/cloudfy/Arcturus/releases)
[![NuGet](https://img.shields.io/badge/NuGet-packages-blue?logo=nuget)](https://www.nuget.org/packages?q=Arcturus)
![NuGet Version](https://img.shields.io/nuget/v/Arcturus.Mediation.Abstracts) [![License](https://img.shields.io/github/license/cloudfy/Arcturus)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10%20%7C%20Standard%202.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)

Arcturus is a modular set of .NET packages for building modern cloud, distributed, and API-driven applications. The repository is split into focused packages so you can reference only the functionality you need.

## Package catalog

### Core result handling
- [Arcturus.ResultObjects](src/Arcturus.ResultObjects/README.md) — Represents success and failure without relying on exceptions for control flow.
- [Arcturus.Extensions.ResultObjects.AspNetCore](src/Arcturus.Extensions.ResultObjects.AspNetCore/README.md) — Maps result objects to ASP.NET Core responses and ProblemDetails.

### Mediation and CQRS
- [Arcturus.Mediation.Abstracts](src/Arcturus.Mediation.Abstracts/README.md) — Core mediator, request/response, notification, and middleware abstractions.
- [Arcturus.Mediation](src/Arcturus.Mediation/README.md) — Runtime mediation and CQRS implementation with middleware and event publishing.

### Patch and endpoint utilities
- [Arcturus.Patchable](src/Arcturus.Patchable/README.md) — Type-safe partial-update and patch support for domain models.
- [Arcturus.Extensions.Patchable.AspNetCore](src/Arcturus.Extensions.Patchable.AspNetCore/README.md) — ASP.NET Core helpers for PATCH endpoints and JSON Patch workflows.
- [Arcturus.AspNetCore.Endpoints](src/Arcturus.AspNetCore.Endpoints/README.md) — Endpoint-building abstractions for ASP.NET Core APIs.
- [Arcturus.Extensions.Validation.AspNetCore](src/Arcturus.Extensions.Validation.AspNetCore/README.md) — Source-generated validation for ASP.NET Core Minimal APIs.

### Repository and data access
- [Arcturus.Repository.Abstracts](src/Arcturus.Data.Repository.Abstracts/README.md) — Repository and specification abstractions for data access layers.
- [Arcturus.Repository.EntityFrameworkCore.SqlServer](src/Arcturus.Data.Repository.EntityFrameworkCore/README.md) — Entity Framework Core repository implementation for SQL Server.
- [Arcturus.Repository.EntityFrameworkCore.InMemory](src/Arcturus.Repository.EntityFrameworkCore.InMemory/README.md) — Entity Framework Core repository implementation for in-memory persistence.
- [Arcturus.Repository.EntityFrameworkCore.PostgresSql](src/Arcturus.Repository.EntityFrameworkCore.PostgresSql/README.md) — Entity Framework Core repository implementation for PostgreSQL.
- [Arcturus.Repository.EntityFrameworkCore.NamingConvention](src/Arcturus.Repository.EntityFrameworkCore.NamingConvention/README.md) — Applies consistent naming conventions to EF Core database objects.
- [Arcturus.Extensions.Repository.Json](src/Arcturus.Extensions.Repository.Json/README.md) — Stores JSON-backed values and owned objects with Entity Framework Core.
- [Arcturus.Extensions.Repository.Pagination](src/Arcturus.Extensions.Repository.Pagination/README.md) — Adds pagination support for repository queries and responses.

### Caching and configuration
- [Arcturus.Extensions.Caching](src/Arcturus.Extensions.Caching/README.md) — Shared extensions and abstractions for in-memory and distributed caching.
- [Arcturus.Extensions.Caching.AzureStorageTable](src/Arcturus.Extensions.Caching.AzureStorageTable/README.md) — Distributed caching backed by Azure Storage Tables.
- [Arcturus.Extensions.Configuration.AzureStorageBlob](src/Arcturus.Extensions.Configuration.AzureStorageBlob/README.md) — Loads configuration from Azure Storage Blob containers.

### Event bus and messaging
- [Arcturus.EventBus.Abstracts](src/Arcturus.EventBus.Abstracts/README.md) — Event bus contracts and base types for publish/subscribe scenarios.
- [Arcturus.EventBus](src/Arcturus.EventBus/README.md) — Core event bus runtime for publishing and handling integration events.
- [Arcturus.EventBus.RabbitMQ](src/Arcturus.EventBus.RabbitMQ/README.md) — RabbitMQ transport for the Arcturus event bus.
- [Arcturus.EventBus.AzureServiceBus](src/Arcturus.EventBus.AzureServiceBus/README.md) — Azure Service Bus transport for the Arcturus event bus.
- [Arcturus.EventBus.AzureStorageQueue](src/Arcturus.EventBus.AzureStorageQueue/README.md) — Azure Storage Queue transport for the Arcturus event bus.
- [Arcturus.EventBus.Sqlite](src/Arcturus.EventBus.Sqlite/README.md) — SQLite-backed event bus implementation for local or persistent messaging.
- [Arcturus.EventBus.OpenTelemetry](src/Arcturus.EventBus.OpenTelemetry/README.md) — OpenTelemetry instrumentation for event publishing and handling.

### Smart enums and code generation
- [Arcturus.SmartEnums](src/Arcturus.SmartEnums/README.md) — Type-safe string-backed smart enums with JSON serialization support.
- [Arcturus.SmartEnums.CodeGenerator](src/Arcturus.SmartEnums.CodeGenerator/README.md) — Source generator that emits smart enum members and supporting code.
- [Arcturus.Validation.CodeGenerator](src/Arcturus.Validation.CodeGenerator/README.md) — Source generator used to add validation code generation.
- [Arcturus.CodeAnalysis.CSharp](src/Arcturus.CodeAnalysis.CSharp/README.md) — Roslyn analyzers that enforce safe development practices.

### Command-line and testing utilities
- [Arcturus.Extensions.CommandLine](src/Arcturus.Extensions.CommandLine/README.md) — CLI helpers for commands, options, arguments, and help text.
- [Arcturus.Xunit](src/Arcturus.Xunit/README.md) — xUnit helpers and dependency-injection support for tests.

### DevHost
- [Arcturus.DevHost.Sdk](src/Arcturus.DevHost/Arcturus.DevHost.Sdk/docs/README.md) — MSBuild SDK for orchestrating multiple projects and executables in a local development host.
- [Arcturus.DevHost.Hosting](src/Arcturus.DevHost/Arcturus.DevHost.Hosting/docs/README.md) — Runtime hosting layer for coordinated local orchestration.
- [Arcturus.DevHost.SourceGenerator](src/Arcturus.DevHost/Arcturus.DevHost.SourceGenerator/docs/README.md) — Source generator that creates strongly typed project metadata.

## Samples

Example applications are available in the `samples/` and `src/` sample projects, including command-line, event bus, mediation, and smart enum samples.

## Contributing

Contributions are welcome. Please see [CONTRIBUTING.md](CONTRIBUTING.md) for contribution guidelines.

## License

The code in this repository is licensed under the [MIT](LICENSE) license.
