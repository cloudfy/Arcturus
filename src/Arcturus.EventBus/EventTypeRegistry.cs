using Arcturus.EventBus.Abstracts;
namespace Arcturus.EventBus;

/// <summary>
/// Pre-computed handler metadata resolved at startup.
/// </summary>
internal sealed record EventHandlerEntry(Type HandlerType, Type EventHandlerInterfaceType, MethodInfo HandleMethod);

public sealed class EventTypeRegistry // singleton
{
    private readonly Lazy<EventTypeEntry[]> _eventMessageRegistry;
    private readonly Lazy<Dictionary<Type, EventHandlerEntry>> _handlerMap;

    internal EventTypeRegistry(IReadOnlyCollection<Assembly> assemblies)
    {
        _eventMessageRegistry = new Lazy<EventTypeEntry[]>(() => {
            List<EventTypeEntry> _list = [];

            foreach (var assembly in assemblies.Distinct())
            {
                foreach (var eventMessageType in Internals.AssemblyExtensions.GetEventMessageTypesFromAssembly(assembly))
                {
                    _list.Add(new EventTypeEntry(eventMessageType.Item1, eventMessageType.Item2));
                }
            }

            return [.. _list.DistinctBy(_ => _.Name)];
        });

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

                    // First registration wins when multiple handlers target the same event type.
                    map.TryAdd(eventType, new EventHandlerEntry(handlerType, handlerInterface, handleMethod));
                }
            }

            return map;
        });
    }

    /// <summary>
    /// O(1) lookup — no reflection at runtime.
    /// </summary>
    internal EventHandlerEntry? GetHandlerEntryByEventType(IEventMessage eventMessage)
        => _handlerMap.Value.TryGetValue(eventMessage.GetType(), out var entry) ? entry : null;

    internal Type? GetTypeByName(string name)
    {
        if (_eventMessageRegistry.Value.FirstOrDefault(_ => _.Name == name) is EventTypeEntry entry)
        {
            return entry.Type;
        }
        return null;
    }

    internal string? GetNameByType(Type type)
    {
        if (_eventMessageRegistry.Value.FirstOrDefault(_ => _.Type == type) is EventTypeEntry entry)
        {
            return entry.Name;
        }
        return null;
    }

    private record EventTypeEntry(string Name, Type Type);
}