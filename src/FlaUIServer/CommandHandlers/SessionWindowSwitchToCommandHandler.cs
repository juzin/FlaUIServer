using FlaUIServer.Models;
using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record SessionWindowSwitchToCommand(Guid SessionId, SessionSwitchToWindowRequest Window) : IRequest;

public class SessionWindowSwitchToCommandHandler(ISessionManager sessionManager) : IRequestHandler<SessionWindowSwitchToCommand>
{
    public async Task Handle(SessionWindowSwitchToCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var session = sessionManager.GetSession(request.SessionId);
        await Task.Run(() => session.SwitchToWindow(request.Window.Name), cancellationToken);
    }
}