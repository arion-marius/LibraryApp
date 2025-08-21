using System;
using System.Runtime.Serialization;

namespace Application.Database.CustomExceptions;

public class InvalidTitleException : Exception
{
    public InvalidTitleException()
    {
    }

    public InvalidTitleException(string message) : base(message)
    {
    }

    public InvalidTitleException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
