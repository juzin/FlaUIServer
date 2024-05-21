namespace FlaUIServer.Helpers;

public static class ResultsHelper
{
    public static IResult InternalServerError<TValue>(TValue error) => new InternalServerErrorStatus<TValue>(error);
}

internal sealed class InternalServerErrorStatus<TValue>(TValue value) : IResult, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<TValue>
{
    object IValueHttpResult.Value => Value;
    public TValue Value { get; } = value;
    public int StatusCode => StatusCodes.Status500InternalServerError;

    int? IStatusCodeHttpResult.StatusCode => StatusCode;
    
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        
        httpContext.Response.StatusCode = StatusCode;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(Value);
    }
}