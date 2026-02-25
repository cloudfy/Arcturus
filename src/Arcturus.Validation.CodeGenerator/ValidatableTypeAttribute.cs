using System;

namespace Arcturus.Validation;

/// <summary>
/// Internal marker attribute used by the source generator to track types that need validation.
/// This attribute is automatically applied during code generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
internal sealed class ValidatableTypeAttribute : Attribute
{
}
