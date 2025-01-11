using Arcturus.EventBus.Abstracts;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arcturus.EventBus.RabbitMQ.Internals;

internal sealed class EventMessageConverter : JsonConverter<IEventMessage>
{
    private const string DiscriminatorPropertyName = "$eventType";
    private readonly DefaultEventMessageTypeResolver _typeResolver;

    internal EventMessageConverter()
    {
        _typeResolver = new DefaultEventMessageTypeResolver();
    }

    public override IEventMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        // Get the discriminator
        if (!root.TryGetProperty(DiscriminatorPropertyName, out var typeProperty))
        {
            throw new JsonException($"Missing discriminator property '{DiscriminatorPropertyName}'.");
        }

        var typeName = typeProperty.GetString() ?? throw new JsonException("Discriminator property value is null.");
        var type = _typeResolver.ResolveType(typeName) ?? throw new JsonException($"Cannot resolve type '{typeName}'.");
        var json = root.GetRawText();
        return (IEventMessage)JsonSerializer.Deserialize(json, type, options)!;
    }

    public override void Write(Utf8JsonWriter writer, IEventMessage value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }// Get the actual type of the object
        var type = value.GetType();
        writer.WriteStartObject();

        // Write the discriminator (type)
        writer.WriteString(DiscriminatorPropertyName, GetEventName(type));

        // Serialize the object properties
        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            var propertyValue = property.GetValue(value);
            if (propertyValue != null)
            {
                writer.WritePropertyName(property.Name);
                JsonSerializer.Serialize(writer, propertyValue, propertyValue?.GetType() ?? typeof(object), options);
            }
            else if (propertyValue == null &&
                options.DefaultIgnoreCondition != JsonIgnoreCondition.WhenWritingNull)
            {
                writer.WritePropertyName(property.Name);
                writer.WriteNullValue();
            }
        }

        writer.WriteEndObject();
    }
    private static string GetEventName(Type type)
    {
        var messageAttribute = type.GetCustomAttribute<EventMessageAttribute>();
        if (messageAttribute is not null)
            return messageAttribute.Name;

        return type.FullName!;
    }
}