using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Arcturus.Validation;

public class ValidateParametersFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
        {
            // Validate all arguments in the endpoint using generated validation method
            var validationResult = ValidationHelper.ValidateArguments(context.Arguments);
            if (validationResult is not null)
            {
                return validationResult;
            }
        }

        return await next(context);
    }
}
