using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record SessionGetSourceCommand(Guid SessionId) : IRequest<string>;

public class SessionGetSourceCommandHandler(ISessionManager sessionManager) : IRequestHandler<SessionGetSourceCommand, string>
{
    public async Task<string> Handle(SessionGetSourceCommand request, CancellationToken cancellationToken)
    {
        // Just to validate session exists
        _ = sessionManager.GetSession(request.SessionId);
        return await Task.Run(() =>"Get of application source is not supported", cancellationToken);
    }
}