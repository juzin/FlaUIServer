using FlaUIServer.Models;
using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record ElementFindElementCommand(Guid SessionId, Guid ElementId, FindElementRequest SearchOptions) : IRequest<FindElementResponse>;

public class ElementFindElementCommandHandler(ISessionManager sessionManager) : IRequestHandler<ElementFindElementCommand, FindElementResponse>
{
    public async Task<FindElementResponse> Handle(ElementFindElementCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var session = sessionManager.GetSession(request.SessionId);
        var elementId = await Task.Run(() => session.ElementFindElement(request.ElementId, request.SearchOptions), cancellationToken);

        return new FindElementResponse(elementId);
    }
}