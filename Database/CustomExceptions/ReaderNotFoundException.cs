using System;

namespace Application.Database.CustomExceptions;

public class ReaderNotFoundException : Exception
{
    public ReaderNotFoundException()
    {
    }

    public ReaderNotFoundException(string message) : base(message)
    {
    }

    public ReaderNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
