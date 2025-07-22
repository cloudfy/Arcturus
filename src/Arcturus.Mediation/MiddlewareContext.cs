using Arcturus.Mediation.Abstracts;

namespace Arcturus.Mediation;

/// <summary>
/// Implementation of the middleware context.
/// </summary>
internal class MiddlewareContext : IMiddlewareContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareContext"/> class.
    /// </summary>
    /// <param name="request">The current request.</param>
    /// <param name="requestType">The type of the request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public MiddlewareContext(object request, Type requestType, CancellationToken cancellationToken)
    {
        Request = request;
        RequestType = requestType;
        CancellationToken = cancellationToken;
        Items = new Dictionary<string, object>();
    }

    /// <inheritdoc />
    public object Request { get; }

    /// <inheritdoc />
    public Type RequestType { get; }

    /// <inheritdoc />
    public CancellationToken CancellationToken { get; }

    /// <inheritdoc />
    public IDictionary<string, object> Items { get; }
}
