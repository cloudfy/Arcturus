using Arcturus.Mediation.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Arcturus.Mediation;

/// <summary>
/// Default implementation of the mediator.
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Mediator> _logger;
    private readonly IEnumerable<IMiddleware> _middlewares;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving handlers.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="middlewares">The registered middlewares.</param>
    public Mediator(IServiceProvider serviceProvider, ILogger<Mediator> logger, IEnumerable<IMiddleware> middlewares)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _middlewares = middlewares;
    }

    /// <inheritdoc />
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        var responseType = typeof(TResponse);
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        _logger.LogDebug("Sending request {RequestType} expecting response {ResponseType}", requestType.Name, responseType.Name);

        var context = new MiddlewareContext(request, requestType, cancellationToken);
        var response = default(TResponse);

        var pipeline = BuildPipeline(context, async () =>
        {
            var handler = _serviceProvider.GetService(handlerType);
            if (handler == null)
            {
                throw new InvalidOperationException($"No handler found for request type {requestType.Name}");
            }

            var handleMethod = handlerType.GetMethod("Handle");
            if (handleMethod == null)
            {
                throw new InvalidOperationException($"Handle method not found on handler for {requestType.Name}");
            }

            var result = handleMethod.Invoke(handler, new object[] { request, cancellationToken });
            if (result is Task<TResponse> taskResult)
            {
                response = await taskResult;
            }
            else if (result is Task task)
            {
                await task;
                var property = task.GetType().GetProperty("Result");
                if (property != null)
                {
                    response = (TResponse)property.GetValue(task)!;
                }
            }
            else
            {
                response = (TResponse)result!;
            }
        });

        await pipeline();

        _logger.LogDebug("Successfully handled request {RequestType}", requestType.Name);
        return response!;
    }

    /// <inheritdoc />
    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);

        _logger.LogDebug("Sending request {RequestType} without response", requestType.Name);

        var context = new MiddlewareContext(request, requestType, cancellationToken);

        var pipeline = BuildPipeline(context, async () =>
        {
            var handler = _serviceProvider.GetService(handlerType);
            if (handler == null)
            {
                throw new InvalidOperationException($"No handler found for request type {requestType.Name}");
            }

            var handleMethod = handlerType.GetMethod("Handle");
            if (handleMethod == null)
            {
                throw new InvalidOperationException($"Handle method not found on handler for {requestType.Name}");
            }

            var result = handleMethod.Invoke(handler, new object[] { request, cancellationToken });
            if (result is Task task)
            {
                await task;
            }
        });

        await pipeline();

        _logger.LogDebug("Successfully handled request {RequestType}", requestType.Name);
    }

    /// <inheritdoc />
    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (notification == null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        var notificationType = typeof(TNotification);
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);

        _logger.LogDebug("Publishing notification {NotificationType}", notificationType.Name);

        var handlers = _serviceProvider.GetServices(handlerType);
        var tasks = new List<Task>();

        foreach (var handler in handlers)
        {
            var handleMethod = handlerType.GetMethod("Handle");
            if (handleMethod != null)
            {
                var result = handleMethod.Invoke(handler, new object[] { notification, cancellationToken });
                if (result is Task task)
                {
                    tasks.Add(task);
                }
            }
        }

        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
            _logger.LogDebug("Successfully published notification {NotificationType} to {HandlerCount} handlers", 
                notificationType.Name, tasks.Count);
        }
        else
        {
            _logger.LogDebug("No handlers found for notification {NotificationType}", notificationType.Name);
        }
    }

    private RequestDelegate BuildPipeline(IMiddlewareContext context, RequestDelegate innerHandler)
    {
        var pipeline = innerHandler;

        // Build pipeline in reverse order so middlewares execute in the correct order
        foreach (var middleware in _middlewares.Reverse())
        {
            var currentPipeline = pipeline;
            pipeline = () => middleware.InvokeAsync(context, currentPipeline);
        }

        return pipeline;
    }
}
