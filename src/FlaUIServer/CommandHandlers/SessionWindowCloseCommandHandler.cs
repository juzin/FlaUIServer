using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record SessionWindowCloseCommand(Guid SessionId) : IRequest;

public class SessionWindowCloseCommandHandler(ISessionManager sessionManager) : IRequestHandler<SessionWindowCloseCommand>
{
    public async Task Handle(SessionWindowCloseCommand request, CancellationToken cancellationToken)
    {
        var session = sessionManager.GetSession(request.SessionId);
        await Task.Run(() => session.CloseActiveWindow(), cancellationToken);
    }
}