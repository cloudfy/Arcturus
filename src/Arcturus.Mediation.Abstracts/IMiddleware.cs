namespace Arcturus.Mediation.Abstracts;

/// <summary>
/// Delegate representing the next step in the middleware pipeline.
/// </summary>
/// <returns>A task that represents the asynchronous operation.</returns>
public delegate Task RequestDelegate();

/// <summary>
/// Interface for middleware components that can wrap request handling with custom behaviors.
/// </summary>
public interface IMiddleware
{
    /// <summary>
    /// Executes the middleware logic.
    /// </summary>
    /// <param name="context">The current middleware context.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InvokeAsync(IMiddlewareContext context, RequestDelegate next);
}

/// <summary>
/// Context object passed through the middleware pipeline.
/// </summary>
public interface IMiddlewareContext
{
    /// <summary>
    /// Gets the current request being processed.
    /// </summary>
    object Request { get; }

    /// <summary>
    /// Gets the type of the request being processed.
    /// </summary>
    Type RequestType { get; }

    /// <summary>
    /// Gets the cancellation token for the current operation.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets or sets arbitrary data associated with the current request.
    /// </summary>
    IDictionary<string, object> Items { get; }
}
