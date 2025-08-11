using Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace Application.Database.Books;

public interface IBooksRepository
{
    Task AddBookAsync(BookModel book);
    Task BorrowAsync(int bookId, int readerId);
    Task<(bool success, string message)> DeleteBookAsync(int id);
    Task<BookModel> GetBookByIdAsync(int id);
    Task<List<BookModel>> GetBooksAsync(string search, int pageNumber = 1, int pageSize = 5);
    PagedList<BookDto> GetPagedBooks(string search, int pageNumber = 1, int pageSize = 5);
    Task UpdateBookAsync(BookModel book);
}