using Arcturus.EventBus.Abstracts;
using System.Collections.Concurrent;

namespace Arcturus.EventBus.Middleware;

public sealed class EventContext(
    IEventMessage @event
    , string eventName
    , IServiceProvider serviceProvider)
{
    /// <summary>
    /// Gets the event message.
    /// </summary>
    public IEventMessage Event { get; } = @event;
    /// <summary>
    /// Gets the name of the event.
    /// </summary>
    public string EventName { get; } = eventName;
    /// <summary>
    /// Gets the service provider.
    /// </summary>
    public IServiceProvider RequestServices { get; } = serviceProvider;
    /// <summary>
    /// Provides methods to add, remove, and retrieve items from a shared property bag.
    /// </summary>
    public IDictionary<object, object?> Items { get; } = new ConcurrentDictionary<object, object?>();
}

