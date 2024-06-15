namespace FlaUIServer.Exceptions;

public class FailedToExecutePowershellException : Exception
{
    public FailedToExecutePowershellException()
    {
    }

    public FailedToExecutePowershellException(string message) 
        : base(message)
    {
    }

    public FailedToExecutePowershellException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}