using System.Collections.Concurrent;
using System.Reflection;

namespace Arcturus.Repository.Pagination.Internals;

internal static class QueryableExtensions
{
    internal static Expression<Func<T, bool>> KeysetPagePredicate<T>(
        params (string Column, object Value)[] keyset)
    {
        if (keyset.Length == 0)
            return x => true;

        var param = Expression.Parameter(typeof(T), "x");
        Expression? filter = null;

        for (int i = 0; i < keyset.Length; i++)
        {
            // Build: (col1 = v1 AND col2 = v2 AND ...)
            Expression? andExpr = null;
            for (int j = 0; j < i; j++)
            {
                andExpr = andExpr == null
                    ? Equal(param, keyset[j].Column, keyset[j].Value)
                    : Expression.AndAlso(andExpr, Equal(param, keyset[j].Column, keyset[j].Value));
            }

            // For the last key, use GreaterThanOrEqual. For others, use GreaterThan.
            var comparison = (i == keyset.Length - 1)
                ? GreaterThanOrEqual(param, keyset[i].Column, keyset[i].Value)
                : GreaterThan(param, keyset[i].Column, keyset[i].Value);

            var currExpr = andExpr != null
                ? Expression.AndAlso(andExpr, comparison)
                : comparison;

            filter = filter != null
                ? Expression.OrElse(filter, currExpr)
                : currExpr;
        }

        return Expression.Lambda<Func<T, bool>>(filter!, param);


        static Expression ConstantToMemberType(object value, Type memberType)
        {
            if (value == null)
                return Expression.Constant(null, memberType);

            // Handle Nullable<T>
            var underlyingType = Nullable.GetUnderlyingType(memberType);
            var targetType = underlyingType ?? memberType;

            // Handle Guid
            if (targetType == typeof(Guid))
            {
                var guidValue = value is Guid g ? g : Guid.Parse(value.ToString()!);
                return Expression.Constant(guidValue, memberType);
            }

            // Handle DateTime
            if (targetType == typeof(DateTime))
            {
                var dtValue = value is DateTime dt ? dt : DateTime.Parse(value.ToString()!);
                return Expression.Constant(dtValue, memberType);
            }

            // Handle Enum
            if (targetType.IsEnum)
            {
                var enumValue = value is string s
                    ? Enum.Parse(targetType, s)
                    : Enum.ToObject(targetType, value);
                return Expression.Constant(enumValue, memberType);
            }

            // If value already matches the target type
            if (targetType.IsAssignableFrom(value.GetType()))
                return Expression.Constant(value, memberType);

            // Handle primitives (int, double, bool, etc.)
            try
            {
                var converted = Convert.ChangeType(value, targetType);
                return Expression.Constant(converted, memberType);
            }
            catch
            {
                // Last resort: try to parse as string
                return Expression.Constant(Convert.ChangeType(value.ToString(), targetType), memberType);
            }
        }

        static Expression Equal(ParameterExpression param, string propertyName, object value)
        {
            var member = CachedPropertyOrField(param, propertyName); // Expression.PropertyOrField(param, propertyName);
            var constant = ConstantToMemberType(value, member.Type);
            return Expression.Equal(member, constant);
        }

        static Expression GreaterThan(ParameterExpression param, string propertyName, object value)
        {
            var member = CachedPropertyOrField(param, propertyName); // Expression.PropertyOrField(param, propertyName);
            var constant = ConstantToMemberType(value, member.Type);

            if (member.Type == typeof(string))
            {
                // string.Compare(member, constant) > 0
                var compare = Expression.Call(
                    typeof(string).GetMethod(nameof(string.Compare), new[] { typeof(string), typeof(string) })!,
                    member,
                    constant
                );
                return Expression.GreaterThan(compare, Expression.Constant(0));
            }
            else
            {
                return Expression.GreaterThan(member, constant);
            }
        }
        static Expression GreaterThanOrEqual(ParameterExpression param, string propertyName, object value)
        {
            var member = CachedPropertyOrField(param, propertyName); // Expression.PropertyOrField(param, propertyName);
            var constant = ConstantToMemberType(value, member.Type);

            if (member.Type == typeof(string))
            {
                // string.Compare(member, constant) >= 0
                var compare = Expression.Call(
                    typeof(string).GetMethod(nameof(string.Compare), new[] { typeof(string), typeof(string) })!,
                    member,
                    constant
                );
                return Expression.GreaterThanOrEqual(compare, Expression.Constant(0));
            }
            else
            {
                return Expression.GreaterThanOrEqual(member, constant);
            }
        }
    }


    private static readonly ConcurrentDictionary<(Type, string), MemberInfo> _propertyCache = new();

    private static MemberExpression CachedPropertyOrField(ParameterExpression param, string propertyName)
    {
        var key = (param.Type, propertyName);

        var member = _propertyCache.GetOrAdd(key, k =>
        {
            // This will cache either a property or a field
            var prop = k.Item1.GetProperty(k.Item2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (prop != null) return prop;
            var field = k.Item1.GetField(k.Item2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (field != null) return field;
            throw new ArgumentException($"Property or field '{k.Item2}' not found on type '{k.Item1}'.");
        });

        return member is PropertyInfo pi
            ? Expression.Property(param, pi)
            : Expression.Field(param, (FieldInfo)member);
    }
}