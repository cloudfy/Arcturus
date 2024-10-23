using System.Net;

namespace Arcturus.ResultObjects.Specialized;

public record StatusCodeFault(string Code, string Message, HttpStatusCode StatusCode)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => StatusCode;
}