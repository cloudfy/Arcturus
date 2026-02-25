using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Arcturus.Validation;

// This class provides validation logic.
// The source generator in the consuming project will generate an override
internal static partial class GeneratedValidationHelper
{
    internal static object? ValidateArguments(IList<object?> arguments)
    {
        // By default, no validation (will be implemented in consuming project)
        return null;
    }
}
