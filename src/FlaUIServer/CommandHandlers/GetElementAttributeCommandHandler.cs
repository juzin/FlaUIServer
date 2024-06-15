using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record GetElementAttributeCommand(Guid SessionId, Guid ElementId, string Name) : IRequest<string>;

public class GetElementAttributeCommandHandler(ISessionManager sessionManager)
    : IRequestHandler<GetElementAttributeCommand, string>
{
    public async Task<string> Handle(GetElementAttributeCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var session = sessionManager.GetSession(request.SessionId);
        return await Task.Run(() => session.GetElementAttribute(request.ElementId, request.Name), cancellationToken);
    }
}