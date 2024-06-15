using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace FlaUIServer.Models;

public class ExecuteScriptRequest
{
    public string Script { get; set; }

    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Serialization")]
    public JsonElement[] Args { get; set; } = [];
}