using System;
namespace Application.Database.CustomExceptions;

public class ReaderMaxLenghtException : Exception
{
    public ReaderMaxLenghtException()
    {
    }

    public ReaderMaxLenghtException(string message) : base(message)
    {
    }

    public ReaderMaxLenghtException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
