using FlaUIServer.Models;

namespace FlaUIServer.Helpers;

public static class ResponseHelper
{
    public static IResult Ok<T>(T value)
    {
        return Results.Ok(new ResponseBase<T>(value));
    }

    public static IResult NotFound(ErrorResponse errorResponse)
    {
        return Results.NotFound(new ResponseBase<ErrorResponse>(errorResponse));
    }

    public static IResult BadRequest(ErrorResponse errorResponse)
    {
        return Results.BadRequest(new ResponseBase<ErrorResponse>(errorResponse));
    }

    public static IResult InternalServerError(ErrorResponse errorResponse)
    {
        return ResultsHelper.InternalServerError(new ResponseBase<ErrorResponse>(errorResponse));
    }
}