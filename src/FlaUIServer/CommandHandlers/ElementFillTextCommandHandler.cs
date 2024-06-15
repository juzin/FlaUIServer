using FlaUIServer.Models;
using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record ElementFillTextCommand(Guid SessionId, Guid ElementId, KeyInputRequest Text) : IRequest;

public class ElementFillTextCommandHandler(ISessionManager sessionManager) : IRequestHandler<ElementFillTextCommand>
{
    public async Task Handle(ElementFillTextCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var session = sessionManager.GetSession(request.SessionId);
        await Task.Run(() => session.ElementFillText(request.ElementId, new string(request.Text.Value)), cancellationToken);
    }
}