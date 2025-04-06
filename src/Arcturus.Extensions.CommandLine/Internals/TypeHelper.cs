using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Arcturus.CommandLine.Internals;

internal static class TypeHelper
{
    internal static bool TryGetMethod(this Type type, string name, [NotNullWhen(true)] out MethodInfo? method)
    {
        method = type.GetMethod(name);
        return method is not null;
    }
    internal static bool IsNullable(Type type)
    {
        return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
    }
    internal static bool IsEnumOrNullableEnum(Type type)
    {
        return type.IsEnum || (Nullable.GetUnderlyingType(type)?.IsEnum ?? false);
    }
    internal static Type GetEnumType(Type type)
    {
        return type.IsEnum ? type : Nullable.GetUnderlyingType(type)!;
    }
}