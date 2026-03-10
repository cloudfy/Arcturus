using Arcturus.EventBus.Abstracts;
using Arcturus.EventBus.Internals;
using System.Reflection.Metadata;

namespace Arcturus.EventBus.Serialization;

/// <summary>
/// Default type resolver using the AppDomain.
/// </summary>
internal sealed class DefaultEventMessageTypeResolver(Assembly[] handlerAssemblies)
{
    private readonly Dictionary<string, Type?> _reflectionCache = [];
    private readonly Assembly[] _handlerAssemblies = handlerAssemblies;

    /// <summary>
    /// Resolves <paramref name="typeName"/> to a <see cref="Type"/>.
    /// </summary>
    /// <param name="typeName">Required. Name of type to resolve.</param>
    /// <returns><see cref="Type"/> or null.</returns>
    /// <exception cref="InvalidOperationException">If more than one event has the same name.</exception>
    internal Type? ResolveType(string typeName)
    {
        ArgumentNullException.ThrowIfNull(typeName, nameof(typeName));

        if (_reflectionCache.TryGetValue(typeName, out var type))
            return type;

        // Ensure all assemblies in the application directory are loaded
        // LoadApplicationAssemblies();

        // TODO: reduce the footprint by configuration (see https://github.com/cloudfy/Arcturus/issues/102)
        var matchingTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypesSafe()) // Use extension method for safe type retrieval
            .Where(t => t != null
                && !t.IsInterface
                && !t.IsAbstract
                && typeof(IEventMessage).IsAssignableFrom(t)
                && t.GetCustomAttribute<EventMessageAttribute>()?.Name == typeName)
            .ToList();

        ApplyHandlerRegistration(matchingTypes, typeName);

        var matchingTypeArray = matchingTypes.ToArray();

        type = matchingTypeArray.Length switch
        {
            0 => null,
            1 => matchingTypeArray[0],
            _ => throw new UnprocessableEventException(
                $"Event named '{typeName}' is duplicated. Only one named event using EventMessageAttribute is allowed.")
        };

        // TODO: implement more advanced type resolution (user delegates etc. from startup configuration)
        // - alternatively allow overriding the type resolver to a custom resolver.
        // - https://github.com/cloudfy/Arcturus/issues/102
        if (type is not null)
        {
            _reflectionCache.Add(typeName, type);
            return type;
        }
        return null;
    }

    private void ApplyHandlerRegistration(List<Type> matchingTypes, string typeName)
    {
        var other = _handlerAssemblies
            .SelectMany(a => a.GetTypesSafe()) // Use extension method for safe type retrieval
            .Where(t => t != null
                && !t.IsInterface
                && !t.IsAbstract
                && typeof(IEventMessage).IsAssignableFrom(t)
                && t.GetCustomAttribute<EventMessageAttribute>()?.Name == typeName)
            .ToList();
        matchingTypes.AddRange(other);
    }

    private static void LoadApplicationAssemblies()
    {
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToHashSet();
        var baseDirectory = AppContext.BaseDirectory;

        foreach (var assemblyFile in Directory.GetFiles(baseDirectory, "*.dll"))
        {
            try
            {
                var assemblyName = AssemblyName.GetAssemblyName(assemblyFile);

                // Skip if already loaded
                if (loadedAssemblies.Any(a => AssemblyName.ReferenceMatchesDefinition(a.GetName(), assemblyName)))
                    continue;

                // Load the assembly into the current AppDomain
                Assembly.Load(assemblyName);
            }
            catch
            {
                // Ignore assemblies that fail to load (native assemblies, incompatible versions, etc.)
            }
        }
    }
}
