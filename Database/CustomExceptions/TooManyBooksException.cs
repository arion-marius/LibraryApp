using System;
namespace Application.Database.CustomExceptions;

public class TooManyBooksException : Exception
{
    public TooManyBooksException()
    {
    }

    public TooManyBooksException(string message) : base(message)
    {
    }

    public TooManyBooksException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
