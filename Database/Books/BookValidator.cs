using Application.Database.CustomExceptions;
using Application.Database.Readers;
using System.Text.RegularExpressions;

namespace Application.Database.Books;

public partial class BookValidator
{
    public static void TryValidate(string author, string title)
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

    static bool IsValidName(string name) => NameRegex().IsMatch(name);

    [GeneratedRegex(@"^[a-zA-ZăâîșțĂÂÎȘȚ -]+$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex NameRegex();
}
