using Microsoft.AspNetCore.Mvc;

namespace Arcturus.ResultObjects;

public sealed class StatusCodeActionResult : IActionResult
{
    private readonly Result _result;

    internal StatusCodeActionResult(Result result) 
    {
        _result = result;
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        throw new NotImplementedException();
    }
}