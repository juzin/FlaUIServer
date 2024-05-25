namespace FlaUIServer.Models.Gestures;

public record ScrollGestureRequest(int X, int Y, int? DeltaX, int? DeltaY);