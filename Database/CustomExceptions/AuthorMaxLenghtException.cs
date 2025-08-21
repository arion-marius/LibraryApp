using System;
namespace Application.Database.CustomExceptions;

public class AuthorMaxLenghtException : Exception
{
    public AuthorMaxLenghtException()
    {
    }

    public AuthorMaxLenghtException(string message) : base(message)
    {
    }

    public AuthorMaxLenghtException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
