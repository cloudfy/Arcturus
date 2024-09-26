using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.Extensions.ResultObjects.AspNetCore;

public static class ServiceExtensions
{
    /// <summary>
    /// Registers <see cref="Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory"/> for use with ResultObjects.
    /// </summary>
    /// <remarks>
    /// You can use the default <code>services.AddProblemDetails();</code> approach or use this.
    /// </remarks>
    /// <param name="services">Required.</param>
    /// <param name="configureProblemDetailsOptions">Optional. Configure options for <see cref="Microsoft.AspNetCore.Http.ProblemDetailsOptions"/>.</param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection UseResultObjects(
        this IServiceCollection services
        , Action<Microsoft.AspNetCore.Http.ProblemDetailsOptions>? configureProblemDetailsOptions = null)
    {
        services.AddProblemDetails(configureProblemDetailsOptions);
        return services;
    }
}