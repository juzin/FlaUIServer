using FlaUI.Core.WindowsAPI;

namespace FlaUIServer.Helpers;

public static class KeyboardHelper
{
    /// <summary>
    /// Map modifier key from unicode to enum
    /// </summary>
    /// <param name="key">Unicode key</param>
    /// <returns>VirtualKeyShort enum or null</returns>
    public static VirtualKeyShort? GetModifierKey(char key)
    {
        return key switch
        {
            // Control
            '\uE009' => VirtualKeyShort.CONTROL,
            // Shift
            '\uE008' => VirtualKeyShort.SHIFT,
            // Alt
            '\uE00A' => VirtualKeyShort.ALT,
            // Meta (WinKey)
            '\uE03D' => VirtualKeyShort.LWIN,
            '\uE012' => VirtualKeyShort.LEFT,
            '\uE013' => VirtualKeyShort.UP,
            '\uE014' => VirtualKeyShort.RIGHT,
            '\uE015' => VirtualKeyShort.DOWN,
            // Backspace
            '\uE003' => VirtualKeyShort.BACK,
            '\uE001' => VirtualKeyShort.CANCEL,
            '\uE005' => VirtualKeyShort.CLEAR,
            '\uE017' => VirtualKeyShort.DELETE,
            '\uE00C' => VirtualKeyShort.ESCAPE,
            '\uE010' => VirtualKeyShort.END,
            '\uE007' => VirtualKeyShort.ENTER,
            // NumpadEquals
            '\uE019' => VirtualKeyShort.SEPARATOR,
            '\uE002' => VirtualKeyShort.HELP,
            '\uE011' => VirtualKeyShort.HOME,
            '\uE016' => VirtualKeyShort.INSERT,
            '\uE00B' => VirtualKeyShort.PAUSE,
            // PageUp
            '\uE00E' => VirtualKeyShort.PRIOR,
            // PageDown
            '\uE00F' => VirtualKeyShort.NEXT,
            // Semicolon
            '\uE018' => VirtualKeyShort.OEM_1,
            '\uE00D' => VirtualKeyShort.SPACE,
            '\uE004' => VirtualKeyShort.TAB,
            '\uE01A' => VirtualKeyShort.NUMPAD0,
            '\uE01B' => VirtualKeyShort.NUMPAD1,
            '\uE01C' => VirtualKeyShort.NUMPAD2,
            '\uE01D' => VirtualKeyShort.NUMPAD3,
            '\uE01E' => VirtualKeyShort.NUMPAD4,
            '\uE01F' => VirtualKeyShort.NUMPAD5,
            '\uE020' => VirtualKeyShort.NUMPAD6,
            '\uE021' => VirtualKeyShort.NUMPAD7,
            '\uE022' => VirtualKeyShort.NUMPAD8,
            '\uE023' => VirtualKeyShort.NUMPAD9,
            '\uE024' => VirtualKeyShort.MULTIPLY,
            '\uE025' => VirtualKeyShort.ADD,
            '\uE026' => VirtualKeyShort.SEPARATOR,
            '\uE027' => VirtualKeyShort.SUBTRACT,
            '\uE028' => VirtualKeyShort.DECIMAL,
            '\uE029' => VirtualKeyShort.DIVIDE,
            '\uE031' => VirtualKeyShort.F1,
            '\uE032' => VirtualKeyShort.F2,
            '\uE033' => VirtualKeyShort.F3,
            '\uE034' => VirtualKeyShort.F4,
            '\uE035' => VirtualKeyShort.F5,
            '\uE036' => VirtualKeyShort.F6,
            '\uE037' => VirtualKeyShort.F7,
            '\uE038' => VirtualKeyShort.F8,
            '\uE039' => VirtualKeyShort.F9,
            '\uE03A' => VirtualKeyShort.F10,
            '\uE03B' => VirtualKeyShort.F11,
            '\uE03C' => VirtualKeyShort.F12,
            _ => null
        };
    }
}