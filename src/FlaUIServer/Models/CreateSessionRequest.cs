using System.Text.Json.Serialization;

namespace FlaUIServer.Models;

public class CreateSessionRequest
{
    public Capabilities Capabilities { get; set; } = new();
}

public class Capabilities
{
    public AlwaysMatch AlwaysMatch { get; set; } = new();
}

public class AlwaysMatch
{
    [JsonPropertyName("appium:app")]
    public string Application { get; set; }
}