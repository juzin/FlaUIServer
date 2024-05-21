using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record SessionGetWindowHandleCommand(Guid SessionId) : IRequest<string>;

public class SessionGetWindowHandleCommandHandler(ISessionManager sessionManager) : IRequestHandler<SessionGetWindowHandleCommand, string>
{
    public async Task<string> Handle(SessionGetWindowHandleCommand request, CancellationToken cancellationToken)
    {
        var session = sessionManager.GetSession(request.SessionId);
        return await Task.Run(() => session.GetWindowHandle(), cancellationToken);
    }
}