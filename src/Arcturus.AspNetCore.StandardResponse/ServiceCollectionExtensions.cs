using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;


namespace Arcturus.AspNetCore.StandardResponse;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStandardResponseHandling(
        this IServiceCollection services
        , bool applyJsonNamingPolicyOnProblemDetails = false
        , bool registerProblemDetails = true)
    {
        if (registerProblemDetails &&
            !services.Any(_ => _.ServiceType == typeof(IProblemDetailsService)))
        {
            services.AddProblemDetails();
        }
        if (!services.Any(_ => _.ServiceType == typeof(IStandardResponseFactory)))
        {
            services.AddSingleton<IStandardResponseFactory, StandardResponseFactory>();
        }

        if (applyJsonNamingPolicyOnProblemDetails)
        {
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(
                    new ProblemDetailsHonorJsonResolver(new DefaultJsonTypeInfoResolver())
                    , options.SerializerOptions.TypeInfoResolver);
            });
            services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
                options =>
                {
                    options.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(
                        new ProblemDetailsHonorJsonResolver(new DefaultJsonTypeInfoResolver())
                        , options.SerializerOptions.TypeInfoResolver);
                });
            services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(
                options =>
                {
                    options.JsonSerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(
                        new ProblemDetailsHonorJsonResolver(new DefaultJsonTypeInfoResolver())
                        , options.JsonSerializerOptions.TypeInfoResolver);
                });
        }

        return services;
    }
}
