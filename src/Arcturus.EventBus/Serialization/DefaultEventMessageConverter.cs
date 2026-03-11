using Arcturus.EventBus.Abstracts;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arcturus.EventBus.Serialization;

// not DI
internal sealed class DefaultEventMessageConverter(
    DefaultEventMessageTypeResolver resolver) 
    : JsonConverter<IEventMessage>
{
    private const string _discriminatorPropertyName = "$eventType";
    private readonly DefaultEventMessageTypeResolver _typeResolver = resolver;
 
    public override IEventMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        // Get the discriminator
        if (!root.TryGetProperty(_discriminatorPropertyName, out var typeProperty))
        {
            throw new JsonException($"Missing discriminator property '{_discriminatorPropertyName}'.");
        }

        // read type from discriminator and resolve
        // - requires that one of the .Registerxxx methods is called.
        var typeName = typeProperty.GetString() 
            ?? throw new JsonException("Discriminator property value is null.");
        var type = _typeResolver.ResolveType(typeName) 
            ?? throw new UnprocessableEventException($"Cannot resolve type '{typeName}'. Ensure to call .RegisterHandlersFromAssembly() or similar on builder at startup.");
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
        writer.WriteString(_discriminatorPropertyName, value.GetEventName());

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
}