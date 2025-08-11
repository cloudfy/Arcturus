using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus.Sqlite;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds the SQLite event bus to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options">Optional. Default options of the SQLite event bus such as connection string.</param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddSqliteEventBus(
        this IServiceCollection services
        , Action<SqliteEventBusOptions>? options = null)
    {
        var currentOptions = new SqliteEventBusOptions();
        if (options is not null)
        {
            options(currentOptions);
        }

        services.AddSingleton<IConnection, SqliteConnection>(
            (sp) => { 
                return new SqliteConnection(
                    currentOptions
                    , sp.GetRequiredService<ILoggerFactory>().CreateLogger<SqliteConnection>()); 
            });
        services.AddSingleton<IEventBusFactory, SqliteEventBusFactory>();
        services.AddSingleton<SqliteEventBusOptions>(currentOptions);

        return services;
    }
}