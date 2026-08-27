namespace Arcturus.SmartEnums;

/// <summary>
/// Base class for type-safe string-backed enumerations.
/// Provides a strongly-typed wrapper around string values with equality semantics based on the underlying value.
/// </summary>
/// <typeparam name="T">The concrete smart enum type.</typeparam>
public abstract record SmartEnum<T>
        where T : SmartEnum<T>
{
    /// <summary>
    /// Gets the underlying string value of this smart enum instance.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmartEnum{T}"/> class with the specified value.
    /// </summary>
    /// <param name="value">The string value for this smart enum instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is empty or whitespace.</exception>
    protected SmartEnum(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Smart enum value cannot be empty or whitespace.", nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// Returns the string value of this smart enum instance.
    /// </summary>
    /// <returns>The underlying string value.</returns>
    public override string ToString() => Value;
}
