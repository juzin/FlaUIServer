using System.Diagnostics.CodeAnalysis;

namespace FlaUIServer.Models;

public class KeyInputRequest
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Serialization")]
    public char[] Value { get; set; } 
}