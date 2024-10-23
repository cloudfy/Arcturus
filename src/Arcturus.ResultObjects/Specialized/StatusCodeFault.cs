using System.Net;

namespace Arcturus.ResultObjects.Specialized;

/// <summary>
/// Provides a generic fault object with a <paramref name="StatusCode"/>.
/// </summary>
/// <param name="Code">Optional code.</param>
/// <param name="Message">A string message indicating the fault.</param>
/// <param name="StatusCode">Http status code.</param>
public record StatusCodeFault(string Code, string Message, HttpStatusCode StatusCode)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => StatusCode;
}