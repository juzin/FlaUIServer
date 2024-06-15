using System.Diagnostics.CodeAnalysis;
using FlaUIServer.Exceptions;
using FlaUIServer.Helpers;
using FlaUIServer.Models;
using MediatR;

namespace FlaUIServer.Extensions;

[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Create 500 response for unknown errors")]
public static class MediatrExtension
{
    /// <summary>
    /// Send command and handle exception
    /// </summary>
    /// <param name="mediator">Mediator</param>
    /// <param name="request">Request command</param>
    /// <param name="ct">Cancellation token</param>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <returns>Result</returns>
    public static async Task<IResult> SendAndCreateResponse<TResponse>(this IMediator mediator, IRequest<TResponse> request, CancellationToken ct = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(mediator);
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

    /// <summary>
    /// Send command and handle exception
    /// </summary>
    /// <param name="mediator">Mediator</param>
    /// <param name="request">Request command</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Result</returns>
    public static async Task<IResult> SendAndCreateResponse(this IMediator mediator, IRequest request, CancellationToken ct = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(mediator);
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