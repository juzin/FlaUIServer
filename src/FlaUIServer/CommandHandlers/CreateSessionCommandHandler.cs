using FlaUIServer.Models;
using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record CreateSessionCommand(CreateSessionRequest CreateSession) : IRequest<SessionResponse>;

public class CreateSessionCommandHandler(ISessionManager sessionManager, ILoggerFactory loggerFactory): IRequestHandler<CreateSessionCommand, SessionResponse>
{
    public async Task<SessionResponse> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var sessionId = await Task.Run(() => sessionManager.CreateSession(request.CreateSession.Capabilities, loggerFactory), cancellationToken);
        return new SessionResponse(sessionId);
    }
}