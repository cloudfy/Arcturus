using Arcturus.EventBus.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Arcturus.EventBus.AzureServiceBus;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds Arcturus.EventBus implementation using Azure ServiceBus to the service collection.
    /// </summary>
    /// <param name="services">Required. Service collection.</param>
    /// <param name="options">Optional. Options to configure.</param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddAzureServiceBusEventBus(
        this IServiceCollection services
        , Action<ServiceBusOptions>? options = null)
    {
        var currentOptions = new ServiceBusOptions();
        if (options is not null)
        {
            options(currentOptions);
        }

        services.AddSingleton<IConnection, ServiceBusConnection>(
            (sp) => { return new ServiceBusConnection(currentOptions); });
        services.AddSingleton<IEventBusFactory, ServiceBusFactory>();
        services.AddSingleton<ServiceBusOptions>(currentOptions);

        return services;
    }
}