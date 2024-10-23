using System.Net;

namespace Arcturus.ResultObjects.Specialized;

/// <summary>
/// Provide a problem fault object (result in HTTP 500).
/// </summary>
/// <param name="Code">Optional code.</param>
/// <param name="Message">A string message indicating the fault.</param>
public record ProblemFault(string Code, string Message)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => HttpStatusCode.InternalServerError;
}
