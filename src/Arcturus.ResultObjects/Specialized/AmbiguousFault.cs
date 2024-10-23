using System.Net;

namespace Arcturus.ResultObjects.Specialized;

/// <summary>
/// Provides a fault for an ambiguous request (result in HTTP 300).
/// </summary>
/// <param name="Code">Optional code.</param>
/// <param name="Message">A string message indicating the fault.</param>
public record AmbiguousFault(string? Code, string Message)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => HttpStatusCode.Ambiguous;
}
