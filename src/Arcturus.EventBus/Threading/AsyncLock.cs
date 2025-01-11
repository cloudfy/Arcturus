namespace Arcturus.EventBus.Threading;

/// <summary>
/// Provides an asynchronous lock mechanism to ensure that only one thread can access a resource at a time.
/// </summary>
public sealed class AsyncLock
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Asynchronously waits to enter the lock. Returns an <see cref="IDisposable"/> that releases the lock when disposed.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the lock.</param>
    /// <returns>An <see cref="IDisposable"/> that releases the lock when disposed.</returns>
    public async Task<IDisposable> LockAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        return new Releaser(_semaphore);
    }

    /// <summary>
    /// A helper class that releases the semaphore when disposed.
    /// </summary>
    private sealed class Releaser : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;

        internal Releaser(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        /// <summary>
        /// Releases the semaphore.
        /// </summary>
        public void Dispose()
        {
            _semaphore.Release();
        }
    }
}
