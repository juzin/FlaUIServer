using System.Text.Json;
using FlaUIServer.Exceptions;

namespace FlaUIServer.Helpers;

public static class GestureRequestHelper
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    /// <summary>
    /// Deserialize gesture request body (First JsonElement)
    /// </summary>
    /// <param name="data">Json element array</param>
    /// <typeparam name="T">Type of gesture body</typeparam>
    /// <returns>Desirialized gesture body</returns>
    /// <exception cref="RequestValidationException">When body is missing</exception>
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