namespace FlaUIServer.Models.Gestures;

public class MoveGestureRequest
{
    public int StartX { get; set; }
    
    public int StartY { get; set; }
    
    public int EndX { get; set; }
    
    public int EndY { get; set; }
    
    public int Duration { get; set; }
}