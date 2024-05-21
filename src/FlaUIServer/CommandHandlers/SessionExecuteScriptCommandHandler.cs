using FlaUIServer.Models;
using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record SessionExecuteScriptCommand(Guid SessionId, ExecuteScriptRequest Data) : IRequest<string>;

public class SessionExecuteScriptCommandHandler(ISessionManager sessionManager) : IRequestHandler<SessionExecuteScriptCommand, string>
{
    public async Task<string> Handle(SessionExecuteScriptCommand request, CancellationToken cancellationToken)
    {
        var session = sessionManager.GetSession(request.SessionId);
        return await Task.Run(() => session.ExecuteScript(request.Data), cancellationToken);
    }
}