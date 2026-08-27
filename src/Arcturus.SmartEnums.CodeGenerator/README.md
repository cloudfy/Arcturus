# Arcturus.SmartEnums.CodeGenerator

Source generator for `Arcturus.SmartEnums` that emits strongly typed smart enum members from an attribute-based declaration.

This package is intended to be consumed as an analyzer, along with the runtime package `Arcturus.SmartEnums`.

## What it generates

When a partial record or partial class is annotated with `[SmartEnum(...)]`, the generator emits a complete smart enum implementation, including:

- static readonly members for each enum state
- a private constructor that accepts the string value
- a `Values` collection
- `FromValue(string value)`
- `TryFromValue(string value, out T? result)`
- `[JsonConverter(typeof(SmartEnumJsonConverter<T>))]` for System.Text.Json serialization

## Usage

Add the package and the runtime package to your project:

```bash
dotnet add package Arcturus.SmartEnums
dotnet add package Arcturus.SmartEnums.CodeGenerator
```

Then declare a partial record:

```csharp
using Arcturus.SmartEnums;

[SmartEnum(
    "Pending=pending",
    "InProgress=in_progress",
    "Completed=completed",
    "Cancelled=cancelled")]
public partial record OrderStatusEnum;
```

The generator will generate code equivalent to:

```csharp
public sealed partial record OrderStatusEnum : SmartEnum<OrderStatusEnum>, ISmartEnum<OrderStatusEnum>
{
    public static readonly OrderStatusEnum Pending = new("pending");
    public static readonly OrderStatusEnum InProgress = new("in_progress");
    public static readonly OrderStatusEnum Completed = new("completed");
    public static readonly OrderStatusEnum Cancelled = new("cancelled");

    private OrderStatusEnum(string value) : base(value) { }

    public static OrderStatusEnum FromValue(string value)
    {
        return value switch
        {
            "pending" => Pending,
            "in_progress" => InProgress,
            "completed" => Completed,
            "cancelled" => Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OrderStatusEnum value.")
        };
    }

    public static bool TryFromValue(string value, out OrderStatusEnum? result)
    {
        switch (value)
        {
            case "pending": result = Pending; return true;
            case "in_progress": result = InProgress; return true;
            case "completed": result = Completed; return true;
            case "cancelled": result = Cancelled; return true;
            default: result = null; return false;
        }
    }
}
```

## Supported syntax

The attribute accepts a list of string entries:

- `"Name"` generates a member named `Name` and uses the same string as its backing value.
- `"Name=value"` generates a member named `Name` and stores `value` as the backing string.

Examples:

```csharp
[SmartEnum("Pending", "Active", "Disabled")]
public partial record Status;

[SmartEnum(
    "Pending=pending",
    "InProgress=in_progress",
    "Completed=completed")]
public partial record OrderStatusEnum;
```

## Notes

- The generated type is `sealed partial`.
- The generated type derives from `SmartEnum<T>` and implements `ISmartEnum<T>`.
- The runtime type is in `Arcturus.SmartEnums`.
- The generator is meant to be used in consuming projects as a source generator/analyzer.
- For manual implementations, you can also inherit directly from `SmartEnum<T>` without using the generator.

## License

MIT
