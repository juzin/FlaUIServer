using FlaUIServer.Session;

namespace FlaUIServer.Services;

public class OrphanedSessionCleanupService(ISessionManager sessionManager, ILogger<OrphanedSessionCleanupService> logger) : IHostedService, IDisposable
{
    private const int CleanPeriod = 90;
    private Timer _timer;
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(CleanPeriod));
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
            logger.LogInformation("Cleaning service removed inactive session '{SessionId}'", sessionId);
        }
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
    }
}