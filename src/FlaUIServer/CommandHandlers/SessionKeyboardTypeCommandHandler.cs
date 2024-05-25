using FlaUIServer.Models;
using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record SessionKeyboardTypeCommand(Guid SessionId, KeyInputRequest Keys) : IRequest;

public class SessionKeyboardTypeCommandHandler(ISessionManager sessionManager) : IRequestHandler<SessionKeyboardTypeCommand>
{
    public async Task Handle(SessionKeyboardTypeCommand request, CancellationToken cancellationToken)
    {
        var session = sessionManager.GetSession(request.SessionId);
        await Task.Run(() => session.KeyboardType(request.Keys), cancellationToken);
    }
}