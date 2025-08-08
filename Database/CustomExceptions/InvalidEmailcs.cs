using System;
using System.Runtime.Serialization;

namespace Application.Database.CustomExceptions;

public class InvalidEmailcs : Exception
{
    public InvalidEmailcs()
    {
    }

    public InvalidEmailcs(string message) : base(message)
    {
    }

    public InvalidEmailcs(string message, Exception innerException) : base(message, innerException)
    {
    }
}
