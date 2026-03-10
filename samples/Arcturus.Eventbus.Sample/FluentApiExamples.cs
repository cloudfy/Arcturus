// Example demonstrating the new fluent EventBus registration API
using Arcturus.EventBus;
using Arcturus.EventBus.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Arcturus.EventBus.Sample;

/// <summary>
/// Examples of the fluent EventBus registration API
/// </summary>
public static class FluentApiExamples
{
    /// <summary>
    /// Example 1: Basic SQLite configuration
    /// </summary>
    public static void Example1_BasicSqlite()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddEventBus(eventBus =>
        {
            // Configure core EventBus options
            eventBus.ApplicationId = "MyApplication";
            eventBus.UseEventHandlersProcessor = true;

            // Add SQLite provider
            eventBus.AddSqlite(sqlite =>
            {
                sqlite.ConnectionString = "Data Source=eventbus.db";
                sqlite.ClientName = "SqliteClient";
            });
        });

        var host = builder.Build();
        // host.Run();
    }

    /// <summary>
    /// Example 2: Minimal configuration
    /// </summary>
    public static void Example2_Minimal()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddEventBus(eventBus =>
        {
            eventBus.UseEventHandlersProcessor = true;

            eventBus.AddSqlite(sqlite =>
            {
                sqlite.ConnectionString = "Data Source=eventbus.db";
            });
        });

        var host = builder.Build();
        // host.Run();
    }

    /// <summary>
    /// Example 3: Advanced configuration with environment variables
    /// </summary>
    public static void Example3_Advanced()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddEventBus(eventBus =>
        {
            // Core options
            eventBus.ApplicationId = Environment.GetEnvironmentVariable("APP_ID") ?? "DefaultApp";
            eventBus.ClientId = $"client-{Environment.MachineName}";
            eventBus.UseEventHandlersProcessor = true;
            eventBus.DefaultQueueName = "default";

            // SQLite configuration
            eventBus.AddSqlite(sqlite =>
            {
                var dbPath = builder.Configuration["EventBus:DatabasePath"] ?? "eventbus.db";
                sqlite.ConnectionString = $"Data Source={dbPath}";
                sqlite.ClientName = "ProductionClient";
            });
        });

        var host = builder.Build();
        // host.Run();
    }

    /// <summary>
    /// Example 4: In-memory SQLite for testing
    /// </summary>
    public static void Example4_InMemory()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddEventBus(eventBus =>
        {
            eventBus.UseEventHandlersProcessor = true;

            eventBus.AddSqlite(sqlite =>
            {
                sqlite.ConnectionString = "Data Source=EventbusSqlite;Mode=Memory;Cache=Shared;";
                sqlite.ClientName = "TestClient";
            });
        });

        var host = builder.Build();
        // host.Run();
    }

    /// <summary>
    /// Example 5: Full configuration with all options
    /// </summary>
    public static void Example5_FullConfiguration()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddEventBus(eventBus =>
        {
            // Set all core options
            eventBus.ApplicationId = "CompleteApp";
            eventBus.ClientId = "client-001";
            eventBus.UseEventHandlersProcessor = true;
            eventBus.DefaultQueueName = "complete-queue";

            // SQLite with all options
            eventBus.AddSqlite(sqlite =>
            {
                sqlite.ConnectionString = "Data Source=complete.db";
                sqlite.ClientName = "CompleteClient";
                sqlite.HostName = Environment.MachineName;
            });
        });

        var host = builder.Build();
        // host.Run();
    }
}
