using Application.Database.CustomExceptions;
using Application.Database.Readers;
using System.Text.RegularExpressions;

namespace Application.Database.Books;

public class BookValidator
{
    public static void Validate(string author, string title)
    {
        if (string.IsNullOrWhiteSpace(author))
        {
            throw new AuthorNotFoundException();
        }
        else if (string.IsNullOrWhiteSpace(title))
        {
            throw new InvalidTitleException();
        }
        else if (author.Length > ReaderModel.NameMaxLength)
        {
            throw new AuthorMaxLenghtException();
        }
        else if (!IsValidName(author))
        {
            throw new InvalidAuthorException();
        }
    }
    static bool IsValidName(string name) => Regex.IsMatch(name, @"^[a-zA-Z -]+$", RegexOptions.IgnoreCase);
}
