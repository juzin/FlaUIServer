namespace FlaUIServer.Session;

public class SessionInfo
{
    public Guid SessionId { get; set; }
    
    public DateTimeOffset Created { get; set; }
    
    public DateTimeOffset LastActionAt { get; set; }
}