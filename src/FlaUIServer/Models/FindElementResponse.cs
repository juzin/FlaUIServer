using System.Text.Json.Serialization;

namespace FlaUIServer.Models;

public class FindElementResponse(Guid elementId)
{
    [JsonPropertyName("element-6066-11e4-a52e-4f735466cecf")]
    public Guid ElementId { get; set; } = elementId;
}