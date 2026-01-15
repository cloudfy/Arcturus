namespace Arcturus.Repository.EntityFrameworkCore.NamingConvention;

/// <summary>
/// Specifies the supported naming conventions for formatting identifiers or text values.
/// </summary>
/// <remarks>Use this enumeration to indicate how names or strings should be transformed, such as when serializing
/// data, generating code, or enforcing style guidelines. The available conventions include options for snake case,
/// camel case, upper case, and others. The meaning of each value depends on the context in which it is
/// applied.</remarks>
public enum NamingConvention
{
    /// <summary>
    /// No convention is applied; names remain unchanged.
    /// </summary>
    None,
    /// <summary>
    /// Snake case formatting (e.g., "example_name_format").
    /// </summary>
    SnakeCase,
    /// <summary>
    /// Lower case formatting (e.g., "examplenameformat").
    /// </summary>
    LowerCase,
    /// <summary>
    /// Camel case formatting (e.g., "exampleNameFormat").
    /// </summary>
    CamelCase,
    /// <summary>
    /// Upper case formatting (e.g., "EXAMPLENAMEFORMAT").
    /// </summary>
    UpperCase,
    /// <summary>
    /// Upper snake case formatting (e.g., "EXAMPLE_NAME_FORMAT").
    /// </summary>
    UpperSnakeCase
}
