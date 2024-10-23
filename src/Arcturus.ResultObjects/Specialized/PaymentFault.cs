using System.Net;

namespace Arcturus.ResultObjects.Specialized;

/// <summary>
/// Provides a payment required fault (result in HTTP 402).
/// </summary>
/// <param name="Code">Optional code.</param>
/// <param name="Message">A string message indicating the fault.</param>
public record PaymentFault(string Code, string Message)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => HttpStatusCode.PaymentRequired;
}
