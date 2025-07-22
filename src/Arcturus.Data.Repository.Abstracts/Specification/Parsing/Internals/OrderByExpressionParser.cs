using System.Reflection;

namespace Arcturus.Repository.Specification.Parsing.Internals;

internal static class OrderByExpressionParser
{
    internal static (Expression<Func<T, object?>>, bool) ParseOrderBy<T>(string orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            throw new ArgumentException("Order by cannot be null or empty.", nameof(orderBy));

        var parts = orderBy.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            throw new ArgumentException("Order by must contain at least a property name.", nameof(orderBy));
        var propertyPath = parts[0];
        var direction = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? true
            : false;
        return (GetOrderByExpression<T>(propertyPath), direction);
    }

    private static Expression<Func<T, object?>> GetOrderByExpression<T>(string propertyPath)
    {
        if (string.IsNullOrWhiteSpace(propertyPath))
            throw new ArgumentException("Property path cannot be null or empty.", nameof(propertyPath));

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression body = parameter;

        foreach (var member in propertyPath.Split('.'))
        {
            var prop = body.Type.GetProperty(member, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
                throw new ArgumentException($"'{member}' is not a property of '{body.Type.Name}'");
            body = Expression.Property(body, prop);
        }

        if (body.Type.IsValueType)
            body = Expression.Convert(body, typeof(object));

        return Expression.Lambda<Func<T, object?>>(body, parameter);
    }
}
