using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arcturus.Repository.Pagination;

/// <summary>
/// Represents a cursor used for paging through a collection of items, with a key and an optional order-by value.
/// </summary>
/// <remarks>The <see cref="PagingCursor{TKey}"/> class provides functionality to serialize and deserialize cursor
/// information for paging operations, allowing for efficient navigation through large datasets.</remarks>
/// <typeparam name="TKey">The type of the key used for paging.</typeparam>
public sealed class PagingCursor<TKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PagingCursor"/> class.
    /// </summary>
    /// <remarks>This constructor creates a default instance of the <see cref="PagingCursor"/> class. Use this
    /// class to manage pagination state when working with paged data.</remarks>
    public PagingCursor() { }

    [JsonPropertyName("k")]
    public required TKey DefaultValue { get; init; }

    [JsonPropertyName("v")]
    public object? OrderByValue { get; init; }

    /// <summary>
    /// Creates a new instance of the <see cref="PagingCursor{TKey}"/> class with the specified default key value and
    /// optional order-by value.
    /// </summary>
    /// <param name="defaultValue">The default key value to initialize the cursor with. This value is used when no specific key is provided.</param>
    /// <param name="orderByValue">An optional value used to determine the order of items. Can be <see langword="null"/> if no order-by value is
    /// required.</param>
    /// <returns>A new instance of the <see cref="PagingCursor{TKey}"/> class initialized with the specified values.</returns>
    public static PagingCursor<TKey> Create(TKey defaultValue, object? orderByValue = null)
        => new() { DefaultValue = defaultValue, OrderByValue = orderByValue };

    public override string ToString()
    {
        var value = JsonSerializer.Serialize(this);
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
    }
    /// <summary>
    /// Attempts to parse the specified string into a <see cref="PagingCursor{TKey}"/> object.
    /// </summary>
    /// <remarks>This method does not throw exceptions if parsing fails. Instead, it returns <see
    /// langword="false"/> and sets <paramref name="result"/> to <see langword="null"/>.</remarks>
    /// <param name="value">The string representation of the paging cursor to parse. Can be <see langword="null"/>.</param>
    /// <param name="result">When this method returns, contains the parsed <see cref="PagingCursor{TKey}"/> object if the parsing was
    /// successful; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? value, out PagingCursor<TKey>? result)
    {
        result = default;
        if (value is null) return false;

        try
        {
            result = Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// Parses a base64-encoded string into a <see cref="PagingCursor{TKey}"/> object.
    /// </summary>
    /// <param name="value">The base64-encoded string representing a serialized <see cref="PagingCursor{TKey}"/>.</param>
    /// <returns>A <see cref="PagingCursor{TKey}"/> object deserialized from the provided string.</returns>
    public static PagingCursor<TKey> Parse(string value)
    {
        var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
        return JsonSerializer.Deserialize<PagingCursor<TKey>>(json)!;
    }
}
