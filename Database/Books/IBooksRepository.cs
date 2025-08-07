using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Database.Books;

public interface IBooksRepository
{
    Task AddBookAsync(BookModel book);
    Task BorrowAsync(int bookId, int readerId);
    Task<(bool success, string message)> DeleteBookAsync(int id);
    Task<BookModel> GetBookByIdAsync(int id);
    Task<List<BookModel>> GetBooksAsync();
    Task UpdateBookAsync(BookModel book);
}