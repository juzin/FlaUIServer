using System.Collections.Concurrent;
using FlaUIServer.Exceptions;
using FlaUIServer.Models;

namespace FlaUIServer.Session;

public class SessionManager : ISessionManager
{
    private readonly ConcurrentDictionary<Guid, WinAppSession> _sessions = new ();

    /// <inheritdoc />
    public Guid CreateSession(Capabilities capabilities, ILoggerFactory loggerFactory)
    {
        var sessionData = new WinAppSession(capabilities, loggerFactory);

        _sessions[sessionData.SessionId] = sessionData;
        return sessionData.SessionId;
    }
    
    /// <inheritdoc />
    public void DeleteSession(Guid sessionId)
    {
        var session = GetSession(sessionId);
        try
        {
            session.Close();
        }
        finally
        {
            _sessions.Remove(sessionId, out _);
            session.Dispose();
        }
    }
    
    /// <inheritdoc />
    public WinAppSession GetSession(Guid sessionId)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            session.UpdateLastActionTime();
            return session;
        }

        throw new ObjectNotFoundException($"Session with id '{sessionId}' was not found");
    }
    
    /// <inheritdoc />
    public SessionInfo[] GetSessions()
    {
        return _sessions.Select(x => new SessionInfo { SessionId = x.Key, Created = x.Value.Created, LastActionAt = x.Value.LastActionAt}).ToArray();
    }
}