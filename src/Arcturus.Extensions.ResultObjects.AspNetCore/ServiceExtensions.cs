using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
        services.AddSingleton<ProblemDetailsFactory, ArcturusAspNetCoreProblemDetailsFactory>();
        //services.AddProblemDetails(configureProblemDetailsOptions);

        services.Configure<ApiBehaviorOptions>(options =>
        {
            var mappings = new Dictionary<int, (string Title, string Link)>
            {
                [400] = ("Bad Request", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"),
                [401] = ("Unauthorized", "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1"),
                [403] = ("Forbidden", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3"),
                [404] = ("Not Found", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4"),
                [405] = ("Method Not Allowed", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.5"),
                [406] = ("Not Acceptable", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.6"),
                [409] = ("Conflict", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8"),
                [415] = ("Unsupported Media Type", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.13"),
                [422] = ("Unprocessable Entity", "https://datatracker.ietf.org/doc/html/rfc4918#section-11.2"),
                [429] = ("Too Many Requests", "https://datatracker.ietf.org/doc/html/rfc6585#section-4"),
                [500] = ("Internal Server Error", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"),
                [501] = ("Not Implemented", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.2"),
                [502] = ("Bad Gateway", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.3"),
                [503] = ("Service Unavailable", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.4"),
                [504] = ("Gateway Timeout", "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.5")
            };
            foreach (var (statusCode, data) in mappings)
            {
                options.ClientErrorMapping[statusCode] = new ClientErrorData
                {
                    Title = data.Title,
                    Link = data.Link
                };
            }
        });
        return services;
    }
}