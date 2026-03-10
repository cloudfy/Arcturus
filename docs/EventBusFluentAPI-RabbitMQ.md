# EventBus Fluent API - RabbitMQ Examples

This document provides specific examples of using the EventBus fluent API with RabbitMQ.

## Prerequisites

Add the NuGet package reference to your project:

```xml
<ItemGroup>
  <PackageReference Include="Arcturus.EventBus.RabbitMQ" Version="x.x.x" />
</ItemGroup>
```

The `Arcturus.EventBus` package is automatically included as a dependency.

## Basic Example

```csharp
using Arcturus.EventBus;
using Arcturus.EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddEventBus(eventBus =>
{
    // Core EventBus configuration
    eventBus.UseEventHandlersProcessor = true;

    // Add RabbitMQ provider
    eventBus.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = "amqp://guest:guest@localhost:5672/";
        rmq.RegisterHandlersFromAssemblyOf<Program>();
    });
});

var host = builder.Build();
await host.RunAsync();
```

## Full Configuration Example

```csharp
builder.Services.AddEventBus(eventBus =>
{
    // Configure all core EventBus options
    eventBus.ApplicationId = "OrderProcessingService";
    eventBus.ClientId = "order-processor-01";
    eventBus.UseEventHandlersProcessor = true;
    eventBus.DefaultQueueName = "orders";

    // Configure RabbitMQ with all options
    eventBus.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = "amqp://user:password@rabbitmq.example.com:5672/production";
        rmq.ClientName = "OrderProcessor";
        
        // Register event handlers from multiple assemblies
        rmq.RegisterHandlersFromAssemblyOf<Program>();
        rmq.RegisterHandlersFromAssemblyOf<OrderCreatedEvent>();
    });
});
```

## Configuration from appsettings.json

```csharp
// appsettings.json
{
  "EventBus": {
    "ApplicationId": "OrderService",
    "UseEventHandlersProcessor": true,
    "DefaultQueueName": "orders"
  },
  "RabbitMQ": {
    "ConnectionString": "amqp://guest:guest@localhost:5672/"
  }
}

// Program.cs
builder.Services.AddEventBus(eventBus =>
{
    eventBus.ApplicationId = builder.Configuration["EventBus:ApplicationId"];
    eventBus.UseEventHandlersProcessor = 
        bool.Parse(builder.Configuration["EventBus:UseEventHandlersProcessor"] ?? "true");
    eventBus.DefaultQueueName = builder.Configuration["EventBus:DefaultQueueName"];

    eventBus.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = builder.Configuration["RabbitMQ:ConnectionString"]!;
        rmq.ClientName = $"Client-{Environment.MachineName}";
        rmq.RegisterHandlersFromAssemblyOf<Program>();
    });
});
```

## Environment-Based Configuration

```csharp
builder.Services.AddEventBus(eventBus =>
{
    var environment = builder.Environment.EnvironmentName;
    
    eventBus.ApplicationId = $"OrderService-{environment}";
    eventBus.UseEventHandlersProcessor = true;
    
    eventBus.AddRabbitMQ(rmq =>
    {
        // Different connection strings based on environment
        rmq.ConnectionString = environment switch
        {
            "Development" => "amqp://localhost",
            "Staging" => builder.Configuration["RabbitMQ:Staging:ConnectionString"]!,
            "Production" => builder.Configuration["RabbitMQ:Production:ConnectionString"]!,
            _ => "amqp://localhost"
        };
        
        rmq.ClientName = $"{environment}-Client";
        rmq.RegisterHandlersFromAssemblyOf<Program>();
    });
});
```

## Multiple Handler Assemblies

```csharp
builder.Services.AddEventBus(eventBus =>
{
    eventBus.UseEventHandlersProcessor = true;

    eventBus.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = "amqp://localhost";
        
        // Register handlers from multiple assemblies
        rmq.RegisterHandlersFromAssemblyOf<Program>();           // Current assembly
        rmq.RegisterHandlersFromAssemblyOf<OrderCreatedEvent>(); // Order events assembly
        rmq.RegisterHandlersFromAssemblyOf<CustomerService>();   // Customer service assembly
    });
});
```

## Using with Event Middleware

```csharp
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddEventBus(eventBus =>
{
    eventBus.ApplicationId = "OrderService";
    eventBus.UseEventHandlersProcessor = true; // Required for middleware

    eventBus.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = "amqp://localhost";
        rmq.RegisterHandlersFromAssemblyOf<Program>();
    });
});

var host = builder.Build();

// Register middleware
host.UseEventMiddleware<LoggingMiddleware>();
host.UseEventMiddleware<ValidationMiddleware>();
host.UseEventMiddleware<RetryMiddleware>();

await host.RunAsync();
```

## Migration from Old API

### Before (Old API)

```csharp
services.AddRabbitMQEventBus(options =>
{
    options.ApplicationId = "MyApp";
    options.ClientId = "client-001";
    options.ConnectionString = "amqp://localhost";
    options.UseEventHandlersProcessor = true;
    options.DefaultQueueName = "my-queue";
    options.ClientName = "MyClient";
    options.RegisterHandlersFromAssemblyOf<Program>();
});
```

### After (New Fluent API)

```csharp
services.AddEventBus(builder =>
{
    // Core EventBus options
    builder.ApplicationId = "MyApp";
    builder.ClientId = "client-001";
    builder.UseEventHandlersProcessor = true;
    builder.DefaultQueueName = "my-queue";

    // RabbitMQ-specific options
    builder.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = "amqp://localhost";
        rmq.ClientName = "MyClient";
        rmq.RegisterHandlersFromAssemblyOf<Program>();
    });
});
```

## Complete Working Example

```csharp
using Arcturus.EventBus;
using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Configure EventBus with RabbitMQ
builder.Services.AddEventBus(eventBus =>
{
    eventBus.ApplicationId = "OrderProcessingService";
    eventBus.UseEventHandlersProcessor = true;
    eventBus.DefaultQueueName = "orders";

    eventBus.AddRabbitMQ(rmq =>
    {
        rmq.ConnectionString = "amqp://guest:guest@localhost:5672/";
        rmq.ClientName = "OrderProcessor";
        rmq.RegisterHandlersFromAssemblyOf<Program>();
    });
});

var host = builder.Build();

// Get EventBus factory
var factory = host.Services.GetRequiredService<IEventBusFactory>();

// Create publisher and processor
var publisher = factory.CreatePublisher();
var processor = factory.CreateProcessor();

// Start processing
_ = Task.Run(async () =>
{
    await processor.StartAsync(CancellationToken.None);
});

// Publish an event
await publisher.PublishAsync(new OrderCreatedEvent
{
    OrderId = Guid.NewGuid(),
    CustomerName = "John Doe",
    Amount = 99.99m
});

await host.RunAsync();

// Event definition
public class OrderCreatedEvent : IEventMessage
{
    public Guid OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
```

## Connection String Formats

RabbitMQ supports AMQP connection strings:

- Local development: `amqp://localhost`
- With credentials: `amqp://username:password@hostname:5672/`
- With virtual host: `amqp://user:pass@host:5672/vhost`
- Secure connection: `amqps://user:pass@host:5671/`
- With parameters: `amqp://host?heartbeat=30&connection_timeout=10`

## Best Practices

1. **Store connection strings in configuration**, not in code
2. **Use different client names** for different services
3. **Register all handler assemblies** at startup
4. **Enable UseEventHandlersProcessor** for middleware support
5. **Set meaningful ApplicationId** for monitoring and debugging
6. **Use environment-based configuration** for different deployment environments
