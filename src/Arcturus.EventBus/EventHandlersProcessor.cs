using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Arcturus.EventBus;

/// <summary>
/// Initializes a new instance of the auto-event wired message processor. This handles the events.
/// </summary>
public sealed class EventHandlersProcessor : IProcessor, IDisposable
{
    private readonly IProcessor _processor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventHandlersProcessor> _logger;
    private readonly EventTypeRegistry _eventTypeRegistry;
    private readonly SemaphoreSlim _semaphore;

    public EventHandlersProcessor(
        IProcessor processor, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _processor = processor;
        _processor.OnProcessAsync += InternalOnProcessAsync;
        _eventTypeRegistry = serviceProvider.GetRequiredService<EventTypeRegistry>();
        _serviceProvider = serviceProvider;
        _logger = loggerFactory.CreateLogger<EventHandlersProcessor>();

        var options = serviceProvider.GetRequiredService<EventBusOptions>();
        var parallelism = Math.Max(1, options.MaxDegreeOfParallelism);
        _semaphore = new SemaphoreSlim(parallelism, parallelism);
    }

    /// <inheritdoc />
    public event Func<IEventMessage, OnProcessEventArgs?, Task>? OnProcessAsync;

    /// <inheritdoc />
    public async Task WaitForEvents(CancellationToken cancellationToken = default)
        => await _processor.WaitForEvents(cancellationToken);

    /// <inheritdoc />
    public void Dispose()
    {
        // Unsubscribe from the inner processor event to avoid retaining this wrapper instance.
        _processor.OnProcessAsync -= InternalOnProcessAsync;

        // Dispose the semaphore used for concurrency control.
        _semaphore.Dispose();

        // Dispose the inner processor if it supports disposal to avoid leaking resources.
        if (_processor is IAsyncDisposable asyncDisposable)
        {
            asyncDisposable.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
        else if (_processor is IDisposable disposable)
        {
            disposable.Dispose();
        }

        System.GC.SuppressFinalize(this);
    }

    private async Task InternalOnProcessAsync(IEventMessage @event, OnProcessEventArgs? e)
    {
        // O(1) dictionary lookup — MakeGenericType, GetMethod, and IsAssignableFrom are all pre-computed at startup.
        var handlerEntry = _eventTypeRegistry.GetHandlerEntryByEventType(@event);

        if (handlerEntry is null)
        {
            _logger.LogWarning("There are no handlers for the following event: {EventName}. Attempting OnProcessAsync delegate.", @event.GetEventName());

            if (OnProcessAsync is not null)
                await OnProcessAsync!.Invoke(@event, e);

            return;
        }

        await using var scope = _serviceProvider.CreateAsyncScope();

        await _semaphore.WaitAsync(e?.CancellationToken ?? default);
        try
        {
            var handler = ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, handlerEntry.HandlerType);
            if (handler == null)
            {
                _logger.LogWarning("There are no handlers for the following event: {EventName}", @event.GetEventName());
                return;
            }

            var pipeline = HostExtensions.BuildByRequestDelegate(
                async () =>
                {
                    await (Task)handlerEntry.HandleMethod.Invoke(handler, [@event!, e?.CancellationToken ?? default])!;
                });

            await pipeline(new EventContext(@event, nameof(@event), scope.ServiceProvider));
        }
        finally
        {
            _semaphore.Release();
        }
    }
}