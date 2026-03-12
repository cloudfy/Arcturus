using Arcturus.CommandLine.Abstractions;
using System.Reflection;

namespace Arcturus.CommandLine.Internals;

internal static class AbstractExtensions
{
    internal static IEnumerable<SubCommandAttribute> GetSubCommandAttributes<T>(
        this T abstractCommand) where T : IAbstractCommand
    {
        return abstractCommand.GetType().GetCustomAttributes<SubCommandAttribute>();
    }

    internal static IEnumerable<(OptionAttribute Option, PropertyInfo PropertyInfo)> GetOptions<T>(
        this T abstractCommand) where T : ICommand
    {
        return abstractCommand.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Select(_ => (Option: _.GetCustomAttribute<OptionAttribute>()!, PropertyInfo: _!))
            .Where(_ => _.Option is not null);
    }

    internal static bool IsRequiredProperty(this PropertyInfo propertyInfo)
    {
        return !TypeExtensions.IsNullable(propertyInfo.PropertyType) ||
                            TypeExtensions.IsRequired(propertyInfo);
    }
}
