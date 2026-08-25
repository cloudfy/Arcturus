# Arcturus.Extensions.Repository.Json

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.Extensions.Repository.Json.svg)](https://www.nuget.org/packages/Arcturus.Extensions.Repository.Json) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.Extensions.Repository.Json.svg)](https://www.nuget.org/packages/Arcturus.Extensions.Repository.Json)

---

Arcturus.Extensions.Repository.Json is a .NET library that makes it easy to persist complex JSON-based values in Entity Framework Core. It provides extension methods for configuring properties, read-only collections, and custom conversions so owned objects, nested DTOs, and JSON document-like data can be stored in relational database columns without losing strong typing in the application model.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.Extensions.Repository.Json
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.Extensions.Repository.Json
```

## Prerequisites

- .NET SDK 10 or later
- Entity Framework Core relational providers that support JSON or `jsonb` columns

## Features

- **JSON Property Mapping**: Serialize and deserialize complex object properties directly to and from database columns.
- **Read-Only Collection Support**: Store `IReadOnlyCollection<T>` values as JSON while keeping a clean domain model.
- **Custom Conversion Helpers**: Configure entity properties with custom conversion logic for non-standard types.
- **EF Core Integration**: Works with `EntityTypeBuilder` configuration to keep repository mappings simple and reusable.
- **Serializer Configuration**: Centralize `System.Text.Json` options for consistent serialization behavior across the application.
- **Modern .NET Support**: Designed for current .NET and EF Core versions with strong type safety.

## Example

```csharp
modelBuilder.Entity<Order>(builder =>
{
    builder.PropertyJsonProperty(x => x.Metadata, "metadata");
    builder.PropertyReadOnlyJsonCollection(x => x.Tags, "tags");
});

services.ConfigureEFCorePropertyJsonOptions(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
```

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).
