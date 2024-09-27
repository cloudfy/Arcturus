using Microsoft.AspNetCore.Mvc;

namespace Arcturus.AspNetCore.Endpoints;

/// <summary>
/// A base class for an endpoint that accepts parameters.
/// </summary>
public static class EndpointsBuilder
{
    public static class WithRequest<TRequest>
    {
        // async

        public abstract class WithResultAsync<TResponse> : AbstractEndpoint
        {
            public abstract Task<TResponse> HandleAsync(
                TRequest request,
                CancellationToken cancellationToken = default
            );
        }

        public abstract class WithoutResultAsync : AbstractEndpoint
        {
            public abstract Task HandleAsync(
                TRequest request,
                CancellationToken cancellationToken = default
            );
        }

        public abstract class WithActionResultAsync<TResponse> : AbstractEndpoint
        {
            public abstract Task<ActionResult<TResponse>> HandleAsync(
                TRequest request,
                CancellationToken cancellationToken = default
            );
        }

        public abstract class WithActionResultAsync : AbstractEndpoint
        {
            public abstract Task<IActionResult> HandleAsync(
                TRequest request,
                CancellationToken cancellationToken = default
            );
        }
        public abstract class WithAsyncEnumerableResult<T> : AbstractEndpoint
        {
            public abstract IAsyncEnumerable<T> HandleAsync(
              TRequest request,
              CancellationToken cancellationToken = default
            );
        }

        // sync

        public abstract class WithResult<TResponse> : AbstractEndpoint
        {
            public abstract TResponse Handle(TRequest request);
        }

        public abstract class WithoutResult : AbstractEndpoint
        {
            public abstract void Handle(TRequest request);
        }

        public abstract class WithActionResult<TResponse> : AbstractEndpoint
        {
            public abstract ActionResult<TResponse> Handle(TRequest request);
        }

        public abstract class WithActionResult : AbstractEndpoint
        {
            public abstract IActionResult Handle(TRequest request);
        }
    }

    public static class WithoutRequest
    {
        // async

        public abstract class WithResultAsync<TResponse> : AbstractEndpoint
        {
            public abstract Task<TResponse> HandleAsync(
                CancellationToken cancellationToken = default
            );
        }

        public abstract class WithoutResultAsync : AbstractEndpoint
        {
            public abstract Task HandleAsync(
                CancellationToken cancellationToken = default
            );
        }

        public abstract class WithActionResultAsync<TResponse> : AbstractEndpoint
        {
            public abstract Task<ActionResult<TResponse>> HandleAsync(
                CancellationToken cancellationToken = default
            );
        }

        public abstract class WithActionResultAsync : AbstractEndpoint
        {
            public abstract Task<IActionResult> HandleAsync(
                CancellationToken cancellationToken = default
            );
        }

        public abstract class WithAsyncEnumerableResult<T> : AbstractEndpoint
        {
            public abstract IAsyncEnumerable<T> HandleAsync(
              CancellationToken cancellationToken = default
            );
        }

        // sync

        public abstract class WithResult<TResponse> : AbstractEndpoint
        {
            public abstract TResponse Handle();
        }

        public abstract class WithoutResult : AbstractEndpoint
        {
            public abstract void Handle();
        }

        public abstract class WithActionResult<TResponse> : AbstractEndpoint
        {
            public abstract ActionResult<TResponse> Handle();
        }

        public abstract class WithActionResult : AbstractEndpoint
        {
            public abstract ActionResult Handle();
        }
    }
}