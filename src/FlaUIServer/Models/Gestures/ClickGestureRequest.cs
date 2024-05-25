using System.Text.Json.Serialization;
using FlaUI.Core.Input;
using NotSupportedException = FlaUI.Core.Exceptions.NotSupportedException;

namespace FlaUIServer.Models.Gestures;

public class ClickGestureRequest
{
    /// <summary>
    /// X coordinate
    /// </summary>
    public int X { get; set; }
    
    /// <summary>
    /// Y coordinate
    /// </summary>
    public int Y { get; set; }
    
    /// <summary>
    /// Button
    /// </summary>
    public string Button { get; set; } = "left";
    
    /// <summary>
    /// Click count
    /// </summary>
    public int Times { get; set; } = 1;
    
    /// <summary>
    /// Delay between clicks
    /// </summary>
    public int InterClickDelayMs { get; set; } = 10;

    /// <summary>
    /// Parse to MouseButton enum
    /// </summary>
    /// <returns></returns>
    [JsonIgnore]
    public MouseButton MouseButton {
        get
        {
            return Button switch
            {
                "left" => MouseButton.Left,
                "right" => MouseButton.Right,
                "middle" => MouseButton.Middle,
                "back" => MouseButton.XButton1,
                "forward" => MouseButton.XButton2,
                _ => throw new NotSupportedException(
                    $"Mouse button '{Button}' is not suppoerted. Supported values 'left', 'right', 'middle', 'back', 'forward'")
            };
        }
    }
}