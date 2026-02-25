using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Arcturus.Validation;

public class ValidateParametersFilter : IEndpointFilter
{
    private static readonly Dictionary<Type, MethodInfo?> _validationMethodCache = new();
    private static readonly object _cacheLock = new();

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
        {
            // Validate all arguments in the endpoint
            foreach (var argument in context.Arguments)
            {
                if (argument is null)
                    continue;

                var argumentType = argument.GetType();

                // Skip primitive types and common framework types
                if (IsPrimitiveOrFrameworkType(argumentType))
                    continue;

                // Get or cache the validation method
                var tryValidateMethod = GetValidationMethod(argumentType);

                if (tryValidateMethod is not null)
                {
                    var parameters = new object?[] { argument, null };
                    var isValid = (bool)tryValidateMethod.Invoke(null, parameters)!;

                    if (!isValid)
                    {
                        var errors = (Dictionary<string, string[]>)parameters[1]!;
                        return Results.ValidationProblem(errors);
                    }
                }
            }
        }

        return await next(context);
    }

    private static MethodInfo? GetValidationMethod(Type argumentType)
    {
        lock (_cacheLock)
        {
            if (_validationMethodCache.TryGetValue(argumentType, out var cachedMethod))
                return cachedMethod;

            // Look for the generated ValidationExtensions class
            var validationExtensionsType = argumentType.Assembly.GetTypes()
                .FirstOrDefault(t => t.Name == "ValidationExtensions" && t.IsClass && !t.IsPublic);

            MethodInfo? method = null;
            if (validationExtensionsType is not null)
            {
                // Find the TryValidate method for this argument type
                method = validationExtensionsType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                    .FirstOrDefault(m => 
                        m.Name == "TryValidate" && 
                        m.GetParameters().Length == 2 &&
                        m.GetParameters()[0].ParameterType == argumentType);
            }

            _validationMethodCache[argumentType] = method;
            return method;
        }
    }

    private static bool IsPrimitiveOrFrameworkType(Type type)
    {
        return type.IsPrimitive 
            || type == typeof(string) 
            || type == typeof(decimal) 
            || type == typeof(DateTime) 
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(Guid)
            || type.Namespace?.StartsWith("Microsoft.") == true
            || type.Namespace?.StartsWith("System.") == true && type.Namespace != "System.ComponentModel.DataAnnotations";
    }
}
