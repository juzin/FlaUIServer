using System.Text.Json.Serialization;

namespace FlaUIServer.Models;

public class SessionInfo
{
    /// <summary>
    /// Session id
    /// Do not expose id in api
    /// </summary>
    [JsonIgnore]
    public Guid SessionId { get; set; }
    
    public DateTimeOffset Created { get; set; }
    
    public DateTimeOffset LastActionAt { get; set; }
}