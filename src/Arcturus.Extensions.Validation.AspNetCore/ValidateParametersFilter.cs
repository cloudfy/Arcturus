using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Arcturus.Validation;

// This is a marker class - the actual implementation is generated in the consuming project
// The source generator will create a complete implementation with the actual validation logic
public class ValidateParametersFilter : IEndpointFilter
{
    // This will be overridden by the generated class in the consuming project
    public virtual async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Default implementation - no validation
        // The generated class will override this
        return await next(context);
    }
}
