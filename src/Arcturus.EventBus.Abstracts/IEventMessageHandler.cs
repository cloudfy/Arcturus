namespace Arcturus.EventBus.Abstracts;

public interface IEventMessageHandler<in TEvent> where TEvent : IEventMessage
{
    Task Handle(TEvent @event, CancellationToken cancellationToken = default);
}
