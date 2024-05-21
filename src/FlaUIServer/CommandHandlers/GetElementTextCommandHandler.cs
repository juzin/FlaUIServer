using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record GetElementTextCommand(Guid SessionId, Guid ElementId) : IRequest<string>;

public class GetElementTextCommandHandler(ISessionManager sessionManager) : IRequestHandler<GetElementTextCommand, string>
{
         public async Task<string> Handle(GetElementTextCommand request, CancellationToken cancellationToken)
         {
             var session = sessionManager.GetSession(request.SessionId);
             return await Task.Run(() => session.GetElementText(request.ElementId), cancellationToken);
         }
}