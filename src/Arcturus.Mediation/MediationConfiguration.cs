using Arcturus.Mediation.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arcturus.Mediation;

/// <summary>
/// Configuration builder for mediation services.
/// </summary>
public class MediationConfiguration
{
    private readonly List<Assembly> _assemblies = [];
    private readonly List<Type> _middlewareTypes = [];

    /// <summary>
    /// Registers handlers from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for handlers.</param>
    /// <returns>The current configuration instance for chaining.</returns>
    public MediationConfiguration RegisterHandlersFromAssembly(Assembly assembly)
    {
        if (assembly == null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        _assemblies.Add(assembly);
        return this;
    }

    /// <summary>
    /// Registers handlers from the assemblies containing the specified types.
    /// </summary>
    /// <param name="types">The types whose assemblies should be scanned for handlers.</param>
    /// <returns>The current configuration instance for chaining.</returns>
    public MediationConfiguration RegisterHandlersFromAssemblyContaining(params Type[] types)
    {
        if (types == null)
        {
            throw new ArgumentNullException(nameof(types));
        }

        foreach (var type in types)
        {
            if (type?.Assembly != null && !_assemblies.Contains(type.Assembly))
            {
                _assemblies.Add(type.Assembly);
            }
        }

        return this;
    }
    /// <summary>
    /// Registers all handlers found in the assembly containing the specified type.
    /// </summary>
    /// <typeparam name="T">The type whose containing assembly will be scanned for handlers.</typeparam>
    /// <returns>The current <see cref="MediationConfiguration"/> instance, allowing for method chaining.</returns>
    public MediationConfiguration RegisterHandlersFromAssemblyContaining<T>()
        => RegisterHandlersFromAssemblyContaining(typeof(T));

    /// <summary>
    /// Adds a middleware to the pipeline.
    /// </summary>
    /// <typeparam name="TMiddleware">The type of middleware to add.</typeparam>
    /// <returns>The current configuration instance for chaining.</returns>
    public MediationConfiguration AddMiddleware<TMiddleware>()
        where TMiddleware : class, IMiddleware
    {
        _middlewareTypes.Add(typeof(TMiddleware));
        return this;
    }

    /// <summary>
    /// Adds a middleware to the pipeline.
    /// </summary>
    /// <param name="middlewareType">The type of middleware to add.</param>
    /// <returns>The current configuration instance for chaining.</returns>
    public MediationConfiguration AddMiddleware(Type middlewareType)
    {
        if (middlewareType == null)
        {
            throw new ArgumentNullException(nameof(middlewareType));
        }

        if (!typeof(IMiddleware).IsAssignableFrom(middlewareType))
        {
            throw new ArgumentException($"Type {middlewareType.Name} must implement {nameof(IMiddleware)}", nameof(middlewareType));
        }

        _middlewareTypes.Add(middlewareType);
        return this;
    }

    /// <summary>
    /// Gets the assemblies to scan for handlers.
    /// </summary>
    internal IReadOnlyCollection<Assembly> Assemblies => _assemblies.AsReadOnly();

    /// <summary>
    /// Gets the middleware types to register.
    /// </summary>
    internal IReadOnlyCollection<Type> MiddlewareTypes => _middlewareTypes.AsReadOnly();

    /// <summary>
    /// Gets or sets the service lifetime, which determines the lifetime of the service instance.
    /// <para>
    /// Default <see cref="ServiceLifetime.Scoped" />.
    /// </para>
    /// </summary>
    public ServiceLifetime LifeTime { get; set; } = ServiceLifetime.Scoped;
}
