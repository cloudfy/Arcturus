using Arcturus.EventBus;
using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.EventBus.RabbitMQ;

public static class EventBusBuilderExtensions
{
    /// <summary>
    /// Adds RabbitMQ as the EventBus provider.
    /// </summary>
    /// <param name="builder">The EventBus builder.</param>
    /// <param name="configure">Configuration action for RabbitMQ options.</param>
    /// <returns>The EventBus builder for chaining.</returns>
    public static EventBusBuilder AddRabbitMQ(
        this EventBusBuilder builder,
        Action<RabbitMQEventBusOptions> configure)
    {
        var options = new RabbitMQEventBusOptions
        {
            ApplicationId = builder.ApplicationId,
            ClientId = builder.ClientId,
            UseEventHandlersProcessor = builder.UseEventHandlersProcessor,
            DefaultQueueName = builder.DefaultQueueName
        };
        
        // Apply RabbitMQ-specific configuration
        configure(options);
        
        // Register RabbitMQ-specific services
        builder.Services.AddSingleton<IDefaultEventMessageSerializer, DefaultEventMessageSerializer>(
            sp => new DefaultEventMessageSerializer([.. options.HandlerAssemblies]));
        
        builder.Services.AddSingleton<IConnection, RabbitMQConnection>(
            sp => new RabbitMQConnection(options));
        
        builder.Services.AddSingleton<IEventBusFactory, RabbitMQEventBusFactory>();
        builder.Services.AddSingleton(options);
        
        return builder;
    }
}
