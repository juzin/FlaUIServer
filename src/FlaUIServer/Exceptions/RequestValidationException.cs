namespace FlaUIServer.Exceptions;

public class RequestValidationException : Exception
{
    public RequestValidationException(string message)
        : base(message)
    {
    }

    public RequestValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public RequestValidationException()
    {
    }
}