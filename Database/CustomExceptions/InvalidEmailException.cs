using System;

namespace Application.Database.CustomExceptions;

public class InvalidEmailException : Exception
{
    public InvalidEmailException()
    {
    }

    public InvalidEmailException(string message) : base(message)
    {
    }

    public InvalidEmailException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
