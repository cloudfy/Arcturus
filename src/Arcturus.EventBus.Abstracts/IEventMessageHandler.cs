namespace Arcturus.EventBus.Abstracts;

/// <summary>
/// Abstracts interface to implement event message handlers, using the <see cref="IProcessor"/>.
/// </summary>
/// <typeparam name="TEvent">Typed of event, implemented using the <see cref="IEventMessage"/> interface.</typeparam>
public interface IEventMessageHandler<in TEvent> where TEvent : IEventMessage
{
    /// <summary>
    /// Handle the event message.
    /// </summary>
    /// <param name="event">Event to handle.</param>
    /// <param name="cancellationToken">Propogates notification that events should be cancelled.</param>
    /// <returns><see cref="Task"/></returns>
    Task Handle(TEvent @event, CancellationToken cancellationToken = default);
}
