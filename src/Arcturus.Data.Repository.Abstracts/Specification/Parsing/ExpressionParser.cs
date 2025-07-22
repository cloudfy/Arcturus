namespace Arcturus.Repository.Specification.Parsing;

/// <summary>
/// Provides methods for parsing string representations of filter and order-by conditions into LINQ expressions.
/// </summary>
/// <remarks>The <see cref="ExpressionParser"/> class includes methods to parse filter conditions and order-by
/// clauses from strings into expressions that can be used in LINQ queries. It supports both direct parsing and
/// try-parse patterns, allowing for error handling without exceptions. Ensure that input strings are correctly
/// formatted to avoid parsing errors.</remarks>
public static class ExpressionParser
{
    /// <summary>
    /// Parses a string representation of a filter condition into a LINQ expression.
    /// </summary>
    /// <remarks>The parsed expression can be used in LINQ queries to filter collections based on the
    /// specified condition. Ensure that the filter condition string is correctly formatted to avoid parsing
    /// errors.</remarks>
    /// <typeparam name="T">The type of the elements in the collection to be filtered.</typeparam>
    /// <param name="where">A string containing the filter condition to parse.</param>
    /// <returns>An expression that can be used to filter a collection of type <typeparamref name="T"/>.</returns>
    public static Expression<Func<T, bool>> ParseWhere<T>(string where)
    {
        return Internals.FilterExpressionParser.Parse<T>([where]);
    }
    /// <summary>
    /// Attempts to parse a string representation of a filter condition into a strongly-typed expression.
    /// </summary>
    /// <remarks>This method does not throw exceptions for parsing errors. Instead, it returns <see
    /// langword="false"/> and sets <paramref name="expression"/> to <see langword="null"/> if the parsing
    /// fails.</remarks>
    /// <typeparam name="T">The type of the object that the expression will evaluate.</typeparam>
    /// <param name="where">The string representation of the filter condition to parse.</param>
    /// <param name="expression">When this method returns, contains the parsed expression if the parsing was successful; otherwise, <see
    /// langword="null"/>.</param>
    /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
    public static bool TryParseWhere<T>(string where, out Expression<Func<T, bool>>? expression)
    {
        try
        {
            expression = Internals.FilterExpressionParser.Parse<T>([where]);
            return true;
        }
        catch
        {
            expression = null;
            return false;
        }
    }
    /// <summary>
    /// Parses an array of string conditions into a LINQ expression for filtering objects of type <typeparamref
    /// name="T"/>.
    /// </summary>
    /// <remarks>The method converts the provided string conditions into a strongly-typed expression that can
    /// be used in LINQ queries. Ensure that the conditions are valid and correspond to properties of the type
    /// <typeparamref name="T"/>.</remarks>
    /// <typeparam name="T">The type of objects to filter.</typeparam>
    /// <param name="where">An array of string conditions that define the filtering criteria.</param>
    /// <returns>An expression that can be used to filter a collection of objects of type <typeparamref name="T"/> based on the
    /// specified conditions.</returns>
    public static Expression<Func<T, bool>> ParseWhere<T>(string[] where)
    {
        return Internals.FilterExpressionParser.Parse<T>(where);
    }
    /// <summary>
    /// Parses the specified order by string into a lambda expression and a boolean indicating sort direction.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence to be ordered.</typeparam>
    /// <param name="orderBy">A string representing the property name to order by, optionally prefixed with a '-' to indicate descending
    /// order.</param>
    /// <returns>A tuple containing a lambda expression that represents the order by clause and a boolean where <see
    /// langword="true"/> indicates descending order, and <see langword="false"/> indicates ascending order.</returns>
    public static (Expression<Func<T, object?>>, bool) ParseOrderby<T>(string orderBy)
    {
        return Internals.OrderByExpressionParser.ParseOrderBy<T>(orderBy);
    }
    /// <summary>
    /// Attempts to parse the specified order-by string into an expression and sort direction.
    /// </summary>
    /// <typeparam name="T">The type of the elements to be ordered.</typeparam>
    /// <param name="orderBy">The string representation of the order-by clause to parse.</param>
    /// <param name="orderByExpression">When this method returns, contains a tuple with the parsed expression and a boolean indicating the sort
    /// direction, if the parsing succeeded; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the order-by string was successfully parsed; otherwise, <see langword="false"/>.</returns>
    public static bool TryParseOrderby<T>(string orderBy, out (Expression<Func<T, object?>>, bool)? orderByExpression)
    {
        try
        {
            orderByExpression = Internals.OrderByExpressionParser.ParseOrderBy<T>(orderBy);
            return true;
        }
        catch
        {
            orderByExpression = null;
            return false;
        }
    }
}
