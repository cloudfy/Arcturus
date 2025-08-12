using Arcturus.EventBus.Abstracts;
using System.Collections.Concurrent;

namespace Arcturus.EventBus.Middleware;


/// <summary>
/// Represents the context for an event being processed in the event bus middleware.
/// Provides access to the event message, event name, service provider, and a shared property bag.
/// </summary>
/// <remarks>
/// The <see cref="EventContext"/> is used to pass contextual information and shared data between middleware components
/// during event processing. The <see cref="Items"/> property allows middleware to store and retrieve arbitrary data.
/// </remarks>
public sealed class EventContext(
    IEventMessage @event,
    string eventName,
    IServiceProvider serviceProvider)
{
    /// <summary>
    /// Gets the event message associated with the current context.
    /// </summary>
    public IEventMessage Event { get; } = @event;

    /// <summary>
    /// Gets the name of the event being processed.
    /// </summary>
    public string EventName { get; } = eventName;

    /// <summary>
    /// Gets the service provider for resolving dependencies within the event context.
    /// </summary>
    public IServiceProvider RequestServices { get; } = serviceProvider;

    /// <summary>
    /// Gets a thread-safe property bag for sharing data between middleware components.
    /// </summary>
    /// <remarks>
    /// The <see cref="Items"/> dictionary can be used to store and retrieve arbitrary objects relevant to the event processing pipeline.
    /// </remarks>
    public IDictionary<object, object?> Items { get; } = new ConcurrentDictionary<object, object?>();
}

