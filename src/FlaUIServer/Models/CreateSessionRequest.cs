using System.Text.Json.Serialization;

namespace FlaUIServer.Models;

public class CreateSessionRequest
{
    public Capabilities Capabilities { get; set; } = new();
}

/// <summary>
/// Session capabilities
/// </summary>
public class Capabilities
{
    /// <summary>
    /// Always match capabilities
    /// </summary>
    public AlwaysMatch AlwaysMatch { get; set; } = new();
}

public class AlwaysMatch
{
    /// <summary>
    /// Path to Windows application executable
    /// </summary>
    [JsonPropertyName("appium:app")]
    public string Application { get; set; }

    /// <summary>
    /// App ProcessId in hexadecimal
    /// </summary>
    [JsonPropertyName("appium:appTopLevelWindow")]
    public string ApplicationTopLevelWindow { get; set; }
}