namespace Arcturus.EventBus.Abstracts;

/// <summary>
/// Abstract interface to publishing events.
/// </summary>
public interface IPublisher
{
    /// <summary>
    /// Publishes an <see cref="IEventMessage" /> via the event bus.
    /// </summary>
    /// <typeparam name="TEvent">Event type implemented by <see cref="IEventMessage"/></typeparam>
    /// <param name="event">Required. Event to publish.</param>
    /// <param name="cancellationToken">Optional. Propogates notification that events should be cancelled.</param>
    /// <returns><see cref="Task"/></returns>
    Task Publish<TEvent>(
        TEvent @event
        , CancellationToken cancellationToken = default) where TEvent : IEventMessage;
}
