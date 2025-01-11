using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.EventBus.RabbitMQ;

public static class ServicesExtensions
{
    /// <summary>
    /// Adds RabbitMQ EventBus to the service collection.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="options">Optional.</param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddRabbitMQEventBus(
        this IServiceCollection services
        , Action<RabbitMQEventBusOptions>? options = null)
    {
        var currentOptions = new RabbitMQEventBusOptions();
        if (options is not null)
        {
            options(currentOptions);
        }

        services.AddSingleton<IConnection, RabbitMQConnection>(
            (sp) => { return new RabbitMQConnection(currentOptions); });
        services.AddSingleton<IEventBusFactory, RabbitMQEventBusFactory>();
        services.AddSingleton<RabbitMQEventBusOptions>(currentOptions);

        return services;
    }
}