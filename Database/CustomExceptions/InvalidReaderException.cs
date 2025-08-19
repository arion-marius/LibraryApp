using System;

namespace Application.Database.CustomExceptions;

public class InvalidReaderException : Exception
{
    public InvalidReaderException()
    {
    }

    public InvalidReaderException(string message) : base(message)
    {
    }

    public InvalidReaderException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
