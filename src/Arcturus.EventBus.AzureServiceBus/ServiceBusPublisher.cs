using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.AzureServiceBus.Internals;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus.AzureServiceBus;

public sealed class ServiceBusPublisher : IPublisher, IAsyncDisposable
{
    private readonly ServiceBusConnection _connection;
    private readonly string _queueName;
    private readonly ILogger<ServiceBusPublisher> _logger;
    private readonly ServiceBusOptions _options;
    private ServiceBusSender? _sender;

    internal ServiceBusPublisher(
        IConnection connection
        , string queueName
        , ServiceBusOptions options
        , ILoggerFactory loggerFactory)
    {
        _connection = (ServiceBusConnection)connection;
        _queueName = queueName;
        _logger = loggerFactory.CreateLogger<ServiceBusPublisher>();
        _options = options;
    }

    private async ValueTask<ServiceBusSender> GetOrCacheSender()
    {
        if (_sender is null)
        {
            var serviceBusClient = await _connection.GetServiceBusClient();
            _sender = serviceBusClient.CreateSender(
                _queueName
                , new ServiceBusSenderOptions
                {
                    Identifier = _options.ClientId
                });
        }
        return _sender;
    }
    public async Task Publish<TEvent>(
        TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEventMessage
    {
        var sender = await GetOrCacheSender();

        var serviceBusMessage = new ServiceBusMessage(EventMessageSerializer.Serialize(@event))
        {
            MessageId = Guid.NewGuid().ToString("N")
        };
        //serviceBusMessage.CorrelationId = ""
        //serviceBusMessage.TimeToLive

        _logger.LogTrace(
            "Publishing event {EventName} to queue {QueueName}, MessageId {MessageId}"
            , @event.GetType().Name
            , _queueName
            , serviceBusMessage.MessageId);

        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _sender?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
}