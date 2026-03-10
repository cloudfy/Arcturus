using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Internals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus;

public sealed class EventBusBuilder
{
    private readonly List<Assembly> _assembliesToScan = new();
    private bool _scanAllAssemblies = false;

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
    /// Gets or sets a default exchange name. Defaults to "events".
    /// </summary>
    public string? DefaultQueueName { get; set; }
    /// <summary>
    /// If true, events will be processed by the <see cref="Arcturus.EventBus.EventHandlersProcessor"/> or fallback to the <see cref="Arcturus.EventBus.Abstracts.IProcessor.OnProcessAsync"/> event. Default true.
    /// </summary>
    /// <remarks>
    /// The <see cref="Arcturus.EventBus.EventHandlersProcessor"/> provide support for event middleware pipelines. Use <see cref="Arcturus.EventBus.Middleware.HostExtensions.UseEventMiddleware{TMiddleware}(Microsoft.Extensions.Hosting.IHost, object?[])"/>.
    /// </remarks>
    public bool UseEventHandlersProcessor { get; set; }

    /// <summary>
    /// Registers event types and handlers from all assemblies in the current AppDomain.
    /// </summary>
    /// <returns>The builder instance for chaining.</returns>
    public EventBusBuilder RegisterHandlersFromAllAssemblies()
    {
        _scanAllAssemblies = true;
        return this;
    }

    /// <summary>
    /// Registers event types and handlers from a specific assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns>The builder instance for chaining.</returns>
    public EventBusBuilder RegisterHandlersFromAssemblyType(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        _assembliesToScan.Add(assembly);
        return this;
    }

    /// <summary>
    /// Registers event types and handlers from the assembly containing the specified type.
    /// </summary>
    /// <typeparam name="T">A type from the assembly to scan.</typeparam>
    /// <returns>The builder instance for chaining.</returns>
    public EventBusBuilder RegisterHandlersFromAssemblyOfType<T>()
    {
        _assembliesToScan.Add(typeof(T).Assembly);
        return this;
    }

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

    /// <summary>
    /// Populates the EventTypeRegistry and registers handlers in DI.
    /// </summary>
    /// <param name="logger">Optional logger for diagnostic output.</param>
    internal void PopulateEventRegistry(ILogger? logger = null)
    {
        var eventCount = 0;
        var handlerCount = 0;

        if (_scanAllAssemblies)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var (events, handlers) = RegisterFromAssembly(assembly, logger);
                eventCount += events;
                handlerCount += handlers;
            }
        }
        else
        {
            foreach (var assembly in _assembliesToScan.Distinct())
            {
                var (events, handlers) = RegisterFromAssembly(assembly, logger);
                eventCount += events;
                handlerCount += handlers;
            }
        }

        var assemblyCountMsg = _scanAllAssemblies 
            ? "all loaded assemblies" 
            : $"{_assembliesToScan.Distinct().Count()} {(_assembliesToScan.Distinct().Count() == 1 ? "assembly" : "assemblies")}";

        if (logger is not null)
        {
            logger.LogInformation("Registered {EventCount} event types and {HandlerCount} handlers from {AssemblyCount}",
                eventCount, handlerCount, assemblyCountMsg);
        }
        else if (eventCount > 0 || handlerCount > 0)
        {
            // Fallback to console if logger not available
            Console.WriteLine($"[Arcturus.EventBus] Registered {eventCount} event types and {handlerCount} handlers from {assemblyCountMsg}");
        }
    }

    /// <summary>
    /// Registers event types and handlers from a specific assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    /// <param name="logger">Optional logger for diagnostic output.</param>
    /// <returns>Tuple containing counts of registered events and handlers.</returns>
    private (int events, int handlers) RegisterFromAssembly(Assembly assembly, ILogger? logger)
    {
        // Register event types in the registry
        var eventTypes = Internals.AssemblyExtensions.GetEventMessageTypesFromAssembly(assembly);
        foreach (var eventType in eventTypes)
        {
            EventTypeRegistry.RegisterEventType(eventType, logger);
        }

        // Register handlers in DI
        var handlerTypes = Internals.AssemblyExtensions.GetEventHandlerTypesFromAssembly(assembly);
        var handlerCount = 0;

        foreach (var handlerType in handlerTypes)
        {
            var eventHandlerInterfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                           i.GetGenericTypeDefinition() == typeof(IEventMessageHandler<>));

            foreach (var interfaceType in eventHandlerInterfaces)
            {
                Services.TryAddScoped(interfaceType, handlerType);
                handlerCount++;

                logger?.LogDebug("Registered handler: {HandlerType} for {EventType}",
                    handlerType.Name, interfaceType.GenericTypeArguments[0].Name);
            }
        }

        return (eventTypes.Length, handlerCount);
    }

    internal EventBusBuilder(IServiceCollection services)
    {
        Services = services;
    }
}