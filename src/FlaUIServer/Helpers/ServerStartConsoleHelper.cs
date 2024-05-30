using System.Text;
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

        var urls = new StringBuilder();
        var swaggerUrls = new StringBuilder();

        foreach (var url in options.Urls)
        {
            urls.Append($"{url}/wd/hub,");
            swaggerUrls.Append($"{url}/swagger/index.html,");
        }
        
        logger.LogInformation("Starting FlaUI Server, listening at url: {Urls}, options: {{ UseSwagger: {UseSwagger}, AllowPowershell: {AllowPowershell}, LogResponseBody: {LogResponseBody} }}", urls, options.UseSwagger, options.UseSwagger, options.LogResponseBody);
        if (options.UseSwagger)
        {
            logger.LogInformation("Swagger available at: {SwaggerUrls}", swaggerUrls);
        }
    }
}