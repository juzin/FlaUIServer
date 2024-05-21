using FlaUIServer.Models;
using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record SessionWindowGetRectangleCommand(Guid SessionId) : IRequest<RectangleResponse>;

public class SessionWindowGetRectangleCommandHandler(ISessionManager sessionManager) : IRequestHandler<SessionWindowGetRectangleCommand, RectangleResponse>
{
    public async Task<RectangleResponse> Handle(SessionWindowGetRectangleCommand request, CancellationToken cancellationToken)
    {
        var session = sessionManager.GetSession(request.SessionId);
        return await Task.Run(() => session.GetWindowRectangle(), cancellationToken);
    }
}