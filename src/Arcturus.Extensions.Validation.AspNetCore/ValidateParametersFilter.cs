using Microsoft.AspNetCore.Http;

namespace Arcturus.Validation;

public class ValidateParametersFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
        {
            // validate using the code generated context
        }
        return await next(context);
    }
}
