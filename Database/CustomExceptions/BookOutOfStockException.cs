using System;
using System.Runtime.Serialization;

namespace Application.Database.CustomExceptions;

public class BookOutOfStockException : Exception
{
    public BookOutOfStockException()
    {
    }

    public BookOutOfStockException(string message) : base(message)
    {
    }
    
    public BookOutOfStockException(string message, Exception innerException) : base(message, innerException)
    {
    }

}
