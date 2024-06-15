using System.Collections.ObjectModel;

namespace FlaUIServer.Models;

public class ServerOptions
{
    /// <summary>
    /// Enable swagger
    /// </summary>
    public bool UseSwagger { get; set; }

    /// <summary>
    /// Allow execution of powershell
    /// </summary>
    public bool AllowPowershell { get; set; }

    /// <summary>
    /// Enables console log to contain request/response body
    /// </summary>
    public bool LogResponseBody { get; set; }

    /// <summary>
    /// Cleanup cycle in second. 0 disables cleanup. Default value is 200s
    /// </summary>
    public ushort SessionCleanupCycleSeconds { get; set; } = 90;

    /// <summary>
    /// Urls with port server listen on
    /// </summary>
    public Collection<string> Urls { get; } = [];
}