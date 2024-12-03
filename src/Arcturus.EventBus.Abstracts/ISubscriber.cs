namespace Arcturus.EventBus.Abstracts;

public interface ISubscriber
{
    Task Subscribe<TEvent>(
        Func<TEvent, Task> handler
        , CancellationToken cancellationToken = default) where TEvent : IEventMessage;
}
