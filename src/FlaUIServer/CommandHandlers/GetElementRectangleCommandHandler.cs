using FlaUIServer.Models;
using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record GetElementRectangleCommand(Guid SessionId, Guid ElementId) : IRequest<RectangleResponse>;

public class GetElementRectangleCommandHandler(ISessionManager sessionManager) : IRequestHandler<GetElementRectangleCommand, RectangleResponse>
{
    public async Task<RectangleResponse> Handle(GetElementRectangleCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var session = sessionManager.GetSession(request.SessionId);
        return await Task.Run(() => session.GetElementRectangle(request.ElementId), cancellationToken);
    }
}