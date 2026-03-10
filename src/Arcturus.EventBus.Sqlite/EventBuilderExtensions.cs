using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus.Sqlite;

public static class EventBuilderExtensions
{
    /// <summary>
    /// Adds the SQLite event bus to the service collection.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options">Optional. Default options of the SQLite event bus such as connection string.</param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static EventBusBuilder AddSqliteEventBus(
        this EventBusBuilder builder
        , Action<SqliteEventBusOptions>? options = null)
    {
        var currentOptions = new SqliteEventBusOptions();
        if (options is not null)
        {
            options(currentOptions);
        }

        builder.Services.AddSingleton<IConnection, SqliteConnection>(
            (sp) =>
            {
                return new SqliteConnection(
                    currentOptions
                    , sp.GetRequiredService<ILoggerFactory>().CreateLogger<SqliteConnection>());
            });
        builder.Services.AddSingleton<IEventBusFactory, SqliteEventBusFactory>();
        builder.Services.AddSingleton<SqliteEventBusOptions>(currentOptions);

        return builder;
    }
}