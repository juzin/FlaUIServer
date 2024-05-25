using System.Text.Json;

namespace FlaUIServer.Models;

public class ExecuteScriptRequest
{
    public string Script { get; set; }
    public JsonElement[] Args { get; set; } = [];
}