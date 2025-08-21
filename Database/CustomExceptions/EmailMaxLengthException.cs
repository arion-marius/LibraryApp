using System;
namespace Application.Database.CustomExceptions;

public class EmailMaxLengthException : Exception
{
    public EmailMaxLengthException()
    {
    }

    public EmailMaxLengthException(string message) : base(message)
    {
    }

    public EmailMaxLengthException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
