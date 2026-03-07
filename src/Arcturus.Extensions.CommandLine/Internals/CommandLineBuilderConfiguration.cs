using System.CommandLine.Builder;

namespace Arcturus.Extensions.CommandLine.Internals;

internal class CommandLineBuilderConfiguration
{
    private readonly List<Action<CommandLineBuilder>> _configurations = [];
    private readonly List<Type> _middlewareTypes = [];

    internal void AddConfiguration(Action<CommandLineBuilder> configure)
    {
        _configurations.Add(configure);
    }

    internal void Apply(CommandLineBuilder builder)
    {
        foreach (var configure in _configurations)
        {
            configure(builder);
        }
    }

    internal void AddMiddleware(Type middlewareType)
    {
        _middlewareTypes.Add(middlewareType);
    }

    internal IEnumerable<Type> GetMiddlewareTypes() => _middlewareTypes;
}
