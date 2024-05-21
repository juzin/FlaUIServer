using FlaUIServer.Models;
using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record CreateSessionCommand(CreateSessionRequest CreateSession) : IRequest<SessionResponse>;

public class CreateSessionCommandHandler(ISessionManager sessionManager): IRequestHandler<CreateSessionCommand, SessionResponse>
{
    public async Task<SessionResponse> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var sessionId = await Task.Run(() => sessionManager.CreateSession(request.CreateSession.Capabilities), cancellationToken);
        return new SessionResponse(sessionId);
    }
}