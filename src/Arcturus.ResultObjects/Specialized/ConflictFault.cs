using System.Net;

namespace Arcturus.ResultObjects.Specialized;

/// <summary>
/// Provides a fault for a conflict (result in HTTP 409).
/// </summary>
/// <param name="Code">Optional code.</param>
/// <param name="Message">A string message indicating the fault.</param>
public record ConflictFault(string Code, string Message)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => HttpStatusCode.Conflict;
}
