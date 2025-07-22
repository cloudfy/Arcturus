namespace Arcturus.Repository.Abstracts.Specification.Parsing;

/// <summary>
/// Represents errors that occur during the evaluation of a filter expression.
/// </summary>
/// <remarks>This exception is thrown when a filter expression cannot be evaluated due to invalid syntax or other
/// errors.</remarks>
/// <param name="message"></param>
/// <param name="inner"></param>
public class FilterExpressionException(string? message, Exception? inner = null) : Exception(message, inner) { }

