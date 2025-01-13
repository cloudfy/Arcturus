using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Arcturus.EventBus.AzureServiceBus;
public sealed class ServiceBusFactory(
    IConnection connection
    , IServiceProvider serviceProvider
    , ILoggerFactory loggerFactory
    , ServiceBusOptions options) : IEventBusFactory, IAsyncDisposable
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IConnection _connection = connection;
    private readonly ILoggerFactory _loggerFactory = loggerFactory;
    private readonly ServiceBusOptions _options = options;

    private readonly ConcurrentDictionary<string, ServiceBusPublisher> _cachedPublishers = [];

    public IProcessor CreateProcessor(string? queueName = null)
    {
        // returns a wrapper processor that will handle the event handlers
        // and fallback to the processor if no handlers are found
        if (_options.UseEventHandlersProcessor.GetValueOrDefault(true))
            return new EventHandlersProcessor(
                new ServiceBusProcessor(
                    _connection
                    , _options
                    , GetQueueOrDefaults(queueName), _loggerFactory)
                , _serviceProvider, _loggerFactory);

        return new ServiceBusProcessor(_connection, _options, GetQueueOrDefaults(queueName), _loggerFactory);
    }

    public IPublisher CreatePublisher(string? queueName = null)
    {
        // cache each publisher per queue
        return _cachedPublishers.GetOrAdd(
            GetQueueOrDefaults(queueName)
            , new ServiceBusPublisher(_connection, GetQueueOrDefaults(queueName), _options, _loggerFactory));
    }

    public async ValueTask DisposeAsync()
    {
        if (_cachedPublishers.Count == 0) return;

        // dispose cached publishers
        foreach (var item in _cachedPublishers.Values)
        {
            await item.DisposeAsync();
        }
    }
    private string GetQueueOrDefaults(string? queueName)
    {
        return queueName ?? _options.DefaultQueueName ?? "default-queue";
    }
}
