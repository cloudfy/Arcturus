# Arcturus AI Instructions

## Repository overview
Arcturus is a modular set of .NET packages targeting .NET 10 and .NET Standard 2.0.

Prefer package README files, sample projects, and `.csproj` metadata as the source of truth for package behavior, names, installation details, dependency relationships, and target frameworks.

## Package map

### Core result handling
- `Arcturus.ResultObjects` — explicit success/failure modeling without exceptions for control flow.
- `Arcturus.Extensions.ResultObjects.AspNetCore` — maps result objects to ASP.NET Core responses and `ProblemDetails`.

### Mediation and CQRS
- `Arcturus.Mediation.Abstracts` — mediator, request/response, notification, and middleware abstractions.
- `Arcturus.Mediation` — runtime mediation and CQRS implementation with middleware and event publishing.

### Patch and endpoint utilities
- `Arcturus.Patchable` — type-safe partial update and patch support for domain models.
- `Arcturus.Extensions.Patchable.AspNetCore` — ASP.NET Core helpers for PATCH endpoints and JSON Patch workflows.
- `Arcturus.AspNetCore.Endpoints` — endpoint-building abstractions for ASP.NET Core APIs.
- `Arcturus.Extensions.Validation.AspNetCore` — source-generated validation for ASP.NET Core Minimal APIs.

### Repository and data access
- `Arcturus.Repository.Abstracts` — repository and specification abstractions for data access layers.
- `Arcturus.Repository.EntityFrameworkCore.SqlServer` — Entity Framework Core repository implementation for SQL Server.
- `Arcturus.Repository.EntityFrameworkCore.InMemory` — Entity Framework Core repository implementation for in-memory persistence.
- `Arcturus.Repository.EntityFrameworkCore.PostgresSql` — Entity Framework Core repository implementation for PostgreSQL.
- `Arcturus.Repository.EntityFrameworkCore.NamingConvention` — applies consistent naming conventions to EF Core database objects.
- `Arcturus.Extensions.Repository.Json` — stores JSON-backed values and owned objects with Entity Framework Core.
- `Arcturus.Extensions.Repository.Pagination` — adds pagination support for repository queries and responses.

### Caching and configuration
- `Arcturus.Extensions.Caching` — shared extensions and abstractions for in-memory and distributed caching.
- `Arcturus.Extensions.Caching.AzureStorageTable` — distributed caching backed by Azure Storage Tables.
- `Arcturus.Extensions.Configuration.AzureStorageBlob` — loads configuration from Azure Storage Blob containers.

### Event bus and messaging
- `Arcturus.EventBus.Abstracts` — event bus contracts and base types for publish/subscribe scenarios.
- `Arcturus.EventBus` — core event bus runtime for publishing and handling integration events.
- `Arcturus.EventBus.RabbitMQ` — RabbitMQ transport for the Arcturus event bus.
- `Arcturus.EventBus.AzureServiceBus` — Azure Service Bus transport for the Arcturus event bus.
- `Arcturus.EventBus.AzureStorageQueue` — Azure Storage Queue transport for the Arcturus event bus.
- `Arcturus.EventBus.Sqlite` — SQLite-backed event bus implementation for local or persistent messaging.
- `Arcturus.EventBus.OpenTelemetry` — OpenTelemetry instrumentation for event publishing and handling.

### Smart enums, analyzers, and generators
- `Arcturus.SmartEnums` — type-safe string-backed smart enums with JSON serialization support.
- `Arcturus.SmartEnums.CodeGenerator` — source generator that emits smart enum members and supporting code.
- `Arcturus.Validation.CodeGenerator` — source generator for validation code generation.
- `Arcturus.CodeAnalysis.CSharp` — Roslyn analyzers that enforce safe development practices.

### Command-line and testing utilities
- `Arcturus.Extensions.CommandLine` — CLI helpers for commands, options, arguments, and help text.
- `Arcturus.Xunit` — xUnit helpers and dependency-injection support for tests.

### DevHost
- `Arcturus.DevHost.Sdk` — MSBuild SDK for orchestrating multiple projects and executables in a local development host.
- `Arcturus.DevHost.Hosting` — runtime hosting layer for coordinated local orchestration.
- `Arcturus.DevHost.SourceGenerator` — source generator that creates strongly typed project metadata.

## Dependency graph

Use this as the high-level package dependency map when reasoning about package interactions.

```text
Arcturus.ResultObjects
└─ Arcturus.Extensions.ResultObjects.AspNetCore

Arcturus.Mediation.Abstracts
└─ Arcturus.Mediation

Arcturus.Patchable
└─ Arcturus.Extensions.Patchable.AspNetCore

Arcturus.Repository.Abstracts
├─ Arcturus.Repository.EntityFrameworkCore.SqlServer
├─ Arcturus.Repository.EntityFrameworkCore.InMemory
├─ Arcturus.Repository.EntityFrameworkCore.PostgresSql
├─ Arcturus.Repository.EntityFrameworkCore.NamingConvention
├─ Arcturus.Extensions.Repository.Json
└─ Arcturus.Extensions.Repository.Pagination

Arcturus.EventBus.Abstracts
└─ Arcturus.EventBus
   ├─ Arcturus.EventBus.RabbitMQ
   ├─ Arcturus.EventBus.AzureServiceBus
   ├─ Arcturus.EventBus.AzureStorageQueue
   ├─ Arcturus.EventBus.Sqlite
   └─ Arcturus.EventBus.OpenTelemetry

Arcturus.SmartEnums.CodeGenerator
└─ Arcturus.SmartEnums

Arcturus.Validation.CodeGenerator
└─ Arcturus.Extensions.Validation.AspNetCore

Arcturus.DevHost.SourceGenerator
└─ Arcturus.DevHost.Hosting / Arcturus.DevHost.Sdk
```

## Coding conventions

- Do not invent APIs; confirm against the relevant package README or source.
- Keep documentation aligned with the actual `PackageId` in each `.csproj`.
- Use package names exactly as published, including suffixes like `.AspNetCore`, `.SqlServer`, and `.CodeGenerator`.
- Prefer minimal, focused changes that match the existing style.
- Keep root `README.md` as the package index; keep package-specific `README.md` files focused on one package.
- Use short, direct examples. Prefer one installation example and one usage example per package.
- If a package README is sparse, infer usage from the source code and sample projects.
- When updating docs, keep categories, package names, and links consistent across root and package READMEs.
- Validate changes with the available build or test tools when code changes are involved.

## Sample usage patterns

Use these patterns when writing examples or docs:

### Package README template
- Purpose
- Installation
- Minimal usage
- Key features
- Link to samples or wiki if applicable

### Code sample style
- Prefer one small, working snippet.
- Show `using` statements only when needed.
- Show the smallest complete call path from registration to usage.
- Keep ASP.NET Core samples focused on one endpoint or one service registration.

### Documentation style
- Use package names, not folder names, unless folder and package name are identical.
- Show `dotnet add package ...` for installation.
- Prefer fenced code blocks with the correct language.
- Keep terminology consistent:
  - `Result` / `Result<T>`
  - `IRequest` / `IRequestHandler`
  - `ProblemDetails`
  - `OpenTelemetry`
  - `Entity Framework Core`

## Do not use this package for X

### Result handling
- Do not use `Arcturus.ResultObjects` for exception-driven control flow.

### Mediation and CQRS
- Do not use `Arcturus.Mediation` as an event bus or transport layer.

### Patch and endpoint utilities
- Do not use `Arcturus.Patchable` for full object replacement.
- Do not use `Arcturus.Extensions.Validation.AspNetCore` for runtime reflection-heavy validation if compile-time generation is required.

### Repository and data access
- Do not use repository packages as a replacement for raw SQL when the use case is SQL-specific reporting or bulk data work.
- Do not use `Arcturus.Repository.EntityFrameworkCore.*` without `Arcturus.Repository.Abstracts` when the abstraction layer is needed.

### Event bus and messaging
- Do not use `Arcturus.EventBus` for local request/response mediation.
- Do not use transport-specific packages directly when only abstraction-level integration is required.

### Smart enums, analyzers, and generators
- Do not use generators/analyzers as runtime business-logic libraries.
- Do not use `Arcturus.SmartEnums.CodeGenerator` without `Arcturus.SmartEnums` when generated runtime types are required.

### Command-line and testing
- Do not use `Arcturus.Extensions.CommandLine` for web API endpoint routing.
- Do not use `Arcturus.Xunit` outside test projects.

### DevHost
- Do not use `Arcturus.DevHost.*` for production hosting; it is for local orchestration and development workflows.

## Versioning and target framework notes

- Repository-wide defaults from `Directory.Build.props`:
  - `TargetFrameworks=net10.0`
  - `ImplicitUsings=enable`
  - `Nullable=enable`
  - `LangVersion=latest`
  - `PackageReadmeFile=README.md`
- Some packages intentionally target `netstandard2.0`, especially analyzers/source generators and compatibility packages.
- Several ASP.NET Core extension packages use `Microsoft.AspNetCore.App`/framework references instead of direct package-only dependencies.
- Always verify the effective target framework in the package `.csproj` before updating docs or examples.
- Prefer version-neutral examples unless the package specifically requires a newer TFM or framework reference.
- When adding or updating package docs, align install instructions and compatibility notes with the package’s actual `.csproj`.

## Working rules

- Root `README.md` is the package index.
- Package-level `README.md` files should describe purpose, install command, and usage.
- Samples should remain executable examples of the documented APIs.
- Keep dependencies and package relationships aligned across the root README, package READMEs, and `.csproj` metadata.
- When changing code, validate with the available build or test tools before finishing.
