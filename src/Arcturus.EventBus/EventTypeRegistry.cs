namespace Arcturus.EventBus;

// singleton
public sealed class EventTypeRegistry
{
    private readonly Lazy<EventTypeEntry[]> _registryList;

    internal EventTypeRegistry(IReadOnlyCollection<Assembly> assemblies)
    {
        _registryList = new Lazy<EventTypeEntry[]>(() => {
            List<EventTypeEntry> _list = [];

            foreach (var assembly in assemblies.Distinct()) 
            {
                foreach (var eventMessageType in Internals.AssemblyExtensions.GetEventMessageTypesFromAssembly(assembly))
                {
                    _list.Add(new EventTypeEntry(eventMessageType.Item1, eventMessageType.Item2));
                }
            }

            return [.. _list.DistinctBy(_ => _.Name)];
        });
    }

    public Type? GetTypeByName(string Name)
    {
        if (_registryList.Value.FirstOrDefault(_ => _.Name == Name) is EventTypeEntry entry)
        {
            return entry.Type;
        }
        return null;
    }
    public string? GetNameByType(Type type)
    {
        if (_registryList.Value.FirstOrDefault(_ => _.Type == type) is EventTypeEntry entry)
        {
            return entry.Name;
        }
        return null;
    }
    private record EventTypeEntry(string Name, Type Type);
}