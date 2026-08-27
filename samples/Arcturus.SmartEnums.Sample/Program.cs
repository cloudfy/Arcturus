using System;
using System.Text.Json;
using Arcturus.SmartEnums;

namespace Arcturus.SmartEnums.Sample;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Arcturus SmartEnums Sample ===\n");

        // Test 1: Manual inheritance pattern
        Console.WriteLine("1. Manual Inheritance Pattern:");
        TestManualPattern();
        Console.WriteLine();

        // Test 2: Attribute-driven generator pattern
        Console.WriteLine("2. Attribute-Driven Generator Pattern:");
        TestGeneratorPattern();
        Console.WriteLine();

        // Test 3: JSON Serialization
        Console.WriteLine("3. JSON Serialization:");
        TestJsonSerialization();
        Console.WriteLine();

        Console.WriteLine("All tests completed!");
    }

    static void TestManualPattern()
    {
        var status1 = Status.Pending;
        var status2 = Status.FromValue("active");

        Console.WriteLine($"  Status1: {status1}");
        Console.WriteLine($"  Status2: {status2}");
        Console.WriteLine($"  Status1 == Status.Pending: {status1 == Status.Pending}");
        Console.WriteLine($"  Status2.Value: {status2.Value}");

        if (Status.TryFromValue("disabled", out var status3))
        {
            Console.WriteLine($"  Status3: {status3}");
        }

        try
        {
            var invalid = Status.FromValue("unknown");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"  ✓ Expected exception for invalid value: {ex.Message}");
        }
    }

    static void TestGeneratorPattern()
    {
        var orderStatus1 = OrderStatus.Pending;
        var orderStatus2 = OrderStatus.FromValue("in_progress");

        Console.WriteLine($"  OrderStatus1: {orderStatus1}");
        Console.WriteLine($"  OrderStatus2: {orderStatus2}");
        Console.WriteLine($"  OrderStatus1 == OrderStatus.Pending: {orderStatus1 == OrderStatus.Pending}");
        Console.WriteLine($"  OrderStatus2.Value: {orderStatus2.Value}");
        Console.WriteLine($"  All Values: {string.Join(", ", OrderStatus.Values)}");

        if (OrderStatus.TryFromValue("completed", out var orderStatus3))
        {
            Console.WriteLine($"  OrderStatus3: {orderStatus3}");
        }

        try
        {
            var invalid = OrderStatus.FromValue("unknown");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"  ✓ Expected exception for invalid value: {ex.Message}");
        }
    }

    static void TestJsonSerialization()
    {
        var status = Status.Active;
        var json = JsonSerializer.Serialize(status);
        Console.WriteLine($"  Serialized: {json}");

        var deserialized = JsonSerializer.Deserialize<Status>(json);
        Console.WriteLine($"  Deserialized: {deserialized}");
        Console.WriteLine($"  Equal: {status == deserialized}");

        var orderStatus = OrderStatus.InProgress;
        var orderJson = JsonSerializer.Serialize(orderStatus);
        Console.WriteLine($"  OrderStatus Serialized: {orderJson}");

        var deserializedOrder = JsonSerializer.Deserialize<OrderStatus>(orderJson);
        Console.WriteLine($"  OrderStatus Deserialized: {deserializedOrder}");
        Console.WriteLine($"  Equal: {orderStatus == deserializedOrder}");
    }
}

// Manual inheritance pattern
[System.Text.Json.Serialization.JsonConverter(typeof(SmartEnumJsonConverter<Status>))]
public sealed record Status : SmartEnum<Status>, ISmartEnum<Status>
{
    public static readonly Status Pending = new("pending");
    public static readonly Status Active = new("active");
    public static readonly Status Disabled = new("disabled");

    private Status(string value) : base(value) { }

    public static Status FromValue(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

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
            case "pending":
                result = Pending;
                return true;
            case "active":
                result = Active;
                return true;
            case "disabled":
                result = Disabled;
                return true;
            default:
                result = null;
                return false;
        }
    }
}

// Attribute-driven generator pattern
[SmartEnum(
    "Pending=pending",
    "InProgress=in_progress",
    "Completed=completed",
    "Cancelled=cancelled")]
public partial record OrderStatus;
