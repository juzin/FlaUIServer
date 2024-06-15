using FlaUIServer.Models;
using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record FindElementCommand(Guid SessionId, FindElementRequest SearchOptions) : IRequest<FindElementResponse>;

public class FindElementCommandHandler(ISessionManager sessionManager) : IRequestHandler<FindElementCommand, FindElementResponse>
{
    public async Task<FindElementResponse> Handle(FindElementCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var session = sessionManager.GetSession(request.SessionId);
        var elementId = await Task.Run(() => session.FindElement(request.SearchOptions), cancellationToken);

        return new FindElementResponse(elementId);
    }
}