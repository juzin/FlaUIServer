using FlaUIServer.Models;
using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record GetSessionsInfoCommand() : IRequest<SessionInfo[]>;

public class GetSessionsInfoCommandHandler(ISessionManager sessionManager) : IRequestHandler<GetSessionsInfoCommand, SessionInfo[]>
{
    public async Task<SessionInfo[]> Handle(GetSessionsInfoCommand request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(sessionManager.GetSessions());
    }
}