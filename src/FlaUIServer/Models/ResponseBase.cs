namespace FlaUIServer.Models;

public class ResponseBase<T>(T value)
{
    public T Value { get; } = value;
}