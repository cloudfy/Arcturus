using Arcturus.Repository.Abstracts.Specification.Parsing;

namespace Arcturus.Repository.Specification.Parsing.Internals;

internal static class FilterExpressionParser
{
    internal static Expression<Func<T, bool>> Parse<T>(string[]? filters)
    {
        Expression<Func<T, bool>> result = _ => true;
        if (filters is null || filters.Length == 0)
            return result;

        foreach (var filter in filters)
            result = result.AndAlso(Parse<T>(filter));

        return result;
    }
    private static Expression<Func<T, bool>> Parse<T>(string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
            throw new FilterExpressionException("Filter cannot be null or empty.");

        // Split the filter into parts
        var parts = Tokenize(filter); // filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3)
            throw new FilterExpressionException("Filter must be in the format 'property operator value'.");

        var propertyName = parts[0];
        var operatorSymbol = parts[1];
        var value = parts[2];

        // Get the parameter for the lambda expression
        var parameter = Expression.Parameter(typeof(T), "x");

        // Get the property
        var property = Expression.Property(parameter, propertyName);

        // Parse the value to the correct type
        var propertyType = property.Type;
        var parsedValue = ConvertValue(value, propertyType)
            ?? throw new FilterExpressionException($"Cannot convert value '{value}' to type '{propertyType.Name}'.");

        // Create the constant expression
        var constant = Expression.Constant(parsedValue, propertyType);

        try
        {
            // Create the binary expression based on the operator
            Expression body = operatorSymbol switch
            {
                "eq" => Expression.Equal(property, constant),
                "ne" => Expression.NotEqual(property, constant),
                "lk" => ExpressionLike(property, constant),
                "gt" => CreateComparisonExpression(property, constant, operatorSymbol),
                "ge" => CreateComparisonExpression(property, constant, operatorSymbol),
                "lt" => CreateComparisonExpression(property, constant, operatorSymbol),
                "le" => CreateComparisonExpression(property, constant, operatorSymbol),
                _ => throw new FilterExpressionException($"Operator '{operatorSymbol}' is not supported.")
            };
            // Build the lambda expression
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
        catch (InvalidOperationException)
        {
            throw new FilterExpressionException($"Operator '{operatorSymbol}' is not supported for the property '{propertyName}'.");
        }
        catch
        {
            throw;
        }
    }

    private static Expression CreateComparisonExpression(MemberExpression property, ConstantExpression constant, string operatorSymbol)
    {
        var propertyType = property.Type;
        
        // Handle string comparisons using string.Compare
        if (propertyType == typeof(string))
        {
            var compareMethod = typeof(string).GetMethod(nameof(string.Compare), new[] { typeof(string), typeof(string) });
            var compareCall = Expression.Call(compareMethod, property, constant);
            var constantValue = Expression.Constant(0);
            
            return operatorSymbol switch
            {
                "gt" => Expression.GreaterThan(compareCall, constantValue),
                "ge" => Expression.GreaterThanOrEqual(compareCall, constantValue),
                "lt" => Expression.LessThan(compareCall, constantValue),
                "le" => Expression.LessThanOrEqual(compareCall, constantValue),
                _ => throw new FilterExpressionException($"Operator '{operatorSymbol}' is not supported.")
            };
        }
        
        // Handle numeric and other comparable types using standard operators
        return operatorSymbol switch
        {
            "gt" => Expression.GreaterThan(property, constant),
            "ge" => Expression.GreaterThanOrEqual(property, constant),
            "lt" => Expression.LessThan(property, constant),
            "le" => Expression.LessThanOrEqual(property, constant),
            _ => throw new FilterExpressionException($"Operator '{operatorSymbol}' is not supported.")
        };
    }

    private static MethodCallExpression ExpressionLike(MemberExpression property, ConstantExpression constant)
    {
        // Create the Contains method call
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]);
        var containsCall = Expression.Call(property, containsMethod, constant);


        return containsCall;
    }

    private static string[] Tokenize(string input)
    {
        var tokens = new List<string>();
        var index = 0;

        while (index < input.Length)
        {
            if (char.IsWhiteSpace(input[index]))
            {
                index++;
                continue;
            }

            if (input[index] == '\'')
            {
                var closingQuote = input.IndexOf('\'', index + 1);
                if (closingQuote == -1)
                    throw new ArgumentException("Unmatched quote in input.");

                tokens.Add(input.Substring(index + 1, closingQuote - index - 1));
                index = closingQuote + 1;
            }
            else
            {
                var nextSpace = input.IndexOf(' ', index);
                if (nextSpace == -1)
                {
                    tokens.Add(input[index..]);
                    break;
                }

                tokens.Add(input[index..nextSpace]);
                index = nextSpace;
            }
        }

        if (tokens.Count != 3)
            throw new ArgumentException("Input must consist of exactly three parts: 'property operator value'.");

        return [.. tokens];
    }

    public static Dictionary<Type, Func<string, (bool, object?)>> CustomParsers = [];

    private static object? ConvertValue(string value, Type targetType)
    {
        if (targetType == typeof(string))
        {
            return value;
        }
// Removed commented-out code related to MonetaryAmount and Currency parsing logic.
        else if (targetType == typeof(DateTime) &&
            DateTime.TryParse(value, out var dateTimeValue))
        {
            return dateTimeValue;
        }
        else if (targetType == typeof(DateOnly) &&
            DateOnly.TryParse(value, out var dateValue))
        {
            return dateValue;
        }
        else if (targetType == typeof(int) &&
            int.TryParse(value, out var intValue))
        {
            return intValue;
        }
        else if (targetType == typeof(long) &&
            long.TryParse(value, out var longValue))
        {
            return longValue;
        }
        else if (targetType == typeof(double) &&
            double.TryParse(value, out var doubleValue))
        {
            return doubleValue;
        }
        else if (targetType == typeof(decimal) &&
            decimal.TryParse(value, out var decimalValue))
        {
            return decimalValue;
        }
        else if (targetType == typeof(bool) &&
            bool.TryParse(value, out var boolValue))
        {
            return boolValue;
        }
        else if (targetType == typeof(Guid) &&
            Guid.TryParse(value, out var guidValue))
        {
            return guidValue;
        }
        else if (targetType.IsEnum &&
            Enum.TryParse(targetType, value, true, out var enumValue))
        {
            return enumValue;
        }

        if (CustomParsers.TryGetValue(targetType, out var outd))
        {
            var result = outd(value);
            if (result.Item1) return result.Item2;
        }

        return null;
    }
}

