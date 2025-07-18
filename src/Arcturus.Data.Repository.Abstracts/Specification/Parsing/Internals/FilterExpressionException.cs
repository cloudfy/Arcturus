namespace Arcturus.Data.Repository.Abstracts.Specification.Parsing.Internals;

public class FilterExpressionException(string? message, Exception? inner = null) : Exception(message, inner) { }

