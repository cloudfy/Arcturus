using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus;

/// <summary>
/// Pre-computed handler metadata resolved at startup.
/// </summary>
internal sealed record EventHandlerEntry(Type HandlerType, Type EventHandlerInterfaceType, MethodInfo HandleMethod);

public sealed class EventTypeRegistry // singleton
{
    private readonly ILogger<EventTypeRegistry>? _logger;
    private readonly Lazy<Dictionary<string, Type>> _typesByName;
    private readonly Lazy<Dictionary<Type, string>> _namesByType;
    private readonly Lazy<Dictionary<Type, EventHandlerEntry>> _handlerMap;

    internal EventTypeRegistry(ILoggerFactory? loggerFactory, IReadOnlyCollection<Assembly> assemblies)
    {
        _logger = loggerFactory?.CreateLogger<EventTypeRegistry>();

        // Built once at startup: event type → pre-computed handler entry (no runtime reflection).
        _handlerMap = new Lazy<Dictionary<Type, EventHandlerEntry>>(() =>
        {
            var map = new Dictionary<Type, EventHandlerEntry>();

            foreach (var assembly in assemblies.Distinct())
            {
                // GetEventHandlerTypesFromAssembly already guarantees concrete IEventMessageHandler<> implementors.
                foreach (var handlerType in Internals.AssemblyExtensions.GetEventHandlerTypesFromAssembly(assembly))
                {
                    var handlerInterface = handlerType.GetInterfaces()
                        .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventMessageHandler<>));

                    var eventType = handlerInterface.GenericTypeArguments[0];
                    var handleMethod = handlerInterface.GetMethod(nameof(IEventMessageHandler<IEventMessage>.Handle))!;

                    _logger?.LogDebug(
                        "Registering handler {FullName} for event type {EventTypeFullName}"
                        , handlerType.FullName, eventType.FullName);
                    // First registration wins when multiple handlers target the same event type.
                    map.TryAdd(eventType, new EventHandlerEntry(handlerType, handlerInterface, handleMethod));
                }
            }

            return map;
        });

        // Single initialisation shared between _typesByName and _namesByType; avoids scanning
        // assemblies twice and re-resolving handler interfaces already computed by _handlerMap.
        var sharedEntries = new Lazy<(Dictionary<string, Type> ByName, Dictionary<Type, string> ByType)>(() =>
        {
            var byName = new Dictionary<string, Type>();
            var byType = new Dictionary<Type, string>();

            foreach (var assembly in assemblies.Distinct())
            {
                foreach (var (name, type) in Internals.AssemblyExtensions.GetEventMessageTypesFromAssembly(assembly))
                {
                    _logger?.LogDebug("Registering event message {Name} with type {TypeFullName}", name, type.FullName);
                    byName.TryAdd(name, type);
                    byType.TryAdd(type, name);
                }
            }

            // Supplement with event types sourced from the handler map; covers cross-assembly
            // scenarios where the event type lives in an assembly that was not explicitly registered.
            foreach (var eventType in _handlerMap.Value.Keys)
            {
                var attr = eventType.GetCustomAttribute<EventMessageAttribute>(true);
                var name = attr?.Name ?? eventType.FullName!;
                byName.TryAdd(name, eventType);
                byType.TryAdd(eventType, name);
            }

            return (byName, byType);
        });

        _typesByName = new Lazy<Dictionary<string, Type>>(() => sharedEntries.Value.ByName);
        _namesByType = new Lazy<Dictionary<Type, string>>(() => sharedEntries.Value.ByType);
    }

    internal EventHandlerEntry? GetHandlerEntryByEventType(IEventMessage eventMessage)
        => _handlerMap.Value.TryGetValue(eventMessage.GetType(), out var entry) ? entry : null;

    internal Type? GetTypeByName(string name)
        => _typesByName.Value.TryGetValue(name, out var type) ? type : null;

    internal string? GetNameByType(Type type)
        => _namesByType.Value.TryGetValue(type, out var name) ? name : null;
}