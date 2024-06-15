using System.Reflection;
using FlaUIServer.Models;

namespace FlaUIServer.Helpers;

public static class ServerStartConsoleHelper
{
    public static void WriteLogo()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(" _____ _       _   _ ___   ____                           \n|  ___| | __ _| | | |_ _| / ___|  ___ _ ____   _____ _ __ \n| |_  | |/ _` | | | || |  \\___ \\ / _ \\ '__\\ \\ / / _ \\ '__|\n|  _| | | (_| | |_| || |   ___) |  __/ |   \\ V /  __/ |   \n|_|   |_|\\__,_|\\___/|___| |____/ \\___|_|    \\_/ \\___|_|   ");
        Console.WriteLine();
    }

    public static void LogServerOptions(ILogger logger, ServerOptions options)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(options);

        var urls = new List<string>();
        var swaggerUrls = new List<string>();

        foreach (var url in options.Urls)
        {
            urls.Add($"{url}/wd/hub");
            swaggerUrls.Add($"{url}/swagger/index.html");
        }

        logger.LogInformation("Starting FlaUI Server v{Version}, listening at url: {Urls}", Assembly.GetExecutingAssembly().GetName().Version?.ToString(), string.Join(", ", urls));
        logger.LogInformation("Server options: {{ UseSwagger: {UseSwagger}, AllowPowershell: {AllowPowershell}, LogResponseBody: {LogResponseBody}, SessionCleanupCycle: {SessionCleanupCycle} }}", options.UseSwagger, options.AllowPowershell, options.LogResponseBody, options.SessionCleanupCycleSeconds);
        if (options.UseSwagger)
        {
            logger.LogInformation("Swagger available at: {SwaggerUrls}", string.Join(", ", swaggerUrls));
        }
    }
}