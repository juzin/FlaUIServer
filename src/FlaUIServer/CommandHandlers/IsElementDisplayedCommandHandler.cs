using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record IsElementDisplayedCommand(Guid SessionId, Guid ElementId) : IRequest<bool>;

public class IsElementDisplayedCommandHandler(ISessionManager sessionManager) : IRequestHandler<IsElementDisplayedCommand, bool>
{
    public async Task<bool> Handle(IsElementDisplayedCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var session = sessionManager.GetSession(request.SessionId);
        return await Task.Run(() => session.IsElementDisplayed(request.ElementId), cancellationToken);
    }
}