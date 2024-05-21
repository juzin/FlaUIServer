using FlaUIServer.Models;

namespace FlaUIServer.Session;

public interface ISessionManager
{
    /// <summary>
    /// Create new session
    /// </summary>
    /// <param name="capabilities">Session capabilities</param>
    /// <returns>Session id</returns>
    Guid CreateSession(Capabilities capabilities);
    
    /// <summary>
    /// Delete existing session
    /// </summary>
    /// <param name="sessionId">Session id</param>
    void DeleteSession(Guid sessionId);
    
    /// <summary>
    /// Get session by id
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns>Win application session</returns>
    WinAppSession GetSession(Guid sessionId);
    
    /// <summary>
    /// Get information about existing sessions
    /// </summary>
    /// <returns>Array of existing sessions information</returns>
    SessionInfo[] GetSessions();
}