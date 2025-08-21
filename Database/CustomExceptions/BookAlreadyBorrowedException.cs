using System;
namespace Application.Database.CustomExceptions;

public class BookAlreadyBorrowedException : Exception
{
    public BookAlreadyBorrowedException()
    {
    }

    public BookAlreadyBorrowedException(string message) : base(message)
    {
    }

    public BookAlreadyBorrowedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
