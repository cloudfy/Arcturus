using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.EventBus;

/// <summary>
/// Builder for configuring EventBus services.
/// </summary>
public sealed class EventBusBuilder
{
    /// <summary>
    /// Gets the service collection.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Gets or sets an application name.
    /// </summary>
    public string? ApplicationId { get; set; }

    /// <summary>
    /// Gets or sets the clientId of the ServiceBus client. This is used for logging.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// If true, events will be processed by the <see cref="EventHandlersProcessor"/> or fallback to the <see cref="Abstracts.IProcessor.OnProcessAsync"/> event.
    /// </summary>
    /// <remarks>
    /// The <see cref="EventHandlersProcessor"/> provide support for event middleware pipelines. Use <see cref="Middleware.HostExtensions.UseEventMiddleware{TMiddleware}(Microsoft.Extensions.Hosting.IHost, object?[])"/>.
    /// </remarks>
    public bool UseEventHandlersProcessor { get; set; }

    /// <summary>
    /// Gets or sets the default queue. If no queue is specified the default queue is 'default_queue'.
    /// </summary>
    public string? DefaultQueueName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBusBuilder"/> class.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public EventBusBuilder(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>
    /// Builds the EventBusOptions from the builder configuration.
    /// </summary>
    /// <returns>An instance of <see cref="EventBusOptions"/>.</returns>
    internal EventBusOptions BuildOptions()
    {
        return new EventBusOptions
        {
            ApplicationId = ApplicationId,
            ClientId = ClientId,
            UseEventHandlersProcessor = UseEventHandlersProcessor,
            DefaultQueueName = DefaultQueueName
        };
    }
}
