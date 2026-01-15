# Arcturus.Repository.EntityFrameworkCore.NamingConvention

[![NuGet](https://img.shields.io/nuget/dt/Arcturus.Repository.EntityFrameworkCore.NamingConvention.svg)](https://www.nuget.org/packages/Arcturus.Repository.EntityFrameworkCore.NamingConvention) 
[![NuGet](https://img.shields.io/nuget/vpre/Arcturus.Repository.EntityFrameworkCore.NamingConvention.svg)](https://www.nuget.org/packages/Arcturus.Repository.EntityFrameworkCore.NamingConvention)

---

Arcturus.Repository.EntityFrameworkCore.NamingConvention is a .NET library that provides automatic naming convention transformations for Entity Framework Core database objects. It enables developers to apply consistent naming standards to tables, columns, keys, indexes, and constraints, ensuring compatibility with database conventions such as snake_case for PostgreSQL or other organizational standards.

## Installation

Install the package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Arcturus.Repository.EntityFrameworkCore.NamingConvention
```

Or, using the Package Manager Console:

```powershell
Install-Package Arcturus.Repository.EntityFrameworkCore.NamingConvention
```

## Prerequisites

- .NET 9 or later
- Entity Framework Core 9.0 or later

## Features

- **Automatic Name Rewriting**: Transforms all database identifiers (tables, columns, keys, indexes, constraints) according to the selected naming convention.
- **Multiple Naming Strategies**:
  - **SnakeCase**: Converts names to snake_case format (e.g., `UserAccount` ? `user_account`)
  - **LowerCase**: Converts names to lowercase format (e.g., `UserAccount` ? `useraccount`)
  - **UpperCase**: Converts names to uppercase format (e.g., `UserAccount` ? `USERACCOUNT`)
  - **UpperSnakeCase**: Converts names to upper snake_case format (e.g., `UserAccount` ? `USER_ACCOUNT`)
  - **CamelCase**: Converts names to camelCase format (e.g., `UserAccount` ? `userAccount`)
- **Culture-Aware Transformations**: Support for culture-specific case conversions.
- **Comprehensive Coverage**: Handles all EF Core database objects including:
  - Entity tables and views
  - Properties and columns
  - Primary keys and foreign keys
  - Indexes and constraints
  - Complex types and JSON columns
  - Owned entities and table splitting scenarios
- **TPH/TPT/TPC Support**: Works seamlessly with all Entity Framework Core inheritance mapping strategies (Table-Per-Hierarchy, Table-Per-Type, Table-Per-Concrete-Type).
- **Convention-Based**: Integrates with EF Core's convention system for automatic application during model building.
- **Extensible**: Implement custom naming strategies using the `INamingStrategy` interface.
- **.NET 9 & .NET 10 Compatible**: Optimized for modern .NET platforms with support for the latest EF Core features.

## Usage

### Basic Configuration

Configure the naming convention in your `DbContext` options:

```csharp
using Arcturus.Repository.EntityFrameworkCore.NamingConvention;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql("your-connection-string")
            .UseNamingConvention(NamingConvention.SnakeCase);
    }
}
```

### Using with Dependency Injection

In your application startup or service configuration:

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
{
    options
        .UseNpgsql(connectionString)
        .UseNamingConvention(NamingConvention.SnakeCase);
});
```

### Specify Culture

You can specify a culture for case conversions:

```csharp
using System.Globalization;

optionsBuilder
    .UseNpgsql(connectionString)
    .UseNamingConvention(NamingConvention.SnakeCase, CultureInfo.InvariantCulture);
```

### Available Naming Conventions

```csharp
// Snake case: user_account, order_detail
.UseNamingConvention(NamingConvention.SnakeCase)

// Lower case: useraccount, orderdetail
.UseNamingConvention(NamingConvention.LowerCase)

// Upper case: USERACCOUNT, ORDERDETAIL
.UseNamingConvention(NamingConvention.UpperCase)

// Upper snake case: USER_ACCOUNT, ORDER_DETAIL
.UseNamingConvention(NamingConvention.UpperSnakeCase)

// Camel case: userAccount, orderDetail
.UseNamingConvention(NamingConvention.CamelCase)
```

## Examples

### Example 1: PostgreSQL with Snake Case

PostgreSQL typically uses snake_case for identifiers:

```csharp
public class User
{
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; }
}

// With SnakeCase naming convention, the table and columns are mapped as:
// Table: "user"
// Columns: "user_id", "first_name", "last_name", "created_at"
```

### Example 2: Complex Types and JSON

The naming convention applies to complex types and JSON columns:

```csharp
public class Order
{
    public int OrderId { get; set; }
    public Address ShippingAddress { get; set; } // Complex type
}

public class Address
{
    public string StreetName { get; set; }
    public string CityName { get; set; }
}

// With SnakeCase, JSON column mapping becomes:
// Table: "order"
// Columns: "order_id", "shipping_address" (JSON column)
// JSON properties: "street_name", "city_name"
```

### Example 3: Custom Naming Strategy

Implement your own naming strategy:

```csharp
using Arcturus.Repository.EntityFrameworkCore.NamingConvention.Abstracts;

public class CustomNamingStrategy : INamingStrategy
{
    public string ApplyNaming(string name)
    {
        // Your custom transformation logic
        return "tbl_" + name.ToLowerInvariant();
    }
}
```

## Known Limitations

- **TPC Hierarchies**: Index and foreign key names on parent types in TPC (Table-Per-Concrete-Type) hierarchies cannot be rewritten individually per child table due to EF Core limitations. The convention clears these names to allow EF to generate unique names per table.
- **TPT Primary Keys**: Primary key naming is not fully supported in TPT (Table-Per-Type) hierarchies due to EF Core limitations (see [dotnet/efcore#23444](https://github.com/dotnet/efcore/issues/23444)).

## Compatibility Notes

### .NET 9 vs .NET 10

The library includes specific handling for differences between EF Core 9 and EF Core 10:

- **EF Core 9**: Uses direct annotation setting for complex type container columns
- **EF Core 10**: Uses builder pattern methods for setting container column names

The package automatically detects the target framework and applies the appropriate implementation.

## Documentation

For detailed documentation, visit [Arcturus Wiki](https://github.com/cloudfy/Arcturus/wiki).

## License

This project is licensed under the [MIT License](LICENSE) - see the [LICENSE](LICENSE) file for details.

## Support

If you encounter issues or have questions, please file an issue on the [GitHub Issues page](https://github.com/cloudfy/Arcturus/issues).

## Related Packages

- [Arcturus.Repository.EntityFrameworkCore](https://www.nuget.org/packages/Arcturus.Repository.EntityFrameworkCore) - Generic repository pattern for EF Core
- [Arcturus.Repository.EntityFrameworkCore.PostgresSql](https://www.nuget.org/packages/Arcturus.Repository.EntityFrameworkCore.PostgresSql) - PostgreSQL-specific extensions for EF Core