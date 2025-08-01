using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Database.Books;

public interface IBooksRepository
{
    Task AddBookAsync(BookModel book);
    Task<(bool success, string message)> DeleteBookAsync(int id);
    Task<BookModel> GetBookByIdAsync(int id);
    Task<List<BookModel>> GetBooksFromDbAsync();
    // Task AddBookAsync(BookModel book);
    Task UpdateBookAsync(BookModel book);
}