using FlaUI.Core.AutomationElements;

namespace FlaUIServer.Extensions;

public static class WindowExtension
{
    /// <summary>
    /// Get runtime id as string
    /// </summary>
    /// <param name="window">Window</param>
    /// <returns>Runtime id as string</returns>
    public static string GetRuntimeId(this Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        
        if (window.Properties.RuntimeId.TryGetValue(out var runtimeId))
        {
            return string.Join(string.Empty, runtimeId);
        }

        return null;
    }
}