using FlaUI.Core.WindowsAPI;

namespace FlaUIServer.Helpers;

public static class KeyboardHelper
{
    public static VirtualKeyShort? GetModifierKey(char key)
    {
        switch (key)
        {
            // Control
            case '\uE009':
                return VirtualKeyShort.CONTROL;
            // Shift
            case '\uE008':
                return VirtualKeyShort.SHIFT;
            // Alt
            case '\uE00A':
                return VirtualKeyShort.ALT;
            // Meta (WinKey)
            case '\uE03D':
                return VirtualKeyShort.LWIN;
        }
        return null;
    }
}