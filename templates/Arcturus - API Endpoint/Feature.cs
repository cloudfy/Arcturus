using Arcturus.AspNetCore.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace $rootnamespace$;

public class $safeitemname$Endpoint : EndpointsBuilder
    .WithRequest<$safeitemname$Endpoint.$safeitemname$Request>
    .WithActionResultAsync
{
    [HttpGet("")]
    public override Task<IActionResult> HandleAsync(
        $safeitemname$Request request
        , CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public class $safeitemname$Request
    {
        [FromHeader]
        public required string MyHeader { get; init; }
        [FromQuery]
        public string? MyQuery { get; init; }

    }
}