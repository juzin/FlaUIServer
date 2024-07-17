using System.Text;
using FlaUIServer.Models;
using Microsoft.Extensions.Options;

namespace FlaUIServer.Middlewares;

public class BasicAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly  BasicAuthenticationConfiguration _basicAuthentication;

    public BasicAuthorizationMiddleware(RequestDelegate next, IOptions<BasicAuthenticationConfiguration> basicAuthentication)
    {
        ArgumentNullException.ThrowIfNull(basicAuthentication);
        _next = next;
        _basicAuthentication = basicAuthentication.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.Request.Headers.TryGetValue("Authorization", out var value))
        {
            await WriteUnauthorizedResponse(context);
            return;
        }

        var authorizationHeader = value.ToString();
        if (string.IsNullOrEmpty(authorizationHeader))
        {
            await WriteUnauthorizedResponse(context);
            return;
        }

        if (!authorizationHeader.StartsWith("basic ", StringComparison.OrdinalIgnoreCase))
        {
            await WriteUnauthorizedResponse(context);
            return;
        }

        var token = authorizationHeader.Substring(6);
        var credentialAsString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
        
        var credentials = credentialAsString.Split(":");
        if (credentials.Length != 2)
        {
            await WriteUnauthorizedResponse(context);
            return;
        }

        var username = credentials[0];
        var password = credentials[1];

        if (username != _basicAuthentication.Username || password != _basicAuthentication.Password)
        {
            await WriteUnauthorizedResponse(context);
            return;
        }

        await _next(context);
    }

    private static async Task WriteUnauthorizedResponse(HttpContext context)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new ResponseBase<ErrorResponse>(new ErrorResponse("Unauthorized", "Authorization error", null)));
    }
}