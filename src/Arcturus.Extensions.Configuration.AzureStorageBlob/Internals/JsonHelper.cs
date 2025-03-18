using System.Text.Json;

namespace Arcturus.Extensions.Configuration.AzureStorageBlob.Internals;

internal static class JsonHelper
{
    private static Dictionary<string, string> FlattenJson(JsonElement element, string parentKey = "")
    {
        var result = new Dictionary<string, string>();
        FlattenJsonElement(element, parentKey, result);
        return result;
    }

    internal static Dictionary<string, string> FlattenJson(JsonDocument doc)
    {
        return FlattenJson(doc.RootElement);
    }

    private static void FlattenJsonElement(JsonElement element, string parentKey, Dictionary<string, string> result)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    string newKey = string.IsNullOrEmpty(parentKey) ? property.Name : $"{parentKey}:{property.Name}";
                    FlattenJsonElement(property.Value, newKey, result);
                }
                break;

            case JsonValueKind.Array:
                int index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    string newKey = $"{parentKey}:{index}";
                    FlattenJsonElement(item, newKey, result);
                    index++;
                }
                break;

            default:
                result[parentKey] = element.ToString();
                break;
        }
    }
}
