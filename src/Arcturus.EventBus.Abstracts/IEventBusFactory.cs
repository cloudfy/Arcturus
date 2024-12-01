namespace Arcturus.EventBus.Abstracts;

public interface IEventBusFactory
{
    IProcessor CreateProcessor();
    IPublisher CreatePublisher();
    ISubscriber CreateSubscriber();
}
