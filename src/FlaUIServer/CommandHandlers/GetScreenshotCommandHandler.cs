using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record GetScreenshotCommand(Guid SessionId) : IRequest<string>;

public class GetScreenshotCommandHandler(ISessionManager sessionManager) : IRequestHandler<GetScreenshotCommand, string>
{
    public async Task<string> Handle(GetScreenshotCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var session = sessionManager.GetSession(request.SessionId);
        return await Task.Run(() => session.GetScreenshot(), cancellationToken);
    }
}