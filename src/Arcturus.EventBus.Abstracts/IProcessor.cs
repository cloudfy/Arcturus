namespace Arcturus.EventBus.Abstracts;

/// <summary>
/// Abstract interface to processing events.
/// </summary>
public interface IProcessor
{
    /// <summary>
    /// Event triggered when a message is processed asynchronously.
    /// </summary>
    event Func<IEventMessage, OnProcessEventArgs?, Task> OnProcessAsync;

    /// <summary>
    /// Waits for events to be processed. This method should be called to start the event processing loop and will continue to run until the provided cancellation token is triggered.
    /// </summary>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// It is recommended to start this using a long-running task, such as:
    /// <code>
    /// Task.Run(() => processor.WaitForEvents(cancellationToken), TaskCreationOptions.LongRunning);
    /// </code>
    /// </remarks>
    Task WaitForEvents(
        CancellationToken cancellationToken = default);
}
