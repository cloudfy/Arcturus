namespace Arcturus.EventBus.Abstracts;

public interface IEventBusFactory
{
    IProcessor CreateProcessor(string? queue = null);
    IPublisher CreatePublisher(string? queue = null);
    ISubscriber CreateSubscriber();
}
