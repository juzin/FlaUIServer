using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record IsElementSelectedCommand(Guid SessionId, Guid ElementId) : IRequest<bool>;

public class IsElementSelectedCommandHandler(ISessionManager sessionManager) : IRequestHandler<IsElementSelectedCommand, bool>
{
    public async Task<bool> Handle(IsElementSelectedCommand request, CancellationToken cancellationToken)
    {
        var session = sessionManager.GetSession(request.SessionId);
       return await Task.Run(() => session.IsElementSelected(request.ElementId), cancellationToken);
    }
}