# EventBus Fluent Registration API

This document describes the restructured EventBus service registration using a fluent builder pattern.

## Architecture Overview

The EventBus registration is now split into two layers:

1. **Arcturus.EventBus** - Core package that registers serializers, default options, and base services
2. **Provider Packages** - Provider-specific packages (RabbitMQ, SQLite, Azure Service Bus, etc.) that register connections and implementations

## Basic Usage

### RabbitMQ Example

```csharp
using Arcturus.EventBus;
using Arcturus.EventBus.RabbitMQ;

builder.Services.AddEventBus(eventBus =>
{
    // Configure generic EventBusOptions
    eventBus.ApplicationId = "MyApplication";
    eventBus.ClientId = "client-001";
    eventBus.UseEventHandlersProcessor = true;
    eventBus.DefaultQueueName = "my-queue";

    // Add RabbitMQ provider
    eventBus.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = "amqp://guest:guest@localhost:5672/";
        rmq.ClientName = "MyRabbitMQClient";
        rmq.RegisterHandlersFromAssemblyOf<Program>();
    });
});
```

### SQLite Example

```csharp
using Arcturus.EventBus;
using Arcturus.EventBus.Sqlite;

builder.Services.AddEventBus(eventBus =>
{
    eventBus.ApplicationId = "MyApplication";
    eventBus.UseEventHandlersProcessor = true;

    // Add SQLite provider
    eventBus.AddSqlite(sqlite =>
    {
        sqlite.ConnectionString = "Data Source=eventbus.db";
        sqlite.ClientName = "MySqliteClient";
    });
});
```

## Configuration Options

### Core EventBus Options (EventBusBuilder)

| Property | Type | Description |
|----------|------|-------------|
| `ApplicationId` | `string?` | Application identifier |
| `ClientId` | `string?` | Client identifier (used for logging) |
| `UseEventHandlersProcessor` | `bool` | Enable event middleware pipeline support |
| `DefaultQueueName` | `string?` | Default queue name if not specified elsewhere |

### RabbitMQ-Specific Options

| Property | Type | Description |
|----------|------|-------------|
| `ConnectionString` | `string?` | AMQP connection string (e.g., `amqp://localhost`) |
| `ClientName` | `string?` | RabbitMQ client name |
| `RegisterHandlersFromAssemblyOf<T>()` | method | Register event handlers from assembly containing type T |

### SQLite-Specific Options

| Property | Type | Description |
|----------|------|-------------|
| `ConnectionString` | `string?` | SQLite connection string |
| `DatabasePath` | `string?` | Path to SQLite database file |
| `ClientName` | `string?` | SQLite client name |
| `HostName` | `string?` | Host name (defaults to machine name) |

## Migration Guide

### Old API (Before)

```csharp
// Old approach - directly on IServiceCollection
services.AddRabbitMQEventBus(options =>
{
    options.ApplicationId = "MyApp";
    options.ConnectionString = "amqp://localhost";
    options.UseEventHandlersProcessor = true;
    options.RegisterHandlersFromAssemblyOf<Program>();
});
```

### New API (After)

```csharp
// New approach - fluent builder pattern
services.AddEventBus(builder =>
{
    // Core options
    builder.ApplicationId = "MyApp";
    builder.UseEventHandlersProcessor = true;

    // Provider-specific configuration
    builder.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = "amqp://localhost";
        rmq.RegisterHandlersFromAssemblyOf<Program>();
    });
});
```

## Benefits

1. **Separation of Concerns**: Core EventBus configuration is separate from provider-specific settings
2. **Fluent API**: Chain configuration calls for better readability
3. **Consistency**: All providers follow the same registration pattern
4. **Extensibility**: Easy to add new providers with their own configuration
5. **Type Safety**: Strongly-typed configuration at each level

## Creating a New Provider

To create a new EventBus provider extension:

1. Create options class inheriting from `EventBusOptions`:
```csharp
public class MyProviderEventBusOptions : EventBusOptions
{
    public string? MyProviderSpecificSetting { get; set; }
}
```

2. Create extension method on `EventBusBuilder`:
```csharp
public static class EventBusBuilderExtensions
{
    public static EventBusBuilder AddMyProvider(
        this EventBusBuilder builder,
        Action<MyProviderEventBusOptions> configure)
    {
        var options = new MyProviderEventBusOptions
        {
            ApplicationId = builder.ApplicationId,
            ClientId = builder.ClientId,
            UseEventHandlersProcessor = builder.UseEventHandlersProcessor,
            DefaultQueueName = builder.DefaultQueueName
        };
        
        configure(options);
        
        // Register provider-specific services
        builder.Services.AddSingleton<IConnection, MyProviderConnection>();
        builder.Services.AddSingleton<IEventBusFactory, MyProviderEventBusFactory>();
        builder.Services.AddSingleton(options);
        
        return builder;
    }
}
```

3. Use it:
```csharp
services.AddEventBus(builder =>
{
    builder.UseEventHandlersProcessor = true;
    builder.AddMyProvider(provider =>
    {
        provider.MyProviderSpecificSetting = "value";
    });
});
```

## Complete Example

```csharp
using Arcturus.EventBus;
using Arcturus.EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Configure EventBus with RabbitMQ
builder.Services.AddEventBus(eventBus =>
{
    // Core configuration
    eventBus.ApplicationId = "OrderProcessingService";
    eventBus.ClientId = $"client-{Environment.MachineName}";
    eventBus.UseEventHandlersProcessor = true;
    eventBus.DefaultQueueName = "orders";

    // RabbitMQ configuration
    eventBus.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = builder.Configuration["RabbitMQ:ConnectionString"]
            ?? "amqp://guest:guest@localhost:5672/";
        rmq.ClientName = "OrderProcessor";
        
        // Register event handlers
        rmq.RegisterHandlersFromAssemblyOf<Program>();
    });
});

var host = builder.Build();
await host.RunAsync();
```
