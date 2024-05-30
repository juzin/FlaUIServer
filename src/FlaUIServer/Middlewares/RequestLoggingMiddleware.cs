using System.Diagnostics;

namespace FlaUIServer.Middlewares;

/// <summary>
/// Request details logging middleware
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var requestBody = await GetRequestBody(context);
        var requestMethod = context.Request.Method;
        var requestPath = context.Request.Path;

        var start = Stopwatch.GetTimestamp();
        
        try
        {
            var originalResponseBody = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            await _next(context);
            var responseContent = await GetResponseBody(context, responseBody, originalResponseBody);
            LogRequestCompletion(requestMethod, requestPath, requestBody, context.Response.StatusCode, GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()), responseContent, null);
        }
        catch (Exception ex)
            // Never caught, because `LogRequestCompletion()` returns false. This ensures e.g. the developer exception page is still
            when (LogRequestCompletion(requestMethod, requestPath, requestBody, 500, GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()), null, ex))
        {
        }
    }

    private bool LogRequestCompletion(string method, string path, string requestBody, int statusCode, double elapsed,  string responseBody, Exception exception)
    {
        if (!string.IsNullOrEmpty(requestBody))
        {
            requestBody = $" {requestBody}";
        }
        
        responseBody = !string.IsNullOrEmpty(responseBody) ? $"{Environment.NewLine}----> {responseBody}" : "";

        var errorMessage = "";
        if (exception is not null)
        {
            errorMessage = $"{Environment.NewLine}{exception}";
        }
        
        _logger.LogInformation("HTTP {Method} {Path}{RequestBody} response {StatusCode} in {Elapsed} ms{ResponseBody}{Exception}", method, path, requestBody, statusCode, elapsed, responseBody, errorMessage);
        return false;
    }
    
    private async Task<string> GetResponseBody(HttpContext context, MemoryStream responseBody, Stream originalResponseBody)
    {
        responseBody.Position = 0;
        var responseContent = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Position = 0;
        await responseBody.CopyToAsync(originalResponseBody);
        context.Response.Body = originalResponseBody;

        return RemoveLongContent(responseContent);
    }

    private async Task<string> GetRequestBody(HttpContext context)
    {
        context.Request.EnableBuffering();
        var requestReader = new StreamReader(context.Request.Body);
        var requestContent = await requestReader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        
        return RemoveLongContent(requestContent);
    }
    
    private static double GetElapsedMilliseconds(long start, long stop)
    {
        return (stop - start) * 1000 / (double)Stopwatch.Frequency;
    }

    private static string RemoveLongContent(string content)
    {
        return content.Length > 4096 ? "<content is too long>" : content;
    }
}