using Arcturus.AspNetCore.StandardResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.Extensions.ResultObjects.AspNetCore;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds services required for result object handling to the specified <see cref="IServiceCollection"/>, with
    /// optional configuration to apply JSON naming policies to problem details.
    /// </summary>
    /// <remarks>If the <see cref="IProblemDetailsService"/> is not already registered, this method adds it to
    /// the service collection. When <paramref name="applyJsonNamingPolicyOnProblemDetails"/> is set to <see
    /// langword="true"/>, the method configures JSON serialization to respect custom naming policies for problem
    /// details, which may affect how error responses are formatted in API output.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the result object services will be added. Cannot be null.</param>
    /// <param name="applyJsonNamingPolicyOnProblemDetails">A value indicating whether to apply JSON naming policies to problem details. If <see langword="true"/>, the JSON
    /// serialization options are configured to honor these policies. NB! Make sure the Json configuration comes AFTER.</param>
    /// <param name="registerProblemDetails">Optional. Registers ProblemDetailsService using AddProblemDetails().</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance with result object services registered.</returns>
    public static IServiceCollection AddResultObjects(
        this IServiceCollection services
        , bool applyJsonNamingPolicyOnProblemDetails = false
        , bool registerProblemDetails = true)
    {
        if (!services.Any(_ => _.ServiceType == typeof(IStandardResponseFactory)))
        {
            services.AddStandardResponseHandling(applyJsonNamingPolicyOnProblemDetails, registerProblemDetails);
        }

        return services;
    }
}