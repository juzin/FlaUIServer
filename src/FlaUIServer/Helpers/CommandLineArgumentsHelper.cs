using FlaUIServer.Models;

namespace FlaUIServer.Helpers;

public static class CommandLineArgumentsHelper
{
    public static ServerOptions ParseArguments(string[] args)
    {
        var options = new ServerOptions();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_URLS") is not null)
        {

        }
        
        var envUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(' ');

        if (envUrls is not null)
        {
            foreach (var url in envUrls)
            {
                options.Urls.Add(url);
            }
        }

        if (args is null) return options;
        
        foreach (var argument in args)
        {
            if (argument.StartsWith("--urls="))
            {
                var urls = argument.Split('=');
                if (urls.Length > 1)
                {
                    foreach (var url in urls[1].Split(' '))
                    {
                        options.Urls.Add(url);
                    }
                }
            }
            else if (argument.StartsWith("--cleanup-cycle="))
            {
                var timeout = argument.Split('=');

                if (timeout.Length > 1)
                {
                    options.SessionCleanupCycle = ushort.Parse(timeout[1]);
                }
            }
            else switch (argument)
            {
                case "--use-swagger":
                    options.UseSwagger = true;
                    break;
                case "--allow-powershell":
                    options.AllowPowershell = true;
                    break;
                case "--log-response-body":
                    options.LogResponseBody = true;
                    break;
            }
        }

        return options;
    }
}