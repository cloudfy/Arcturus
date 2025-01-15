using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace Arcturus.Extensions.Patchable.AspNetCore;

internal sealed class PatchRequestValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.Arguments.Count >= 1 && context.Arguments[0] is IPatchRequest patchRequest)
        {
            Dictionary<string, string[]> modelValidation = [];

            // TODO: Cache the properties
            var properties = patchRequest.Type.GetProperties();

            foreach (var property in patchRequest)
            {
                var propertyInfo = properties
                    .FirstOrDefault(p => p.Name.Equals(property.Key, StringComparison.OrdinalIgnoreCase));
                if (propertyInfo is null)
                {
                    modelValidation.Add(property.Key, ["Property is not known"]);
                }
                if (property.Value is null && IsPropertyNullable(propertyInfo!) == false) 
                {
                    modelValidation.Add(property.Key, [$"Property '{propertyInfo!.Name}' does not allow null."]);
                }
            }
            if (modelValidation.Count > 0)
                return Results.ValidationProblem(modelValidation);
        }
        
        return await next(context);
    }
    private static bool IsPropertyNullable(PropertyInfo propertyInfo)
    {
        // Check if the property type is Nullable<T>
        var type = propertyInfo.PropertyType;
        if (Nullable.GetUnderlyingType(type) != null)
        {
            return true;
        }

        // For reference types with Nullable Reference Types (NRT) enabled
        if (!type.IsValueType)
        {
            var nullabilityContext = new NullabilityInfoContext();
            var nullabilityInfo = nullabilityContext.Create(propertyInfo);
            return nullabilityInfo.ReadState == NullabilityState.Nullable;
        }

        // Not nullable
        return false;
    }
}