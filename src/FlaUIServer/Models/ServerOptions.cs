namespace FlaUIServer.Models;

public class ServerOptions
{
    public bool UseSwagger { get; set; } = false;
    public bool AllowPowershell { get; set; } = false;
    public bool LogResponseBody { get; set; } = false;
    public List<string> Urls { get; } = new ();
}