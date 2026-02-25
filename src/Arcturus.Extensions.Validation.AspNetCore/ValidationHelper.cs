using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Arcturus.Validation;

internal static partial class ValidationHelper
{
    // Function delegate that will be set by the generated code
    // If null, no validation will occur (no generated code available)
#pragma warning disable CS0649 // Field is assigned by generated code
    private static Func<IList<object?>, object?>? _validateFunc;
#pragma warning restore CS0649

    // This method is called by ValidateParametersFilter
    internal static object? ValidateArguments(IList<object?> arguments)
    {
        return _validateFunc?.Invoke(arguments);
    }

    // This method will be implemented by the source generator to register the validation function
    static partial void InitializeValidation();

    // Static constructor to initialize the validation  
    static ValidationHelper()
    {
        InitializeValidation();
    }
}
