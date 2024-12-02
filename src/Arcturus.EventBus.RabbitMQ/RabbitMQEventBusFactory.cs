using Arcturus.EventBus.Abstracts;

namespace Arcturus.EventBus.RabbitMQ;

public sealed class RabbitMQEventBusFactory(
    IConnection connection) : IEventBusFactory
{
    public IPublisher CreatePublisher(string? queue = null)
    {
        return new RabbitMQPublisher(connection, queue);
    }
    public IProcessor CreateProcessor(string? queue = null)
    {
        return new RabbitMQProcessor(connection, queue);
    }
    public ISubscriber CreateSubscriber()
    {
        throw new NotImplementedException();
    }
}
