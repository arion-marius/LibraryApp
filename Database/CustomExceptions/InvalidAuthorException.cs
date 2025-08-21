using System;
using System.Runtime.Serialization;

namespace Application.Database.CustomExceptions;

public class InvalidAuthorException : Exception
{
    public InvalidAuthorException()
    {
    }

    public InvalidAuthorException(string message) : base(message)
    {
    }

    public InvalidAuthorException(string message, Exception innerException) : base(message, innerException)
    {
    }

}
