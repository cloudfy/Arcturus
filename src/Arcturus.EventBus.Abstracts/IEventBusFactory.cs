namespace Arcturus.EventBus.Abstracts;

/// <summary>
/// Provides methods to creates instances of <see cref="IProcessor"/> and <see cref="IPublisher"/>.
/// </summary>
public interface IEventBusFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IProcessor"/>. Processors are used to process messages from the event bus asynchronously.
    /// </summary>
    /// <param name="queue">Optional. Name of the queue.</param>
    /// <returns><see cref="IProcessor"/> instance.</returns>
    IProcessor CreateProcessor(string? queue = null);
    /// <summary>
    /// Creates a new instance of <see cref="IPublisher"/>. Publishers are used to publish messages to the event bus.
    /// </summary>
    /// <param name="queue">Optional. Name of the queue.</param>
    /// <returns><see cref="IPublisher"/> instance.</returns>
    IPublisher CreatePublisher(string? queue = null);
    //ISubscriber CreateSubscriber();
}
