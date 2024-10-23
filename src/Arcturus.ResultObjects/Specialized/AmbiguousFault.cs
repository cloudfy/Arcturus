using System.Net;

namespace Arcturus.ResultObjects.Specialized;

/// <summary>
/// 
/// </summary>
/// <param name="Code"></param>
/// <param name="Message"></param>
public record AmbiguousFault(string Code, string Message)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => HttpStatusCode.Ambiguous;
}
