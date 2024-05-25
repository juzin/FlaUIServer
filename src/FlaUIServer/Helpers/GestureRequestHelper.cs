using System.Text.Json;
using FlaUIServer.Exceptions;

namespace FlaUIServer.Helpers;

public static class GestureRequestHelper
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public static T DeserializeGestureRequest<T>(JsonElement[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Length == 0)
        {
            throw new RequestValidationException("Gesture request body does not contain any data");
        }

        return JsonSerializer.Deserialize<T>(data[0], Options);
    }
}