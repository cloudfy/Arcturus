using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.EventBus.AzureServiceBus;

public static class EventBuilderExtensions
{
    /// <summary>
    /// Configures the event bus to use Azure Service Bus as the underlying message transport.
    /// </summary>
    /// <remarks>
    /// This method registers the necessary services for using Azure Service Bus with the event bus,
    /// including the connection and factory implementations.
    /// </remarks>
    /// <param name="builder">The EventBusBuilder instance used to configure event bus services.</param>
    /// <param name="options">An optional delegate to configure the ServiceBusOptions for the event bus.</param>
    /// <returns>The updated EventBusBuilder instance for further configuration.</returns>
    public static EventBusBuilder AddAzureServiceBusEventBus(
        this EventBusBuilder builder
        , Action<ServiceBusOptions>? options = null)
    {
        var currentOptions = new ServiceBusOptions();
        if (options is not null)
        {
            options(currentOptions);
        }

        builder.Services.AddSingleton<IConnection, ServiceBusConnection>(
            (sp) => { return new ServiceBusConnection(currentOptions); });
        builder.Services.AddSingleton<IEventBusFactory, ServiceBusFactory>();
        builder.Services.AddSingleton<ServiceBusOptions>(currentOptions);

        return builder;
    }
}