using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record ElementClickCommand(Guid SessionId, Guid ElementId) : IRequest;

public class ElementClickCommandHandler(ISessionManager sessionManager) : IRequestHandler<ElementClickCommand>
{
    public async Task Handle(ElementClickCommand request, CancellationToken cancellationToken)
    {
        var session = sessionManager.GetSession(request.SessionId);
        await Task.Run(() => session.ElementCLick(request.ElementId), cancellationToken);
    }
}