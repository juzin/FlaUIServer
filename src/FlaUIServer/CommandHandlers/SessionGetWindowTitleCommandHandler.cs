﻿using FlaUIServer.Session;
using MediatR;

namespace FlaUIServer.CommandHandlers;

public record SessionGetWindowTitleCommand(Guid SessionId) : IRequest<string>;

public class SessionGetWindowTitleCommandHandler(ISessionManager sessionManager) : IRequestHandler<SessionGetWindowTitleCommand, string>
{
    public async Task<string> Handle(SessionGetWindowTitleCommand request, CancellationToken cancellationToken)
    {
        var session = sessionManager.GetSession(request.SessionId);
        return await Task.Run(() => session.GetWindowTitle(), cancellationToken);
    }
}