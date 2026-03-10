namespace Arcturus.EventBus.RabbitMQ;

public sealed class RabbitMQEventBusOptions : EventBusOptions
{
    internal List<Assembly> HandlerAssemblies { get; } = [];
    
    /// <summary>
    /// Gets or sets a client name.
    /// </summary>
    public string? ClientName { get; set; }
    
    /// <summary>
    /// Gets or sets a host name. Defaults to localhost.
    /// </summary>
    [Obsolete("HostName is deprecated. Use ConnectionString instead.")]
    public string? HostName { get; }
    /// <summary>
    /// Gets or sets the amqps:// connection string. If provided, it will override the HostName property.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Registers the assembly that contains the specified type as a handler assembly if it has not already been
    /// registered.
    /// </summary>
    /// <remarks>Use this method to ensure that event handlers or related components defined in the assembly
    /// of type T are available for registration. If the assembly is already registered, this method has no
    /// effect.</remarks>
    /// <typeparam name="T">The type whose containing assembly will be registered for handler discovery.</typeparam>
    public void RegisterHandlersFromAssemblyOf<T>()
    {
        if (HandlerAssemblies.Contains(typeof(T).Assembly))
            return;
        HandlerAssemblies.Add(typeof(T).Assembly);
    }
}