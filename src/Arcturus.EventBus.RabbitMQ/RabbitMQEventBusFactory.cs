using Arcturus.EventBus.Abstracts;

namespace Arcturus.EventBus.RabbitMQ;

public sealed class RabbitMQEventBusFactory(
    IConnection connection) : IEventBusFactory
{
    public IPublisher CreatePublisher()
    {
        return new RabbitMQPublisher(connection);
    }
    public IProcessor CreateProcessor()
    {
        return new RabbitMQProcessor(connection);
    }
    public ISubscriber CreateSubscriber()
    {
        throw new NotImplementedException();
    }
}
