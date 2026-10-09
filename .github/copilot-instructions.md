# Arcturus Copilot Instructions

## Big picture
Arcturus is a modular .NET package set. Use package READMEs, sample projects, and `.csproj` metadata as the source of truth. The repo targets .NET 10 by default; analyzers/source generators often target .NET Standard 2.0.

## Architecture and package map
- Abstractions first: `Arcturus.Mediation.Abstracts`, `Arcturus.EventBus.Abstracts`, and `Arcturus.Repository.Abstracts` define contracts; the runtime packages implement them.
- ASP.NET Core layers are thin integrations over core packages: `Arcturus.Extensions.ResultObjects.AspNetCore`, `Arcturus.Extensions.Patchable.AspNetCore`, `Arcturus.Extensions.Validation.AspNetCore`.
- Generators ship as analyzers: `Arcturus.SmartEnums.CodeGenerator` → `Arcturus.SmartEnums`, `Arcturus.Validation.CodeGenerator` → `Arcturus.Extensions.Validation.AspNetCore`, `Arcturus.DevHost.SourceGenerator` → `Arcturus.DevHost.Hosting` / `Sdk`.
- DevHost docs live under `src/Arcturus.DevHost/*/docs/README.md`, not package root READMEs.
- Root `README.md` is the package index; package READMEs should stay focused on purpose, install, and minimal usage.
## Key files
- `README.md` and `AGENTS.md` — package catalog plus repo-wide AI guidance.
- `src/Directory.Build.props` — shared TFM, nullable, SourceLink, and NuGet defaults.
- `src/*/*.csproj`, `samples/*`, and `src/*Sample` — authoritative package metadata plus executable examples.

## Dependency map
```text
ResultObjects -> Extensions.ResultObjects.AspNetCore
Mediation.Abstracts -> Mediation
Patchable -> Extensions.Patchable.AspNetCore
Repository.Abstracts -> SqlServer / InMemory / PostgresSql / NamingConvention / Json / Pagination
EventBus.Abstracts -> EventBus -> RabbitMQ / AzureServiceBus / AzureStorageQueue / Sqlite / OpenTelemetry
SmartEnums.CodeGenerator -> SmartEnums
Validation.CodeGenerator -> Extensions.Validation.AspNetCore
DevHost.SourceGenerator -> DevHost.Hosting / DevHost.Sdk
```
## Workflow notes
- Build the repo with `dotnet build Arcturus.sln`; build the affected `.csproj` directly for package-specific changes.
- There are no dedicated test projects in the solution; use sample projects as smoke tests.

## Conventions
- Use published `PackageId` names, not folder names, when they differ (`Arcturus.Repository.Abstracts` lives in `src/Arcturus.Data.Repository.Abstracts`).
- Keep examples short and working; verify against source or samples instead of inventing APIs.
- Use the repo’s canonical terms: `Result` / `Result<T>`, `ProblemDetails`, `OpenTelemetry`, `Entity Framework Core`.
- ASP.NET Core packages commonly reference `Microsoft.AspNetCore.App`, while analyzers/generators usually pack under `analyzers/dotnet/cs`.

## Do not use
- `Arcturus.ResultObjects` for exception-driven control flow.
- `Arcturus.Mediation` as an event bus or transport layer.
- `Arcturus.Patchable` for full object replacement.
- `Arcturus.Repository.EntityFrameworkCore.*` without `Arcturus.Repository.Abstracts` when abstraction is required.
- `Arcturus.EventBus` for local request/response mediation.
- `Arcturus.Xunit` outside test projects.
- `Arcturus.DevHost.*` for production hosting.

## Versioning and target frameworks
- Repo defaults in `src/Directory.Build.props`: `net10.0`, `ImplicitUsings=enable`, `Nullable=enable`, `LangVersion=latest`, `PackageReadmeFile=README.md`.
- Some packages intentionally target `netstandard2.0` for analyzer/generator compatibility.
- Verify each package `.csproj` before documenting framework support or installation guidance.
