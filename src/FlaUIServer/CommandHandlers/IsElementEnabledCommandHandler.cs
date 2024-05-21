using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record IsElementEnabledCommand(Guid SessionId, Guid ElementId) : IRequest<bool>;

public class IsElementEnabledCommandHandler(ISessionManager sessionManager) : IRequestHandler<IsElementEnabledCommand, bool>
{
    public async Task<bool> Handle(IsElementEnabledCommand request, CancellationToken cancellationToken)
    {
        var session = sessionManager.GetSession(request.SessionId);
        return await Task.Run(() => session.IsElementEnabled(request.ElementId), cancellationToken);
    }
}