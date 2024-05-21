namespace FlaUIServer.Models;

public class StatusResponse
{
    public bool Ready => true;

    public string Message => "Server is up and running";
}