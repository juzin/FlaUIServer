namespace FlaUIServer.Models.Gestures;

public record ClipboardGestureRequest(string ContentType, string Base64Content);