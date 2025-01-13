using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.AzureServiceBus.Internals;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus.AzureServiceBus;

public sealed class ServiceBusPublisher : IPublisher
{
    private readonly ServiceBusConnection _connection;
    private readonly string _queueName;
    private readonly ILogger<ServiceBusPublisher> _logger;
    private readonly ServiceBusOptions _options;

    internal ServiceBusPublisher(
        IConnection connection
        , string? queue
        , ServiceBusOptions options
        , ILoggerFactory loggerFactory)
    {
        _connection = (ServiceBusConnection)connection;
        _queueName = queue ?? "default-queue";
        _logger = loggerFactory.CreateLogger<ServiceBusPublisher>();
        _options = options;
    }

    public async Task Publish<TEvent>(
        TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEventMessage
    {
        var servieBusClient = await _connection.GetServiceBusClient();
        var sender = servieBusClient.CreateSender(
            _queueName
            , new Azure.Messaging.ServiceBus.ServiceBusSenderOptions
            {
                Identifier = _options.ClientId
            });

        var serviceBusMessage = new Azure.Messaging.ServiceBus.ServiceBusMessage(EventMessageSerializer.Serialize(@event));
        //serviceBusMessage.CorrelationId = ""
        //serviceBusMessage.TimeToLive

        _logger.LogTrace(
            "Publishing event {EventName} to queue {QueueName}, MessageId {MessageId}"
            , @event.GetType().Name
            , _queueName
            , serviceBusMessage.MessageId);

        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
        await sender.DisposeAsync();
    }
}
