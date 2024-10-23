using System.Net;

namespace Arcturus.ResultObjects.Specialized;

/// <summary>
/// Provides a forbidden fault (result in HTTP 403).
/// </summary>
/// <param name="Code">Optional code.</param>
/// <param name="Message">A string message indicating the fault.</param>
public record ForbiddenFault(string Code, string Message)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => HttpStatusCode.Forbidden;
}