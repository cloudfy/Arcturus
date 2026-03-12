namespace Arcturus.EventBus;

/// <summary>
/// Represents an exception that is thrown when an event cannot be processed due to invalid or unprocessable content.
/// </summary>
/// <remarks>This exception indicates that the event was received but could not be handled by the system. It is
/// typically used to signal unrecoverable issues with the event data, such as schema violations or unsupported event
/// types. This exception is intended for internal use and is not expected to be caught or handled by external
/// callers.</remarks>
public sealed class UnprocessableEventException : Exception
{
    internal UnprocessableEventException(string message) : base(message) { }
}
