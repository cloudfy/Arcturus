namespace Arcturus.EventBus;

public sealed class UnprocessableEventException : Exception
{
    internal UnprocessableEventException(string message) : base(message) { }
}
