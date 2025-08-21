using Application.Database.CustomExceptions;
using System.Text.RegularExpressions;

namespace Application.Database.Readers;

public class ReaderValidator
{
    public static void TryValidate(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ReaderNotFoundException();
        }
        else if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
        {
            throw new InvalidEmailException();
        }
        else if (email.Length > ReaderModel.EmailMaxLength)
        {
            throw new EmailMaxLengthException();
        }
        else if (name.Length > ReaderModel.NameMaxLength)
        {
            throw new ReaderMaxLenghtException();
        }
        else if (!IsValidName(name))
        {
            throw new InvalidReaderException();
        }
        bool IsValidEmail(string email) => Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);
        bool IsValidName(string name) => Regex.IsMatch(name, @"^[a-zA-Z -]+$", RegexOptions.IgnoreCase);
    }

}
