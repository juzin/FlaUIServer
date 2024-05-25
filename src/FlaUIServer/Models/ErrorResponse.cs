using System.Text.Json.Serialization;

namespace FlaUIServer.Models;

public record ErrorResponse(string Error, string Message,[property: JsonIgnore] string StackTrace);