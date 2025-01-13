using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.AzureServiceBus.Internals;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus.AzureServiceBus;

public sealed class ServiceBusProcessor : IProcessor
{
    private readonly ServiceBusConnection _connection;
    private readonly string _queueName;
    private readonly ServiceBusOptions _options;
    private readonly ILogger<ServiceBusProcessor> _logger;

    internal ServiceBusProcessor(
        IConnection connection, ServiceBusOptions options, string? queueName, ILoggerFactory loggerFactory)
    {
        _connection = (ServiceBusConnection)connection;
        _queueName = queueName ?? "default_queue";
        _options = options;
        _logger = loggerFactory.CreateLogger<ServiceBusProcessor>();
    }

    public event Func<IEventMessage, OnProcessEventArgs?, Task>? OnProcessAsync;

    public async Task WaitForEvents(CancellationToken cancellationToken = default)
    {
        await _connection.EnsureConnected();

        // loop and await messages
        var queueClient = await _connection.GetServiceBusClient();
        var processorClient = queueClient.CreateProcessor(
            _queueName
            , new Azure.Messaging.ServiceBus.ServiceBusProcessorOptions {
                ReceiveMode = Azure.Messaging.ServiceBus.ServiceBusReceiveMode.ReceiveAndDelete //.PeekLock
                , Identifier = _options.ClientId
                , MaxConcurrentCalls = 1
                , AutoCompleteMessages = true
            });

        processorClient.ProcessMessageAsync += async args =>
        {
            if (OnProcessAsync is not null)
            {
                _logger.LogTrace("Processing message {MessageId}", args.Message.MessageId);

                var messageBody = args.Message.Body.ToString();
                var @event = EventMessageSerializer.Deserialize(messageBody);
                
                if (OnProcessAsync is not null)
                    await OnProcessAsync.Invoke(
                        @event
                        , new OnProcessEventArgs(
                            args.Message.MessageId
                            , args.Message.ApplicationProperties.ToDictionary()
                            , args.CancellationToken));
            }
        };
        processorClient.ProcessErrorAsync += args =>
        {
            _logger.LogError(args.Exception, "Error processing message");

            return Task.CompletedTask;
        };

        await processorClient.StartProcessingAsync(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            cancellationToken.WaitHandle.WaitOne(_options.PullIntervalMilliseconds ?? 100);
        }
        await processorClient.StopProcessingAsync(cancellationToken);
    }
}