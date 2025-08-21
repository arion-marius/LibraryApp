using System;

namespace Application.Database.CustomExceptions;

public class UsedEmailException : Exception
{
    public UsedEmailException()
    {
    }

    public UsedEmailException(string message) : base(message)
    {
    }

    public UsedEmailException(string message, Exception innerException) : base(message, innerException)
    {
    }

}
