using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Arcturus.Extensions.Patchable.AspNetCore;

public static class Extensions
{
    public static RouteHandlerBuilder UsePatchRequestValidation(this RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter<PatchRequestValidationFilter>();
        return builder;
    }
}