using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record SessionGetWindowHandlesCommand(Guid SessionId) : IRequest<string[]>;

public class SessionGetWindowHandlesCommandHandler(ISessionManager sessionManager) : IRequestHandler<SessionGetWindowHandlesCommand, string[]>
{
    public async Task<string[]> Handle(SessionGetWindowHandlesCommand request, CancellationToken cancellationToken)
    {
        var session = sessionManager.GetSession(request.SessionId);
        return await Task.Run(() => session.GetWindowHandles(), cancellationToken);
    }
}