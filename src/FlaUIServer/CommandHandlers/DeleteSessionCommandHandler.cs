using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record DeleteSessionCommand(Guid SessionId) : IRequest;

public class DeleteSessionCommandHandler(ISessionManager sessionManager) : IRequestHandler<DeleteSessionCommand>
{
    public async Task Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        await Task.Run(() => sessionManager.DeleteSession(request.SessionId), cancellationToken);
    }
}