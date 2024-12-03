using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.EventBus.RabbitMQ;

public static class ServicesExtensions
{
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
