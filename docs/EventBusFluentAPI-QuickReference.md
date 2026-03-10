# EventBus Fluent API - Quick Reference

## Installation

```bash
# For RabbitMQ
dotnet add package Arcturus.EventBus.RabbitMQ

# For SQLite
dotnet add package Arcturus.EventBus.Sqlite

# Core package is automatically included as a dependency
```

## Basic Pattern

```csharp
builder.Services.AddEventBus(builder =>
{
    // Core options (applies to all providers)
    builder.UseEventHandlersProcessor = true;

    // Provider-specific configuration
    builder.AddProviderName(options =>
    {
        // Provider-specific options
    });
});
```

## Core EventBus Options

| Property | Type | Description |
|----------|------|-------------|
| `ApplicationId` | `string?` | Application identifier |
| `ClientId` | `string?` | Client identifier (logging) |
| `UseEventHandlersProcessor` | `bool` | Enable middleware pipeline |
| `DefaultQueueName` | `string?` | Default queue name |

## RabbitMQ Provider

### Package
```xml
<PackageReference Include="Arcturus.EventBus.RabbitMQ" />
```

### Configuration
```csharp
builder.Services.AddEventBus(b =>
{
    b.UseEventHandlersProcessor = true;
    
    b.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = "amqp://localhost";
        rmq.ClientName = "MyClient";
        rmq.RegisterHandlersFromAssemblyOf<Program>();
    });
});
```

### RabbitMQ Options
- `ConnectionString` - AMQP connection string
- `ClientName` - RabbitMQ client name
- `RegisterHandlersFromAssemblyOf<T>()` - Register handlers

## SQLite Provider

### Package
```xml
<PackageReference Include="Arcturus.EventBus.Sqlite" />
```

### Configuration
```csharp
builder.Services.AddEventBus(b =>
{
    b.UseEventHandlersProcessor = true;
    
    b.AddSqlite(sqlite =>
    {
        sqlite.ConnectionString = "Data Source=eventbus.db";
        sqlite.ClientName = "MyClient";
    });
});
```

### SQLite Options
- `ConnectionString` - SQLite connection string
- `DatabasePath` - Path to database file
- `ClientName` - Client name
- `HostName` - Host name

## Complete Examples

### Minimal
```csharp
services.AddEventBus(b => b.AddRabbitMQ(rmq => 
    rmq.ConnectionString = "amqp://localhost"));
```

### Standard
```csharp
services.AddEventBus(b =>
{
    b.ApplicationId = "MyApp";
    b.UseEventHandlersProcessor = true;
    
    b.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = "amqp://localhost";
        rmq.RegisterHandlersFromAssemblyOf<Program>();
    });
});
```

### Full
```csharp
services.AddEventBus(b =>
{
    b.ApplicationId = "OrderService";
    b.ClientId = "order-001";
    b.UseEventHandlersProcessor = true;
    b.DefaultQueueName = "orders";
    
    b.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = config["RabbitMQ:ConnectionString"];
        rmq.ClientName = "OrderProcessor";
        rmq.RegisterHandlersFromAssemblyOf<Program>();
        rmq.RegisterHandlersFromAssemblyOf<OrderEvent>();
    });
});
```

## Common Patterns

### Configuration-Based
```csharp
services.AddEventBus(b =>
{
    b.ApplicationId = config["EventBus:AppId"];
    b.UseEventHandlersProcessor = true;
    
    b.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = config["RabbitMQ:Connection"];
        rmq.ClientName = config["RabbitMQ:ClientName"];
        rmq.RegisterHandlersFromAssemblyOf<Program>();
    });
});
```

### Environment-Based
```csharp
services.AddEventBus(b =>
{
    b.UseEventHandlersProcessor = true;
    
    if (env.IsDevelopment())
        b.AddSqlite(s => s.ConnectionString = "Data Source=dev.db");
    else
        b.AddRabbitMQ(r => r.ConnectionString = config["RabbitMQ:Prod"]);
});
```

### Multiple Assemblies
```csharp
services.AddEventBus(b =>
{
    b.UseEventHandlersProcessor = true;
    
    b.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = "amqp://localhost";
        rmq.RegisterHandlersFromAssemblyOf<Program>();
        rmq.RegisterHandlersFromAssemblyOf<OrderEvent>();
        rmq.RegisterHandlersFromAssemblyOf<CustomerEvent>();
    });
});
```

## Migration from Old API

### Before
```csharp
services.AddRabbitMQEventBus(o =>
{
    o.ConnectionString = "amqp://localhost";
    o.UseEventHandlersProcessor = true;
});
```

### After
```csharp
services.AddEventBus(b =>
{
    b.UseEventHandlersProcessor = true;
    b.AddRabbitMQ(r => r.ConnectionString = "amqp://localhost");
});
```

## Key Points

✅ **One Entry Point** - Always start with `AddEventBus()`  
✅ **Core First** - Set core options on builder  
✅ **Provider Last** - Chain provider with `Add{Provider}()`  
✅ **Type Safe** - Strongly-typed at each level  
✅ **Fluent** - Readable, chainable API  

## Documentation

- **General Guide**: `docs/EventBusFluentAPI.md`
- **RabbitMQ Examples**: `docs/EventBusFluentAPI-RabbitMQ.md`
- **Specific Example**: `docs/SpecificExample.cs`
- **Summary**: `docs/EventBusRestructure-Summary.md`
