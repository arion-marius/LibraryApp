﻿using System;
namespace Application.Database.CustomExceptions;

public class BookNotFoundException : Exception
{
    public BookNotFoundException()
    {
    }

    public BookNotFoundException(string message) : base(message)
    {
    }

    public BookNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
