using System.Text.Json.Serialization;

namespace FlaUIServer.Models;

public record ErrorResponse(string Error, string Message,[property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string StackTrace);