# EventBus Service Collection Registration Restructure

## Overview

This implementation restructures the EventBus service registration to provide a clean, fluent API that separates core configuration from provider-specific implementation.

## Architecture

### Two-Layer Design

1. **Arcturus.EventBus (Core)**
   - Registers serializers, default options, and base services
   - Provides `EventBusBuilder` for fluent configuration
   - Defines core options: `ApplicationId`, `ClientId`, `UseEventHandlersProcessor`, `DefaultQueueName`

2. **Provider Packages (RabbitMQ, SQLite, etc.)**
   - Registers connection and implementation-specific services
   - Extends `EventBusBuilder` with provider methods
   - Inherits core options from builder

## Usage

### The Exact Pattern You Requested

```csharp
using Arcturus.EventBus;
using Arcturus.EventBus.RabbitMQ;

builder.Services.AddEventBus(builder =>
{
    // Generic EventBusOptions
    builder.UseEventHandlersProcessor = true;

    builder.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = "amqp://guest:guest@localhost:5672/";
        rmq.ClientName = "MyRabbitMQClient";
    });
});
```

### Why This Works

1. **Client apps reference only the provider package** (e.g., `Arcturus.EventBus.RabbitMQ`)
2. **Core package is automatically included** as a dependency
3. **`AddEventBus()`** is defined in the core package
4. **`AddRabbitMQ()`** is an extension method in the RabbitMQ package that extends `EventBusBuilder`
5. **Core options flow to provider** - builder properties are copied to provider options

## Implementation Files

### Core Package (Arcturus.EventBus)

| File | Purpose |
|------|---------|
| `EventBusBuilder.cs` | Builder class with core properties and IServiceCollection |
| `ServiceExtensions.cs` | `AddEventBus()` extension method |
| `EventBusOptions.cs` | Core options class (existing) |

### Provider Packages

| Package | File | Extension Method |
|---------|------|------------------|
| Arcturus.EventBus.RabbitMQ | `EventBusBuilderExtensions.cs` | `AddRabbitMQ()` |
| Arcturus.EventBus.Sqlite | `EventBusBuilderExtensions.cs` | `AddSqlite()` |

## Examples Provided

### Documentation Files

1. **`docs/EventBusFluentAPI.md`**
   - Complete guide to the fluent API
   - Architecture overview
   - Configuration options for all components
   - Migration guide from old API
   - How to create new providers

2. **`docs/EventBusFluentAPI-RabbitMQ.md`**
   - RabbitMQ-specific examples
   - Connection string formats
   - Environment-based configuration
   - Multiple handler assemblies
   - Complete working examples

3. **`docs/EventBusFluentAPI-QuickReference.md`**
   - Quick reference for all providers
   - Common patterns
   - Side-by-side comparisons

4. **`docs/SpecificExample.cs`**
   - The exact API pattern you requested
   - Multiple variations
   - Conditional provider selection

5. **`docs/EventBusRestructure-Summary.md`**
   - Implementation summary
   - File changes
   - Benefits
   - Next steps

### Code Examples

6. **`samples/Arcturus.Eventbus.Sample/FluentApiExamples.cs`**
   - Five working code examples
   - From minimal to full configuration
   - In-memory, file-based, and advanced scenarios

## Quick Start

### 1. Install Package

```bash
# For RabbitMQ
dotnet add package Arcturus.EventBus.RabbitMQ

# For SQLite
dotnet add package Arcturus.EventBus.Sqlite
```

### 2. Configure in Program.cs

```csharp
builder.Services.AddEventBus(eventBus =>
{
    eventBus.UseEventHandlersProcessor = true;
    
    eventBus.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = builder.Configuration["RabbitMQ:ConnectionString"];
        rmq.RegisterHandlersFromAssemblyOf<Program>();
    });
});
```

### 3. Use EventBus

```csharp
var factory = host.Services.GetRequiredService<IEventBusFactory>();
var publisher = factory.CreatePublisher();
await publisher.PublishAsync(new MyEvent());
```

## Benefits

✅ **Separation of Concerns** - Core vs provider configuration  
✅ **Fluent API** - Readable, chainable syntax  
✅ **Type Safety** - Strongly-typed at each level  
✅ **Extensibility** - Easy to add new providers  
✅ **Consistency** - Same pattern across all providers  
✅ **Flexibility** - Mix core and provider options naturally  

## Provider Pattern

To add a new provider:

```csharp
// In Arcturus.EventBus.NewProvider project
public static class EventBusBuilderExtensions
{
    public static EventBusBuilder AddNewProvider(
        this EventBusBuilder builder,
        Action<NewProviderOptions> configure)
    {
        var options = new NewProviderOptions
        {
            ApplicationId = builder.ApplicationId,
            ClientId = builder.ClientId,
            UseEventHandlersProcessor = builder.UseEventHandlersProcessor,
            DefaultQueueName = builder.DefaultQueueName
        };
        
        configure(options);
        
        builder.Services.AddSingleton<IConnection, NewProviderConnection>();
        builder.Services.AddSingleton<IEventBusFactory, NewProviderFactory>();
        builder.Services.AddSingleton(options);
        
        return builder;
    }
}
```

## Testing

All new files compile successfully:
- ✅ `EventBusBuilder.cs`
- ✅ `ServiceExtensions.cs`
- ✅ RabbitMQ `EventBusBuilderExtensions.cs`
- ✅ SQLite `EventBusBuilderExtensions.cs`
- ✅ Sample code

## Next Steps

1. **Implement for remaining providers**: Azure Service Bus, Azure Storage Queue
2. **Add unit tests** for the builder pattern
3. **Update samples** to use new API
4. **Consider deprecating** old `AddRabbitMQEventBus()` methods
5. **Add intellisense** XML documentation

## Summary

This implementation provides exactly what you requested:

> From the client application one would reference the Arcturus.EventBus.RabbitMQ (which inherits Arcturus.EventBus) and call:
>
> ```csharp
> builder.Services.AddEventBus((builder) => {
>   builder.UseEventHandlersProcessor = true;
>   
>   builder.AddRabbitMQ((rmq) => {
>     rmq.ConnectionString = "....";
>   });
> });
> ```

The pattern is:
1. ✅ Core package handles base registration
2. ✅ Provider package handles specific implementation
3. ✅ Fluent builder API with lambda configuration
4. ✅ Clear separation of core vs provider options
5. ✅ Extensible for new providers

All files are created and ready to use!
