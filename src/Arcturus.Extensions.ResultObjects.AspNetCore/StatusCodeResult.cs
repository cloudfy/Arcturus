using Microsoft.AspNetCore.Http;

namespace Arcturus.ResultObjects;

public class StatusCodeResult : IResult
{
    private readonly Result _result;

    internal StatusCodeResult(Result result) 
    {
        _result = result;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        throw new NotImplementedException();
    }
}
