# Arcturus SmartEnums

Type-safe string-backed enumerations for .NET with built-in JSON serialization support.

## Features

- **Type-Safe**: Strongly-typed wrapper around string values with compile-time safety
- **JSON Serialization**: Automatic `System.Text.Json` support - serializes as plain strings
- **Value Parsing**: Built-in `FromValue` and `TryFromValue` factory methods
- **Two Usage Patterns**: Manual inheritance or source-generated with attributes
- **Immutable**: Record-based design with value equality semantics
- **Validation**: Automatic validation of non-empty string values

## Installation

```bash
dotnet add package Arcturus.SmartEnums
```

## Usage

### Option 1: Attribute-Driven Source Generation (Recommended)

Use the `[SmartEnum]` attribute to automatically generate a complete smart enum implementation:

```csharp
using Arcturus.SmartEnums;

[SmartEnum(
    "Pending=pending",
    "InProgress=in_progress",
    "Completed=completed",
    "Cancelled=cancelled")]
public partial record OrderStatus;

// Usage
var status = OrderStatus.Pending;
var parsed = OrderStatus.FromValue("in_progress");
var json = JsonSerializer.Serialize(status); // "pending"
```

### Option 2: Manual Inheritance

For more control, inherit from `SmartEnum<T>` and implement `ISmartEnum<T>`:

```csharp
using Arcturus.SmartEnums;

[JsonConverter(typeof(SmartEnumJsonConverter<Status>))]
public sealed record Status : SmartEnum<Status>, ISmartEnum<Status>
{
    public static readonly Status Pending = new("pending");
    public static readonly Status Active = new("active");
    public static readonly Status Disabled = new("disabled");

    private Status(string value) : base(value) { }

    public static Status FromValue(string value)
    {
        return value switch
        {
            "pending" => Pending,
            "active" => Active,
            "disabled" => Disabled,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown Status value.")
        };
    }

    public static bool TryFromValue(string value, out Status? result)
    {
        switch (value)
        {
            case "pending": result = Pending; return true;
            case "active": result = Active; return true;
            case "disabled": result = Disabled; return true;
            default: result = null; return false;
        }
    }
}
```

## API Reference

### `SmartEnum<T>`

Base class for smart enum implementations.

```csharp
public abstract record SmartEnum<T> where T : SmartEnum<T>
{
    public string Value { get; }
    protected SmartEnum(string value);
    public override string ToString();
}
```

### `ISmartEnum<T>`

Interface defining factory methods for parsing string values.

```csharp
public interface ISmartEnum<T> where T : ISmartEnum<T>
{
    string Value { get; }
    static abstract T FromValue(string value);
    static abstract bool TryFromValue(string value, out T? result);
}
```

### `[SmartEnum]` Attribute

Marks a partial record/class for code generation.

```csharp
[SmartEnum("Name", "OtherName=other_value")]
public partial record MyEnum;
```

**Syntax:**
- `"Name"` - Member name and serialized value are both "Name"
- `"Name=value"` - Member name is "Name", serialized value is "value"

## License

MIT
