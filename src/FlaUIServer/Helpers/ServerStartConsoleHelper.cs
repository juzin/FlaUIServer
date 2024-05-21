namespace FlaUIServer.Helpers;

public static class ServerStartConsoleHelper
{
    public static void WriteLogo()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(" _____ _       _   _ ___   ____                           \n|  ___| | __ _| | | |_ _| / ___|  ___ _ ____   _____ _ __ \n| |_  | |/ _` | | | || |  \\___ \\ / _ \\ '__\\ \\ / / _ \\ '__|\n|  _| | | (_| | |_| || |   ___) |  __/ |   \\ V /  __/ |   \n|_|   |_|\\__,_|\\___/|___| |____/ \\___|_|    \\_/ \\___|_|   ");
        Console.WriteLine();
    }
}