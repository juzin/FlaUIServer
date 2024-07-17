using FlaUIServer.CommandHandlers;
using FlaUIServer.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlaUIServer.Extensions;

public static class RouteGroupBuilderExtension
{
    /// <summary>
    /// Map Session endpoints
    /// </summary>
    /// <param name="routeBuilder">Route builder</param>
    /// <returns>Group builder</returns>
    public static RouteGroupBuilder MapSessionEndpoints(this RouteGroupBuilder routeBuilder)
    {
        ArgumentNullException.ThrowIfNull(routeBuilder);

        routeBuilder.MapGet("/status", 
                async () => await Task.FromResult(new StatusResponse()))
            .WithName("Get server status")
            .Produces<StatusResponse>()
            .Produces<ResponseBase<ErrorResponse>>(401);

        routeBuilder.MapPost("/session",
                async ([FromBody] CreateSessionRequest request, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new CreateSessionCommand(request), ct))
            .WithName("Create session")
            .Produces<ResponseBase<SessionResponse>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapDelete("/session/{sessionId}",
                async ([FromRoute] Guid sessionId, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new DeleteSessionCommand(sessionId), ct))
            .WithName("Close session")
            .Produces(200)
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapGet("/session/{sessionId}/source",
                async ([FromRoute] Guid sessionId, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new SessionGetSourceCommand(sessionId), ct))
            .WithName("Get app source")
            .Produces<ResponseBase<string>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapPost("/session/{sessionId}/window",
                async ([FromRoute] Guid sessionId, [FromBody] SessionSwitchToWindowRequest window, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new SessionWindowSwitchToCommand(sessionId, window), ct))
            .WithName("Switch to window")
            .Produces(200)
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapPost("/session/{sessionId}/execute",
                async ([FromRoute] Guid sessionId, [FromBody] ExecuteScriptRequest request, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new SessionExecuteScriptCommand(sessionId, request), ct))
            .WithName("Execute script")
            .Produces<ResponseBase<string>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapPost("/session/{sessionId}/keys",
                async ([FromRoute] Guid sessionId, [FromBody] KeyInputRequest request, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new SessionKeyboardTypeCommand(sessionId, request), ct))
            .WithName("Keyboard action")
            .Produces(200)
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapPost("/session/{sessionId}/element",
            async ([FromRoute] Guid sessionId, [FromBody] FindElementRequest searchOptions, IMediator mediator, CancellationToken ct) 
                => await mediator.SendAndCreateResponse(new FindElementCommand(sessionId, searchOptions), ct))
            .WithName("Find element")
            .Produces<ResponseBase<FindElementResponse>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapPost("/session/{sessionId}/element/{elementId}/element",
                async ([FromRoute] Guid sessionId,[FromRoute] Guid elementId, [FromBody] FindElementRequest searchOptions, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new ElementFindElementCommand(sessionId, elementId, searchOptions), ct))
            .WithName("Find element in parent element")
            .Produces<ResponseBase<FindElementResponse>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapPost("/session/{sessionId}/element/{elementId}/elements",
                async ([FromRoute] Guid sessionId, [FromRoute] Guid elementId, [FromBody] FindElementRequest searchOptions, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new ElementFindElementsCommand(sessionId, elementId, searchOptions), ct))
            .WithName("Find elements in parent element")
            .Produces<ResponseBase<Guid[]>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapPost("/session/{sessionId}/elements",
                async ([FromRoute] Guid sessionId, [FromBody] FindElementRequest searchOptions, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new FindElementsCommand(sessionId, searchOptions), ct))
            .WithName("Find elements")
            .Produces<ResponseBase<Guid[]>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapPost("/session/{sessionId}/element/{elementId}/click",
                async ([FromRoute] Guid sessionId, [FromRoute] Guid elementId, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new ElementClickCommand(sessionId, elementId), ct))
            .WithName("Element click")
            .Produces(200)
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapPost("/session/{sessionId}/element/{elementId}/value",
                async ([FromRoute] Guid sessionId, [FromRoute] Guid elementId, [FromBody] KeyInputRequest text, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new ElementFillTextCommand(sessionId, elementId, text), ct))
            .WithName("Element fill text")
            .Produces(200)
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapPost("/session/{sessionId}/element/{elementId}/clear",
                async ([FromRoute] Guid sessionId, [FromRoute] Guid elementId, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new ElementClearTextCommand(sessionId, elementId), ct))
            .WithName("Element clear text")
            .Produces(200)
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapGet("/session/{sessionId}/title",
                async ([FromRoute] Guid sessionId, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new SessionGetWindowTitleCommand(sessionId), ct))
            .WithName("Get window title")
            .Produces<ResponseBase<string>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapGet("/session/{sessionId}/window/rect",
                async ([FromRoute] Guid sessionId, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new SessionWindowGetRectangleCommand(sessionId), ct))
            .WithName("Get window rectangle")
            .Produces<ResponseBase<RectangleResponse>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapGet("/session/{sessionId}/window_handle",
                async ([FromRoute] Guid sessionId, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new SessionGetWindowHandleCommand(sessionId), ct))
            .WithName("Get active window handle")
            .Produces<ResponseBase<string>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapGet("/session/{sessionId}/window_handles",
                async ([FromRoute] Guid sessionId, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new SessionGetWindowHandlesCommand(sessionId), ct))
            .WithName("Get all window handles")
            .Produces<ResponseBase<string[]>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapGet("/session/{sessionId}/element/{elementId}/text",
                async ([FromRoute] Guid sessionId, [FromRoute] Guid elementId, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new GetElementTextCommand(sessionId, elementId), ct))
            .WithName("Element get text")
            .Produces<ResponseBase<string>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapGet("/session/{sessionId}/element/{elementId}/displayed",
                async ([FromRoute] Guid sessionId, [FromRoute] Guid elementId, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new IsElementDisplayedCommand(sessionId, elementId), ct))
            .WithName("Element is displayed")
            .Produces<ResponseBase<bool>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapGet("/session/{sessionId}/element/{elementId}/enabled",
                async ([FromRoute] Guid sessionId, [FromRoute] Guid elementId, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new IsElementEnabledCommand(sessionId, elementId), ct))
            .WithName("Element is enabled")
            .Produces<ResponseBase<bool>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapGet("/session/{sessionId}/element/{elementId}/selected",
                async ([FromRoute] Guid sessionId, [FromRoute] Guid elementId, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new IsElementSelectedCommand(sessionId, elementId), ct))
            .WithName("Element is selected")
            .Produces<ResponseBase<bool>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapGet("/session/{sessionId}/element/{elementId}/rect",
                async ([FromRoute] Guid sessionId, [FromRoute] Guid elementId, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new GetElementRectangleCommand(sessionId, elementId), ct))
            .WithName("Element get rectangle")
            .Produces<ResponseBase<RectangleResponse>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapGet("/session/{sessionId}/element/{elementId}/attribute/{name}",
                async ([FromRoute] Guid sessionId, [FromRoute] Guid elementId, [FromRoute] string name, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new GetElementAttributeCommand(sessionId, elementId, name), ct))
            .WithName("Element get attribute value")
            .Produces<ResponseBase<string>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapGet("/session/{sessionId}/screenshot",
                async ([FromRoute] Guid sessionId, IMediator mediator, CancellationToken ct) 
                    => await mediator.SendAndCreateResponse(new GetScreenshotCommand(sessionId), ct))
            .WithName("Take screenshot")
            .Produces<ResponseBase<string>>()
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400)
            .Produces<ResponseBase<ErrorResponse>>(404);

        routeBuilder.MapDelete("/session/{sessionId}/window",
                async ([FromRoute] Guid sessionId, IMediator mediator, CancellationToken ct)
                    => await mediator.SendAndCreateResponse(new SessionWindowCloseCommand(sessionId), ct))
            .WithName("Close active window")
            .Produces(200)
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<ErrorResponse>>(400);

        return routeBuilder;
    }

    /// <summary>
    /// Map server info endpoints
    /// </summary>
    /// <param name="routeBuilder">Group builder</param>
    /// <returns>Group builder</returns>
    public static RouteGroupBuilder MapServerInfoEndpoint(this RouteGroupBuilder routeBuilder)
    {
        routeBuilder.MapGet("/sessions",
                async (IMediator mediator, CancellationToken ct)
                    => await mediator.SendAndCreateResponse(new GetSessionsInfoCommand(), ct))
            .WithName("Get existing sessions information")
            .Produces<ResponseBase<ErrorResponse>>(401)
            .Produces<ResponseBase<SessionInfo[]>>();

        return routeBuilder;
    }
}