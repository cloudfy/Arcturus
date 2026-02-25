using System;

namespace Arcturus.Validation;

/// <summary>
/// Factory for creating ValidateParametersFilter instances.
/// The source generator will register a factory that creates the generated implementation.
/// </summary>
public static class ValidateParametersFilterFactory
{
    private static Func<ValidateParametersFilter>? _factory;

    /// <summary>
    /// Sets the factory function used to create ValidateParametersFilter instances.
    /// This is called by the generated code during module initialization.
    /// </summary>
    /// <param name="factory">The factory function to use.</param>
    public static void SetFactory(Func<ValidateParametersFilter> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Creates an instance of ValidateParametersFilter.
    /// If source generation is active, returns the generated implementation.
    /// Otherwise, returns the default implementation.
    /// </summary>
    public static ValidateParametersFilter Create()
    {
        return _factory?.Invoke() ?? new ValidateParametersFilter();
    }
}
