using Arcturus.EventBus;
using Arcturus.EventBus.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Arcturus.EventBus.Examples;

/// <summary>
/// This example demonstrates the exact API pattern requested:
/// - Arcturus.EventBus registers serializers, default options, etc.
/// - Arcturus.EventBus.RabbitMQ registers the connection and implementation
/// - Client applications reference Arcturus.EventBus.RabbitMQ (which inherits Arcturus.EventBus)
/// - Use fluent builder pattern with AddEventBus() and AddRabbitMQ()
/// </summary>
public class SpecificExample
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // This is the EXACT pattern you requested
        builder.Services.AddEventBus(builder =>
        {
            // Generic EventBusOptions
            builder.UseEventHandlersProcessor = true;

            // Add RabbitMQ with specific options
            builder.AddRabbitMQ(rmq =>
            {
                rmq.ConnectionString = "amqp://guest:guest@localhost:5672/";
                rmq.ClientName = "MyRabbitMQClient";
                // Additional RabbitMQ-specific options
                rmq.RegisterHandlersFromAssemblyOf<SpecificExample>();
            });
        });

        var host = builder.Build();
        host.Run();
    }

    /// <summary>
    /// Expanded example with all options
    /// </summary>
    public static void FullExample()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddEventBus(eventBusBuilder =>
        {
            // Generic EventBusOptions (from Arcturus.EventBus)
            eventBusBuilder.ApplicationId = "MyApplication";
            eventBusBuilder.ClientId = "client-001";
            eventBusBuilder.UseEventHandlersProcessor = true;
            eventBusBuilder.DefaultQueueName = "my-default-queue";

            // RabbitMQ-specific configuration (from Arcturus.EventBus.RabbitMQ)
            eventBusBuilder.AddRabbitMQ(rmq =>
            {
                // RabbitMQ connection
                rmq.ConnectionString = "amqp://guest:guest@localhost:5672/";
                
                // RabbitMQ-specific options
                rmq.ClientName = "MyRabbitMQClient";
                
                // Register handler assemblies
                rmq.RegisterHandlersFromAssemblyOf<SpecificExample>();
            });
        });

        var host = builder.Build();
        host.Run();
    }

    /// <summary>
    /// Alternative with SQLite provider
    /// </summary>
    public static void SqliteExample()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddEventBus(eventBusBuilder =>
        {
            // Generic EventBusOptions
            eventBusBuilder.UseEventHandlersProcessor = true;
            eventBusBuilder.ApplicationId = "MyApp";

            // SQLite-specific configuration (from Arcturus.EventBus.Sqlite)
            eventBusBuilder.AddSqlite(sqlite =>
            {
                sqlite.ConnectionString = "Data Source=eventbus.db";
                sqlite.ClientName = "MySqliteClient";
            });
        });

        var host = builder.Build();
        host.Run();
    }

    /// <summary>
    /// Environment-based provider selection
    /// </summary>
    public static void ConditionalProviderExample()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddEventBus(eventBusBuilder =>
        {
            // Common configuration for all providers
            eventBusBuilder.ApplicationId = "MyApplication";
            eventBusBuilder.UseEventHandlersProcessor = true;

            // Choose provider based on environment
            if (builder.Environment.IsDevelopment())
            {
                // Use SQLite for local development
                eventBusBuilder.AddSqlite(sqlite =>
                {
                    sqlite.ConnectionString = "Data Source=dev-eventbus.db";
                });
            }
            else
            {
                // Use RabbitMQ for production
                eventBusBuilder.AddRabbitMQ(rmq =>
                {
                    rmq.ConnectionString = builder.Configuration["RabbitMQ:ConnectionString"]!;
                    rmq.ClientName = "ProductionClient";
                    rmq.RegisterHandlersFromAssemblyOf<SpecificExample>();
                });
            }
        });

        var host = builder.Build();
        host.Run();
    }
}
