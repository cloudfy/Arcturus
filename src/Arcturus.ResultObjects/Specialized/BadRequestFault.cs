using System.Net;

namespace Arcturus.ResultObjects.Specialized;

/// <summary>
/// Provides a fault for a bad request (result in HTTP 400).
/// </summary>
/// <param name="Code">Optional code.</param>
/// <param name="Message">A string message indicating the fault.</param>
public record BadRequestFault(string Code, string Message)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}
