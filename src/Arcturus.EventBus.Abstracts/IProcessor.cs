namespace Arcturus.EventBus.Abstracts;

public interface IProcessor
{
    /// <summary>
    /// Event triggered when a message is processed asynchronously.
    /// </summary>
    event Func<IEventMessage, OnProcessEventArgs?, Task> OnProcessAsync;

    /// <summary>
    /// Waits for events to be processed.
    /// </summary>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task WaitForEvents(
        CancellationToken cancellationToken = default);
}
