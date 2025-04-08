using Microsoft.AspNetCore.Mvc;

namespace Arcturus.AspNetCore.Endpoints;


/// <summary>
/// Provides a builder for defining various types of endpoints with or without requests.
/// <para>
/// Appends the <see cref="AbstractEndpoint"/>.
/// </para>
/// </summary>
public static class EndpointsBuilder
{
    /// <summary>
    /// Contains endpoint definitions that require a request of type <typeparamref name="TRequest"/>.
    /// </summary>
    public static class WithRequest<TRequest>
    {
        /// <summary>
        /// Represents an asynchronous endpoint that returns a result of type <typeparamref name="TResponse"/>.
        /// </summary>
        public abstract class WithResultAsync<TResponse> : AbstractEndpoint
        {
            /// <summary>
            /// Handles the request asynchronously and returns a result of type <typeparamref name="TResponse"/>.
            /// </summary>
            /// <param name="request">The request object.</param>
            /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
            /// <returns>A task that represents the asynchronous operation, containing the result.</returns>
            public abstract Task<TResponse> HandleAsync(
                TRequest request,
                CancellationToken cancellationToken = default
            );
        }

        /// <summary>
        /// Represents an asynchronous endpoint that does not return a result.
        /// </summary>
        public abstract class WithoutResultAsync : AbstractEndpoint
        {
            /// <summary>
            /// Handles the request asynchronously.
            /// </summary>
            /// <param name="request">The request object.</param>
            /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
            /// <returns>A task that represents the asynchronous operation.</returns>
            public abstract Task HandleAsync(
                TRequest request,
                CancellationToken cancellationToken = default
            );
        }

        /// <summary>
        /// Represents an asynchronous endpoint that returns an <see cref="ActionResult{TResponse}"/>.
        /// </summary>
        public abstract class WithActionResultAsync<TResponse> : AbstractEndpoint
        {
            /// <summary>
            /// Handles the request asynchronously and returns an <see cref="ActionResult{TResponse}"/>.
            /// </summary>
            /// <param name="request">The request object.</param>
            /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
            /// <returns>A task that represents the asynchronous operation, containing the action result.</returns>
            public abstract Task<ActionResult<TResponse>> HandleAsync(
                TRequest request,
                CancellationToken cancellationToken = default
            );
        }

        /// <summary>
        /// Represents an asynchronous endpoint that returns an <see cref="IActionResult"/>.
        /// </summary>
        public abstract class WithActionResultAsync : AbstractEndpoint
        {
            /// <summary>
            /// Handles the request asynchronously and returns an <see cref="IActionResult"/>.
            /// </summary>
            /// <param name="request">The request object.</param>
            /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
            /// <returns>A task that represents the asynchronous operation, containing the action result.</returns>
            public abstract Task<IActionResult> HandleAsync(
                TRequest request,
                CancellationToken cancellationToken = default
            );
        }

        /// <summary>
        /// Represents an asynchronous endpoint that returns an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        public abstract class WithAsyncEnumerableResult<T> : AbstractEndpoint
        {
            /// <summary>
            /// Handles the request asynchronously and returns an <see cref="IAsyncEnumerable{T}"/>.
            /// </summary>
            /// <param name="request">The request object.</param>
            /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
            /// <returns>An asynchronous enumerable containing the results.</returns>
            public abstract IAsyncEnumerable<T> HandleAsync(
              TRequest request,
              CancellationToken cancellationToken = default
            );
        }

        // sync

        /// <summary>
        /// Represents a synchronous endpoint that returns a result of type <typeparamref name="TResponse"/>.
        /// </summary>
        public abstract class WithResult<TResponse> : AbstractEndpoint
        {
            /// <summary>
            /// Handles the request and returns a result of type <typeparamref name="TResponse"/>.
            /// </summary>
            /// <param name="request">The request object.</param>
            /// <returns>The result of the operation.</returns>
            public abstract TResponse Handle(TRequest request);
        }

        /// <summary>
        /// Represents a synchronous endpoint that does not return a result.
        /// </summary>
        public abstract class WithoutResult : AbstractEndpoint
        {
            /// <summary>
            /// Handles the request.
            /// </summary>
            /// <param name="request">The request object.</param>
            public abstract void Handle(TRequest request);
        }

        /// <summary>
        /// Represents a synchronous endpoint that returns an <see cref="ActionResult{TResponse}"/>.
        /// </summary>
        public abstract class WithActionResult<TResponse> : AbstractEndpoint
        {
            /// <summary>
            /// Handles the request and returns an <see cref="ActionResult{TResponse}"/>.
            /// </summary>
            /// <param name="request">The request object.</param>
            /// <returns>The action result of the operation.</returns>
            public abstract ActionResult<TResponse> Handle(TRequest request);
        }

        /// <summary>
        /// Represents a synchronous endpoint that returns an <see cref="IActionResult"/>.
        /// </summary>
        public abstract class WithActionResult : AbstractEndpoint
        {
            /// <summary>
            /// Handles the request and returns an <see cref="IActionResult"/>.
            /// </summary>
            /// <param name="request">The request object.</param>
            /// <returns>The action result of the operation.</returns>
            public abstract IActionResult Handle(TRequest request);
        }
    }

    /// <summary>
    /// Contains endpoint definitions that do not require a request.
    /// </summary>
    public static class WithoutRequest
    {
        // async

        /// <summary>
        /// Represents an asynchronous endpoint that returns a result of type <typeparamref name="TResponse"/>.
        /// </summary>
        public abstract class WithResultAsync<TResponse> : AbstractEndpoint
        {
            /// <summary>
            /// Handles the operation asynchronously and returns a result of type <typeparamref name="TResponse"/>.
            /// </summary>
            /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
            /// <returns>A task that represents the asynchronous operation, containing the result.</returns>
            public abstract Task<TResponse> HandleAsync(
                CancellationToken cancellationToken = default
            );
        }

        /// <summary>
        /// Represents an asynchronous endpoint that does not return a result.
        /// </summary>
        public abstract class WithoutResultAsync : AbstractEndpoint
        {
            /// <summary>
            /// Handles the operation asynchronously.
            /// </summary>
            /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
            /// <returns>A task that represents the asynchronous operation.</returns>
            public abstract Task HandleAsync(
                CancellationToken cancellationToken = default
            );
        }

        /// <summary>
        /// Represents an asynchronous endpoint that returns an <see cref="ActionResult{TResponse}"/>.
        /// </summary>
        public abstract class WithActionResultAsync<TResponse> : AbstractEndpoint
        {
            /// <summary>
            /// Handles the operation asynchronously and returns an <see cref="ActionResult{TResponse}"/>.
            /// </summary>
            /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
            /// <returns>A task that represents the asynchronous operation, containing the action result.</returns>
            public abstract Task<ActionResult<TResponse>> HandleAsync(
                CancellationToken cancellationToken = default
            );
        }

        /// <summary>
        /// Represents an asynchronous endpoint that returns an <see cref="IActionResult"/>.
        /// </summary>
        public abstract class WithActionResultAsync : AbstractEndpoint
        {
            /// <summary>
            /// Handles the operation asynchronously and returns an <see cref="IActionResult"/>.
            /// </summary>
            /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
            /// <returns>A task that represents the asynchronous operation, containing the action result.</returns>
            public abstract Task<IActionResult> HandleAsync(
                CancellationToken cancellationToken = default
            );
        }

        /// <summary>
        /// Represents an asynchronous endpoint that returns an <see cref="IAsyncEnumerable{T}"/>.
        /// </summary>
        public abstract class WithAsyncEnumerableResult<T> : AbstractEndpoint
        {
            /// <summary>
            /// Handles the operation asynchronously and returns an <see cref="IAsyncEnumerable{T}"/>.
            /// </summary>
            /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
            /// <returns>An asynchronous enumerable containing the results.</returns>
            public abstract IAsyncEnumerable<T> HandleAsync(
              CancellationToken cancellationToken = default
            );
        }

        // sync

        /// <summary>
        /// Represents a synchronous endpoint that returns a result of type <typeparamref name="TResponse"/>.
        /// </summary>
        public abstract class WithResult<TResponse> : AbstractEndpoint
        {
            /// <summary>
            /// Handles the operation and returns a result of type <typeparamref name="TResponse"/>.
            /// </summary>
            /// <returns>The result of the operation.</returns>
            public abstract TResponse Handle();
        }

        /// <summary>
        /// Represents a synchronous endpoint that does not return a result.
        /// </summary>
        public abstract class WithoutResult : AbstractEndpoint
        {
            /// <summary>
            /// Handles the operation.
            /// </summary>
            public abstract void Handle();
        }

        /// <summary>
        /// Represents a synchronous endpoint that returns an <see cref="ActionResult{TResponse}"/>.
        /// </summary>
        public abstract class WithActionResult<TResponse> : AbstractEndpoint
        {
            /// <summary>
            /// Handles the operation and returns an <see cref="ActionResult{TResponse}"/>.
            /// </summary>
            /// <returns>The action result of the operation.</returns>
            public abstract ActionResult<TResponse> Handle();
        }

        /// <summary>
        /// Represents a synchronous endpoint that returns an <see cref="ActionResult"/>.
        /// </summary>
        public abstract class WithActionResult : AbstractEndpoint
        {
            /// <summary>
            /// Handles the operation and returns an <see cref="ActionResult"/>.
            /// </summary>
            /// <returns>The action result of the operation.</returns>
            public abstract ActionResult Handle();
        }
    }
}
