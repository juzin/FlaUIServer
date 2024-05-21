using FlaUIServer.Exceptions;
using FlaUIServer.Helpers;
using FlaUIServer.Models;
using MediatR;

namespace FlaUIServer.Extensions;

public static class MediatrExtension
{
    public static async Task<IResult> SendAndCreateResponse<TResponse>(this IMediator mediator, IRequest<TResponse> request, CancellationToken ct = default)
    {
        try
        {
            return ResponseHelper.Ok(await mediator.Send(request, ct));
        }
        catch (ObjectNotFoundException nfe)
        {
            return ResponseHelper.NotFound(new ErrorResponse("Not Found", nfe.Message, null));
        }
        catch (RequestValidationException ve)
        {
            return ResponseHelper.BadRequest(new ErrorResponse("Validation Error", ve.Message, null));
        }
        catch (Exception e)
        {
            return ResponseHelper.InternalServerError(new ErrorResponse("Unknown Error", e.Message, e.StackTrace));
        }
    }
    
    public static async Task<IResult> SendAndCreateResponse(this IMediator mediator, IRequest request, CancellationToken ct = default)
    {
        try
        {
            await mediator.Send(request, ct);
            return Results.Ok();
        }
        catch (ObjectNotFoundException nfe)
        {
            return ResponseHelper.NotFound(new ErrorResponse("Not Found", nfe.Message, null));
        }
        catch (RequestValidationException ve)
        {
            return ResponseHelper.BadRequest(new ErrorResponse("Validation Error", ve.Message, null));
        }
        catch (Exception e)
        {
            return ResponseHelper.InternalServerError(new ErrorResponse("Unknown Error", e.Message, e.StackTrace));
        }
    }
}