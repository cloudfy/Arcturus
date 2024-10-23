using System.Net;

namespace Arcturus.ResultObjects.Specialized;

/// <summary>
/// Provides a not found fault (result in HTTP 404).
/// </summary>
/// <param name="Code">Optional code.</param>
/// <param name="Message">A string message indicating the fault.</param>
public record NotFoundFault(string Code, string Message)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => HttpStatusCode.NotFound;
}
