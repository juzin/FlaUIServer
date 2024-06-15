using FlaUIServer.Models;
using FlaUIServer.Session;

namespace FlaUIServer.Services;

/// <summary>
/// Service periodically closing and removing inactive sessions 
/// </summary>
/// <param name="sessionManager">Session manager</param>
/// <param name="logger">Logger</param>
/// <param name="options">Server options</param>
public sealed class SessionCleanupService(ISessionManager sessionManager, ILogger<SessionCleanupService> logger, ServerOptions options) : IHostedService, IDisposable
{
    private Timer _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Inactive session cleanup service started, cleanup cycle: {CleanupCycle}s", options.SessionCleanupCycleSeconds);
        _timer = new Timer(CleanupInactive, null, TimeSpan.Zero, TimeSpan.FromSeconds(options.SessionCleanupCycleSeconds));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        logger.LogInformation("FlaUI server is shutting down...");
        CleanupAll();
        return Task.CompletedTask;
    }

    private void CleanupInactive(object state)
    {
        var deleted = sessionManager.RemoveInactive();
        LogRemovedSessions(deleted);
    }

    private void CleanupAll()
    {
        var deleted = sessionManager.RemoveAll();
        LogRemovedSessions(deleted);
    }

    private void LogRemovedSessions(Guid[] sessionIds)
    {
        foreach (var sessionId in sessionIds)
        {
            logger.LogInformation("Cleanup service removed session '{SessionId}'", sessionId);
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}