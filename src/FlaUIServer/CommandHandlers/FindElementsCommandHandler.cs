using FlaUIServer.Models;
using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record FindElementsCommand(Guid SessionId, FindElementRequest SearchOptions) : IRequest<Guid[]>;

public class FindElementsCommandHandler(ISessionManager sessionManager) : IRequestHandler<FindElementsCommand, Guid[]>
{
    public async Task<Guid[]> Handle(FindElementsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var session = sessionManager.GetSession(request.SessionId);
        return await Task.Run(() => session.FindElements(request.SearchOptions), cancellationToken);
    }
}