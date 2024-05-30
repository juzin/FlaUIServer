using System.Collections.Concurrent;
using FlaUIServer.Exceptions;
using FlaUIServer.Models;

namespace FlaUIServer.Session;

public class SessionManager(ServerOptions options) : ISessionManager
{
    private const int InactiveTimeout = 90;
    private readonly ConcurrentDictionary<Guid, WinAppSession> _sessions = new ();

    /// <inheritdoc />
    public Guid CreateSession(Capabilities capabilities, ILoggerFactory loggerFactory)
    {
        var sessionData = new WinAppSession(capabilities, loggerFactory, options);

        _sessions[sessionData.SessionId] = sessionData;
        return sessionData.SessionId;
    }
    
    /// <inheritdoc />
    public void DeleteSession(Guid sessionId)
    {
        if (_sessions.TryRemove(sessionId, out var session))
        {
            try
            {
                session.Close();
            }
            finally
            {
                session.Dispose();
            }
        }
        else
        {
            throw new ObjectNotFoundException($"Session with id '{sessionId}' was not found");
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

    /// <inheritdoc />
    public Guid[] RemoveInactive()
    {
        var deleted = new List<Guid>();
        var orphanedSession = _sessions.Where(x => x.Value.LastActionAt.AddSeconds(InactiveTimeout) < DateTimeOffset.Now).ToArray();
        foreach (var session in orphanedSession)
        {
            DeleteSession(session.Key);
            deleted.Add(session.Key);
        }
        
        return deleted.ToArray();
    }
}