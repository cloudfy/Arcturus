using System.Net;

namespace Arcturus.ResultObjects.Specialized;

/// <summary>
/// Provides an unauthorized fault (result in HTTP 401).
/// </summary>
/// <param name="Code">Optional code.</param>
/// <param name="Message">A string message indicating the fault.</param>
public record UnauthorizedFault(string Code, string Message)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => HttpStatusCode.Unauthorized;
}
