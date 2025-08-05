using Application.Database.ReaderBooks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Database.Readers;

public interface IReadersRepository
{
    Task<List<ReaderModel>> GetReadersFromDbAsync();
    Task<ReaderModel?> GetReaderByIdAsync(int id);
    Task UpdateReaderAsync(ReaderModel reader);
    Task<ReaderModel?> GetReaderWithBooksByIdAsync(int id);
    Task<bool> HasReachedBorrowLimitAsync(int readerId);
    Task<(bool success, string message)> DeleteReaderAsync(int id);
    Task AddReaderBookAsync(int readerId, int bookId);
}
