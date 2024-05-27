using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record SessionGetSourceCommand(Guid SessionId) : IRequest<string>;

public class SessionGetSourceCommandHandler(ISessionManager sessionManager) : IRequestHandler<SessionGetSourceCommand, string>
{
    public async Task<string> Handle(SessionGetSourceCommand request, CancellationToken cancellationToken)
    {
        var session = sessionManager.GetSession(request.SessionId);
        return await Task.Run(() => session.WindowGetSource(), cancellationToken);
    }
}