using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.EventBus.RabbitMQ;

public static class EventBuilderExtensions
{
    /// <summary>
    /// Configures the event bus to use RabbitMQ as the underlying message broker.
    /// </summary>
    /// <remarks>This method registers the necessary services for RabbitMQ, including the connection and event
    /// bus factory. Ensure that RabbitMQ is properly configured before using this method.</remarks>
    /// <param name="builder">The <see cref="EventBusBuilder"/> instance used to configure the event bus.</param>
    /// <param name="options">An optional action to configure the <see cref="RabbitMQEventBusOptions"/> for RabbitMQ settings.</param>
    /// <returns>The updated <see cref="EventBusBuilder"/> instance for method chaining.</returns>
    public static EventBusBuilder UseRabbitMQ(
        this EventBusBuilder builder
        , Action<RabbitMQEventBusOptions>? options = null)
    {
        var currentOptions = new RabbitMQEventBusOptions();
        if (options is not null)
        {
            options(currentOptions);
        }

        currentOptions.UseEventHandlersProcessor = builder.UseEventHandlersProcessor;
        currentOptions.ApplicationId = builder.ApplicationId;
        currentOptions.ClientName = builder.ClientName;
        currentOptions.MaxDegreeOfParallelism = builder.MaxDegreeOfParallelism;

        builder.Services.AddSingleton<IConnection, RabbitMQConnection>(
            (sp) => { return new RabbitMQConnection(currentOptions); });
        builder.Services.AddSingleton<IEventBusFactory, RabbitMQEventBusFactory>();
        builder.Services.AddSingleton<RabbitMQEventBusOptions>(currentOptions);

        return builder;
    }
}