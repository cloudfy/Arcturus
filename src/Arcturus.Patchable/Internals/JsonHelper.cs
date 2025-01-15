using System.Text.Json;

namespace Arcturus.Patchable.Internals;

internal sealed class JsonHelper
{
    internal static object ToObject(JsonElement element, Type returnType)
    {
        var json = element.GetRawText();
        return JsonSerializer.Deserialize(json, returnType)!;
    }
}