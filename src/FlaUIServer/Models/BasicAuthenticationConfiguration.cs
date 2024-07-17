namespace FlaUIServer.Models;

public class BasicAuthenticationConfiguration
{
    /// <summary>
    /// Configuration section key
    /// </summary>
    public const string BasicAuthentication = "BasicAuthentication";

    /// <summary>
    /// Basic auth username
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Basic auth password
    /// </summary>
    public string Password { get; set; }
}