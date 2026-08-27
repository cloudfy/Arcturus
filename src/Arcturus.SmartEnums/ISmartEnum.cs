namespace Arcturus.SmartEnums;

/// <summary>
/// Defines the contract for type-safe string-backed enumerations with factory methods.
/// Implementations must provide static factory methods for parsing string values.
/// </summary>
/// <typeparam name="T">The concrete smart enum type implementing this interface.</typeparam>
public interface ISmartEnum<T>
    where T : ISmartEnum<T>
{
    /// <summary>
    /// Gets the underlying string value of this smart enum instance.
    /// </summary>
    string Value { get; }

    /// <summary>
    /// Creates a smart enum instance from the specified string value.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <returns>A smart enum instance corresponding to the specified value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not recognized.</exception>
    static abstract T FromValue(string value);

    /// <summary>
    /// Attempts to create a smart enum instance from the specified string value.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <param name="result">
    /// When this method returns, contains the smart enum instance if the value was recognized; otherwise, null.
    /// </param>
    /// <returns>true if the value was successfully parsed; otherwise, false.</returns>
    static abstract bool TryFromValue(
        string value,
        out T? result);
}
