using System.Net;

namespace Arcturus.ResultObjects.Specialized;

public record BadRequestFault(string Code, string Message)
    : Fault(Code, Message), ISpecializedFault
{
    public HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
}
