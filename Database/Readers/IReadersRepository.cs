using Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Database.Readers;

public interface IReadersRepository
{
    Task<ReaderDto?> GetReaderByIdAsync(int id);
    Task UpdateReaderAsync(ReaderDto reader, ReaderDto email);
    Task<ReaderDto?> GetReaderWithBooksByIdAsync(int id);
    Task<(bool success, string message)> DeleteReaderAsync(int id);
    Task<List<ReaderSummaryDto>> GetPaginatedReadersFromDbAsync(string search);
    void Insert(string name, string email);
    Task<ReaderDto> RemoveReaderBook(int readerId, int bookId);
}
