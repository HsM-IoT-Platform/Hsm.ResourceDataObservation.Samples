using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeviceForwarding.Queue;

internal static class Constants
{
    public static readonly JsonSerializerOptions DefaultSerializerOptions = new()
    {
        Converters = {new JsonStringEnumConverter()},
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    
    public static readonly JsonSerializerOptions WriteIndentedSerializerOptions = new()
    {
        Converters = {new JsonStringEnumConverter()},
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true
    };
}