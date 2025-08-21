using System;
using System.Runtime.Serialization;

namespace Application.Database.CustomExceptions;

public class AuthorNotFoundException : Exception
{
    public AuthorNotFoundException()
    {
    }

    public AuthorNotFoundException(string message) : base(message)
    {
    }

    public AuthorNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
