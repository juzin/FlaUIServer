using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record ElementClearTextCommand(Guid SessionId, Guid ElementId) : IRequest;

public class ElementClearTextCommandHandler(ISessionManager sessionManager) : IRequestHandler<ElementClearTextCommand>
{
    public async Task Handle(ElementClearTextCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var session = sessionManager.GetSession(request.SessionId);
        await Task.Run(() => session.ElementClearText(request.ElementId), cancellationToken);
    }
}