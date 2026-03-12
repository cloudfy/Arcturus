using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.EventBus;

public sealed class EventBusBuilder
{
    private readonly List<Assembly> _assembliesToScan = [];
    
    internal EventBusBuilder(IServiceCollection services) => Services = services;

    /// <summary>
    /// Gets the service collection.
    /// </summary>
    public IServiceCollection Services { get; }
    /// <summary>
    /// Gets or sets an application id.
    /// </summary>
    public string? ApplicationId { get; set; }
    /// <summary>
    /// Gets or sets a client name.
    /// </summary>
    public string? ClientName { get; set; }
    /// <summary>
    /// Gets or sets a default queue name. Defaults to "events".
    /// </summary>
    public string? DefaultQueueName { get; set; }
    /// <summary>
    /// If true, events will be processed by the <see cref="Arcturus.EventBus.EventHandlersProcessor"/> or fallback to the <see cref="Arcturus.EventBus.Abstracts.IProcessor.OnProcessAsync"/> event. Default true.
    /// </summary>
    /// <remarks>
    /// The <see cref="Arcturus.EventBus.EventHandlersProcessor"/> provide support for event middleware pipelines. Use <see cref="Arcturus.EventBus.Middleware.HostExtensions.UseEventMiddleware{TMiddleware}(Microsoft.Extensions.Hosting.IHost, object?[])"/>.
    /// </remarks>
    public bool UseEventHandlersProcessor { get; set; }

    internal EventBusOptions BuildOptions()
    {
        return new EventBusOptions
        {
            ApplicationId = ApplicationId,
            ClientName = ClientName,
            UseEventHandlersProcessor = UseEventHandlersProcessor,
            DefaultQueueName = DefaultQueueName
        };
    }

    internal IReadOnlyCollection<Assembly> AssembliesToScan => _assembliesToScan;
    internal bool ScanAllAssemblies { get; private set; } = false;

    /// <summary>
    /// Registers all all <see cref="Arcturus.EventBus.Abstracts.IEventMessageHandler{TEvent}" /> types types found in the specified assembly for use with the event bus.
    /// </summary>
    /// <remarks>Use this method to add event handler types from an external or dynamically loaded assembly.
    /// The assembly should contain types that implement the required event handler interfaces for them to be discovered
    /// and registered.</remarks>
    /// <param name="assembly">The assembly to scan for event handler types. Cannot be null.</param>
    /// <returns>The current instance of the EventBusBuilder, enabling method chaining.</returns>
    public EventBusBuilder RegisterHandlersFromAssemblyType(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        _assembliesToScan.Add(assembly);
        return this;
    }
    /// <summary>
    /// Configures the event bus to automatically register all <see cref="Arcturus.EventBus.Abstracts.IEventMessageHandler{TEvent}" /> types by scanning all loaded assemblies.
    /// </summary>
    /// <remarks>Use this method to simplify event handler registration when handlers are defined across
    /// multiple assemblies. All assemblies loaded in the current application domain will be scanned for compatible
    /// event handler types.</remarks>
    /// <returns>The current instance of the EventBusBuilder, enabling method chaining.</returns>
    public EventBusBuilder RegisterHandlersFromAllAssemblies()
    {
        ScanAllAssemblies = true;
        return this;
    }
    /// <summary>
    /// Registers all <see cref="Arcturus.EventBus.Abstracts.IEventMessageHandler{TEvent}" /> types found in the assembly that contains the specified type parameter.
    /// </summary>
    /// <remarks>Use this method to automatically discover and register event handlers from a specific
    /// assembly without manually specifying each handler type. This is useful for modular applications where handlers
    /// are grouped by assembly.</remarks>
    /// <typeparam name="T">The type whose containing assembly will be scanned for event handler implementations.</typeparam>
    /// <returns>The current instance of the EventBusBuilder to allow for method chaining.</returns>
    public EventBusBuilder RegisterHandlersFromAssemblyOfType<T>()
    {
        _assembliesToScan.Add(typeof(T).Assembly);
        return this;
    }
}
