using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Arcturus.EventBus.Internals;

/// <summary>
/// Central registry for mapping event names to types and vice versa.
/// Populated at startup to enable fast serialization/deserialization.
/// </summary>
internal static class EventTypeRegistry
{
    private static readonly ConcurrentDictionary<string, Type> _nameToType = new();
    private static readonly ConcurrentDictionary<Type, string> _typeToName = new();
    private static readonly object _lock = new();

    /// <summary>
    /// Registers a single event type in the registry.
    /// </summary>
    /// <param name="eventType">The event type to register.</param>
    /// <param name="logger">Optional logger for diagnostic output.</param>
    /// <exception cref="ArgumentNullException">Thrown when eventType is null.</exception>
    /// <exception cref="ArgumentException">Thrown when eventType doesn't implement IEventMessage or is abstract/interface.</exception>
    /// <exception cref="InvalidOperationException">Thrown when duplicate event names are detected.</exception>
    internal static void RegisterEventType(Type eventType, ILogger? logger = null)
    {
        ValidateEventType(eventType);

        var eventName = GetEventName(eventType);

        lock (_lock)
        {
            // Check for duplicates - THROW EXCEPTION
            if (_nameToType.TryGetValue(eventName, out var existingType) && existingType != eventType)
            {
                throw new InvalidOperationException(
                    $"Duplicate event name '{eventName}' detected. " +
                    $"Type '{eventType.FullName}' conflicts with existing type '{existingType.FullName}'.");
            }

            _nameToType.TryAdd(eventName, eventType);
            _typeToName.TryAdd(eventType, eventName);

            logger?.LogDebug("Registered event type: {EventName} -> {TypeFullName}",
                eventName, eventType.FullName);
        }
    }

    /// <summary>
    /// Registers all event types from a specific assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for event types.</param>
    /// <param name="logger">Optional logger for diagnostic output.</param>
    internal static void RegisterEventTypesFromAssembly(Assembly assembly, ILogger? logger = null)
    {
        var eventTypes = AssemblyExtensions.GetEventMessageTypesFromAssembly(assembly);
        foreach (var eventType in eventTypes)
        {
            RegisterEventType(eventType, logger);
        }
    }

    /// <summary>
    /// Registers all event types from all assemblies in the current AppDomain.
    /// </summary>
    /// <param name="logger">Optional logger for diagnostic output.</param>
    internal static void RegisterEventTypesFromAllAssemblies(ILogger? logger = null)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            RegisterEventTypesFromAssembly(assembly, logger);
        }
    }

    /// <summary>
    /// Gets the event type by its registered name.
    /// </summary>
    /// <param name="eventName">The name of the event to lookup.</param>
    /// <returns>The Type if found, otherwise null.</returns>
    internal static Type? GetTypeByName(string eventName)
    {
        ArgumentNullException.ThrowIfNull(eventName);
        return _nameToType.TryGetValue(eventName, out var type) ? type : null;
    }

    /// <summary>
    /// Gets the registered name for an event type.
    /// </summary>
    /// <param name="eventType">The event type to lookup.</param>
    /// <returns>The event name if found, otherwise null.</returns>
    internal static string? GetNameByType(Type eventType)
    {
        ArgumentNullException.ThrowIfNull(eventType);
        return _typeToName.TryGetValue(eventType, out var name) ? name : null;
    }

    /// <summary>
    /// Checks if an event type is registered in the cache.
    /// </summary>
    /// <param name="eventType">The event type to check.</param>
    /// <returns>True if registered, otherwise false.</returns>
    internal static bool IsRegistered(Type eventType)
    {
        ArgumentNullException.ThrowIfNull(eventType);
        return _typeToName.ContainsKey(eventType);
    }

    /// <summary>
    /// Gets the event name from the type, using EventMessageAttribute if present, otherwise FullName.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <returns>The event name.</returns>
    private static string GetEventName(Type eventType)
    {
        var attr = eventType.GetCustomAttribute<EventMessageAttribute>();
        return attr?.Name ?? eventType.FullName ?? eventType.Name;
    }

    /// <summary>
    /// Validates that the type is a valid event type.
    /// </summary>
    /// <param name="eventType">The type to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when eventType is null.</exception>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    private static void ValidateEventType(Type eventType)
    {
        ArgumentNullException.ThrowIfNull(eventType);

        if (!typeof(IEventMessage).IsAssignableFrom(eventType))
            throw new ArgumentException($"Type '{eventType.FullName}' must implement IEventMessage.", nameof(eventType));

        if (eventType.IsInterface || eventType.IsAbstract)
            throw new ArgumentException($"Type '{eventType.FullName}' cannot be abstract or interface.", nameof(eventType));
    }

    /// <summary>
    /// Clears all registrations. Used for testing purposes.
    /// </summary>
    internal static void Clear()
    {
        lock (_lock)
        {
            _nameToType.Clear();
            _typeToName.Clear();
        }
    }
}
