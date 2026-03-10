using Arcturus.EventBus;
using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus.Sqlite;

public static class EventBusBuilderExtensions
{
    /// <summary>
    /// Adds SQLite as the EventBus provider.
    /// </summary>
    /// <param name="builder">The EventBus builder.</param>
    /// <param name="configure">Configuration action for SQLite options.</param>
    /// <returns>The EventBus builder for chaining.</returns>
    public static EventBusBuilder AddSqlite(
        this EventBusBuilder builder,
        Action<SqliteEventBusOptions> configure)
    {
        var options = new SqliteEventBusOptions
        {
            ApplicationId = builder.ApplicationId,
            DefaultQueueName = builder.DefaultQueueName
        };
        
        // Apply SQLite-specific configuration
        configure(options);
        
        // Register SQLite-specific services
        builder.Services.AddSingleton<IConnection, SqliteConnection>(sp =>
        {
            return new SqliteConnection(
                options,
                sp.GetRequiredService<ILoggerFactory>().CreateLogger<SqliteConnection>());
        });
        
        builder.Services.AddSingleton<IEventBusFactory, SqliteEventBusFactory>();
        builder.Services.AddSingleton(options);
        
        return builder;
    }
}
