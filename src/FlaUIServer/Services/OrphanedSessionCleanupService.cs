﻿using FlaUIServer.Models;
using FlaUIServer.Session;

namespace FlaUIServer.Services;

public class OrphanedSessionCleanupService(ISessionManager sessionManager, ILogger<OrphanedSessionCleanupService> logger, ServerOptions options) : IHostedService, IDisposable
{
    private Timer _timer;
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Inactive session cleanup service started, cleanup cycle: {CleanupCycle}s", options.SessionCleanupCycle);
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(options.SessionCleanupCycle));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
    
    private void DoWork(object state)
    {
        var deleted = sessionManager.RemoveInactive();

        foreach (var sessionId in deleted)
        {
            logger.LogInformation("Cleanup service removed inactive session '{SessionId}'", sessionId);
        }
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
    }
}