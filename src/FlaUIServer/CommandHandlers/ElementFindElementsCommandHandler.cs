using FlaUIServer.Models;
using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record ElementFindElementsCommand(Guid SessionId, Guid ElementId, FindElementRequest SearchOptions) : IRequest<Guid[]>;

public class ElementFindElementsCommandHandler(ISessionManager sessionManager) : IRequestHandler<ElementFindElementsCommand, Guid[]>
{
    public async Task<Guid[]> Handle(ElementFindElementsCommand request, CancellationToken cancellationToken)
    {
        var session = sessionManager.GetSession(request.SessionId);
        return await Task.Run(() => session.ElementFindElements(request.ElementId, request.SearchOptions), cancellationToken);
    }
}